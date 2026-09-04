import type { Candle } from '../api/marketData'

const priceFormatter = new Intl.NumberFormat(undefined, {
  minimumFractionDigits: 2,
  maximumFractionDigits: 2,
})

const volumeFormatter = new Intl.NumberFormat(undefined, {
  minimumFractionDigits: 2,
  maximumFractionDigits: 8,
})

type CandleDetailsProps = {
  candle: Candle | null
}

/**
 * Pure presentational panel showing the candle currently under the chart
 * crosshair (or the result of a tap on touch devices).
 *
 * - Renders as real DOM text, completely outside the chart's role="img"
 *   container, so it stays in the accessibility tree.
 * - No aria-live: the panel intentionally does not announce every crosshair
 *   movement (that would flood screen readers).
 * - Direction is conveyed as text (Up/Down), never by color alone.
 */
export default function CandleDetails({ candle }: CandleDetailsProps) {
  return (
    <section className="candle-details" aria-label="Inspected candle">
      <h3 className="candle-details-title">Inspected candle</h3>

      {candle === null ? (
        <p className="candle-details-placeholder">Hover or tap the chart to inspect a candle</p>
      ) : (
        <>
          <dl className="candle-details-grid">
            <div>
              <dt>Time</dt>
              <dd>{new Date(candle.intervalStart).toLocaleString()}</dd>
            </div>
            <div>
              <dt>Open</dt>
              <dd>{priceFormatter.format(candle.open)}</dd>
            </div>
            <div>
              <dt>High</dt>
              <dd>{priceFormatter.format(candle.high)}</dd>
            </div>
            <div>
              <dt>Low</dt>
              <dd>{priceFormatter.format(candle.low)}</dd>
            </div>
            <div>
              <dt>Close</dt>
              <dd>{priceFormatter.format(candle.close)}</dd>
            </div>
            <div>
              <dt>Volume</dt>
              <dd>{volumeFormatter.format(candle.volume)}</dd>
            </div>
          </dl>
          <p className="candle-details-direction">
            {candle.close >= candle.open ? 'Up' : 'Down'}
          </p>
        </>
      )}
    </section>
  )
}