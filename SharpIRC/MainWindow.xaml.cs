#region License
// Copyright 2013 Tama Waddell <me@tama.id.au>
// 
// This file is a part of SharpIRC. <https://github.com/tamaw/SharpIRC>
//  
// This source is subject to the Microsoft Public License.
// <http://www.microsoft.com/opensource/licenses.mspx#Ms-PL>
//  All other rights reserved.
#endregion
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using IRC;
using MahApps.Metro.Controls;
using SharpIRC.ViewModel;
using SharpIRC.Views;

namespace SharpIRC
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private readonly App _app = (App) Application.Current;
        private readonly ClientViewModel _clientViewModel;
        private IIrcTabItemModel _currentChannelViewModel; 

        public MainWindow()
        {
            InitializeComponent();
            _clientViewModel = new ClientViewModel();
            DataContext = _clientViewModel;
        }

        private void AddServerChannel()
        {
            var serverTab = new ServerViewModel(_app.IRCClient);
            _app.IRCClient.Logger += serverTab.Message;
            _clientViewModel.IrcTabItems.Add(serverTab);
            ChannelTabControl.SelectedIndex = 0;

            serverTab.Message("Welcome to SharpIRC.");
            serverTab.Message("See README for more information.");
            serverTab.Message("Type /help to begin.");
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            AddServerChannel();
            AddUserCommandlets();

            AboutButton.Click += (o, args) =>
            {
                var about = new AboutWindow(this);
                about.okButton.Click += (sender1, eventArgs) => about.Close();
                about.Show();
            };

            SettingsButton.Click += (o, args) => new SettingsWindow(this).Show();

        }

        private void TestRoom()
        {
            // todo remove me
            var chan = new Channel(_app.IRCClient, "#testing");
            var cvm = new ChannelViewModel(chan);
            _clientViewModel.IrcTabItems.Add(cvm);

            cvm.Users.Add(new User(_app.IRCClient, "A"));
            cvm.Users.Add(new User(_app.IRCClient, "Q"));
            cvm.Users.Add(new User(_app.IRCClient, "b"));
            cvm.Users.Add(new User(_app.IRCClient, "h"));
            cvm.Users.Add(new User(_app.IRCClient, "Z"));
            cvm.Users.Add(new User(_app.IRCClient, "0"));
            cvm.Users.Add(new User(_app.IRCClient, "Ar"));
            cvm.Users.Add(new User(_app.IRCClient, "p"));

            ChannelTabControl.SelectedIndex = 1;
        }

        private void AddUserCommandlets()
        {
            UserCommand.AddCommandlet("help", parameters => _currentChannelViewModel.Message(
                "Commands: /connect, /join, /leave, /exit, /clear, /say, /msg, /nick, /help"));
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

                // TODO say connecting to x
                new Thread(() => _app.IRCClient.Connect()).Start();
            });
            UserCommand.AddCommandlet("msg", parameters =>
            {
                if(parameters.Length >= 2 && !string.IsNullOrEmpty(parameters[0]) && !string.IsNullOrEmpty(parameters[1]))
                {
                    string[] message = new string[parameters.Length -1];
                    Array.Copy(parameters, 1, message, 0, parameters.Length - 1); // todo, just pass me all as a string dammit
                    _app.IRCClient.PrivMsg(parameters[0], string.Join(" ", message));
                }
            });

            UserCommand.AddCommandlet("join", parameters =>
            {
                if (parameters.Length != 1 || string.IsNullOrEmpty(parameters[0])) return;

                // create a channel based off of the first parameter
                Channel channel = _app.IRCClient.CreateChannel(parameters[0]);
                // create a representation of the channel in the view
                var cvm = new ChannelViewModel(channel);
                // add the channel to the tabs list
                _clientViewModel.IrcTabItems.Add(cvm);
                // select the newly created tab
                ChannelTabControl.SelectedIndex = ChannelTabControl.Items.IndexOf(cvm);
                cvm.Channel.NamesList += (sender, list) => Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                {
                    var userListBox = FindVisualChildByName<ListBox>(ChannelTabControl, "UsersListView");
                    if(userListBox == null)
                        Debug.WriteLine("not found: null");
                    else 
                        userListBox.Items.SortDescriptions.Add(new SortDescription("Nick", ListSortDirection.Ascending));
                }));
                // join the channel
                channel.Join();
            });

            UserCommand.AddCommandlet("leave", parameters =>
            {
                var cvm = ChannelTabControl.SelectedItem as ChannelViewModel;
                if (cvm == null) return;

                if(cvm.Channel.IsConnected) 
                {
                    if(parameters.Length == 1 && !string.IsNullOrEmpty(parameters[0]))
                        cvm.Channel.Leave(parameters[0]);
                    else
                        cvm.Channel.Leave();
                }
                _clientViewModel.IrcTabItems.Remove(cvm);
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
            else // without a slash this is a user say command
            {
                var channel = _currentChannelViewModel as ChannelViewModel;
                if (channel != null)
                    channel.Say(tBox.Text);
            }

            tBox.Text = string.Empty;
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _currentChannelViewModel = ChannelTabControl.SelectedItem as IIrcTabItemModel;
            
            if(_currentChannelViewModel != null)
                Debug.WriteLine("Selection Changed: " + _currentChannelViewModel.Header);
        }

        private T FindVisualChildByName<T>(DependencyObject parent, string name) where T : FrameworkElement
        {
            T child = default(T);
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var ch = VisualTreeHelper.GetChild(parent, i);
                Debug.WriteLine(ch.ToString());
                child = ch as T;
                if (child != null && child.Name == name)
                    break; // found

                child = FindVisualChildByName<T>(ch, name); 

                if (child != null) break; // nothing left
            }
            return child;
        }

        public bool AutoScroll = true;
        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            // User scroll event : set or unset autoscroll mode
            if (e.ExtentHeightChange == 0)
            {   // Content unchanged : user scroll event
                if ((e.Source as ScrollViewer).VerticalOffset == (e.Source as ScrollViewer).ScrollableHeight)
                {   // Scroll bar is in bottom
                    // Set autoscroll mode
                    AutoScroll = true;
                }
                else
                {   // Scroll bar isn't in bottom
                    // Unset autoscroll mode
                    AutoScroll = false;
                }
            }

            // Content scroll event : autoscroll eventually
            if (AutoScroll && e.ExtentHeightChange != 0)
            {   // Content changed and autoscroll mode set
                // Autoscroll
                (e.Source as ScrollViewer).ScrollToVerticalOffset((e.Source as ScrollViewer).ExtentHeight);
            }
        }
    }
}
