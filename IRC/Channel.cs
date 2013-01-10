using System;
using System.Collections.Generic;

namespace IRC
{
    public class Channel
    {
        public List<User> Users; // todo maybe hashmap with string, user
        public string Name { get; private set; } // case insensitive
        public string Key { get; private set; }
        private string _topic;
        public string Topic {
            get { return _topic; } // this.Topic(Name);
            set { _topic = value; }
        }

        private readonly Client _client;

        public Channel(Client client, string name, string key = "")
        {
            Name = name;
            _client = client;
            Users = new List<User>(10);
            _client.Join(name); // todo should we have join here
        }

        /*
        public void Part(string message = "") // todo message could come from the settings 
        {

        }
        */

        public static Channel Join(Client client, string name, string key = "")
        {
            // channel needs to store in client and on listener response assign to existing object
            // add listener here?
            return new Channel(client, name, key);
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
            
        }
    }
}