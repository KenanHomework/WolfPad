using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WolfPad.MVVM.ViewModels;

namespace WolfPad.MVVM.Views
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : Window
    {
        public About()
        {
            InitializeComponent();
            InfoText.Text = "Hello everyone ! I'm Kenan Yusubov.\nI'm a c# developer \n(at least let's say I'm trying to be :D).\nAnd yes, \nif you are looking for a simple and useful text editor app, you are in the right place!";
        }

        private void ResizeButton_Click(object sender, RoutedEventArgs e) => this.Close();

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

    }
}
