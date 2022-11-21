using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;

namespace WolfPad.Services
{
    public abstract class MessageBoxService
    {
        public static Wpf.Ui.Controls.MessageBox GetStandartMessageBox(string text, string title)
        {
            var message = new Wpf.Ui.Controls.MessageBox()
            {
                Title = title,
                Content = text,
                ButtonLeftName = "Ok",
                ButtonRightAppearance = ControlAppearance.Transparent,
                ButtonRightName = "Close",
            };

            message.ButtonRightClick += new RoutedEventHandler((o, e) => { message.Close(); });
            message.ButtonLeftClick += new RoutedEventHandler((o, e) => { message.Close(); });


            return message;
        }

        public static Wpf.Ui.Controls.MessageBox GetSuccesMessageBox(string text, string title)
        {
            var message = new Wpf.Ui.Controls.MessageBox()
            {
                Title = title,
                Content = text,
                ButtonLeftAppearance = ControlAppearance.Success,
                ButtonLeftName = "Ok",
                ButtonRightAppearance = ControlAppearance.Transparent,
                ButtonRightName = "Close",
            };

            message.ButtonRightClick += new RoutedEventHandler((o, e) => { message.Close(); });
            message.ButtonLeftClick += new RoutedEventHandler((o, e) => { message.Close(); });


            return message;
        }

        public static Wpf.Ui.Controls.MessageBox GetQuestionMessageBox(string text, string title, ref bool result)
        {
            var message = new Wpf.Ui.Controls.MessageBox()
            {
                Title = title,
                Content = text,
                ButtonLeftName = "Ok",
                ButtonRightAppearance = ControlAppearance.Transparent,
                ButtonRightName = "Close",
            };

            message.ButtonRightClick += new RoutedEventHandler((o, e) => { message.Close(); });
            message.ButtonLeftClick += new RoutedEventHandler((o, e) => { message.Close(); });


            return message;
        }
    }
}
