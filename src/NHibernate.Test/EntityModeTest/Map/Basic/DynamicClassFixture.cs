using System.Collections;
using System.Collections.Generic;
using NHibernate.Cfg;
using NHibernate.Engine;
using NUnit.Framework;

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

		[Test]
		public void TestLazyDynamicClass()
		{
			ITransaction t;
			using(ISession s = OpenSession())
			{
				ISessionImplementor si = (ISessionImplementor) s;
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

				models = new List<IDictionary>();
				models.Add(monaro);
				models.Add(hsv);

				cars["Models"] = models;

				s.Save("ProductLine", cars);
				t.Commit();
			}

			using (ISession s = OpenSession())
			{
				t = s.BeginTransaction();
				cars = (IDictionary) s.CreateQuery("from ProductLine pl order by pl.Description").UniqueResult();
				models = (IList) cars["Models"];
				Assert.IsFalse(NHibernateUtil.IsInitialized(models));
				Assert.AreEqual(2, models.Count);
				Assert.IsTrue(NHibernateUtil.IsInitialized(models));
				s.Clear();
				IList list = s.CreateQuery("from Model m").List();
				foreach (IDictionary ht in list)
				{
					Assert.IsFalse(NHibernateUtil.IsInitialized(ht["ProductLine"]));
				}
				IDictionary model = (IDictionary) list[0];
				Assert.IsTrue(((IList) ((IDictionary) model["ProductLine"])["Models"]).Contains(model));
				s.Clear();

				t.Commit();
			}

			using (ISession s = OpenSession())
			{
				t = s.BeginTransaction();
				cars = (IDictionary) s.CreateQuery("from ProductLine pl order by pl.Description").UniqueResult();
				s.Delete(cars);
				t.Commit();
			}
		}
	}
}