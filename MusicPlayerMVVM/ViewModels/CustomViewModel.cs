using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;
using MusicPlayerMVVM.Common;
using MusicPlayerMVVM.Models;
using Prism.Events;
using MusicPlayerMVVM.Events;
using MusicPlayerMVVM.Data;

namespace MusicPlayerMVVM.ViewModels
{
    /// <summary>
    /// Steuert die eigene Playlist. Hier können neue Lieder von Hand eingetragen, 
    /// gelöscht und abgespielt werden. Alles wird dauerhaft in der Datenbank gespeichert.
    /// </summary>
    public class CustomViewModel : ViewModelBase
    {
        private MediaPlayer _mediaPlayer;
        private ObservableCollection<Song> _songs;
        private Song _selectedSong;
        private double _volume;
        private bool _isPlaying;

        // Felder für die Datenerfassung über die UI
        private string _newTitle;
        private string _newArtist;
        private string _newFilePath;
        private string _newDuration = "00:00";

        /// <summary>
        /// Startet das ViewModel, richtet die Knöpfe (Commands) ein und lädt die Lieder aus der Datenbank.
        /// </summary>
        public CustomViewModel(IEventAggregator eventAggregator) : base(eventAggregator)
        {
            _mediaPlayer = new MediaPlayer();
            _songs = new ObservableCollection<Song>();
            _volume = 0.5;

            PlayPauseCommand = new ActionCommand(PlayPauseExecute, CanExecuteWithSelection);
            PreviousSongCommand = new ActionCommand(PreviousSongExecute, CanExecuteWithSelection);
            NextSongCommand = new ActionCommand(NextSongExecute, CanExecuteWithSelection);
            BackToHomeCommand = new ActionCommand(BackToHomeExecute, param => true);

            // Command für das Hinzufügen nutzt jetzt die Prüfung, ob alle Felder ausgefüllt sind
            AddSongCommand = new ActionCommand(AddSongExecute, CanAddSongExecute);
            DeleteSongCommand = new ActionCommand(DeleteSongExecute, CanExecuteWithSelection);

            // Command für den Datei-Explorer
            BrowseFileCommand = new ActionCommand(BrowseFileExecute, param => true);

            EventAggregator.GetEvent<AddSongEvent>().Subscribe(OnSongAdded);
            EventAggregator.GetEvent<VolumeChangedEvent>().Subscribe(neueLautstaerke => Volume = neueLautstaerke);
            EventAggregator.GetEvent<PlayControlEvent>().Subscribe(OnGlobalPlayControl);

            LoadSongsFromDatabase();
        }

        /// <summary>Liste aller Lieder, die in der Ansicht angezeigt werden.</summary>
        public ObservableCollection<Song> Songs
        {
            get => _songs;
            set { _songs = value; OnPropertyChanged(nameof(Songs)); }
        }

        /// <summary>Das aktuell ausgewählte Lied. Startet automatisch, wenn der Player gerade läuft.</summary>
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

        /// <summary>Die aktuelle Lautstärke des Players.</summary>
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

        /// <summary>Der eingegebene Titel für das neue Lied.</summary>
        public string NewTitle
        {
            get => _newTitle;
            set { _newTitle = value; OnPropertyChanged(nameof(NewTitle)); }
        }

        /// <summary>Der eingegebene Interpret für das neue Lied.</summary>
        public string NewArtist
        {
            get => _newArtist;
            set { _newArtist = value; OnPropertyChanged(nameof(NewArtist)); }
        }

        /// <summary>Der Dateipfad zum neuen Lied.</summary>
        public string NewFilePath
        {
            get => _newFilePath;
            set { _newFilePath = value; OnPropertyChanged(nameof(NewFilePath)); }
        }

        /// <summary>Die eingegebene Dauer des neuen Liedes.</summary>
        public string NewDuration
        {
            get => _newDuration;
            set { _newDuration = value; OnPropertyChanged(nameof(NewDuration)); }
        }

        /// <summary>Knopf für Start und Pause.</summary>
        public ICommand PlayPauseCommand { get; }

        /// <summary>Knopf für das vorherige Lied.</summary>
        public ICommand PreviousSongCommand { get; }

        /// <summary>Knopf für das nächste Lied.</summary>
        public ICommand NextSongCommand { get; }

        /// <summary>Knopf, um zurück zum Hauptmenü zu gehen.</summary>
        public ICommand BackToHomeCommand { get; }

        /// <summary>Knopf, um das eingetragene Lied zu speichern.</summary>
        public ICommand AddSongCommand { get; }

        /// <summary>Knopf, um das ausgewählte Lied zu löschen.</summary>
        public ICommand DeleteSongCommand { get; }

        /// <summary>Knopf, um den Windows-Dateiexplorer zu öffnen.</summary>
        public ICommand BrowseFileCommand { get; }

        /// <summary>Prüft, ob ein Lied ausgewählt ist, damit die Knöpfe aktiviert werden.</summary>
        private bool CanExecuteWithSelection(object parameter) => SelectedSong != null;

        /// <summary>Prüft, ob alle Pflichtfelder für ein neues Lied ausgefüllt sind.</summary>
        private bool CanAddSongExecute(object parameter)
        {
            return !string.IsNullOrWhiteSpace(NewTitle) &&
                   !string.IsNullOrWhiteSpace(NewArtist) &&
                   !string.IsNullOrWhiteSpace(NewFilePath);
        }

        /// <summary>Fügt das Lied zur Liste hinzu, wenn es von woanders gemeldet wird.</summary>
        private void OnSongAdded(Song newSong)
        {
            if (newSong != null && !Songs.Contains(newSong))
            {
                Songs.Add(newSong);
            }
        }

        /// <summary>Öffnet den Windows-Dateiexplorer zur Auswahl einer Audiodatei.</summary>
        private void BrowseFileExecute(object parameter)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Audio Dateien (*.wav;*.mp3)|*.wav;*.mp3",
                Title = "Audiodatei auswählen",
                Multiselect = false
            };

            if (openFileDialog.ShowDialog() == true)
            {
                NewFilePath = openFileDialog.FileName;

                if (string.IsNullOrWhiteSpace(NewTitle))
                {
                    NewTitle = Path.GetFileNameWithoutExtension(openFileDialog.FileName);
                }
            }
        }

        /// <summary>Speichert die eingegebenen Daten als neues Lied in der Datenbank ab.</summary>
        private void AddSongExecute(object parameter)
        {
            var newSong = new CustomSong
            {
                Title = NewTitle,
                Artist = NewArtist,
                FilePath = NewFilePath,
                Duration = string.IsNullOrWhiteSpace(NewDuration) ? "00:00" : NewDuration
            };

            try
            {
                using (var context = new MusicDbContext())
                {
                    context.Set<CustomSong>().Add(newSong);
                    context.SaveChanges();
                }

                // Fügt das Lied zur Anzeige hinzu
                Songs.Add(newSong);
                EventAggregator.GetEvent<AddSongEvent>().Publish(newSong);

                // Felder nach dem Speichern wieder leeren
                NewTitle = string.Empty;
                NewArtist = string.Empty;
                NewFilePath = string.Empty;
                NewDuration = "00:00";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Speichern des Titels:\n{ex.Message}", "Datenbankfehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>Löscht das ausgewählte Lied aus der Liste und aus der Datenbank.</summary>
        private void DeleteSongExecute(object parameter)
        {
            if (SelectedSong != null)
            {
                var songToDelete = SelectedSong;

                if (_isPlaying && _mediaPlayer.Source?.LocalPath == songToDelete.FilePath)
                {
                    _mediaPlayer.Stop();
                    _isPlaying = false;
                }

                Songs.Remove(songToDelete);

                try
                {
                    using (var context = new MusicDbContext())
                    {
                        var entity = context.Set<CustomSong>().Find(songToDelete.Id);
                        if (entity != null)
                        {
                            context.Set<CustomSong>().Remove(entity);
                            context.SaveChanges();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Fehler beim Löschen des Titels:\n{ex.Message}", "Datenbankfehler", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>Holt beim Start alle eigenen Lieder aus der Datenbank.</summary>
        private void LoadSongsFromDatabase()
        {
            try
            {
                Songs.Clear();

                using (var context = new MusicDbContext())
                {
                    var customEntities = context.Set<CustomSong>().ToList();

                    foreach (var songEntity in customEntities)
                    {
                        Songs.Add(songEntity);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Laden der eigenen Playlist:\n{ex.Message}", "Datenbankfehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>Startet oder pausiert das Lied.</summary>
        private void PlayPauseExecute(object parameter)
        {
            if (SelectedSong == null) return;
            if (_isPlaying) { _mediaPlayer.Pause(); _isPlaying = false; }
            else { PlaySong(SelectedSong); }
        }

        /// <summary>Geht zum vorherigen Lied in der Liste.</summary>
        private void PreviousSongExecute(object parameter)
        {
            int index = Songs.IndexOf(SelectedSong);
            if (index > 0) { SelectedSong = Songs[index - 1]; PlaySong(SelectedSong); }
        }

        /// <summary>Geht zum nächsten Lied in der Liste.</summary>
        private void NextSongExecute(object parameter)
        {
            int index = Songs.IndexOf(SelectedSong);
            if (index < Songs.Count - 1) { SelectedSong = Songs[index + 1]; PlaySong(SelectedSong); }
        }

        /// <summary>Stoppt die Musik und geht zur Startseite zurück.</summary>
        private void BackToHomeExecute(object parameter)
        {
            _mediaPlayer.Stop();
            _isPlaying = false;
            EventAggregator.GetEvent<NavigationEvent>().Publish("HomeView");
        }

        /// <summary>Sucht die Musikdatei auf dem PC und spielt sie ab.</summary>
        private void PlaySong(Song song)
        {
            if (string.IsNullOrWhiteSpace(song.FilePath)) return;

            // Entfernt doppelte Anführungszeichen und Zeilenumbrüche aus der Eingabe
            string cleanPath = song.FilePath.Trim('"', ' ', '\r', '\n');

            string fullPath = Path.IsPathRooted(cleanPath)
                ? cleanPath
                : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, cleanPath);

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
                MessageBox.Show($"Dateipfad konnte nicht gefunden werden:\n{fullPath}", "Wiedergabefehler", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        /// <summary>Reagiert auf Klicks der globalen Steuerungs-Knöpfe im Hauptfenster.</summary>
        private void OnGlobalPlayControl(string command)
        {
            switch (command)
            {
                case "PlayPause":
                    if (PlayPauseCommand.CanExecute(null)) PlayPauseCommand.Execute(null);
                    break;
                case "Previous":
                    if (PreviousSongCommand.CanExecute(null)) PreviousSongCommand.Execute(null);
                    break;
                case "Next":
                    if (NextSongCommand.CanExecute(null)) NextSongCommand.Execute(null);
                    break;
            }
        }
    }
}