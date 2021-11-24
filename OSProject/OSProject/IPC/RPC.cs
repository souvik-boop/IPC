using OSProject.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace OSProject.IPC
{
    public class RPC
    {
        private static DateTime startTime = DateTime.MinValue;
        private static DateTime endTime = DateTime.MinValue;
        private static IPHostEntry host = Dns.GetHostEntry("localhost");
        private static IPAddress ipAddress = host.AddressList[0];
        private static Random random = new Random(Guid.NewGuid().GetHashCode());

        private static void Listener(object data)
        {
            int port = ((dynamic)data).port;
            bool showMessages = ((dynamic)data).showMessages;

            IPAddress ipaddress = IPAddress.Parse("127.0.0.1");
            TcpListener mylist = new TcpListener(ipaddress, port);
            mylist.Start();
            System.Net.Sockets.Socket s = mylist.AcceptSocket();
            byte[] b = new byte[10000];
            int k = s.Receive(b);
            string text = string.Empty;
            for (int i = 0; i < k; i++)
                text += Convert.ToChar(b[i]);
            
            s.Close();
            mylist.Stop();

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
        private static void Sender(object data)
        {
            int port = ((dynamic)data).port;
            bool showMessages = ((dynamic)data).showMessages;

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

            TcpClient Tcpclient = new TcpClient();
            Tcpclient.Connect("127.0.0.1", port);
            Stream stm = Tcpclient.GetStream();
            ASCIIEncoding ascnd = new ASCIIEncoding();
            byte[] ba = ascnd.GetBytes(Constants.MESSAGE);
            stm.Write(ba, 0, ba.Length);
            byte[] bb = new byte[100];
            int k = stm.Read(bb, 0, 100);
            for (int i = 0; i < k; i++)
            {
                Console.Write(Convert.ToChar(bb[i]));
            }

            Tcpclient.Close();
        }

        public static void Run(bool showMessages = true, int port = 20000)
        {
            var thread1 = new Thread(new ParameterizedThreadStart(Listener));
            var thread2 = new Thread(new ParameterizedThreadStart(Sender));

            thread1.Start(new { port, showMessages });
            thread2.Start(new { port, showMessages });
            thread2.Join();
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
                try
                {
                    int port = random.Next(20000, 30000);
                    Run(false, port);

                    timeTakenArr.Add((endTime - startTime).TotalMilliseconds);
                }
                catch (Exception)
                {
                    --i;
                }
            }

            return new ExecutionStats
            {
                NumberOfRuns = n,
                AverageTimeTaken = timeTakenArr.Average()
            };
        }
    }
}