using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Dialect.Function;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1849
{
	public class CustomDialect : MsSql2005Dialect
	{
		public CustomDialect()
		{
			RegisterFunction("contains", new StandardSQLFunction("contains", NHibernateUtil.Boolean));
		}
	}

	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return dialect is MsSql2005Dialect;
		}

		protected override void Configure(Configuration configuration)
		{
			base.Configure(configuration);

			configuration.SetProperty("dialect", "NHibernate.Test.NHSpecificTest.NH1849.CustomDialect, NHibernate.Test");
		}

		/// <summary>
		/// This test may throw an ado exception due to the absence of a full text index,
		/// however the query should compile
		/// </summary>
		[Test, Ignore]
		public void ExecutesCustomSqlFunctionContains()
		{
			sessions.Statistics.Clear();
			using (ISession session = OpenSession())
			{
				session.CreateQuery("from Customer c where contains(c.Name, :smth)").SetString("smth", "aaaa").List();

				Assert.That(sessions.Statistics.QueryExecutionCount, Is.EqualTo(1));
			}
		}
	}
}