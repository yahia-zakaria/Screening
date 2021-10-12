using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Studentscreeningsystem.ViewsModel
{
    public class PercentageGSVM
    {
        public int IdSector { get; set; }
        public string Requirement { get; set; }
        public string NameRequirement { get; set; }
        public decimal PercentageGraduate { get; set; }
        public decimal PercentageSector { get; set; }
        //public int Priority { get; set; }
    }

}