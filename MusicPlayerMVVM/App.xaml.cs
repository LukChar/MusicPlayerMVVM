using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using MusicPlayerMVVM.ViewModels;
using MusicPlayerMVVM.Views;
using Prism.Events;

namespace MusicPlayerMVVM
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            IEventAggregator eventAggregator = new EventAggregator();

            // 1. Hauptfenster erstellen
            MainWindow mainWindow = new MainWindow();

            // 2. Das MainViewModel erstellen
            MainViewModel mainViewModel = new MainViewModel(eventAggregator);

            // 3. Das ViewModel als Datenkontext an das Fenster hängen
            mainWindow.DataContext = mainViewModel;

            // 4. Fenster anzeigen
            mainWindow.Show();
        }
    }
}