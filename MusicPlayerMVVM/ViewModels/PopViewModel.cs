using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using MusicPlayerMVVM.Common;
using MusicPlayerMVVM.Models;

namespace MusicPlayerMVVM.ViewModels
{
    public class PopViewModel : NotifyPropertyChanged
    {
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["MusicDB"].ConnectionString;
        private readonly MediaPlayer _mediaPlayer;
        private ObservableCollection<Song> _songs;
        private Song _selectedSong;
        private double _volume;
        private bool _isPlaying;

        public PopViewModel(MainViewModel mainViewModel)
        {
            MainViewModel = mainViewModel;
            _mediaPlayer = new MediaPlayer();
            _songs = new ObservableCollection<Song>();
            _volume = 0.5;

            PlayPauseCommand = new ActionCommand(PlayPauseExecute, CanExecuteWithSelection);
            PreviousSongCommand = new ActionCommand(PreviousSongExecute, CanExecuteWithSelection);
            NextSongCommand = new ActionCommand(NextSongExecute, CanExecuteWithSelection);

            LoadSongsFromDatabase();
        }

        public MainViewModel MainViewModel { get; }

        public ObservableCollection<Song> Songs
        {
            get => _songs;
            set { _songs = value; OnPropertyChanged(nameof(Songs)); }
        }

        public Song SelectedSong
        {
            get => _selectedSong;
            set
            {
                _selectedSong = value;
                OnPropertyChanged(nameof(SelectedSong));
                if (_selectedSong != null && _isPlaying)
                {
                    PlaySong(_selectedSong);
                }
            }
        }

        public double Volume
        {
            get => _volume;
            set
            {
                _volume = value;
                OnPropertyChanged(nameof(Volume));
                _mediaPlayer.Volume = _volume;
            }
        }

        public ICommand PlayPauseCommand { get; }
        public ICommand PreviousSongCommand { get; }
        public ICommand NextSongCommand { get; }

        private bool CanExecuteWithSelection(object parameter)
        {
            return SelectedSong != null;
        }

        private void LoadSongsFromDatabase()
        {
            Songs.Clear();
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();
                string query = "SELECT * FROM Songs_Pop";
                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Songs.Add(new Song
                    {
                        Id = reader.GetInt32(0),
                        Title = reader.GetString(1),
                        Artist = reader.GetString(2),
                        Duration = reader.GetString(3),
                        FilePath = reader.GetString(4)
                    });
                }
            }
        }

        private void PlayPauseExecute(object parameter)
        {
            if (SelectedSong == null) return;

            if (_isPlaying)
            {
                _mediaPlayer.Pause();
                _isPlaying = false;
            }
            else
            {
                PlaySong(SelectedSong);
            }
        }

        private void PreviousSongExecute(object parameter)
        {
            int index = Songs.IndexOf(SelectedSong);
            if (index > 0)
            {
                SelectedSong = Songs[index - 1];
                PlaySong(SelectedSong);
            }
        }

        private void NextSongExecute(object parameter)
        {
            int index = Songs.IndexOf(SelectedSong);
            if (index < Songs.Count - 1)
            {
                SelectedSong = Songs[index + 1];
                PlaySong(SelectedSong);
            }
        }

        private void PlaySong(Song song)
        {
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            string fullPath = Path.Combine(basePath, song.FilePath);

            if (File.Exists(fullPath))
            {
                _mediaPlayer.Stop();
                _mediaPlayer.Open(new Uri(fullPath, UriKind.Absolute));
                _mediaPlayer.Volume = Volume;
                _mediaPlayer.Play();
                _isPlaying = true;
            }
            else
            {
                MessageBox.Show($"Dateipfad nicht gefunden:\n{fullPath}");
            }
        }
    }
}