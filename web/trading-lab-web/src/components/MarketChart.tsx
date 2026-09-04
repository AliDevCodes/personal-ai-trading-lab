import { useCallback, useEffect, useMemo, useRef, useState } from 'react'
import {
  CandlestickSeries,
  ColorType,
  HistogramSeries,
  createChart,
} from 'lightweight-charts'
import type {
  ChartOptions,
  DeepPartial,
  IChartApi,
  ISeriesApi,
  MouseEventParams,
  UTCTimestamp,
} from 'lightweight-charts'
import type { Candle } from '../api/marketData'
import CandleDetails from './CandleDetails'

type Theme = 'light' | 'dark'

// Palette values mirror the design tokens in src/index.css
// (--bg, --text, --border). Keep these two in sync when either changes.
const PALETTES: Record<Theme, { background: string; text: string; grid: string; border: string }> = {
  light: { background: '#ffffff', text: '#6b6375', grid: '#e5e4e7', border: '#e5e4e7' },
  dark: { background: '#16171d', text: '#9ca3af', grid: '#2e303a', border: '#2e303a' },
}

// Up/down colors chosen to stay readable on both light and dark backgrounds.
const UP_COLOR = '#26a69a'
const DOWN_COLOR = '#ef5350'
const VOLUME_UP = 'rgba(38, 166, 154, 0.35)'
const VOLUME_DOWN = 'rgba(239, 83, 80, 0.35)'

const priceFormatter = new Intl.NumberFormat(undefined, {
  minimumFractionDigits: 2,
  maximumFractionDigits: 2,
})

function toUtcSeconds(iso: string): UTCTimestamp {
  return (Date.parse(iso) / 1000) as UTCTimestamp
}

function toTimeKey(iso: string): number {
  return Math.floor(Date.parse(iso) / 1000)
}

/**
 * Resolve the candle time from a chart mouse event. Prefers the candle series
 * data (authoritative OHLCV row for the hovered/tapped bar) and falls back to
 * the event's own `time`. Returns null when there is no meaningful bar.
 */
function resolveTime(
  param: MouseEventParams,
  series: ISeriesApi<'Candlestick'> | null,
): number | null {
  if (!series) return null
  const bar = param.seriesData.get(series)
  if (bar) {
    if (typeof bar.time === 'number') return bar.time
    if (typeof bar.time === 'string') return Math.floor(Date.parse(bar.time) / 1000)
  }
  if (param.time !== undefined) {
    if (typeof param.time === 'number') return param.time
    if (typeof param.time === 'string') return Math.floor(Date.parse(param.time) / 1000)
  }
  return null
}

function chartOptions(theme: Theme): DeepPartial<ChartOptions> {
  const palette = PALETTES[theme]
  return {
    autoSize: true,
    layout: {
      background: { type: ColorType.Solid, color: palette.background },
      textColor: palette.text,
      fontSize: 12,
    },
    grid: {
      vertLines: { color: palette.grid },
      horzLines: { color: palette.grid },
    },
    rightPriceScale: { borderColor: palette.border },
    timeScale: { borderColor: palette.border, timeVisible: true, secondsVisible: false },
    localization: { priceFormatter: (price: number) => priceFormatter.format(price) },
  }
}

type MarketChartProps = {
  candles: Candle[]
  market: string
  timeframe: string
}

export default function MarketChart({ candles, market, timeframe }: MarketChartProps) {
  const containerRef = useRef<HTMLDivElement | null>(null)
  const chartRef = useRef<IChartApi | null>(null)
  const candleSeriesRef = useRef<ISeriesApi<'Candlestick'> | null>(null)
  const volumeSeriesRef = useRef<ISeriesApi<'Histogram'> | null>(null)

  // The inspected candle is tracked by its unix-second time. The displayed
  // candle is always derived from the CURRENT candles array, so a stale time
  // (e.g. after a refresh) resolves to null and the placeholder returns.
  const [inspectedTime, setInspectedTime] = useState<number | null>(null)

  // When a new candle dataset arrives (refresh / new data), the inspection is
  // reset — even if the refreshed window still contains the same timestamp.
  // Adjusting state during render is the documented React pattern for
  // resetting state when a prop changes (no effect, no timing workaround).
  const [prevCandles, setPrevCandles] = useState<Candle[]>(candles)
  if (prevCandles !== candles) {
    setPrevCandles(candles)
    setInspectedTime(null)
  }

  // O(1) time -> Candle lookup. Rebuilt whenever the API candles change;
  // mirrored into a ref so the (stable) event handler can use it.
  const candlesByTime = useMemo(() => {
    const map = new Map<number, Candle>()
    for (const c of candles) map.set(toTimeKey(c.intervalStart), c)
    return map
  }, [candles])
  const candlesByTimeRef = useRef(candlesByTime)

  // While a dataset replacement is in flight, Lightweight Charts re-emits the
  // crosshair for the bar the pointer is still parked on, which would
  // re-inspect and defeat the refresh reset. Library crosshair events are
  // suppressed until real pointer input proves the user is interacting again.
  const suppressCrosshairRef = useRef(false)

  const inspectedCandle = inspectedTime === null ? null : (candlesByTime.get(inspectedTime) ?? null)

  // Shared by crosshair movement (desktop hover, touch drag) and click/tap.
  // Leaves the chart (point === undefined) or whitespace clears the inspection.
  const handleInspect = useCallback((param: MouseEventParams) => {
    // Ignore the synthetic crosshair event the library re-emits right after a
    // data replacement while the pointer is still parked (see
    // suppressCrosshairRef). Any genuine pointer input deactivates it, so
    // subsequent events inspect normally.
    if (suppressCrosshairRef.current) return

    if (param.point === undefined) {
      setInspectedTime(null)
      return
    }
    const time = resolveTime(param, candleSeriesRef.current)
    if (time === null || !candlesByTimeRef.current.has(time)) {
      setInspectedTime(null)
      return
    }
    // No state churn when the inspected candle has not changed.
    setInspectedTime((prev) => (prev === time ? prev : time))
  }, [])

  const [theme, setTheme] = useState<Theme>(() =>
    window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light',
  )
  const themeRef = useRef<Theme>(theme)

  // Follow the OS color scheme so the chart always matches the app theme.
  useEffect(() => {
    const mq = window.matchMedia('(prefers-color-scheme: dark)')
    const onChange = (event: MediaQueryListEvent) => setTheme(event.matches ? 'dark' : 'light')
    mq.addEventListener('change', onChange)
    return () => mq.removeEventListener('change', onChange)
  }, [])

  // Create the chart exactly once and subscribe to inspection events. Data and
  // theme are applied through separate effects, so we never leak chart
  // instances or duplicate subscriptions on prop changes.
  useEffect(() => {
    const container = containerRef.current
    if (!container) return

    // Any genuine pointer input means the user is interacting again, so the
    // synthetic-event suppression is lifted. Capture phase runs before the
    // chart's own listeners, so the very first move is never swallowed.
    const onPointerInput = () => {
      suppressCrosshairRef.current = false
    }
    container.addEventListener('pointermove', onPointerInput, true)
    container.addEventListener('pointerdown', onPointerInput, true)
    container.addEventListener('pointerleave', onPointerInput, true)

    const chart = createChart(container, chartOptions(themeRef.current))

    const candleSeries = chart.addSeries(CandlestickSeries, {
      upColor: UP_COLOR,
      downColor: DOWN_COLOR,
      borderUpColor: UP_COLOR,
      borderDownColor: DOWN_COLOR,
      wickUpColor: UP_COLOR,
      wickDownColor: DOWN_COLOR,
    })

    const volumeSeries = chart.addSeries(HistogramSeries, {
      priceScaleId: 'volume',
      priceFormat: { type: 'volume' },
      lastValueVisible: false,
      priceLineVisible: false,
    })
    // Reserve the bottom ~18% of the chart for the volume pane.
    chart.priceScale('volume').applyOptions({ scaleMargins: { top: 0.82, bottom: 0 } })

    chart.subscribeCrosshairMove(handleInspect)
    chart.subscribeClick(handleInspect)

    chartRef.current = chart
    candleSeriesRef.current = candleSeries
    volumeSeriesRef.current = volumeSeries

    return () => {
      container.removeEventListener('pointermove', onPointerInput, true)
      container.removeEventListener('pointerdown', onPointerInput, true)
      container.removeEventListener('pointerleave', onPointerInput, true)
      chart.unsubscribeCrosshairMove(handleInspect)
      chart.unsubscribeClick(handleInspect)
      chart.remove()
      chartRef.current = null
      candleSeriesRef.current = null
      volumeSeriesRef.current = null
    }
  }, [handleInspect])

  // Apply theme colors on mount and whenever the OS scheme changes.
  useEffect(() => {
    themeRef.current = theme
    chartRef.current?.applyOptions(chartOptions(theme))
  }, [theme])

  // Push candle data into the existing series. setData replaces the contents
  // in place, preserving the API's chronological order (oldest -> newest).
  useEffect(() => {
    const chart = chartRef.current
    const candleSeries = candleSeriesRef.current
    const volumeSeries = volumeSeriesRef.current
    if (!chart || !candleSeries || !volumeSeries) return

    candlesByTimeRef.current = candlesByTime
    // Data is about to be replaced: suppress the library's crosshair
    // re-emission (see handleInspect) until real pointer input arrives.
    suppressCrosshairRef.current = true

    if (candles.length === 0) {
      candleSeries.setData([])
      volumeSeries.setData([])
      return
    }

    candleSeries.setData(
      candles.map((c) => ({
        time: toUtcSeconds(c.intervalStart),
        open: c.open,
        high: c.high,
        low: c.low,
        close: c.close,
      })),
    )
    volumeSeries.setData(
      candles.map((c) => ({
        time: toUtcSeconds(c.intervalStart),
        value: c.volume,
        color: c.close >= c.open ? VOLUME_UP : VOLUME_DOWN,
      })),
    )
    chart.timeScale().fitContent()
  }, [candles, candlesByTime])

  // Truthful, human-readable description used as the chart's accessible name.
  // Lightweight Charts does not animate by default, so no additional
  // prefers-reduced-motion handling is needed for this slice.
  const accessibleLabel = useMemo(() => {
    const label = `${market} ${timeframe.toUpperCase()} candlestick chart with volume`
    if (candles.length === 0) return `${label} — no data`
    const last = candles[candles.length - 1]
    return `${label}, ${candles.length} candles, latest close ${priceFormatter.format(last.close)} USDT`
  }, [candles, market, timeframe])

  return (
    <div className="chart">
      <div className="chart-canvas" role="img" aria-label={accessibleLabel} ref={containerRef} />
      <CandleDetails candle={inspectedCandle} />
      <p className="chart-attribution">
        <a href="https://www.tradingview.com" target="_blank" rel="noreferrer">
          Charts by TradingView
        </a>
      </p>
    </div>
  )
}