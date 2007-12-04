using System;

namespace NHibernate.Search.Bridge.Builtin
{
	/// <summary>
	/// Bridge class for long.
	/// </summary>
	public class LongBridge : SimpleBridge
	{
		public override object StringToObject(string stringValue)
		{
			try
			{
				return short.Parse(stringValue);
			}
			catch (Exception ex)
			{
				return 0L;
			}
		}
	}
}
