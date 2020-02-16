﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Criterion;
using NUnit.Framework;
using System.Collections;
namespace NHibernate.Test.NHSpecificTest.NH1033
{
	using System.Threading.Tasks;
	[TestFixture]
	public class FixtureAsync : BugTestCase
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
		public async Task CanUseClassConstraintAsync()
		{
			using(ISession session=OpenSession())
			{
				var crit = session
					.CreateCriteria(typeof (Animal), "a")
					.Add(Property
					     	.ForName("a.class")
							.Eq(typeof(Animal)));
				var results = await (crit.ListAsync<Animal>());
				Assert.AreEqual(1,results.Count);
				Assert.AreEqual(typeof(Animal), await (NHibernateUtil.GetClassAsync(results[0])));
			}
		}
	}
}
