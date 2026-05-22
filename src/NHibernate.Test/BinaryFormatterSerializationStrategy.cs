using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using NHibernate.Util;

#if NET6_0_OR_GREATER
#pragma warning disable CS0618 // Serialization is obsolete
#endif

namespace NHibernate.Test
{
	/// <summary>
	/// Legacy serialization strategy based on BinaryFormatter.
	/// </summary>
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
