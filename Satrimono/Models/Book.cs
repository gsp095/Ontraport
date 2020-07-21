using System;
using System.ComponentModel.DataAnnotations;

namespace HanumanInstitute.Satrimono.Models
{
    public partial class Book
    {
        public int Id { get; set; }
        [Required]
        public string Key { get; set; } = string.Empty;
        public string? Title { get; set; }
        public string? Subtitle { get; set; }
        public string? Author { get; set; }
        [DisplayFormat(DataFormatString = "{0:p1}")]
        public double? Accuracy { get; set; }
        [DisplayFormat(DataFormatString = "{0:#,###}")]
        public int? Vibration { get; set; }
        [Required]
        public bool? IsFiction { get; set; } = false;
        public string? Url { get; set; }
        //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        //public DateTime DateMofidied { get; set; }
    }
}
