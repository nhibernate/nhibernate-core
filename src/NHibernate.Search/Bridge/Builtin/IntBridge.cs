using System;

namespace NHibernate.Search.Bridge.Builtin
{
	/// <summary>
	/// Bridge class for integers.
	/// </summary>
	public class IntBridge : SimpleBridge
	{
		public override object StringToObject(string stringValue)
		{
			try
			{
				return int.Parse(stringValue);
			}
			catch (Exception ex)
			{
				return 0;
			}
		}
	}
}
