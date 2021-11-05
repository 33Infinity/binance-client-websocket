namespace WebsocketClient {
    public class Program {
        private static void Main(string[] args) {
            List<Exchange.Exchange> _exchanges = new List<Exchange.Exchange>();
            var binance = new Exchange.Binance();
            _exchanges.Add(binance);
            foreach (var exchange in _exchanges) {
                exchange.SubscribeToStreams();
            }
        }
    }
}