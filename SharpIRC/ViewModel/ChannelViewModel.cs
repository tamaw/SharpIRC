#region License
// Copyright 2013 Tama Waddell <me@tama.id.au>
// 
// This file is a part of SharpIRC. <https://github.com/tamaw/SharpIRC>
//  
// This source is subject to the Microsoft Public License.
// <http://www.microsoft.com/opensource/licenses.mspx#Ms-PL>
//  All other rights reserved.
#endregion
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using IRC;
using SharpIRC.Views;

namespace SharpIRC.ViewModel
{
    public class ChannelViewModel : IIrcTabItemModel
    {
        public ObservableCollection<string> TimeStamps { get; private set; }
        public ObservableCollection<string> Messages { get; private set; }
        public ObservableCollection<User> Users { get; set; } // TODO could be userviewmodel
        public string InputText { get; set; }

        public Channel Channel
        {
            get { return _channel; }
        }

        public ChannelViewModel(Channel channel)
            : this()
        {
            _channel = channel;

            //Application.Current.MainWindow.Dispatcher

            // todo pretty format message (colours)
            channel.Message += (sender, m) => Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                    (Action) (() =>
                    {
                        TimeStamps.Add(String.Format("[{0:HH:mm:ss}]\t<{1}>", DateTime.Now, m.User.Nick));
                        Messages.Add(m.Text);
                    }));

            channel.NamesList += (sender, list) => Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (Action) (() =>
                {
                    foreach (User user in list)
                        Users.Add(user);
                }));

            channel.Joined += (sender, user) =>
            {
                if (!Users.Contains(user))
                {
                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                        (Action) (() => Users.Add(user)));
                }

                Message(user.Nick + " has joined the room.");
            };

            channel.Parted += (sender, user) =>
            {
                if (Users.Contains(user))
                {
                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                        (Action) (() => Users.Remove(user)));
                }
                Message(user.Nick + " has left the room.");
            };

            // print the topic when it changes
            channel.TopicChanged += (sender, topic) => Message("Topic: " + topic.Text);
        }

        public void Clear()
        {
            TimeStamps.Clear();
            Messages.Clear();
        }

        public void Message(Message message)
        {
            if (Messages != null) Message(message.Text);
        }

        // todo maybe move me to serverviewmodel when needed
        public void Message(string message)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (Action) (() =>
                {
                    TimeStamps.Add(String.Format("[{0:HH:mm:ss}]\t\t*", DateTime.Now));
                    Messages.Add(message);
                }));
        }

        public ChannelViewModel()
        {
            TimeStamps = new ObservableCollection<string>();
            Messages = new ObservableCollection<string>();
            Users = new ObservableCollection<User>();
        }

        public string Header
        {
            get { return _channel.Name ?? string.Empty; }
        }

        public Type Type
        {
            get
            {
                return GetType();
            }
        }

        public void Say(string message)
        {
            _channel.Say(message);
        }

        public override string ToString()
        {
            return _channel.Name ?? string.Empty;
        }

        private readonly Channel _channel;
    }
}
