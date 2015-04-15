namespace Protocol
{
	public class Headers
	{
		public Headers(SerializationSchema schema)
		{
			Schema = schema;
		}

		public Headers(string schema)
		{
			if (schema.ToLower() == SerializationSchema.Custom.ToString().ToLower())
				Schema = SerializationSchema.Custom;
			else if (schema.ToLower() == SerializationSchema.Json.ToString().ToLower())
				Schema = SerializationSchema.Json;
			else
				Schema = SerializationSchema.Unknown;
		}

		public SerializationSchema Schema { get; private set; }
		
	}

	public enum SerializationSchema
	{
		// with numbers it is much easier to guess another scheme. Remove?
		Custom = 0,
		Json = 1,
		Unknown = 2
	}
}