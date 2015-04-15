using System;
using System.Net;
using System.Net.Sockets;

namespace TrickyServer
{
	class MyTcpListener
	{
		public static void Main(string[] args)
		{
			TcpListener server = null;
			try
			{
				var port = args.Length > 0 ? int.Parse(args[0]) : 13000;
				var localAddr = IPAddress.Parse(args.Length > 1 ? args[1] : "127.0.0.1");

				server = new TcpListener(localAddr, port);

				server.Start();

				
				var buffer = new Byte[BufferSize];
				String data = null;

				while (true)
				{
					Console.Write("Waiting for a connection... ");

					var client = server.AcceptTcpClient();
					Console.WriteLine("Connected!");

					data = null;

					NetworkStream stream = client.GetStream();

					int i;

					while ((i = stream.Read(buffer, 0, buffer.Length)) != 0)
					{
						data = System.Text.Encoding.ASCII.GetString(buffer, 0, i);
						Console.WriteLine("Received: {0}", data);

						var response = GetResponse(data);

						byte[] msg = System.Text.Encoding.ASCII.GetBytes(response);

						stream.Write(msg, 0, msg.Length);
						Console.WriteLine("Sent: {0}", response);
					}

					client.Close();
				}
			}
			catch (SocketException e)
			{
				Console.WriteLine("SocketException: {0}", e);
			}
			finally
			{
				server.Stop();
			}
		}

		private static string GetResponse(string data)
		{
			throw new NotImplementedException();
		}

		const int BufferSize = 256;
	}
}