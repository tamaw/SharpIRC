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
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using IRC;
using MahApps.Metro;
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

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            // close IRC connections
            if(IRCClient.IsConnected)
                IRCClient.Disconnect();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            // set theme
            ThemeManager.ChangeTheme(this,
                  ThemeManager.DefaultAccents.First(x => x.Name == Settings.Default.Theme),
                  (Settings.Default.Style == "Dark") ? Theme.Dark : Theme.Light);
            base.OnStartup(e);
        }
    }
}