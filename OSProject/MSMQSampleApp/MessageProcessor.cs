using System.Messaging;

namespace MSMQSampleApp
{
    public class MessageProcessor
    {
        private MessageQueue queue;
        private string queueName = @"msi\private$\sample";

        public MessageProcessor()
        {
            if (MessageQueue.Exists(queueName))
                queue = new MessageQueue(queueName);
            else
                queue = MessageQueue.Create(queueName, false);
        }

        public void SendMessage(string message)
        {
            queue.Send(message);
        }

        public string ReceiveMessage()
        {
            string msg = queue.Receive().Body.ToString();
            queue.Purge();
            return msg;
        }
    }
}