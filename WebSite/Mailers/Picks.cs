using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using WebSite.Models.Data.Picks;
using WebSite.Models.Data;
using ActionMailer.Net.Mvc;
using WebSite.Models;
using WebSite.Database;

namespace WebSite.Mailers
{ 
    public class Picks : MailerBase    
	{		
		public virtual EmailResult Stock(StockPick stockPick)
		{
            To.Add("noreply@stockwinners.com");
            Subject = "New Stock Pick (" + stockPick.Symbol + ")";

            return this.Email(viewName: "StockPickEmail", model: stockPick);
		}

		public virtual EmailResult Option(OptionPick optionPick)
		{
            To.Add("noreply@stockwinners.com");
			Subject = "New Option Pick (" + optionPick.Symbol + ")";

            return this.Email(viewName: "OptionPickEmail", model: optionPick);
		}
		
		public virtual EmailResult Alert(DailyAlert dailyAlert)
		{
            To.Add("noreply@stockwinners.com");
			Subject = "Market Alert";

            ViewBag.AlertContents = dailyAlert.Content;

            return this.Email(viewName: "AlertEmail");
		}

        public virtual EmailResult PickUpdate(PickUpdate pickUpdate)
        {
            To.Add("noreply@stockwinners.com");
            Subject = "Update on " + pickUpdate.Pick.Symbol;

            return this.Email(viewName: "PickUpdateEmail", model: pickUpdate);
        }
	}
}