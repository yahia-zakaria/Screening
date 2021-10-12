using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Studentscreeningsystem.Models
{
    public class DegreeGraduate
    {
        public int Id { get; set; }
        public decimal percentage { get; set; }
        public int IdRequirement { get; set; }
        public Requirement Requirement { get; set; }
        public int User_Id { get; set; }
        public USER USER { get; set; }
    }
}