using NHibernate.Proxy.DynamicProxy;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2862
{
    [TestFixture]
    public class ProxyTest
    {
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

    #endregion

}