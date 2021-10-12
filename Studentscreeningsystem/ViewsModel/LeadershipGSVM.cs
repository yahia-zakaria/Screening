using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Studentscreeningsystem.ViewsModel
{
    public class LeadershipGSVM
    {
        public int IdSector { get; set; }
        public bool IsCheckedGraduate { get; set; }
        public bool IsCheckedSector { get; set; }
        public string NameLeadership { get; set; }
    }
}