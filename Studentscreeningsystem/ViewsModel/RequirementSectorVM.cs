using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace Studentscreeningsystem.ViewsModel
{
    public class RequirementSectorVM
    {
        public RequirementSectorVM()
        {
            RequirementMatrial = new List<RequirementVM>();
            Requirementsupplement = new List<RequirementVM>();
            RequirementFitness = new List<RequirementVM>();
            RequirementMesure = new List<RequirementVM>();
            LeadershipVM = new List<LeadershipVM>();

        }
        public int IdSector { get; set; }
        public int NbGraduates { get; set; }
        public int NameSector { get; set; }
        public String LogoPath { get; set; }
        [Range(120, 230, ErrorMessage = "الرجاء إدخال الطول المناسب ")]
        public int Maxlenght { get; set; }
        [Range(120, 230, ErrorMessage = "الرجاء إدخال الطول المناسب ")]
        public int Minlenght { get; set; }
        [Range(40, 120, ErrorMessage = "الرجاء إدخال الوزن المناسب ")]
        public int Maxweight { get; set; }
        [Range(40, 120, ErrorMessage = "الرجاء إدخال الوزن المناسب ")]
        public int Minweight { get; set; }
        
       public List<LeadershipVM> LeadershipVM { get; set; }
        public List<RequirementVM> RequirementMatrial { get; set; }
        public List<RequirementVM> Requirementsupplement { get; set; }
        public List<RequirementVM> RequirementFitness { get; set; }
        public List<RequirementVM> RequirementMesure { get; set; }
    }
}