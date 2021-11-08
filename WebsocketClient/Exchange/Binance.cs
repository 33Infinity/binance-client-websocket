using System.Data.SqlClient;
using Binance.Client.Websocket.Client;
using Binance.Client.Websocket.Communicator;
using Binance.Client.Websocket.Subscriptions;
using Binance.Client.Websocket.Websockets;


namespace WebsocketClient.Exchange {
    public class Binance : Exchange {

        private static readonly ManualResetEvent ExitEvent = new ManualResetEvent(false);
        private static readonly Uri ApiWebsocketUrl = new Uri("wss://stream.binance.us:9443");

        public override string Name => "Binance";

        public override void SubscribeToStreams() {
            var url = ApiWebsocketUrl;
            using (var communicator = new BinanceWebsocketCommunicator(url)) {
                communicator.Name = "Binance-1";
                communicator.ReconnectTimeout = TimeSpan.FromMinutes(1);
                communicator.ReconnectionHappened.Subscribe(type =>
                    Console.WriteLine($"Reconnection happened, type: {type}"));



                using (var client = new BinanceWebsocketClient(communicator)) {
                    SubscribeToStreams(client, communicator);

                    client.SetSubscriptions(
                        new KlineSubscription("btcusd", "5m")
                    );


                    communicator.Start().Wait();
                    ExitEvent.WaitOne();
                }
            }
        }

        private void SubscribeToStreams(BinanceWebsocketClient client, IBinanceCommunicator comm) {
            client.Streams.KlineStream.Subscribe(response => {
                var ob = response;
                //Console.Write($"Kline [{ob.Symbol}] " +
                //                $"Kline start time: {ob.StartTime} " +
                //                $"Kline close time: {ob.CloseTime} " +
                //                $"Interval: {ob.Interval} " +
                //                $"First trade ID: {ob.FirstTradeId} " +
                //                $"Last trade ID: {ob.LastTradeId} " +
                //                $"Open price: {ob.OpenPrice} " +
                //                $"Close price: {ob.ClosePrice} " +
                //                $"High price: {ob.HighPrice} " +
                //                $"Low price: {ob.LowPrice} " +
                //                $"Base asset volume: {ob.BaseAssetVolume} " +
                //                $"Number of trades: {ob.NumberTrades} " +
                //                $"Is this kline closed?: {ob.IsClose} " +
                //                $"Quote asset volume: {ob.QuoteAssetVolume} " +
                //                $"Taker buy base: {ob.TakerBuyBaseAssetVolume} " +
                //                $"Taker buy quote: {ob.TakerBuyQuoteAssetVolume} " +
                //                $"Ignore: {ob.Ignore} ");
                Console.WriteLine(ob.StartTime);
                Console.WriteLine(ob.CloseTime);
                var candlestickBO = BusinessObjects.CandlestickBO.New(Name, UnixToUtc(response.StartTime), UnixToUtc(response.CloseTime), response.OpenPrice, response.ClosePrice, response.LowPrice, response.HighPrice, response.NumberTrades);
                candlestickBO.Save();
            });
        }

        private DateTime UnixToUtc(double aUnixTimeStamp) => DateTimeOffset.FromUnixTimeSeconds((long)aUnixTimeStamp).UtcDateTime;
    }
}
