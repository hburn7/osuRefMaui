using Newtonsoft.Json;

namespace osuRefMaui.Core.IRC.LoginInformation
{
	public static class CredentialsHandler
	{
        private static readonly string CredentialsPath = Path.Join(Path.GetTempPath(), "credentials.json");

        public static Credentials DeserializeCredentials()
        {
            if (File.Exists(CredentialsPath))
            {
                try
                {
                    string json = File.ReadAllText(CredentialsPath);
                    return JsonConvert.DeserializeObject<Credentials>(json) ?? throw new InvalidOperationException();
                }
                catch (InvalidOperationException)
                {
                }
            }

            SerializeCredentials(new Credentials());
            return DeserializeCredentials();
        }

        public static void SerializeCredentials(Credentials credentials)
        {
            // Todo: Hash password / store securely
            string json = JsonConvert.SerializeObject(credentials, Formatting.Indented);
            File.WriteAllText(CredentialsPath, json);
        }
}
}

