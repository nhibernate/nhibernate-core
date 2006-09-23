using NHibernate.DomainModel;
using NHibernate.Expression;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NUnit.Framework;

namespace NHibernate.Test.ExpressionTest.Projection
{
    [TestFixture]
    public class ProjectionFixture : BaseExpressionFixture
    {
        [Test]
        public void RowCountTest()
        {
            ISession session = factory.OpenSession();
            IProjection expression = Projections.RowCount();
            CreateObjects(typeof(Simple), session);
            SqlString sqlString = expression.ToSqlString(criteria, 0, criteriaQuery);
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
            SqlString sqlString = expression.ToSqlString(criteria, 0, criteriaQuery);
            string expectedSql = "avg(sql_alias.Pay) as y0_";
            CompareSqlStrings(sqlString, expectedSql, 0);
            session.Close();
        }

        [Test]
        public void MaxTest()
        {
            ISession session = factory.OpenSession();
            IProjection expression = Projections.Max("Pay");
            CreateObjects(typeof(Simple), session);
            SqlString sqlString = expression.ToSqlString(criteria, 0, criteriaQuery);
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
            SqlString sqlString = expression.ToSqlString(criteria, 0, criteriaQuery);
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
            SqlString sqlString = expression.ToSqlString(criteria, 0, criteriaQuery);
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
            SqlString sqlString = expression.ToSqlString(criteria, 0, criteriaQuery);
            string expectedSql = "count(distinct sql_alias.Pay) as y0_";
            CompareSqlStrings(sqlString, expectedSql, 0);
            session.Close();
        }
        
        [Test]
        public void DistinctTest()
        {
            ISession session = factory.OpenSession();
            IProjection expression = Projections.Distinct(Projections.Property("Pay"));
            CreateObjects(typeof(Simple), session);
            SqlString sqlString = expression.ToSqlString(criteria, 0, criteriaQuery);
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
            SqlString sqlString = expression.ToSqlString(criteria, 0, criteriaQuery);
            string expectedSql = "sql_alias.Pay as y0_";
            CompareSqlStrings(sqlString, expectedSql, 0);
            SqlString groupSql = expression.ToGroupSqlString(criteria, criteriaQuery);
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
            SqlString sqlString = expression.ToSqlString(criteria, 0, criteriaQuery);
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
            SqlString sqlString = expression.ToSqlString(criteria, 0, criteriaQuery);
            string expectedSql = "sql_alias.Pay as y0_";
            CompareSqlStrings(sqlString, expectedSql, 0);
            session.Close();
        }

        [Test]
        public void SqlGroupProjectionTest()
        {
            ISession session = factory.OpenSession();
            IProjection expression = Projections.SqlGroupProjection("count(Pay)", "Pay",
                new string[] { "PayCount" },
                new IType[] { NHibernateUtil.Double }
                );
            CreateObjects(typeof(Simple), session);
            SqlString sqlString = expression.ToSqlString(criteria, 0, criteriaQuery);
            string expectedSql = "count(Pay)";
            CompareSqlStrings(sqlString, expectedSql, 0);
            session.Close();
        }
        
        [Test]
        public void SqlProjectionTest()
        {
            ISession session = factory.OpenSession();
            IProjection expression = Projections.SqlProjection("count(Pay)", 
                new string[] { "CountOfPay" }, new 
                IType[] { NHibernateUtil.Double});
            CreateObjects(typeof(Simple), session);
            SqlString sqlString = expression.ToSqlString(criteria, 0, criteriaQuery);
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
            SqlString sqlString = expression.ToSqlString(criteria, 0, criteriaQuery);
            string expectedSql = "sum(sql_alias.Pay) as y0_";
            CompareSqlStrings(sqlString, expectedSql, 0);
            session.Close();
        }
    }
}