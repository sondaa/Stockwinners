namespace Stockwinners
{
    using System;

    public interface IUser
    {
        string EmailAddress { get; set; }
        string FirstName { get; set; }
        int IdentityProvider { get; set; }
        string IdentityProviderIssuedUserId { get; set; }
        bool IsBanned { get; set; }
        bool IsTrialValid();
        DateTime LastLoginDate { get; set; }
        string LastName { get; set; }
        DateTime SignUpDate { get; set; }
        DateTime? SubscriptionExpiryDate { get; set; }
        int? SubscriptionId { get; set; }
        DateTime TrialExpiryDate { get; set; }
        int UserId { get; set; }
    }
}
