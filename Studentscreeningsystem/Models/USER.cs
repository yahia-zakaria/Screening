using Studentscreeningsystem.ViewsModel;

namespace Studentscreeningsystem.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("USERS")]
    public partial class USER
    {
        public USER()
        {
            ROLES = new HashSet<ROLE>();
            GraduateWishes = new List<GraduateWishes>();
            RapportGraduate = new alldegreeGraduateVM();
        }

        [Key] public int User_Id { get; set; }

        [Required(ErrorMessage = "يجب تعبئة هذا الحقل")]
        [MinLength(10, ErrorMessage = "الرجاء إدخال رقم الهوية المتكون من  عشرة أرقام")]
        [MaxLength(10, ErrorMessage = "الرجاء إدخال رقم الهوية المتكون من  عشرة أرقام")]
        public string Username { get; set; }

        public DateTime? LastModified { get; set; }

        public bool? Inactive { get; set; }

        [StringLength(50)] public string Firstname { get; set; }

        [StringLength(50)] public string Lastname { get; set; }

        [StringLength(30)] public string Title { get; set; }

        public bool? Insortable { get; set; }


        [DataType(DataType.Password)]
        [Required(ErrorMessage = "يجب تعبئة هذا الحقل")]
        public string Passeword { get; set; }

        [Compare("Passeword", ErrorMessage = "الرقم السري غير متطابق")]
        [DataType(DataType.Password)]
        public string Comfirmepasseword { get; set; }

        public virtual ICollection<ROLE> ROLES { get; set; }
        public virtual List<GraduateWishes> GraduateWishes { get; set; }
        public virtual alldegreeGraduateVM RapportGraduate { get; set; }

        public int? IdSector { get; set; }
        public Sector Sector { get; set; }
        public int Length { get; set; }
        public int Weight { get; set; }
        public float AverageGraduate { get; set; }
    }
}