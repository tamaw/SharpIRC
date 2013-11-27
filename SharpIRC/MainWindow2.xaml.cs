using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;
using IRC;
using MahApps.Metro.Controls;
using SharpIRC.ViewModel;

namespace SharpIRC
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow2 : MetroWindow
    {
        private readonly App _app = (App) Application.Current;
        private readonly ClientViewModel _clientViewModel;
        private ChannelViewModel _currentChannelViewModel; 

        public MainWindow2()
        {
            InitializeComponent();
            _clientViewModel = new ClientViewModel();
            DataContext = _clientViewModel;

            //var tb = FindVisualChildByName<TextBlock>(ChannelTabControl, "Nickname");
            //Debug.WriteLine("text:" + tb);
        }

        // TODO this is to be replaced with server view model in the future
        private void AddServerChannel()
        {
            var serverChannel = new ChannelViewModel(new Channel(_app.IRCClient, _app.IRCClient.Server));
            _app.IRCClient.Logger += serverChannel.SystemMessage;
            _clientViewModel.Channels.Add(serverChannel);
            ChannelTabControl.SelectedIndex = 0;

            serverChannel.SystemMessage("Welcome to SharpIRC.");
            serverChannel.SystemMessage("Created by Tama Waddell.");
            serverChannel.SystemMessage("GNU GENERAL PUBLIC LICENSE and Creative Commerce license.");
            serverChannel.SystemMessage("See README for more information.");
            serverChannel.SystemMessage("Type /help to begin.");
        }

        private void MainWindow2_OnLoaded(object sender, RoutedEventArgs e)
        {
            AddServerChannel();
            AddUserCommandlets();
        }

        private void AddUserCommandlets()
        {
            UserCommand.AddCommandlet("help", parameters => _currentChannelViewModel.SystemMessage(
                "Commands: /connect, /join, /leave, /exit, /clear, /say, /nick, /help"));
            UserCommand.AddCommandlet("exit", parameters => _app.Shutdown(0));
            UserCommand.AddCommandlet("clear", parameters => _currentChannelViewModel.Clear());
            UserCommand.AddCommandlet("nick", parameters =>
            {
                if (parameters.Length == 1 && !string.IsNullOrEmpty(parameters[0]))
                    _clientViewModel.Nickname = parameters[0];
            });
            UserCommand.AddCommandlet("connect", parameters =>
            {
                if (parameters.Length == 1)
                    _app.IRCClient.Server = parameters[0];

                // clientviewmodel. server model. message. connecting to x
                new Thread(() => _app.IRCClient.Connect()).Start();
            });


            UserCommand.AddCommandlet("join", parameters =>
            {
                if (parameters.Length != 1 || string.IsNullOrEmpty(parameters[0])) return;

                Channel channel = _app.IRCClient.CreateChannel(parameters[0]);
                _clientViewModel.Channels.Add(new ChannelViewModel(channel));
                /* myChannel.TopicChanged += (sender, s) => Dispatcher.Invoke(DispatcherPriority.Normal,
                    (Action)(() => Title = s.Text)); */
                channel.Join();
            });

            UserCommand.AddCommandlet("leave", parameters =>
            {
                if (parameters.Length == 1 && !string.IsNullOrEmpty(parameters[0]))
                    _app.IRCClient.Channels.Remove(parameters[0]);
            });
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;

            var tBox = (TextBox)sender;

            // if it starts in a slash process the command
            if (tBox.Text.StartsWith(UserCommand.CommandStart))
            {
                UserCommand.Cook(tBox.Text);
            }

            tBox.Text = string.Empty;
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _currentChannelViewModel = (ChannelViewModel) ChannelTabControl.SelectedItem;
            Debug.WriteLine("Selection Changed: " + _currentChannelViewModel.Header);
        }

        private T FindVisualChildByName<T>(DependencyObject parent, string name) where T : FrameworkElement
        {
            T child = default(T);
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var ch = VisualTreeHelper.GetChild(parent, i);
                child = ch as T;
                if (child != null && child.Name == name)
                    break; // found

                child = FindVisualChildByName<T>(ch, name); // recursive call

                if (child != null) break; // nothing left
            }
            return child;
        }

    }
}
