using System;

namespace Protocol
{
	public class TrickyRequest
	{
		public Headers Headers { get; private set; }
		public DateTime Date;
		public string FileName;
		public byte[] FileContent;
	}
}