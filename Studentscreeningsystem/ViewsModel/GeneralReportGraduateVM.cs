using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Studentscreeningsystem.ViewsModel
{
    public class GeneralReportGraduateVM
    {
        public int IdGraduate{ get; set; }
        public Int64 NumGraduate { get; set; }
        public  Int64 IdentityGraduate { get; set; }
        public string NameGraduate { get; set; }
        public float AverageGraduate { get; set; }

        public List<WishesVM> WishesList { get; set; }
        public List<PercentageGSVM> MaterialList { get; set; }
        public List<PercentageGSVM> FitnessList { get; set; }
        public List<PercentageGSVM> MeasureList { get; set; }
        public List<PercentageGSVM> BehaviorList { get; set; }
        public List<LeadershipGSVM> LeadershipList { get; set; }
    }
}