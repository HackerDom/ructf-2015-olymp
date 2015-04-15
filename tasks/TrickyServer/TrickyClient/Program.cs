using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using Protocol;

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
			var stream = client.GetStream();

			const string fileName = @"../TestData/pic.jpg";
			var headers = new Headers(SerializationSchema.Custom);

			var correctRequest = new TrickyRequest(headers, DateTime.Now, fileName, File.ReadAllBytes(fileName));
			SendUploadRequest(stream, correctRequest);
			

			
			
			var buffer = new Byte[BufferSize];

			var bytesRead = stream.Read(buffer, 0, buffer.Length);
			var responseData = Encoding.ASCII.GetString(buffer, 0, bytesRead);
			Console.WriteLine("Received: {0}", responseData);

			stream.Close();
		}

		private static void SendUploadRequest(NetworkStream stream, TrickyRequest request)
		{
			stream.Write(request.ToBytes(), 0, request.ToBytes().Length);

			Console.WriteLine("File uploaded: {0}", request.FileName);
		}

		private const int BufferSize = 256;
	}
}
