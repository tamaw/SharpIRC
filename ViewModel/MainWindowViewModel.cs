using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IRC;

namespace ViewModel
{
    public class MainWindowViewModel
    {
        public MainWindowViewModel(Client client)
        {
            _client = client;
        }

        public static void Main(string[] args)
        {
        }

        private Client _client;
    }
}
