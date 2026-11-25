using System;

namespace ACE.Server.Network
{
    /// <summary>
    /// Account names are found through the database's case-insensitive collation, so an account is one name whatever its
    /// case: the name a client sends with its character requests is the session's account when it matches that way.
    /// </summary>
    public static class AccountNames
    {
        public static bool Match(string clientAccount, string sessionAccount)
        {
            return string.Equals(clientAccount, sessionAccount, StringComparison.OrdinalIgnoreCase);
        }
    }
}
