﻿using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WorkerRole.Jobs
{
    abstract class EmailingJobBase : IJob
    {
        public abstract void Execute(IJobExecutionContext context);

        protected string GetEmailHeader()
        {
            return @"<!DOCTYPE html>
<html lang='en' style='margin: 0; padding: 0;'>
<head>
    <meta charset='utf-8' />
    <meta name='viewport' content='width=device-width' />
</head>
<body style='background-color: #eaeaea;'>
    <div style=""border-style: solid; border-width: 1px; font-family: 'Open Sans', Arial, Helvetica, sans-serif; font-weight: normal; font-size: 11pt; top: 4px; bottom: 4px; width: 960px; margin: 0px auto; box-shadow: 0px 0px 10px 1px #333333; background-color: #ffffff"">
        <div id='header'>
            <div id='logo-bar' style='padding: 4px 10px 0px 10px;'>
                <table style='width: 100%; padding: 0px; margin: 0px;'>
                    <tr>
                        <td>
                            <a href='http://www.stockwinners.com'><img src='http://stockwinners.com/Images/Logo.gif' alt='Stockwinners.com' style='border-width: 0px;' /></a>
                        </td>
                        <td style='text-align: right;'>
                            <a href='http://www.stockwinners.com/members' style='color: #000000; font-size: 10pt;'>Unsubscribe From Email Alerts</a>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
        <div id='contents' style='padding: 10px;'>
            <!-- BODY START -->
            <div id='body'>";
        }

        protected string GetEmailFooter()
        {
            return @"
            <!-- BODY END -->
            </div>
        </div>
        <div style='background-color: #454545; font-size: 9pt; color: #cccccc; padding: 10px;'>
            <div>For feedback, questions or comments, please email <a href='mailto:info@stockwinners.com' style='color: #cccccc;'>info@stockwinners.com</a>.</div>
            <div>The information contained in this report may not be published, broadcast, rewritten or otherwise distributed without prior written authority of Stockwinners.com, Inc.</div>
            <div style='text-align: center;'><table style='margin: 0px auto;'><tr><td><a href='https://twitter.com/stockwinnerscom' style='color: #cccccc; text-decoration: none;'><img src='http://www.stockwinners.com/Images/Social%20Networks/twitter-email.png' style='border: none; vertical-align: middle; padding-right: 5px;' />Follow us on Twitter</a></td><td><a href='http://www.facebook.com/pages/STOCKWINNERScom/210099385707437' style='color: #cccccc; text-decoration: none;'><img src='http://www.stockwinners.com/Images/Social%20Networks/facebook-16.png' style='border: none; vertical-align: middle; padding-right: 5px;'/>Like us on Facebook</a></td></tr></table></div>
            <div style='text-align: center;'><table style='margin: 0px auto;'><tr><td style='padding-right: 5px;'>Address: P.O. Box 10815, Austin, Texas 78766, USA</td><td>Phone: 1-972-638-7233</td></tr></table></div>
            <div style='text-align: center;'>&copy; 2012 - Stockwinners.com</div>
        </div>
    </div>
</body>
</html>";
        }
    }
}
