using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public ChannelViewModel(Dispatcher dispatcher, Channel channel) : this()
        {
            _channel = channel;
            _dispatcher = dispatcher;

            //Application.Current.MainWindow.Dispatcher

            // todo pretty format message (colours)
            channel.Message += (sender, s) => dispatcher.Invoke(() => Messages.Add(s));
        }

        public ChannelViewModel()
        {
            Messages = new ObservableCollection<string>();
        }

        private Channel _channel;
        private Dispatcher _dispatcher;
    }
}
