namespace NHibernate.Validator.Tests.Collections
{
	using System;

	public class Show
	{
		[NotNull] public String name;

		public Show()
		{
		}

		public Show(string name)
		{
			this.name = name;
		}
	}
}