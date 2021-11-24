using OSProject.Models;
using SampleCOMObjectNameSpace;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Threading;

namespace OSProject.IPC
{
    public class FileMapping
    {
        private static DateTime startTime = DateTime.MinValue;
        private static DateTime endTime = DateTime.MinValue;
        private static MemoryMappedFile mmf;

        private static void Listener(object showMessages)
        {
            using (MemoryMappedViewAccessor accessor = mmf.CreateViewAccessor())
            {
                byte[] bytes = new byte[1000];
                var temp = accessor.ReadArray(0, bytes, 0, bytes.Length);
                string text = Encoding.UTF8.GetString(bytes).Trim('\0');

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
            using (MemoryMappedViewAccessor accessor = mmf.CreateViewAccessor())
            {
                byte[] array = Encoding.UTF8.GetBytes(Constants.MESSAGE);
                accessor.WriteArray<byte>(0, array, 0, array.Length);
            }
        }

        public static void Run(bool showMessages = true)
        {
            using (mmf = MemoryMappedFile.CreateOrOpen("file-mapping", 10000))
            {
                var thread1 = new Thread(new ParameterizedThreadStart(Listener));
                var thread2 = new Thread(new ParameterizedThreadStart(Sender));

                thread2.Start(showMessages);
                thread2.Join();
                thread1.Start(showMessages);
                thread1.Join(); 
            }
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

            mmf.Dispose();

            return new ExecutionStats
            {
                NumberOfRuns = n,
                AverageTimeTaken = timeTakenArr.Average()
            };
        }
    }
}