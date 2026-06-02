using System;
using System.Windows;

namespace MusicPlayerMVVM.Models
{
    public class MusicPlayer : BasePlaylist
    {
        public double CurrentVolume { get; set; } = 0.5;

        public MusicPlayer(string genreName) : base(genreName)
        {
        }

        public int GetTotalTracksCount()
        {
            return TrackList.Count;
        }

        public void PrintPlaylistDiagnostic()
        {
            try
            {
                foreach (var song in TrackList)
                {
                    bool isAvailable = ValidateTrackPath(song);
                    System.Diagnostics.Debug.WriteLine($"Song: {song.Title} - Verfügbar: {isAvailable}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler bei der Playlist-Diagnose: {ex.Message}");
            }
        }
    }
}