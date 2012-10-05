namespace Stockwinners
{
    using Stockwinners.Email;
    using System;

    public interface IUser : IEmailRecipient
    {
        string FirstName { get; }
        string LastName { get; }
        int IdentityProvider { get; set; }
        string IdentityProviderIssuedUserId { get; set; }
        bool IsBanned { get; set; }
        bool IsTrialValid();
        DateTime LastLoginDate { get; set; }
        DateTime SignUpDate { get; set; }
        DateTime? SubscriptionExpiryDate { get; set; }
        int? SubscriptionId { get; set; }
        DateTime TrialExpiryDate { get; set; }
        int UserId { get; set; }
        bool SentInactiveReminder { get; set; }
        bool SentFeedbackRequest { get; set; }
        bool SentTrialExpiryEmail { get; set; }
    }
}
