using System;
using System.Collections.Generic;
using System.Reflection;
using NHibernate.Linq;
using NHibernate.Util;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.UtilityTest
{
	public class ReflectionHelperIsMethodOfTests
	{
		[Test]
		public void WhenNullMethodInfoThenThrows()
		{
			((MethodInfo) null).Executing(mi => mi.IsMethodOf(typeof (Object))).Throws<ArgumentNullException>();
		}

		[Test]
		public void WhenNullTypeThenThrows()
		{
			ReflectionHelper.GetMethodDefinition<List<int>>(t => t.Contains(5)).Executing(mi => mi.IsMethodOf(null)).Throws<ArgumentNullException>();
		}

		[Test]
		public void WhenDeclaringTypeMatchThenTrue()
		{
			ReflectionHelper.GetMethodDefinition<List<int>>(t => t.Contains(5)).IsMethodOf(typeof(List<int>)).Should().Be.True();
		}

		private class MyCollection: List<int>
		{
			
		}

		[Test]
		public void WhenCustomTypeMatchThenTrue()
		{
			ReflectionHelper.GetMethodDefinition<MyCollection>(t => t.Contains(5)).IsMethodOf(typeof(List<int>)).Should().Be.True();
		}

		[Test]
		public void WhenTypeIsGenericDefinitionAndMatchThenTrue()
		{
			ReflectionHelper.GetMethodDefinition<List<int>>(t => t.Contains(5)).IsMethodOf(typeof(List<>)).Should().Be.True();
		}

		[Test]
		public void WhenTypeIsGenericImplementedInterfaceAndMatchThenTrue()
		{
			var containsMethodDefinition = ReflectionHelper.GetMethodDefinition<List<int>>(t => t.Contains(5));
			containsMethodDefinition.IsMethodOf(typeof(ICollection<int>)).Should().Be.True();
		}

		[Test]
		public void WhenTypeIsGenericImplementedInterfaceAndMatchGenericInterfaceDefinitionThenTrue()
		{
			var containsMethodDefinition = ReflectionHelper.GetMethodDefinition<List<int>>(t => t.Contains(5));
			containsMethodDefinition.IsMethodOf(typeof(ICollection<>)).Should().Be.True();
		}

		[Test]
		public void WhenNoMatchThenFalse()
		{
			ReflectionHelper.GetMethodDefinition<List<int>>(t => t.Contains(5)).IsMethodOf(typeof(IEnumerable<>)).Should().Be.False();
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
			containsMethodDefinition.IsMethodOf(typeof(MyAbstractClass<int>)).Should().Be.True();
		}

		[Test]
		public void WhenTypeIsGenericImplementedAbstractAndMatchGenericInterfaceDefinitionThenTrue()
		{
			var containsMethodDefinition = ReflectionHelper.GetMethodDefinition<MyClass>(t => t.MyMethod());
			containsMethodDefinition.IsMethodOf(typeof(MyAbstractClass<>)).Should().Be.True();
		}
	}
}