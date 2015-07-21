using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Criterion;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1877
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			using(var session=OpenSession())
			using(var tran=session.BeginTransaction())
			{
				session.Save(new Person {BirthDate = new DateTime(1988, 7, 21)});
				session.Save(new Person { BirthDate = new DateTime(1987, 7, 22) });
				session.Save(new Person { BirthDate = new DateTime(1986, 7, 23) });
				session.Save(new Person { BirthDate = new DateTime(1987, 7, 24) });
				session.Save(new Person { BirthDate = new DateTime(1988, 7, 25) });
				tran.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var tran = session.BeginTransaction())
			{
				session.CreateQuery("delete from Person").ExecuteUpdate();
				tran.Commit();
			}
		}

		[Test]
		public void CanGroupByWithPropertyName()
		{
			using(var session=OpenSession())
			{
				var crit = session.CreateCriteria(typeof (Person))
					.SetProjection(Projections.GroupProperty("BirthDate"),
					               Projections.Count("Id"));
				var result = crit.List();
				Assert.That(result,Has.Count.EqualTo(5));
			}
		}

		[Test]
		public void CanGroupByWithSqlFunctionProjection()
		{
			using (var session = OpenSession())
			{
				var crit = session.CreateCriteria(typeof (Person))
					.SetProjection(
					Projections.GroupProperty(Projections.SqlFunction("month", NHibernateUtil.Int32, Projections.Property("BirthDate"))));

				var result = crit.UniqueResult();
				Assert.That(result,Is.EqualTo(7));
			}
		}
	}
}