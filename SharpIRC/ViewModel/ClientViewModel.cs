using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using IRC;
using SharpIRC.Annotations;

namespace SharpIRC.ViewModel
{
    public class ClientViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<ChannelViewModel> Channels { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public User IamUser { get; set; } // should be use rmodel

        public string Nickname {
            get { return Client.Nickname; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    Client.Nickname = value;
                    OnPropertyChanged("Nickname");
                }
            }
        }

        public Client Client {
            get { return _client ?? (_client = ((App) Application.Current).IRCClient); }
        }

        public ClientViewModel()
        {
            Channels = new ObservableCollection<ChannelViewModel>();
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private Client _client;
    }
}
