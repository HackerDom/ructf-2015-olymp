using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
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

				while (true) {}
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
			var range = GetRequestedRange(context.Request);
			await GenerateFileContent(context.Response.OutputStream, range.Item1, range.Item2);			
		}

		private static Tuple<int?, int?> GetRequestedRange(HttpListenerRequest request)
		{
			try
			{
				log.Debug(string.Join(";", request.Headers.AllKeys));
				var rangeHeader = request.Headers.AllKeys.Contains(HttpRequestHeader.Range.ToString())
					? request.Headers[HttpRequestHeader.Range.ToString()]
					: "";

				var res = RangeHeaderValue.Parse(rangeHeader);
				log.Debug(string.Format("Range: {0}", res.Ranges.First().From));

				//todo: поддерживаем только один диапазон?
				return new Tuple<int?, int?>((int?) res.Ranges.First().From, (int?) res.Ranges.First().To);
			}
			catch (Exception e)
			{
				log.Error(e);
				return new Tuple<int?, int?>(null,null);
			}
		}

		private static async Task GenerateFileContent(Stream stream, int? from, int? to)
		{
			int startOffset;
			int endOffset;
			if (from == null)
				if (to == null)
					startOffset = 0;
				else
					startOffset = TotalFileLength - to ?? 0;
			else
				startOffset = from ?? 0;

			if (from == null)
				endOffset = TotalFileLength;
			else
				endOffset = to ?? TotalFileLength;

			log.Debug(string.Format("Requested range: {0} - {1}", startOffset, endOffset));

			byte onebyte;

			for (var i = startOffset; i <= endOffset; i++)
			{
				onebyte = GetByteByIndex(i);
				log.Debug(string.Format("i# {0} onebyte: {1} is {2}", i, onebyte, Encoding.UTF8.GetString(new []{onebyte})));
				await stream.WriteAsync( new[]{onebyte}, 0, 1);
			}

			stream.Close();
		}

		private static byte GetByteByIndex(int i)
		{
			int hintIndex;
			if (AnyHintContentOnByte(i, out hintIndex))
			{
				var bytes = Encoding.UTF8.GetBytes(Hints[hintIndex]);
				return bytes[i - HintOffsets[hintIndex]];
			}
			return HashOf(i);
		}

		private static bool AnyHintContentOnByte(int byteNumber, out int hintIndex)
		{
			hintIndex = -1;
			for (var i = 0; i < Hints.Count; i++)
			{
				if (HintOffsets[i] <= byteNumber && byteNumber < HintOffsets[i] + Encoding.UTF8.GetBytes(Hints[i]).Length)
				{
					hintIndex = i;
					return true;
				}
			}
			return false;
		}

		private static byte HashOf(int i)
		{
			var bytes = BitConverter.GetBytes(i);
			byte oneByte = 0; 
			foreach (var b in bytes)
			{
				oneByte ^= b;
			}
			return oneByte;
		}

		private static void InitHintsOffsets()
		{
			// Нужно растянуть подсказки так, чтобы за час, или даже четыре, их было нереально докачать. 
			// Соответственно, последняя подсказка должна быть дальше, чем:
			// DownloadSpeedBytesPerSecond * 3600 * 3 (bytes)
			// В соответствии с задумкой располагать данные через увеличивающиеся промежутки трэша, то есть: +-+--+----+--------+...
			// получаем, что нужно найти степень двойки >= чем наш последний оффсет:

			//const int minimumLastOffset = DownloadSpeedBytesPerSecond*3600; //todo: make it long!
			//log.Debug(string.Format("minimumLastOffset: {0}", minimumLastOffset));

			var numberOfOffsets = 24;// todo: (int) Math.Ceiling(Math.Log(minimumLastOffset) / Math.Log(2));
			var spacing = Encoding.UTF8.GetBytes(FirstHints[0]).Length;
			var rand = new Random();

			HintOffsets.Add(0);

			for (var i = 0; i < numberOfOffsets - 1; i++)
			{
				var nextOffset = (int) (spacing* (Math.Pow(2, i) + 1));
				HintOffsets.Add(nextOffset);
				var nextHint = i < 4 ? FirstHints[i] : HintsPool[rand.Next(HintsPool.Length)];
				
				Hints.Add(string.Format(nextHint, nextOffset));
				//Console.WriteLine(Hints[i]);
				log.Info(i + ": " +  Hints[i]);
			}

			Hints.Add(Flag);

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

		private static string Flag = "Congrats! Here is your flag: THISWASEASYIFYOUKNOWABOUTHTTPRANGEHEADER";

		private static string[] HintsPool = new[]
		{
			"Now see {0}",
			"Then see {0}",
			"Please see {0}"
		};

		private static int TotalFileLength;
		private static int bufferSize = 1024;
	}
}