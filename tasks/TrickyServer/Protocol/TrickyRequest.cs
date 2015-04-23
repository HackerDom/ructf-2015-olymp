using System;
using System.IO;
using System.Text;

namespace Protocol
{
	public class TrickyRequest
	{
		public TrickyRequest(Headers headers, DateTime dateTime, string fileName, byte[] content)
		{
			Headers = headers;
			Date = dateTime;
			FileName = fileName;
			FileContent = content;
		}

		public byte[] ToBytes()
		{
			var stream = new MemoryStream();
			var startPosition = stream.Position;
			Packer.PackLabel(stream);
			Packer.PackBinary(Encoding.ASCII.GetBytes(Headers.ToString()), stream);
			Packer.PackInt64(Date.ToBinary(), stream);
			Packer.PackBinary(Encoding.ASCII.GetBytes(FileName), stream);
			Packer.PackBinary(FileContent, stream);
			var crc = Packer.GetCheckSum(stream, startPosition, (int)(stream.Length - startPosition));
			Packer.PackInt32(crc, stream);
			return stream.ToArray();
		}

		public static TrickyRequest FromBytes(byte[] bytes)
		{
			var stream = new MemoryStream(bytes);
			Packer.UnPackLabel(stream);
			var headers = Encoding.ASCII.GetString(Packer.UnPackBinary(stream));
			var date = DateTime.FromBinary(Packer.UnPackInt64(stream));
			var filename = Encoding.ASCII.GetString(Packer.UnPackBinary(stream));
			var content = Packer.UnPackBinary(stream);
			return new TrickyRequest(new Headers(headers), date, filename, content);
		}

		public Headers Headers { get; private set; }
		public DateTime Date;
		public string FileName;
		public byte[] FileContent;
	}
}