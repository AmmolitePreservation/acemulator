
namespace ACE.Common
{
    public class AccountDefaults
    {
        /// <summary>
        /// whether or not the server uses account level permissions or character level permissions (as AC retail did). for backwards compatibility, this is true
        /// by default.
        /// </summary>
        public bool OverrideCharacterPermissions { get; set; } = true;

        /// <summary>
        /// Default AccessLevel for new accounts. Used when accesslevel is not specified by user.  for backwards compatibility, this is 0 (Player)
        /// by default.
        /// </summary>
        public uint DefaultAccessLevel { get; set; } = 0;

        /// <summary>
        /// whether or not this server creates new accounts when one does not exist.  for backwards compatibility, this is true
        /// by default.
        /// </summary>
        public bool AllowAutoAccountCreation { get; set; } = true;

        /// <summary>
        /// Default WorkFactor for account passwords.
        /// 8 by default.
        /// </summary>
        public int PasswordHashWorkFactor { get; set; } = 8;

        /// <summary>
        /// Upgrade or downgrade passwords to match PasswordHashWorkFactor specified in config 
        /// True by default.
        /// </summary>
        public bool ForceWorkFactorMigration { get; set; } = true;

        /// <summary>
        /// Whether several sessions may log in to one account at once, each playing its own character; a character is
        /// still in the world only once. True by default.
        /// </summary>
        public bool AllowSeveralSessionsPerAccount { get; set; } = true;

        /// <summary>
        /// When set, every password login is checked with this site address (an https POST of the name, the password and the
        /// player's address) instead of the account's own password, and the site names the account the login belongs to.
        /// Empty by default: the account's own password is checked.
        /// </summary>
        public string SiteLoginCheckUrl { get; set; } = "";

        /// <summary>
        /// The site's token for this server, sent with every site login check. Required when SiteLoginCheckUrl is set.
        /// </summary>
        public string SiteLoginCheckToken { get; set; } = "";
    }
}
