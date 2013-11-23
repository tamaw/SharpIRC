using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
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
using System.Windows.Shapes;
using System.Windows.Threading;
using IRC;
using MahApps.Metro.Controls;
using ViewModel;

namespace SharpIRC
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private readonly App _app = (App) Application.Current;

        private readonly ServerViewModel _serverViewModel;

        public MainWindow()
        {
            InitializeComponent();
            _serverViewModel = new ServerViewModel(_app.IRCConnection);

            // bind data context
            MessageListBox.DataContext = _serverViewModel.Messages;
            StampMessageListBox.DataContext = _serverViewModel.StampMessages;
            UsersListView.DataContext = _serverViewModel.Users;

            // update scroll on new message
            _serverViewModel.Messages.CollectionChanged += (sender, args) =>
                MessageListBox.SelectedIndex = MessageListBox.Items.Count - 1;
            _serverViewModel.StampMessages.CollectionChanged += (sender, args) =>
                StampMessageListBox.SelectedIndex = MessageListBox.Items.Count - 1;

            AddUserCommandlets();
        }


        private void AddUserCommandlets()
        {
            UserCommand.AddCommandlet("help", parameters => _serverViewModel.Message("Commands: help, connect, join, leave, exit, clear, say, nick"));
            UserCommand.AddCommandlet("exit", parameters => _app.Shutdown(0));
            UserCommand.AddCommandlet("clear", parameters => _serverViewModel.Clear());

            UserCommand.AddCommandlet("connect", parameters =>
            {
                if (parameters.Length == 1)
                    _app.IRCConnection.Server = parameters[0];
                // for testing todo remove
                _app.IRCConnection.RealName = "Tama";

                new Thread(() => _app.IRCConnection.Connect()).Start();
            });

            UserCommand.AddCommandlet("join", parameters =>
            {
                if (parameters.Length == 1 && !string.IsNullOrEmpty(parameters[0])) {
                    Channel myChannel = _app.IRCConnection.CreateChannel(parameters[0]);
                    myChannel.Users.CollectionChanged += (sender, args) =>
                    {
                        if (args.Action == NotifyCollectionChangedAction.Add)
                            _serverViewModel.AddUser((User)args.NewItems[0]);
                        if (args.Action == NotifyCollectionChangedAction.Remove)
                            _serverViewModel.RemoveUser((User)args.OldItems[0]);
                    };
                    myChannel.Joined += (sender, args) => _serverViewModel.Message("You have joined " + myChannel.Name);
                    myChannel.TopicChanged += (sender, s) => Dispatcher.Invoke(DispatcherPriority.Normal,
                        (Action)(() => Title = s ));
                    myChannel.Message += (sender, message) => _serverViewModel.Message(message);
                    myChannel.Join();
                }
            });

            UserCommand.AddCommandlet("nick", parameters =>
            {
                if (parameters.Length == 1 && !string.IsNullOrEmpty(parameters[0]))
                    _app.IRCConnection.Nickname = parameters[0];
            });

            UserCommand.AddCommandlet("leave", parameters =>
            {
                if (parameters.Length == 1 && !string.IsNullOrEmpty(parameters[0]))
                    _app.IRCConnection.Channels.Remove(parameters[0]);
            });

        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;

            var tBox = (TextBox) sender;

            // if it starts in a slash process the command
            if (tBox.Text.StartsWith(UserCommand.CommandStart))
            {
                UserCommand.Cook(tBox.Text);
            }

            tBox.Text = string.Empty;
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (xamlTabItem.IsSelected)
            {
                Debug.WriteLine("i'm selected biatch");
            }
        }

        
        

    }
}
