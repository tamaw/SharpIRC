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
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;

namespace IRC
{

    public sealed class Topic : EventArgs
    {
        public string Text { get; set; }
        public User ChangeUser { get; set; }

        public Topic()
        {
            Text = string.Empty;
            ChangeUser = null;
        }

        public Topic(string topicText)
        {
            Text = topicText;
        }

        public override string ToString()
        {
            return Text;
        }
    }

    public sealed class Channel
    {
        public string Name { get; private set; } // case insensitive
        public string Key { get; private set; } // set changes if OP
        public bool IsConnected { get; set; }
        // todo what permission you have on this channel
        public event EventHandler<User> Parted;
        public event EventHandler Left;
        public event EventHandler<User> Joined;
        public event EventHandler<NamesList> NamesList;

        private void OnNamesList(NamesList e)
        {
            EventHandler<NamesList> handler = NamesList;
            if (handler != null) handler(this, e);
        }

        public event EventHandler<Topic> TopicChanged;
        public event EventHandler<Message> Message;

        public Topic Topic {
            get { return _topic; } 
            set { _topic = value;  } // todo test admin then set topic?!?
        }

        private void OnParted(User user)
        {
            EventHandler<User> handler = Parted;
            if (handler != null) handler(this, user);
        }

        private void OnMessage(Message e)
        {
            EventHandler<Message> handler = Message;
            if (handler != null) handler(this, e);
        }

        private void OnJoined(User user)
        {
            if (Joined != null) Joined(this, user);
        }

        private void OnLeft()
        {
            EventHandler handler = Left;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        private void OnTopicChanged()
        {
            EventHandler<Topic> handler = TopicChanged;
            if (handler != null) handler(this, Topic);
        }

        public Channel(Client client, string name, string key = "")
        {
            Name = name;
            Key = key;
            _client = client;
            Joined += (sender, user) => IsConnected = true;
            Left += (sender, args) => IsConnected = false;
        }

        public void Join()
        {
            if (_client.IsConnected)
            {
                _client.ReceivedReply += ProcessReply;
                _client.Join(Name, Key);
            }
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
            _client.PrivMsg(Name, message);
        }

        public void ProcessReply(object sender, Reply reply)
        {

            // todo maybe check parm[0] for "name" before continuing
            // parm 0 is always channel name on text and nickname on code?!?
            // not sure why we bother to test this!?!
            //if (reply.Params[0] != _client.Nickname)
                //return;

            // todo maybe handle text only in client and delgate codes to the domain classes
            switch (reply.Command)
            {
                case "JOIN" :
                    if (reply.Params.Count <= 0 || reply.Params[0] != Name)
                        OnJoined(new User(_client, reply.Prefix.Substring(0, reply.Prefix.IndexOf('!'))));
                    break;
                case "PRIVMSG" :
                    {
                        if (reply.Params.Count == 0 || reply.Params[0] != Name)
                            return;
                        var user = new User(_client, reply.Prefix.Substring(0, reply.Prefix.IndexOf('!')));
                        OnMessage(new Message(user, reply.Trailing));
                        break;
                    }
                case "QUIT" :
                    if (reply.Params.Count <= 0 || reply.Params[0] != Name)
                    {
                        var user = new User(_client, reply.Prefix.Substring(0, reply.Prefix.IndexOf('!')))
                        {
                            LeaveMessage = reply.Trailing
                        };
                        OnParted(user);
                    }
                    break;
            }

            int code;
            if (!int.TryParse(reply.Command, out code))
                return;

            switch ((ReplyCode) code)
            {
                case ReplyCode.RplTopic :
                    if (reply.Params[1] != Name)
                        return;
                    Topic = new Topic(reply.Trailing);
                    OnTopicChanged();
                    break;
                case ReplyCode.RplTopicSetBy:
                    if (reply.Params[1] != Name) // not this channel
                        return;
                            // 0 is client nickname
                    // todo may not use this
                    _client.Logger("Topic set by " + reply.Params[2]);
                    break;
                case ReplyCode.RplNameReply:
                    if(_names == null) _names = new NamesList();
                   foreach (var user in reply.Trailing.Split().Select(name => new User(_client, name)))
                       _names.Add(user);
                    break;
                case ReplyCode.RplEndOfNames:
                    OnNamesList(new NamesList(_names));
                    break;
                case ReplyCode.RplNoTopic:
                    Topic = new Topic();
                    break;
            }
        }

        private readonly Client _client;
        private NamesList _names;
        private Topic _topic;
    }

}
