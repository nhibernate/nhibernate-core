using System;
using NUnit.Framework;

namespace NHibernate.PostSharp.Proxies.Tests
{
    [TestFixture]
    public class ClassesAreWeavedCorrectly
    {
        private System.Type[] entities = new[] {typeof (Customer), typeof (Address)};

        [Test]
        public void EntitiesImplementsIPostSharpProxy()
        {
            foreach (var type in entities)
            {
                Assert.IsTrue(
                    Activator.CreateInstance(type) is IPostSharpNHibernateProxy
                    );
            }
        }

        [Test]
        public void CanSetAndGetNHibernateInitializer()
        {
            IPostSharpNHibernateProxy proxy = (IPostSharpNHibernateProxy) new Customer();
            var interceptor = new PostSharpInitializer(null, null, null, null, null, null);
            proxy.SetInterceptor(interceptor);
            Assert.AreSame(interceptor, proxy.HibernateLazyInitializer);
        }
    }
}