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
            //ServerPass = "none";
            // todo defaults for other types?
        }

        // todo could be static return instance of this class?
        public new void Connect() 
        {

            // todo ensure required filds are populated

            // create the tcp connection
            base.Connect();

            // start listening
            var listener = new Listener(this, TcpClient.GetStream());
            // todo instead of passing client maybe lisenter subscribe to client events?
            _listenerThread = new Thread(listener.Listen);
            _listenerThread.Start();

            // offical RFC 2812 doesn't support a message to start the
            // client registration. additionally CAP does.

            // send the password is there is one
            if (!string.IsNullOrEmpty(ServerPass))
                this.Pass(ServerPass);

            // register nickname message
            this.Nick(Nickname);

            // send user message
            this.User(Nickname, (User.Mode) 8, RealName);



            // sleep and quit
            //Thread.Sleep(10000); 

            //this.Quit("Test quit");
        }

        new public void Disconnect()
        {

            // todo send Quit
            this.Quit(); // todo add quit messages
            // wait for error message to acknoledge quit


            // todo prob sholdnt send the bottom stuff
            if(_listenerThread != null)
                _listenerThread.Abort();

            base.Disconnect();
        }

        public static void Main()
        {
            var client = new Client
                             {
                                 Nickname = "TamaTest",
                                 RealName = "Tama",
                                 Server = "chat.freenode.org"
                             };

            client.Connect();


            //Channel.Join(client, "#test");
            //client.Join("#test");

            // send pass (skip)
            // send nick message
            // send user message
            // wait for welcome

        }

    }

}
