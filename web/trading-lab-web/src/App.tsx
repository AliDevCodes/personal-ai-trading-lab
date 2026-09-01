import { useEffect, useState, useCallback } from 'react'
import './App.css'

type MarketDataDto = {
  market: string
  timeframe: string
  currentPriceAmount: number
  currentPriceQuote: string
  latestCandleIntervalStart: string
  open: number
  high: number
  low: number
  close: number
  volume: number
}

function formatDecimal(v: number) {
  return new Intl.NumberFormat(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 8 }).format(v)
}

export default function App() {
  const [data, setData] = useState<MarketDataDto | null>(null)
  const [loading, setLoading] = useState<boolean>(false)
  const [error, setError] = useState<string | null>(null)

  const fetchData = useCallback(async (signal?: AbortSignal) => {
    setLoading(true)
    setError(null)
    try {
      const res = await fetch('/api/market-data/BTCUSDT?timeframe=1h', { signal })
      if (!res.ok) {
        setError(`Failed to load market data (status ${res.status})`)
        setData(null)
        return
      }
      const json = (await res.json()) as MarketDataDto
      setData(json)
    } catch (e) {
      if ((e as DOMException).name === 'AbortError') {
        // aborted, ignore
        return
      }
      setError('Network error while fetching market data')
      setData(null)
    } finally {
      setLoading(false)
    }
  }, [])

  useEffect(() => {
    const ac = new AbortController()
    void (async () => {
      await fetchData(ac.signal)
    })()
    return () => ac.abort()
  }, [fetchData])

  return (
    <main className="container">
      <h1>Trading Lab</h1>
      <div className="card">
        <div className="card-header">
          <div>
            <strong>Market:</strong> BTC/USDT
          </div>
          <div>
            <strong>Timeframe:</strong> 1H
          </div>
        </div>

        <div className="card-body">
          {loading && <div className="status">Loading market data…</div>}
          {error && <div className="error">{error}</div>}

          {data && !loading && !error && (
            <div className="market-grid">
              <div className="primary">
                <div className="label">Current Price</div>
                <div className="value">
                  {formatDecimal(data.currentPriceAmount)} {data.currentPriceQuote}
                </div>
                <div className="timestamp">{new Date(data.latestCandleIntervalStart).toLocaleString()}</div>
              </div>
              <div className="ohlc">
                <div><span className="label">Open</span><span className="val">{formatDecimal(data.open)}</span></div>
                <div><span className="label">High</span><span className="val">{formatDecimal(data.high)}</span></div>
                <div><span className="label">Low</span><span className="val">{formatDecimal(data.low)}</span></div>
                <div><span className="label">Close</span><span className="val">{formatDecimal(data.close)}</span></div>
                <div><span className="label">Volume</span><span className="val">{formatDecimal(data.volume)}</span></div>
              </div>
            </div>
          )}
        </div>

        <div className="card-footer">
          <button
            type="button"
            onClick={() => {
              const ac = new AbortController()
              fetchData(ac.signal)
            }}
            aria-label="Refresh market data"
          >
            Refresh
          </button>
        </div>
      </div>
    </main>
  )
}
