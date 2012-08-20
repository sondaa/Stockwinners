using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebSite.Models.Data
{
    public class NotificationSettings
    {
        [Key]
        [Required]
        public int NotificationSettingId { get; set; }

        [Required]
        [Display(Name = "Receive daily alerts")]
        public bool ReceiveDailyAlerts { get; set; }

        [Required]
        [Display(Name = "Receive option picks")]
        public bool ReceiveOptionPicks { get; set; }

        [Required]
        [Display(Name = "Receive stock picks")]
        public bool ReceiveStockPicks { get; set; }

        [Required]
        [Display(Name = "Receive weekly alerts")]
        public bool ReceiveWeeklyAlerts { get; set; }

        [Required]
        [Display(Name = "Receive general announcements")]
        public bool ReceiveGeneralAnnouncements { get; set; }
    }
}