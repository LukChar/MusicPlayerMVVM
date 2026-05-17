using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Events;
using System.Windows.Input;
using MusicPlayerMVVM.Common;

namespace MusicPlayerMVVM.ViewModels
{
    public class HomeViewModel : NotifyPropertyChanged
    {
        private readonly MainViewModel _mainViewModel;

        public HomeViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            SelectGenreCommand = new ActionCommand(SelectGenreExecute, param => true);
        }

        public ICommand SelectGenreCommand { get; }

        private void SelectGenreExecute(object parameter)
        {
            if (parameter is string genre)
            {
                switch (genre)
                {
                    case "HipHop":
                        _mainViewModel.CurrentView = new HipHopViewModel(_mainViewModel);
                        break;
                    case "Rock":
                        _mainViewModel.CurrentView = new RockViewModel(_mainViewModel);
                        break;
                    case "Pop":
                        _mainViewModel.CurrentView = new PopViewModel(_mainViewModel);
                        break;
                    case "Klassik":
                        _mainViewModel.CurrentView = new KlassikViewModel(_mainViewModel);
                        break;
                    case "Jazz":
                        _mainViewModel.CurrentView = new JazzViewModel(_mainViewModel);
                        break;
                    case "Custom":
                        _mainViewModel.CurrentView = new CustomViewModel(_mainViewModel);
                        break;
                }
            }
        }
    }
}