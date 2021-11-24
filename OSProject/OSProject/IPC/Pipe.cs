using OSProject.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OSProject.IPC
{
    public class Pipe
    {
        private static DateTime startTime = DateTime.MinValue;
        private static DateTime endTime = DateTime.MinValue;

        public static void Run(bool showMessages = true)
        {
            var thread1 = new Thread(new ParameterizedThreadStart(PipeServer));
            thread1.Start(new string[] { showMessages.ToString() });
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

        private static void PipeClient(object args)
        {
            if (args != null && ((string[])args).Length > 0)
            {
                string pipeHandleAsString = ((string[])args)[1].ToString();
                bool showMessages = bool.Parse(((string[])args)[0].ToString());

                using (PipeStream pipeClient =
                    new AnonymousPipeClientStream(PipeDirection.In, pipeHandleAsString))
                {
                    using (StreamReader sr = new StreamReader(pipeClient))
                    {
                        // Display the read text to the console
                        string temp;

                        // Read the server data and echo to the console.
                        while ((temp = sr.ReadLine()) != null)
                        {
                            if (showMessages)
                            {
                                Console.WriteLine($"Received message : ");
                                Console.WriteLine("------------------------------------------------------------");
                                Console.WriteLine();
                                Console.WriteLine(temp);
                                Console.WriteLine();
                                Console.WriteLine("------------------------------------------------------------");
                            }
                            endTime = DateTime.Now;
                        }
                    }
                }
            }

            try
            {
                Task.Delay(-1).Wait();
            }
            catch (Exception)
            {
            }
        }
        private static void PipeServer(object args)
        {
            bool showMessages = bool.Parse(((string[])args)[0].ToString());
            Thread thread1;
            using (AnonymousPipeServerStream pipeServer =
                new AnonymousPipeServerStream(PipeDirection.Out,
                HandleInheritability.Inheritable))
            {
                // Pass the client process a handle to the server.
                thread1 = new Thread(new ParameterizedThreadStart(PipeClient));
                thread1.Start(new string[] { ((string[])args)[0], pipeServer.GetClientHandleAsString() });

                try
                {
                    // Read user input and send that to the client process.
                    using (StreamWriter sw = new StreamWriter(pipeServer))
                    {
                        sw.AutoFlush = true;
                        // Send a 'sync message' and wait for client to receive it.
                        startTime = DateTime.Now;
                        sw.WriteLine(Constants.MESSAGE);
                        if (showMessages)
                        {
                            Console.WriteLine($"Sent message : ");
                            Console.WriteLine("------------------------------------------------------------");
                            Console.WriteLine();
                            Console.WriteLine(Constants.MESSAGE);
                            Console.WriteLine();
                            Console.WriteLine("------------------------------------------------------------");
                        }
                        pipeServer.WaitForPipeDrain();
                    }
                }
                // Catch the IOException that is raised if the pipe is broken
                // or disconnected.
                catch (IOException e)
                {
                    if (showMessages) Console.WriteLine("[SERVER] Error: {0}", e.Message);
                }
            }

            thread1.Interrupt();
        }
    }
}