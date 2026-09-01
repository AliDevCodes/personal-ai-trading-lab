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
    public class NobitexHistoryHttpClientTests
    {
        [Fact]
        public async Task ProviderFormsUdfHistoryRequest_WithCountbackAndTo()
        {
            var requests = new List<Uri>();
            var handler = new FakeHandler((req) =>
            {
                requests.Add(req.RequestUri!);
                // return minimal valid UDF
                var json = "{ \"s\": \"ok\", \"t\": [1690000000], \"o\": [\"28000\"], \"h\": [\"29000\"], \"l\": [\"27900\"], \"c\": [\"28900\"], \"v\": [\"12.5\"] }";
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(json) };
            });

            using var client = new HttpClient(handler) { BaseAddress = new Uri("https://apiv2.nobitex.ir/") };
            var provider = new NobitexMarketDataProvider(client);

            var to = DateTimeOffset.FromUnixTimeSeconds(1690003600);
            var res = await provider.GetHistoryAsync(TradingLab.Domain.Market.Market.BtcUsdt, TradingLab.Domain.Market.Timeframe.OneHour, 50, to, CancellationToken.None);
            Assert.True(res.Success);
            // verify request formation
            Assert.Contains(requests, u => u.AbsolutePath.Contains("/market/udf/history") && u.Query.Contains("symbol=BTCUSDT") && u.Query.Contains("resolution=60") && u.Query.Contains("countback=50") && u.Query.Contains($"to={to.ToUnixTimeSeconds()}"));
        }

        private class FakeHandler : HttpMessageHandler
        {
            private readonly Func<HttpRequestMessage, HttpResponseMessage> _responder;
            public FakeHandler(Func<HttpRequestMessage, HttpResponseMessage> responder) { _responder = responder; }
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) => Task.FromResult(_responder(request));
        }
    }
}
