using System;
using System.Collections.Generic;
using System.Reflection;
using NHibernate.Linq;
using NHibernate.Util;
using NUnit.Framework;

namespace NHibernate.Test.UtilityTest
{
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
			var methodInfo = ReflectionHelper.GetMethodDefinition<List<int>>(t => t.Contains(5));
			Assert.Throws<ArgumentNullException>(() => methodInfo.IsMethodOf(null));
		}

		[Test]
		public void WhenDeclaringTypeMatchThenTrue()
		{
			Assert.That(ReflectionHelper.GetMethodDefinition<List<int>>(t => t.Contains(5)).IsMethodOf(typeof(List<int>)), Is.True);
		}

		private class MyCollection: List<int>
		{
			
		}

		[Test]
		public void WhenCustomTypeMatchThenTrue()
		{
			Assert.That(ReflectionHelper.GetMethodDefinition<MyCollection>(t => t.Contains(5)).IsMethodOf(typeof(List<int>)), Is.True);
		}

		[Test]
		public void WhenTypeIsGenericDefinitionAndMatchThenTrue()
		{
			Assert.That(ReflectionHelper.GetMethodDefinition<List<int>>(t => t.Contains(5)).IsMethodOf(typeof(List<>)), Is.True);
		}

		[Test]
		public void WhenTypeIsGenericImplementedInterfaceAndMatchThenTrue()
		{
			var containsMethodDefinition = ReflectionHelper.GetMethodDefinition<List<int>>(t => t.Contains(5));
			Assert.That(containsMethodDefinition.IsMethodOf(typeof(ICollection<int>)), Is.True);
		}

		[Test]
		public void WhenTypeIsGenericImplementedInterfaceAndMatchGenericInterfaceDefinitionThenTrue()
		{
			var containsMethodDefinition = ReflectionHelper.GetMethodDefinition<List<int>>(t => t.Contains(5));
			Assert.That(containsMethodDefinition.IsMethodOf(typeof(ICollection<>)), Is.True);
		}

		[Test]
		public void WhenNoMatchThenFalse()
		{
			Assert.That(ReflectionHelper.GetMethodDefinition<List<int>>(t => t.Contains(5)).IsMethodOf(typeof(IEnumerable<>)), Is.False);
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
			var containsMethodDefinition = ReflectionHelper.GetMethodDefinition<MyClass>(t => t.MyMethod());
			Assert.That(containsMethodDefinition.IsMethodOf(typeof(MyAbstractClass<int>)), Is.True);
		}

		[Test]
		public void WhenTypeIsGenericImplementedAbstractAndMatchGenericInterfaceDefinitionThenTrue()
		{
			var containsMethodDefinition = ReflectionHelper.GetMethodDefinition<MyClass>(t => t.MyMethod());
			Assert.That(containsMethodDefinition.IsMethodOf(typeof(MyAbstractClass<>)), Is.True);
		}
	}
}