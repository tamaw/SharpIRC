using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace IRC
{
    class Program
    {

        public static void Main()
        {
            var client = new Client
            {
                Nickname = "TamaTest",
                RealName = "Tama",
                Server = "chat.freenode.org"
            };

            Console.WriteLine("starting..");

            client.Connect(); // enable settgin actions before connect

            Channel c = client.CreateChannel("#testroom2013");
            c.Users.CollectionChanged += (sender, args) =>
                                             {
                                                 if(args.Action == NotifyCollectionChangedAction.Add)
                                                     Console.WriteLine(args.NewItems[0].ToString() + " has joined " + c.Name);
                                             };
            c.Joined += (sender, args) => Console.WriteLine("You have joined " + c.Name);
            c.TopicChanged += (sender, args) => Console.WriteLine("Topic: " + args);
            c.Message += (sender, message) => Console.WriteLine(message.Text);
            c.Join();

            Console.ReadKey();
            client.Disconnect();
        }
    }
}
