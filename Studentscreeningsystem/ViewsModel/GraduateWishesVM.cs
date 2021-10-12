using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Studentscreeningsystem.ViewsModel
{
    public class GraduateWishesVM
    {
        public GraduateWishesVM()
        {
            IdSectorList = new List<ListSectorsWishesVM>();
        }
        public int IdGraduate { get; set; }
        public List<ListSectorsWishesVM> IdSectorList { get; set; }
    }
}