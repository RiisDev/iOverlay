using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.IO;
using iOverlay.Logic.ValorantApi.Clients;
using iOverlay.Logic.ValorantApi.DataTypes;

namespace iOverlay.Logic.ValorantApi.Methods
{
    public class NetHandler
    {
        internal HttpClient Client = new(GetHttpClientHandler());
        internal UserClient User;

        public NetHandler(UserClient user)
        {
            User = user;
            Client.DefaultRequestHeaders.TryAddWithoutValidation("X-Riot-ClientPlatform", "ew0KCSJwbGF0Zm9ybVR5cGUiOiAiUEMiLA0KCSJwbGF0Zm9ybU9TIjogIldpbmRvd3MiLA0KCSJwbGF0Zm9ybU9TVmVyc2lvbiI6ICIxMC4wLjE5MDQyLjEuMjU2LjY0Yml0IiwNCgkicGxhdGZvcm1DaGlwc2V0IjogIlVua25vd24iDQp9");
            Client.DefaultRequestHeaders.TryAddWithoutValidation("X-Riot-ClientVersion", User.ValorantClient.ValorantClientVersion.RiotClientVersion);
            Client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "ShooterGame/13 Windows/10.0.19043.1.256.64bit");
        }

        private static HttpClientHandler GetHttpClientHandler()
        {
            return new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (_, _, _, _) => true
            };
        }

        private static UserAuth? GetAuth()
        {
            string lockFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "AppData", "Local", "Riot Games", "Riot Client", "Config", "lockfile");
            string? fileText;
            try
            {
                File.Copy(lockFile, $"{lockFile}.tmp", true);
                using FileStream fs = new($"{lockFile}.tmp", FileMode.Open, FileAccess.Read, FileShare.Read);
                using StreamReader reader = new(fs);
                fileText = reader.ReadToEnd();
            }
            finally
            {
                File.Delete($"{lockFile}.tmp");
            }

            string[] fileValues = fileText.Split(':');

            if (fileValues.Length < 3) return null;

            int authPort = int.Parse(fileValues[2]);
            string oAuth = fileValues[3];

            return new UserAuth(lockFile, authPort, oAuth);
        }

        private async Task<(string, string)> GetAuthorizationToken()
        {
            UserAuth? auth = GetAuth();
            string toEncode = $"riot:{auth?.OAuth}";
            byte[] stringBytes = Encoding.UTF8.GetBytes(toEncode);
            string base64Encode = Convert.ToBase64String(stringBytes);
            Client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse($"Basic {base64Encode}");
            HttpResponseMessage response = await Client.GetAsync($"https://127.0.0.1:{auth?.AuthorizationPort}/entitlements/v1/token");

            if (!response.IsSuccessStatusCode) return ("", $"Failed to get entitlement | {response.StatusCode} | {response.Content.ReadAsStringAsync().Result}");

            Entitlement? entitlement = JsonSerializer.Deserialize<Entitlement>(response.Content.ReadAsStringAsync().Result);

            return (entitlement?.AccessToken ?? "", entitlement?.Token ?? "");
        }

        public async Task<string?> GetAsync(string baseAddress, string endpoint)
        {
            Client.DefaultRequestHeaders.Remove("X-Riot-Entitlements-JWT");
            Client.DefaultRequestHeaders.Remove("Authorization");

            (string, string) authTokens = await GetAuthorizationToken();

            if (string.IsNullOrEmpty(authTokens.Item1)) return "Failed, missing auth";

            Client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse($"Bearer {authTokens.Item1}");
            Client.DefaultRequestHeaders.TryAddWithoutValidation("X-Riot-Entitlements-JWT", authTokens.Item2);

            HttpResponseMessage response = await Client.GetAsync($"{baseAddress}{endpoint}");

            if (!response.IsSuccessStatusCode) return $"Failed: {response.StatusCode} | {response.Content.ReadAsStringAsync().Result}";

            return await response.Content.ReadAsStringAsync();
        }
    }
}