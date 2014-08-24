using System;
using NHibernate.Bytecode;
using NUnit.Framework;

namespace NHibernate.Test.Bytecode
{
	[TestFixture]
	public class ActivatorObjectFactoryFixture
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

		public struct ValueType
		{
			public string Something { get; set; }
		}

		protected virtual IObjectsFactory GetObjectsFactory()
		{
			return new ActivatorObjectsFactory();
		}

		[Test]
		public void CreateInstanceDefCtor()
		{
			IObjectsFactory of = GetObjectsFactory();
			Assert.Throws<ArgumentNullException>(() => of.CreateInstance(null));
			Assert.Throws<MissingMethodException>(() => of.CreateInstance(typeof(WithOutPublicParameterLessCtor)));
			var instance = of.CreateInstance(typeof(PublicParameterLessCtor));
			Assert.That(instance, Is.Not.Null);
			Assert.That(instance, Is.InstanceOf<PublicParameterLessCtor>());
		}



		[Test]
		public void CreateInstanceWithNoPublicCtor()
		{
			IObjectsFactory of = GetObjectsFactory();
			Assert.Throws<ArgumentNullException>(() => of.CreateInstance(null, false));
			var instance = of.CreateInstance(typeof(WithOutPublicParameterLessCtor), true);
			Assert.That(instance, Is.Not.Null);
			Assert.That(instance, Is.InstanceOf<WithOutPublicParameterLessCtor>());
		}

		[Test]
		public void CreateInstanceOfValueType()
		{
			IObjectsFactory of = GetObjectsFactory();
			var instance = of.CreateInstance(typeof(ValueType), true);
			Assert.That(instance, Is.Not.Null);
			Assert.That(instance, Is.InstanceOf<ValueType>());
		}

		[Test]
		public void CreateInstanceWithArguments()
		{
			IObjectsFactory of = GetObjectsFactory();
			Assert.Throws<ArgumentNullException>(() => of.CreateInstance(null, new[] {1}));
			var value = "a value";
			var instance = of.CreateInstance(typeof(WithOutPublicParameterLessCtor), new[]{value});
			Assert.That(instance, Is.Not.Null);
			Assert.That(instance, Is.InstanceOf<WithOutPublicParameterLessCtor>());
			Assert.That(((WithOutPublicParameterLessCtor)instance).Something, Is.EqualTo(value));
		}
	}
}