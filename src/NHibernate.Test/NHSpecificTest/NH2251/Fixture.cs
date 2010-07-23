using System.Linq;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.NHSpecificTest.NH2251
{
	public class Fixture : BugTestCase
	{
		[Test, Ignore("Changing the original query parameters after FutureValue cause the mix of parameters in SQL.")]
		public void WhenUseFutureSkipTakeThenNotThrow()
		{
			using (var session = OpenSession())
			{
				var query = session.QueryOver<Foo>().Where(o => o.Name == "Graeme");

				var rowcountQuery = query.ToRowCountQuery().FutureValue<int>();
				var resultsQuery = query.Skip(0).Take(50).Future();

				int rowcount;
				Foo[] items;
				Executing.This(() =>
												{
													rowcount = rowcountQuery.Value;
													items = resultsQuery.ToArray();
												}
					).Should().NotThrow();
			}
		}

		[Test]
		public void EnlistingFirstThePaginationAndThenTheRowCountDoesNotThrows()
		{
			using (var session = OpenSession())
			{
				var query = session.QueryOver<Foo>().Where(o => o.Name == "Graeme");

				var resultsQuery = query.Skip(0).Take(50).Future();
				var rowcountQuery = query.ToRowCountQuery().FutureValue<int>();

				int rowcount;
				Foo[] items;
				Executing.This(() =>
				{
					rowcount = rowcountQuery.Value;
					items = resultsQuery.ToArray();
				}
					).Should().NotThrow();
			}
		}
	}
}