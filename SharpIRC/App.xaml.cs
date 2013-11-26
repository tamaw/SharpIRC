using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using IRC;


namespace SharpIRC
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// A list of all IRC connections.
        /// </summary>
        public readonly Client IRCClient;
        /// <summary>
        /// An event handler for all user commands
        /// </summary>

        public App()
        {
            // Default connection
            IRCClient = new Client(); // todo populate from config file
        }

        protected override void OnLoadCompleted(NavigationEventArgs e)
        {
            base.OnLoadCompleted(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            // close IRC connections
            if(IRCClient.IsConnected)
                IRCClient.Disconnect();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
        }
    }
}