using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using DataAnnotationsExtensions;
using System.Web.Mvc;

namespace WebSite.Models.Data
{
    public class Homepage
    {
        [Key]
        [Required]
        public int HomepageId { get; set; }

        [UIHint("tinymce_jquery_full"), AllowHtml]
        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Contents")]
        public string Contents { get; set; }
    }
}
