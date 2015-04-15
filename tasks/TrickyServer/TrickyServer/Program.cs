using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Protocol;

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

				while (true)
				{
					Console.Write("Waiting for a connection... ");

					var socket = server.AcceptSocket();
					Console.WriteLine("Connected!");

					var request = ReceiveRequest(socket);
					Console.WriteLine("Request from client: {0}", request);

					var response = HandleRequest(request);
					Console.WriteLine("Server's response: {0}", response);
					SendResponse(socket, response);

					socket.Close();
				}
			}
			catch (Exception e)
			{
				Console.WriteLine("Exception: {0}", e);
			}
			finally
			{
				if (server != null) server.Stop();
			}
		}

		private static TrickyRequest ReceiveRequest(Socket socket)
		{
			var bytes = new byte[BufferSize];
			var bytesOffset = socket.Receive(bytes, bytes.Length, SocketFlags.None);
			return bytesOffset < 0 ? null : TrickyRequest.FromBytes(bytes);
		}

		private static void SendResponse(Socket s, string response)
		{
			var bytes = new byte[BufferSize];
			var contentLengh = 0;

			//todo
			contentLengh = Encoding.UTF8.GetBytes(response, 0, response.Length, bytes, 0);

			var offset = 0;
			while (true)
			{
				var bytesWrite = s.Send(bytes, offset, contentLengh - offset, SocketFlags.None);
				offset += bytesWrite;
				if (offset == contentLengh) return;
			}
		}

		private static string HandleRequest(TrickyRequest request)
		{
			//todo
			return request.FileName;
		}

		public List<string> storedFilesHashes = new List<string>();

		const int BufferSize = 1 * 1024 * 1024;
	}
}