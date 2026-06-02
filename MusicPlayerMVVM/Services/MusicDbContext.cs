using MusicPlayerMVVM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Microsoft.EntityFrameworkCore;

namespace MusicPlayerMVVM.Data
{
    /// <summary>
    /// Stellt die Laufzeitumgebung für die Datenbankverbindung und das O/R-Mapping bereit.
    /// Erbt von <see cref="DbContext"/> des Entity Framework Core.
    /// </summary>
    public class MusicDbContext : DbContext
    {
        /// <summary>
        /// Initialisiert den Kontext und stellt sicher, dass die Datenbank und alle Tabellen 
        /// auf dem Server existieren. 
        /// </summary>
        public MusicDbContext()
        {
            Database.EnsureCreated();
        }

        /// <summary>
        /// Ruft die Entitätsmenge für HipHop-Titel ab oder legt diese fest.
        /// </summary>
        public DbSet<HipHopSong> HipHopSongs { get; set; }

        /// <summary>
        /// Ruft die Entitätsmenge für Rock-Titel ab oder legt diese fest.
        /// </summary>
        public DbSet<RockSong> RockSongs { get; set; }

        /// <summary>
        /// Ruft die Entitätsmenge für Pop-Titel ab oder legt diese fest.
        /// </summary>
        public DbSet<PopSong> PopSongs { get; set; }

        /// <summary>
        /// Ruft die Entitätsmenge für Jazz-Titel ab oder legt diese fest.
        /// </summary>
        public DbSet<JazzSong> JazzSongs { get; set; }

        /// <summary>
        /// Ruft die Entitätsmenge für Klassik-Titel ab oder legt diese fest.
        /// </summary>
        public DbSet<KlassikSong> KlassikSongs { get; set; }

        /// <summary>
        /// Ruft die Entitätsmenge für benutzerdefinierte Titel ab oder legt diese fest.
        /// </summary>
        public DbSet<CustomSong> CustomSongs { get; set; }

        /// <summary>
        /// Konfiguriert den Kontext für die Verwendung mit dem Microsoft SQL Server.
        /// </summary>
        /// <param name="optionsBuilder">Ein Konstruktor, der zum Erstellen oder Ändern von Optionen für diesen Kontext verwendet wird.</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Holt den ConnectionString sicher aus der App.config
                string connectionString = ConfigurationManager.ConnectionStrings["MusicDB"].ConnectionString;
                optionsBuilder.UseSqlServer(connectionString);
            }
        }
    }
}