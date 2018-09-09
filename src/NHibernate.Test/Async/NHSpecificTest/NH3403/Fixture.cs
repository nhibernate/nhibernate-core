﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


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
	using System.Threading.Tasks;
	using System.Threading;
	[TestFixture]
	public class FixtureAsync : BugTestCase
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
		public async Task InsertShouldUseMappedSizeAsync()
		{
			Driver.ClearCommands();

			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				var e1 = new Entity { Name = "Al", AnsiName = "Al" };
				await (session.SaveAsync(e1));
				await (transaction.CommitAsync());
				Assert.AreEqual(SqlDbType.NVarChar, Driver.LastCommandParameters.First().SqlDbType);
				Assert.AreEqual(3, Driver.LastCommandParameters.First().Size);
				Assert.AreEqual(SqlDbType.VarChar, Driver.LastCommandParameters.Last().SqlDbType);
				Assert.AreEqual(3, Driver.LastCommandParameters.Last().Size);
			}
		}

		[Test]
		public void InsertWithTooLongValuesShouldThrowAsync()
		{
			Driver.ClearCommands();

			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				var e1 = new Entity { Name = "Alal", AnsiName = "Alal" };

				var ex = Assert.ThrowsAsync<GenericADOException>(
					async () =>
					{
						await (session.SaveAsync(e1));
						await (transaction.CommitAsync());
					});

				var sqlEx = ex.InnerException as SqlException;
				Assert.IsNotNull(sqlEx);
				Assert.That(sqlEx.Number, Is.EqualTo(8152));
			}
		}

		[TestCase("Name", SqlDbType.NVarChar)]
		[TestCase("AnsiName", SqlDbType.VarChar)]
		public async Task LinqEqualsShouldUseMappedSizeAsync(string property, SqlDbType expectedDbType, CancellationToken cancellationToken = default(CancellationToken))
		{
			Driver.ClearCommands();

			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				if (property == "Name")
				{
					await (session.Query<Entity>().Where(x => x.Name == "Bob").ToListAsync(cancellationToken));
				}
				else
				{
					await (session.Query<Entity>().Where(x => x.AnsiName == "Bob").ToListAsync(cancellationToken));
				}
				Assert.AreEqual(3, Driver.LastCommandParameters.First().Size);
				Assert.AreEqual(expectedDbType, Driver.LastCommandParameters.First().SqlDbType);
			}
		}
		
		[TestCase("Name", SqlDbType.NVarChar)]
		[TestCase("AnsiName", SqlDbType.VarChar)]
		public async Task HqlLikeShouldUseLargerSizeAsync(string property, SqlDbType expectedDbType, CancellationToken cancellationToken = default(CancellationToken))
		{
			Driver.ClearCommands();

			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				await (session.CreateQuery("from Entity where " + property + " like :name").SetParameter("name", "%Bob%").ListAsync<Entity>(cancellationToken));

				Assert.GreaterOrEqual(Driver.LastCommandParameters.First().Size, 5);
				Assert.AreEqual(expectedDbType, Driver.LastCommandParameters.First().SqlDbType);
			}
		}

		[TestCase("Name", SqlDbType.NVarChar)]
		[TestCase("AnsiName", SqlDbType.VarChar)]
		public async Task CriteriaEqualsShouldUseMappedSizeAsync(string property, SqlDbType expectedDbType, CancellationToken cancellationToken = default(CancellationToken))
		{
			Driver.ClearCommands();

			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				Driver.ClearCommands();

				await (session.CreateCriteria<Entity>().Add(Restrictions.Eq(property, "Bob"))
								  .ListAsync<Entity>(cancellationToken));

				Assert.GreaterOrEqual(Driver.LastCommandParameters.First().Size, 3);
				Assert.AreEqual(expectedDbType, Driver.LastCommandParameters.First().SqlDbType);
			}
		}
		
		[TestCase("Name", SqlDbType.NVarChar)]
		[TestCase("AnsiName", SqlDbType.VarChar)]
		public async Task CriteriaLikeShouldUseLargerSizeAsync(string property, SqlDbType expectedDbType, CancellationToken cancellationToken = default(CancellationToken))
		{
			Driver.ClearCommands();

			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				await (session.CreateCriteria<Entity>().Add(Restrictions.Like(property, "%Bob%"))
								  .ListAsync<Entity>(cancellationToken));

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
