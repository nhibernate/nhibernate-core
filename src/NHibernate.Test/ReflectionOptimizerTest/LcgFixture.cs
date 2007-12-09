using System;
using NHibernate.Bytecode.Lightweight;
using NHibernate.Property;
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
		
		[Test, ExpectedException(typeof(PropertyNotFoundException))]
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

			new ReflectionOptimizer(typeof (NoSetterClass), getters, setters);
		}

		public class NoGetterClass
		{
			public int Property
			{
				set { }
			}
		}
		
		[Test, ExpectedException(typeof(PropertyNotFoundException))]
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

			new ReflectionOptimizer(typeof (NoGetterClass), getters, setters);
		}
	}
}
