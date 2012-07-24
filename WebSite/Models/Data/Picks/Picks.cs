using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebSite.Models.Data.Picks
{
    public abstract class Pick
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
        public string Description { get; set; }

        /// <summary>
        /// Whether the pick is available for members to see.
        /// </summary>
        public bool IsPublished { get; set; }

        /// <summary>
        /// The date when the trade has been created.
        /// </summary>
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// The date at which the trade has been closed.
        /// </summary>
        public DateTime? ClosingDate { get; set; }

        /// <summary>
        /// Sets of figures used by this pick.
        /// </summary>
        public virtual ICollection<Figure> Figures { get; set; }

        /// <summary>
        /// Sets of updates provided for this pick.
        /// </summary>
        public virtual ICollection<PickUpdate> Updates { get; set; }
    }
}