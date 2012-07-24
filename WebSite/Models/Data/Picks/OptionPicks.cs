using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebSite.Models.Data.Picks
{
    [Table("OptionPicks")]
    public class OptionPick : Pick
    {
        /// <summary>
        /// Legs that this option pick contains.
        /// </summary>
        public virtual ICollection<OptionPickLeg> Legs { get; set; }

        public override void Email()
        {
            Helpers.Email.Send(this);
        }
    }
}