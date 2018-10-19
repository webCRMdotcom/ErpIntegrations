using Newtonsoft.Json;

namespace Webcrm.ErpIntegrations.ApiClients.PowerofficeApiClient
{
    internal class AuthorizationTokens
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }
    }
}