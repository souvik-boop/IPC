using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSMQSampleApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            MessageProcessor messageProcessor = new MessageProcessor();

            messageProcessor.SendMessage(args[0]);

            if (bool.Parse(args[1]))
            {
                Console.WriteLine($"Sending message : ");
                Console.WriteLine("------------------------------------------------------------");
                Console.WriteLine();
                Console.WriteLine(args[0]);
                Console.WriteLine();
                Console.WriteLine("------------------------------------------------------------");
            }
            var startTime = DateTime.Now;
        }
    }
}
