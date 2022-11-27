using Faker;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using WolfPad.Commands;
using WolfPad.Enums;
using WolfPad.ExtensionMethods;
using WolfPad.MVVM.Models;
using WolfPad.MVVM.Views;
using WolfPad.Services;
using WolfPad.UserControls;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;
using static System.Net.Mime.MediaTypeNames;
using RelayCommand = WolfPad.Commands.RelayCommand;

namespace WolfPad.MVVM.ViewModels
{
    public class MainWindowVM
    {

        #region Members

        private string titleBAr = "Wolf Pad";

        public string TitleBarText
        {
            get { return titleBAr; }
            set { titleBAr = value; Window.TitleBar.Content = value; Window.Title = value; }
        }

        private WPState state;

        public WPState State
        {
            get { return state; }
            set { state = value; OnPropertyChanged(); }
        }

        private string path;
        private ObservableCollection<SuggestionWord> suggestionWords = new ObservableCollection<SuggestionWord>();
        private string filterWord;

        public string Path
        {
            get { return path; }
            set { path = value; }
        }

        public MainWindow Window { get; set; }

        public ObservableCollection<SuggestionWord> Words { get; set; } = new ObservableCollection<SuggestionWord>();

        public ObservableCollection<SuggestionWord> SuggestionWords
        {
            get => suggestionWords;
            set
            {
                suggestionWords = value;
                Window.WordListView.ItemsSource = value;
                try { Window.WordListView.SelectedIndex = 0; }
                catch (Exception) { }

            }
        }

        public string FilterWord
        {
            get => filterWord; set
            {
                filterWord = value;
                FilterWords();
            }
        }

        public int Index { get; set; }

        public int Length { get; set; }

        public string Data { get; set; }

        public TextPointer CaretPosition { get; set; }

        #endregion

        #region Methods

        public void ResetAutocompleteProperties()
        {
            CaretPosition = null;
            Words.Clear();
            SuggestionWords.Clear();
            FilterWord = string.Empty;
            Data = string.Empty;
            Index = 0;
            Length = 0;
        }

        public void GetFilterWord(TextChangedEventArgs e)
        {

            var change = e.Changes.First();

            string data = Window.TextArea.GetText();

            Length += change.AddedLength;
            Length -= change.RemovedLength;

            if (Length == 0)
            {
                FilterWord = string.Empty;
                return;
            }
            else if (Length < 0)
            {
                ExitAutoCompleteRun(e);
                return;
            }


            FilterWord = data.Substring(Index, Length);
        }

        public async Task CloseAsync()
        {
            if (State != WPState.Changed)
                System.Windows.Application.Current.Shutdown();

            var message = new Wpf.Ui.Controls.MessageBox()
            {
                Content = "Do you want save filse ?",
                ButtonLeftName = "Yes",
                ButtonRightAppearance = Wpf.Ui.Common.ControlAppearance.Transparent,
                ButtonRightName = "No",
            };

            bool result = false;

            message.ButtonRightClick += new RoutedEventHandler((o, e) => { message.Close(); });
            message.ButtonLeftClick += new RoutedEventHandler((o, e) => { result = true; message.Close(); });

            message.ShowDialog();

            if (result && string.IsNullOrWhiteSpace(path))
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.ShowDialog();

                if (!string.IsNullOrWhiteSpace(sfd.FileName))
                {
                    path = sfd.FileName;
                    await FileService.SaveFile(path, Window.TextArea.GetText());
                }
            }

            System.Windows.Application.Current.Shutdown();

        }

        public void TexChanged(bool changed)
        {
            TitleBarText = TextService.GetFileName(path) ?? "Wolf Pad";
            if (changed)
            {
                TitleBarText = $"*{TitleBarText.TrimStart('*')}";
                State = WPState.Changed;
            }
            else
            {
                TitleBarText = TitleBarText.TrimStart('*');
                State = WPState.Saved;
            }
        }

        public void TextChangedRun(object param) { TexChanged(true); }

        public async void ExitCurrentFile()
        {
            if (State == WPState.Changed)
            {
                bool result = false;
                var message = new Wpf.Ui.Controls.MessageBox()
                {
                    Title = "Save File",
                    Content = "Do you want save file?",
                    ButtonLeftAppearance = ControlAppearance.Info,
                    ButtonLeftName = "Yes",
                    ButtonRightAppearance = ControlAppearance.Transparent,
                    ButtonRightName = "No",
                };

                message.ButtonLeftClick += new RoutedEventHandler((o, e) => { result = true; message.Close(); });
                message.ButtonRightClick += new RoutedEventHandler((o, e) => { message.Close(); });
                message.ShowDialog();

                if (result)
                {
                    if (!string.IsNullOrWhiteSpace(Path))
                    {
                        await File.WriteAllTextAsync(Path, Window.TextArea.GetText());
                    }
                    else
                    {
                        AskSavePath();
                        await AssignPathAsync(Path);
                    }
                }
            }

            Window.TextArea.SetText(string.Empty);
            Path = string.Empty;
            TitleBarText = "Wolf Pad";
            State = WPState.Empty;
        }

        public void NewCommandRunAsync(object param) => ExitCurrentFile();

        public async void OpenCommandRun(object param)
        {
            ExitCurrentFile();

            AskOpenPath();
            await AssignPathAsync(Path, AssignPathTailMode.Read);
        }

        public async void SaveCommandRun(object param)
        {
            if (string.IsNullOrWhiteSpace(Path))
                AskSavePath();

            await AssignPathAsync(Path);

        }

        public async void SaveAsCommandRun(object param)
        {
            AskSavePath();

            await AssignPathAsync(Path);
        }

        public async void SaveToReopenRun(object param)
        {
            if (State == WPState.Empty)
                return;

            bool result = false;
            var message = new Wpf.Ui.Controls.MessageBox()
            {
                Title = "Save to Re-Open",
                Content = "Do you want save to Re-Open ?",
                ButtonLeftAppearance = ControlAppearance.Info,
                ButtonLeftName = "Yes",
                ButtonRightAppearance = ControlAppearance.Transparent,
                ButtonRightName = "No",
            };

            message.ButtonLeftClick += new RoutedEventHandler((o, e) => { result = true; message.Close(); });
            message.ButtonRightClick += new RoutedEventHandler((o, e) => { message.Close(); });
            message.ShowDialog();

            if (!result)
                return;

            if (string.IsNullOrWhiteSpace(Path))
                AskSavePath();

            App.AppConfig.ReOpenPath = Path;
            await AssignPathAsync(Path);

            App.AppConfig.Write();

            MessageBoxService.GetSuccesMessageBox("Succes to save reopen !", "Succes").ShowDialog();
        }

        public void RemoveFromReopenCommandRun(object param)
        {
            bool result = false;
            var message = new Wpf.Ui.Controls.MessageBox()
            {
                Title = "Remove Re-Open",
                Content = $"Do you want remove '{TextService.GetFileName(Path)}.txt' from re-open ?",
                ButtonLeftAppearance = ControlAppearance.Danger,
                ButtonLeftName = "Yes",
                ButtonRightAppearance = ControlAppearance.Transparent,
                ButtonRightName = "No",
            };

            message.ButtonLeftClick += new RoutedEventHandler((o, e) => { result = true; message.Close(); });
            message.ButtonRightClick += new RoutedEventHandler((o, e) => { message.Close(); });
            message.ShowDialog();

            if (!result)
                return;
            App.AppConfig.ReOpenPath = string.Empty;
            App.AppConfig.Write();
        }

        public async void ExitCommandRun(object param) => await CloseAsync();

        public void AskSavePath()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Tex File (*.txt)| *.txt";
            if (sfd.ShowDialog() == true)
                Path = sfd.FileName;
        }

        public void AskOpenPath()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Tex File (*.txt)| *.txt";
            if (ofd.ShowDialog() == true)
                Path = ofd.FileName;
        }

        public async Task AssignPathAsync(string _path, AssignPathTailMode mode = AssignPathTailMode.Write)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_path))
                    _path = string.Empty;

                path = _path;

                switch (mode)
                {
                    case AssignPathTailMode.Write:
                        await File.WriteAllTextAsync(path, Window.TextArea.GetText());
                        break;
                    case AssignPathTailMode.Read:
                        Window.TextArea.SetText(await File.ReadAllTextAsync(path));
                        State = WPState.Saved;
                        break;
                    case AssignPathTailMode.None:
                        break;
                    default:
                        break;
                }

                TexChanged(false);
            }
            catch (Exception) { }
        }

        public async Task ReadReoOpen()
        {
            if (!string.IsNullOrWhiteSpace(App.AppConfig.ReOpenPath))
            {
                await AssignPathAsync(App.AppConfig.ReOpenPath, AssignPathTailMode.Read);
            }
            else
            {
                State = WPState.Empty;
            }
        }

        public bool TextBoxAreaIsNull(object param) => State == WPState.Changed;

        public bool SaveToReopenCanRun(object param)
        {
            App.AppConfig.Read();
            return string.IsNullOrEmpty(App.AppConfig.ReOpenPath) && State != WPState.Empty;
        }

        public bool RemoveFromReopenCommandCanRun(object param)
        {
            App.AppConfig.Read();
            return !string.IsNullOrEmpty(App.AppConfig.ReOpenPath);
        }

        public void AboutRun(object param) => new About().ShowDialog();

        public void ShowShorcutRun(object param) => new Shortcuts().ShowDialog();


        public async Task ReadData()
        {
            string data = Window.TextArea.GetText();
            List<string> list = new();

            list = data.Split(' ', '\n', '\t').ToList();

            list.ForEach(x =>
            {
                if (!string.IsNullOrWhiteSpace(x))
                    Words.Add(new SuggestionWord() { Word = TextService.RemoveEscapeSequences(x) }); ;
            });
        }

        public void FilterWords()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(FilterWord))
                {
                    SuggestionWords = Words;
                    return;
                }
                SuggestionWords = new ObservableCollection<SuggestionWord>(Words.Where(w => { return w.Word.StartsWith(FilterWord); }).ToList());
            }
            catch (Exception) { }
        }

        public async void AutoCompleteRun(object param)
        {

            ResetAutocompleteProperties();

            await ReadData();

            Data = Window.TextArea.GetText();

            SuggestionWords = Words;
            Window.SuggestionBarPopup.IsOpen = true;

            CaretPosition = Window.TextArea.CaretPosition;
            Index = new TextRange(Window.TextArea.Document.ContentStart, Window.TextArea.CaretPosition).Text.Length;
        }

        public void ApplySuggestionRun(object param)
        {
            if (!Window.SuggestionBarPopup.IsOpen)
                return;

            try
            {
                //string word = TextService.RemoveEscapeSequences(((SuggestionWord)Window.WordListView.SelectedItem).Word);
                string word = ((SuggestionWord)Window.WordListView.SelectedItem).Word;
                word = $" {word} ";

                string data = Window.TextArea.GetText();

                string after = Data.Substring(0, Index);
                after += word;
                after += Data.Substring(Index);

                Window.TextArea.SetText(after);
                Window.TextArea.CaretPosition = CaretPosition;
            }
            catch (Exception) { }
            finally { ExitAutoCompleteRun(param); }
        }

        public void ExitAutoCompleteRun(object param)
        {
            ResetAutocompleteProperties();

            Window.SuggestionBarPopup.IsOpen = false;
        }

        #endregion

        #region Commands

        public RelayCommand TextChangedCommand { get; set; }
        public RelayCommand NewCommand { get; set; }
        public RelayCommand OpenCommand { get; set; }
        public RelayCommand SaveCommand { get; set; }
        public RelayCommand SaveAsCommand { get; set; }
        public RelayCommand SaveToReopenCommand { get; set; }
        public RelayCommand RemoveFromReopenCommand { get; set; }
        public RelayCommand ExitCommand { get; set; }
        public RelayCommand AboutCommand { get; set; }
        public RelayCommand ShowShorcutCommand { get; set; }

        public RelayCommand AutoCompleteCommand { get; set; }

        public RelayCommand ExitAutoCompleteCommand { get; set; }
        public RelayCommand ApplySuggestionCommand { get; set; }

        #endregion

        #region PropertyChangedEventHandler

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion

        public MainWindowVM()
        {
            TextChangedCommand = new RelayCommand(TextChangedRun);

            /* File */
            NewCommand = new RelayCommand(NewCommandRunAsync);
            OpenCommand = new RelayCommand(OpenCommandRun);
            SaveCommand = new RelayCommand(SaveCommandRun, TextBoxAreaIsNull);
            SaveAsCommand = new RelayCommand(SaveAsCommandRun, TextBoxAreaIsNull);

            /* Re-Open */
            SaveToReopenCommand = new RelayCommand(SaveToReopenRun, SaveToReopenCanRun);
            RemoveFromReopenCommand = new RelayCommand(RemoveFromReopenCommandRun, RemoveFromReopenCommandCanRun);

            /* Exit */
            ExitCommand = new RelayCommand(ExitCommandRun);

            /* Help Menu */
            AboutCommand = new RelayCommand(AboutRun);
            ShowShorcutCommand = new RelayCommand(ShowShorcutRun);

            /* Autocomplete */
            AutoCompleteCommand = new RelayCommand(AutoCompleteRun);
            ApplySuggestionCommand = new RelayCommand(ApplySuggestionRun);
            ExitAutoCompleteCommand = new RelayCommand(ExitAutoCompleteRun);

            State = WPState.Empty;

        }

    }
}
