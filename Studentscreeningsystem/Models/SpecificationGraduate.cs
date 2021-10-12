using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Studentscreeningsystem.Models
{
    public class SpecificationGraduate
    {
        [Key]
        public int IdSpecification { get; set; }
        public int Length { get; set; }
        public int Weight { get; set; }
        public float AverageGraduate { get; set; }
        public int User_Id { get; set; }
        public USER USER { get; set; }
    }
}