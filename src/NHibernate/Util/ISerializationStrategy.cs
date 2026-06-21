namespace NHibernate.Util
{
	/// <summary>
	/// Defines how NHibernate serializes and deserializes objects for serialization-based features.
	/// </summary>
	public interface ISerializationStrategy
	{
		/// <summary>
		/// Serializes the given value to a binary payload.
		/// </summary>
		/// <param name="value">The value to serialize.</param>
		/// <returns>The serialized payload.</returns>
		byte[] Serialize(object value);

		/// <summary>
		/// Deserializes a value from the given binary payload.
		/// </summary>
		/// <param name="data">The serialized payload.</param>
		/// <returns>The deserialized value.</returns>
		object Deserialize(byte[] data);
	}
}
