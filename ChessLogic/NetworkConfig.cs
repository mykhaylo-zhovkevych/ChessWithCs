using System;
using System.Text.Json;

namespace ChessLogic
{
    /// <summary>
    /// Represents the configuration settings for a network connection.
    /// </summary>
    public class NetworkConfig
    {
        public string Mode { get; set; } = "LoopbackTest";
        public string Host { get; set; } = "127.0.0.1";
        public int Port { get; set; } = 5000;

        public static NetworkConfig Load(string path = "network.config.json")
        {
            if (!File.Exists(path))
            {
                return new NetworkConfig();
            }
            return JsonSerializer.Deserialize<NetworkConfig>(File.ReadAllText(path)) ?? new NetworkConfig();
        }        
    }
}
