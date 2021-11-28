using Binance.Client.Websocket.Client;
using Binance.Client.Websocket.Subscriptions;
using Binance.Client.Websocket.Websockets;
using BusinessObjects;


namespace WebsocketClient.Exchange {
    public class Binance : Exchange {

        private static readonly ManualResetEvent ExitEvent = new ManualResetEvent(false);
        private static readonly Uri ApiWebsocketUrl = new Uri("wss://stream.binance.us:9443");
        private const string INTERVAL = "1m";
        private const string EXCHANGE = "Binance";

        public override string Name => "Binance";

        public override void SubscribeToStreams() {
            var url = ApiWebsocketUrl;
            using (var communicator = new BinanceWebsocketCommunicator(url)) {
                communicator.Name = "Binance-1";
                communicator.ReconnectTimeout = TimeSpan.FromMinutes(1);
                communicator.ReconnectionHappened.Subscribe(type =>
                    Console.WriteLine($"Reconnection happened, type: {type}"));



                using (var client = new BinanceWebsocketClient(communicator)) {
                    SubscribeToStreams(client);
                    var subscriptions = new List<KlineSubscription>();
                    var tradingPairBOs = TradingPairBO.GetByExchange(EXCHANGE);
                    foreach (var pair in tradingPairBOs)
                        subscriptions.Add(new KlineSubscription(pair.Symbol, INTERVAL));
                    client.SetSubscriptions(
                        subscriptions.ToArray()
                    );


                    communicator.Start().Wait();
                    ExitEvent.WaitOne();
                }
            }
        }

        private void SubscribeToStreams(BinanceWebsocketClient client) {
            client.Streams.KlineStream.Subscribe(response => {
                var candlestickBO = CandlestickBO.New(Name, response.Symbol, UnixToUtc(response.StartTime), UnixToUtc(response.CloseTime), response.OpenPrice, response.ClosePrice, response.LowPrice, response.HighPrice, response.NumberTrades);
                candlestickBO.Save();
            });
        }

        private DateTime UnixToUtc(double aUnixTimeStamp) => DateTimeOffset.FromUnixTimeMilliseconds((long)aUnixTimeStamp).UtcDateTime;
    }
}
