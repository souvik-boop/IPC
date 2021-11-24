using OSProject.Models;
using OSProject.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OSProject.IPC
{
    public class MailSlot
    {
        private static DateTime startTime = DateTime.MinValue;
        private static DateTime endTime = DateTime.MinValue;

        private static void Listener(object data)
        {
            using (var server = new MailslotServer(((string[])data)[0]))
            {
                while (true)
                {
                    var msg = server.GetNextMessage();

                    if (msg != null)
                    {
                        string text = msg;

                        endTime = DateTime.Now;
                        if (bool.Parse(((string[])data)[1]))
                        {
                            Console.WriteLine($"Received message : ");
                            Console.WriteLine("------------------------------------------------------------");
                            Console.WriteLine();
                            Console.WriteLine(text);
                            Console.WriteLine();
                            Console.WriteLine("------------------------------------------------------------");
                        }

                        server.Dispose();
                        break;
                    }
                }
            }
        }
        private static void Sender(object data)
        {
            if (bool.Parse(((string[])data)[1]))
            {
                Console.WriteLine($"Sending message : ");
                Console.WriteLine("------------------------------------------------------------");
                Console.WriteLine();
                Console.WriteLine(Constants.MESSAGE);
                Console.WriteLine();
                Console.WriteLine("------------------------------------------------------------");
            }
            startTime = DateTime.Now;
            using (var client = new MailslotClient(((string[])data)[0]))
            {
                while (true)
                {
                    try
                    {
                        client.SendMessage(Constants.MESSAGE);
                    }
                    catch (Win32Exception)
                    {
                        continue;
                    }
                    break;
                }
            }
        }

        public static void Run(bool showMessages = true)
        {
            var thread1 = new Thread(new ParameterizedThreadStart(Listener));
            var thread2 = new Thread(new ParameterizedThreadStart(Sender));

            string[] parameter = new string[] { Guid.NewGuid().ToString(), showMessages.ToString() };
            thread1.Start(parameter);
            thread2.Start(parameter);
            thread2.Join();
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
