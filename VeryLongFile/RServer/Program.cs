using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;

namespace RServer
{
	internal class Program
	{
		private static HttpListener listener;

		private static void Main(string[] args)
		{
			var port = args.Length == 0 ? 9090 : int.Parse(args[0]);
			listener = new HttpListener();
			listener.Prefixes.Add(string.Format("http://+:{0}/secret/", port));
			listener.Start();

			var timers = new List<Timer>();

			while (true)
			{
				try
				{
					var context = listener.GetContext();
					Console.WriteLine("Accepted client {0}", context.Request.RemoteEndPoint);
					timers.Add(new Timer(state =>
					{
						var ctx = (HttpListenerContext) state;
						try
						{
							Console.WriteLine("Processing client {0}	{1}", context.Request.RemoteEndPoint, ctx.Request.RawUrl);
							var response = ProcessRequest(ctx.Request);
							var bytes = response.ToBytes();
							ctx.Response.OutputStream.Write(bytes, 0, bytes.Length);
							Console.WriteLine("Written to client {0}	{1}", context.Request.RemoteEndPoint, ctx.Request.RawUrl);
						}
						finally
						{
							ctx.Response.Close();
							Console.WriteLine("Processed client {0}	{1}", context.Request.RemoteEndPoint, ctx.Request.RawUrl);
						}
					}, context, 0, Timeout.Infinite));
				}
				catch (Exception e)
				{
					Console.Error.WriteLine(e);
				}
			}
		}

		private static Response ProcessRequest(HttpListenerRequest request)
		{
			var rand = new Random();
			Thread.Sleep(rand.Next(5000));
			return new Response();
		}
	}

	public class Response
	{
		public byte[] ToBytes()
		{
			return new byte[] {};
		}
	}
}
