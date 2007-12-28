namespace NHibernate.Validator.Tests.Collections
{
	using System.Collections.Generic;

	public class HasShowCollection
	{
		[Valid, Size(Min = 2,Max = 3)]
		public IList<Show> Shows;
	}
}