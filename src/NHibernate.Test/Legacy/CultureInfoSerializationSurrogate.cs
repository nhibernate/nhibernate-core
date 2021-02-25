#if !NETFX
using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace NHibernate.Test.Legacy
{
	class CultureInfoSerializationSurrogate : ISerializationSurrogate
	{
		public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
		{
			var culture = (CultureInfo) obj;
			info.AddValue("LCID", culture.LCID);
			info.AddValue("UseUserOverride", culture.UseUserOverride);
		}

		public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
		{
			return new CultureInfo(
				info.GetInt32("LCID"),
				info.GetBoolean("UseUserOverride"));
		}
	}
}
#endif