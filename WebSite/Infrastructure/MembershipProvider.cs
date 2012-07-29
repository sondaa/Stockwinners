using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Security;
using WebSite.Database;
using WebSite.Models;

namespace WebSite.Infrastructure
{
    /// <summary>
    /// Provides membership support for Stockwinners member *only*. Unified users from all sources (Facebook, Google, Stockwinners) are handled 
    /// differently and stored in the database using the <see cref="User"/> model class.
    /// </summary>
    public class MembershipProvider : System.Web.Security.MembershipProvider
    {
        public override string ApplicationName { get; set; }

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            if (this.ValidateUser(username, oldPassword))
            {
                DatabaseContext db = DatabaseContext.GetInstance();

                StockwinnersMember member = db.StockwinnersMembers.First(m => m.EmailAddress == username);

                member.Password = MembershipProvider.HashPassword(newPassword);

                // Update the legacy member's bit so that the next time the new hash algorithm is used to verify the user's password
                member.IsLegacyMember = false;

                db.SaveChanges();

                return true;
            }

            return false;
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new NotSupportedException();
        }

        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            throw new NotSupportedException();
        }

        public int CreateStockwinnersMember(string emailAddress, string password, string firstName, string lastName, out MembershipCreateStatus status)
        {
            status = MembershipCreateStatus.Success;
            DatabaseContext db = DatabaseContext.GetInstance();

            // Is member unique?
            if (db.StockwinnersMembers.FirstOrDefault(m => m.EmailAddress == emailAddress) != null)
            {
                status = MembershipCreateStatus.DuplicateEmail;

                return 0;
            }

            StockwinnersMember newMember = new StockwinnersMember()
            {
                EmailAddress = emailAddress,
                FirstName = firstName,
                LastName = lastName,
                Password = MembershipProvider.HashPassword(password),
                IsLegacyMember = false
            };

            db.StockwinnersMembers.Add(newMember);

            db.SaveChanges();

            return newMember.MemberId;
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            throw new NotImplementedException();
        }

        public override bool EnablePasswordReset
        {
            get { return false; }
        }

        public override bool EnablePasswordRetrieval
        {
            get { return false; }
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            MembershipUserCollection results = new MembershipUserCollection();
            totalRecords = 0;
            DatabaseContext db = DatabaseContext.GetInstance();
            StockwinnersMember member = db.StockwinnersMembers.FirstOrDefault(m => m.EmailAddress == emailToMatch);

            if (member != null)
            {
                results.Add(MembershipProvider.GetMembershipUser(member));
                totalRecords = 1;
            }

            return results;
        }


        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            // Username and email are treated the same by our membership provider. We use emails as user names.
            return this.FindUsersByEmail(usernameToMatch, pageIndex, pageSize, out totalRecords);
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            totalRecords = 0;
            MembershipUserCollection members = new MembershipUserCollection();

            foreach (StockwinnersMember member in DatabaseContext.GetInstance().StockwinnersMembers)
            {
                members.Add(MembershipProvider.GetMembershipUser(member));
                totalRecords++;
            }

            return members;
        }

        public override int GetNumberOfUsersOnline()
        {
            throw new NotSupportedException();
        }

        public override string GetPassword(string username, string answer)
        {
            throw new NotSupportedException();
        }

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            int count = 0;
            MembershipUserCollection matches = this.FindUsersByEmail(username, 0, 0, out count);

            if (count > 0)
            {
                return matches[username];
            }

            return null;
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            throw new NotSupportedException();
        }

        public override string GetUserNameByEmail(string email)
        {
            // We use email address and username the same
            return email;
        }

        public override int MaxInvalidPasswordAttempts
        {
            get { return 3; }
        }

        public override int MinRequiredNonAlphanumericCharacters
        {
            get { return 0; }
        }

        public override int MinRequiredPasswordLength
        {
            get { return 6; }
        }

        public override int PasswordAttemptWindow
        {
            get { return 5; }
        }

        public override MembershipPasswordFormat PasswordFormat
        {
            get { return MembershipPasswordFormat.Hashed; }
        }

        public override string PasswordStrengthRegularExpression
        {
            get { return string.Empty; }
        }

        public override bool RequiresQuestionAndAnswer
        {
            get { return false; }
        }

        public override bool RequiresUniqueEmail
        {
            get { return true; }
        }

        public override string ResetPassword(string username, string answer)
        {
            throw new NotSupportedException();
        }

        public override bool UnlockUser(string userName)
        {
            throw new NotSupportedException();
        }

        public override void UpdateUser(MembershipUser user)
        {
            throw new NotSupportedException();
        }

        public override bool ValidateUser(string username, string password)
        {
            StockwinnersMember member = DatabaseContext.GetInstance().StockwinnersMembers.FirstOrDefault(m => m.EmailAddress == username); 
            
            return member != null && MembershipProvider.VerifyPassword(password, member.Password, member.IsLegacyMember);
        }

        public StockwinnersMember GetStockwinnersMember(string emailAddress, string password)
        {
            StockwinnersMember member = DatabaseContext.GetInstance().StockwinnersMembers.FirstOrDefault(m => m.EmailAddress == emailAddress);

            if (member != null && MembershipProvider.VerifyPassword(password, member.Password, member.IsLegacyMember))
            {
                return member;
            }

            return null;
        }

        #region Private Helpers

        private static MembershipUser GetMembershipUser(StockwinnersMember member)
        {
            // We don't supply any real dates as those are stored in the real User data model
            return new MembershipUser("DefaultMembershipProvider", member.EmailAddress, null, member.EmailAddress, null,
                null, true, false, DateTime.UtcNow, DateTime.UtcNow, DateTime.UtcNow, DateTime.UtcNow, DateTime.UtcNow);
        }

        private static string HashPassword(string rawPassword)
        {
            using (SHA256 hash = SHA256.Create())
            {
                return Convert.ToBase64String(hash.ComputeHash(Encoding.UTF8.GetBytes(rawPassword)));
            }
        }

        /// <summary>
        /// Verifies that the provided plain text password matches that of the hashed value stored in the database. If the member is a legacy member
        /// then we use the old Stockwinners' Apache hashing method to check the value. This should only be used for the set of members that have been ported
        /// from the old Stockwinners' database.
        /// </summary>
        private static bool VerifyPassword(string rawPassword, string hashedValue, bool isLegacyMember)
        {
            if (isLegacyMember)
            {
                return Helpers.ApacheEncryption.VerifyPassword(rawPassword, hashedValue);
            }
            else
            {
                return MembershipProvider.HashPassword(rawPassword) == hashedValue;
            }
        }

        #endregion
    }
}