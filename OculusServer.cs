using System.ComponentModel;
using System.Text.Json;
using System.Net;

namespace RecRoom
{
    public class OculusServer
    {
        private static readonly string AppSecret = "paste your app secret";
        private static readonly HttpClient Http = new();

        public static async Task<bool> VerifyNonceAsync(ulong userId, string nonce)
        {
            var content = new FormUrlEncodedContent(
            [
                new KeyValuePair<string, string>("access_token", AppSecret),
                new KeyValuePair<string, string>("nonce", nonce),
                new KeyValuePair<string, string>("user_id", userId.ToString())
            ]);

            HttpResponseMessage resp = await Http.PostAsync("https://graph.oculus.com/user_nonce_validate", content);
            if (!resp.IsSuccessStatusCode)
                return false;

            string respJson = await resp.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(respJson);
            if (doc.RootElement.TryGetProperty("is_valid", out JsonElement validEl))
            {
                return validEl.GetBoolean();
            }

            return false;
        }

        public static async Task<User?> GetUserAsync(ulong userId)
        {
            var uri = $"https://graph.oculus.com/{userId}?fields=org_scoped_id,alias,display_name&access_token={AppSecret}";

            HttpResponseMessage resp = await Http.GetAsync(uri);
            if (resp.StatusCode != HttpStatusCode.OK)
            {
                if (userId == your oculus id)
                {
                    return new User
                    {
                        Alias = "Coach"
                    };
                }

                return null;
            }

            string respJson = await resp.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(respJson);

            string? alias = "default";
            if (doc.RootElement.TryGetProperty("alias", out JsonElement aliasEl))
            {
                alias = aliasEl.GetString();
            }

            return new User
            {
                Alias = alias
            };
        }

        public class User
        {
            public string? Alias;
        }

        public enum UserPresenceStatus
        {
            [Description("UNKNOWN")]
            Unknown,
            [Description("ONLINE")]
            Online,
            [Description("OFFLINE")]
            Offline
        }
    }
}
