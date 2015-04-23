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

		private static async Task GenerateFileContent(Stream stream, int? startOffset, int? endOffset)
		{
			log.Debug("GenerateFileContent hit");
			if (startOffset == null && endOffset == null || startOffset == 0 && endOffset == 0)
				await WriteWholeFile(stream);
			if (startOffset == null)
				startOffset = TotalFileLength - endOffset;
			if (endOffset == null)
				endOffset = TotalFileLength;

			var bytesWritten = 0;

			var buffer = new byte[bufferSize];
			var rand = new Random();
			int hintIndex;

			log.Debug(string.Format("Requested range: {0} - {1}", startOffset, endOffset));

			while (bytesWritten < endOffset - startOffset)
			{
				rand.NextBytes(buffer);

				if (AnyHintWithinRange(startOffset + bytesWritten, startOffset + bytesWritten + bufferSize, out hintIndex))
				{
					var writeHintFrom = HintOffsets[hintIndex] % bufferSize;
					var writeHintUpTo = Math.Min(Encoding.UTF8.GetBytes(Hints[hintIndex]).Length,
						bufferSize - writeHintFrom - 1);
					var source = Encoding.UTF8.GetBytes(Hints[hintIndex]);
					Buffer.BlockCopy(source, 0, buffer, writeHintFrom, writeHintUpTo);
					log.Debug(String.Format("Writing hint # {0} from {1} offset. Buffer content: {2}", hintIndex, HintOffsets[hintIndex],
					Encoding.UTF8.GetString(buffer)));
				}

				await stream.WriteAsync(buffer, 0, buffer.Length);

				bytesWritten += bufferSize;
			}

			stream.Close();
		}

		private static async Task WriteWholeFile(Stream stream)
		{
			log.Debug("WriteWholeFile hit");
			var bytesWritten = 0;

			var buffer = new byte[bufferSize];
			var rand = new Random();
			int hintIndex;

			log.Debug(string.Format("TotalFileLength: {0}", TotalFileLength));

			while (bytesWritten < TotalFileLength)
			{
				rand.NextBytes(buffer);

				if (AnyHintWithinRange(bytesWritten, bytesWritten + bufferSize, out hintIndex))
				{
					var writeHintFrom = HintOffsets[hintIndex] % bufferSize;
					var writeHintUpTo = Math.Min(Encoding.UTF8.GetBytes(Hints[hintIndex]).Length,
						bufferSize - writeHintFrom - 1);
					var source = Encoding.UTF8.GetBytes(Hints[hintIndex]);
					Buffer.BlockCopy(source, 0, buffer, writeHintFrom, writeHintUpTo);
					log.Debug(String.Format("Writing hint # {0} from {1} offset. Buffer content: {2}", hintIndex, HintOffsets[hintIndex],
					Encoding.UTF8.GetString(buffer)));
				}

				await stream.WriteAsync(buffer, 0, buffer.Length);

				bytesWritten += bufferSize;
			}
			
			stream.Close();
		}

		private static void WriteHint(Stream stream, int? startOffset, int? endOffset)
		{
			throw new NotImplementedException();
		}

		private static bool AnyHintWithinRange(int? startOffset, int? endOffset, out int hintIndex)
		{
			hintIndex = -1;
			for (var i = 0; i < HintOffsets.Count; i++)
			{
				if (startOffset <= HintOffsets[i] && HintOffsets[i] <= endOffset)
				{
					hintIndex = i;
					return true;
				}
			}
			return false;
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
				var nextOffset = (int) (spacing*Math.Pow(2, i));
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