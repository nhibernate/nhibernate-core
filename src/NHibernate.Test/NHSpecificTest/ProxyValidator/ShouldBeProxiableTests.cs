using System;
using System.Reflection;
using NHibernate.Proxy;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.NHSpecificTest.ProxyValidator
{
	public class ShouldBeProxiableTests
	{
		private class MyClass: IDisposable
		{
			public void Dispose()
			{
				
			}
		}
		private class ProtectedNoVirtualProperty
		{
			protected int Aprop { get; set; }
			protected internal int AProtectedInternalProp { get; set; }
			internal int AInternalProp { get; set; }
		}

		private class NoVirtualMethods
		{
			protected void AProtected() {  }
			private void APrivate() { }
			public void APublic() { }
			internal void AInternal() { }
			protected internal void AProtectedInternal() { }
		}

		[Test]
		public void GetTypeNotBeProxiable()
		{
			var method = typeof(object).GetMethod("GetType");
			method.ShouldBeProxiable().Should().Be.False();
		}

		[Test]
		public void DisposeNotBeProxiable()
		{
			var method = typeof(MyClass).GetMethod("Dispose");
			method.ShouldBeProxiable().Should().Be.False();
		}

		[Test]
		public void WhenProtectedNoVirtualPropertyThenShouldntBeProxiable()
		{
			var prop = typeof(ProtectedNoVirtualProperty).GetProperty("Aprop", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			prop.ShouldBeProxiable().Should().Be.False();			
		}

		[Test]
		public void WhenProtectedInternalNoVirtualPropertyThenShouldBeProxiable()
		{
			var prop = typeof(ProtectedNoVirtualProperty).GetProperty("AProtectedInternalProp", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			prop.ShouldBeProxiable().Should().Be.True();
		}

		[Test]
		public void WhenInternalNoVirtualPropertyThenShouldBeProxiable()
		{
			var prop = typeof(ProtectedNoVirtualProperty).GetProperty("AInternalProp", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			prop.ShouldBeProxiable().Should().Be.True();
		}

		[Test]
		public void WhenProtectedNoVirtualMethodThenShouldntBeProxiable()
		{
			var method = typeof(NoVirtualMethods).GetMethod("AProtected", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			method.ShouldBeProxiable().Should().Be.False();
		}

		[Test]
		public void WhenProtectedInternalNoVirtualMethodThenShouldBeProxiable()
		{
			var method = typeof(NoVirtualMethods).GetMethod("AProtectedInternal", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			method.ShouldBeProxiable().Should().Be.True();
		}

		[Test]
		public void WhenPrivateMethodThenShouldntBeProxiable()
		{
			var method = typeof(NoVirtualMethods).GetMethod("APrivate", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			method.ShouldBeProxiable().Should().Be.False();
		}

		[Test]
		public void WhenPublicMethodThenShouldBeProxiable()
		{
			var method = typeof(NoVirtualMethods).GetMethod("APublic", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			method.ShouldBeProxiable().Should().Be.True();
		}

		[Test]
		public void WhenInternalMethodThenShouldBeProxiable()
		{
			var method = typeof(NoVirtualMethods).GetMethod("AInternal", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			method.ShouldBeProxiable().Should().Be.True();
		}
	}
}