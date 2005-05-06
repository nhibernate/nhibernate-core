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
		protected override IList Mappings
		{
			get
			{
				return new string[] { "Simple.hbm.xml"};
			}
		}

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
