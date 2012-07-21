using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebSite.Models
{
    /// <summary>
    /// Tracks roles the users can have such as member, administrator, moderator, etc.
    /// </summary>
    public class Role
    {
        [Key]
        [Required]
        public int RoleId { get; set; }

        [Required]
        [MaxLength(20)]
        public string Name { get; set; }

        // There are a set of Users belonging to each role
        public virtual ICollection<User> Users { get; set; }
    }

    public static class PredefinedRoles
    {
        public static string Member = "Member";
        public static string Administrator = "Administrator";
    }
}