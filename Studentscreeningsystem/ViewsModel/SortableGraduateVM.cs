using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Studentscreeningsystem.ViewsModel
{
    public class SortableGraduateVM
    {
        public int IdGraduate { get; set; }
        public String NameGraduate { get; set; }
        public Int64 NumGraduate { get; set; }
        public int? IdSector { get; set; }
        public string NameSector { get; set; }
        public float Average { get; set; }
        public int Rank { get; set; }
    }
}