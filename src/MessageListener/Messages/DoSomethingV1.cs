using MessageListenerBase.Messages;

namespace MessageListener.Messages
{
    public class DoSomethingV1 : Message
    {
        public string What { get; set; }
    }
}