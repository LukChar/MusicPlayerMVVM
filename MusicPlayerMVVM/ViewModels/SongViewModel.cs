using MusicPlayerMVVM.Models;
using MusicPlayerMVVM.Common;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace MusicPlayerMVVM.ViewModels
{
    public class SongViewModel : NotifyPropertyChanged
    {
        private ObservableCollection<Song> _songs;
        private Song _selectedSong;
        private double _volume = 0.5;
        private bool _isPlaying = false;

        private static readonly System.Windows.Controls.MediaElement MediaPlayer = new System.Windows.Controls.MediaElement
        {
            LoadedBehavior = System.Windows.Controls.MediaState.Manual,
            UnloadedBehavior = System.Windows.Controls.MediaState.Manual
        };

        public SongViewModel()
        {
            Songs = new ObservableCollection<Song>();
            LoadSongsFromDatabase();

            PlayPauseCommand = new ActionCommand(PlayPauseCommandExecute, PlayPauseCommandCanExecute);
            PreviousSongCommand = new ActionCommand(PreviousSongCommandExecute, PreviousSongCommandCanExecute);
            NextSongCommand = new ActionCommand(NextSongCommandExecute, NextSongCommandCanExecute);
            AddSongCommand = new ActionCommand(AddSongCommandExecute, AddSongCommandCanExecute);
            DeleteSongCommand = new ActionCommand(DeleteSongCommandExecute, DeleteSongCommandCanExecute);
        }

        #region ------------------------- Properties -------------------------

        public ObservableCollection<Song> Songs
        {
            get { return _songs; }
            set
            {
                _songs = value;
                OnPropertyChanged(nameof(Songs));
            }
        }

        public Song SelectedSong
        {
            get { return _selectedSong; }
            set
            {
                if (_selectedSong == value) return;
                _selectedSong = value;
                OnPropertyChanged(nameof(SelectedSong));
            }
        }

        public double Volume
        {
            get { return _volume; }
            set
            {
                if (_volume == value) return;
                _volume = value;
                MediaPlayer.Volume = _volume;
                OnPropertyChanged(nameof(Volume));
            }
        }

        #endregion

        #region ------------------------- Commands -------------------------

        public ICommand PlayPauseCommand { get; private set; }
        public ICommand PreviousSongCommand { get; private set; }
        public ICommand NextSongCommand { get; private set; }
        public ICommand AddSongCommand { get; private set; }
        public ICommand DeleteSongCommand { get; private set; }

        #endregion

        #region ------------------------- Command Execution -------------------------

        private bool PlayPauseCommandCanExecute(object parameter) => true;

        private void PlayPauseCommandExecute(object parameter)
        {
            if (SelectedSong != null)
            {
                if (_isPlaying)
                {
                    MediaPlayer.Pause();
                    _isPlaying = false;
                }
                else
                {
                    if (File.Exists(SelectedSong.FilePath))
                    {
                        MediaPlayer.Source = new Uri(SelectedSong.FilePath, UriKind.Absolute);
                        MediaPlayer.Play();
                        _isPlaying = true;
                    }
                    else
                    {
                        MessageBox.Show("Dateipfad nicht gefunden!");
                    }
                }
            }
            else
            {
                MessageBox.Show("Bitte wähle ein Lied aus.");
            }
        }

        private bool PreviousSongCommandCanExecute(object parameter) => true;

        private void PreviousSongCommandExecute(object parameter)
        {
            if (SelectedSong == null) return;
            int index = Songs.IndexOf(SelectedSong);

            if (index > 0)
            {
                SelectedSong = Songs[index - 1];
                PlaySong(SelectedSong);
            }
        }

        private bool NextSongCommandCanExecute(object parameter) => true;

        private void NextSongCommandExecute(object parameter)
        {
            if (SelectedSong == null) return;
            int index = Songs.IndexOf(SelectedSong);

            if (index < Songs.Count - 1)
            {
                SelectedSong = Songs[index + 1];
                PlaySong(SelectedSong);
            }
        }

        private bool AddSongCommandCanExecute(object parameter) => true;

        private void AddSongCommandExecute(object parameter)
        {
            Microsoft.Win32.OpenFileDialog dateiAuswahl = new Microsoft.Win32.OpenFileDialog();

            if (dateiAuswahl.ShowDialog() == true)
            {
                string filePath = dateiAuswahl.FileName;
                string fileName = Path.GetFileNameWithoutExtension(filePath);

                using (var context = new MusicDbContext())
                {
                    var newSong = new Song
                    {
                        Title = fileName,
                        Artist = "Unbekannt",
                        Duration = "00:00",
                        FilePath = filePath
                    };

                    context.Songs.Add(newSong);
                    context.SaveChanges();
                }

                LoadSongsFromDatabase();
            }
        }

        private bool DeleteSongCommandCanExecute(object parameter) => true;

        private void DeleteSongCommandExecute(object parameter)
        {
            if (SelectedSong != null)
            {
                using (var context = new MusicDbContext())
                {
                    var songToDelete = context.Songs.FirstOrDefault(s => s.Id == SelectedSong.Id);
                    if (songToDelete != null)
                    {
                        context.Songs.Remove(songToDelete);
                        context.SaveChanges();
                    }
                }

                LoadSongsFromDatabase();
            }
            else
            {
                MessageBox.Show("Bitte wähle ein Lied aus.");
            }
        }

        #endregion

        #region ------------------------- Helper Methods -------------------------

        private void LoadSongsFromDatabase()
        {
            Songs.Clear();

            using (var context = new MusicDbContext())
            {
                var dbSongs = context.Songs.ToList();
                foreach (var song in dbSongs)
                {
                    Songs.Add(song);
                }
            }
        }

        private void PlaySong(Song song)
        {
            if (File.Exists(song.FilePath))
            {
                MediaPlayer.Stop();
                MediaPlayer.Source = new Uri(song.FilePath, UriKind.Absolute);
                MediaPlayer.Play();
                _isPlaying = true;
            }
            else
            {
                MessageBox.Show("Dateipfad nicht gefunden!");
            }
        }

        #endregion
    }
}