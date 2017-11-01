using NHibernate.Proxy.DynamicProxy;
using NUnit.Framework;

namespace NHibernate.Test.DynamicProxyTests.ProxiedMembers
{
	public class ClassWithVarietyOfMembers
	{
		public virtual void Method1(out int x)
		{
			x = 3;
		}

		public virtual void Method2(ref int x)
		{
			x++;
		}
		
		public virtual void Method3(out int? y)
		{
			y = 4;
		}

		public virtual void Method4(ref int? y)
		{
			y++;
		}
	}

	[TestFixture]
	public class Fixture
	{
		[Test]
		public void Proxy()
		{
			var factory = new ProxyFactory();
			var c = (ClassWithVarietyOfMembers)factory.CreateProxy(typeof(ClassWithVarietyOfMembers), new PassThroughInterceptor(new ClassWithVarietyOfMembers()), null);

			int x;
			c.Method1(out x);
			Assert.AreEqual(3, x);

			x = 4;
			c.Method2(ref x);
			Assert.AreEqual(5, x);
			
			int? y;
			c.Method3(out y);
			Assert.AreEqual(4, y);
			
			c.Method4(ref y);
			Assert.AreEqual(5, y);
		}
	}
}
