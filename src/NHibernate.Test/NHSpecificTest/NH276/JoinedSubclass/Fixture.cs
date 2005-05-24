using System;
using System.Collections;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH276.JoinedSubclass
{
	/// <summary>
	/// Got another report in NH276 that they are still
	/// getting the error.  
	/// </summary>
	[TestFixture]
	public class Fixture : TestCase
	{

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override System.Collections.IList Mappings
		{
			get
			{
				return new string[] { "NHSpecificTest.NH276.JoinedSubclass.Mappings.hbm.xml"};
			}
		}

		[Test]
		public void ManyToOneIdProperties()
		{
			Organization org = new Organization();
			org.OrganizationId = 5;
			org.Name = "the org";

			Status stat = new Status();
			stat.StatusId = 4;
			stat.Name = "the stat";

			Request r = new Request();
			r.Extra = "extra";
			r.Office = org;
			r.Status = stat;

			ISession s = OpenSession();
			s.Save( org );
			s.Save( stat );
			s.Save( r );

			s.Flush();
			s.Close();

			s = OpenSession();
			ICriteria c = s.CreateCriteria( typeof(Request) );
			c.Add( Expression.Expression.Eq( "Status.StatusId", 1) );
			c.Add( Expression.Expression.Eq( "Office.OrganizationId", 1) );
			IList list = c.List();

			Assert.AreEqual( 0, list.Count, "should contain no results" );

			c = s.CreateCriteria( typeof(Request) );
			c.Add( Expression.Expression.Eq( "Status.StatusId", 4) );
			c.Add( Expression.Expression.Eq( "Office.OrganizationId", 5) );
			list = c.List();

			Assert.AreEqual( 1, list.Count, "one matching result" );

			r = list[0] as Request;
			s.Delete( r );
			s.Delete( r.Status );
			s.Delete( r.Office );
			s.Flush();
			s.Close();

		}
	}
}
