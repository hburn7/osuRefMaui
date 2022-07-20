using IrcDotNet;
using Microsoft.Extensions.Logging;
using osuRefMaui.Core.IRC.Interfaces;
using osuRefMaui.Core.IRC.LoginInformation;

namespace osuRefMaui.Core.IRC
{
	public class ConnectionHandler
	{
		private readonly ChatQueue _chatQueue;
		private readonly StandardIrcClient _client;
		private readonly Credentials _credentials;
		private readonly ILogger<ConnectionHandler> _logger;
		private readonly TabHandler _tabHandler;

		public ConnectionHandler(ILogger<ConnectionHandler> logger, TabHandler tabHandler,
			StandardIrcClient client, Credentials credentials, ChatQueue chatQueue)
		{
			_logger = logger;
			_tabHandler = tabHandler;
			_client = client;
			_credentials = credentials;
			_chatQueue = chatQueue;

			Connectivity.ConnectivityChanged += OnConnectionChanged;

			_client.Connected += (_, _) => _logger.LogInformation("Client connected");
			_client.Disconnected += (_, _) => _logger.LogInformation("Client disconnected");

			_tabHandler.OnTabCreated += channel =>
			{
				if (!channel.Equals(TabHandler.DefaultTabName))
				{
					JoinChannel(channel);
				}
			};
		}

		/// <summary>
		///  Attempts to connect to osu!Bancho (IRC)
		/// </summary>
		/// <returns>
		///  True if the connection is already established or a new
		///  one has been made. False if the user has no internet access
		///  or cannot connect.
		/// </returns>
		public async Task<bool> Connect()
		{
			var regInfo = new IrcUserRegistrationInfo
			{
				UserName = _credentials.Username,
				NickName = _credentials.Username,
				Password = _credentials.IrcPassword
			};

			if (_client.IsConnected)
			{
				return true;
			}

			bool success = false;
			if (Connectivity.NetworkAccess == NetworkAccess.Internet)
			{
				void StatusCheck(IChatMessage m)
				{
					// Invalid credentials
					success = !m.IsStatusCode(464);

					_logger.LogInformation($"Message received: {m}");
					_logger.LogInformation($"success = {success}");
				}

				return await Task.Run(async () =>
				{
					try
					{
						_client.Connect("irc.ppy.sh", false, regInfo);

						_chatQueue.OnEnqueue += StatusCheck;

						int retries = 10;
						while (!success)
						{
							if (retries == 0)
							{
								break;
							}

							_logger.LogInformation($"Awaiting connection. {retries} attempts remaining.");
							await Task.Delay(1000);

							retries -= 1;
						}

						_chatQueue.OnEnqueue -= StatusCheck;
					}
					catch (Exception)
					{
						return false;
					}

					return _client.IsConnected; // If the connection failed, this will be false.
				});
			}

			// User is unable to access the internet
			return false;
		}

		/// <summary>
		///  Joins an IRC channel or queries a user.
		/// </summary>
		/// <param name="channel">Name of the channel (e.g. #osu) or name of user (e.g. Stage)</param>
		public void JoinChannel(string channel)
		{
			if (!_client.IsConnected)
			{
				_logger.LogWarning($"Attempted to join channel {channel} but the client is disconnected.");
				return;
			}

			if (channel.StartsWith("#"))
			{
				_client.Channels.Join(channel);
			}
			else
			{
				_client.QueryWho(channel);
			}
		}

		/// <summary>
		///  Triggered whenever connectivity status changes. Attempts to reconnect
		///  the user to bancho if needed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <exception cref="NotImplementedException"></exception>
		private void OnConnectionChanged(object sender, ConnectivityChangedEventArgs e)
		{
			// todo: Reconnect if needed
		}
	}
}