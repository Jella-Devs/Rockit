using Microsoft.EntityFrameworkCore;
using Rockit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rockit.Data
{
    public class RockolaDbContext : DbContext
    {
        public DbSet<Artist> Artists { get; set; }
        public DbSet<Song> Songs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
     .UseLazyLoadingProxies()
     .UseNpgsql("Host=localhost;Port=5432;Database=rockola_db;Username=rockola_user;Password=rockola_pass");

        }
    }
}
