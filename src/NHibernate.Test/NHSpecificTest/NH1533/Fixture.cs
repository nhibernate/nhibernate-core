using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1533
{
	[TestFixture]
	public class Fixture:BugTestCase
	{
		protected override void OnTearDown()
		{
			base.OnTearDown();
			using (ISession session = OpenSession())
			{
				using (ITransaction tx = session.BeginTransaction())
				{
					session.Delete("from Person");
					tx.Commit();
				}
			}
		}
		protected override void OnSetUp()
		{
			using (ISession s = OpenSession())
			{
				using (ITransaction tx = s.BeginTransaction())
				{
					Person e1 = new Person("Joe", 10, 9);
					Person e2 = new Person("Sally", 10, 8);
					Person e3 = new Person("Tim", 20, 40); //20
					Person e4 = new Person("Fred", 20, 7);
					Person e5 = new Person("Mike", 50, 50);
					s.Save(e1);
					s.Save(e2);
					s.Save(e3);
					s.Save(e4);
					s.Save(e5);
					tx.Commit();
				}
			}
		}

		[Test]
		public void Can_query_using_two_orderby_and_limit_altogether()
		{
			using(var sess=OpenSession())
			{
				using(var tran=sess.BeginTransaction() )
				{
					var query =
						sess.CreateQuery(
							"select this.Name,this.ShoeSize,this.IQ from Person as this order by this.IQ asc,this.ShoeSize asc");
					query.SetMaxResults(2);
					query.SetFirstResult(2);
					IList results = query.List();
					Assert.That(results.Count, Is.EqualTo(2));
					Assert.That(((IList)results[0])[0], Is.EqualTo("Fred"));
					Assert.That(((IList)results[1])[0], Is.EqualTo("Tim"));
				}
			}
		}
		[Test]
		public void Can_query_using_two_orderby_and_limit_with_maxresult_only()
		{
			using (var sess = OpenSession())
			{
				using (var tran = sess.BeginTransaction())
				{
					var query =
						sess.CreateQuery(
							"select this.Name,this.ShoeSize,this.IQ from Person as this order by this.IQ asc,this.ShoeSize asc");
					query.SetMaxResults(2);
					IList results = query.List();
					Assert.That(results.Count, Is.EqualTo(2));
					Assert.That(((IList)results[0])[0], Is.EqualTo("Sally"));
					Assert.That(((IList)results[1])[0], Is.EqualTo("Joe"));
				}
			}
		}

		[Test]
		public void Can_query_using_two_orderby_and_limit_with_firstresult_only()
		{
			using (var sess = OpenSession())
			{
				using (var tran = sess.BeginTransaction())
				{
					var query =
						sess.CreateQuery(
							"select this.Name,this.ShoeSize,this.IQ from Person as this order by this.IQ asc,this.ShoeSize asc");
					query.SetFirstResult(2);
					IList results = query.List();
					Assert.That(results.Count, Is.EqualTo(3));
					Assert.That(((IList)results[0])[0], Is.EqualTo("Fred"));
					Assert.That(((IList)results[1])[0], Is.EqualTo("Tim"));
					Assert.That(((IList)results[2])[0], Is.EqualTo("Mike"));
				}
			}
		}
	}
}
