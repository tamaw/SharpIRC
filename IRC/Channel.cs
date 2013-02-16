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
using System.Linq;

namespace IRC
{
    public sealed class Channel
    {
        public readonly ObservableCollection<User> Users = new ObservableCollection<User>();  
        public string Name { get; private set; } // case insensitive
        public string Key { get; private set; } // set changes if OP
        public Client Client { get; private set; }
        public bool IsConnected { get; set; }
        // todo what permission you have on this channel
        public event EventHandler Joined;
        public event EventHandler Parted; //todo rename left
        public event EventHandler TopicChanged;
        public event EventHandler<string> Message;

        public string Topic {
            get { return _topic; } // this.Topic(Name);
            set { _topic = value;  } // todo test admin then set topic?!?
        }

        private void OnMessage(string e)
        {
            EventHandler<string> handler = Message;
            if (handler != null) handler(this, e);
        }

        private void OnJoined()
        {
            if (Joined != null) Joined(this, EventArgs.Empty);
        }

        private void OnParted()
        {
            EventHandler handler = Parted;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        private void OnTopicChanged()
        {
            EventHandler handler = TopicChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public Channel(Client client, string name, string key = "")
        {
            Name = name;
            Key = key;
            _client = client;
        }

        public void Join()
        {
            _client.Join(Name, Key); 
        }

        public void Leave(string message = "")
        {
            _client.Part(Name, message);
        }

        public void Kick(User user)
        {

        }

        public void Invite(User user)
        {
            _client.Invite(user.Nick, Name);
        }

        public void Say(string message)
        {
            //_client.
        }

        public void ProcessReply(object sender, Reply reply)
        {

            // todo maybe check parm[0] for "name" before continuing
            // parm 0 is always channel name on text and nickname on code?!?
            // not sure why we bother to test this!?!
            //if (reply.Params[0] != _client.Nickname)
                //return;
            // todo maybe handle text only in client
            // maybe handle by logger class for different levels

            switch (reply.Command)
            {
                case "JOIN" :
                    if(reply.Params.Count > 0 && reply.Params[0] == Name)
                        OnJoined();
                    break;
                case "PRIVMSG" :
                    if(reply.Params.Count > 0 && reply.Params[0] == Name)
                        OnMessage(reply.Trailing);
                    break;
            }

            int code;
            if (!int.TryParse(reply.Command, out code))
                return;

            var replyCode = (ReplyCode) code;
            switch (replyCode)
            {
                case ReplyCode.RplTopic :
                    if (reply.Params[1] != Name)
                        return;
                    Topic = reply.Trailing;
                    OnTopicChanged();
                    break;
                case ReplyCode.RplTopicSetBy:
                    if (reply.Params[1] != Name)
                        return;
                            // 0 is client nickname
                    // todo may not use this
                    _client.Logger("Topic set by " + reply.Params[2]);
                    break;
                case ReplyCode.RplNameReply:
                    foreach (var user in reply.Trailing.Split().Select(name => new User(_client, name)).Where(user => !Users.Contains(user)))
                        Users.Add(user);
                    break;
                case ReplyCode.RplNoTopic:
                    Topic = String.Empty;
                    break;
            }
        }

        private readonly Client _client;
        private string _topic;
    }

}