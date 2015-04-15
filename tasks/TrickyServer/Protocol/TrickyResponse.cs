using System;

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