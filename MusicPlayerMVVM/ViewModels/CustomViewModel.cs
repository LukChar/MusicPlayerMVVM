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
    /// Steuert die benutzerdefinierte Playlist, ermöglicht das lokale Hinzufügen und Löschen von Titeln 
    /// und synchronisiert diese über das Entity Framework Core mit der Datenbank.
    /// </summary>
    public class CustomViewModel : ViewModelBase
    {
        private MediaPlayer _mediaPlayer;
        private ObservableCollection<Song> _songs;
        private Song _selectedSong;
        private double _volume;
        private bool _isPlaying;

        /// <summary>
        /// Initialisiert das CustomViewModel, konfiguriert alle Steuerungskommandos und lädt persistierte Daten.
        /// </summary>
        /// <param name="eventAggregator">Die Instanz des Prism EventAggregators für systemweites Messaging.</param>
        public CustomViewModel(IEventAggregator eventAggregator) : base(eventAggregator)
        {
            _mediaPlayer = new MediaPlayer();
            _songs = new ObservableCollection<Song>();
            _volume = 0.5;

            PlayPauseCommand = new ActionCommand(PlayPauseExecute, CanExecuteWithSelection);
            PreviousSongCommand = new ActionCommand(PreviousSongExecute, CanExecuteWithSelection);
            NextSongCommand = new ActionCommand(NextSongExecute, CanExecuteWithSelection);
            BackToHomeCommand = new ActionCommand(BackToHomeExecute, param => true);

            AddSongCommand = new ActionCommand(AddSongExecute, param => true);
            DeleteSongCommand = new ActionCommand(DeleteSongExecute, CanExecuteWithSelection);

            EventAggregator.GetEvent<AddSongEvent>().Subscribe(OnSongAdded);

            EventAggregator.GetEvent<VolumeChangedEvent>().Subscribe(neueLautstaerke => Volume = neueLautstaerke);

            EventAggregator.GetEvent<PlayControlEvent>().Subscribe(OnGlobalPlayControl);


            LoadSongsFromDatabase();
        }

        /// <summary>Holt oder setzt die bindbare Auflistung der Musiktitel.</summary>
        public ObservableCollection<Song> Songs
        {
            get => _songs;
            set { _songs = value; OnPropertyChanged(nameof(Songs)); }
        }

        /// <summary>Holt oder setzt den aktuell selektierten Musiktitel und startet diesen bei aktiver Wiedergabe.</summary>
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

        /// <summary>Ruft den Befehl zum Hinzufügen einer neuen lokalen Audiodatei ab.</summary>
        public ICommand AddSongCommand { get; }

        /// <summary>Ruft den Befehl zum Löschen der aktuell ausgewählten Audiodatei ab.</summary>
        public ICommand DeleteSongCommand { get; }

        /// <summary>
        /// Validiert den Ausführungsstatus der befehlsgebundenen Aktionen basierend auf der aktuellen Selektion.
        /// </summary>
        private bool CanExecuteWithSelection(object parameter) => SelectedSong != null;

        /// <summary>
        /// Erfasst ein übermitteltes Song-Objekt und fügt dieses der aktiven Laufzeitauflistung hinzu.
        /// </summary>
        private void OnSongAdded(Song newSong)
        {
            if (newSong != null)
            {
                Songs.Add(newSong);
            }
        }

        /// <summary>
        /// Öffnet einen Dateiauswahldialog zur Selektion einer Audiodatei, erstellt eine neue Entität 
        /// und persistiert diese asynchron in der Datenbank.
        /// </summary>
        private void AddSongExecute(object parameter)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Audio Dateien (*.mp3;*.wav)|*.mp3;*.wav",
                Multiselect = false
            };

            if (openFileDialog.ShowDialog() == true)
            {
                var newSong = new CustomSong
                {
                    Title = Path.GetFileNameWithoutExtension(openFileDialog.FileName),
                    Artist = "Lokale Datei",
                    FilePath = openFileDialog.FileName,
                    Duration = "00:00"
                };

                try
                {
                    using (var context = new MusicDbContext())
                    {
                        context.Set<CustomSong>().Add(newSong);
                        context.SaveChanges();
                    }

                    // Das eigene Subscribe löst daraufhin OnSongAdded aus und fügt den Song der Liste hinzu.
                    EventAggregator.GetEvent<AddSongEvent>().Publish(newSong);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Fehler beim Speichern des Titels:\n{ex.Message}", "Datenbankfehler", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// Entfernt den selektierten Song aus der UI-Auflistung, stoppt eine eventuell laufende Wiedergabe 
        /// und löscht den Datensatz physisch aus der Datenbank.
        /// </summary>
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

        /// <summary>
        /// Liest die benutzerdefinierten Datensätze beim Initialisieren mittels Entity Framework Core aus.
        /// </summary>
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

        /// <summary>Ändert den Wiedergabezustand der Audiokomponente zwischen Aktiv und Pausiert.</summary>
        private void PlayPauseExecute(object parameter)
        {
            if (SelectedSong == null) return;
            if (_isPlaying) { _mediaPlayer.Pause(); _isPlaying = false; }
            else { PlaySong(SelectedSong); }
        }

        /// <summary>Dekrementiert den Listenindex und startet den vorherigen Titel.</summary>
        private void PreviousSongExecute(object parameter)
        {
            int index = Songs.IndexOf(SelectedSong);
            if (index > 0) { SelectedSong = Songs[index - 1]; PlaySong(SelectedSong); }
        }

        /// <summary>Inkrementiert den Listenindex und startet den nächsten Titel.</summary>
        private void NextSongExecute(object parameter)
        {
            int index = Songs.IndexOf(SelectedSong);
            if (index < Songs.Count - 1) { SelectedSong = Songs[index + 1]; PlaySong(SelectedSong); }
        }

        /// <summary>Beendet die Medienwiedergabe und führt eine Navigation zur Startseite aus.</summary>
        private void BackToHomeExecute(object parameter)
        {
            _mediaPlayer.Stop();
            _isPlaying = false;
            EventAggregator.GetEvent<NavigationEvent>().Publish("HomeView");
        }

        /// <summary>
        /// Analysiert den Dateipfad (relativ oder absolut) und initiiert die hardwarenahe Audiowiedergabe.
        /// </summary>
        /// <param name="song">Die abzuspielende Song-Entität.</param>
        private void PlaySong(Song song)
        {
            string fullPath = Path.IsPathRooted(song.FilePath)
                ? song.FilePath
                : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, song.FilePath);

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