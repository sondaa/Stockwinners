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
            Subject = "New Stock Pick";

            ViewBag.StockPickContents = stockPick.Description;
            ViewBag.EntryPrice = stockPick.EntryPrice;
            ViewBag.IsLongPosition = stockPick.IsLongPosition;
            ViewBag.Symbol = stockPick.Symbol;
            ViewBag.PublishingDate = stockPick.PublishingDate ?? DateTime.UtcNow;

            return this.Email(viewName: "StockPickEmail");
		}

		
		public virtual EmailResult Option(OptionPick optionPick)
		{
            To.Add("noreply@stockwinners.com");
			Subject = "New Option Pick";

            ViewBag.OptionPickContents = optionPick.Description;
            ViewBag.Symbol = optionPick.Symbol;
            ViewBag.Type = optionPick.Type;
            ViewBag.PublishingDate = optionPick.PublishingDate ?? DateTime.UtcNow;

            return this.Email(viewName: "OptionPickEmail");
		}

		
		public virtual EmailResult Alert(DailyAlert dailyAlert)
		{
            To.Add("noreply@stockwinners.com");
			Subject = "Market Alert";

            ViewBag.AlertContents = dailyAlert.Content;

            return this.Email(viewName: "AlertEmail");
		}
	}
}