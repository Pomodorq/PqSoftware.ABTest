using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PqSoftware.ABTest.Data.Models
{
    public class Project
    {
        public int ProjectId { get; set; }
        [Required]
        public string Name { get; set; }

        public IEnumerable<ProjectUser> ProjectUsers{ get; set; }
    }
}
