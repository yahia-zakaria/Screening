using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Studentscreeningsystem.ViewsModel
{
    public class DetailsRequirementVM
    {
        public int Id { get; set; }
        public int IdRequirement { get; set; }
        public String NameRequirement { get; set; }
        public decimal percentage { get; set; }
        //public int priority { get; set; }
    }
}