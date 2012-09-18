﻿using ActionMailer.Net.Mvc;
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
    }
}