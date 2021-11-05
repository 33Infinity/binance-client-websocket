using System.Reactive.Subjects;
using Binance.Client.Websocket.Json;
using Newtonsoft.Json.Linq;

namespace Binance.Client.Websocket.Responses.Kline
{
    public class KlineResponse : ResponseBase<Kline>
    {
        internal static bool TryHandle(JObject response, ISubject<Kline> subject)
        {
            var stream = response?["stream"]?.Value<string>();
            if (stream == null)
            {
                return false;
            }

            if (!stream.Contains("kline"))
            {
                return false;
            }
            var parsed = BinanceJsonSerializer.Deserialize<Kline>(response["data"]["k"].ToString());
            subject.OnNext(parsed);

            return true;
        }
    }
}