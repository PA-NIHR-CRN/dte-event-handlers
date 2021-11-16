using Application.Messages;

namespace MessageListener.Messages
{
    public class SendNotificationV1 : Message
    {
        public string NotificationSubject { get; set; }        
    }
}