using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Senjyouhara.Common.Models
{
    public class MenuModel
    {

        public int MenuId { get; set; }

        public string name { get; set; }

        public string path { get; set; }

        public string component { get; set; }
        public Meta meta { get; set; }

        public List<MenuModel> children { get; set; } = new List<MenuModel>();
    }

    public class Meta
    {
        public string icon { get; set; }
        public string title { get; set; }
    }
}
