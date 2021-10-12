using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Studentscreeningsystem.ViewsModel
{
    public class RapportSectorVM
    {
        public int IdSector { get; set; }
        public String NameSector { get; set; }
        public int NbGraduates { get; set; }
        public String LogoPath { get; set; }
        public int Maxlenght { get; set; }
        public int Minlenght { get; set; }
        public int Maxweight { get; set; }
        public int Minweight { get; set; }
        public int Idspesification { get; set; }
        public List<DetailsLeadershipVM> RequirementLeadershipVM { get; set; }
        public List<DetailsRequirementVM> RequirementMatrial { get; set; }
        public List<DetailsRequirementVM> Requirementsupplement { get; set; }
        public List<DetailsRequirementVM> RequirementFitness { get; set; }
        public List<DetailsRequirementVM> RequirementMesure { get; set; }
    }
}