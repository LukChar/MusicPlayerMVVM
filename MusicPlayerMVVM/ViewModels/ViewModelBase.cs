using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Events;
using MusicPlayerMVVM.Common;

namespace MusicPlayerMVVM.ViewModels
{
    /// <summary>
    /// Abstrakte Basisklasse für alle ViewModels zur Bereitstellung der EventAggregator-Infrastruktur.
    /// Erbt von NotifyPropertyChanged für Datenbindung.
    /// </summary>
    public class ViewModelBase : NotifyPropertyChanged
    {
        /// <summary>
        /// Initialisiert die Basisklasse und ordnet den zentralen Ereignisdienst zu.
        /// </summary>
        /// <param name="eventAggregator">Die Instanz des Prism EventAggregators.</param>
        public ViewModelBase(IEventAggregator eventAggregator)
        {
            EventAggregator = eventAggregator;
        }

        /// <summary>
        /// Ruft den internen Ereignisdienst für abgeleitete Klassen ab.
        /// </summary>
        protected IEventAggregator EventAggregator { get; }
    }
}
