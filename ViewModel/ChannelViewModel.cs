using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using IRC;

namespace ViewModel
{
    public class ChannelViewModel
    {
        public ObservableCollection<string> Messages { get; private set; }
        public ObservableCollection<User> Users { get; set; } 

        public ChannelViewModel(Dispatcher dispatcher, Channel channel) : this()
        {
            _channel = channel;
            _dispatcher = dispatcher;

            //Application.Current.MainWindow.Dispatcher

            // todo pretty format message (colours)
            channel.Message += (sender, s) => dispatcher.Invoke(() => Messages.Add(s));
            channel.Users.CollectionChanged += (sender, args) => dispatcher.Invoke(() => 
                                                   {
                                                       if(args.Action == NotifyCollectionChangedAction.Add)
                                                           Users.Add((User) args.NewItems[0]);
                                                       if(args.Action == NotifyCollectionChangedAction.Remove)
                                                           Users.Remove((User) args.OldItems[0]);
                                                   });
        }

        public ChannelViewModel()
        {
            Messages = new ObservableCollection<string>();
            Users = new ObservableCollection<User>();
        }

        private Channel _channel;
        private Dispatcher _dispatcher;
    }
}
