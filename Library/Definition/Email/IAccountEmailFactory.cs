using ActionMailer.Net.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stockwinners.Email
{
    public interface IAccountEmailFactory
    {
        /// <summary>
        /// Email to send to members when their trial membership expired.
        /// </summary>
        EmailResult TrialExpired();

        /// <summary>
        /// Email to send to members when they have signed up for a trial account but have not been checking the page actively.
        /// </summary>
        EmailResult InactiveTrialAccount();

        /// <summary>
        /// Email to send to members who have not signed up for our service after their trial has expired.
        /// </summary>
        EmailResult LostUserFeedback();
    }
}
