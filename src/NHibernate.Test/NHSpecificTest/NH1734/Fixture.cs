using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1734
{
	[TestFixture]
	public class Fixture:BugTestCase
	{
		protected override void OnSetUp()
		{
			using(var session=this.OpenSession())
			using(var tran=session.BeginTransaction())
			{
				var product = new Product {Amount = 3, Price = 43.2};
				var product2 = new Product { Amount = 3, Price = 43.2 };
				session.Save(product);
				session.Save(product2);
				tran.Commit();
			}
		}
		protected override void OnTearDown()
		{
			using(var session=this.OpenSession())
			using (var tran = session.BeginTransaction())
			{
				session.Delete("from Product");
				tran.Commit();
			}
		}

		[Test]
		public void ReturnsApropriateTypeWhenSumUsedWithSomeFormula()
		{
			using (var session = this.OpenSession())
			using (var tran = session.BeginTransaction())
			{
			    double delta = 0.0000000000001;

				var query=session.CreateQuery("select sum(Amount*Price) from Product");
				var result=query.UniqueResult();
				Assert.That(result, Is.InstanceOf(typeof (double)));
                Assert.AreEqual(43.2 * 3 * 2, (double)result, delta);
				query = session.CreateQuery("select sum(Price*Amount) from Product");
				result = query.UniqueResult();
				Assert.That(result, Is.InstanceOf(typeof(double)));
                Assert.AreEqual(43.2 * 3 * 2, (double)result, delta);

				query = session.CreateQuery("select sum(Price) from Product");
				result = query.UniqueResult();
				Assert.That(result, Is.InstanceOf(typeof(double)));
                Assert.AreEqual(43.2 * 2, (double)result, delta);

				query = session.CreateQuery("select sum(Amount) from Product");
				result = query.UniqueResult();
				Assert.That(result, Is.InstanceOf(typeof(Int64)));
				Assert.That(result, Is.EqualTo(6));
			}
		}
	}
}
