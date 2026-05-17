using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using MusicPlayerMVVM.ViewModels;
using MusicPlayerMVVM.Views;

namespace MusicPlayerMVVM
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // 1. Hauptfenster erstellen
            MainWindow mainWindow = new MainWindow();

            // 2. Das MainViewModel erstellen
            MainViewModel mainViewModel = new MainViewModel();

            // 3. Das ViewModel als Datenkontext an das Fenster hängen
            mainWindow.DataContext = mainViewModel;

            // 4. Fenster anzeigen
            mainWindow.Show();
        }
    }
}