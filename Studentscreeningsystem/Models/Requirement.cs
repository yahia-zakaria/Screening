using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Studentscreeningsystem.Models
{//المتطلبات
    public enum Categorie
    {
        [Display(Name = "مادة دراسية")]
        مادة_دراسية = 0,
        [Display(Name = "متطلبات بدنية ")]
        متطلبات_بدنية = 1,
        [Display(Name = "متطلبات قياسية ")]
        متطلبات_قياسية = 2,
        [Display(Name = "السلوك و المواضبة")]
        السلوك_و_المواضبة = 3,

    }
    public class Requirement
    {
        
        [Key]
        public int IdRequirement { get; set; }


        [Display(Name = "المتطلب")]
        public String NameRequirement { get; set; }
        [Display(Name = "الصنف")]
        public Categorie Categorie { get; set; }
    }
}