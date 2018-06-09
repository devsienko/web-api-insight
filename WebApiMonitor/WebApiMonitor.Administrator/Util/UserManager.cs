using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Microsoft.AspNet.Identity;
using WebApiMonitor.Administrator.Models;

namespace WebApiMonitor.Administrator
{
    public class UserManager : IUserStore<ApplicationUser>, IUserPasswordStore<ApplicationUser, string>, 
        IUserLockoutStore<ApplicationUser, string>
    {
        private readonly string ConfigPath = Path.Combine(PathHelper.GetAssemblyLocation(), "Conf/users.json");

        public Task CreateAsync(ApplicationUser user)
        {
            var users = GetUsers();
            user.Id = Guid.NewGuid().ToString();
            user.CreatingDate = DateTime.Now;
            user.UserName = user.Email;
            users.Add(user);
            var json = new JavaScriptSerializer().Serialize(users);
            File.WriteAllText(ConfigPath, json);
            return Task.FromResult(0);

        }

        public Task DeleteAsync(ApplicationUser user)
        {
            throw new NotImplementedException();
        }

        public Task<ApplicationUser> FindByIdAsync(string userId)
        {
            var users = GetUsers();
            var result = users.FirstOrDefault(u => u.Id == userId);
            return Task.FromResult(result);
        }

        public Task<ApplicationUser> FindByNameAsync(string email)
        {
            var users = GetUsers();
            var result = users.FirstOrDefault(u => u.Email == email);
            return Task.FromResult(result);
        }

        public List<ApplicationUser> GetUsers()
        {
            var jsonConfig = File.ReadAllText(ConfigPath);
            var deserializer = new JavaScriptSerializer();
            var result = deserializer.Deserialize<ApplicationUser[]>(jsonConfig);
            if (result == null)
                return new List<ApplicationUser>();
            return result.ToList();
        }

        public void RemoveByIds(string[] ids)
        {
            var users = GetUsers();
            var remain = users.Where(a => !ids.Contains(a.Id)).ToArray();
            var json = new JavaScriptSerializer().Serialize(remain);
            File.WriteAllText(ConfigPath, json);
        }

        public Task UpdateAsync(ApplicationUser user)
        {
            throw new NotImplementedException();
        }

        public Task SetPasswordHashAsync(ApplicationUser user, string passwordHash)
        {
            user.PasswordHash = passwordHash;
            return Task.FromResult(0);
        }

        public Task<string> GetPasswordHashAsync(ApplicationUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(ApplicationUser user)
        {
            throw new NotImplementedException();
        }
        
        public Task<DateTimeOffset> GetLockoutEndDateAsync(ApplicationUser user)
        {
            throw new NotImplementedException();
        }

        public Task SetLockoutEndDateAsync(ApplicationUser user, DateTimeOffset lockoutEnd)
        {
            throw new NotImplementedException();
        }

        public Task<int> IncrementAccessFailedCountAsync(ApplicationUser user)
        {
            throw new NotImplementedException();
        }

        public Task ResetAccessFailedCountAsync(ApplicationUser user)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetAccessFailedCountAsync(ApplicationUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return Task.FromResult(user.AccessFailedCount);
        }

        public Task<bool> GetLockoutEnabledAsync(ApplicationUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return Task.FromResult(user.LockoutEnabled);
        }

        public Task SetLockoutEnabledAsync(ApplicationUser user, bool enabled)
        {
            throw new NotImplementedException();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Test() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
