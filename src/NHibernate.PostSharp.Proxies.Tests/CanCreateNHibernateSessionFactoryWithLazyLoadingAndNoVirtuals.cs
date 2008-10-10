using NHibernate.Cfg;
using NUnit.Framework;

namespace NHibernate.PostSharp.Proxies.Tests
{
    [TestFixture]
    public class CanCreateNHibernateSessionFactoryWithLazyLoadingAndNoVirtuals : TestCase
    {
        [Test]
        public void SessionFactoryCreatedSuccessfully()
        {
            Assert.IsNotNull(sessions);
        }

        [Test]
        public void ProxyFactoryFactoryIsSetProperly()
        {
            Assert.IsInstanceOfType(typeof (PostSharpProxyFactoryFactory),
                                    Environment.BytecodeProvider.ProxyFactoryFactory);
        }

        [Test]
        public void ProxyValidatorIsSetProperly()
        {
            Assert.IsInstanceOfType(typeof(PostSharpProxyValidator),
                Environment.BytecodeProvider.ProxyFactoryFactory.Validator);
        }
    }
}