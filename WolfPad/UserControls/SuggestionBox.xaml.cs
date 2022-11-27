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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WolfPad.MVVM.Models;

namespace WolfPad.UserControls
{
    /// <summary>
    /// Interaction logic for SuggestionBox.xaml
    /// </summary>
    public partial class SuggestionBox : UserControl
    {
        public SuggestionBox(string data) : this()
        {
            InitializeComponent();
            ReadWords(data);
        }

        public SuggestionBox()
        {
            InitializeComponent();
        }

        public void ReadWords(string data)
        {
            //var templist = data.Split(' ').ToList();

            //templist.ForEach(item => { Words.Add(new SuggestionWord() { Word = item }); });

            Words = new ObservableCollection<SuggestionWord>() { 
                new SuggestionWord() { Word="Salam"},
                new SuggestionWord() { Word="Dagol"},
                new SuggestionWord() { Word="sdad"}
            };

            WordListView.ItemsSource = null;
            WordListView.ItemsSource = Words;
        }

        public ObservableCollection<SuggestionWord> Words { get; set; } = new ObservableCollection<SuggestionWord>();

    }
}
