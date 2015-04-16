using System;
using System.IO;
using System.Text;

namespace Protocol
{
	public class TrickyResponse
	{
		public TrickyResponse(Status status, string additionalMessage, Guid id, DateTime dateTime)
		{
			Status = status;
			Message = additionalMessage;
			Id = id;
			Date = dateTime;
		}

		public byte[] ToBytes()
		{
			var stream = new MemoryStream();
			var startPosition = stream.Position;
			Packer.PackLabel(stream);
			Packer.PackBinary(Encoding.ASCII.GetBytes(Status.ToString()), stream);
			Packer.PackInt64(Date.ToBinary(), stream);
			Packer.PackBinary(Encoding.ASCII.GetBytes(Message), stream);
			Packer.PackGuid(Id, stream);
			var crc = Packer.GetCheckSum(stream, startPosition, (int)(stream.Length - startPosition));
			Packer.PackInt32(crc, stream);
			return stream.ToArray();
		}

		public static TrickyResponse FromBytes(byte[] bytes)
		{
			var stream = new MemoryStream(bytes);
			Packer.UnPackLabel(stream);
			var status = Enum.Parse(typeof(Status), Encoding.ASCII.GetString(Packer.UnPackBinary(stream)), true);
			var date = DateTime.FromBinary(Packer.UnPackInt64(stream));
			var message = Encoding.ASCII.GetString(Packer.UnPackBinary(stream));
			var guid = Packer.UnPackGuid(stream);
			return new TrickyResponse((Status)status, message, guid, date);
		}

		public Status Status { get; private set; }
		public string Message { get; private set; }
		public Guid Id;
		public DateTime Date;
	}

	public enum Status
	{
		Success,
		Error,
		AlreadyExists,
		Unknown
	}
}