using System;

namespace NHibernate.Search.Attributes
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
	public class DateBridgeAttribute : Attribute
	{
		private readonly Resolution resolution;

		public DateBridgeAttribute(Resolution resolution)
		{
			this.resolution = resolution;
		}

		public Resolution Resolution
		{
			get { return resolution; }
		}
	}
}