using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using WolfPad.MVVM.Models;
using WolfPad.MVVM.ViewModels;

namespace WolfPad
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        #region Members

        public static Container Container = new Container();

        public static AppConfig AppConfig = new AppConfig();

        #endregion

        #region Methods

        void Register()
        {
            Container.RegisterSingleton<MainWindowVM>();

            Container.Verify();
        }

        #endregion


        public App()
        {
            Register();
            AppConfig.Read();
        }

    }
}
