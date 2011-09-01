using NHibernate.Bytecode.Lightweight;
using NHibernate.Cfg;
using NHibernate.DependencyInjection.Tests.Model;
using NHibernate.Proxy;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;

namespace NHibernate.DependencyInjection.Tests
{
    [TestFixture]
    public class When_using_custom_bytecode_provider
    {
        private readonly ISessionFactory _sessionFactory;

        public When_using_custom_bytecode_provider()
        {
            //register the bytecode provider with NHibernate
            //Initializer.RegisterBytecodeProvider(new EntityInjector());
            Environment.BytecodeProvider = new BytecodeProviderImpl(new EntityInjector());
            //configure NHibernate
            var config = new Configuration();
            config.AddClass(typeof(BasicCat));
            config.AddClass(typeof(InterfaceCat));
            config.AddClass(typeof(DependencyInjectionCat));
            //create the database
            var tool = new SchemaExport(config);
            tool.Execute(false, true, false);
            //build the session factory
            _sessionFactory = config.BuildSessionFactory();
        }

        [Test]
        public void Can_proxy_basic_cat()
        {
            //arrange
            var grannyCat = new BasicCat {Name = "Granny"};
            var mommyCat = new BasicCat { Name = "Mommy" };
            var babyCat = new BasicCat { Name = "Baby" };
            using (var session = _sessionFactory.OpenSession())
            {
                using (var t = session.BeginTransaction())
                {
                    session.SaveOrUpdate(grannyCat);
                    grannyCat.Kittens.Add(mommyCat);
                    mommyCat.Parent = grannyCat;
                    mommyCat.Kittens.Add(babyCat);
                    babyCat.Parent = mommyCat;
                    t.Commit();
                }
            }
            //act
            using (var session = _sessionFactory.OpenSession())
            {
                using (var t = session.BeginTransaction())
                {
                    babyCat = session.Get<BasicCat>(babyCat.Id);
                    //assert proxys created
                    Assert.IsTrue(babyCat.Parent.IsProxy());
                    Assert.IsTrue(babyCat.Parent.Parent.IsProxy());
                    //update the proxy cats
                    babyCat.Parent.Parent.Name = "Gramma";
                    var siblingCat = new BasicCat {Name = "Sibling"};
                    babyCat.Parent.Kittens.Add(siblingCat);
                    siblingCat.Parent = babyCat.Parent;
                    t.Commit();
                }
            }
            //assert
            using (var session = _sessionFactory.OpenSession())
            {
                babyCat = session.Get<BasicCat>(babyCat.Id);
                //assert proxys created
                Assert.IsTrue(babyCat.Parent.IsProxy());
                Assert.IsTrue(babyCat.Parent.Parent.IsProxy());
                //assert persistence via proxy
                Assert.AreEqual(2, babyCat.Parent.Kittens.Count);
                Assert.AreEqual("Gramma", babyCat.Parent.Parent.Name);
            }
        }

        [Test]
        public void Can_proxy_interface_cat()
        {
            //arrange
            var grannyCat = new InterfaceCat { Name = "Granny" };
            var mommyCat = new InterfaceCat { Name = "Mommy" };
            var babyCat = new InterfaceCat { Name = "Baby" };
            using (var session = _sessionFactory.OpenSession())
            {
                using (var t = session.BeginTransaction())
                {
                    session.SaveOrUpdate(grannyCat);
                    grannyCat.Kittens.Add(mommyCat);
                    mommyCat.Parent = grannyCat;
                    mommyCat.Kittens.Add(babyCat);
                    babyCat.Parent = mommyCat;
                    t.Commit();
                }
            }
            //act
            using (var session = _sessionFactory.OpenSession())
            {
                using (var t = session.BeginTransaction())
                {
                    babyCat = session.Get<InterfaceCat>(babyCat.Id);
                    //assert proxys created
                    Assert.IsTrue(babyCat.Parent.IsProxy());
                    Assert.IsTrue(babyCat.Parent.Parent.IsProxy());
                    //update the proxy cats
                    babyCat.Parent.Parent.Name = "Gramma";
                    var siblingCat = new InterfaceCat { Name = "Sibling" };
                    babyCat.Parent.Kittens.Add(siblingCat);
                    siblingCat.Parent = babyCat.Parent;
                    t.Commit();
                }
            }
            //assert
            using (var session = _sessionFactory.OpenSession())
            {
                babyCat = session.Get<InterfaceCat>(babyCat.Id);
                //assert proxys created
                Assert.IsTrue(babyCat.Parent.IsProxy());
                Assert.IsTrue(babyCat.Parent.Parent.IsProxy());
                //assert persistence via proxy
                Assert.AreEqual(2, babyCat.Parent.Kittens.Count);
                Assert.AreEqual("Gramma", babyCat.Parent.Parent.Name);
            }
        }

        [Test]
        public void Can_proxy_dependency_injection_cat()
        {
            //arrange
            var grannyCat = new DependencyInjectionCat(new CatBehavior()) { Name = "Granny" };
            var mommyCat = new DependencyInjectionCat(new CatBehavior()) { Name = "Mommy" };
            var babyCat = new DependencyInjectionCat(new CatBehavior()) { Name = "Baby" };
            using (var session = _sessionFactory.OpenSession())
            {
                using (var t = session.BeginTransaction())
                {
                    session.SaveOrUpdate(grannyCat);
                    grannyCat.Kittens.Add(mommyCat);
                    mommyCat.Parent = grannyCat;
                    mommyCat.Kittens.Add(babyCat);
                    babyCat.Parent = mommyCat;
                    t.Commit();
                }
            }
            //act
            using (var session = _sessionFactory.OpenSession())
            {
                using (var t = session.BeginTransaction())
                {
                    babyCat = session.Get<DependencyInjectionCat>(babyCat.Id);
                    //assert proxys created
                    Assert.IsTrue(babyCat.Parent.IsProxy());
                    Assert.IsTrue(babyCat.Parent.Parent.IsProxy());
                    //assert behavior is intact
                    Assert.AreEqual(new CatBehavior().Meow(), babyCat.Parent.Meow());
                    Assert.AreEqual(new CatBehavior().Meow(), babyCat.Parent.Parent.Meow());
                    //update the proxy cats
                    babyCat.Parent.Parent.Name = "Gramma";
                    var siblingCat = new DependencyInjectionCat(new CatBehavior()) { Name = "Sibling" };
                    babyCat.Parent.Kittens.Add(siblingCat);
                    siblingCat.Parent = babyCat.Parent;
                    t.Commit();
                }
            }
            //assert
            using (var session = _sessionFactory.OpenSession())
            {
                babyCat = session.Get<DependencyInjectionCat>(babyCat.Id);
                //assert proxys created
                Assert.IsTrue(babyCat.Parent.IsProxy());
                Assert.IsTrue(babyCat.Parent.Parent.IsProxy());
                //assert behavior is intact
                Assert.AreEqual(new CatBehavior().Meow(), babyCat.Parent.Meow());
                Assert.AreEqual(new CatBehavior().Meow(), babyCat.Parent.Parent.Meow());
                //assert persistence via proxy
                Assert.AreEqual(2, babyCat.Parent.Kittens.Count);
                Assert.AreEqual("Gramma", babyCat.Parent.Parent.Name);
            }
        }
    }
}
