using NHibernate.Bytecode;
using NHibernate.Bytecode.Lightweight;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace NHibernate.Test.Bytecode.Lightweight
{
	[TestFixture]
	public class BytecodeProviderFixture
	{
		[Test]
		public void NotConfiguredProxyFactoryFactory()
		{
			try
			{
				var bcp = new BytecodeProviderImpl();
				IProxyFactoryFactory p = bcp.ProxyFactoryFactory;
				Assert.Fail();
			}
			catch (HibernateByteCodeException e)
			{
				Assert.That(e.Message, Text.StartsWith("The ProxyFactoryFactory was not configured"));
				Assert.That(e.Message, Text.Contains("Example"));
			}			
		}

		[Test]
		public void UnableToLoadProxyFactoryFactory()
		{
			try
			{
				var bcp = new BytecodeProviderImpl();
				bcp.SetProxyFactoryFactory("whatever");
				Assert.Fail();
			}
			catch (HibernateByteCodeException e)
			{
				Assert.That(e.Message, Text.StartsWith("Unable to load type"));
				Assert.That(e.Message, Text.Contains("Possible causes"));
				Assert.That(e.Message, Text.Contains("Confirm that your deployment folder contains"));
			}
		}

		[Test]
		public void DoesNotImplementProxyFactoryFactory()
		{
			try
			{
				var bcp = new BytecodeProviderImpl();
				bcp.SetProxyFactoryFactory(GetType().AssemblyQualifiedName);
				Assert.Fail();
			}
			catch (HibernateByteCodeException e)
			{
				Assert.That(e.Message,
										Is.EqualTo(GetType().FullName + " does not implement " + typeof(IProxyFactoryFactory).FullName));
			}
		}

		[Test]
		public void CantCreateProxyFactoryFactory()
		{
			try
			{
				var bcp = new BytecodeProviderImpl();
				bcp.SetProxyFactoryFactory(typeof(WrongProxyFactoryFactory).AssemblyQualifiedName);
				IProxyFactoryFactory p = bcp.ProxyFactoryFactory;
				Assert.Fail();
			}
			catch (HibernateByteCodeException e)
			{
				Assert.That(e.Message,Text.StartsWith("Failed to create an instance of"));
			}
		}
	}
}