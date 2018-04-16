using NUnit.Framework;
using NHibernate.Dialect;
using NHibernate.Driver;
using NHibernate.Exceptions;
using NHibernate.Test.ExceptionsTest;
using NHibernate.Engine;

namespace NHibernate.Test.NHSpecificTest.NH2020
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void Configure(Cfg.Configuration configuration)
		{
			configuration.SetProperty(Cfg.Environment.BatchSize, "10");

			configuration.SetProperty(	Cfg.Environment.SqlExceptionConverter,
										typeof (MSSQLExceptionConverterExample).AssemblyQualifiedName);
		}

		protected override bool AppliesTo(NHibernate.Dialect.Dialect dialect)
		{
			return dialect is MsSql2000Dialect;
		}

		protected override bool AppliesTo(ISessionFactoryImplementor factory)
		{
			// Use a SQL Server Client exception converter, cannot work for ODBC or OleDb
			return factory.ConnectionProvider.Driver is SqlClientDriver;
		}

		protected override void OnTearDown()
		{
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				s.Delete("from One");
				s.Delete("from Many");
				tx.Commit();
			}
		}

		[Test]
		public void ISQLExceptionConverter_gets_called_if_batch_size_enabled()
		{
			long oneId;

			using(var s = OpenSession())
			using(var tx = s.BeginTransaction())
			{
				var one = new One();
				s.Save(one);

				var many = new Many { One = one };
				s.Save(many);

				tx.Commit();

				oneId = one.Id;
			}

			using(ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				var one = s.Load<One>(oneId);
				s.Delete(one);
				Assert.That(() => tx.Commit(), Throws.TypeOf<ConstraintViolationException>());
			}
		}
	}
}
