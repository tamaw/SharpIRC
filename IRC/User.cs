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

namespace IRC
{
    public class User : EventArgs, IComparable
    {
        private readonly Client _client;

        [Flags]
        public enum Mode // todo private and use getters/setters
        {
            Away,           // a - user is flagged as away
            Invisible,      // i - marks a users as invisible
            Wallops,        // w - user receives wallops
            Restricted,     // r - restricted user connection
            Operator,       // o - operator flag;
            LocalOperator,	// O - local operator flag
            Notices         // s - marks a user for receipt of server notices
        }
                   
        public User(Client client, string nick)
        {
            _client = client;
            Nick = nick;
        }

        public string Nick { get; set; } // unique to server
        public string RealName { get; set; }
        public string LeaveMessage { get; set; }
        public Mode Modes;

        public void Ignore()
        {
            // todo stop logging to UI for this user?
            //_client.
        }

        public void Say(String message)
        {
        }

        public override string ToString()
        {
            return Nick;
        }

        public int CompareTo(object obj)
        {
            var user = (User) obj;
            return String.Compare(Nick, user.Nick, StringComparison.Ordinal);
        }


        /*
        public string Message {
            set
            {
                if (!string.IsNullOrEmpty(value))
                    _client.PrivMsg(Nick, value);

            }
        */
    }
}
