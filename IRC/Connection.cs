using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IRC
{
    public class Connection
    {
        public string Server { get; set; } // max 63 characters
        public int Port { get; set; }
        public Logger Logger { get; set; }
        protected TcpClient TcpClient;

        public Connection()
        {
        }

        public Connection(string server,  int port = 6667, string pass = "")
        {
            Server = server;
            Port = port;
        }

        protected internal void Send(string message) // sends all messages
        {
            byte[] data = Encoding.ASCII.GetBytes(message);
            TcpClient.GetStream().Write(data, 0, data.Length);
        }

        protected void Connect()
        {
            try
            {
                TcpClient = new TcpClient(Server, Port);
            }
            catch (SocketException e)
            {
                Logger("Fatal Error: " + e);
                Environment.Exit(1); // todo report error instead of quit
            }

        }

        protected void Disconnect()
        {
            TcpClient.Close();
        }

   }
}
