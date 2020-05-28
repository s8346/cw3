using System;
using System.ComponentModel.DataAnnotations;

namespace cw3.DTOs.Requests
{
    public class UpdateStudentRequest
    {
        
       [Required]
        public string IndexNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
    }

}
