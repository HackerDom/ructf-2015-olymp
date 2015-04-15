using System;
using System.Net.Sockets;
using System.Text;

namespace TrickyClient
{
	class Program
	{
		static void Main(string[] args)
		{
			try
			{
				var port = args.Length > 0 ? int.Parse(args[0]) : 13000;
				var server = args.Length > 1 ? args[1] : "127.0.0.1";
				var client = new TcpClient(server, port);

				MakeTestCaseCommunication(client);

				client.Close();
			}
			catch (Exception e)
			{
				Console.WriteLine("Exception: {0}", e);
			}

			Console.Read();
		}

		private static void MakeTestCaseCommunication(TcpClient client)
		{
			var message = "Test message";

			Byte[] data = Encoding.ASCII.GetBytes(message);

			NetworkStream stream = client.GetStream();


			stream.Write(data, 0, data.Length);

			Console.WriteLine("Sent: {0}", message);


			var buffer = new Byte[BufferSize];

			var responseData = String.Empty;

			// Read the first batch of the TcpServer response bytes.
			Int32 bytes = stream.Read(buffer, 0, buffer.Length);
			responseData = Encoding.ASCII.GetString(data, 0, bytes);
			Console.WriteLine("Received: {0}", responseData);

			stream.Close();
		}

		private const int BufferSize = 256;
	}
}
