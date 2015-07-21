using NHibernate.DomainModel;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;
using NHibernate.Type;
using NUnit.Framework;

namespace NHibernate.Test.ExpressionTest.Projection
{
	using Util;
    using NHibernate.Dialect.Function;

	[TestFixture]
	public class ProjectionFixture : BaseExpressionFixture
	{
		[Test]
		public void RowCountTest()
		{
			ISession session = factory.OpenSession();
			IProjection expression = Projections.RowCount();
			CreateObjects(typeof(Simple), session);
			SqlString sqlString = expression.ToSqlString(criteria, 0, criteriaQuery, new CollectionHelper.EmptyMapClass<string,IFilter>());
			string expectedSql = "count(*) as y0_";
			CompareSqlStrings(sqlString, expectedSql, 0);
			session.Close();
		}

		[Test]
		public void AvgTest()
		{
			ISession session = factory.OpenSession();
			IProjection expression = Projections.Avg("Pay");
			CreateObjects(typeof(Simple), session);
			IType nhType = NHibernateUtil.GuessType(typeof (double));
			SqlType[] sqlTypes = nhType.SqlTypes(this.factoryImpl);
			string sqlTypeString = factoryImpl.Dialect.GetCastTypeName(sqlTypes[0]);
			SqlString sqlString = expression.ToSqlString(criteria, 0, criteriaQuery, new CollectionHelper.EmptyMapClass<string, IFilter>());
			string expectedSql = string.Format("avg(cast(sql_alias.Pay as {0})) as y0_",sqlTypeString);
			CompareSqlStrings(sqlString, expectedSql, 0);
			session.Close();
		}

		[Test]
		public void MaxTest()
		{
			ISession session = factory.OpenSession();
			IProjection expression = Projections.Max("Pay");
			CreateObjects(typeof(Simple), session);
			SqlString sqlString = expression.ToSqlString(criteria, 0, criteriaQuery, new CollectionHelper.EmptyMapClass<string, IFilter>());
			string expectedSql = "max(sql_alias.Pay) as y0_";
			CompareSqlStrings(sqlString, expectedSql, 0);
			session.Close();
		}

		[Test]
		public void MinTest()
		{
			ISession session = factory.OpenSession();
			IProjection expression = Projections.Min("Pay");
			CreateObjects(typeof(Simple), session);
			SqlString sqlString = expression.ToSqlString(criteria, 0, criteriaQuery, new CollectionHelper.EmptyMapClass<string, IFilter>());
			string expectedSql = "min(sql_alias.Pay) as y0_";
			CompareSqlStrings(sqlString, expectedSql, 0);
			session.Close();
		}

		[Test]
		public void CountTest()
		{
			ISession session = factory.OpenSession();
			IProjection expression = Projections.Count("Pay");
			CreateObjects(typeof(Simple), session);
			SqlString sqlString = expression.ToSqlString(criteria, 0, criteriaQuery, new CollectionHelper.EmptyMapClass<string, IFilter>());
			string expectedSql = "count(sql_alias.Pay) as y0_";
			CompareSqlStrings(sqlString, expectedSql, 0);
			session.Close();
		}

		[Test]
		public void CountDistinctTest()
		{
			ISession session = factory.OpenSession();
			IProjection expression = Projections.CountDistinct("Pay");
			CreateObjects(typeof(Simple), session);
			SqlString sqlString = expression.ToSqlString(criteria, 0, criteriaQuery, new CollectionHelper.EmptyMapClass<string, IFilter>());
			string expectedSql = "count(distinct sql_alias.Pay) as y0_";
			CompareSqlStrings(sqlString, expectedSql, 0);
			session.Close();
		}

		[Test]
		public void NvlTest()
		{
			ISession session = factory.OpenSession();
			IProjection expression = Projections.SqlFunction(new NvlFunction(),
			                                                 NHibernateUtil.String, Projections.Property("Name"), Projections.Property("Address"));
			CreateObjects(typeof (Simple), session);
			SqlString sqlString = expression.ToSqlString(criteria, 0, criteriaQuery, new CollectionHelper.EmptyMapClass<string, IFilter>());
			string expectedSql = "nvl(sql_alias.Name, sql_alias.address) as y0_";
			CompareSqlStrings(sqlString, expectedSql, 0);
			session.Close();
		}

		[Test]
		public void DistinctTest()
		{
			ISession session = factory.OpenSession();
			IProjection expression = Projections.Distinct(Projections.Property("Pay"));
			CreateObjects(typeof(Simple), session);
			SqlString sqlString = expression.ToSqlString(criteria, 0, criteriaQuery, new CollectionHelper.EmptyMapClass<string, IFilter>());
			string expectedSql = "distinct sql_alias.Pay as y0_";
			CompareSqlStrings(sqlString, expectedSql, 0);
			session.Close();
		}

		[Test]
		public void GroupPropertyTest()
		{
			ISession session = factory.OpenSession();
			IProjection expression = Projections.GroupProperty("Pay");
			CreateObjects(typeof(Simple), session);
			SqlString sqlString = expression.ToSqlString(criteria, 0, criteriaQuery, new CollectionHelper.EmptyMapClass<string, IFilter>());
			string expectedSql = "sql_alias.Pay as y0_";
			CompareSqlStrings(sqlString, expectedSql, 0);
			SqlString groupSql = expression.ToGroupSqlString(criteria, criteriaQuery, new CollectionHelper.EmptyMapClass<string, IFilter>());
			string expectedGroupSql = "sql_alias.Pay";
			CompareSqlStrings(groupSql, expectedGroupSql);
			session.Close();
		}

		[Test]
		public void IdTest()
		{
			ISession session = factory.OpenSession();
			IProjection expression = Projections.Id();
			CreateObjects(typeof(Simple), session);
			SqlString sqlString = expression.ToSqlString(criteria, 0, criteriaQuery, new CollectionHelper.EmptyMapClass<string, IFilter>());
			string expectedSql = "sql_alias.id_ as y0_";
			CompareSqlStrings(sqlString, expectedSql, 0);
			session.Close();
		}

		[Test]
		public void PropertyTest()
		{
			ISession session = factory.OpenSession();
			IProjection expression = Projections.Property("Pay");
			CreateObjects(typeof(Simple), session);
			SqlString sqlString = expression.ToSqlString(criteria, 0, criteriaQuery, new CollectionHelper.EmptyMapClass<string, IFilter>());
			string expectedSql = "sql_alias.Pay as y0_";
			CompareSqlStrings(sqlString, expectedSql, 0);
			session.Close();
		}

		[Test]
		public void SqlGroupProjectionTest()
		{
			ISession session = factory.OpenSession();
			IProjection expression = Projections.SqlGroupProjection("count(Pay)", "Pay",
			                                                        new string[] {"PayCount"},
			                                                        new IType[] {NHibernateUtil.Double}
				);
			CreateObjects(typeof(Simple), session);
			SqlString sqlString = expression.ToSqlString(criteria, 0, criteriaQuery, new CollectionHelper.EmptyMapClass<string, IFilter>());
			string expectedSql = "count(Pay)";
			CompareSqlStrings(sqlString, expectedSql, 0);
			session.Close();
		}

		[Test]
		public void SqlProjectionTest()
		{
			ISession session = factory.OpenSession();
			IProjection expression = Projections.SqlProjection("count(Pay)",
			                                                   new string[] {"CountOfPay"}, new
			                                                                                	IType[] {NHibernateUtil.Double});
			CreateObjects(typeof(Simple), session);
			SqlString sqlString = expression.ToSqlString(criteria, 0, criteriaQuery, new CollectionHelper.EmptyMapClass<string, IFilter>());
			string expectedSql = "count(Pay)";
			CompareSqlStrings(sqlString, expectedSql, 0);
			session.Close();
		}

		[Test]
		public void SumTest()
		{
			ISession session = factory.OpenSession();
			IProjection expression = Projections.Sum("Pay");
			CreateObjects(typeof(Simple), session);
			SqlString sqlString = expression.ToSqlString(criteria, 0, criteriaQuery, new CollectionHelper.EmptyMapClass<string, IFilter>());
			string expectedSql = "sum(sql_alias.Pay) as y0_";
			CompareSqlStrings(sqlString, expectedSql, 0);
			session.Close();
		}
	}
}
