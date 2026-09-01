using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using TradingLab.Domain.Market;
using TradingLab.Infrastructure.MarketData.Nobitex;
using Xunit;

namespace TradingLab.IntegrationTests.Market
{
    public class NobitexHttpClientTests
    {
        [Fact]
        public async Task Provider_FormsExpectedRequests_And_ParsesResponses()
        {
            var requests = new List<Uri>();

            var handler = new FakeHandler((req) =>
            {
                requests.Add(req.RequestUri!);
                var path = req.RequestUri.AbsolutePath;
                var query = req.RequestUri.Query;
                if (path.Contains("/market/stats"))
                {
                    var json = "{ \"status\": \"ok\", \"stats\": { \"btc-usdt\": { \"latest\": \"28934.12\" } } }";
                    return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(json) };
                }
                if (path.Contains("/market/udf/history"))
                {
                    var json = "{ \"t\": [1690000000], \"o\": [\"28000\"], \"h\": [\"29000\"], \"l\": [\"27900\"], \"c\": [\"28900\"], \"v\": [\"12.5\"] }";
                    return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(json) };
                }

                return new HttpResponseMessage(HttpStatusCode.NotFound) { Content = new StringContent("{}") };
            });

            using var client = new HttpClient(handler) { BaseAddress = new Uri("https://api.nobitex.ir/") };
            var provider = new NobitexMarketDataProvider(client);
            var result = await provider.GetLatestAsync(TradingLab.Domain.Market.Market.BtcUsdt, Timeframe.OneHour, CancellationToken.None);

            Assert.True(result.Success);
            Assert.NotNull(result.CurrentPrice);
            Assert.NotNull(result.LatestCandle);

            // verify expected requests
            Assert.Contains(requests, u => u.PathAndQuery.Contains("/market/stats") && u.Query.Contains("srcCurrency=btc") && u.Query.Contains("dstCurrency=usdt"));
            Assert.Contains(requests, u => u.PathAndQuery.Contains("/market/udf/history") && u.Query.Contains("symbol=BTCUSDT") && u.Query.Contains("resolution=60"));
        }

        private class FakeHandler : HttpMessageHandler
        {
            private readonly Func<HttpRequestMessage, HttpResponseMessage> _responder;

            public FakeHandler(Func<HttpRequestMessage, HttpResponseMessage> responder)
            {
                _responder = responder;
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                var resp = _responder(request);
                return Task.FromResult(resp);
            }
        }
    }
}
