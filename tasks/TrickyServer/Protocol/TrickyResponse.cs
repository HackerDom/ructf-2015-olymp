using System;

namespace Protocol
{
	public class TrickyResponse
	{
		public Status Status { get; private set; }
		public string Message { get; private set; }
		public Guid Id;
		public DateTime Date;
	}

	public enum Status
	{
		Success,
		AlreadyExists
	}
}