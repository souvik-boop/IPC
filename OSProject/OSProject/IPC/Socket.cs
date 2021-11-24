using OSProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace OSProject.IPC
{
    public class Socket
    {
        private static DateTime startTime = DateTime.MinValue;
        private static DateTime endTime = DateTime.MinValue;
        private static IPHostEntry host = Dns.GetHostEntry("localhost");
        private static IPAddress ipAddress = host.AddressList[0];
        private static Random random = new Random(Guid.NewGuid().GetHashCode());

        private static void Listener(object data)
        {
            IPEndPoint localEndPoint = ((dynamic)data).localEndPoint;
            bool showMessages = ((dynamic)data).showMessages;

            // Create a Socket that will use Tcp protocol
            System.Net.Sockets.Socket listener = new System.Net.Sockets.Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            // A Socket must be associated with an endpoint using the Bind method
            listener.Bind(localEndPoint);
            // Specify how many requests a Socket can listen before it gives Server busy response.
            // We will listen 10 requests at a time
            listener.Listen(10);

            var handler = listener.Accept();

            // Incoming data from the client.
            string text = null;
            byte[] bytes = null;

            while (true)
            {
                bytes = new byte[1024];
                int bytesRec = handler.Receive(bytes);
                text += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                if (text.IndexOf("<EOF>") > -1)
                {
                    break;
                }
            }

            byte[] msg = Encoding.ASCII.GetBytes(text);
            handler.Send(msg);
            handler.Shutdown(SocketShutdown.Both);
            handler.Close();

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
            IPEndPoint localEndPoint = ((dynamic)data).localEndPoint;
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

            byte[] bytes = new byte[1024];

            // Create a TCP/IP  socket.
            System.Net.Sockets.Socket sender = new System.Net.Sockets.Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            // Connect the socket to the remote endpoint. Catch any errors.
            // Connect to Remote EndPoint
            sender.Connect(localEndPoint);

            // Encode the data string into a byte array.
            byte[] msg = Encoding.ASCII.GetBytes(Constants.MESSAGE + "<EOF>");

            // Send the data through the socket.
            int bytesSent = sender.Send(msg);

            // Receive the response from the remote device.
            int bytesRec = sender.Receive(bytes);

            // Release the socket.
            sender.Shutdown(SocketShutdown.Both);
            sender.Close();
        }

        public static void Run(bool showMessages = true, int port = 20000)
        {
            var localEndPoint = new IPEndPoint(ipAddress, port);

            var thread1 = new Thread(new ParameterizedThreadStart(Listener));
            var thread2 = new Thread(new ParameterizedThreadStart(Sender));

            thread1.Start(new { localEndPoint, showMessages });
            thread2.Start(new { localEndPoint, showMessages });
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
                catch (SocketException)
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