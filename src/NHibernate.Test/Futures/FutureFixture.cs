using System.Collections;

using NUnit.Framework;

namespace NHibernate.Test.Futures
{
	public abstract class FutureFixture : TestCase
	{
		protected override string[] Mappings
		{
			get { return new[] { "Futures.Mappings.hbm.xml" }; }
		}

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected void IgnoreThisTestIfMultipleQueriesArentSupportedByDriver()
		{
			var driver = Sfi.ConnectionProvider.Driver;
			if (driver.SupportsMultipleQueries == false)
				Assert.Ignore("Driver {0} does not support multi-queries", driver.GetType().FullName);
		}

		protected void CreatePersons()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var p = new Person { Name = "ParentTwoChildren", Age = 40 };
				var c1 = new Person { Parent = p, Name = "Child1", Age = 7 };
				var c2 = new Person { Parent = p, Name = "Child2", Age = 3 };
				p.Children.Add(c1);
				p.Children.Add(c2);

				session.Save(p);
				session.Save(c1);
				session.Save(c2);
				transaction.Commit();
			}
		}
	}
}
