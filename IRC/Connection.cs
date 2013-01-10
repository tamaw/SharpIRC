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
    public class Connection : IDisposable
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

        public void Dispose()
        {
            TcpClient.Close();
        }
    }
}
