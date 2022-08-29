using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace NHibernate.Util
{
	public static partial class SerializationHelper
	{
		public static byte[] Serialize(object obj)
		{
			var formatter = CreateFormatter();
			using (var stream = new MemoryStream())
			{
				formatter.Serialize(stream, obj);
				return stream.ToArray();
			}
		}

		public static object Deserialize(byte[] data)
		{
			var formatter = CreateFormatter();
			using (var stream = new MemoryStream(data))
			{
				return formatter.Deserialize(stream);
			}
		}

		internal static void AddValueArray<T>(this SerializationInfo info, string name, T[] values)
		{
			info.AddValue($"{name}.Length", values?.Length);
			if (values == null)
				return;

			for (var i = 0; i < values.Length; i++)
				info.AddValue($"{name}[{i}]", values[i]);
		}

		internal static T[] GetValueArray<T>(this SerializationInfo info, string name)
		{
			var length = info.GetValue<int?>($"{name}.Length");
			if (length == null)
				return null;

			var result = new T[length.Value];
			for (var i = 0; i < result.Length; i++)
				result[i] = info.GetValue<T>($"{name}[{i}]");
			return result;
		}

		internal static T GetValue<T>(this SerializationInfo info, string name)
		{
			return (T) info.GetValue(name, typeof(T));
		}

		private static BinaryFormatter CreateFormatter()
		{
			return new BinaryFormatter
			{
#if !NETFX
				SurrogateSelector = new SurrogateSelector()
#endif
			};
		}
	}
}
