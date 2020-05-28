using System;
using System.ComponentModel.DataAnnotations;

namespace cw3.DTOs.Requests
{
    public class EnrollStudentRequest
    {
        [RegularExpression("s[0-9]+$")]
        public String IndexNumber { get; set; }

        [Required]
        [MaxLength(15)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(255)]
        public string LastName { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd MMM, yyyy}")]
        public DateTime BirthDate { get; set; }

        [Required]
        public String Studies { get; set; }
    }
}

