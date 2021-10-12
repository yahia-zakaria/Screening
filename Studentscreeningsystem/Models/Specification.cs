using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace Studentscreeningsystem.Models

{//الطول الوزن
    public class Specification
    {
        [Key]
        public int IdSpecification { get; set; }
        [Range(120, 230, ErrorMessage = "الرجاء إدخال الطول المناسب ")]
        public int Maxlenght { get; set; }
        [Range(120, 230, ErrorMessage = "الرجاء إدخال الطول المناسب ")]
        public int Minlenght { get; set; }
        [Range(40, 120, ErrorMessage = "الرجاء إدخال الوزن المناسب ")]
        public int Maxweight{ get; set; }
        [Range(40, 120, ErrorMessage = "الرجاء إدخال الوزن المناسب ")]
        public int Minweight { get; set; }
        public int IdSector { get; set; }
        public Sector Sector { get; set; }
    }
}