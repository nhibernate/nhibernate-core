using System.Reflection;
using NHibernate.Cfg;
using NHibernate.DomainModel;
using NHibernate.Engine;
using NHibernate.Impl;
using NHibernate.Loader.Criteria;
using NHibernate.SqlCommand;
using NUnit.Framework;

namespace NHibernate.Test.ExpressionTest
{
	/// <summary>
	/// Contains all of the base functionality for the ExpressionFixtures.
	/// </summary>
	public class BaseExpressionFixture
	{
		protected ISessionFactory factory;
		protected ISessionFactoryImplementor factoryImpl;
		protected Dialect.Dialect dialect;

		protected const string SqlAlias = "sql_alias";
		protected CriteriaImpl criteria;
		protected CriteriaQueryTranslator criteriaQuery;

		protected void CreateObjects(System.Type rootClass, ISession session)
		{
			criteria = (CriteriaImpl) session.CreateCriteria(rootClass);
			criteriaQuery = new CriteriaQueryTranslator(
				(ISessionFactoryImplementor) factory,
				criteria, criteria.EntityOrClassName, SqlAlias);
		}

		[SetUp]
		public virtual void SetUp()
		{
			Configuration cfg = new Configuration();
			Assembly dm = Assembly.GetAssembly(typeof(Simple));
			cfg.AddResource("NHibernate.DomainModel.Simple.hbm.xml", dm);
			cfg.AddResource("NHibernate.DomainModel.NHSpecific.SimpleComponent.hbm.xml", dm);
			cfg.AddResource("NHibernate.DomainModel.Multi.hbm.xml", dm);

			factory = cfg.BuildSessionFactory();
			factoryImpl = (ISessionFactoryImplementor) factory;
			dialect = factoryImpl.Dialect;
		}

		/// <summary>
		/// This compares the text output of the SqlString to what was expected.  It does
		/// not take into account the parameters.
		/// </summary>
		protected void CompareSqlStrings(SqlString actualSqlString, string expectedString)
		{
			Assert.AreEqual(expectedString, actualSqlString.ToString(), "SqlString.ToString()");
		}

		protected void CompareSqlStrings(SqlString actualSqlString, string expectedString, int expectedNumOfParameters)
		{
			Assert.AreEqual(expectedString, actualSqlString.ToString(), "SqlString.ToString()");
			Assert.AreEqual(expectedNumOfParameters, actualSqlString.GetParameterCount(), "Num of Parameters");
		}
	}
}