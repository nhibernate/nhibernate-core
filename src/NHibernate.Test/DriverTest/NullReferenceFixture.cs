using System;
using System.Collections;
using NUnit.Framework;

namespace NHibernate.Test.DriverTest
{
	/// <summary>
	/// http://jira.nhibernate.org/browse/NH-177
	/// </summary>
	[TestFixture]
	public class NullReferenceFixture : TestCase
	{
		[SetUp]
		public void Setup()
		{
			ExportSchema( new string[] { "Simple.hbm.xml"} );
		}

		/// <summary>
		/// No value is assigned for the given named parameter
		/// </summary>
		/// <remarks>
		/// Was giving a NullReferenceException in NHibernate.Driver.DriverBase.GenerateParameter
		/// at the line "dbParam.DbType = parameter.SqlType.DbType;" because "parameter" existed
		/// but all properties were null.
		/// </remarks>
		/// TODO: I think this fixture is redundant now due to the QueryTest fixtures, just mark it so that it catches the new exception type for now
		[Test, ExpectedException(typeof(NHibernate.QueryException))]
		public void NoParameterNameNullReference()
		{
			ISession s = null;
			try
			{
				s = sessions.OpenSession();
				Console.WriteLine( "about to run query" );
				IQuery q = s.CreateQuery( "from Simple s where s.Name = :missing" );
				Assert.IsNotNull( q );
				q.List();
			}
			catch( ADOException ae )
			{
				Assert.IsTrue( ae.InnerException is QueryException );
				Assert.IsTrue( ae.InnerException.Message.StartsWith( "No value assigned to parameter" ) );
			}
			catch( Exception e )
			{
				Console.WriteLine( e.ToString() );
				throw;
			}
			finally
			{
				s.Close();
			}
		}

		/// <summary>
		/// query succeeds when the parameter is assigned a value
		/// </summary>
		[Test]
		public void NamedParameterAssignedNoError()
		{
			ISession s = null;
			try
			{
				s = sessions.OpenSession();
				Console.WriteLine( "about to run query" );
				IQuery q = s.CreateQuery( "from Simple s where s.Name = :missing" );
				Assert.IsNotNull( q );
				q.SetString( "missing", "nhibernate" );
				IList list = q.List();
				Assert.IsNotNull( list );
			}
			catch( Exception e )
			{
				Console.WriteLine( e.ToString() );
				throw;
			}
			finally
			{
				s.Close();
			}
		}
	}
}