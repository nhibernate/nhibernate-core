namespace NHibernate.Validator
{
	using System;

	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	[ValidatorClass(typeof(EmailValidator))]
	public class EmailAttribute : Attribute 
	{
		private string message = "{validator.email}";

		public string Message
		{
			get { return message; }
			set { message = value; }
		}
	}
}