using System;
using System.Collections;

using NUnit.Framework;

namespace NHibernate.Test.QueryTest
{
	/// <summary>
	/// Tests functionality for named parameter queries.
	/// </summary>
	[TestFixture]
	public class NamedParametersFixture : TestCase
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
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			
			try
			{
				
				IQuery q = s.CreateQuery("from s in class Simple where s.Name=:Name and s.Count=:Count");
				// Just set the Name property not the count
				q.SetAnsiString("Name", "Fred");

				// Try to execute it
				IList list = q.List();
			}
			finally
			{
				t.Rollback();
				s.Close();
			}
		}
	}
}
