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
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
                string[] messages = Encoding.ASCII.GetString(data, 0 , size).Split('\n');

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
