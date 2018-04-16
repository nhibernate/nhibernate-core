﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System.Collections;
using System.Collections.Generic;
using NHibernate.Cfg;
using NHibernate.Engine;
using NUnit.Framework;
using NHibernate.Criterion;

namespace NHibernate.Test.EntityModeTest.Map.Basic
{
	using System.Threading.Tasks;
	using System.Threading;
	[TestFixture]
	public class DynamicClassFixtureAsync : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get { return new string[] {"EntityModeTest.Map.Basic.ProductLine.hbm.xml"}; }
		}

		public delegate IDictionary SingleCarQueryDelegate(ISession session);
		public delegate IList AllModelQueryDelegate(ISession session);

		[Test]
		public Task ShouldWorkWithHQLAsync()
		{
			return TestLazyDynamicClassAsync(
				s => (IDictionary) s.CreateQuery("from ProductLine pl order by pl.Description").UniqueResult(),
				s => s.CreateQuery("from Model m").List());
		}

		[Test]
		public Task ShouldWorkWithCriteriaAsync()
		{
			return TestLazyDynamicClassAsync(
				s => (IDictionary) s.CreateCriteria("ProductLine").AddOrder(Order.Asc("Description")).UniqueResult(),
				s => s.CreateCriteria("Model").List());
		}

		public async Task TestLazyDynamicClassAsync(SingleCarQueryDelegate singleCarQueryHandler, AllModelQueryDelegate allModelQueryHandler, CancellationToken cancellationToken = default(CancellationToken))
		{
			ITransaction t;
			IDictionary cars;
			IList models;
			using (ISession s = OpenSession())
			{
				t = s.BeginTransaction();

				cars = new Hashtable();
				cars["Description"] = "Cars";

				IDictionary monaro = new Hashtable();
				monaro["ProductLine"] = cars;
				monaro["Name"] = "Monaro";
				monaro["Description"] = "Holden Monaro";

				IDictionary hsv = new Hashtable();
				hsv["ProductLine"] = cars;
				hsv["Name"] = "hsv";
				hsv["Description"] = "Holden hsv";

				models = new List<IDictionary> {monaro, hsv};

				cars["Models"] = models;

				await (s.SaveAsync("ProductLine", cars, cancellationToken));
				await (t.CommitAsync(cancellationToken));
			}

			using (ISession s = OpenSession())
			{
				t = s.BeginTransaction();
				cars = singleCarQueryHandler(s);
				models = (IList)cars["Models"];
				Assert.IsFalse(NHibernateUtil.IsInitialized(models));
				Assert.AreEqual(2, models.Count);
				Assert.IsTrue(NHibernateUtil.IsInitialized(models));
				s.Clear();
				IList list = allModelQueryHandler(s);
				foreach (IDictionary ht in list)
				{
					Assert.IsFalse(NHibernateUtil.IsInitialized(ht["ProductLine"]));
				}
				var model = (IDictionary)list[0];
				Assert.IsTrue(((IList)((IDictionary)model["ProductLine"])["Models"]).Contains(model));
				s.Clear();

				await (t.CommitAsync(cancellationToken));
			}

			using (ISession s = OpenSession())
			{
				t = s.BeginTransaction();
				cars = singleCarQueryHandler(s);
				await (s.DeleteAsync(cars, cancellationToken));
				await (t.CommitAsync(cancellationToken));
			}
		}
	}
}