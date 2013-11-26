using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;
using IRC;

namespace SharpIRC.ViewModel
{
    // todo this is redundandt replaced with clientview model
    public sealed class ServerViewModel 
    {

        public ObservableCollection<ChannelViewModel> Channels { get; set; }

        public ServerViewModel(Client ircClient)
        {
            Messages = new ObservableCollection<string>();
            StampMessages  = new ObservableCollection<string>();
            Users = new ObservableCollection<User>();
            Channels = new ObservableCollection<ChannelViewModel>();
            
            // maybe move this
            Channels.Add(new ChannelViewModel(new Channel(ircClient, "Freenodez")));

            ircClient.Logger = Message;
        }

        public void Message(string message)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (Action)(() =>
                {
                    StampMessages.Add(String.Format("[{0:HH:mm:ss}]", DateTime.Now));
                    Messages.Add(message);
                }));
        }

        public void Message(Message message)
        {
             Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (Action)(() =>
                {
                    StampMessages.Add((String.Format("[{0:HH:mm:ss}]\t\t\t<{1}>", DateTime.Now, message.User.Nick)));
                    Messages.Add(message.Text);
                }));           
        }

        public void AddUser(User user)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (Action) (() => Users.Add(user)));
            Message(user.Nick + " has joined.");
        }

        public void RemoveUser(User user)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (Action) (() => Users.Remove(user)));
            Message(user.Nick + " has left.");
        }

        public void Clear()
        {
            Messages.Clear();
            StampMessages.Clear();
        }

        public ObservableCollection<string> StampMessages { get; private set; }
        public ObservableCollection<string> Messages { get; private set; }
        public ObservableCollection<User> Users { get; set; } 
    }

}
