﻿using System.Reactive.Subjects;
using Binance.Client.Websocket.Json;
using Newtonsoft.Json.Linq;

namespace Binance.Client.Websocket.Responses.Kline
{
    public class KlineResponse : ResponseBase<Kline>
    {
        internal static bool TryHandle(JObject response, ISubject<KlineResponse> subject)
        {
            System.Console.Write("kline here");
            var stream = response?["stream"]?.Value<string>();
            if (stream == null)
            {
                return false;
            }

            if (!stream.Contains("kline"))
            {
                return false;
            }

            var parsed = response.ToObject<KlineResponse>(BinanceJsonSerializer.Serializer);
            subject.OnNext(parsed);

            return true;
        }
    }
}