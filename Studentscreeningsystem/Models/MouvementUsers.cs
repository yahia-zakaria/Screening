using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Studentscreeningsystem.Models
{
    public class MouvementUsers
    {
        public int Id { get; set; }
        public int User_Id { get; set; }
        public USER USER { get; set; }
        public String Link { get; set; }
        public DateTime? DateMouvement { get; set; }

    }
}