using System;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.NHSpecificTest.NH2324
{
	public class BulkUpdateWithCustomCompositeType: BugTestCase
	{
		public class Scenario: IDisposable
		{
			private readonly ISessionFactory factory;

			public Scenario(ISessionFactory factory)
			{
				this.factory = factory;
				using (ISession s = factory.OpenSession())
				using (ITransaction t = s.BeginTransaction())
				{
					var e = new Entity
					           	{
					           		Data = new CompositeData {DataA = new DateTime(2010, 1, 1), DataB = new DateTime(2010, 2, 2)}
					           	};
					s.Save(e);
					t.Commit();
				}
			}

			public void Dispose()
			{
				using (ISession s = factory.OpenSession())
				using (ITransaction t = s.BeginTransaction())
				{
					s.CreateQuery("delete from Entity").ExecuteUpdate();
					t.Commit();
				}
			}
		}

		[Test]
		public void ShouldAllowBulkupdateWithCompositeUserType()
		{
			using (new Scenario(Sfi))
			{
				string queryString = @"update Entity m set m.Data.DataA = :dataA, m.Data.DataB = :dataB";

				using (ISession s = OpenSession())
				using (ITransaction t = s.BeginTransaction())
				{
					var query = s.CreateQuery(queryString)
					 .SetDateTime("dataA", new DateTime(2010, 3, 3))
					 .SetDateTime("dataB", new DateTime(2010, 4, 4));

					query.Executing(q => q.ExecuteUpdate()).NotThrows();

					t.Commit();
				}
			}
		}
	}
}