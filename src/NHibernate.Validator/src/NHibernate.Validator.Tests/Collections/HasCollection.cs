namespace NHibernate.Validator.Tests.Collections
{
	using System.Collections.Generic;

	public class HasCollection
	{
		[Size(Min = 2, Max = 5)]
		public IList<string> StringCollection;
	}
}