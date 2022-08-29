using NHibernate.Connection;
using NHibernate.Dialect;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1985
{
	[TestFixture]
	public class SampleTest : BugTestCase
	{
		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			// Test written with raw SQL queries, not compatible with all databases.
			return dialect is MsSql2000Dialect;
		}

		protected override void OnSetUp()
		{
			if (0 == ExecuteStatement("INSERT INTO DomainClass (Id, Label) VALUES (1, 'TEST record');"))
			{
				Assert.Fail("Insertion of test record failed.");
			}
		}

		protected override void OnTearDown()
		{
			base.OnTearDown();

			ExecuteStatement("DELETE FROM DomainClass WHERE Id=1;");
		}

		[Test]
		[Ignore("It is valid to delete immutable entities")]
		public void AttemptToDeleteImmutableObjectShouldThrow()
		{
			using (ISession session = OpenSession())
			{
				Assert.Throws<HibernateException>(() =>
					{
						using (ITransaction trans = session.BeginTransaction())
						{
							var entity = session.Get<DomainClass>(1);
							session.Delete(entity);

							trans.Commit(); // This used to throw...
						}
					});
			}

			using (IConnectionProvider prov = ConnectionProviderFactory.NewConnectionProvider(cfg.Properties))
			{
				var conn = prov.GetConnection();

				try
				{
					using (var comm = conn.CreateCommand())
					{
						comm.CommandText = "SELECT Id FROM DomainClass WHERE Id=1 AND Label='TEST record'";
						object result = comm.ExecuteScalar();

						Assert.That(result != null, "Immutable object has been deleted!");
					}
				}
				finally
				{
					prov.CloseConnection(conn);
				}
			}
		}

		[Test]
		public void AllowDeletionOfImmutableObject()
		{
			using (ISession session = OpenSession())
			{
				Assert.DoesNotThrow(() =>
					{
						using (ITransaction trans = session.BeginTransaction())
						{
							var entity = session.Get<DomainClass>(1);
							session.Delete(entity);

							trans.Commit();
						}
					});
			}

			using (IConnectionProvider prov = ConnectionProviderFactory.NewConnectionProvider(cfg.Properties))
			{
				var conn = prov.GetConnection();

				try
				{
					using (var comm = conn.CreateCommand())
					{
						comm.CommandText = "SELECT Id FROM DomainClass WHERE Id=1 AND Label='TEST record'";
						object result = comm.ExecuteScalar();

						Assert.That(result == null, "Immutable object has not been deleted!");
					}
				}
				finally
				{
					prov.CloseConnection(conn);
				}
			}
		}
	}
}
