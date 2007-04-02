using System;
using System.Reflection;
using NHibernate.Util;

namespace NHibernate.Search
{
	public class StringBridge : SimpleBridge
	{
		public override object StringToObject(string stringValue)
		{
			return stringValue;
		}
	}
}