#region License
// Copyright 2013 Tama Waddell <tamrix@gmail.com>
// 
// This file is part of SharpIRC.
// 
// SharpIRC is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// SharpIRC is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with SharpIRC.  If not, see <http://www.gnu.org/licenses/>.
// 
#endregion
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
