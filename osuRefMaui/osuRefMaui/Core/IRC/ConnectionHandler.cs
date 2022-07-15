using IrcDotNet;
using osuRefMaui.Core.IRC.LoginInformation;

namespace osuRefMaui.Core.IRC
{
	public class ConnectionHandler
	{
        private readonly StandardIrcClient _client;
        private readonly Credentials _credentials;

        public ConnectionHandler(StandardIrcClient client, Credentials credentials)
        {
            _client = client;
            _credentials = credentials;
            NetworkAccess = Connectivity.NetworkAccess;

            Connectivity.ConnectivityChanged += OnConnectionChanged;
        }

        public NetworkAccess NetworkAccess { get; private set; }

        /// <summary>
        /// Conencts the client with the given credentials.
        /// </summary>
        /// <returns>True if the connection is already established or a new
        /// one has been made. False if the user has no internet access
        /// or cannot connect.</returns>
        public bool Connect()
        {
            var regInfo = new IrcUserRegistrationInfo
            {
                UserName = _credentials.Username,
                NickName = _credentials.Username,
                Password = _credentials.IrcPassword
            };

            if(_client.IsConnected)
            {
                return true;
            }

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                try
                {
                    _client.Connect("irc.ppy.sh", false, regInfo);

                    // todo: check for successful login
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            
            // User is unable to access the internet
            return false;
        }

        /// <summary>
        /// Triggered whenever connectivity status changes. Attempts to reconnect
        /// the user to bancho if needed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void OnConnectionChanged(object sender, ConnectivityChangedEventArgs e)
        {
            // todo: Reconnect if needed
            NetworkAccess = e.NetworkAccess;
            
            MessagingCenter.Send<ConnectionHandler, NetworkAccess>(this, "Connectivity changed", NetworkAccess);
        }
    }
}

