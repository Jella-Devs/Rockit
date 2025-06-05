using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rockit.Models
{
    public class PlayListItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string SongName { get; set; }

        [Required]
        public string SongPath { get; set; }

        [Required]
        public string SongKey => $"{SongName}|{SongPath}";

        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    }

    public static class PlaylistStore
    {
        public static List<PlayListItem> playlist { get; set; } = new List<PlayListItem>();
    }
}
