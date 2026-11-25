using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using log4net;

using ACE.Common;

namespace ACE.Server.Network.Handlers
{
    public enum SiteLoginOutcome
    {
        NotChecked,
        Accepted,
        Refused,
        LockedOut,
        Unavailable
    }

    /// <summary>
    /// Checks a password login with the site that keeps the players' names and passwords (Ammolite's login server check):
    /// the name and password a player gave, or a one-time login key the site issued, and the player's address. The site
    /// answers with the account the login belongs to, made in this server's database at the first good login.
    /// </summary>
    public static class SiteLogin
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly HttpClient client = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };

        public static bool IsEnabled => !string.IsNullOrEmpty(ConfigManager.Config.Server.Accounts.SiteLoginCheckUrl);

        /// <summary>
        /// Fails the server's start when the site check is half set up or not over https: the player's password goes to it.
        /// </summary>
        public static void RequireValidConfiguration()
        {
            if (!IsEnabled)
                return;

            var accounts = ConfigManager.Config.Server.Accounts;
            if (!accounts.SiteLoginCheckUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Server.Accounts.SiteLoginCheckUrl must be an https address: the player's password goes to it.");
            if (string.IsNullOrEmpty(accounts.SiteLoginCheckToken))
                throw new InvalidOperationException("Server.Accounts.SiteLoginCheckUrl is set, so Server.Accounts.SiteLoginCheckToken must be too.");
        }

        public static SiteLoginOutcome Check(string username, string password, IPAddress playerAddress, out string accountName)
        {
            accountName = null;
            var accounts = ConfigManager.Config.Server.Accounts;
            var body = JsonSerializer.Serialize(new { username, password, playerAddress = playerAddress.ToString() });

            using var request = new HttpRequestMessage(HttpMethod.Post, accounts.SiteLoginCheckUrl);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accounts.SiteLoginCheckToken);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");

            HttpResponseMessage response;
            try
            {
                response = client.Send(request);
            }
            catch (Exception ex) when (ex is HttpRequestException || ex is TaskCanceledException)
            {
                log.Error($"Site login check for {username} could not reach the site: {ex.Message}");
                return SiteLoginOutcome.Unavailable;
            }

            using (response)
            {
                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                        using (var answer = JsonDocument.Parse(response.Content.ReadAsStream()))
                        {
                            if (!answer.RootElement.TryGetProperty("accountName", out var name) || name.ValueKind != JsonValueKind.String || name.GetString() == "")
                            {
                                log.Error($"Site login check for {username} accepted it but named no account");
                                return SiteLoginOutcome.Unavailable;
                            }
                            accountName = name.GetString();
                        }
                        log.Info($"Site login check accepted {username} as {accountName}");
                        return SiteLoginOutcome.Accepted;
                    case HttpStatusCode.Unauthorized:
                        log.Info($"Site login check refused {username}");
                        return SiteLoginOutcome.Refused;
                    case HttpStatusCode.TooManyRequests:
                        log.Warn($"Site login check: {username} is locked out for now");
                        return SiteLoginOutcome.LockedOut;
                    default:
                        log.Error($"Site login check for {username} answered {(int)response.StatusCode}");
                        return SiteLoginOutcome.Unavailable;
                }
            }
        }
    }
}
