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
using System.Linq;
using System.Text;

namespace IRC
{
    public class Message : EventArgs
    {
        public DateTime DateTime { get; set; }
        public User User { get; set; }
        public string Text { get; set; }

        //todo type? normal, quit, join??

        public Message()
        {
            DateTime = DateTime.Now;
        }

        public Message(string text)
        {
            Text = text;
        }

        public Message(User user, string text)
        {
            DateTime = DateTime.Now;
            User = user;
            Text = text;
        }
    }
}
