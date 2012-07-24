using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSite.Interfaces;

namespace WebSite.Models.Data.Picks
{
    public abstract class Pick : IEmailable
    {
        [Key]
        [Required]
        public int PickId { get; set; }

        /// <summary>
        /// The symbol of the underlying financial instrument for which the pick is being given.
        /// </summary>
        [Required]
        [StringLength(6)]
        public string Symbol { get; set; }

        /// <summary>
        /// Textual description of the pick shown to users.
        /// </summary>
        [Required]
        [UIHint("tinymce_jquery_full")]
        [AllowHtml]
        [Display(Name = "Description")]
        public string Description { get; set; }

        /// <summary>
        /// The time at which the pick has been published.
        /// </summary>
        [Display(Name = "Published On")]
        public DateTime? PublishingDate { get; set; }

        /// <summary>
        /// Whether the pick is available for members to see.
        /// </summary>
        [Display(Name = "Published to members?")]
        public bool IsPublished { get; set; }

        /// <summary>
        /// The date when the trade has been created.
        /// </summary>
        [Display(Name = "Created On")]
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// The date at which the trade has been closed.
        /// </summary>
        [Display(Name = "Trade's Closing Date")]
        public DateTime? ClosingDate { get; set; }

        /// <summary>
        /// Sets of figures used by this pick.
        /// </summary>
        public virtual ICollection<Figure> Figures { get; set; }

        /// <summary>
        /// Sets of updates provided for this pick.
        /// </summary>
        public virtual ICollection<PickUpdate> Updates { get; set; }

        public void Publish()
        {
            this.IsPublished = true;
            this.PublishingDate = DateTime.UtcNow;
        }

        public void Initialize()
        {
            this.CreationDate = DateTime.UtcNow;
        }

        public void Close()
        {
            this.ClosingDate = DateTime.UtcNow;
        }

        public abstract void Email();
    }
}