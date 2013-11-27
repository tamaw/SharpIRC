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

namespace IRC
{
    public class User
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
