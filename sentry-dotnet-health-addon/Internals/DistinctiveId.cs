using Sentry.Protocol;
using System;
using System.Security.Cryptography;
using System.Text;

namespace sentry_dotnet_health_addon.Internals
{
    internal class DistinctiveId
    {
        internal string GetDistinctiveId(string identifier)
        {
            return HashString(identifier);
        }
        internal string GetDistinctiveId(User user)
        {
            if(user?.Id != null)
                return HashString(user.Id);
            if (user?.Email != null)
                return HashString(user.Email);
            if (user?.Username != null)
                return HashString(user.Username);
            return null;
        }

        private string HashString(string @string)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(@string);
            SHA256Managed hashstring = new SHA256Managed();
            byte[] hash = hashstring.ComputeHash(bytes);
            string hashString = string.Empty;
            foreach (byte x in hash)
            {
                hashString += String.Format("{0:x2}", x);
            }
            return hashString;
        }
    }
}
