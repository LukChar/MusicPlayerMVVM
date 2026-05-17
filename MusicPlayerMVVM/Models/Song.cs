using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System;
using MusicPlayerMVVM.Common;

namespace MusicPlayerMVVM.Models
{
    public class Song : NotifyPropertyChanged
    {
        private int _id;
        private string _title;
        private string _artist;
        private string _duration;
        private string _filePath;

        public int Id
        {
            get { return _id; }
            set { _id = value; OnPropertyChanged(nameof(Id)); }
        }

        public string Title
        {
            get { return _title; }
            set { _title = value; OnPropertyChanged(nameof(Title)); }
        }

        public string Artist
        {
            get { return _artist; }
            set { _artist = value; OnPropertyChanged(nameof(Artist)); }
        }

        public string Duration
        {
            get { return _duration; }
            set { _duration = value; OnPropertyChanged(nameof(Duration)); }
        }

        public string FilePath
        {
            get { return _filePath; }
            set { _filePath = value; OnPropertyChanged(nameof(FilePath)); }
        }
    }
}