using System;
using System.Linq;
using NHibernate.Linq;
using NHibernate.SqlCommand;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3386
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return dialect is Dialect.MsSql2000Dialect;
		}

		protected override void OnSetUp()
		{
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				var e1 = new Entity { Name = "Bob" };
				session.Save(e1);

				var e2 = new Entity { Name = "Sally" };
				session.Save(e2);

				session.Flush();
				transaction.Commit();
			}
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

		[Test]
		public void ShouldSupportNonRuntimeExtensionWithoutEntityReference()
		{
			var sqlInterceptor = new SqlInterceptor();
			using (ISession session = OpenSession(sqlInterceptor))
			using (session.BeginTransaction())
			{
				var result = session.Query<Entity>()
					.OrderBy(e => SqlServerFunction.NewID());

				Assert.DoesNotThrow(() => { result.ToList(); });
				Assert.That(sqlInterceptor.Sql.ToString(), Does.Contain(nameof(SqlServerFunction.NewID)).IgnoreCase);
			}
		}
	}

	public static class SqlServerFunction
	{
		[LinqExtensionMethod]
		public static Guid NewID()
		{
			throw new InvalidOperationException("To be translated to SQL only");
		}
	}

	public class SqlInterceptor : EmptyInterceptor
	{
		public SqlString Sql { get; private set; }

		public override SqlString OnPrepareStatement(SqlString sql)
		{
			Sql = sql;
			return base.OnPrepareStatement(sql);
		}
	}
}
