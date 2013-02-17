using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using IRC;
using ViewModel;

namespace SharpIRC
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        // todo to manage multiple clients

        public MainWindow()
        {
            InitializeComponent();


            _client = new Client
            {
                Nickname = "TamaTest3",
                RealName = "Tama",
                Server = "chat.freenode.org"
            };
        }

        private void ConnectMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            _client.Connect();
        }

        private void JoinRoomMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            Channel channel = _client.CreateChannel("#ubuntu"); // todo this step to be done by the viewmodel?
            var channelViewModel = new ChannelViewModel(Dispatcher, channel);

            // todo below should be delegated to viewmodel
            channel.TopicChanged += (o, args) => Dispatcher.Invoke(() =>  TopicTextBlock.Text = args.ToString(CultureInfo.InvariantCulture));

            MessageListBox.DataContext = channelViewModel.Messages;
            UsersListView.DataContext = channelViewModel.Users;

            var sortProperty = UsersListView.Tag as string;

            UsersListView.Items.SortDescriptions.Add(new SortDescription(sortProperty, ListSortDirection.Ascending));
            channel.Join();
        }

        private readonly Client _client;
    }
}
