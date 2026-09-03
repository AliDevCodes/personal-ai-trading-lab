import { useCallback, useEffect, useRef, useState } from 'react'
import './App.css'
import { fetchMarketHistory } from './api/marketData'
import type { MarketHistory, MarketHistoryError, MarketHistoryResult } from './api/marketData'
import MarketChart from './components/MarketChart'

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

type LoadError = { status: number | null }

type QuoteResult = { ok: true; data: MarketDataDto } | { ok: false; error: LoadError }

function isAbortError(e: unknown): boolean {
  return e instanceof DOMException && e.name === 'AbortError'
}

function formatDecimal(v: number) {
  return new Intl.NumberFormat(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 8 }).format(v)
}

const priceFormatter = new Intl.NumberFormat(undefined, {
  minimumFractionDigits: 2,
  maximumFractionDigits: 2,
})

function formatPrice(v: number) {
  return priceFormatter.format(v)
}

function quoteErrorMessage(error: LoadError | null): string | null {
  if (!error) return null
  switch (error.status) {
    case 404:
      return 'Market not found.'
    case 502:
      return 'The market data provider returned an invalid response.'
    case 503:
      return 'The market data provider is temporarily unavailable.'
    case null:
      return 'Network error while fetching market data.'
    default:
      return `Failed to load market data (status ${error.status}).`
  }
}

function historyErrorMessage(error: MarketHistoryError | null): string | null {
  if (!error) return null
  switch (error.kind) {
    case 'bad-request':
      return 'Invalid request for historical data.'
    case 'not-found':
      return 'BTC/USDT is not supported.'
    case 'provider-invalid':
      return 'The market data provider returned an invalid response.'
    case 'provider-unavailable':
      return 'The market data provider is temporarily unavailable.'
    case 'network':
      return 'Network error while loading historical data.'
    case 'server':
      return 'Something went wrong while loading historical data.'
  }
}

export default function App() {
  const [quote, setQuote] = useState<MarketDataDto | null>(null)
  const [quoteLoading, setQuoteLoading] = useState(true)
  const [quoteError, setQuoteError] = useState<LoadError | null>(null)

  const [history, setHistory] = useState<MarketHistory | null>(null)
  const [historyLoading, setHistoryLoading] = useState(true)
  const [historyError, setHistoryError] = useState<MarketHistoryError | null>(null)

  // Track in-flight requests so refresh and unmount can abort cleanly
  // without leaving stale state updates behind.
  const controllersRef = useRef({
    quote: null as AbortController | null,
    history: null as AbortController | null,
  })

  // Fetch helpers only produce results — they never touch React state, so they
  // are safe to call from effects. State is applied in promise callbacks only.
  const fetchQuote = useCallback(async (signal: AbortSignal): Promise<QuoteResult> => {
    try {
      const res = await fetch('/api/market-data/BTCUSDT?timeframe=1h', { signal })
      if (!res.ok) return { ok: false, error: { status: res.status } }
      return { ok: true, data: (await res.json()) as MarketDataDto }
    } catch (e) {
      if (isAbortError(e)) throw e
      return { ok: false, error: { status: null } }
    }
  }, [])

  const applyQuoteResult = useCallback((result: QuoteResult) => {
    if (result.ok) {
      setQuote(result.data)
    } else {
      setQuote(null)
      setQuoteError(result.error)
    }
  }, [])

  const applyHistoryResult = useCallback((result: MarketHistoryResult) => {
    if (result.ok) {
      setHistory(result.data)
    } else {
      setHistory(null)
      setHistoryError(result.error)
    }
  }, [])

  // Initial load. All setState calls happen inside promise callbacks (never
  // synchronously in the effect body), and each request checks that it is
  // still the latest one before applying anything.
  useEffect(() => {
    const controllers = controllersRef.current
    const quoteAc = new AbortController()
    const historyAc = new AbortController()
    controllers.quote = quoteAc
    controllers.history = historyAc

    fetchQuote(quoteAc.signal)
      .then((result) => {
        if (controllersRef.current.quote !== quoteAc) return
        applyQuoteResult(result)
        setQuoteLoading(false)
      })
      .catch((e: unknown) => {
        if (!isAbortError(e) && controllersRef.current.quote === quoteAc) {
          applyQuoteResult({ ok: false, error: { status: null } })
          setQuoteLoading(false)
        }
      })

    fetchMarketHistory(historyAc.signal)
      .then((result) => {
        if (controllersRef.current.history !== historyAc) return
        applyHistoryResult(result)
        setHistoryLoading(false)
      })
      .catch((e: unknown) => {
        if (!isAbortError(e) && controllersRef.current.history === historyAc) {
          applyHistoryResult({ ok: false, error: { kind: 'network', status: null } })
          setHistoryLoading(false)
        }
      })

    return () => {
      quoteAc.abort()
      historyAc.abort()
    }
  }, [applyQuoteResult, applyHistoryResult, fetchQuote])

  const handleRefresh = useCallback(() => {
    const controllers = controllersRef.current
    controllers.quote?.abort()
    controllers.history?.abort()

    setQuoteLoading(true)
    setQuoteError(null)
    setHistoryLoading(true)
    setHistoryError(null)

    const quoteAc = new AbortController()
    const historyAc = new AbortController()
    controllers.quote = quoteAc
    controllers.history = historyAc

    fetchQuote(quoteAc.signal)
      .then((result) => {
        if (controllersRef.current.quote !== quoteAc) return
        applyQuoteResult(result)
        setQuoteLoading(false)
      })
      .catch((e: unknown) => {
        if (!isAbortError(e) && controllersRef.current.quote === quoteAc) {
          applyQuoteResult({ ok: false, error: { status: null } })
          setQuoteLoading(false)
        }
      })

    fetchMarketHistory(historyAc.signal)
      .then((result) => {
        if (controllersRef.current.history !== historyAc) return
        applyHistoryResult(result)
        setHistoryLoading(false)
      })
      .catch((e: unknown) => {
        if (!isAbortError(e) && controllersRef.current.history === historyAc) {
          applyHistoryResult({ ok: false, error: { kind: 'network', status: null } })
          setHistoryLoading(false)
        }
      })
  }, [applyQuoteResult, applyHistoryResult, fetchQuote])

  const quoteMessage = quoteErrorMessage(quoteError)
  const historyMessage = historyErrorMessage(historyError)
  const latestCandle = history?.candles[history.candles.length - 1]

  return (
    <main className="container">
      <h1>Trading Lab</h1>

      <section className="card" aria-labelledby="quote-heading">
        <header className="card-header">
          <h2 id="quote-heading" className="card-title">
            Market
          </h2>
          <div className="card-header-meta">BTC/USDT · 1H</div>
        </header>

        <div className="card-body">
          {quoteLoading && <p className="status">Loading market data…</p>}
          {quoteMessage && !quoteLoading && (
            <p className="error" role="alert">
              {quoteMessage}
            </p>
          )}

          {quote && !quoteLoading && (
            <div className="market-grid">
              <div className="primary">
                <div className="label">Current Price</div>
                <div className="value">
                  {formatDecimal(quote.currentPriceAmount)} {quote.currentPriceQuote}
                </div>
                <div className="timestamp">{new Date(quote.latestCandleIntervalStart).toLocaleString()}</div>
              </div>
              <div className="ohlc">
                <div><span className="label">Open</span><span className="val">{formatDecimal(quote.open)}</span></div>
                <div><span className="label">High</span><span className="val">{formatDecimal(quote.high)}</span></div>
                <div><span className="label">Low</span><span className="val">{formatDecimal(quote.low)}</span></div>
                <div><span className="label">Close</span><span className="val">{formatDecimal(quote.close)}</span></div>
                <div><span className="label">Volume</span><span className="val">{formatDecimal(quote.volume)}</span></div>
              </div>
            </div>
          )}
        </div>
      </section>

      <section className="card card--wide" aria-labelledby="history-heading">
        <header className="history-header">
          <div className="history-title">
            <span className="history-market" id="history-heading">BTC/USDT</span>
            <span className="history-timeframe">1H</span>
          </div>
          <div className="history-meta">
            {history && (
              <>
                <span>{history.candles.length} candles</span>
                {latestCandle && (
                  <span className="history-latest">
                    Latest close <strong>{formatPrice(latestCandle.close)} USDT</strong>
                  </span>
                )}
              </>
            )}
          </div>
        </header>

        <div className="card-body">
          {historyLoading && <p className="status">Loading historical data…</p>}

          {historyMessage && !historyLoading && (
            <div className="history-state" role="alert">
              <p className="error">{historyMessage}</p>
              <button type="button" className="retry" onClick={handleRefresh} aria-label="Retry loading historical data">
                Retry
              </button>
            </div>
          )}

          {history && !historyLoading && history.candles.length === 0 && (
            <p className="status">No historical data is available for BTC/USDT 1H yet.</p>
          )}

          {history && !historyLoading && history.candles.length > 0 && (
            <MarketChart candles={history.candles} market={history.market} timeframe={history.timeframe} />
          )}
        </div>

        <footer className="card-footer">
          <button type="button" onClick={handleRefresh} aria-label="Refresh market data">
            Refresh data
          </button>
        </footer>
      </section>
    </main>
  )
}