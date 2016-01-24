using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace MusicStore.Models
{
    public class Artist
    {
        [Key]
        public int ArtistId { get; set; }
        [DisplayName("Artist")]
        public string Name { get; set; }
    }
}