using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using MusicPlayerMVVM.Common;
using System.Windows.Controls;
using Prism.Events;
using MusicPlayerMVVM.Events;
using MusicPlayerMVVM.Views;

namespace MusicPlayerMVVM.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private object _currentView;
        private double _volume = 0.5;

        private HomeViewModel _homeViewModel;
        private HipHopViewModel _hipHopViewModel;
        private RockViewModel _rockViewModel;
        private PopViewModel _popViewModel;
        private KlassikViewModel _klassikViewModel;
        private JazzViewModel _jazzViewModel;
        private CustomViewModel _customViewModel;

        public MainViewModel(IEventAggregator eventAggregator) : base(eventAggregator)
        {
            // Alle ViewModels beim Start einmalig erzeugen und den EventAggregator injizieren
            _homeViewModel = new HomeViewModel(eventAggregator);
            _hipHopViewModel = new HipHopViewModel(eventAggregator);
            _rockViewModel = new RockViewModel(eventAggregator);
            _popViewModel = new PopViewModel(eventAggregator);
            _klassikViewModel = new KlassikViewModel(eventAggregator);
            _jazzViewModel = new JazzViewModel(eventAggregator);
            _customViewModel = new CustomViewModel(eventAggregator);

            // Auf Navigation hören
            EventAggregator.GetEvent<NavigationEvent>().Subscribe(NavigateToViewModel);

            CurrentView = _homeViewModel;
        }

        public object CurrentView
        {
            get => _currentView;
            set { _currentView = value; OnPropertyChanged(nameof(CurrentView)); }
        }

        public double Volume
        {
            get => _volume;
            set
            {
                if (_volume != value)
                {
                    _volume = value;
                    OnPropertyChanged(nameof(Volume));
                    EventAggregator.GetEvent<VolumeChangedEvent>().Publish(_volume);
                }
            }
        }

        private void NavigateToViewModel(string targetViewName)
        {
            switch (targetViewName)
            {
                case "HomeView":
                    CurrentView = _homeViewModel;
                    break;
                case "HipHopView":
                    CurrentView = _hipHopViewModel;
                    break;
                case "RockView":
                    CurrentView = _rockViewModel;
                    break;
                case "PopView":
                    CurrentView = _popViewModel;
                    break;
                case "KlassikView":
                    CurrentView = _klassikViewModel;
                    break;
                case "JazzView":
                    CurrentView = _jazzViewModel;
                    break;
                case "CustomView":
                    CurrentView = _customViewModel;
                    break;
            }
        }
    }
}