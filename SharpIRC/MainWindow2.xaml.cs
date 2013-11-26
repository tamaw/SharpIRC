using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using IRC;
using MahApps.Metro.Controls;
using SharpIRC.ViewModel;

namespace SharpIRC
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow2 : MetroWindow
    {
        public MainWindow2()
        {
            InitializeComponent();

        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
        }
    }
}
