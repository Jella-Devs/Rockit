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

        public int Players { get; set; }

        [Required]
        public string Front { get; set; }

        public virtual ICollection<Song> Songs { get; set; } = new List<Song>();
    }
}
