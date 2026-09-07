using System;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using ChessLogic.Enum;

namespace ChessLogic
{
	public class ChessClient
	{
		public string UserName { get; }
		public string LocalIp { get; }
		public int Port { get; private set; }
		public string Host { get; private set; }

		public event Action<string> ConnectionEstablished;
		public event Action<string> ConnectionFailed;
		public event Action<string> MoveRejected;
		public event Action<GameStateDto> StateUpdated;

		private TcpClient client;
		private StreamWriter writer;


        public ChessClient()
		{
			UserName = Environment.UserName;
			LocalIp = GetLocalIp();

			NetworkConfig config = NetworkConfig.Load();

			Host = config.Host;
			Port = config.Port;
        }

		private static string GetLocalIp()
		{
			return Dns.GetHostEntry(Dns.GetHostName())
				.AddressList.FirstOrDefault(a => a.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
				?.ToString() ?? "unknown";
        }

        // Check If connection established
		public void Connect(string host, int port)
		{
			Host = host;
			Port = port;

			try
			{
				client = new TcpClient();
				client.Connect(host, port);

				NetworkStream stream = client.GetStream();
				writer = new StreamWriter(stream) { AutoFlush = true };

				StreamReader reader = new StreamReader(stream);

				new Thread(() => ReceiveLoop(reader)) { IsBackground = true }.Start();
            }
			catch (Exception ex)
			{
				ConnectionFailed?.Invoke(ex.Message);
			}
		}

		public void SendMove(int fromRow, int fromCol, int toRow, int toCol)
		{
			if (writer == null) return;

            string payload = JsonSerializer.Serialize(new MoveRequestDto
            {
                FromRow = fromRow, FromCol = fromCol, ToRow = toRow, ToCol = toCol
            });
			writer.WriteLine(JsonSerializer.Serialize(new NetworkMessage { Type = "move", Payload = payload }));
        }

        // Handles incoming messages from the server and invokes the appropriate events based on the message type
        private void ReceiveLoop(StreamReader reader)
		{
			try
			{
				string line;
				while ((line = reader.ReadLine()) != null)
				{
					NetworkMessage msg = JsonSerializer.Deserialize<NetworkMessage>(line);
					switch (msg.Type)
					{
						case "hello": ConnectionEstablished?.Invoke(msg.Payload); break;
						case "state": StateUpdated?.Invoke(JsonSerializer.Deserialize<GameStateDto>(msg.Payload)); break;
						//case "reject": MoveRejected?.Invoke(msg.Payload); break;
					}
				}
			}
			catch (IOException) { /* connection closed */ }
		}
	}
}
