namespace NHibernate.Validator
{
	using System;
	using System.Text.RegularExpressions;

	public class PatternValidator : IValidator<PatternAttribute>
	{
		private Regex regex;

		public bool IsValid(object value)
		{
			if (value == null) return true;

			if (!(value is string)) return false;

			return regex.IsMatch((string) value);
		}

		public void Initialize(Attribute parameters)
		{
			PatternAttribute @param =(PatternAttribute)parameters;
			regex = new Regex(@param.Regex,@param.Flags);
		}
	}
}