using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1097
{
	[TestFixture]
	public class Fixture:BugTestCase
	{
		protected override void OnSetUp()
		{
			using(var session=this.OpenSession())
			using(var tran=session.BeginTransaction())
			{
				session.Save(new Person {Name = "Fabio"});
				session.Save(new Person { Name = "Dario" });
				tran.Commit();
			}
		}
		protected override void OnTearDown()
		{
			using (var session = this.OpenSession())
			using (var tran = session.BeginTransaction())
			{
				session.Save(new Person { Name = "Fabio" });
				session.Save(new Person { Name = "Dario" });
				session.Delete("from Person");
				tran.Commit();
			}
		}

		[Test]
		public void ThrowsExceptionWhenColumnNameIsUsedInQuery()
		{
			using (var session = this.OpenSession())
			using (var tran = session.BeginTransaction())
			{

				Assert.Throws<QueryException>(delegate
				                              	{
													var query = session.CreateQuery("from Person p where p.namecolumn=:nameOfPerson");
													query.SetString("nameOfPerson", "Dario");
				                              		query.List();
				                              	});
			}
		}
	}
}
