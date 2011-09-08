using NHibernate.Proxy;
using NHibernate.Proxy.DynamicProxy;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2862
{
    [TestFixture]
    public class ProxyTest
    {
        #region proxy tests

        [Test]
        public void ProxyCallsParameterlessConstructorOfImmediateParentClass()
        {
            var pf = new ProxyFactory();
            var proxy = pf.CreateProxy(typeof(Entity1), new Interceptor(), new[] { typeof(Proxy.INHibernateProxy) }) as Entity1;
            
            Assume.That(proxy, Is.Not.Null);
            Assert.That(proxy.BaseMember, Is.Not.Null);
        }

        [Test]
        public void ProxyCallsAllBaseParameterlessConstructorsOfProxiedObject()
        {
            var pf = new ProxyFactory();
            var proxy = pf.CreateProxy(typeof(Entity2), new Interceptor(), new[] { typeof(Proxy.INHibernateProxy) }) as Entity2;

            Assume.That(proxy, Is.Not.Null);
            Assert.That(proxy.BaseMember, Is.Not.Null);
            Assert.That(proxy.DerivedMember, Is.Not.Null);
        }

        [Test]
        public void ProxyCallsBaseParameterlessConstructorIfDerivedDoesNotExplicityDeclareOne()
        {
            var pf = new ProxyFactory();
            var proxy = pf.CreateProxy(typeof (EntityWithProtectedInternalConstructor), new Interceptor(), new[] {typeof (INHibernateProxy)}) as EntityWithProtectedInternalConstructor;

            Assume.That(proxy, Is.Not.Null);
            Assert.That(proxy.BaseMember, Is.Not.Null);

        }

        [Test]
        public void ProxyIsConstructedCallingMostSpecificSafeConstructor()
        {
            var pf = new ProxyFactory();
            var proxy = pf.CreateProxy(typeof(EntityWithPrivateConstructor), new Interceptor(), new[] { typeof(INHibernateProxy) }) as EntityWithPrivateConstructor;

            Assume.That(proxy, Is.Not.Null);
            Assert.That(proxy.BaseMember, Is.Not.Null);
        }

        #endregion


        #region Constructor search tests

        [Test]
        public void FindsBaseConstructorWhenTypeHasPrivateDefaultConstructor()
        {
            var type = typeof (EntityWithPrivateConstructor);
            var cs = ProxyFactory.GetMostSpecificSafeDefaultConstructorForType(type);

            Assert.That(cs.DeclaringType, Is.EqualTo(type.BaseType));
        }

        [Test]
        public void FindsConstructorWhenTypeHasProtectedDefaultConstructor()
        {
            var type = typeof (EntityWithProtectedConstructor);
            var cs = ProxyFactory.GetMostSpecificSafeDefaultConstructorForType(type);

            Assert.That(cs.DeclaringType, Is.EqualTo(type));
        }

        [Test]
        public void FindsConstructorWhenTypeHasProtectedInternalDefaultConstructor()
        {
            var type = typeof (EntityWithProtectedInternalConstructor);
            var cs = ProxyFactory.GetMostSpecificSafeDefaultConstructorForType(type);

            Assert.That(cs.DeclaringType, Is.EqualTo(type));
        }

        #endregion


    }

    class Interceptor : Proxy.DynamicProxy.IInterceptor
    {
        #region Implementation of IInterceptor

        public object Intercept(InvocationInfo info)
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }

    #region Helper classes

    public class EntityBaseWithDefaultConstructor
    {
        private readonly object _baseMember;

        public EntityBaseWithDefaultConstructor()
        {
            _baseMember = new object();
        }

        public object BaseMember
        {
            get { return _baseMember; }
        }
    }

    public class Entity1 : EntityBaseWithDefaultConstructor
    {

    }

    public class Entity2 : EntityBaseWithDefaultConstructor
    {
        private readonly object _derivedMember;

        public Entity2()
        {
            _derivedMember = new object();
        }

        public object DerivedMember
        {
            get { return _derivedMember; }
        }
    }

    public class EntityWithProtectedInternalConstructor : EntityBaseWithDefaultConstructor
    {
        protected internal EntityWithProtectedInternalConstructor(){}
    }

    public class EntityWithProtectedConstructor : EntityBaseWithDefaultConstructor
    {
        protected EntityWithProtectedConstructor() { }
    }

    public class EntityWithPrivateConstructor : EntityBaseWithDefaultConstructor
    {
        private EntityWithPrivateConstructor() {}
    }

    #endregion

}