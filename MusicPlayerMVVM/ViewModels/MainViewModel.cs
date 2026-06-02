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
            _homeViewModel = new HomeViewModel(eventAggregator);
            _hipHopViewModel = new HipHopViewModel(eventAggregator);
            _rockViewModel = new RockViewModel(eventAggregator);
            _popViewModel = new PopViewModel(eventAggregator);
            _klassikViewModel = new KlassikViewModel(eventAggregator);
            _jazzViewModel = new JazzViewModel(eventAggregator);
            _customViewModel = new CustomViewModel(eventAggregator);

            EventAggregator.GetEvent<NavigationEvent>().Subscribe(NavigateToViewModel);

            GlobalPlayPauseCommand = new ActionCommand(ExecuteGlobalPlayPause, param => true);
            GlobalPreviousCommand = new ActionCommand(ExecuteGlobalPrevious, param => true);
            GlobalNextCommand = new ActionCommand(ExecuteGlobalNext, param => true);

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

        public ICommand GlobalPlayPauseCommand { get; }
        public ICommand GlobalPreviousCommand { get; }
        public ICommand GlobalNextCommand { get; }

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

        private void ExecuteGlobalPlayPause(object parameter)
        {
            if (CurrentView is HipHopViewModel v1) v1.PlayPauseCommand.Execute(null);
            else if (CurrentView is RockViewModel v2) v2.PlayPauseCommand.Execute(null);
            else if (CurrentView is PopViewModel v3) v3.PlayPauseCommand.Execute(null);
            else if (CurrentView is JazzViewModel v4) v4.PlayPauseCommand.Execute(null);
            else if (CurrentView is KlassikViewModel v5) v5.PlayPauseCommand.Execute(null);
            else if (CurrentView is CustomViewModel v6) v6.PlayPauseCommand.Execute(null);
        }

        private void ExecuteGlobalPrevious(object parameter)
        {
            if (CurrentView is HipHopViewModel v1) v1.PreviousSongCommand.Execute(null);
            else if (CurrentView is RockViewModel v2) v2.PreviousSongCommand.Execute(null);
            else if (CurrentView is PopViewModel v3) v3.PreviousSongCommand.Execute(null);
            else if (CurrentView is JazzViewModel v4) v4.PreviousSongCommand.Execute(null);
            else if (CurrentView is KlassikViewModel v5) v5.PreviousSongCommand.Execute(null);
            else if (CurrentView is CustomViewModel v6) v6.PreviousSongCommand.Execute(null);
        }

        private void ExecuteGlobalNext(object parameter)
        {
            if (CurrentView is HipHopViewModel v1) v1.NextSongCommand.Execute(null);
            else if (CurrentView is RockViewModel v2) v2.NextSongCommand.Execute(null);
            else if (CurrentView is PopViewModel v3) v3.NextSongCommand.Execute(null);
            else if (CurrentView is JazzViewModel v4) v4.NextSongCommand.Execute(null);
            else if (CurrentView is KlassikViewModel v5) v5.NextSongCommand.Execute(null);
            else if (CurrentView is CustomViewModel v6) v6.NextSongCommand.Execute(null);
        }
    }
}