using System;
using System.IO;
using System.Text;

namespace Protocol
{
	internal class EqualizedException : Exception
	{
		public EqualizedException()
			: base("Data doesn't equalize.")
		{
		}
	}

	public static class Packer
	{
		public static int GetPackedGuidSize()
		{
			return 16;
		}

		public static void PackGuid(Guid guid, Stream stream)
		{
			stream.Write(guid.ToByteArray(), 0, 16);
		}

		public static Guid UnPackGuid(Stream stream)
		{
			byte[] bytes = new byte[16];
			stream.Read(bytes, 0, 16);
			return new Guid(bytes);
		}

		public static void PackString(string value, byte fullLen, Stream stream)
		{
			PackString(value, fullLen, stream, Encoding.GetEncoding(1251));
		}

		public static void PackString(string value, byte fullLen, Stream stream, Encoding encoding)
		{
			string str = value ?? "";
			if ((fullLen + 1) % 4 != 0)
				throw new EqualizedException();
			byte realLen = (byte)Math.Min(str.Length, fullLen);
			stream.WriteByte(realLen);
			byte[] bytes = new byte[fullLen];
			encoding.GetBytes(str, 0, realLen, bytes, 0);
			stream.Write(bytes, 0, bytes.Length);
		}

		public static string UnPackString(Stream stream, byte fullLen)
		{
			return UnPackString(stream, fullLen, Encoding.GetEncoding(1251));
		}

		public static string UnPackString(Stream stream, byte fullLen, Encoding encoding)
		{
			if ((fullLen + 1) % 4 != 0)
				throw new EqualizedException();
			int length = stream.ReadByte();
			if (length < 0)
				return "";
			if (length > fullLen)
				length = fullLen;
			if (length == 0)
			{
				stream.Seek(fullLen, SeekOrigin.Current);
				return "";
			}
			byte[] bytes = new byte[length];
			stream.Read(bytes, 0, bytes.Length);
			stream.Seek(fullLen - length, SeekOrigin.Current);
			return encoding.GetString(bytes, 0, length);
		}

		public static int GetPackedStringSize(byte fullLen)
		{
			return fullLen + 1;
		}

		public static void PackBool(bool b, Stream stream)
		{
			PackInt32(b ? 1 : 0, stream);
		}

		public static bool UnPackBool(Stream stream)
		{
			return UnPackInt32(stream) == 1;
		}

		public static int GetPackedBoolSize()
		{
			return GetPackedInt32Size();
		}

		public static int GetPackedInt32Size()
		{
			return 4;
		}

		public static void PackInt32(Int32 i, Stream stream)
		{
			byte[] bytes = BitConverter.GetBytes(i);
			stream.Write(bytes, 0, bytes.Length);
		}

		public static int UnPackInt32(Stream stream)
		{
			byte[] bytes = new byte[4];
			stream.Read(bytes, 0, bytes.Length);
			return BitConverter.ToInt32(bytes, 0);
		}

		public static void PackUInt32(UInt32 i, Stream stream)
		{
			byte[] bytes = BitConverter.GetBytes(i);
			stream.Write(bytes, 0, bytes.Length);
		}

		public static UInt32 UnPackUInt32(Stream stream)
		{
			byte[] bytes = new byte[4];
			stream.Read(bytes, 0, bytes.Length);
			return BitConverter.ToUInt32(bytes, 0);
		}

		public static int GetPackedInt64Size()
		{
			return 8;
		}

		public static void PackInt64(Int64 l, Stream stream)
		{
			byte[] bytes = BitConverter.GetBytes(l);
			stream.Write(bytes, 0, bytes.Length);
		}

		public static long UnPackInt64(Stream stream)
		{
			byte[] bytes = new byte[8];
			stream.Read(bytes, 0, bytes.Length);
			return BitConverter.ToInt64(bytes, 0);
		}

		public static void PackUInt64(UInt64 u, Stream stream)
		{
			byte[] bytes = BitConverter.GetBytes(u);
			stream.Write(bytes, 0, bytes.Length);
		}

		public static ulong UnPackUInt64(Stream stream)
		{
			byte[] bytes = new byte[8];
			stream.Read(bytes, 0, bytes.Length);
			return BitConverter.ToUInt64(bytes, 0);
		}

		public static int GetPackedBinarySize(int bytesCount)
		{
			return 4 + GetAlignedSize(bytesCount);
		}

		public static void PackBinary(byte[] bytes, Stream stream)
		{
			if (bytes == null)
				throw new ArgumentNullException();
			PackInt32(bytes.Length, stream);
			stream.Write(bytes, 0, bytes.Length);
			int reminder = GetAlignedSize(bytes.Length) - bytes.Length;
			if (reminder > 0)
				stream.Write(new byte[reminder], 0, reminder);
		}

		public static byte[] UnPackBinary(Stream stream)
		{
			int length = UnPackInt32(stream);
			byte[] bytes = new byte[length];
			stream.Read(bytes, 0, length);
			stream.Seek(GetAlignedSize(length) - length, SeekOrigin.Current);
			return bytes;
		}

		public static byte[] UnPackBinary(Stream stream, int maxSize)
		{
			int length = UnPackInt32(stream);
			if (length > maxSize)
				throw new Exception(string.Format("Binary size {0} can't be more than maxSize {1}", length, maxSize));
			byte[] bytes = new byte[length];
			int read = stream.Read(bytes, 0, length);
			if (read != length) throw new Exception(string.Format("Readed bytes: {0}. Need read bytes: {1}", read, length));
			stream.Seek(GetAlignedSize(length) - length, SeekOrigin.Current);
			return bytes;
		}

		public static int GetPackedLabelSize()
		{
			return Delimiter.Length;
		}

		public static void PackLabel(Stream stream)
		{
			stream.Write(Delimiter, 0, Delimiter.Length);
		}

		public static void UnPackLabel(Stream stream)
		{
			byte[] bytes = new byte[Delimiter.Length];
			stream.Read(bytes, 0, bytes.Length);
			if (bytes[0] != Delimiter[0])
				throw new FormatException("Label must be equal |-|. Current value = " + Encoding.ASCII.GetString(bytes, 0, 4));
		}

		public static void PackLabel(byte[] label, Stream stream)
		{
			stream.Write(label, 0, label.Length);
		}

		public static void UnPackLabel(byte[] label, Stream stream)
		{
			byte[] bytes = new byte[label.Length];
			stream.Read(bytes, 0, bytes.Length);
			for (int i = 0; i < bytes.Length; i++)
				if (bytes[i] != label[i])
					throw new FormatException(string.Format("Label must be equal {0}. Current value = {1}", Encoding.ASCII.GetString(label, 0, label.Length), Encoding.ASCII.GetString(bytes, 0, bytes.Length)));
		}

		public static Int32 GetCheckSum(Stream stream, long startPosition, int count)
		{
			long bufPosition = stream.Position;
			stream.Position = startPosition;
			int checkSum = 0;
			for (int i = 0; i < count; i += sizeof(Int32))
				checkSum ^= UnPackInt32(stream);
			stream.Position = bufPosition;
			return checkSum;
		}

		public static void VerifyCheckSum(Stream stream, long startPosition, int count, int checkSum)
		{
			int calcCheckSum = GetCheckSum(stream, startPosition, count);
			if (checkSum != calcCheckSum)
			{
				throw new Exception(string.Format("Bad check sum. Check sum from stream = {0:X8}. Calculated check sum = {1:X8}, offset={2}",
								  checkSum, calcCheckSum, startPosition));
			}
		}

		public static int GetAlignedSize(int realSize)
		{
			return realSize + (4 - realSize % 4) % 4;
		}

		public static readonly byte[] Delimiter = Encoding.ASCII.GetBytes("|--|");
		public const int CheckSumTypeSize = sizeof(Int32);
	}
}