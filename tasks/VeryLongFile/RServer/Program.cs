using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using Born2Code.Net;

namespace RServer
{
	public class Program
	{
		private static HttpListener listener;
		private const int DownloadSpeedBytesPerSecond = 100 * 1024 / 8; // 100Kb/sec

		private static void Main(string[] args)
		{
			var port = args.Length == 0 ? 9090 : int.Parse(args[0]);
			listener = new HttpListener();
			listener.Prefixes.Add(string.Format("http://+:{0}/secret/", port));
			listener.Start();

			InitHintsOffsets();

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
							var throttledStream = new ThrottledStream(ctx.Response.OutputStream, DownloadSpeedBytesPerSecond);
							var range = GetRequestedRange(ctx.Request);
							GenerateFileContent(throttledStream, range.Item1, range.Item2);
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

		private static Tuple<int?, int?> GetRequestedRange(HttpListenerRequest request)
		{
			//todo: extract range boundaries here
			return new Tuple<int?, int?>(null, null);
		}

		private static void GenerateFileContent(Stream stream, int? startOffset, int? endOffset)
		{
			if (startOffset == null && endOffset == null) return;
			if (startOffset == null)
				startOffset = TotalFileLength - endOffset;
			if (endOffset == null)
				endOffset = TotalFileLength;

			var stub = new byte[(int) (endOffset - startOffset)];
			var rand = new Random();
			rand.NextBytes(stub);

			if (AnyHintWithinRange(startOffset, endOffset))
				WriteHint(stream, startOffset, endOffset);
		}

		private static void WriteHint(Stream stream, int? startOffset, int? endOffset)
		{
			throw new NotImplementedException();
		}

		private static bool AnyHintWithinRange(int? startOffset, int? endOffset)
		{
			throw new NotImplementedException();
		}

		private static void InitHintsOffsets()
		{
			// Нужно растянуть подсказки так, чтобы за час, или даже два, их было нереально докачать. 
			// Соответственно, последняя подсказка должна быть дальше, чем:
			// DownloadSpeedBytesPerSecond * 3600 * 2 (bytes)
			// В соответствии с задумкой располагать данные через увеличивающиеся промежутки трэша, то есть: +-+--+----+--------+...
			// получаем, что нужно найти степень двойки >= чем наш последний оффсет:

			const int minimumLastOffset = DownloadSpeedBytesPerSecond*3600*2;

			var numberOfOffsets = (int) Math.Ceiling(Math.Log(minimumLastOffset) / Math.Log(2));
			var spacing = Encoding.UTF8.GetBytes(FirstHints[0]).Length;
			var rand = new Random();

			for (var i = 0; i < numberOfOffsets; i++)
			{
				var nextOffset = (int) (spacing*Math.Pow(2, i));
				HintOffsets.Add(nextOffset);
				var nextHint = i < 4 ? FirstHints[i] : HintsPool[rand.Next(HintsPool.Length)];
				
				Hints.Add(string.Format(nextHint, nextOffset));
				Console.WriteLine(Hints[i]);
			}

			TotalFileLength = HintOffsets[numberOfOffsets - 1] + Encoding.UTF8.GetBytes(Hints[numberOfOffsets - 1]).Length;
		}

		private static List<int> HintOffsets = new List<int>();
		private static List<string> Hints = new List<string>();

		private static string[] FirstHints = new []
		{
			"Would you like to play a game? If so - look at {0} offset and we'll start!",
			"I knew you would! Otherwise you were not here. Jump to {0}",
			"Ok, let's warm up! Jump to {0}",
			"Hope you're young enough for this game: you'll jump a lot! See {0}",
		};

		private static string[] HintsPool = new[]
		{
			"Now see {0}",
			"Then see {0}",
			"Please see{0}"
		};

		private static int TotalFileLength;
	}
}