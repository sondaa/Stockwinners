using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebSite.Models.Data.Picks
{
    /// <summary>
    /// An update for a given stock or option pick.
    /// </summary>
    public class PickUpdate
    {
        [Key]
        public int PickUpdateId { get; set; }

        [Required]
        public string Description { get; set; }

        /// <summary>
        /// When the update was issued.
        /// </summary>
        public DateTime IssueDate { get; set; }
    }
}