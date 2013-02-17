using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRC
{
    public class Message
    {
        public DateTime DateTime { get; set; }
        public User User { get; set; }
        public string Text { get; set; }

        //todo type? normal, quit, join??

        public Message()
        {
            DateTime = DateTime.Now;
        }

        public Message(User user, string text)
        {
            DateTime = DateTime.Now;
            User = user;
            Text = text;
        }
    }
}
