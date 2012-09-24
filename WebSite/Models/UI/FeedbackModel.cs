using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebSite.Models.UI
{
    /// <summary>
    /// Feedback provided by the user about our services and products.
    /// </summary>
    public class FeedbackModel
    {
        [Required]
        [StringLength(50)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; }

        [Required]
        [Display(Name = "Message")]
        public string Message { get; set; }

        /// <summary>
        /// Whether the feedback was successfully submitted
        /// </summary>
        public bool IsSubmittedSuccessfully { get; set; }
    }
}