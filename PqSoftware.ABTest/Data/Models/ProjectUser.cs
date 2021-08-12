using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PqSoftware.ABTest.Data.Models
{
    public class ProjectUser
    {
        public int Id { get; set; }

        [Required]
        public int? UserId { get; set; }

        [Required]
        public int? ProjectId { get; set; }
        [Required]
        public DateTime? DateRegistration { get; set; }
        [Required]
        public DateTime? DateLastActivity { get; set; }
        public Project Project { get; set; }
    }
}
