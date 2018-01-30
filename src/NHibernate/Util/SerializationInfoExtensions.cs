using System;
using System.Runtime.Serialization;

namespace NHibernate.Util
{
	internal static class SerializationInfoExtensions
	{
		public static T GetValue<T>(this SerializationInfo info, string name)
		{
			if (info == null) throw new ArgumentNullException(nameof(info));
			return (T) info.GetValue(name, typeof(T));
		}
	}
}
