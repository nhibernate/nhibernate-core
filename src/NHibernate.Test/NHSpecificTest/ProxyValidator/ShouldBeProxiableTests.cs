using System;
using System.Reflection;
using NHibernate.Proxy;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.ProxyValidator
{
	[TestFixture]
	public class ShouldBeProxiableTests
	{
		private class MyClass : IDisposable
		{
			public void Dispose()
			{
			}

			~MyClass()
			{
			}

			// ReSharper disable once InconsistentNaming
			// This is intentionally lower case
			public virtual void finalize()
			{
			}

			public virtual void Finalize(int a)
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
			Assert.That(method.ShouldBeProxiable(), Is.False);
		}

		[Test]
		public void DisposeNotBeProxiable()
		{
			var method = typeof(MyClass).GetMethod("Dispose");
			Assert.That(method.ShouldBeProxiable(), Is.False);
		}

		[Test]
		public void ObjectDestructorShouldNotBeProxiable()
		{
			var method = typeof(object).GetMethod(
				"Finalize",
				BindingFlags.NonPublic | BindingFlags.Instance);

			Assert.That(method.ShouldBeProxiable(), Is.False);
		}

		[Test]
		public void ObjectDestructorIsNotProxiable()
		{
			var method = typeof(object).GetMethod(
				"Finalize",
				BindingFlags.NonPublic | BindingFlags.Instance);

			Assert.That(method.IsProxiable(), Is.False);
		}
		
		[Test]
		public void MyClassDestructorShouldNotBeProxiable()
		{
			var method = typeof(MyClass).GetMethod(
				"Finalize",
				BindingFlags.NonPublic | BindingFlags.Instance,
				null,
				System.Type.EmptyTypes,
				null);

			Assert.That(method.ShouldBeProxiable(), Is.False);
		}

		[Test]
		public void MyClassDestructorIsNotProxiable()
		{
			var method = typeof(MyClass).GetMethod(
				"Finalize",
				BindingFlags.NonPublic | BindingFlags.Instance,
				null,
				System.Type.EmptyTypes,
				null);

			Assert.That(method.IsProxiable(), Is.False);
		}

		[Test]
		public void MyClassLowerCaseFinalizeShouldBeProxiable()
		{
			var method = typeof(MyClass).GetMethod(
				"finalize",
				BindingFlags.Public | BindingFlags.Instance,
				null,
				System.Type.EmptyTypes,
				null);

			Assert.That(method.ShouldBeProxiable(), Is.True);
		}

		[Test]
		public void MyClassLowerCaseFinalizeIsProxiable()
		{
			var method = typeof(MyClass).GetMethod(
				"finalize",
				BindingFlags.Public | BindingFlags.Instance,
				null,
				System.Type.EmptyTypes,
				null);

			Assert.That(method.IsProxiable(), Is.True);
		}
		
		[Test]
		public void MyClassFinalizeWithParametersShouldBeProxiable()
		{
			var method = typeof(MyClass).GetMethod(
				"Finalize",
				BindingFlags.Public | BindingFlags.Instance,
				null,
				new[] { typeof(int) },
				null);

			Assert.That(method.ShouldBeProxiable(), Is.True);
		}

		[Test]
		public void MyClassFinalizeWithParametersIsProxiable()
		{
			var method = typeof(MyClass).GetMethod(
				"Finalize",
				BindingFlags.Public | BindingFlags.Instance,
				null,
				new[] { typeof(int) },
				null);

			Assert.That(method.IsProxiable(), Is.True);
		}

		[Test]
		public void WhenProtectedNoVirtualPropertyThenShouldntBeProxiable()
		{
			var prop = typeof(ProtectedNoVirtualProperty).GetProperty("Aprop", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			Assert.That(prop.ShouldBeProxiable(), Is.False);			
		}

		[Test]
		public void WhenProtectedInternalNoVirtualPropertyThenShouldBeProxiable()
		{
			var prop = typeof(ProtectedNoVirtualProperty).GetProperty("AProtectedInternalProp", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			Assert.That(prop.ShouldBeProxiable(), Is.True);
		}

		[Test]
		public void WhenInternalNoVirtualPropertyThenShouldBeProxiable()
		{
			var prop = typeof(ProtectedNoVirtualProperty).GetProperty("AInternalProp", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			Assert.That(prop.ShouldBeProxiable(), Is.True);
		}

		[Test]
		public void WhenProtectedNoVirtualMethodThenShouldntBeProxiable()
		{
			var method = typeof(NoVirtualMethods).GetMethod("AProtected", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			Assert.That(method.ShouldBeProxiable(), Is.False);
		}

		[Test]
		public void WhenProtectedInternalNoVirtualMethodThenShouldBeProxiable()
		{
			var method = typeof(NoVirtualMethods).GetMethod("AProtectedInternal", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			Assert.That(method.ShouldBeProxiable(), Is.True);
		}

		[Test]
		public void WhenPrivateMethodThenShouldntBeProxiable()
		{
			var method = typeof(NoVirtualMethods).GetMethod("APrivate", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			Assert.That(method.ShouldBeProxiable(), Is.False);
		}

		[Test]
		public void WhenPublicMethodThenShouldBeProxiable()
		{
			var method = typeof(NoVirtualMethods).GetMethod("APublic", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			Assert.That(method.ShouldBeProxiable(), Is.True);
		}

		[Test]
		public void WhenInternalMethodThenShouldBeProxiable()
		{
			var method = typeof(NoVirtualMethods).GetMethod("AInternal", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			Assert.That(method.ShouldBeProxiable(), Is.True);
		}
	}
}
