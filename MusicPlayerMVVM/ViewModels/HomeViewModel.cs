using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Events;
using System.Windows.Input;
using MusicPlayerMVVM.Common;
using MusicPlayerMVVM.Events;

namespace MusicPlayerMVVM.ViewModels
{
    /// <summary>
    /// Steuert die Startseite (Genre-Auswahl) der Anwendung. 
    /// Kommuniziert Navigationswünsche über den zentralen EventAggregator.
    /// </summary>
    public class HomeViewModel : ViewModelBase
    {
        /// <summary>
        /// Initialisiert das HomeViewModel und richtet die Navigations-Befehle ein.
        /// </summary>
        /// <param name="eventAggregator">Die Instanz des Prism EventAggregators.</param>
        public HomeViewModel(IEventAggregator eventAggregator) : base(eventAggregator)
        {
            // Initialisierung des Commands für die Klicks auf die Genre-Kacheln/Buttons
            SelectGenreCommand = new ActionCommand(SelectGenreExecute, param => true);
        }

        /// <summary>
        /// Holt den Befehl, der bei der Auswahl eines Genres ausgeführt wird.
        /// </summary>
        public ICommand SelectGenreCommand { get; }

        /// <summary>
        /// Verarbeitet den Klick auf ein Genre und sendet einen Navigationsbefehl an das System.
        /// </summary>
        /// <param name="parameter">Der Name des Genres (z. B. "HipHop", "Rock"), der aus dem XAML übergeben wird.</param>
        private void SelectGenreExecute(object parameter)
        {
            if (parameter is string genre)
            {
                // Erstellt den Namen der Ziel-Ansicht dynamisch (z.B. "HipHop" -> "HipHopView")
                string targetViewName = genre + "View";

                // Das MainViewModel hat dieses Event abonniert und übernimmt das eigentliche Umschalten.
                EventAggregator.GetEvent<NavigationEvent>().Publish(targetViewName);
            }
        }
    }
}