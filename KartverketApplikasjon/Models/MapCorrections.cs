using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace KartverketApplikasjon.Models
{
    public class MapCorrections 
    {
        [Required]
        public string Description { get; set; }
        [Required]
        public string X { get; set; }
        [Required]
        public string Y { get; set; }
    }
}