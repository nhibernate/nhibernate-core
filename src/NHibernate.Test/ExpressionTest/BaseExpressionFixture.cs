using System;
using System.Reflection;

using NHibernate.Cfg;
using NHibernate.Engine;
using NHibernate.SqlCommand;

using NUnit.Framework;

namespace NHibernate.Test.ExpressionTest
{
	/// <summary>
	/// Contains all of the base functionallity for the ExpressionFixtures.
	/// </summary>
	public class BaseExpressionFixture
	{
		protected ISessionFactory factory;
		protected ISessionFactoryImplementor factoryImpl;
		protected Dialect.Dialect dialect;
		
		[SetUp]
		public void SetUp() 
		{
			Configuration cfg = new Configuration();
			cfg.AddResource("NHibernate.DomainModel.Simple.hbm.xml", Assembly.Load("NHibernate.DomainModel"));

			factory = cfg.BuildSessionFactory();
			factoryImpl = (ISessionFactoryImplementor)factory;
			dialect = Dialect.Dialect.GetDialect();
		}

		/// <summary>
		/// Used to pull the Parameter[] and Number of Parameters out of the SqlString
		/// </summary>
		/// <param name="sqlString"></param>
		/// <param name="parameters"></param>
		/// <param name="numOfParameters"></param>
		protected void GetParameters(SqlString sqlString, ref Parameter[] parameters, ref int numOfParameters) 
		{
			
			foreach(object part in sqlString.SqlParts) 
			{
				if(part is Parameter) 
				{
					parameters[numOfParameters] = (Parameter)part;
					numOfParameters++;
				}
			}
		}

		/// <summary>
		/// This compares the text output of the SqlString to what was expected.  It does
		/// not take into account the parameters.
		/// </summary>
		/// <param name="actualSqlString"></param>
		/// <param name="expectedString"></param>
		protected void CompareSqlStrings(SqlString actualSqlString, string expectedString) 
		{
			Assertion.AssertEquals("SqlString.ToString()", expectedString, actualSqlString.ToString());
		}

		protected void CompareSqlStrings(SqlString actualSqlString, string expectedString, int expectedNumOfParameters) 
		{
			Parameter[] actualParameters = new Parameter[expectedNumOfParameters];
			int numOfParameters = 0;

			GetParameters(actualSqlString, ref actualParameters, ref numOfParameters);
			Assertion.AssertEquals("SqlString.ToString()", expectedString, actualSqlString.ToString());
			Assertion.AssertEquals("Num of Parameters", expectedNumOfParameters, numOfParameters);

		}

		protected void CompareSqlStrings(SqlString actualSqlString, string expectedString, Parameter[] expectedParameters) 
		{
			Parameter[] actualParameters = new Parameter[expectedParameters.Length];
			int numOfParameters = 0;

			GetParameters(actualSqlString, ref actualParameters, ref numOfParameters);

			Assertion.AssertEquals("SqlString.ToString()", expectedString, actualSqlString.ToString());
			Assertion.AssertEquals("Num of Parameters", expectedParameters.Length, numOfParameters);


			for(int i = 0; i < expectedParameters.Length; i++) 
			{
				Assertion.AssertEquals("Parameter[" + i + "]", expectedParameters[i], actualParameters[i]);
			}
		}



	}
}
