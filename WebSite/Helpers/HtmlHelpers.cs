using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebSite.Helpers
{
    public static class HtmlHelpers
    {
        public static IHtmlString InformationMessage<T>(this HtmlHelper<T> html, string message)
        {
            return MessageTable(html, message, "/Images/info.png");
        }

        public static IHtmlString ErrorMessage<T>(this HtmlHelper<T> html, string message)
        {
            return MessageTable(html, message, "/Images/error.png");
        }

        public static IHtmlString SuccessMessage<T>(this HtmlHelper<T> html, string message)
        {
            return MessageTable(html, message, "/Images/success.png");
        }

        private static IHtmlString MessageTable(HtmlHelper helper, string message, string iconPath)
        {
            TagBuilder row = new TagBuilder("tr");
            TagBuilder iconCell = new TagBuilder("td");
            TagBuilder messageCell = new TagBuilder("td");
            TagBuilder table = new TagBuilder("table");

            // Add css class to table
            table.AddCssClass("message-table");

            // Add contents of the message cell
            messageCell.InnerHtml = message;

            // Add icon
            iconCell.InnerHtml = "<img src=\"" + iconPath + "\"/>";

            row.InnerHtml = iconCell.ToString() + messageCell.ToString();
            table.InnerHtml = row.ToString();

            return helper.Raw(table.ToString());
        }
    }
}