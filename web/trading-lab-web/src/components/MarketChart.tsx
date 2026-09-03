import { useEffect, useMemo, useRef, useState } from 'react'
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
  UTCTimestamp,
} from 'lightweight-charts'
import type { Candle } from '../api/marketData'

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

  // Create the chart exactly once. Data and theme are applied through
  // separate effects below, so we never leak chart instances on prop changes.
  useEffect(() => {
    const container = containerRef.current
    if (!container) return

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

    chartRef.current = chart
    candleSeriesRef.current = candleSeries
    volumeSeriesRef.current = volumeSeries

    return () => {
      chart.remove()
      chartRef.current = null
      candleSeriesRef.current = null
      volumeSeriesRef.current = null
    }
  }, [])

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
  }, [candles])

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
      <p className="chart-attribution">
        <a href="https://www.tradingview.com" target="_blank" rel="noreferrer">
          Charts by TradingView
        </a>
      </p>
    </div>
  )
}