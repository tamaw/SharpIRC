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
using SharpIRC.Properties;


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

        public App()
        {
            // Default connection
            IRCClient = new Client
            {
                Nickname = Settings.Default.Nickname,
                RealName = Settings.Default.RealName,
                Server = Settings.Default.Server
            }; 
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