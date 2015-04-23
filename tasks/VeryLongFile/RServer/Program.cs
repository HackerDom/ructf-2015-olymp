using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Born2Code.Net;
using log4net;
using log4net.Config;

namespace RServer
{
	public class Program
	{
		private const int DownloadSpeedBytesPerSecond = 100 * 1024 / 8; // 100Kb/sec
		private static ILog log;

		private static void Main(string[] args)
		{
			XmlConfigurator.ConfigureAndWatch(new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.config.xml")));
			log = LogManager.GetLogger(typeof (Program));
			var port = args.Length == 0 ? 9090 : int.Parse(args[0]);

			InitHintsOffsets();

			try
			{
				var listener = new Listener(port, "/secret", OnContextAsync);
				listener.Start();

				log.InfoFormat("Server started listening on port {0}", port);
				var delayMs = 0;

				while (true)
				{
					//var timeToSleepString = Console.ReadLine();
					//int timeToSleep;
					//if (int.TryParse(timeToSleepString, out timeToSleep))
					//	delayMs = timeToSleep;
					//else
					//	Console.WriteLine("Couldn't parse \"{0}\" as valid int.", timeToSleepString);
					//log.InfoFormat("Delay is {0} ms", delayMs);
				}
			}
			catch (Exception e)
			{
				log.Fatal(e);
				throw;
			}
		}

		private static async Task OnContextAsync(HttpListenerContext context)
		{
			log.Debug("OnContextAsync hit");
			var requestId = Guid.NewGuid();
			var query = context.Request.QueryString["query"];
			var remoteEndPoint = context.Request.RemoteEndPoint;
			log.DebugFormat("{0}: received {1} from {2}", requestId, query, remoteEndPoint);
			context.Request.InputStream.Close();

			//var throttledStream = new ThrottledStream(context.Response.OutputStream, DownloadSpeedBytesPerSecond);
			//var range = GetRequestedRange(context.Request);
			//GenerateFileContent(throttledStream, range.Item1, range.Item2);
			var bytes = Encoding.UTF8.GetBytes("Gochya!");

			await context.Response.OutputStream.WriteAsync(bytes, 0, bytes.Length); //todo!!
			context.Response.OutputStream.Close();
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
			// Нужно растянуть подсказки так, чтобы за час, или даже четыре, их было нереально докачать. 
			// Соответственно, последняя подсказка должна быть дальше, чем:
			// DownloadSpeedBytesPerSecond * 3600 * 4 (bytes)
			// В соответствии с задумкой располагать данные через увеличивающиеся промежутки трэша, то есть: +-+--+----+--------+...
			// получаем, что нужно найти степень двойки >= чем наш последний оффсет:

			const int minimumLastOffset = DownloadSpeedBytesPerSecond*3600*4;

			var numberOfOffsets = (int) Math.Ceiling(Math.Log(minimumLastOffset) / Math.Log(2));
			var spacing = Encoding.UTF8.GetBytes(FirstHints[0]).Length;
			var rand = new Random();

			for (var i = 0; i < numberOfOffsets; i++)
			{
				var nextOffset = (int) (spacing*Math.Pow(2, i));
				HintOffsets.Add(nextOffset);
				var nextHint = i < 4 ? FirstHints[i] : HintsPool[rand.Next(HintsPool.Length)];
				
				Hints.Add(string.Format(nextHint, nextOffset));
				//Console.WriteLine(Hints[i]);
				log.Info(i + ": " +  Hints[i]);
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
			"Please see {0}"
		};

		private static int TotalFileLength;
	}
}