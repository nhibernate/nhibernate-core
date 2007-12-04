using System;

namespace NHibernate.Search.Bridge.Builtin
{
	/// <summary>
	/// Bridge class for float.
	/// </summary>
	public class FloatBridge : SimpleBridge
	{
		public override object StringToObject(string stringValue)
		{
			try
			{
				return float.Parse(stringValue);
			}
			catch (Exception ex)
			{
				return 0.0F;
			}
		}
	}
}
