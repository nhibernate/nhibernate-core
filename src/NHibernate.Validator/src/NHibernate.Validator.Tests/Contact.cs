namespace NHibernate.Validator.Tests
{
	using System;

	public class Contact
	{
		[NotNull] private String name;

		[NotNull] private String phone;

		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		public string Phone
		{
			get { return phone; }
			set { phone = value; }
		}
	}
}