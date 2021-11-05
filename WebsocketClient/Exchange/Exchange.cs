using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebsocketClient.Exchange
{
    public abstract class Exchange
    {
        public abstract void SubscribeToStreams();
    }
}
