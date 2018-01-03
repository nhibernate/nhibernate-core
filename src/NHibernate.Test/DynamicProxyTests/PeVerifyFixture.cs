using System;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using NUnit.Framework;
using NHibernate.Proxy.DynamicProxy;

namespace NHibernate.Test.DynamicProxyTests
{
	[TestFixture]
	public class PeVerifyFixture
	{
		private static bool wasCalled;

		private const string assemblyName = "peVerifyAssembly";
		private const string assemblyFileName = "peVerifyAssembly.dll";

		[Test]
#if NETCOREAPP2_0
		[Ignore("This platform does not support saving dynamic assemblies.")]
#endif
		public void VerifyClassWithPublicConstructor()
		{
			var factory = new ProxyFactory(new SavingProxyAssemblyBuilder(assemblyName));
			var proxyType = factory.CreateProxyType(typeof(ClassWithPublicDefaultConstructor), null);

			wasCalled = false;
			Activator.CreateInstance(proxyType);

			Assert.That(wasCalled);
			new PeVerifier(assemblyFileName).AssertIsValid();
		}

		[Test]
#if NETCOREAPP2_0
		[Ignore("This platform does not support saving dynamic assemblies.")]
#endif
		public void VerifyClassWithProtectedConstructor()
		{
			var factory = new ProxyFactory(new SavingProxyAssemblyBuilder(assemblyName));
			var proxyType = factory.CreateProxyType(typeof(ClassWithProtectedDefaultConstructor), null);

			wasCalled = false;
			Activator.CreateInstance(proxyType);

			Assert.That(wasCalled);
			new PeVerifier(assemblyFileName).AssertIsValid();
		}

		[Test]
#if NETCOREAPP2_0
		[Ignore("This platform does not support saving dynamic assemblies.")]
#endif
		public void VerifyClassWithPrivateConstructor()
		{
			var factory = new ProxyFactory(new SavingProxyAssemblyBuilder(assemblyName));
			var proxyType = factory.CreateProxyType(typeof(ClassWithPrivateDefaultConstructor), null);

			wasCalled = false;
			Activator.CreateInstance(proxyType);

			Assert.That(!wasCalled); // System.Object constructor called - works, but fails PeVerify
		}

		[Test]
#if NETCOREAPP2_0
		[Ignore("This platform does not support saving dynamic assemblies.")]
#endif
		public void VerifyClassWithNoDefaultConstructor()
		{
			var factory = new ProxyFactory(new SavingProxyAssemblyBuilder(assemblyName));
			var proxyType = factory.CreateProxyType(typeof(ClassWithNoDefaultConstructor), null);

			wasCalled = false;
			Activator.CreateInstance(proxyType);

			Assert.That(!wasCalled); // System.Object constructor called - works, but fails PeVerify
		}

		[Test]
#if NETCOREAPP2_0
		[Ignore("This platform does not support saving dynamic assemblies.")]
#endif
		public void VerifyClassWithInternalConstructor()
		{
			var factory = new ProxyFactory(new SavingProxyAssemblyBuilder(assemblyName));
			var proxyType = factory.CreateProxyType(typeof(ClassWithInternalConstructor), null);

			wasCalled = false;
			Activator.CreateInstance(proxyType);

			Assert.That(!wasCalled); // System.Object constructor called - works, but fails PeVerify
		}

		#region PeVerifyTypes

		public class ClassWithPublicDefaultConstructor
		{
			public ClassWithPublicDefaultConstructor() { InitG<int>(1); }
			public ClassWithPublicDefaultConstructor(int unused) { }
			public virtual int Prop1 { get; set; }
			public virtual void InitG<T>(T value) { Init((int)(object)value); }
			public virtual void Init(int value) { Prop1 = value; if (Prop1 == 1) wasCalled = true; }
		}

		public class ClassWithProtectedDefaultConstructor
		{
			protected ClassWithProtectedDefaultConstructor() { wasCalled = true; }
		}

		public class ClassWithPrivateDefaultConstructor
		{
			private ClassWithPrivateDefaultConstructor() { wasCalled = true; }
		}

		public class ClassWithNoDefaultConstructor
		{
			public ClassWithNoDefaultConstructor(int unused) { wasCalled = true; }
			public ClassWithNoDefaultConstructor(string unused) { wasCalled = true; }
		}

		public class ClassWithInternalConstructor
		{
			internal ClassWithInternalConstructor() { wasCalled = true; }
		}

		#endregion

		#region ProxyFactory.IProxyAssemblyBuilder

		public class SavingProxyAssemblyBuilder : IProxyAssemblyBuilder
		{
			private string assemblyName;

			public SavingProxyAssemblyBuilder(string assemblyName)
			{
				this.assemblyName = assemblyName;
			}

			public AssemblyBuilder DefineDynamicAssembly(AppDomain appDomain, AssemblyName name)
			{
#if NETCOREAPP2_0
				throw new NotSupportedException("AppDomain.DefineDynamicModule not supported on this platform.");
#else
				AssemblyBuilderAccess access = AssemblyBuilderAccess.RunAndSave;
				return appDomain.DefineDynamicAssembly(new AssemblyName(assemblyName), access, TestContext.CurrentContext.TestDirectory);
#endif
			}

			public ModuleBuilder DefineDynamicModule(AssemblyBuilder assemblyBuilder, string moduleName)
			{
#if NETCOREAPP2_0
				throw new NotSupportedException("AssemblyBuilder.DefineDynamicModule not supported on this platform.");
#else
				return assemblyBuilder.DefineDynamicModule(moduleName, string.Format("{0}.mod", assemblyName), true);
#endif
			}

			public void Save(AssemblyBuilder assemblyBuilder)
			{
#if NETCOREAPP2_0
				throw new NotSupportedException("AssemblyBuilder.Save not supported on this platform.");
#else
				assemblyBuilder.Save(assemblyName + ".dll");
#endif
			}
		}

		#endregion
	}
}
