namespace NHibernate.Test.Linq
{
	using System.Collections;
	using System.Linq;
	using NUnit.Framework;

	public class CollectionAssert
	{
		public static void AreCountEqual(int expectedCount, IEnumerable collection)
		{
			Assert.AreEqual(expectedCount, collection.Cast<object>().Count());
		}
	}
}