using System;
using System.Reflection;
using NHibernate.Util;

namespace NHibernate.Search
{
	public class ValueTypeBridge<T> : SimpleBridge
		where T: struct
	{
		public delegate T Parse(string str);

		public Parse parse = (Parse) Delegate.CreateDelegate(typeof (Parse), typeof (T)
			.GetMethod("Parse", new System.Type[]{typeof(string)}));

		public override Object StringToObject(String stringValue)
		{
			if (StringHelper.IsEmpty(stringValue)) return null;
			return parse(stringValue);
		}
	}
}