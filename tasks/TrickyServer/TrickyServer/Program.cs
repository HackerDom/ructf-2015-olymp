using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
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
			try
			{
				var bytes = new byte[BufferSize];
				var bytesOffset = socket.Receive(bytes, bytes.Length, SocketFlags.None);
				return bytesOffset < 0 ? null : TrickyRequest.FromBytes(bytes);
			}
			catch (Exception)
			{
				return null;
			}
		}

		private static void SendResponse(Socket s, TrickyResponse response)
		{
			var content = response.ToBytes();
			var contentLengh = content.Length;

			var offset = 0;
			while (true)
			{
				var bytesWrite = s.Send(content, offset, contentLengh - offset, SocketFlags.None);
				offset += bytesWrite;
				if (offset == contentLengh) return;
			}
		}

		private static TrickyResponse HandleRequest(TrickyRequest request)
		{
			try
			{
			var hash = CalculateHash(request.FileContent);
			if (storedFilesHashes.Contains(hash))
				return new TrickyResponse(Status.AlreadyExists, string.Format("File with hash {0} already exists in our DB", hash),
					Guid.Empty, DateTime.Now);

			storedFilesHashes.Add(hash);
			return new TrickyResponse(Status.Success, string.Format("File {0} successfully saved in our DB", request.FileName),
				Guid.NewGuid(), DateTime.Now);

			}
			catch (Exception e)
			{
				return new TrickyResponse(Status.Error, e.Message, Guid.Empty, DateTime.Now);
			}
		}

		private static string CalculateHash(byte[] fileContent)
		{
			var sha56 = SHA256Managed.Create();
			var hash = sha56.ComputeHash(fileContent);
			return BitConverter.ToString(hash);
		}

		public static List<string> storedFilesHashes = new List<string>();

		const int BufferSize = 1 * 1024 * 1024;
	}
}