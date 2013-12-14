#region License
// Copyright 2013 Tama Waddell <me@tama.id.au>
// 
// This file is a part of IRC. <https://github.com/tamaw/SharpIRC>
//  
// This source is subject to the Microsoft Public License.
// <http://www.microsoft.com/opensource/licenses.mspx#Ms-PL>
//  All other rights reserved.
#endregion
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace IRC
{
    public class Connection 
    {
        public string Server { get; set; } // max 63 characters
        public int Port { get; set; }
        public Logger Logger { get; set; }
        protected TcpClient TcpClient; // tcplistener?
        public bool IsConnected { get; private set; }

        public Connection()
        {
        }

        public Connection(string server,  int port = 6667)
        {
            Server = server;
            Port = port;
        }

        protected internal void Send(string message) // sends all messages
        {
            byte[] data = Encoding.ASCII.GetBytes(message + "\n");
            Debug.WriteLine(message); // todo make debug?
            TcpClient.GetStream().Write(data, 0, data.Length);
        }

        protected void Connect()
        {
            try
            {
                TcpClient = new TcpClient(Server, Port);
                IsConnected = true;
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

        /*
        public void Dispose()
        {
            TcpClient.Close();
        }
        */
    }
}
