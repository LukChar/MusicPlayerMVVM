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
    /// Abstrakte Basisklasse für alle ViewModels der Anwendung.
    /// Erbt von NotifyPropertyChanged für das UI-Data-Binding und stellt den systemweiten EventAggregator bereit.
    /// </summary>
    /// <remarks>
    /// Diese Klasse dient als Fundament der MVVM-Architektur im Projekt. 
    /// Sie zentralisiert die Bereitstellung des EventAggregators zur losen Kopplung und Kommunikation zwischen verschiedenen ViewModels.
    /// </remarks>
    public abstract class ViewModelBase : NotifyPropertyChanged
    {
        /// <summary>
        /// Initialisiert eine neue Instanz der <see cref="ViewModelBase"/>-Klasse und weist den zentralen Ereignisdienst zu.
        /// </summary>
        /// <param name="eventAggregator">Die Instanz des Prism EventAggregators für systemweites Messaging (Publish/Subscribe).</param>
        public ViewModelBase(IEventAggregator eventAggregator)
        {
            EventAggregator = eventAggregator;
        }

        /// <summary>
        /// Ruft den systemweiten Ereignisdienst ab, der von abgeleiteten ViewModels genutzt wird, um auf Ereignisse zu reagieren oder diese auszulösen.
        /// </summary>
        protected IEventAggregator EventAggregator { get; }
    }
}