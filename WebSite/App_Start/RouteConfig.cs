using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace WebSite
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            #region Search Engine Optimization Related Mappings

            routes.MapRoute(
                name: "Momentum Trading",
                url: "momentum-trading",
                defaults: new { controller = "SEO", action = "MomentumTrading" }
            );

            routes.MapRoute(
                name: "Stock Market Analysis",
                url: "stock-market-analysis",
                defaults: new { controller = "SEO", action = "StockMarketAnalysis" }
            );

            routes.MapRoute(
                name: "Best Daytrading Site",
                url: "best-daytrading-site",
                defaults: new { controller = "SEO", action = "BestDaytradingSite" }
            );

            routes.MapRoute(
                name: "Stock Rumors",
                url: "stock-rumors",
                defaults: new { controller = "SEO", action = "StockRumors" }
            );

            routes.MapRoute(
                name: "Stocks To Buy",
                url: "stocks-to-buy",
                defaults: new { controller = "SEO", action = "StocksToBuy" }
            );

            routes.MapRoute(
                name: "Best Stock on the Market",
                url: "best-stock-on-the-market",
                defaults: new { controller = "SEO", action = "BestStockOnTheMarket" }
            );

            routes.MapRoute(
                name: "Call Option Strategies",
                url: "call-option-strategies",
                defaults: new { controller = "SEO", action = "CallOptionStrategies" }
            );

            routes.MapRoute(
                name: "Stock Advice",
                url: "stock-advice",
                defaults: new { controller = "SEO", action = "StockAdvice" }
            );

            routes.MapRoute(
                name: "Stock Market Tips and Invest Guide",
                url: "stock-market-tips-and-invest-guide",
                defaults: new { controller = "SEO", action = "StockMarketTipsAndInvestGuide" }
            );

            #endregion

            routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // For sitemap.xml
            routes.MapRoute(name: "Sitemap", url: "sitemap.xml", defaults: new { controller = "Home", action = "Sitemap" });

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}