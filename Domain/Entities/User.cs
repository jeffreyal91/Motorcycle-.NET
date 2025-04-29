using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Entities
{
    public class User : EntityBase
    {
        public virtual string Email { get; protected set; }
        public virtual string PasswordHash { get; protected set; }
        public virtual ICollection<Role> Roles { get; protected set; } = new List<Role>();

        protected User() { } // Constructor para NHibernate

        public User(string email, string passwordHash)
        {
            Email = email ?? throw new ArgumentNullException(nameof(email));
            PasswordHash = passwordHash ?? throw new ArgumentNullException(nameof(passwordHash));
        }

        // Métodos para gestión de roles
        public virtual void AssignRole(Role role)
        {
            if (role == null) 
                throw new ArgumentNullException(nameof(role));

            if (!Roles.Any(r => r.Id == role.Id))
            {
                Roles.Add(role);
            }
        }

        public virtual void RemoveRole(Role role)
        {
            if (role == null)
                throw new ArgumentNullException(nameof(role));

            var roleToRemove = Roles.FirstOrDefault(r => r.Id == role.Id);
            if (roleToRemove != null)
            {
                Roles.Remove(roleToRemove);
            }
        }

        public virtual void RemoveRole(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
                throw new ArgumentException("Role name cannot be empty", nameof(roleName));

            var role = Roles.FirstOrDefault(r => 
                r.Name.Equals(roleName, StringComparison.OrdinalIgnoreCase));
            
            if (role != null)
            {
                Roles.Remove(role);
            }
        }

        public virtual bool HasRole(Role role)
        {
            return role != null && Roles.Any(r => r.Id == role.Id);
        }

        public virtual bool HasRole(string roleName)
        {
            return !string.IsNullOrWhiteSpace(roleName) && 
                   Roles.Any(r => r.Name.Equals(roleName, StringComparison.OrdinalIgnoreCase));
        }

        // Métodos para actualizar propiedades
        public virtual void UpdateEmail(string newEmail)
        {
            Email = newEmail ?? throw new ArgumentNullException(nameof(newEmail));
        }

        public virtual void UpdatePasswordHash(string newPasswordHash)
        {
            PasswordHash = newPasswordHash ?? throw new ArgumentNullException(nameof(newPasswordHash));
        }
    }
}