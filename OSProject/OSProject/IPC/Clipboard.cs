using OSProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OSProject.IPC
{
    public class Clipboard
    {
        private static readonly TextCopy.Clipboard clipboard = new TextCopy.Clipboard();
        private static DateTime startTime = DateTime.MinValue;
        private static DateTime endTime = DateTime.MinValue;

        private static async void Listener(object showMessages)
        {
            string text = await clipboard.GetTextAsync();
            endTime = DateTime.Now;
            if ((bool)showMessages)
            {
                Console.WriteLine($"Received message : ");
                Console.WriteLine("------------------------------------------------------------");
                Console.WriteLine();
                Console.WriteLine(text);
                Console.WriteLine();
                Console.WriteLine("------------------------------------------------------------");
            }
        }
        private static void Sender(object showMessages)
        {
            if ((bool)showMessages)
            {
                Console.WriteLine($"Sending message : ");
                Console.WriteLine("------------------------------------------------------------");
                Console.WriteLine();
                Console.WriteLine(Constants.MESSAGE);
                Console.WriteLine();
                Console.WriteLine("------------------------------------------------------------");
            }
            startTime = DateTime.Now;
            clipboard.SetText(Constants.MESSAGE);
        }

        public static void Run(bool showMessages = true)
        {
            var thread1 = new Thread(new ParameterizedThreadStart(Listener));
            var thread2 = new Thread(new ParameterizedThreadStart(Sender));

            thread2.Start(showMessages);
            thread2.Join();
            thread1.Start(showMessages);
            thread1.Join();
        }

        public static ExecutionStats GetStats(int n = 100, bool suppressOutput = false)
        {
            if (suppressOutput)
            {
                Console.WriteLine("Getting Statistics of the run ...");
                Console.WriteLine(); 
            }

            List<double> timeTakenArr = new List<double>();

            for (int i = 0; i < n; i++)
            {
                Run(false);

                timeTakenArr.Add((endTime - startTime).TotalMilliseconds);
            }

            return new ExecutionStats
            {
                NumberOfRuns = n,
                AverageTimeTaken = timeTakenArr.Average()
            };
        }
    }
}
