using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using MusicPlayerMVVM.Common;

namespace MusicPlayerMVVM.ViewModels
{
    public class MainViewModel : NotifyPropertyChanged
    {
        private object _currentView;

        public MainViewModel()
        {
            // Command für die Home-Navigation bereitstellen
            NavigateHomeCommand = new ActionCommand(NavigateHomeExecute, param => true);

            // Start-View festlegen
            CurrentView = new HomeViewModel(this);
        }

        public object CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged(nameof(CurrentView));

                if (Application.Current.MainWindow != null)
                {
                    if (_currentView is HomeViewModel)
                    {
                        Application.Current.MainWindow.Width = 575;
                        Application.Current.MainWindow.Height = 600;
                    }
                    else
                    {
                        Application.Current.MainWindow.Width = 450;
                        Application.Current.MainWindow.Height = 900;
                    }
                }
            }
        }

        public ICommand NavigateHomeCommand { get; }

        private void NavigateHomeExecute(object parameter)
        {
            CurrentView = new HomeViewModel(this);
        }
    }
}