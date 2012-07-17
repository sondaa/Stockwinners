using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using WebSite.Database;
using WebSite.Models;

namespace WebSite.Infrastructure
{
    /// <summary>
    /// Implements a role provider over the unified range of users from variety of sources (Stockwinners, Facebook, Google, etc.)
    /// </summary>
    public class RoleProvider : System.Web.Security.RoleProvider
    {
        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            DatabaseContext db = DatabaseContext.GetInstance();
            
            for (int usernameIndex = 0; usernameIndex < usernames.Length; usernameIndex++)
            {
                User user = db.Users.FirstOrDefault(u => u.EmailAddress == usernames[usernameIndex]);

                if (user != null)
                {
                    for (int roleIndex = 0; roleIndex < roleNames.Length; roleIndex++)
                    {
                        Role role = db.Roles.FirstOrDefault(r => r.Name == roleNames[roleIndex]);

                        if (role != null)
                        {
                            user.Roles.Add(role);
                        }
                    }
                }
            }

            db.SaveChanges();
        }

        public override string ApplicationName { get; set; }

        public override void CreateRole(string roleName)
        {
            DatabaseContext db = DatabaseContext.GetInstance();

            db.Roles.Add(new Role() { Name = roleName });
            db.SaveChanges();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotSupportedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotSupportedException();
        }

        public override string[] GetAllRoles()
        {
            return (from role in DatabaseContext.GetInstance().Roles select role.Name).ToArray();
        }

        public override string[] GetRolesForUser(string username)
        {
            DatabaseContext db = DatabaseContext.GetInstance();

            User user = db.Users.FirstOrDefault(u => u.EmailAddress == username);

            if (user != null)
            {
                return (from role in user.Roles select role.Name).ToArray();
            }

            return null;
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotSupportedException();
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            DatabaseContext db = DatabaseContext.GetInstance();

            User user = db.Users.FirstOrDefault(u => u.EmailAddress == username);

            if (user != null)
            {
                return user.Roles.FirstOrDefault(role => role.Name == roleName) != null;
            }

            return false;
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            DatabaseContext db = DatabaseContext.GetInstance();

            foreach (string username in usernames)
            {
                User user = db.Users.FirstOrDefault(u => u.EmailAddress == username);

                if (user != null)
                {
                    foreach (string role in roleNames)
                    {
                        List<Role> userRoles = user.Roles.ToList();
                        user.Roles.Clear();

                        foreach (Role userRole in userRoles)
                        {
                            if (userRole.Name != role)
                            {
                                user.Roles.Add(userRole);
                            }
                        }
                    }
                }
            }

            db.SaveChanges();
        }

        public override bool RoleExists(string roleName)
        {
            return DatabaseContext.GetInstance().Roles.FirstOrDefault(role => role.Name == roleName) != null;
        }
    }
}