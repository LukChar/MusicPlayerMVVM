using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Events;
using MusicPlayerMVVM.Models;

namespace MusicPlayerMVVM.Events
{
    public class AddSongEvent : PubSubEvent<Song>
    {
    }
}
