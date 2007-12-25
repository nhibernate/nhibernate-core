namespace NHibernate.Validator
{
	using System;

	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public class PatternsAttribute : Attribute
	{
		public PatternsAttribute(PatternAttribute[] value)
		{
			this.value = value;
		}

		private PatternAttribute[] value;

		public PatternAttribute[] Value
		{
			get { return value; }
			set { this.value = value; }
		}
	}
}