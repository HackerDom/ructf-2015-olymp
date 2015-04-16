using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using Protocol;

namespace TrickyClient
{
	class Program
	{
		private static int port;
		private static string server;

		static void Main(string[] args)
		{
			try
			{
				port = args.Length > 0 ? int.Parse(args[0]) : 13000;
				server = args.Length > 1 ? args[1] : "127.0.0.1";
				
				MakeTestCaseCommunication();
			}
			catch (Exception e)
			{
				Console.WriteLine("Exception: {0}", e);
			}

			Console.Read();
		}

		private static void MakeTestCaseCommunication()
		{
			const string fileName1 = @"../TestData/pic.jpg";
			const string fileName2 = @"../TestData/pic2.jpg";
			var headers = new Headers(SerializationSchema.Custom);

			var correctRequest = new TrickyRequest(headers, DateTime.Now, fileName1, File.ReadAllBytes(fileName1));
			SendRequest(correctRequest);

			var secondCorrectRequest = new TrickyRequest(headers, DateTime.Now, fileName2, File.ReadAllBytes(fileName2));
			SendRequest(secondCorrectRequest);

			var incorrectRequest = new TrickyRequest(headers, DateTime.Now, fileName1, null);
			SendRequest(incorrectRequest);

			//todo: add some trash here

			SendRequest(secondCorrectRequest);

		}

		private static void SendRequest(TrickyRequest correctRequest)
		{
			var client = new TcpClient(server, port);
			var stream = client.GetStream();
			SendUploadRequest(stream, correctRequest);

			GetResponse(stream);

			stream.Close();
			client.Close();
		}

		private static TrickyResponse GetResponse(Stream stream)
		{
			var buffer = new Byte[BufferSize];

			var bytesRead = stream.Read(buffer, 0, buffer.Length);
			var response = TrickyResponse.FromBytes(buffer);
			Console.WriteLine("Received: {0}", response.Message);
			return response;
		}

		private static void SendUploadRequest(Stream stream, TrickyRequest request)
		{
			byte[] data = {};
			try
			{
				data = request.ToBytes();
				Console.WriteLine("File uploaded: {0}", request.FileName);
			}
			catch (Exception)
			{
				data = Encoding.ASCII.GetBytes("Trash");
			}
			finally
			{
				stream.Write(data, 0, data.Length);
			}
		}

		private const int BufferSize = 256;
	}
}
