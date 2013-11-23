using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Threading;
using System.Windows;
using IRC;

namespace ViewModel
{
    public class ChannelViewModel 
    {
        public ObservableCollection<string> TimeStamps { get; private set; }
        public ObservableCollection<string> Messages { get; private set; }
        public ObservableCollection<User> Users { get; set; } 

        public ChannelViewModel(Channel channel) : this()
        {
            _channel = channel;
            Users = _channel.Users;

            //Application.Current.MainWindow.Dispatcher

            // todo pretty format message (colours)
            channel.Message += (sender, m) => Application.Current.Dispatcher.Invoke(
                () =>
                {
                });

            // todo should reverse list and trim 
            channel.Users.CollectionChanged += (sender, args) =>
                Application.Current.Dispatcher.Invoke(() => 
                                                   {
                                                       if(args.Action == NotifyCollectionChangedAction.Add)
                                                           Users.Add(((User) args.NewItems[0]));
                                                       if(args.Action == NotifyCollectionChangedAction.Remove)
                                                           Users.Remove(((User) args.OldItems[0]));
                                                   });
        }


        public void AddUser(User user)
        {
            
        }

        public void Message(Message message)
        {
            TimeStamps.Add(String.Format("[{0:HH:mm:ss}]\t\t\t<{1}>", DateTime.Now, message.User.Nick));
            Messages.Add(message.Text);
        }

        public ChannelViewModel()
        {
            TimeStamps = new ObservableCollection<string>();
            Messages = new ObservableCollection<string>();
            Users = new ObservableCollection<User>();
        }

        private Channel _channel;
    }
}
