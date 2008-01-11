namespace NHibernate.Validator.Tests.Validators
{
	using System;

	public class FooDate
	{
		[Future]
		public DateTime Future;

		[Past]
		public DateTime Past;
	}
}