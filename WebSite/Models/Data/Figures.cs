using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebSite.Models.Data
{
    /// <summary>
    /// Pictures added to a given pick or alert.
    /// </summary>
    public class Figure
    {
        [Key]
        [Required]
        public int FigureId { get; set; }

        /// <summary>
        /// A reference to the name of the blob stored in Azure.
        /// </summary>
        [Required]
        [StringLength(36)]
        public string BlobGuidUri { get; set; }
    }
}