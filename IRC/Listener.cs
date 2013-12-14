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
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace IRC
{

    internal class Listener
    {
        private NetworkStream _networkStream;
        private Client _client;
        public event EventHandler<Reply> ReceivedReply;

        protected virtual void OnReceivedReply(Reply reply)
        {
            if (ReceivedReply != null)
                ReceivedReply(this, reply);
        }

        public Listener(Client client, NetworkStream networkStream)
        {
            _networkStream = networkStream;
            _client = client;
        }

        public void Listen() // todo maybe move later
        {
            var data = new byte[4076]; // MAX 512 characters per message (ascii/unicode?) multi messages however/!?

            int size;

            while ((size = _networkStream.Read(data, 0, data.Length)) != 0)
            {
                string[] messages = Encoding.ASCII.GetString(data, 0, size).Split('\n');

                foreach (var message in messages)
                {
                    // trim message?
                    if (string.IsNullOrEmpty(message))
                        continue;
                    if (!message.EndsWith("\r")) // half message
                        continue; //for now. eventuall rejoin it with the next packet
                    var reply = Reply.Decode(message);
                    //_client.Logger(reply.ToString());
                    //_client.Logger(message); //todo remove me

                    OnReceivedReply(reply);
                }
                // messages can continue to come in different reads

                // todo concurrency issues below?? maybe make logger a queue?
                //_client.Logger(Encoding.ASCII.GetString(data, 0, size));
            }
        }

    }
}
