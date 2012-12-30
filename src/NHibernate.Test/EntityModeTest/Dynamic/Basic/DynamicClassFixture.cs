using System.Collections;
using System.Collections.Generic;
using NHibernate.Cfg;
using NHibernate.Engine;
using NUnit.Framework;
using NHibernate.Criterion;
using System.Dynamic;

namespace NHibernate.Test.EntityModeTest.Dynamic.Basic
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
            get { return new string[] { "EntityModeTest.Dynamic.Basic.ProductLine.hbm.xml" }; }
        }

        protected override void Configure(Configuration configuration)
        {
            configuration.SetProperty(Environment.DefaultEntityMode, EntityModeHelper.ToString(EntityMode.Dynamic));
        }

        public delegate IDynamicMetaObjectProvider SingleCarQueryDelegate(ISession session);
        public delegate IList AllModelQueryDelegate(ISession session);

        [Test]
        public void ShouldWorkWithHQL()
        {
            TestLazyDynamicClass(s => (IDynamicMetaObjectProvider)s.CreateQuery("from ProductLine pl order by pl.Description").UniqueResult(),
                                 s => s.CreateQuery("from Model m  left join fetch m.ProductLine").List());
        }

        [Test]
        public void ShouldWorkWithCriteria()
        {
            TestLazyDynamicClass(
                s => (IDynamicMetaObjectProvider)s.CreateCriteria("ProductLine").AddOrder(Order.Asc("Description")).UniqueResult(),
                s => s.CreateCriteria("Model").List());
        }

        public void TestLazyDynamicClass(SingleCarQueryDelegate singleCarQueryHandler, AllModelQueryDelegate allModelQueryHandler)
        {
            ITransaction t;
            using (ISession s = OpenSession())
            {
                var si = (ISessionImplementor)s;
                Assert.IsTrue(si.EntityMode == EntityMode.Dynamic, "Incorrectly handled default_entity_mode");
                ISession other = s.GetSession(EntityMode.Poco);
                other.Close();
                Assert.IsFalse(other.IsOpen);
            }
            dynamic cars;
            IList models;
            using (ISession s = OpenSession())
            {
                t = s.BeginTransaction();

                cars = new ExpandoObject();
                cars.Description = "Cars";

                dynamic monaro = new ExpandoObject();
                monaro.ProductLine = cars;
                monaro.Name = "Monaro";
                monaro.Description = "Holden Monaro";

                dynamic hsv = new ExpandoObject();
                hsv.ProductLine = cars;
                hsv.Name = "hsv";
                hsv.Description = "Holden hsv";

                models = new List<IDynamicMetaObjectProvider> { monaro, hsv };

                cars.Models = models;

                s.Save("ProductLine", cars);
                t.Commit();
            }

            using (ISession s = OpenSession())
            {
                t = s.BeginTransaction();
                cars = singleCarQueryHandler(s);
                models = cars.Models;
                //Assert.IsFalse(NHibernateUtil.IsInitialized(models));
                //Assert.AreEqual(2, models.Count);
                //Assert.IsTrue(NHibernateUtil.IsInitialized(models));
                s.Clear();
                IList list = allModelQueryHandler(s);
                //foreach (dynamic ht in list)
                //{
                //    Assert.IsFalse(NHibernateUtil.IsInitialized(ht.ProductLine));
                //}
                dynamic model = list[0];
                Assert.IsTrue(model.ProductLine.Models.Contains(model));
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