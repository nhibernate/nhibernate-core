using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Criterion;
using NUnit.Framework;
using System.Collections;
namespace NHibernate.Test.NHSpecificTest.NH1033
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			using(var session=OpenSession())
			{
				using (var tran = session.BeginTransaction())
				{

					var animal0 = new Animal();
					var animal1 = new Reptile();

					animal0.SerialNumber = "00001";

					animal1.SerialNumber = "00002";
					animal1.BodyTemperature = 34;

					session.Save(animal0);
					session.Save(animal1);
					tran.Commit();
				}
			}
		}
		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var tran = session.BeginTransaction())
			{
				session.Delete("from Animal");
				session.Delete("from Reptile");
				tran.Commit();
			}
		}

		[Test]
		public void CanUseClassConstraint()
		{
			using(ISession session=OpenSession())
			{
				var crit = session
					.CreateCriteria(typeof (Animal), "a")
					.Add(Property
					     	.ForName("a.class")
							.Eq(typeof(Animal)));
				var results = crit.List<Animal>();
				Assert.AreEqual(1,results.Count);
				Assert.AreEqual(typeof(Animal), NHibernateUtil.GetClass(results[0]));
			}
		}
	}
}
