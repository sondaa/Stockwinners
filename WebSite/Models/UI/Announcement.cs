using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebSite.Models.UI
{
    public class Announcement
    {
        [Required]
        [StringLength(60)]
        public string Subject { get; set; }

        [Required]
        [UIHint("tinymce_jquery_full"), AllowHtml]
        public string Message { get; set; }
    }
}