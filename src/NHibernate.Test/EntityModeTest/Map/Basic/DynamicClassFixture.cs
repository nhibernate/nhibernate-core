using System.Collections;
using System.Collections.Generic;
using NHibernate.Cfg;
using NHibernate.Engine;
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
			get { return new string[] {"EntityModeTest.Map.Basic.ProductLine.hbm.xml"}; }
		}

		protected override void Configure(Configuration configuration)
		{
			configuration.SetProperty(Environment.DefaultEntityMode, EntityModeHelper.ToString(EntityMode.Map));
		}

		public delegate IDictionary SingleCarQueryDelegate(ISession session);
		public delegate IList AllModelQueryDelegate(ISession session);

		[Test]
		public void ShouldWorkWithHQL()
		{
			TestLazyDynamicClass(s => (IDictionary) s.CreateQuery("from ProductLine pl order by pl.Description").UniqueResult(),
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
			using (ISession s = OpenSession())
			{
				var si = (ISessionImplementor)s;
				Assert.IsTrue(si.EntityMode == EntityMode.Map, "Incorrectly handled default_entity_mode");
				ISession other = s.GetSession(EntityMode.Poco);
				other.Close();
				Assert.IsFalse(other.IsOpen);
			}
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
	}
}