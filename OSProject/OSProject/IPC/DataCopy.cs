using OSProject.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OSProject.IPC
{
    public class DataCopy
    {
        private const string FileName = @"..\..\..\..\SendMessageDemo\bin\Debug\SendMessageDemo.exe";
        private const string EndTimePath = @"end-time";

        public static void Run(bool showMessages = true)
        {
            var process = new Process();
            process.StartInfo = new ProcessStartInfo(FileName, $"\"{Constants.MESSAGE}\"");

            if (!showMessages)
            {
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            }

            process.Start();
            process.WaitForInputIdle();
            Task.Delay(1000).Wait();
        }

        public static ExecutionStats GetStats(int n = 100, bool suppressOutput = false)
        {
            File.Delete(EndTimePath);

            if (suppressOutput)
            {
                Console.WriteLine("Getting Statistics of the run ...");
                Console.WriteLine();
            }

            List<double> timeTakenArr = new List<double>();
            List<DateTime> startTimeArr = new List<DateTime>();
            List<DateTime> endTimeArr = new List<DateTime>();

            for (int i = 0; i < n; i++)
            {
                startTimeArr.Add(DateTime.Now);
                Run(false);
                Task.Delay(1000).Wait();
            }

            foreach (string line in File.ReadLines(EndTimePath))
                endTimeArr.Add(DateTime.ParseExact(line, "dddd, dd MMMM yyyy HH:mm:ss:fffffff", CultureInfo.InvariantCulture));
            for (int i = 0; i < n; i++)
                timeTakenArr.Add((endTimeArr[i] - startTimeArr[i]).TotalMilliseconds);

            File.Delete(EndTimePath);

            return new ExecutionStats
            {
                NumberOfRuns = n,
                AverageTimeTaken = timeTakenArr.Average()
            };
        }
    }
}