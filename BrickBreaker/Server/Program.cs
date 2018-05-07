using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    class Program
    {
        public static void CVlose()
        {
            Environment.Exit(0);
        }

        static void Main(string[] args)
        {
            ThreadPool.QueueUserWorkItem(ServerZapusk);
            Console.WriteLine("Press any key");
            Console.ReadKey();
        }

        private static void ServerZapusk(Object ob)
        {
            try
            {
                TcpListener server = new TcpListener(IPAddress.Any, 11000);
                server.Start();

                while (true)
                {
                    var client = server.AcceptTcpClient();
                    Thread thread = new Thread(new ServerNet().ListenerClient);
                    thread.IsBackground = true;
                    thread.Start(client);
                }
            }
            catch { }
        }
    }
}
