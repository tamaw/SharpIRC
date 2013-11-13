using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpIRC
{
    public class UserCommand
    {
        public const string CommandStart = "\\";

        public enum SharpIRCCommand
        {
            Help,
            Connect,
            Disconnect,
            Join,
            Leave,
            Exit,
            Clear,
            Say
        }

        public SharpIRCCommand Command { get; set; }
        public List<string> Params { get; set; }

        private UserCommand()
        {
            Params = new List<string>();
        }

        static private UserCommand Decode(string commandString)
        {
            var userCommand = new UserCommand();

            if (!commandString.StartsWith(CommandStart))
                return userCommand; // empty command

            string[] commandAndParameters = commandString.Split(' ');

            //string[] commandAndParameters = message.Substring(prefixEnd + 1, trailingStart - prefixEnd - 1).Split(' ');

            return userCommand;
        }

    }
}
