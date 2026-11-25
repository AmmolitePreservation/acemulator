using Microsoft.VisualStudio.TestTools.UnitTesting;

using ACE.Server.Network;

namespace ACE.Server.Tests
{
    [TestClass]
    public class AccountNamesTests
    {
        [TestMethod]
        public void TheClientsLowercasedNameMatchesTheAccountRowsCapitalizedName()
        {
            Assert.IsTrue(AccountNames.Match("claudetestuser", "claudeTestUser"));
        }

        [TestMethod]
        public void AnotherAccountsNameDoesNotMatch()
        {
            Assert.IsFalse(AccountNames.Match("claudetestuser2", "claudeTestUser"));
        }
    }
}
