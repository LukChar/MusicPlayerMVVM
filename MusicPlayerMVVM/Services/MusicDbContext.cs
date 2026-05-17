using MusicPlayerMVVM.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayerMVVM.Models
{
    public class MusicDbContext : DbContext
    {
        // "MusicDB" verweist auf den ConnectionString in deiner App.config
        public MusicDbContext() : base("name=MusicDB")
        {
        }

        public DbSet<Song> Songs { get; set; }
    }
}
