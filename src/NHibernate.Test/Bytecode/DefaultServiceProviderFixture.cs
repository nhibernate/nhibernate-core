using System;
using NHibernate.Bytecode;
using NUnit.Framework;

namespace NHibernate.Test.Bytecode
{
	[TestFixture]
	public class DefaultServiceProviderFixture
	{
		public class WithOutPublicParameterLessCtor
		{
			public string Something { get; set; }
			protected WithOutPublicParameterLessCtor() { }

			public WithOutPublicParameterLessCtor(string something)
			{
				Something = something;
			}
		}

		public class PublicParameterLessCtor
		{
		}

		protected virtual IServiceProvider GetServiceProvider()
		{
			return new DefaultServiceProvider();
		}

		[Test]
		public void CreateInstanceDefCtor()
		{
			var sp = GetServiceProvider();
			Assert.Throws<ArgumentNullException>(() => sp.GetService(null));
			Assert.Throws<MissingMethodException>(() => sp.GetService(typeof(WithOutPublicParameterLessCtor)));
			var instance = sp.GetService(typeof(PublicParameterLessCtor));
			Assert.That(instance, Is.Not.Null);
			Assert.That(instance, Is.InstanceOf<PublicParameterLessCtor>());
		}


		[Test]
		public void RegisterService()
		{
			var sp = new DefaultServiceProvider();

			Assert.That(sp.GetService(typeof(IInterceptor)), Is.Null);

			sp.Register<IInterceptor, EmptyInterceptor>();
			var instance = sp.GetService(typeof(IInterceptor));
			Assert.That(instance, Is.Not.Null);
			Assert.That(instance, Is.InstanceOf<EmptyInterceptor>());

			Assert.Throws<InvalidOperationException>(() => sp.Register<IInterceptor, EmptyInterceptor>(), "service should not be registered twice.");
			Assert.Throws<InvalidOperationException>(() => sp.Register<Dialect.Dialect, Dialect.Dialect>(), "non concrete implementation type should not be permitted.");
			Assert.Throws<InvalidOperationException>(() => sp.Register(typeof(Dialect.Dialect), typeof(EmptyInterceptor)), "concrete implementation type should derive from service type.");
		}

		[Test]
		public void RegisterServiceCreator()
		{
			var sp = new DefaultServiceProvider();

			Assert.That(sp.GetService(typeof(IInterceptor)), Is.Null);

			sp.Register<IInterceptor>(() => new EmptyInterceptor());
			var instance = sp.GetService(typeof(IInterceptor));
			Assert.That(instance, Is.Not.Null);
			Assert.That(instance, Is.InstanceOf<EmptyInterceptor>());

			Assert.Throws<InvalidOperationException>(() => sp.Register<IInterceptor>(() => new EmptyInterceptor()), "service should not be registered twice.");
		}

	}
}
