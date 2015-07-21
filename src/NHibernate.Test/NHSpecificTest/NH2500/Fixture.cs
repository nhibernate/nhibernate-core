using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NHibernate.Cfg.MappingSchema;
using NHibernate.Linq;
using NHibernate.Mapping.ByCode;

using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2500
{
    public class Foo
    {
        public virtual Guid Id { get; set; }

        public virtual string Name { get; set; }
    }


    [TestFixture]
    public class Fixture : TestCaseMappingByCode
    {
        protected override HbmMapping GetMappings()
        {
            var mapper = new ConventionModelMapper();
            mapper.BeforeMapClass += (mi, t, x) => x.Id(map => map.Generator(Generators.Guid));
            return mapper.CompileMappingFor(new[] { typeof(Foo) });
        }

        protected override void OnSetUp()
        {
            base.OnSetUp();

            using (ISession session = Sfi.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                session.Persist(new Foo { Name = "Banana" });
                session.Persist(new Foo { Name = "Egg" });
                transaction.Commit();
            }
        }

        protected override void OnTearDown()
        {
            using (ISession session = Sfi.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                session.CreateQuery("delete from Foo").ExecuteUpdate();
                transaction.Commit();
            }

            base.OnTearDown();
        }

		private int count;
		
		[Test]
        public void TestLinqProjectionExpressionDoesntCacheParameters()
        {
            using (ISession session = Sfi.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
            	this.count = 1;

            	var foos1 = session.Query<Foo>()
            		.Where(x => x.Name == "Banana")
            		.Select(x => new
            		{
            			x.Name,
            			count,
            			User = "abc"
            		}).First();

            	this.count = 2;

            	var foos2 = session.Query<Foo>()
            		.Where(x => x.Name == "Egg")
            		.Select(x => new
            		{
            			x.Name,
            			count,
            			User = "def"
            		}).First();

				Assert.AreEqual(1, foos1.count);
				Assert.AreEqual(2, foos2.count);
				Assert.AreEqual("abc", foos1.User);
				Assert.AreEqual("def", foos2.User);

                transaction.Commit();
            }
        }
    }
}
