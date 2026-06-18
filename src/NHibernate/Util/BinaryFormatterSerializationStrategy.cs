#if !NET9_0_OR_GREATER
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

#if NET6_0_OR_GREATER
#pragma warning disable CS0618 // Serialization is obsolete
#endif

namespace NHibernate.Util
{
	/// <summary>
	/// Legacy serialization strategy based on <see cref="BinaryFormatter"/>.
	/// </summary>
	/// <remarks>
	/// This strategy is not available on .NET 9.0 or greater, where <see cref="BinaryFormatter"/>
	/// has been removed from the framework. Applications targeting those frameworks must provide
	/// their own <see cref="ISerializationStrategy"/> implementation.
	/// </remarks>
	public sealed class BinaryFormatterSerializationStrategy : ISerializationStrategy
	{
		/// <summary>
		/// Serializes the given value with <see cref="BinaryFormatter"/>.
		/// </summary>
		/// <param name="value">The value to serialize.</param>
		/// <returns>The serialized payload.</returns>
		public byte[] Serialize(object value)
		{
			var formatter = CreateFormatter();
			using (var stream = new MemoryStream())
			{
				formatter.Serialize(stream, value);
				return stream.ToArray();
			}
		}

		/// <summary>
		/// Deserializes a value with <see cref="BinaryFormatter"/>.
		/// </summary>
		/// <param name="data">The serialized payload.</param>
		/// <returns>The deserialized value.</returns>
		public object Deserialize(byte[] data)
		{
			var formatter = CreateFormatter();
			using (var stream = new MemoryStream(data))
			{
				return formatter.Deserialize(stream);
			}
		}

		private static BinaryFormatter CreateFormatter()
		{
			return new BinaryFormatter
			{
#if !NETFX
				SurrogateSelector = new SerializationHelper.SurrogateSelector()
#endif
			};
		}
	}
}
#endif
