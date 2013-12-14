#region License
// Copyright 2013 Tama Waddell <me@tama.id.au>
// 
// This file is a part of IRC. <https://github.com/tamaw/SharpIRC>
//  
// This source is subject to the Microsoft Public License.
// <http://www.microsoft.com/opensource/licenses.mspx#Ms-PL>
//  All other rights reserved.
#endregion
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace IRC
{

    // RFC 2812
    // these probably don't have to be extension methods
    // or maybe move them all to extend client
    internal static class Commands 
    {

        #region Connection Registration

        public static void Pass(this Client client, string password)
        {
            client.Send("PASS " + password);
        }

        public static void Nick(this Client client, string nickname)
        {
            client.Send("NICK " + nickname);
        }

        public static void User(this Client client, string user, User.Mode mode, string realname)
        {
            client.Send("USER " + user + " " + mode + " * :" + realname);
        }

        public static void Oper(this Client client, string name, string password)
        {
            client.Send("OPER " + name + " " + password);
        }

        // this mode here is + or - the user.mode mode
        public static void Mode(this Client client, string nickname, string mode)
        {
            client.Send("MODE " + nickname + " " + mode);
        }

        public static void Service(this Client client, string nickname, string type, string reserved, string info)
        {
            // todo not using service just yet
        }

        public static void Quit(this Client client, string message = "")
        {
            client.Send("QUIT " + ":" + message);
        }

        public static void SQuit(this Client client, string server, string comment)
        {
            // server and comment required
            client.Send("SQUIT " + server + " :" + comment);
        }

        #endregion

        #region Channel Operations
        public static void Join(this Client client, string name, string key = "")
        {
            client.Send("JOIN " + name + " " + key);
        }

        public static void Join(this Client client, string[] channels, string[] keys = null)
        {
            if (keys == null) keys = new string[] {};
            client.Send("JOIN " + String.Join(",", channels) + " " + String.Join(",", keys));
        }

        // JOIN 0 ; leaves all channels (part)
        /*
        public void PartAll() // todo may remove this as it has no message
        {
            Send("JOIN 0");
        }
        */

        public static void Part(this Client client, string name, string message = "")
        {
            client.Send("PART " + name + " " + message);
        }

        public static void Part(this Client client, string[] names, string message = "")
        {
            client.Send("PART " + String.Join(",", names) + " " + message);
        }

        public static void Mode(this Client client, string name, string modes, string modeParams)
        {
            // todo 
        }

        public static void Topic(this Client client, string name)
        {
            client.Send("TOPIC " + name);
        }

        public static void Topic(this Client client, string channel, string topic)
        {
            client.Send("TOPIC " + channel + " :" + topic);
        }

        // also has a target argument?!?
        public static void Names(this Client client, string channel)
        {
            client.Send("NAMES " + channel);
        }

        public static void Names(this Client client, string[] channels)
        {
            client.Send("NAMES " + String.Join(",", channels));
        }

        // lists all visiable channels and users (dangrous?)
        public static void Names(this Client client)
        {
            client.Send("NAMES");
        }

        // List all channels or selected channels
        public static void List(this Client client, string[] channels = null) 
        {
            if(channels == null) channels = new string[] {};
            client.Send("LIST " + String.Join(",", channels));
        }

        // send invitation to the channel. can only invite if youre a memeber
        public static void Invite(this Client client, string nickname, string channel)
        {
            client.Send("INVITE " + nickname + " " + channel);
        }

        public static void Kick(this Client client, string channel, string user, string comment = "")
        {
            if (!string.IsNullOrEmpty(comment))
                client.Send("KICK " + channel + " " + user);
            else
                client.Send("KICK " + channel + " " + user + " :" + comment);
        }

        public static void Kick(this Client client, string[] channels, string[] users, string comment = "")
        {
            // either one channel and many users or channel length = user length
            if (!string.IsNullOrEmpty(comment))
                client.Send("KICK " + string.Join(",", channels) + " " + string.Join(",", users));
            else
                client.Send("KICK " + string.Join(",", channels) + " " + string.Join(",", users) + " :" + comment);
        }
        #endregion

        #region Sending Messages
        // send a message to 
        public static void PrivMsg(this Client client, string msgTarget, string message)
        {
            client.Send("PRIVMSG " + msgTarget + " :" + message);
        }

        // notice is the same as privmsg excpet no automatic messages should be sent back
        public static void Notice(this Client client, string msgTarget, string text)
        {
            client.Send("NOTICE " + msgTarget + " " + text);
        }
        #endregion

        #region Server Queries and Commands
        public static void LUsers(this Client client, string mask = "", string target = "")
        {
            client.Send("LUSER " + mask + " " + target);
        }

        public static void Version(this Client client, string target = "")
        {
            client.Send("VERSION " + target);
        }

        public static void Stats(this Client client, string query = "", string target = "")
        {
            client.Send("STATS " + query + " " + target);
        }

        public static void Links(this Client client, string server = "", string mask = "")
        {
            client.Send("LINKS " + server + " " + mask);
        }

        public static void Time(this Client client, string target = "")
        {
            client.Send("TIME " + target);
        }

        /// <summary>
        /// The CONNECT command can be used to request a server to try to
        /// establish a new connection to another server immediately.  CONNECT is
        /// a privileged command and SHOULD be available only to IRC Operators.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="target"></param>
        /// <param name="port"></param>
        /// <param name="remote"></param>
        public static void Connect(this Client client, string target, string port, string remote = "")
        {
            client.Send("CONNECT " + target + " " + port + " " + remote);
        }

        public static void Trace(this Client client, string target = "")
        {
            client.Send("TRACE " + target);
        }

        /// <summary>
        /// The admin command is used to find information about the administrator
        /// of the given server, or current server if <target> parameter is
        /// omitted.  Each server MUST have the ability to forward ADMIN messages
        /// to other servers.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="target"></param>
        public static void Admin(this Client client, string target = "")
        {
            client.Send("ADMIN " + target);
        }

        /// <summary>
        /// The INFO command is REQUIRED to return information describing the
        /// server: its version, when it was compiled, the patchlevel, when it
        /// was started, and any other miscellaneous information which may be
        /// considered to be relevant.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="target"></param>
        public static void Info(this Client client, string target = "")
        {
            client.Send("INFO " + target);
        }

        #endregion

        #region Service Query and Commands
        /*
         * The service query group of commands has been designed to return
         * information about any service which is connected to the network.
         */

        /// <summary>
        /// The SERVLIST command is used to list services currently connected to
        /// the network and visible to the user issuing the command.  The
        /// optional parameters may be used to restrict the result of the query
        /// (to matching services names, and services type).
        /// </summary>
        /// <param name="client"></param>
        /// <param name="mask"></param>
        /// <param name="type"></param>
        public static void ServList(this Client client, string mask = "", string type = "")
        {
            client.Send("SERVLIST " + mask + " " + type);
        }

        /// <summary>
        /// The SQUERY command is used similarly to PRIVMSG.  The only difference
        /// is that the recipient MUST be a service.  This is the only way for a
        /// text message to be delivered to a service.
        /// </summary>
        /// <see cref="PrivMsg"/>
        /// <param name="client"></param>
        /// <param name="servicename"></param>
        /// <param name="text"></param>
        public static void SQuery(this Client client, string servicename, string text)
        {
            client.Send("SQUERY " + servicename + " :" + text);
        }

        #endregion

        #region User Based Queries
        /*
         * User queries are a group of commands which are primarily concerned
         * with finding details on a particular user or group users.  When using
         * wildcards with any of these commands, if they match, they will only
         * return information on users who are 'visible' to you.  The visibility
         * of a user is determined as a combination of the user's mode and the
         * common set of channels you are both on.
         */

        /// <summary>
        /// The WHO command is used by a client to generate a query which returns
        /// a list of information which 'matches' the <mask> parameter given by
        /// the client.  In the absence of the <mask> parameter, all visible
        /// (users who aren't invisible (user mode +i) and who don't have a
        /// common channel with the requesting client) are listed.  The same
        /// result can be achieved by using a <mask> of "0" or any wildcard which
        /// will end up matching every visible user.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="mask"> The &lt;mask&gt; passed to WHO is matched against users' host, server, real
        /// name and nickname if the channel <mask> cannot be found.</param>
        public static void Who(this Client client, string mask = "")
        {
            client.Send("WHO " + mask);
        }

        public static void WhoIs(this Client client, string mask = "", string target = "")
        {
            WhoIs(client, new[] {mask}, target);
        }

        // todo does two spaces count for an a missing argument or is it ignored
        public static void WhoIs(this Client client, string[] mask = null, string target = "")
        {
            if(mask == null) mask = new string[] {};
            client.Send("WHOIS " + target + " " + string.Join(",", mask));
        }

        public static void WhoWas(this Client client, string nickname, string count = "", string target = "")
        {
            WhoWas(client, new[] {nickname}, count, target);
        }

        public static void WhoWas(this Client client, string[] nickname, string count = "", string target = "")
        {
            if(nickname == null) nickname = new string[] {};
            client.Send("WHOWAS " + nickname + " " + count + " " + target);
        }

        #endregion

        #region Miscellaneous Messages
        /*
         * Messages in this category do not fit into any of the above categories
         * but are nonetheless still a part of and REQUIRED by the protocol.
         */

        public static void Kill(this Client client, string nickname, string comment)
        {
            client.Send("KILL " + nickname + " " + comment);
        }

        public static void Ping(this Client client, string server1, string server2 = "")
        {
            client.Send("PING " + server1 + " " + server2);
        }

        public static void Pong(this Client client, string server, string server2 = "")
        {
            client.Send("PONG " + server + " " + server2);
        }

        /* error is only used by servers
        public static void Error(this Client client, string message)
        {
        }
        */

        #endregion

        #region Option Features
        /* This section describes OPTIONAL messages.  They are not required in a
         * working server implementation of the protocol described herein.  In
         * the absence of the feature, an error reply message MUST be generated
         * or an unknown command error.  If the message is destined for another
         * server to answer then it MUST be passed on (elementary parsing
         * REQUIRED) The allocated numerics for this are listed with the
         * messages below.*
         */


        public static void Away(this Client client, string text = "")
        {
            client.Send("AWAY :" + text);
        }

        /// <summary>
        /// The rehash command is an administrative command which can be used by
        /// an operator to force the server to re-read and process its
        /// configuration file. 
        /// </summary>
        /// <param name="client"></param>
        public static void Rehash(this Client client)
        {
            client.Send("REHASH");
        }

        public static void Die(this Client client)
        {
            client.Send("DIE");
        }

        public static void Restart(this Client client)
        {
            client.Send("RESTART");
        }

        public static void Summon(this Client client, string user, string target = "", string channel = "")
        {
            client.Send("SUMMON " + user + " " + target + " " + channel);
        }

        public static void Target(this Client client, string target = "")
        {
            client.Send("USERS " + target);
        }

        public static void WallOps(this Client client, string text)
        {
            client.Send("WALLOPS " + text);
        }

        public static void UserHost(this Client client, string nickname)
        {
            UserHost(client, new[] {nickname});
        }

        public static void UserHost(this Client client, string[] nicknames)
        {
            client.Send("USERHOST " + String.Join(" ", nicknames));
        }

        public static void Ison(this Client client, string nickname)
        {
            Ison(client, new[] {nickname});
        }

        public static void Ison(this Client client, string[] nicknames)
        {
            client.Send("ISON " + string.Join(" " , nicknames));
        }

        #endregion

    }
}
