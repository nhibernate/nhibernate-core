using System;
using System.Collections;
using System.Data;

using NHibernate.Cfg;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.Type;

using NUnit.Framework;

namespace NHibernate.Test.QueryTest
{
	/// <summary>
	/// Summary description for PositionalParametersFixture.
	/// </summary>
	[TestFixture]
	public class PositionalParametersFixture : TestCase
	{
		#region NUnit.Framework.TestFixture Members

		[TestFixtureSetUp]
		public void TestFixtureSetUp() 
		{
			ExportSchema( new string[] { "Simple.hbm.xml"} );
		}

		[SetUp]
		public void SetUp() 
		{
			// there are test in here where we don't need to resetup the 
			// tables - so only set the tables up once
		}

		[TearDown]
		public override void TearDown() 
		{
			// do nothing except not let the base TearDown get called
		}

		[TestFixtureTearDown]
		public void TestFixtureTearDown() 
		{
			base.TearDown();
		}

		#endregion

		[Test, ExpectedException(typeof(NHibernate.QueryException))]
		public void TestMissingHQLParameters()
		{
			ISession s = sessions.OpenSession();
			ITransaction t = s.BeginTransaction();
			
			IQuery q = s.CreateQuery("from s in class Simple where s.Name=? and s.Count=?");
			// Set the first property, but not the second
			q.SetParameter(0, "Fred");

			// Try to execute it
			IList list = q.List();
		}

		[Test, ExpectedException(typeof(NHibernate.QueryException))]
		public void TestMissingHQLParameters2()
		{
			ISession s = sessions.OpenSession();
			ITransaction t = s.BeginTransaction();
			
			IQuery q = s.CreateQuery("from s in class Simple where s.Name=? and s.Count=?");
			// Set the second property, but not the first - should give a nice not found at position xxx error
			q.SetParameter(1, "Fred");

			// Try to execute it
			IList list = q.List();
		}

		[Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void TestPositionOutOfBounds()
		{
			ISession s = sessions.OpenSession();
			ITransaction t = s.BeginTransaction();
			
			IQuery q = s.CreateQuery("from s in class Simple where s.Name=? and s.Count=?");
			// Try to set the third positional parameter
			q.SetParameter(3, "Fred");

			// Try to execute it
			IList list = q.List();
		}

		[Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void TestNoPositionalParameters()
		{
			ISession s = sessions.OpenSession();
			ITransaction t = s.BeginTransaction();
			
			IQuery q = s.CreateQuery("from s in class Simple where s.Name=:Name and s.Count=:Count");
			// Try to set the first property
			q.SetParameter(0, "Fred");

			// Try to execute it
			IList list = q.List();
		}
	}
}
