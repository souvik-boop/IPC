using OSProject.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace OSProject
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Enter your choice: ");
                ViewChoices<IPCChoices>();
                Console.WriteLine();
                var choice = Console.ReadLine();
                Console.WriteLine("------------------------------------------------------------");
                Console.WriteLine();
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Green;

                const int N = 5;
                switch ((IPCChoices)(int.Parse(choice) - 1))
                {
                    case IPCChoices.Clipboard:
                        IPC.Clipboard.Run();
                        PrintStats(IPC.Clipboard.GetStats(N));
                        break;
                    case IPCChoices.COM:
                        IPC.COM.Run();
                        PrintStats(IPC.COM.GetStats(N));
                        break;
                    case IPCChoices.DataCopy:
                        IPC.DataCopy.Run();
                        PrintStats(IPC.DataCopy.GetStats(N));
                        break;
                    case IPCChoices.FileMapping:
                        IPC.FileMapping.Run();
                        PrintStats(IPC.FileMapping.GetStats(N));
                        break;
                    case IPCChoices.MailSlots:
                        IPC.MailSlot.Run();
                        PrintStats(IPC.MailSlot.GetStats(N));
                        break;
                    case IPCChoices.Pipes:
                        IPC.Pipe.Run();
                        PrintStats(IPC.Pipe.GetStats(N));
                        break;
                    case IPCChoices.RPC:
                        IPC.RPC.Run();
                        PrintStats(IPC.RPC.GetStats(N));
                        break;
                    case IPCChoices.WindowsSockets:
                        IPC.Socket.Run();
                        PrintStats(IPC.Socket.GetStats(N));
                        break;
                    case IPCChoices.RunItAll:
                        Dictionary<IPCChoices, double> timeTakenArr = new Dictionary<IPCChoices, double>();

                        timeTakenArr.Add(IPCChoices.Clipboard, IPC.Clipboard.GetStats(N, false).AverageTimeTaken);
                        Task.Delay(2000).Wait();
                        timeTakenArr.Add(IPCChoices.COM, IPC.COM.GetStats(N, false).AverageTimeTaken);
                        Task.Delay(2000).Wait();
                        timeTakenArr.Add(IPCChoices.DataCopy, IPC.DataCopy.GetStats(N, false).AverageTimeTaken);
                        Task.Delay(2000).Wait();
                        timeTakenArr.Add(IPCChoices.FileMapping, IPC.FileMapping.GetStats(N, false).AverageTimeTaken);
                        Task.Delay(2000).Wait();
                        timeTakenArr.Add(IPCChoices.MailSlots, IPC.MailSlot.GetStats(N, false).AverageTimeTaken);
                        Task.Delay(2000).Wait();
                        timeTakenArr.Add(IPCChoices.Pipes, IPC.Pipe.GetStats(N, false).AverageTimeTaken);
                        Task.Delay(2000).Wait();
                        timeTakenArr.Add(IPCChoices.RPC, IPC.RPC.GetStats(N, false).AverageTimeTaken);
                        Task.Delay(2000).Wait();
                        timeTakenArr.Add(IPCChoices.WindowsSockets, IPC.Socket.GetStats(N, false).AverageTimeTaken);
                        Task.Delay(2000).Wait();

                        int padding = 40;
                        foreach (var item in timeTakenArr)
                            Console.WriteLine($"Time taken for IPC Method ({ DescriptionAttr(item.Key)}){new string(' ', padding - DescriptionAttr(item.Key).Length)}\t:\t{item.Value}ms");
                        break;
                    default:
                        break;
                }

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("------------------------------------------------------------");
                Console.WriteLine();
            }

#pragma warning disable CS0162 // Unreachable code detected
            Task.Delay(-1).Wait();
#pragma warning restore CS0162 // Unreachable code detected
        }

        #region PRIVATE METHODS
        private static void ViewChoices<TEnum>()
        {
            foreach (var (item, index) in WithIndex((TEnum[])Enum.GetValues(typeof(TEnum))))
            {
                Console.WriteLine($"{ (object)(index + 1)}. { (object)DescriptionAttr((TEnum)item)}");
            }
        }

        private static string DescriptionAttr<T>(T source)
        {
            FieldInfo fi = source.GetType().GetField(source.ToString());

            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0) return attributes[0].Description;
            else return source.ToString();
        }

        private static IEnumerable<(T item, int index)> WithIndex<T>(IEnumerable<T> self) => self.Select((item, index) => (item, index));

        private static void PrintStats(ExecutionStats stats)
        {
            Console.WriteLine("Number of runs : " + stats.NumberOfRuns);
            Console.WriteLine("Average Time Taken : " + stats.AverageTimeTaken + " ms");
        }
        #endregion
    }
}