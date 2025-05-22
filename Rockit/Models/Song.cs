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
        public int SongId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Route { get; set; }

        public int Players { get; set; }

        public int ArtistId { get; set; }

        public virtual Artist Artist { get; set; }
    }
}
