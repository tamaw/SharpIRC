using System;

namespace IRC
{
    public class User
    {
        [Flags]
        public enum Mode // todo private and use getters/setters
        {
            Away,           // a - user is flagged as away
            Invisible,      // i - marks a users as invisible
            Wallops,        // w - user receives wallops
            Restricted,     // r - restricted user connection
            Operator,       // o - operator flag;
            LocalOperator,	// O - local operator flag
            Notices         // s - marks a user for receipt of server notices
        }
                   
        public User()
        {
            
        }

        public User(Client client)
        {
        }

        public string Nick { get; set; } // unique to server
        public string RealName { get; set; }
        public Mode Modes;
    }

    // maybe scrap this
    /*
    public class LocalUser : User
    {
        private readonly Client _client;

        public new string Nick
        {
            get { return base.Nick; }
            set
            {
                if (value == base.Nick) return;

                _client.Nick(value);
                base.Nick = value;
            }
        }

        //public new string RealName 

        public void Oper(string password)
        {
        }


        public LocalUser(Client client)
        {
            _client = client;
        }
    }
    */

}