using System;

namespace NHibernate.Search
{
	public class EnumBridge : SimpleBridge
	{
		private System.Type type;

		public EnumBridge(System.Type type)
		{
			this.type = type;
		}

		public override object StringToObject(string stringValue)
		{
			return Enum.Parse(type, stringValue);
		}
	}
}