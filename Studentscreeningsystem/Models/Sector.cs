using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;

 namespace Studentscreeningsystem.Models

{  //الاسلحة
    public class Sector
    {

        [Key]
        public int IdSector { get; set; }

        [Required]
        public String NameSector { get; set; }
        public int NbGraduates { get; set; }
        public String LogoPath { get; set; }
        
    }
}