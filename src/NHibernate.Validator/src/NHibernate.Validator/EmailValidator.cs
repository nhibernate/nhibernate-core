namespace NHibernate.Validator
{
	using System;
	using System.Text.RegularExpressions;

	public class EmailValidator : IValidator<EmailAttribute>
	{
		private static String ATOM = "[^\\x00-\\x1F^\\(^\\)^\\<^\\>^\\@^\\,^\\(;^\\:^\\\\^\\\"^\\.^\\[^\\]^\\s]";
		private static String DOMAIN = "(" + ATOM + "+(\\." + ATOM + "+)*";
		private static String IP_DOMAIN = "\\[[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\]";
		private Regex regex;

		public bool IsValid(object value) 
		{
			if (value == null) return true;
			if (!(value is string)) return false;
			string @string = (String) value;
			if ( @string.Length == 0 ) return true; //TODO: See if it's well done.
			return regex.IsMatch(@string);
		}

		public void Initialize(Attribute parameters)
		{
			regex = new Regex(string.Concat("^",ATOM, "+(\\." , ATOM , "+)*@" , DOMAIN , "|" , IP_DOMAIN , ")$"), RegexOptions.Compiled);
		}
	}
}