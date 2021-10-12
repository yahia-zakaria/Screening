using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Studentscreeningsystem.Models
{
    public class GraduateWishes
    {
        public int Id { get; set; }
        public int User_Id { get; set; }
        public USER USER { get; set; }
        public int IdSector { get; set; }
        public Sector Sector { get; set; }
        public int Rank { get; set; }
        //public int Approved { get; set; }
        
    }
}