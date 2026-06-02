using System;
using System.Collections.ObjectModel;
using System.IO;

namespace MusicPlayerMVVM.Models
{
    public abstract class BasePlaylist
    {
        public string PlaylistName { get; set; }
        public ObservableCollection<Song> TrackList { get; set; } = new ObservableCollection<Song>();

        protected BasePlaylist(string name)
        {
            PlaylistName = name;
        }

        public bool ValidateTrackPath(Song song)
        {
            if (song == null) return false;

            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            string fullPath = Path.Combine(basePath, song.FilePath);

            return File.Exists(fullPath);
        }
    }
}