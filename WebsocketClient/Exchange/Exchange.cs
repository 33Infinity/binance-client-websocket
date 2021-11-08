namespace WebsocketClient.Exchange
{
    public abstract class Exchange{
        public abstract string Name { get; }    
        public abstract void SubscribeToStreams();
    }
}
