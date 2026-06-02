using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Events;

namespace MusicPlayerMVVM.Events
{
    /// <summary>
    /// Repräsentiert ein systemweites Messaging-Ereignis zur synchronen Änderung der Audio-Lautstärke.
    /// </summary>
    /// <remarks>
    /// Erbt von Prisms PubSubEvent. Demonstriert das Prinzip der losen Kopplung: 
    /// Der Slider im MainViewModel sendet (Publish) den neuen Double-Wert, und alle aktiven Playlist-ViewModels 
    /// empfangen (Subscribe) diesen Wert gleichzeitig, ohne direkte Objektreferenzen aufeinander zu haben.
    /// </remarks>
    public class VolumeChangedEvent : PubSubEvent<double>
    {
    }
}
