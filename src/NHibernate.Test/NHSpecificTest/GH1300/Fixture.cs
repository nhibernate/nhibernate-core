using System.Data;
using System.Data.SqlClient;
using System.Linq;
using NHibernate.Cfg;
using NHibernate.Connection;
using NHibernate.Criterion;
using NHibernate.Dialect;
using NHibernate.Driver;
using NHibernate.Engine;
using NHibernate.Exceptions;
using NHibernate.Linq;
using NHibernate.Type;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH1300
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return dialect is MsSql2000Dialect;
		}

		protected override bool AppliesTo(ISessionFactoryImplementor factory)
		{
			return factory.ConnectionProvider.Driver is SqlClientDriver;
		}

		protected override void Configure(Configuration configuration)
		{
			using (var cp = ConnectionProviderFactory.NewConnectionProvider(cfg.Properties))
			{
				if (cp.Driver is SqlClientDriver)
				{
					configuration.SetProperty(Environment.ConnectionDriver, typeof(TestSqlClientDriver).AssemblyQualifiedName);
				}
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.Delete("from System.Object");

				session.Flush();
				transaction.Commit();
			}
		}

		protected override void OnSetUp()
		{
			base.OnSetUp();
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var e1 = new Entity { Name = "Bob" };
				session.Save(e1);
				transaction.Commit();
			}
		}

		[Test]
		public void InsertShouldUseMappedSize()
		{
			Driver.ClearCommands();

			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var e1 = new Entity { Name = "1", AnsiName = "2", FullText = "3", AnsiFullText = "4" };
				session.Save(e1);
				transaction.Commit();
				Assert.That(Driver.LastCommandParameters.Single(x => (string) x.Value == "1").SqlDbType, Is.EqualTo(SqlDbType.NVarChar));
				Assert.That(Driver.LastCommandParameters.Single(x => (string) x.Value == "1").Size, Is.EqualTo(3));
				Assert.That(Driver.LastCommandParameters.Single(x => (string) x.Value == "2").SqlDbType, Is.EqualTo(SqlDbType.VarChar));
				Assert.That(Driver.LastCommandParameters.Single(x => (string) x.Value == "2").Size, Is.EqualTo(3));
				Assert.That(Driver.LastCommandParameters.Single(x => (string) x.Value == "3").SqlDbType, Is.EqualTo(SqlDbType.NVarChar));
				Assert.That(Driver.LastCommandParameters.Single(x => (string) x.Value == "3").Size, Is.EqualTo(MsSql2000Dialect.MaxSizeForClob));
				Assert.That(Driver.LastCommandParameters.Single(x => (string) x.Value == "4").SqlDbType, Is.EqualTo(SqlDbType.VarChar));
				Assert.That(Driver.LastCommandParameters.Single(x => (string) x.Value == "4").Size, Is.EqualTo(MsSql2000Dialect.MaxSizeForAnsiClob));
			}
		}

		[Test]
		public void InsertWithTooLongValuesShouldThrow()
		{
			Driver.ClearCommands();

			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var e1 = new Entity { Name = "Alal", AnsiName = "Alal" };

				var ex = Assert.Throws<GenericADOException>(
					() =>
					{
						session.Save(e1);
						transaction.Commit();
					});

				var sqlEx = ex.InnerException as SqlException;
				Assert.That(sqlEx, Is.Not.Null);
				// Error code is different if verbose truncation warning is enabled
				// See details: https://www.brentozar.com/archive/2019/03/how-to-fix-the-error-string-or-binary-data-would-be-truncated/
				Assert.That(sqlEx.Number, Is.EqualTo(8152).Or.EqualTo(2628));
			}
		}

		[TestCase("Name", SqlDbType.NVarChar)]
		[TestCase("AnsiName", SqlDbType.VarChar)]
		public void LinqEqualsShouldUseMappedSize(string property, SqlDbType expectedDbType)
		{
			Driver.ClearCommands();

			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				if (property == "Name")
				{
					session.Query<Entity>().Where(x => x.Name == "Bob").ToList();
				}
				else
				{
					session.Query<Entity>().Where(x => x.AnsiName == "Bob").ToList();
				}
				Assert.That(Driver.LastCommandParameters.First().Size, Is.EqualTo(3));
				Assert.That(Driver.LastCommandParameters.First().SqlDbType, Is.EqualTo(expectedDbType));
			}
		}

		[Test]
		public void MappedAsShouldUseExplicitSize()
		{
			Driver.ClearCommands();

			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.Query<Entity>().Where(x => x.Name == "Bob".MappedAs(TypeFactory.Basic("AnsiString(200)"))).ToList();

				Assert.That(Driver.LastCommandParameters.First().Size, Is.EqualTo(200));
				Assert.That(Driver.LastCommandParameters.First().SqlDbType, Is.EqualTo(SqlDbType.VarChar));
			}
		}

		[TestCase("Name", SqlDbType.NVarChar)]
		[TestCase("AnsiName", SqlDbType.VarChar)]
		public void HqlLikeShouldUseLargerSize(string property, SqlDbType expectedDbType)
		{
			Driver.ClearCommands();

			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.CreateQuery("from Entity where " + property + " like :name").SetParameter("name", "%Bob%").List<Entity>();

				Assert.That(Driver.LastCommandParameters.First().Size, Is.GreaterThanOrEqualTo(5));
				Assert.That(Driver.LastCommandParameters.First().SqlDbType, Is.EqualTo(expectedDbType));
			}
		}

		[TestCase("Name", SqlDbType.NVarChar)]
		[TestCase("AnsiName", SqlDbType.VarChar)]
		public void CriteriaEqualsShouldUseMappedSize(string property, SqlDbType expectedDbType)
		{
			Driver.ClearCommands();

			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				Driver.ClearCommands();

				session.CreateCriteria<Entity>().Add(Restrictions.Eq(property, "Bob"))
								  .List<Entity>();

				Assert.That(Driver.LastCommandParameters.First().Size, Is.GreaterThanOrEqualTo(3));
				Assert.That(Driver.LastCommandParameters.First().SqlDbType, Is.EqualTo(expectedDbType));
			}
		}

		[TestCase("Name", SqlDbType.NVarChar)]
		[TestCase("AnsiName", SqlDbType.VarChar)]
		public void CriteriaLikeShouldUseLargerSize(string property, SqlDbType expectedDbType)
		{
			Driver.ClearCommands();

			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.CreateCriteria<Entity>().Add(Restrictions.Like(property, "%Bob%"))
								  .List<Entity>();

				Assert.That(Driver.LastCommandParameters.First().Size, Is.GreaterThanOrEqualTo(5));
				Assert.That(Driver.LastCommandParameters.First().SqlDbType, Is.EqualTo(expectedDbType));
			}
		}
		private TestSqlClientDriver Driver => Sfi.ConnectionProvider.Driver as TestSqlClientDriver;
	}
}
