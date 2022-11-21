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
using Wpf.Ui.Common;
using Wpf.Ui.Controls;

namespace WolfPad.MVVM.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            App.Container.GetInstance<MainWindowVM>().Window = this;
            DataContext = App.Container.GetInstance<MainWindowVM>();
            App.Container.GetInstance<MainWindowVM>().ReadReoOpen();
        }

        private void ResizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.Button btn)
            {
                switch (btn.Content.ToString())
                {
                    case "🗕":
                        WindowState = WindowState.Minimized;
                        break;
                    case "🗖":
                        if (Application.Current.MainWindow.WindowState == WindowState.Maximized)
                            Application.Current.MainWindow.WindowState = WindowState.Normal;
                        else
                            Application.Current.MainWindow.WindowState = WindowState.Maximized;
                        break;
                    case "X":
                        App.Container.GetInstance<MainWindowVM>().CloseAsync();
                        break;
                    default:
                        break;
                }
            }
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void TextArea_TextChanged(object sender, TextChangedEventArgs e)
        {
            App.Container.GetInstance<MainWindowVM>().TexChanged(true);
        }
    }
}
