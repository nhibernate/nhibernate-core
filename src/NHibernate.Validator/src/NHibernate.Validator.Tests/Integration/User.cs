namespace NHibernate.Validator.Tests.Integration
{
	using System.Collections.Generic;

	public class User
	{
		private IList<Email> emails;

		[Valid]
		public IList<Email> Emails
		{
			get { return emails; }
			set { emails = value; }
		}
	}

	public class Email
	{
		[Email]
		public string Address;
	}
}