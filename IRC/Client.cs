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
    // instance of a client connecting to a server, allow multiple
    public delegate void Logger(string message);

    public class Client : Connection //todo client to not extend commands?
    {
        // todo maybe replace below with localuser

        public string Nickname { get; set; }
        public string RealName { get; set; }
        public string ServerPass { get; set; }

        private Thread _listenerThread;

        // todo connection status?

        public Client()
        {
            Logger = Console.WriteLine;
            Port = 6667;
            // todo defaults for other types?
        }

        new public void Connect() 
        {
            base.Connect();

            // start listening
            var listener = new Listener(this, TcpClient.GetStream());
            // todo instead of passing client maybe subscribe to listener events?
            _listenerThread = new Thread(listener.Listen);
            _listenerThread.Start();
        }

        new public void Disconnect()
        {

            // todo send Quit
            this.Quit(); // todo add quit messages
            // wait for error message to acknoledge quit

            if(_listenerThread != null)
                _listenerThread.Abort();

            base.Disconnect();
        }

        public static void Main()
        {
            var client = new Client
                             {
                                 Nickname = "Tama00",
                                 RealName = "Tama",
                                 Server = "chat.freenode.org"
                             };

            client.Connect();

            //??

            //Channel.Join(client, "#test");
            //client.Join("#test");

            // send pass (skip)
            // send nick message
            // send user message
            // wait for welcome

        }

    }

}
