namespace NHibernate.Validator
{
	using System;
	using System.Text.RegularExpressions;

	/// <summary>
	/// The annotated element must follow the regex pattern
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
	[ValidatorClass(typeof(PatternValidator))]
	public class PatternAttribute : Attribute
	{
		private RegexOptions flags;
		private string message = "{validator.pattern}";
		private string regex;

		public PatternAttribute(string regex, RegexOptions flags)
		{
			this.regex = regex;
			this.flags = flags;
		}

		public PatternAttribute(string regex, RegexOptions flags, string message)
		{
			this.regex = regex;
			this.message = message;
			this.flags = flags;
		}

		public PatternAttribute(string regex)
		{
			this.regex = regex;
		}

		public PatternAttribute()
		{
		}

		public string Regex
		{
			get { return regex; }
			set { regex = value; }
		}

		public RegexOptions Flags
		{
			get { return flags; }
			set { flags = value; }
		}

		public string Message
		{
			get { return message; }
			set { message = value; }
		}
	}
}