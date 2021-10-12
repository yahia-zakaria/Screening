using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Studentscreeningsystem.ViewsModel
{
    public class DetailsLeadershipVM
    {
        public int Id { get; set; }
        public int IdLeadership { get; set; }
        public String NameLeadership { get; set; }
        public bool IsChecked { get; set; }
    }
}