using System.Data;
using NHibernate.Dialect;
using NHibernate.SqlTypes;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH3311SqlQueryParam
{
	[TestFixture]
	public class SqlQueryParamTypeFixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			using var session = OpenSession();
			using var transaction = session.BeginTransaction();
			var e1 = new Entity {Name = "Bob"};
			session.Save(e1);

			var e2 = new Entity {Name = "Sally"};
			session.Save(e2);

			transaction.Commit();
		}

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return
				//Dialects like SQL Server CE, Firebird don't distinguish AnsiString from String
				(Dialect.GetTypeName(new SqlType(DbType.AnsiString)) != Dialect.GetTypeName(new SqlType(DbType.String))
				 || Dialect is SQLiteDialect);
		}

		protected override void OnTearDown()
		{
			using var session = OpenSession();
			using var transaction = session.BeginTransaction();
			session.CreateQuery("delete from System.Object").ExecuteUpdate();

			transaction.Commit();
		}

		[Test]
		public void AppliesParameterTypeFromQueryParam()
		{
			using var log = new SqlLogSpy();
			using var s = OpenSession();
			s.GetNamedQuery("entityIdByName").SetParameter("name", "Bob").UniqueResult<long>();
			Assert.That(log.GetWholeLog(), Does.Contain("Type: AnsiString"));
		}
	}
}
