using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace Studentscreeningsystem.ViewsModel
{
    public class RequirementVM
    {
        public int Id { get; set; }
        public int IdRequirement { get; set; }
      
        public int percentage { get; set; }
        public int priority { get; set; }
    }
}