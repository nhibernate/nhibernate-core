using System;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode.Impl;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.MappingByCode
{
	public class MyEntity
	{

	}
	public class MyGenericEntity<T>
	{

	}

	public class TypeNameUtilTests
	{
		[Test]
		public void WhenTypeNullThenNull()
		{
			System.Type variableType = null;
			variableType.GetShortClassName(new HbmMapping()).Should().Be.Null();
		}

		[Test]
		public void WhenMapDocNullThenAssemblyQualifiedName()
		{
			typeof(MyEntity).GetShortClassName(null).Should().Be.EqualTo(typeof(MyEntity).AssemblyQualifiedName);
		}

		[Test]
		public void WhenMapDocDoesNotHaveDefaultsThenAssemblyQualifiedName()
		{
			var mapDoc = new HbmMapping();
			typeof(MyEntity).GetShortClassName(mapDoc).Should().Be.EqualTo(typeof(MyEntity).AssemblyQualifiedName);
		}

		[Test]
		public void WhenMapDocHaveDefaultAssemblyThenFullName()
		{
			var mapDoc = new HbmMapping();
			mapDoc.assembly = typeof(MyEntity).Assembly.FullName;
			typeof(MyEntity).GetShortClassName(mapDoc).Should().Be.EqualTo(typeof(MyEntity).FullName);
		}

		[Test]
		public void WhenMapDocHaveDefaultAssemblyNameThenFullName()
		{
			var mapDoc = new HbmMapping();
			mapDoc.assembly = typeof(MyEntity).Assembly.GetName().Name;
			typeof(MyEntity).GetShortClassName(mapDoc).Should().Be.EqualTo(typeof(MyEntity).FullName);
		}

		[Test]
		public void WhenMapDocHaveDefaultsThenName()
		{
			var mapDoc = new HbmMapping();
			mapDoc.assembly = typeof(MyEntity).Assembly.FullName;
			mapDoc.@namespace = typeof(MyEntity).Namespace;
			typeof(MyEntity).GetShortClassName(mapDoc).Should().Be.EqualTo(typeof(MyEntity).Name);
		}

		[Test]
		public void WhenMapDocDefaultsDoesNotMatchsThenAssemblyQualifiedName()
		{
			var mapDoc = new HbmMapping();
			mapDoc.assembly = "whatever";
			mapDoc.@namespace = "whatever";
			typeof(MyEntity).GetShortClassName(mapDoc).Should().Be.EqualTo(typeof(MyEntity).AssemblyQualifiedName);
		}

		[Test]
		public void WhenMatchNamespaceButNotAssemblyThenOnlyNameAndAssembly()
		{
			// strange but possible
			var mapDoc = new HbmMapping();
			mapDoc.assembly = "whatever";
			mapDoc.@namespace = typeof(MyEntity).Namespace;
			typeof(MyEntity).GetShortClassName(mapDoc).Should().StartWith(typeof(MyEntity).Name).And.EndWith(", " + typeof(MyEntity).Assembly.GetName().Name);
		}

		[Test]
		public void WithGenericWhenMapDocNullThenAssemblyQualifiedName()
		{
			typeof(MyGenericEntity<int>).GetShortClassName(null).Should().Be.EqualTo(typeof(MyGenericEntity<int>).AssemblyQualifiedName);
		}

		[Test]
		public void WithGenericWhenMapDocDoesNotHaveDefaultsThenAssemblyQualifiedName()
		{
			var mapDoc = new HbmMapping();
			typeof(MyGenericEntity<int>).GetShortClassName(mapDoc).Should().Be.EqualTo(typeof(MyGenericEntity<int>).AssemblyQualifiedName);
		}

		[Test]
		public void WithGenericWhenMapDocHaveDefaultAssemblyThenFullName()
		{
			var mapDoc = new HbmMapping();
			mapDoc.assembly = typeof(MyGenericEntity<>).Assembly.FullName;
			typeof(MyGenericEntity<int>).GetShortClassName(mapDoc).Should().Be.EqualTo(typeof(MyGenericEntity<int>).FullName);
		}

		[Test]
		public void WithGenericWhenMapDocHaveDefaultAssemblyNameThenFullName()
		{
			var mapDoc = new HbmMapping();
			mapDoc.assembly = typeof(MyGenericEntity<>).Assembly.GetName().Name;
			typeof(MyGenericEntity<int>).GetShortClassName(mapDoc).Should().Be.EqualTo(typeof(MyGenericEntity<int>).FullName);
		}

		[Test]
		public void WithGenericWhenMapDocHaveDefaultsThenName()
		{
			var mapDoc = new HbmMapping();
			mapDoc.assembly = typeof(MyGenericEntity<>).Assembly.FullName;
			mapDoc.@namespace = typeof(MyGenericEntity<>).Namespace;
			typeof(MyGenericEntity<int>).GetShortClassName(mapDoc).Should().Be.EqualTo(typeof(MyGenericEntity<int>).FullName);
		}

		[Test]
		public void WithGenericWhenMapDocDefaultsDoesNotMatchsThenAssemblyQualifiedName()
		{
			var mapDoc = new HbmMapping();
			mapDoc.assembly = "whatever";
			mapDoc.@namespace = "whatever";
			typeof(MyGenericEntity<int>).GetShortClassName(mapDoc).Should().Be.EqualTo(typeof(MyGenericEntity<int>).AssemblyQualifiedName);
		}

		[Test]
		public void WithGenericWhenMatchNamespaceButNotAssemblyThenOnlyNameAndAssembly()
		{
			// strange but possible
			var mapDoc = new HbmMapping();
			mapDoc.assembly = "whatever";
			mapDoc.@namespace = typeof(MyGenericEntity<>).Namespace;
			typeof(MyGenericEntity<int>).GetShortClassName(mapDoc).Should().StartWith(typeof(MyGenericEntity<int>).FullName).And.EndWith(", " + typeof(MyGenericEntity<int>).Assembly.GetName().Name);
		}
	}
}