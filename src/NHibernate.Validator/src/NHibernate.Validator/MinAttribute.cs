namespace NHibernate.Validator
{
	using System;

	/// <summary>
	/// Min restriction on a numeric annotated elemnt (or the string representation of a numeric)
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	[ValidatorClass(typeof(MinValidator))]
	public class MinAttribute : Attribute
	{
		private string message = "{validator.min}";
		private long value;

		public MinAttribute(long min)
		{
			value = min;
		}

		public MinAttribute(long min, string message)
		{
			this.value = min;
			this.message = message;
		}

		public MinAttribute()
		{
		}

		public long Value
		{
			get { return value; }
			set { this.value = value; }
		}

		public string Message
		{
			get { return message; }
			set { message = value; }
		}
	}
}