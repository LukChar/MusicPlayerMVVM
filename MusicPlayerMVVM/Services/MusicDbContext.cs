using MusicPlayerMVVM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace MusicPlayerMVVM.Data
{
    /// <summary>
    /// Repräsentiert den Datenbankkontext für den Musikplayer.
    /// Verwaltet die Verbindung zur Datenbank und die Tabellen der verschiedenen Playlists.
    /// </summary>
    public class MusicDbContext : DbContext
    {
        /// <summary>
        /// Initialisiert eine neue Instanz der <see cref="MusicDbContext"/> Klasse.
        /// Stellt sicher, dass die Datenbank erstellt ist und initialisiert die Standard-Songs.
        /// </summary>
        public MusicDbContext()
        {
            Database.EnsureCreated();

            // Startet den Scan und überschreibt alles
            SeedSongsFromDirectories(@"C:\Users\Lukas\source\repos\MusicPlayerMVVM\MusicPlayerMVVM\Resources");
        }

        /// <summary>Ruft die Tabelle für Hip-Hop-Songs ab oder legt diese fest.</summary>
        public DbSet<HipHopSong> HipHopSongs { get; set; }

        /// <summary>Ruft die Tabelle für Rock-Songs ab oder legt diese fest.</summary>
        public DbSet<RockSong> RockSongs { get; set; }

        /// <summary>Ruft die Tabelle für Pop-Songs ab oder legt diese fest.</summary>
        public DbSet<PopSong> PopSongs { get; set; }

        /// <summary>Ruft die Tabelle für Jazz-Songs ab oder legt diese fest.</summary>
        public DbSet<JazzSong> JazzSongs { get; set; }

        /// <summary>Ruft die Tabelle für Klassik-Songs ab oder legt diese fest.</summary>
        public DbSet<KlassikSong> KlassikSongs { get; set; }

        /// <summary>Ruft die Tabelle für benutzerdefinierte Songs ab oder legt diese fest.</summary>
        public DbSet<CustomSong> CustomSongs { get; set; }

        /// <summary>
        /// Konfiguriert den Datenbankkontext für die Verwendung des SQL Servers mit dem Connection String aus der App.config.
        /// </summary>
        /// <param name="optionsBuilder">Ein Builder zur Erstellung oder Änderung von Optionen für diesen Kontext.</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                string connectionString = ConfigurationManager.ConnectionStrings["MusicDB"].ConnectionString;
                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        /// <summary>
        /// Durchsucht das angegebene Basisverzeichnis nach .wav-Dateien in genrespezifischen Unterordnern
        /// und fügt diese der Datenbank hinzu.
        /// </summary>
        /// <param name="baseFolderPath">Der absolute Pfad zum Ressourcen-Ordner.</param>
        private void SeedSongsFromDirectories(string baseFolderPath)
        {
            if (!Directory.Exists(baseFolderPath)) return;

            // ALLES LÖSCHEN 5 fehlerhaften Lieder entferne
            HipHopSongs.RemoveRange(HipHopSongs.ToList());

            /* 
            RockSongs.RemoveRange(RockSongs.ToList());
            PopSongs.RemoveRange(PopSongs.ToList());
            JazzSongs.RemoveRange(JazzSongs.ToList());
            KlassikSongs.RemoveRange(KlassikSongs.ToList());
            CustomSongs.RemoveRange(CustomSongs.ToList());
            */

            // Speichert die Löschung, Datenbank ist jetzt zu 100 % leer
            this.SaveChanges();

            // .WAV DATEIEN NEU EINLESEN
            string hipHopPath = Path.Combine(baseFolderPath, "HipHop");
            if (Directory.Exists(hipHopPath))
            {
                foreach (string file in Directory.GetFiles(hipHopPath, "*.wav"))
                    HipHopSongs.Add(new HipHopSong { Title = Path.GetFileNameWithoutExtension(file), Artist = "Lokale Datei", FilePath = file, Duration = "00:00" });
            }

            /* // 

            string rockPath = Path.Combine(baseFolderPath, "Rock");
            if (Directory.Exists(rockPath))
            {
                foreach (string file in Directory.GetFiles(rockPath, "*.wav"))
                    RockSongs.Add(new RockSong { Title = Path.GetFileNameWithoutExtension(file), Artist = "Lokale Datei", FilePath = file, Duration = "00:00" });
            }

            string popPath = Path.Combine(baseFolderPath, "Pop");
            if (Directory.Exists(popPath))
            {
                foreach (string file in Directory.GetFiles(popPath, "*.wav"))
                    PopSongs.Add(new PopSong { Title = Path.GetFileNameWithoutExtension(file), Artist = "Lokale Datei", FilePath = file, Duration = "00:00" });
            }

            string jazzPath = Path.Combine(baseFolderPath, "Jazz");
            if (Directory.Exists(jazzPath))
            {
                foreach (string file in Directory.GetFiles(jazzPath, "*.wav"))
                    JazzSongs.Add(new JazzSong { Title = Path.GetFileNameWithoutExtension(file), Artist = "Lokale Datei", FilePath = file, Duration = "00:00" });
            }

            string klassikPath = Path.Combine(baseFolderPath, "Klassik");
            if (Directory.Exists(klassikPath))
            {
                foreach (string file in Directory.GetFiles(klassikPath, "*.wav"))
                    KlassikSongs.Add(new KlassikSong { Title = Path.GetFileNameWithoutExtension(file), Artist = "Lokale Datei", FilePath = file, Duration = "00:00" });
            }

            string customPath = Path.Combine(baseFolderPath, "Custom");
            if (Directory.Exists(customPath))
            {
                foreach (string file in Directory.GetFiles(customPath, "*.wav"))
                    CustomSongs.Add(new CustomSong { Title = Path.GetFileNameWithoutExtension(file), Artist = "Lokale Datei", FilePath = file, Duration = "00:00" });
            }
            */

            // Lieder Datenbank speichern
            this.SaveChanges();
        }
    }
}