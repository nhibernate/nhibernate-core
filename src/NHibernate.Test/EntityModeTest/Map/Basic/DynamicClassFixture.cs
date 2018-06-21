using System.Collections;
using System.Collections.Generic;
using Antlr.Runtime.Misc;
using NUnit.Framework;
using NHibernate.Criterion;

namespace NHibernate.Test.EntityModeTest.Map.Basic
{
	[TestFixture]
	public class DynamicClassFixture : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get { return new[] {"EntityModeTest.Map.Basic.ProductLine.hbm.xml"}; }
		}

		public delegate IDictionary SingleCarQueryDelegate(ISession session);
		public delegate IList AllModelQueryDelegate(ISession session);

		[Test]
		public void ShouldWorkWithHQL()
		{
			TestLazyDynamicClass(
				s => (IDictionary) s.CreateQuery("from ProductLine pl order by pl.Description").UniqueResult(),
				s => s.CreateQuery("from Model m").List());
		}

		[Test]
		public void ShouldWorkWithCriteria()
		{
			TestLazyDynamicClass(
				s => (IDictionary) s.CreateCriteria("ProductLine").AddOrder(Order.Asc("Description")).UniqueResult(),
				s => s.CreateCriteria("Model").List());
		}

		public void TestLazyDynamicClass(SingleCarQueryDelegate singleCarQueryHandler, AllModelQueryDelegate allModelQueryHandler)
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

				s.Save("ProductLine", cars);
				t.Commit();
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

				t.Commit();
			}

			using (ISession s = OpenSession())
			{
				t = s.BeginTransaction();
				cars = singleCarQueryHandler(s);
				s.Delete(cars);
				t.Commit();
			}
		}

		[Test]
		public void ShouldWorkWithHQLAndGenerics()
		{
			TestLazyDynamicClass(
				s => s.CreateQuery("from ProductLine pl order by pl.Description").UniqueResult<IDictionary<string, object>>(),
				s => s.CreateQuery("from Model m").List<IDictionary<string, object>>());
		}

		[Test]
		public void ShouldWorkWithCriteriaAndGenerics()
		{
			TestLazyDynamicClass(
				s => s.CreateCriteria("ProductLine").AddOrder(Order.Asc("Description")).UniqueResult<IDictionary<string, object>>(),
				s => s.CreateCriteria("Model").List<IDictionary<string, object>>());
		}

		public void TestLazyDynamicClass(
			Func<ISession, IDictionary<string, object>> singleCarQueryHandler,
			Func<ISession, IList<IDictionary<string, object>>> allModelQueryHandler)
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var cars = new Dictionary<string, object> { ["Description"] = "Cars" };

				var monaro = new Dictionary<string, object>
				{
					["ProductLine"] = cars,
					["Name"] = "Monaro",
					["Description"] = "Holden Monaro"
				};

				var hsv = new Dictionary<string, object>
				{
					["ProductLine"] = cars,
					["Name"] = "hsv",
					["Description"] = "Holden hsv"
				};

				var models = new List<IDictionary<string, object>> {monaro, hsv};

				cars["Models"] = models;

				s.Save("ProductLine", cars);
				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var cars = singleCarQueryHandler(s);
				var models = (IList<object>) cars["Models"];
				Assert.That(NHibernateUtil.IsInitialized(models), Is.False);
				Assert.That(models.Count, Is.EqualTo(2));
				Assert.That(NHibernateUtil.IsInitialized(models), Is.True);
				s.Clear();
				var list = allModelQueryHandler(s);
				foreach (var dic in list)
				{
					Assert.That(NHibernateUtil.IsInitialized(dic["ProductLine"]), Is.False);
				}
				var model = list[0];
				Assert.That(((IList<object>) ((IDictionary<string, object>) model["ProductLine"])["Models"]).Contains(model), Is.True);
				s.Clear();

				t.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.Delete("from ProductLine");
				t.Commit();
			}
		}
	}
}
