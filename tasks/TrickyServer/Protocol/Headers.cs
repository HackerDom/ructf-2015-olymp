namespace Protocol
{
	public class Headers
	{
		public SerializationSchema Schema { get; private set; }
		
	}

	public enum SerializationSchema
	{
		// with numbers it is much easier to guess another scheme. Remove?
		Custom = 0,
		Json = 1
	}
}