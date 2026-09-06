using System;
using System.Net;

namespace ChessLogic
{
	public class ChessClient
	{
		
		public string UserName { get; }
		public string LocalIp { get; }


		public ChessClient()
		{
			UserName = Environment.UserName;
			LocalIp = GetLocalIp();

		}


		private static string GetLocalIp()
		{
			return Dns.GetHostEntry(Dns.GetHostName())
				.AddressList.FirstOrDefault(a => a.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
				?.ToString() ?? "unknown";
        }




		
	}
}
