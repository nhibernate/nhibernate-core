using System;
using NHibernate.Bytecode;
using NUnit.Framework;

namespace NHibernate.Test.Bytecode
{
	[TestFixture]
	public class ActivatorServiceProviderFixture
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
			return new ActivatorServiceProvider();
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
	}
}
