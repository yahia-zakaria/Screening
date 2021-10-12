using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Studentscreeningsystem.Models
{
    public class LeadershipGraduate
    {
        public int Id { get; set; }
        public bool IsChecked { get; set; }
        public int IdLeadership { get; set; }
        public Leadershiprequirement Leadershiprequirement { get; set; }
        public int User_Id { get; set; }
        public USER USER { get; set; }
    }
}