using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace Studentscreeningsystem.Models
{
    public class RequirementSector
    {
        public int Id { get; set; }
        [Range(0, 100, ErrorMessage = "الرجاء إدخال النسبة المئوية المناسبة ")]
        [Required(ErrorMessage = "هذا الحقل إجباري ")]
        public decimal percentage { get; set; }
        //الأولوية
    
        //public int priority { get; set; }
        public int IdRequirement { get; set; }
        public Requirement Requirement { get; set; }
        public int IdSector { get; set; }
        public Sector Sector { get; set; }
    }
}