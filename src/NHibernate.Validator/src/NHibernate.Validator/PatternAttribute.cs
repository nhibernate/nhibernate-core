namespace NHibernate.Validator
{
	using System;
	using System.Text.RegularExpressions;

	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property,AllowMultiple = true)]
	[ValidatorClass(typeof(PatternValidator))]
	public class PatternAttribute : Attribute
	{
		private RegexOptions flags;
		private string message = "{validator.pattern}";
		private string regex;

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