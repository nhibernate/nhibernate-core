using System;
using NHibernate.Criterion;
using NHibernate.Dialect;
using NHibernate.Impl;
using NHibernate.Loader.Criteria;
using NUnit.Framework;
using Environment = NHibernate.Cfg.Environment;

namespace NHibernate.Test.NHSpecificTest.NH2700
{
    [TestFixture]
	public class Fixture : BugTestCase
	{
        private Dialect.Dialect _originalDialect;

        protected override bool AppliesTo(Dialect.Dialect dialect)
        {
            return _originalDialect is MsSql2005Dialect;
        }

        protected override void Configure(Cfg.Configuration configuration)
        {
            _originalDialect = Dialect;

            cfg.SetProperty(Environment.Dialect, typeof(CustomDialect).AssemblyQualifiedName);
        }

        public static string GetSql(ICriteria criteria)
        {
            var criteriaImpl = (CriteriaImpl)criteria;
            var session = criteriaImpl.Session;
            var factory = session.Factory;

            var translator =
                new CriteriaQueryTranslator(
                    factory,
                    criteriaImpl,
                    criteriaImpl.EntityOrClassName,
                    CriteriaQueryTranslator.RootSqlAlias);

            var implementors = factory.GetImplementors(criteriaImpl.EntityOrClassName);

            var walker = new CriteriaJoinWalker(
                (Persister.Entity.IOuterJoinLoadable)factory.GetEntityPersister(implementors[0]),
                                    translator,
                                    factory,
                                    criteriaImpl,
                                    criteriaImpl.EntityOrClassName,
                                    session.EnabledFilters);

            return walker.SqlString.ToString();
        }

		[Test]
		public void TestProjection()
		{
            
            using (var s = OpenSession())
            {
                var proj = new SqlFunctionProjection("AddDays", NHibernateUtil.DateTime,
                                                     new IProjection[]
                                                         {
                                                             Projections.Property<ModelClass>(p=>p.Date1),
                                                             Projections.Property<ModelClass>(p=>p.Value1)
                                                         });
                var criteria = s.CreateCriteria<ModelClass>();
                criteria.SetProjection(proj);

                var sql = GetSql(criteria);

                Assert.That(sql, Is.StringMatching("dateadd\\(day,(.*?)Value1,(.*?)Date1\\)"));
                Console.WriteLine(sql.ToString());
            }      
		}
	}
}
