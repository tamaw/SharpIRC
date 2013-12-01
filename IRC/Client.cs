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
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace IRC
{
    // instance of a client connecting to a server, allow multiple
    public delegate void Logger(string message);

    // todo maybe bring connection into client
    public class Client : Connection  // connection to be obtained from client manager
    {
        public string RealName { get; set; }
        public string ServerPass { get; set; }
        public string LeaveMessage { get; set; }
        public Dictionary<string, Channel> Channels { get; private set; }

        public string Nickname {
            get { return _nickname; }
            set
            {
                if(IsConnected)
                    this.Nick(value); // update on server
                _nickname = value; // update locally
                Logger("You are now known as " + _nickname);
            }
        }

        #region Constructors
        public Client()
        {
            Channels = new Dictionary<string, Channel>(5);
            Logger = message => Debug.WriteLine(message);
            Port = 6667;
            //ServerPass = "none";
            // todo defaults for other types?
            LeaveMessage = "Bye everyone!";
        }
        #endregion

        // todo could be static return instance of this class?
        public new void Connect() 
        {
            // todo ensure required filds are populated

            // create the tcp connection
            base.Connect();

            // start listening
            _listener = new Listener(this, TcpClient.GetStream());
            _listener.ReceivedReply += ProcessReply;
            // todo instead of passing client maybe lisenter subscribe to client events?
            _listenerThread = new Thread(_listener.Listen);
            _listenerThread.Start();

            // offical RFC 2812 doesn't support a message to start the
            // client registration. additionally CAP does.

            Thread.Sleep(500);
            // send the password is there is one
            if (!string.IsNullOrEmpty(ServerPass))
                this.Pass(ServerPass);

            // register nickname message
            this.Nick(Nickname);

            // send user message
            this.User(Nickname, (User.Mode) 8, RealName);
        }

        protected void ProcessReply(object sender, Reply reply)
        {
            switch (reply.Command)
            {
                case "NOTICE":
                    Logger(reply.Trailing);
                    System.Media.SystemSounds.Beep.Play();
                    break;
                case "PING":
                    this.Pong(reply.Trailing);
                    Logger(reply.ToString());
                    break;
                case "JOIN" :
                    if (reply.Params.Count <= 0 && !Channels.ContainsKey(reply.Params[0]))
                        return;
                    break;
                case "MODE":
                    break;
                case "ERROR" :
                    Logger("error here");
                    break;
            }

            int code;
            if (!int.TryParse(reply.Command, out code))
                return;

            switch ((ReplyCode)code)
            {
                // welcome messages
                case ReplyCode.RplWelcome:
                case ReplyCode.RplYourHost:
                case ReplyCode.RplCreated:
                case ReplyCode.RplMyInfo:
                //case ReplyCode.RplMap: // map needs to be handled differently
                //case ReplyCode.RplEndOfMap: // describes that the server supports
                case ReplyCode.RplMotdStart:
                case ReplyCode.RplMotd:
                case ReplyCode.RplMotdAlt:
                case ReplyCode.RplMotdAlt2:
                case ReplyCode.RplMotdEnd:
                case ReplyCode.RplUModeIs:
                // LUser 
                case ReplyCode.RplLUserClient:
                case ReplyCode.RplLUserOp:
                case ReplyCode.RplLUserUnknown:
                case ReplyCode.RplLUserChannels:
                case ReplyCode.RplLUserMe:
                case ReplyCode.RplLUserLocalUser:
                case ReplyCode.RplLUserGlobalUser:
                    Logger(reply.Trailing);
                    break;
                default:
                    Debug.WriteLine(reply.Trailing);
                    break;
            }

            Debug.WriteLine(reply.ToString());
        }

        public new void Disconnect()
        {
            this.Quit(LeaveMessage);
            // wait for error message to acknowledge quit

            // better way to end thread
            if(_listenerThread != null)
                _listenerThread.Abort();

            base.Disconnect();
        }

        #region Channels

        public event EventHandler<Reply> ReceivedReply
        {
            add { _listener.ReceivedReply += value; }
            remove { _listener.ReceivedReply -= value; }
        }

        public Channel CreateChannel(string name, string key = "")
        {
            var channel = new Channel(this, name, key);
            Channels.Add(name, channel);

            return channel;
        }

        public Channel[] CreateChannels(string[] names, string[] keys = null)
        {
            var channels = new Channel[names.Count()];
            if(keys != null && names.Count() != keys.Count())
                throw new ArgumentException("Must have a key for each channel name.");

            for (int i = 0; i < names.Length; i++)
            {
                var channel = keys != null ? new Channel(this, names[i], keys[i]) 
                                      : new Channel(this, names[i]);

                Channels.Add(names[i], channel);
                //_listener.ReceivedReply += channel.ProcessReply;
            }

            // Multiple channels at once
            //this.Join(names, keys);

            return channels;
        } 

        #endregion

        public User CreateUser(string nickname)
        {
            return new User(this, nickname);
        }

        private string _nickname;
        private Thread _listenerThread;
        private Listener _listener;
    }
}
