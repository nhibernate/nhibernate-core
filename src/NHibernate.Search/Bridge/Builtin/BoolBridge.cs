using System;

namespace NHibernate.Search.Bridge.Builtin
{
	/// <summary>
	/// Bridge class for boolean.
	/// </summary>
	public class BoolBridge : SimpleBridge
	{
		public override object StringToObject(string stringValue)
		{
			try
			{
				return bool.Parse(stringValue);
			}
			catch (Exception ex)
			{
				return false;
			}
		}
	}
}
