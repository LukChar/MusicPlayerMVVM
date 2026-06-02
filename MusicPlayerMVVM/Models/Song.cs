using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System;
using MusicPlayerMVVM.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicPlayerMVVM.Models
{
    /// <summary>
    /// Stellt die abstrakte Basisklasse für alle Musiktitel dar.
    /// Enthält die primären Eigenschaften, die auf die korrespondierenden SQL-Spalten abgebildet werden.
    /// </summary>
    public abstract class Song
    {
        /// <summary>
        /// Ruft den Primärschlüssel des Datensatzes ab oder legt diesen fest.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Ruft den Titel des Musikstücks ab oder legt diesen fest.
        /// </summary>
        [Required]
        [MaxLength(255)]
        public string Title { get; set; }

        /// <summary>
        /// Ruft den Interpreten des Musikstücks ab oder legt diesen fest.
        /// </summary>
        [Required]
        [MaxLength(255)]
        public string Artist { get; set; }

        /// <summary>
        /// Ruft die formatierte Abspieldauer (z. B. "03:45") ab oder legt diese fest.
        /// </summary>
        [MaxLength(10)]
        public string Duration { get; set; }

        /// <summary>
        /// Ruft den relativen Dateipfad zur physikalischen Audiodatei ab oder legt diesen fest.
        /// </summary>
        [Required]
        public string FilePath { get; set; }
    }

    /// <summary>
    /// Repräsentiert die Entität für die SQL-Tabelle 'Songs_HipHop'.
    /// </summary>
    [Table("Songs_HipHop")]
    public class HipHopSong : Song { }

    /// <summary>
    /// Repräsentiert die Entität für die SQL-Tabelle 'Songs_Rock'.
    /// </summary>
    [Table("Songs_Rock")]
    public class RockSong : Song { }

    /// <summary>
    /// Repräsentiert die Entität für die SQL-Tabelle 'Songs_Pop'.
    /// </summary>
    [Table("Songs_Pop")]
    public class PopSong : Song { }

    /// <summary>
    /// Repräsentiert die Entität für die SQL-Tabelle 'Songs_Jazz'.
    /// </summary>
    [Table("Songs_Jazz")]
    public class JazzSong : Song { }

    /// <summary>
    /// Repräsentiert die Entität für die SQL-Tabelle 'Songs_Klassik'.
    /// </summary>
    [Table("Songs_Klassik")]
    public class KlassikSong : Song { }

    /// <summary>
    /// Repräsentiert die Entität für die SQL-Tabelle 'Songs_Custom'.
    /// </summary>
    [Table("Songs_Custom")]
    public class CustomSong : Song { }
}