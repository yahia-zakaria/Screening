using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Studentscreeningsystem.ViewsModel
{
    public class DetailsSectorVM
    {
        public int IdSector { get; set; }
        public String NameSector { get; set; }
        public int NbGraduates { get; set; }
        public List<DetailsRequirementVM> AllRequirements { get; set; }
    }
}