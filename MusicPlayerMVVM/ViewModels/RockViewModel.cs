using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using MusicPlayerMVVM.Common;
using MusicPlayerMVVM.Data;
using MusicPlayerMVVM.Models;
using Prism.Events;
using MusicPlayerMVVM.Events;

namespace MusicPlayerMVVM.ViewModels
{
    /// <summary>
    /// Steuert die Geschäftslogik der Rock-Wiedergabeliste und verarbeitet eingehende Datenereignisse.
    /// </summary>
    public class RockViewModel : ViewModelBase
    {
        private MediaPlayer _mediaPlayer;
        private ObservableCollection<Song> _songs;
        private Song _selectedSong;
        private double _volume;
        private bool _isPlaying;

        /// <summary>
        /// Initialisiert das RockViewModel, konfiguriert Steuerungskommandos und abonniert Datenereignisse.
        /// </summary>
        /// <param name="eventAggregator">Die Instanz des Prism EventAggregators für systemweites Messaging.</param>
        public RockViewModel(IEventAggregator eventAggregator) : base(eventAggregator)
        {
            _mediaPlayer = new MediaPlayer();
            _songs = new ObservableCollection<Song>();
            _volume = 0.5;

            PlayPauseCommand = new ActionCommand(PlayPauseExecute, CanExecuteWithSelection);
            PreviousSongCommand = new ActionCommand(PreviousSongExecute, CanExecuteWithSelection);
            NextSongCommand = new ActionCommand(NextSongExecute, CanExecuteWithSelection);
            BackToHomeCommand = new ActionCommand(BackToHomeExecute, param => true);

            EventAggregator.GetEvent<AddSongEvent>().Subscribe(AddSong);

            EventAggregator.GetEvent<VolumeChangedEvent>().Subscribe(neueLautstaerke => Volume = neueLautstaerke);

            LoadSongsFromDatabase();
        }

        /// <summary>Holt oder setzt die bindbare Auflistung der Musiktitel.</summary>
        public ObservableCollection<Song> Songs
        {
            get => _songs;
            set { _songs = value; OnPropertyChanged(nameof(Songs)); }
        }

        /// <summary>Holt oder setzt den aktuell selektierten Musiktitel.</summary>
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

        /// <summary>Holt oder setzt die Systemlautstärke der Audiokomponente.</summary>
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

        /// <summary>Ruft den Befehl für die Play/Pause-Interaktion ab.</summary>
        public ICommand PlayPauseCommand { get; }

        /// <summary>Ruft den Befehl für den Wechsel zum vorherigen Titel ab.</summary>
        public ICommand PreviousSongCommand { get; }

        /// <summary>Ruft den Befehl für den Wechsel zum nächsten Titel ab.</summary>
        public ICommand NextSongCommand { get; }

        /// <summary>Ruft den Befehl für die Rückkehr zur Startseite ab.</summary>
        public ICommand BackToHomeCommand { get; }

        private bool CanExecuteWithSelection(object parameter) => SelectedSong != null;

        private void AddSong(Song song)
        {
            if (song != null) Songs.Add(song);
        }

        /// <summary>
        /// Initialisiert eine synchrone Verbindung zur SQL-Datenbank über das Entity Framework Core 
        /// und lädt die Rock-Titelliste.
        /// </summary>
        private void LoadSongsFromDatabase()
        {
            try
            {
                Songs.Clear();

                using (var context = new MusicDbContext())
                {
                    var rockEntities = context.RockSongs.ToList();

                    foreach (var song in rockEntities)
                    {
                        Songs.Add(song);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Laden von Rock:\n{ex.Message}", "Datenbank-Diagnose", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PlayPauseExecute(object parameter)
        {
            if (SelectedSong == null) return;
            if (_isPlaying) { _mediaPlayer.Pause(); _isPlaying = false; }
            else { PlaySong(SelectedSong); }
        }

        private void PreviousSongExecute(object parameter)
        {
            int index = Songs.IndexOf(SelectedSong);
            if (index > 0) { SelectedSong = Songs[index - 1]; PlaySong(SelectedSong); }
        }

        private void NextSongExecute(object parameter)
        {
            int index = Songs.IndexOf(SelectedSong);
            if (index < Songs.Count - 1) { SelectedSong = Songs[index + 1]; PlaySong(SelectedSong); }
        }

        private void BackToHomeExecute(object parameter)
        {
            _mediaPlayer.Stop();
            _isPlaying = false;
            EventAggregator.GetEvent<NavigationEvent>().Publish("HomeView");
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