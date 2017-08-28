using System;
using NHibernate.Bytecode;
using NHibernate.Bytecode.Lightweight;
using NHibernate.Properties;
using NUnit.Framework;

namespace NHibernate.Test.ReflectionOptimizerTest
{
	[TestFixture]
	public class LcgFixture
	{
		public class NoSetterClass
		{
			public int Property
			{
				get { return 0; }
			}
		}
		
		[Test]
		public void NoSetter()
		{
			IGetter[] getters = new IGetter[]
				{
					new BasicPropertyAccessor.BasicGetter(typeof (NoSetterClass), typeof (NoSetterClass).GetProperty("Property"), "Property")
				};
			ISetter[] setters = new ISetter[]
				{
					new BasicPropertyAccessor.BasicSetter(typeof (NoSetterClass), typeof (NoSetterClass).GetProperty("Property"), "Property")
				};

			Assert.Throws<PropertyNotFoundException>(() => new ReflectionOptimizer(typeof(NoSetterClass), getters, setters));
		}

		public class NoGetterClass
		{
			public int Property
			{
				set { }
			}
		}
		
		[Test]
		public void NoGetter()
		{
			IGetter[] getters = new IGetter[]
				{
					new BasicPropertyAccessor.BasicGetter(typeof (NoGetterClass), typeof (NoGetterClass).GetProperty("Property"), "Property")
				};
			ISetter[] setters = new ISetter[]
				{
					new BasicPropertyAccessor.BasicSetter(typeof (NoGetterClass), typeof (NoGetterClass).GetProperty("Property"), "Property")
				};

			Assert.Throws<PropertyNotFoundException>(() => new ReflectionOptimizer(typeof (NoGetterClass), getters, setters));
		}

		public class GetterTypeMismatchClass
		{
			// Assigned by reflection
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
			object _property;
#pragma warning restore CS0649 // Field is never assigned to, and will always have its default value

			public string Property
			{
				get { return _property as string ?? "str"; }
			}
		}

		// Property and field may have different incompatible types
		// e.g. shadowed by a derived class to return a more specific type.
		// Make sure we handle this correctly.
		[Test]
		public void TestGetterTypeMismatch()
		{
			var obj = new GetterTypeMismatchClass();
			const string property = "Property";

			NoSetterAccessor accessor = new NoSetterAccessor(new CamelCaseUnderscoreStrategy());
			Assert.IsTrue(accessor.CanAccessThroughReflectionOptimizer);

			ReflectionOptimizer reflectionOptimizer = new ReflectionOptimizer(
				obj.GetType(),
				new[] { accessor.GetGetter(obj.GetType(), property) },
				new[] { accessor.GetSetter(obj.GetType(), property) });

			IAccessOptimizer accessOptimizer = reflectionOptimizer.AccessOptimizer;

			accessOptimizer.SetPropertyValues(obj, new object[] { 10 });
			object[] values = accessOptimizer.GetPropertyValues(obj);

			Assert.AreEqual("str", values[0]);
		}

	}
}
