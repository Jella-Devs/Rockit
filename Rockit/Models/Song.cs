using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rockit.Models
{
    public class Song
    {
        public string SongId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Path { get; set; }

        public int Rp { get; set; }

        public string ArtistName { get; set; }
    }
    public static class SongStore
    {
        public static List<Song> ListOfSongs { get; set; } = new List<Song>();
    }
}
