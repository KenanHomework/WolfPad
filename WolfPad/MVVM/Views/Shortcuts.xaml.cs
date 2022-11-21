using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using WolfPad.MVVM.Models;

namespace WolfPad.MVVM.Views
{
    /// <summary>
    /// Interaction logic for Shortcuts.xaml
    /// </summary>
    public partial class Shortcuts : Window
    {
        public Shortcuts()
        {
            InitializeComponent();
            DataContext = this;
            ShortcutInfos.Add(new ShortcutInfo() { Name = "New", Gesture = "CTRL + N" });
            ShortcutInfos.Add(new ShortcutInfo() { Name = "Open", Gesture = "CTRL + O" });
            ShortcutInfos.Add(new ShortcutInfo() { Name = "Save", Gesture = "CTRL + S" });
            ShortcutInfos.Add(new ShortcutInfo() { Name = "Save as", Gesture = "CTRL + SHIFT + S" });
            ShortcutInfos.Add(new ShortcutInfo() { Name = "Save to reopen", Gesture = "CTRL + SHIFT + ALT + S" });
            ShortcutInfos.Add(new ShortcutInfo() { Name = "Remove from reopen", Gesture = "CTRL + SHIFT + ALT + X" });
            ShortcutInfos.Add(new ShortcutInfo() { Name = "Exit", Gesture = "CTRL + F4" });
            ShortcutInfos.Add(new ShortcutInfo() { Name = "Shortcuts", Gesture = "CTRL + SHIFT + P" });
            ShortcutInfos.Add(new ShortcutInfo() { Name = "AutoComplete", Gesture = "CTRL + SPACE" });
        }

        public ObservableCollection<ShortcutInfo> ShortcutInfos { get; set; } = new ObservableCollection<ShortcutInfo>();

        private void ResizeButton_Click(object sender, RoutedEventArgs e) => this.Close();

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }
    }
}
