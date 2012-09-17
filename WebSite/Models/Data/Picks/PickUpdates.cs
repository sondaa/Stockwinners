using ActionMailer.Net.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;
using WebSite.Database;
using System.Data.Entity;
using System.Data;
using System.Linq;

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
        [UIHint("tinymce_jquery_full")]
        [AllowHtml]
        public string Description { get; set; }

        /// <summary>
        /// When the update was issued.
        /// </summary>
        public DateTime IssueDate { get; set; }

        /// <summary>
        /// The pick this update belongs to.
        /// </summary>
        [ForeignKey("Pick")]
        public int PickId { get; set; }
        public virtual Pick Pick { get; set; }

        public void Email()
        {
            // Ensure the pick is retrieved from the database before sending the email
            DatabaseContext db = System.Web.Mvc.DependencyResolver.Current.GetService(typeof(DatabaseContext)) as DatabaseContext;
            db.Entry(this).Reference(pickUpdate => pickUpdate.Pick).Load();

            // Prepare email message
            EmailResult email = new WebSite.Mailers.Picks().PickUpdate(this);

            // Send the message to pick subscribers who have an active membership
            WebSite.Helpers.Email.SendEmail(email, db.Entry(this.Pick).Collection(pick => pick.Subscribers).Query().Where(WebSite.Helpers.Email.ActiveUserPredicate));
        }
    }
}