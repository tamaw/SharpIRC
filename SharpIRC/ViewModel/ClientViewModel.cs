using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using IRC;

namespace SharpIRC.ViewModel
{
    public class ClientViewModel
    {
        public ObservableCollection<ChannelViewModel> Channels { get; set; }
        public Client Client {
            get { return _client ?? (_client = ((App) Application.Current).IRCClient); }
        }

        public ClientViewModel()
        {
            Channels = new ObservableCollection<ChannelViewModel>();

            var c = new Channel(Client, "servername");
            var c1 = new Channel(Client, "testing");

            Channels.Add(new ChannelViewModel(c));
            Channels.Add(new ChannelViewModel(c1));

            Channels[0].Message(new Message(new User(Client, "a user"), "i am test message"));
            Channels[1].Message(new Message(new User(Client, "user2"), "i am another test message"));
            Channels[0].Users.Add(new User(Client, "person"));
            Channels[0].Users.Add(new User(Client, "joebob"));
            Channels[1].Users.Add(new User(Client, "bobbyjoe"));
        }

        private Client _client;
    }
}
