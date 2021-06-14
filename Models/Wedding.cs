using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace WeddingPlanner.Models
{
    public class Wedding
    {
        [Key]
        public int WeddingId { get; set; }
        [Required]
        [Display(Name = "Wedder One")]
        public string WedderOne { get; set; }
        [Required]
        [Display(Name = "Wedder Two")]
        public string WedderTwo { get; set; }
        [Required]
        [Display(Name = "Date")]
        public DateTime Date { get; set; }
        [Required]
        [Display(Name = "Wedding Address")]
        public string WeddingAdress { get; set; }

        public int PlannerId { get; set; }

        public List<RSVP> Guests { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}