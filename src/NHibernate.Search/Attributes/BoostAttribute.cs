using System;

namespace NHibernate.Search.Attributes
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
	public class BoostAttribute : Attribute
	{
		private float value;

		public BoostAttribute(float value)
		{
			this.value = value;
		}


		public float Value
		{
			get { return value; }
		}
	}
}