using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Rockit.Models
{
    public class Artist
    {
        [Key]
        public int ArtistId { get; set; }

        [Required]
        public string Name { get; set; }

        public int Rp { get; set; }

        [Required]
        public string Picture { get; set; }

    }

    public static class ArtistStore
    {
        public static List<Artist> ListOfArtist { get; set; } = new List<Artist>();
    }
}
