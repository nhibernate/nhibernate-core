namespace NHibernate.Validator
{
	using System;

	/// <summary>
	/// The annotated elemnt has to be in the appropriate range. Apply on numeric values or string
	/// representation of the numeric value.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	[ValidatorClass(typeof(RangeValidator))]
	public class RangeAttribute : Attribute
	{
		private int max;
		private string message = "{validator.range}";
		private int min;

		public int Min
		{
			get { return min; }
			set { min = value; }
		}

		public int Max
		{
			get { return max; }
			set { max = value; }
		}

		public string Message
		{
			get { return message; }
			set { message = value; }
		}
	}
}