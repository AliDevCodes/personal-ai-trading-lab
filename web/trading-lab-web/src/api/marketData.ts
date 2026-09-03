// Typed client for the TradingLab historical market-data API (Phase 3.1 slice).
// This module owns HTTP + JSON concerns only; it contains no UI logic.

export type Candle = {
  intervalStart: string
  open: number
  high: number
  low: number
  close: number
  volume: number
}

export type MarketHistory = {
  market: string
  timeframe: string
  candles: Candle[]
}

export type MarketHistoryErrorKind =
  | 'bad-request'
  | 'not-found'
  | 'provider-invalid'
  | 'provider-unavailable'
  | 'server'
  | 'network'

export type MarketHistoryError = {
  kind: MarketHistoryErrorKind
  status: number | null
}

export type MarketHistoryResult =
  | { ok: true; data: MarketHistory }
  | { ok: false; error: MarketHistoryError }

const HISTORY_ENDPOINT = '/api/market-data/BTCUSDT/history'
const SUPPORTED_TIMEFRAME = '1h'

function kindForStatus(status: number): MarketHistoryErrorKind {
  switch (status) {
    case 400:
      return 'bad-request'
    case 404:
      return 'not-found'
    case 502:
      return 'provider-invalid'
    case 503:
      return 'provider-unavailable'
    default:
      return 'server'
  }
}

/**
 * Fetch historical candles for the single supported Phase 3.1 market (BTC/USDT 1H).
 *
 * - `limit` defaults to 100 (the API contract range is 1..1000).
 * - When `signal` is aborted, the AbortError is re-thrown so the caller can
 *   distinguish an intentional abort from a real failure.
 * - Non-OK HTTP responses are mapped to a typed error; network failures map
 *   to `kind: 'network'` with a null status.
 */
export async function fetchMarketHistory(
  signal?: AbortSignal,
  limit = 100,
): Promise<MarketHistoryResult> {
  const params = new URLSearchParams({ timeframe: SUPPORTED_TIMEFRAME, limit: String(limit) })
  try {
    const res = await fetch(`${HISTORY_ENDPOINT}?${params.toString()}`, { signal })
    if (!res.ok) {
      return { ok: false, error: { kind: kindForStatus(res.status), status: res.status } }
    }
    const data = (await res.json()) as MarketHistory
    return { ok: true, data }
  } catch (e) {
    if (e instanceof DOMException && e.name === 'AbortError') {
      throw e
    }
    return { ok: false, error: { kind: 'network', status: null } }
  }
}