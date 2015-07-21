using System;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode.Impl;
using NUnit.Framework;

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
			Assert.That(variableType.GetShortClassName(new HbmMapping()), Is.Null);
		}

		[Test]
		public void WhenMapDocNullThenAssemblyQualifiedName()
		{
			Assert.That(typeof(MyEntity).GetShortClassName(null), Is.EqualTo(typeof(MyEntity).AssemblyQualifiedName));
		}

		[Test]
		public void WhenMapDocDoesNotHaveDefaultsThenAssemblyQualifiedName()
		{
			var mapDoc = new HbmMapping();
			Assert.That(typeof(MyEntity).GetShortClassName(mapDoc), Is.EqualTo(typeof(MyEntity).AssemblyQualifiedName));
		}

		[Test]
		public void WhenMapDocHaveDefaultAssemblyThenFullName()
		{
			var mapDoc = new HbmMapping();
			mapDoc.assembly = typeof(MyEntity).Assembly.FullName;
			Assert.That(typeof(MyEntity).GetShortClassName(mapDoc), Is.EqualTo(typeof(MyEntity).FullName));
		}

		[Test]
		public void WhenMapDocHaveDefaultAssemblyNameThenFullName()
		{
			var mapDoc = new HbmMapping();
			mapDoc.assembly = typeof(MyEntity).Assembly.GetName().Name;
			Assert.That(typeof(MyEntity).GetShortClassName(mapDoc), Is.EqualTo(typeof(MyEntity).FullName));
		}

		[Test]
		public void WhenMapDocHaveDefaultsThenName()
		{
			var mapDoc = new HbmMapping();
			mapDoc.assembly = typeof(MyEntity).Assembly.FullName;
			mapDoc.@namespace = typeof(MyEntity).Namespace;
			Assert.That(typeof(MyEntity).GetShortClassName(mapDoc), Is.EqualTo(typeof(MyEntity).Name));
		}

		[Test]
		public void WhenMapDocDefaultsDoesNotMatchsThenAssemblyQualifiedName()
		{
			var mapDoc = new HbmMapping();
			mapDoc.assembly = "whatever";
			mapDoc.@namespace = "whatever";
			Assert.That(typeof(MyEntity).GetShortClassName(mapDoc), Is.EqualTo(typeof(MyEntity).AssemblyQualifiedName));
		}

		[Test]
		public void WhenMatchNamespaceButNotAssemblyThenOnlyNameAndAssembly()
		{
			// strange but possible
			var mapDoc = new HbmMapping();
			mapDoc.assembly = "whatever";
			mapDoc.@namespace = typeof(MyEntity).Namespace;
			Assert.That(typeof(MyEntity).GetShortClassName(mapDoc), Is.StringStarting(typeof(MyEntity).Name).And.EndsWith(", " + typeof(MyEntity).Assembly.GetName().Name));
		}

		[Test]
		public void WithGenericWhenMapDocNullThenAssemblyQualifiedName()
		{
			Assert.That(typeof(MyGenericEntity<int>).GetShortClassName(null), Is.EqualTo(typeof(MyGenericEntity<int>).AssemblyQualifiedName));
		}

		[Test]
		public void WithGenericWhenMapDocDoesNotHaveDefaultsThenAssemblyQualifiedName()
		{
			var mapDoc = new HbmMapping();
			Assert.That(typeof(MyGenericEntity<int>).GetShortClassName(mapDoc), Is.EqualTo(typeof(MyGenericEntity<int>).AssemblyQualifiedName));
		}

		[Test]
		public void WithGenericWhenMapDocHaveDefaultAssemblyThenFullName()
		{
			var mapDoc = new HbmMapping();
			mapDoc.assembly = typeof(MyGenericEntity<>).Assembly.FullName;
			Assert.That(typeof(MyGenericEntity<int>).GetShortClassName(mapDoc), Is.EqualTo(typeof(MyGenericEntity<int>).FullName));
		}

		[Test]
		public void WithGenericWhenMapDocHaveDefaultAssemblyNameThenFullName()
		{
			var mapDoc = new HbmMapping();
			mapDoc.assembly = typeof(MyGenericEntity<>).Assembly.GetName().Name;
			Assert.That(typeof(MyGenericEntity<int>).GetShortClassName(mapDoc), Is.EqualTo(typeof(MyGenericEntity<int>).FullName));
		}

		[Test]
		public void WithGenericWhenMapDocHaveDefaultsThenName()
		{
			var mapDoc = new HbmMapping();
			mapDoc.assembly = typeof(MyGenericEntity<>).Assembly.FullName;
			mapDoc.@namespace = typeof(MyGenericEntity<>).Namespace;
			Assert.That(typeof(MyGenericEntity<int>).GetShortClassName(mapDoc), Is.EqualTo(typeof(MyGenericEntity<int>).FullName));
		}

		[Test]
		public void WithGenericWhenMapDocDefaultsDoesNotMatchsThenAssemblyQualifiedName()
		{
			var mapDoc = new HbmMapping();
			mapDoc.assembly = "whatever";
			mapDoc.@namespace = "whatever";
			Assert.That(typeof(MyGenericEntity<int>).GetShortClassName(mapDoc), Is.EqualTo(typeof(MyGenericEntity<int>).AssemblyQualifiedName));
		}

		[Test]
		public void WithGenericWhenMatchNamespaceButNotAssemblyThenOnlyNameAndAssembly()
		{
			// strange but possible
			var mapDoc = new HbmMapping();
			mapDoc.assembly = "whatever";
			mapDoc.@namespace = typeof(MyGenericEntity<>).Namespace;
			Assert.That(typeof(MyGenericEntity<int>).GetShortClassName(mapDoc), Is.StringStarting(typeof(MyGenericEntity<int>).FullName).And.EndsWith(", " + typeof(MyGenericEntity<int>).Assembly.GetName().Name));
		}
	}
}