using System.Data;
using System.Data.SqlClient;
using System.Linq;
using NHibernate.Cfg;
using NHibernate.Criterion;
using NHibernate.Dialect;
using NHibernate.Driver;
using NHibernate.Engine;
using NHibernate.Exceptions;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3403
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
			cfg.SetProperty(Environment.ConnectionDriver, typeof(TestSqlClientDriver).AssemblyQualifiedName);
		}

		protected override void OnTearDown()
		{
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				session.Delete("from System.Object");

				session.Flush();
				transaction.Commit();
			}
		}

		protected override void OnSetUp()
		{
			base.OnSetUp();
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
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

			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				var e1 = new Entity { Name = "Al", AnsiName = "Al" };
				session.Save(e1);
				transaction.Commit();
				Assert.AreEqual(SqlDbType.NVarChar, Driver.LastCommandParameters.First().SqlDbType);
				Assert.AreEqual(3, Driver.LastCommandParameters.First().Size);
				Assert.AreEqual(SqlDbType.VarChar, Driver.LastCommandParameters.Last().SqlDbType);
				Assert.AreEqual(3, Driver.LastCommandParameters.Last().Size);
			}
		}

		[Test]
		public void InsertWithTooLongValuesShouldThrow()
		{
			Driver.ClearCommands();

			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				var e1 = new Entity { Name = "Alal", AnsiName = "Alal" };

				var ex = Assert.Throws<GenericADOException>(
					() =>
					{
						session.Save(e1);
						transaction.Commit();
					});

				var sqlEx = ex.InnerException as SqlException;
				Assert.IsNotNull(sqlEx);
				Assert.That(sqlEx.Number, Is.EqualTo(8152));
			}
		}

		[TestCase("Name", SqlDbType.NVarChar)]
		[TestCase("AnsiName", SqlDbType.VarChar)]
		public void LinqEqualsShouldUseMappedSize(string property, SqlDbType expectedDbType)
		{
			Driver.ClearCommands();

			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				if (property == "Name")
				{
					session.Query<Entity>().Where(x => x.Name == "Bob").ToList();
				}
				else
				{
					session.Query<Entity>().Where(x => x.AnsiName == "Bob").ToList();
				}
				Assert.AreEqual(3, Driver.LastCommandParameters.First().Size);
				Assert.AreEqual(expectedDbType, Driver.LastCommandParameters.First().SqlDbType);
			}
		}
		
		[TestCase("Name", SqlDbType.NVarChar)]
		[TestCase("AnsiName", SqlDbType.VarChar)]
		public void HqlLikeShouldUseLargerSize(string property, SqlDbType expectedDbType)
		{
			Driver.ClearCommands();

			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				session.CreateQuery("from Entity where " + property + " like :name").SetParameter("name", "%Bob%").List<Entity>();

				Assert.GreaterOrEqual(Driver.LastCommandParameters.First().Size, 5);
				Assert.AreEqual(expectedDbType, Driver.LastCommandParameters.First().SqlDbType);
			}
		}

		[TestCase("Name", SqlDbType.NVarChar)]
		[TestCase("AnsiName", SqlDbType.VarChar)]
		public void CriteriaEqualsShouldUseMappedSize(string property, SqlDbType expectedDbType)
		{
			Driver.ClearCommands();

			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				Driver.ClearCommands();

				session.CreateCriteria<Entity>().Add(Restrictions.Eq(property, "Bob"))
								  .List<Entity>();

				Assert.GreaterOrEqual(Driver.LastCommandParameters.First().Size, 3);
				Assert.AreEqual(expectedDbType, Driver.LastCommandParameters.First().SqlDbType);
			}
		}
		
		[TestCase("Name", SqlDbType.NVarChar)]
		[TestCase("AnsiName", SqlDbType.VarChar)]
		public void CriteriaLikeShouldUseLargerSize(string property, SqlDbType expectedDbType)
		{
			Driver.ClearCommands();

			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				session.CreateCriteria<Entity>().Add(Restrictions.Like(property, "%Bob%"))
								  .List<Entity>();

				Assert.GreaterOrEqual(Driver.LastCommandParameters.First().Size, 5);
				Assert.AreEqual(expectedDbType, Driver.LastCommandParameters.First().SqlDbType);
			}
		}
		private TestSqlClientDriver Driver
		{
			get { return Sfi.ConnectionProvider.Driver as TestSqlClientDriver; }
		}
	}
}
