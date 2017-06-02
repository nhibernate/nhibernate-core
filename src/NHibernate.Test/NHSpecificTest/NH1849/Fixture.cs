using System.Text.RegularExpressions;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Dialect.Function;
using NHibernate.Engine.Query;
using NHibernate.Hql;
using NHibernate.Util;
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
		private bool _OrignalDialectIsMsSql2005Dialect;

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return _OrignalDialectIsMsSql2005Dialect;
		}

		protected override void Configure(Configuration configuration)
		{
			base.Configure(configuration);

			// Ugly hack.
			_OrignalDialectIsMsSql2005Dialect = Regex.IsMatch(configuration.GetProperty("dialect"), "MsSql200(5|8)Dialect");

			configuration.SetProperty("dialect", "NHibernate.Test.NHSpecificTest.NH1849.CustomDialect, NHibernate.Test");
		}

		/// <summary>
	  /// We don't actually execute the query, since it will throw an ado exception due to the absence of a full text index,
	  /// however the query should compile
	  /// </summary>
		[Test]
		public void ExecutesCustomSqlFunctionContains()
		{
		 string hql = @"from Customer c where contains(c.Name, :smth)";

		 HQLQueryPlan plan = new QueryExpressionPlan(new StringQueryExpression(hql), false, new CollectionHelper.EmptyMapClass<string, IFilter>(), Sfi);

		 Assert.AreEqual(1, plan.ParameterMetadata.NamedParameterNames.Count);
		 Assert.AreEqual(1, plan.QuerySpaces.Count);
		 Assert.AreEqual(1, plan.SqlStrings.Length);
	  }
	}
}