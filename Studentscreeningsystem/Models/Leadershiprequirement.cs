using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Studentscreeningsystem.Models
{//متطلبات قيادية
    public class Leadershiprequirement
    {
        [Key]
        public int IdLeadership { get; set; }
        public String NameLeadership { get; set; }
    }
}