using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public MainWindow()
        {
            InitializeComponent();
            var client = new Client
            {
                Nickname = "TamaTest",
                RealName = "Tama",
                Server = "chat.freenode.org"
            };

            client.Connect();

            Channel channel = client.CreateChannel("#ubuntu");
            var channelViewModel = new ChannelViewModel(Dispatcher, channel);

            MessageDispalyListBox.DataContext = channelViewModel.Messages;
            UsersListView.DataContext = channelViewModel.Users;

            channel.Message += (sender, s) => Debug.WriteLine("message: " + s);
            // todo message should probably have sender 
            
            /*
            channel.Message += (sender, s) =>
                                   {
                                       channelViewModel
                                   };
            */

            channel.Join();

        }
    }
}
