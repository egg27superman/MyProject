using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace AFH.Barcaldine.Models
{
    public class ProductDescribleModel
    {
        public string Language { get; set; }

        public DescribleDetailModel DescribleDetail { get; set; }

    }

    public class DescribleDetailModel
    {
        [DisplayName("Wine Name")]
        [Required]
        public string Name { get; set; }
        public string ShortIntro { get; set; }
        public string Desc { get; set; }

    }
}
