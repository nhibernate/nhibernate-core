using System;

namespace NHibernate.Search.Bridge.Builtin
{
	/// <summary>
	/// Bridge class for doubles.
	/// </summary>
	public class DoubleBridge : SimpleBridge
	{
		public override object StringToObject(string stringValue)
		{
			try
			{
				return short.Parse(stringValue);
			}
			catch (Exception ex)
			{
				return 0.0D;
			}
		}
	}
}
