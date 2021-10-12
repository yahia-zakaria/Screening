using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Studentscreeningsystem.ViewsModel
{
    public class alldegreeGraduateVM
    {
        public alldegreeGraduateVM()
        {
            degreeMatrial = new List<DetailsRequirementVM>();
            degreesupplement = new List<DetailsRequirementVM>();
            degreeFitness = new List<DetailsRequirementVM>();
            degreeMesure = new List<DetailsRequirementVM>();
            LeadershipVM = new List<DetailsLeadershipVM>();

        }

        public int Id { get; set; }
        public int IdGraduate { get; set; }
        //public String NameGraduate { get; set; }
        public string Firstname { get; set; }

        public string Lastname { get; set; }
        public Int64 NumGraduate { get; set; }
        public Int64 IdentityGraduate { get; set; }
        public int lenght { get; set; }
        public int weight { get; set; }

        public float AverageGraduate { get; set; }
        public List<DetailsLeadershipVM> LeadershipVM { get; set; }
        public List<DetailsRequirementVM> degreeMatrial { get; set; }
        public List<DetailsRequirementVM> degreesupplement { get; set; }
        public List<DetailsRequirementVM> degreeFitness { get; set; }
        public List<DetailsRequirementVM> degreeMesure { get; set; }
    }
}