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
        public ObservableCollection<string> Users { get; set; } 

        public ChannelViewModel(Dispatcher dispatcher, Channel channel) : this()
        {
            _channel = channel;
            _dispatcher = dispatcher;

            //Application.Current.MainWindow.Dispatcher

            // todo pretty format message (colours)
            channel.Message += (sender, m) => dispatcher.Invoke(() => Messages.Add(String.Format("{0:HH:mm:ss} <{1}> {2}", m.DateTime, m.User.Nick, m.Text)));

            // todo should reverse list and trim 
            channel.Users.CollectionChanged += (sender, args) => dispatcher.Invoke(() => 
                                                   {
                                                       if(args.Action == NotifyCollectionChangedAction.Add)
                                                           Users.Add(((User) args.NewItems[0]).Nick);
                                                       if(args.Action == NotifyCollectionChangedAction.Remove)
                                                           Users.Remove(((User) args.OldItems[0]).Nick);
                                                   });
        }

        public ChannelViewModel()
        {
            Messages = new ObservableCollection<string>();
            Users = new ObservableCollection<string>();
        }

        private Channel _channel;
        private Dispatcher _dispatcher;
    }
}
