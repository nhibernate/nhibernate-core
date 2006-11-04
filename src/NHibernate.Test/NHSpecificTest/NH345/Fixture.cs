using System;
using System.Collections;
using System.Reflection;

using NUnit.Framework;

using NHibernate.Cfg;
using NHibernate.Engine;

namespace NHibernate.Test.NHSpecificTest.NH345
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		public override string BugNumber
		{
			get { return "NH345"; }
		}

		[Test]
		public void OrderByCompositeProperty()
		{
			if( dialect is Dialect.MsSql2000Dialect )
			{
				Assert.Ignore( "This test fails on MS SQL 2000 because of SQL Server bug" );
			}

			using( ISession s = OpenSession() )
			{
				Client client1 = new Client();
				client1.Name = "Client A";

				Client client2 = new Client();
				client2.Name = "Client B";

				Project project1 = new Project();
				project1.Client = client1;

				Project project2 = new Project();
				project2.Client = client2;

				s.Save( client1 );
				s.Save( client2 );
				s.Save( project1 );
				s.Save( project2 );

				IList listAsc = s.CreateQuery(
					"select p from Project as p order by p.Client.Name asc" ).List();

				Assert.AreEqual( 2, listAsc.Count );
				Assert.AreSame( project1, listAsc[0] );
				Assert.AreSame( project2, listAsc[1] );

				IList listDesc = s.CreateQuery(
					"select p from Project as p order by p.Client.Name desc" ).List();
				Assert.AreEqual( 2, listDesc.Count );
				Assert.AreSame( project1, listDesc[1] );
				Assert.AreSame( project2, listDesc[0] );

				s.Delete( "from Project" );
				s.Delete( "from Client" );
				s.Flush();
			}
		}
	}
}
