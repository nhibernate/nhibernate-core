using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Intercept;
using NHibernate.Proxy;
using NHibernate.Test.DynamicProxyTests;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3500
{
    [Serializable]
    public class MyClass
    {
        public virtual int Id { get; set; }

        public virtual void ThrowError()
        {
            throw new Exception("test");
        }
    }

    [TestFixture]
    public class Fixture
    {
        [Test]
        public void DefaultDynamicLazyFieldInterceptorUnWrapsTIEExceptions()
        {
            var pf = new DefaultProxyFactory();
            var propertyInfo = typeof(MyClass).GetProperty("Id");
            pf.PostInstantiate("MyClass", typeof(MyClass), new HashSet<System.Type>(), propertyInfo.GetGetMethod(), propertyInfo.GetSetMethod(), null);
            var myClassProxied = (MyClass)pf.GetFieldInterceptionProxy(new MyClass());
            Assert.Throws<Exception>(() => myClassProxied.ThrowError(), "test");
        }
    }
}