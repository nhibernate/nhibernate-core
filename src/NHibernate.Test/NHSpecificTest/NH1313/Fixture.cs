using System;
using NHibernate.Cfg;
using NHibernate.Criterion;
using NHibernate.Dialect.Function;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1313
{
	// http://jira.nhibernate.org/browse/NH-1313
	[TestFixture]
	public class Fixture : BugTestCase
	{

		public override string BugNumber
		{
			get { return "NH1313"; }
		}

		protected override void Configure(Configuration configuration)
		{
			Dialect.Dialect d = Dialect;
			ISQLFunction toReRegister = d.Functions["current_timestamp"];
			configuration.AddSqlFunction("MyCurrentTime", toReRegister);
		}

		[Test]
		public void Bug()
		{
			A a = new A("NH1313");

			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				s.Save(a);
				tx.Commit();
			}
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				DateTime result =
					s.CreateCriteria(typeof (A)).SetProjection(new SqlFunctionProjection("MyCurrentTime", NHibernateUtil.DateTime)).
						UniqueResult<DateTime>();
				// we are simply checking that the function is parsed and executed
				s.Delete(a);
				tx.Commit();
			}
		}

	}
}