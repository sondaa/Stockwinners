using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSite.Interfaces;

namespace WebSite.Models.Data
{
    public class DailyAlert : IEmailable
    {
        [Key]
        public int DailyAlertId { get; set; }

        [Required]
        [UIHint("tinymce_jquery_full"), AllowHtml]
        [Display(Name = "Alert Contents")]
        public string Content { get; set; }

        [Required]
        [Display(Name = "Created On")]
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// The time at which the alert was published.
        /// </summary>
        [Display(Name = "Published On")]
        public DateTime? PublishDate { get; set; }

        /// <summary>
        /// Is this alert visible to users?
        /// </summary>
        [Display(Name = "Has been published to users?")]
        public bool IsPublished { get; set; }

        /// <summary>
        /// Set of figures associated with this alert.
        /// </summary>
        public virtual ICollection<Figure> Figures { get; set; }

        public void Initialize()
        {
            this.CreationDate = DateTime.UtcNow;
        }

        public void Publish()
        {
            this.IsPublished = true;
            this.PublishDate = DateTime.UtcNow;
        }

        public void Email()
        {
            Helpers.Email.Send(this);
        }
    }
}