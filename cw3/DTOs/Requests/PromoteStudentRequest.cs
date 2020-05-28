using System;
using System.ComponentModel.DataAnnotations;

namespace cw3.DTOs.Requests
{
    public class PromoteStudentRequest
    {
        [Required]
        public String Studies { get; set; }

        [Required]
        public int Semester { get; set; }
    }
}
