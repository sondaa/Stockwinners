using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebSite.Models.Data.Picks
{
    /// <summary>
    /// Pictures added to a given pick.
    /// </summary>
    public class PickFigure
    {
        [Key]
        [Required]
        public int PickFigureId { get; set; }

        [Required]
        [StringLength(150)]
        public string Caption { get; set; }

        /// <summary>
        /// A reference to the name of the blob stored in Azure.
        /// </summary>
        [Required]
        [StringLength(36)]
        public string BlobGuidUri { get; set; }
    }
}