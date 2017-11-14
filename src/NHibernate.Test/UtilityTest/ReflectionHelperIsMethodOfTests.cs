using System;
using System.Collections.Generic;
using System.Reflection;
using NHibernate.Linq;
using NHibernate.Util;
using NUnit.Framework;

namespace NHibernate.Test.UtilityTest
{
	[TestFixture]
	public class ReflectionHelperIsMethodOfTests
	{
		[Test]
		public void WhenNullMethodInfoThenThrows()
		{
			Assert.Throws<ArgumentNullException>(() => ((MethodInfo) null).IsMethodOf(typeof (Object)));
		}

		[Test]
		public void WhenNullTypeThenThrows()
		{
			var methodInfo = ReflectHelper.GetMethodDefinition<List<int>>(t => t.Contains(5));
			Assert.Throws<ArgumentNullException>(() => methodInfo.IsMethodOf(null));
		}

		[Test]
		public void WhenDeclaringTypeMatchThenTrue()
		{
			Assert.That(ReflectHelper.GetMethodDefinition<List<int>>(t => t.Contains(5)).IsMethodOf(typeof(List<int>)), Is.True);
		}

		private class MyCollection: List<int>
		{
			
		}

		[Test]
		public void WhenCustomTypeMatchThenTrue()
		{
			Assert.That(ReflectHelper.GetMethodDefinition<MyCollection>(t => t.Contains(5)).IsMethodOf(typeof(List<int>)), Is.True);
		}

		[Test]
		public void WhenTypeIsGenericDefinitionAndMatchThenTrue()
		{
			Assert.That(ReflectHelper.GetMethodDefinition<List<int>>(t => t.Contains(5)).IsMethodOf(typeof(List<>)), Is.True);
		}

		[Test]
		public void WhenTypeIsGenericImplementedInterfaceAndMatchThenTrue()
		{
			var containsMethodDefinition = ReflectHelper.GetMethodDefinition<List<int>>(t => t.Contains(5));
			Assert.That(containsMethodDefinition.IsMethodOf(typeof(ICollection<int>)), Is.True);
		}

		[Test]
		public void WhenTypeIsGenericImplementedInterfaceAndMatchGenericInterfaceDefinitionThenTrue()
		{
			var containsMethodDefinition = ReflectHelper.GetMethodDefinition<List<int>>(t => t.Contains(5));
			Assert.That(containsMethodDefinition.IsMethodOf(typeof(ICollection<>)), Is.True);
		}

		[Test]
		public void WhenNoMatchThenFalse()
		{
			Assert.That(ReflectHelper.GetMethodDefinition<List<int>>(t => t.Contains(5)).IsMethodOf(typeof(IEnumerable<>)), Is.False);
		}

		private abstract class MyAbstractClass<T>
		{
			public abstract T MyMethod();
		}

		private class MyClass : MyAbstractClass<int>
		{
			public override int MyMethod() {return 0;}
		}

		[Test]
		public void WhenTypeIsGenericImplementedAbstractAndMatchThenTrue()
		{
			var containsMethodDefinition = ReflectHelper.GetMethodDefinition<MyClass>(t => t.MyMethod());
			Assert.That(containsMethodDefinition.IsMethodOf(typeof(MyAbstractClass<int>)), Is.True);
		}

		[Test]
		public void WhenTypeIsGenericImplementedAbstractAndMatchGenericInterfaceDefinitionThenTrue()
		{
			var containsMethodDefinition = ReflectHelper.GetMethodDefinition<MyClass>(t => t.MyMethod());
			Assert.That(containsMethodDefinition.IsMethodOf(typeof(MyAbstractClass<>)), Is.True);
		}
	}
}
