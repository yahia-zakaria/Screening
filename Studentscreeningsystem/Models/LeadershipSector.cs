using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Studentscreeningsystem.Models
{
    public class LeadershipSector
    {
        public int Id { get; set; }
        public bool IsChecked { get; set; }
        public int IdLeadership { get; set; }
        public Leadershiprequirement Leadershiprequirement { get; set; }
       
        public int IdSector { get; set; }
        public Sector Sector { get; set; }
    }
}