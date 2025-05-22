using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rockit.Models
{
    public class Artist
    {
        public int ArtistId { get; set; }

        [Required]
        public string Name { get; set; }

        public int Rp { get; set; }

        [Required]
        public string Picture { get; set; }

        public virtual ICollection<Song> Songs { get; set; } = new List<Song>();
    }
}
