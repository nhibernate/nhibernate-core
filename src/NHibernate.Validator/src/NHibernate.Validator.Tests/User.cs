namespace NHibernate.Validator.Tests
{
	using System;

	public class User
	{
		[Email] public String email;

		[NotNull] public String name;
	}
}