﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NHibernate.Transform;
using NHibernate.Util;
using NUnit.Framework;

namespace NHibernate.Test.TransformTests
{
	using System.Threading.Tasks;
	using System.Threading;
	[TestFixture]
	public class AliasToBeanResultTransformerFixtureAsync : TestCase
	{
		public class WithOutPublicParameterLessCtor
		{
			private string something;
			protected WithOutPublicParameterLessCtor() { }

			public WithOutPublicParameterLessCtor(string something)
			{
				this.something = something;
			}

			public string Something
			{
				get { return something; }
			}
		}

		public class PublicParameterLessCtor
		{
			private string something;

			public string Something
			{
				get { return something; }
				set { something = value; }
			}
		}

		public struct TestStruct
		{
			public string Something { get; set; }
		}

		public class PublicPropertiesSimpleDTO
		{
			public object Id { get; set; }
			public string Name { get; set; }
		}

		public class PrivateFieldsSimpleDTO
		{
#pragma warning disable CS0649
			private object id;
			private string name;
#pragma warning restore CS0649

			public object Id { get { return id; } }
			public string Name { get { return name; } }
		}

		public class BasePublicPropsSimpleDTO
		{
			public object Id { get; set; }
		}

		public class PublicInheritedPropertiesSimpleDTO : BasePublicPropsSimpleDTO
		{
			public string Name { get; set; }
		}

		public class BasePrivateFieldSimpleDTO
		{
#pragma warning disable CS0649
			private object id;
#pragma warning restore CS0649
			public object Id { get { return id; } }
		}

		public class PrivateInheritedFieldsSimpleDTO : BasePrivateFieldSimpleDTO
		{
#pragma warning disable CS0649
			private string name;
#pragma warning restore CS0649
			public string Name { get { return name; } }
		}

		public class PropertiesInsensitivelyDuplicated
		{
			public object Id { get; set; }
			public string NaMe { get; set; }
			public string NamE { get; set; }
		}

		public class BasePrivateFieldSimpleDTO2
		{
#pragma warning disable CS0649
			private object _id;
#pragma warning restore CS0649
			public object iD => _id;
		}

		public class PrivateInheritedFieldsSimpleDTO2 : BasePrivateFieldSimpleDTO2
		{
#pragma warning disable CS0649
			private string namE;
#pragma warning restore CS0649
			public string Name { get { return namE; } }
		}

		public class NewPropertiesSimpleDTO : PrivateInheritedFieldsSimpleDTO2
		{
			public new string Name { get; set; }
			internal string NaMe { get; set; }

			public object Id { get; set; }
		}

		#region Overrides of TestCase

		protected override string[] Mappings
		{
			get { return new[] { "TransformTests.Simple.hbm.xml" }; }
		}

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override void OnSetUp()
		{
			using (var s = OpenSession())
			{
				using (s.BeginTransaction())
				{
					s.Save(new Simple { Name = "Name1" });
					s.Save(new Simple { Name = "Name2" });
					s.Transaction.Commit();
				}
			}
		}

		protected override void OnTearDown()
		{
			using (var s = OpenSession())
			{
				using (s.BeginTransaction())
				{
					s.Delete("from Simple");
					s.Transaction.Commit();
				}
			}
		}

		#endregion

		[Test]
		public async Task WorkWithOutPublicParameterLessCtorAsync()
		{
			await (AssertCardinalityAndSomethingAsync<WithOutPublicParameterLessCtor>());
		}

		[Test]
		public async Task ToPublicProperties_WithoutAnyProjectionsAsync()
		{
			await (AssertCardinalityNameAndIdAsync<PublicPropertiesSimpleDTO>());
		}

		[Test]
		public async Task ToPrivateFields_WithoutAnyProjectionsAsync()
		{
			await (AssertCardinalityNameAndIdAsync<PrivateFieldsSimpleDTO>());
		}

		[Test]
		public async Task ToInheritedPublicProperties_WithoutProjectionsAsync()
		{
			await (AssertCardinalityNameAndIdAsync<PublicInheritedPropertiesSimpleDTO>());
		}

		[Test]
		public async Task ToInheritedPrivateFields_WithoutProjectionsAsync()
		{
			await (AssertCardinalityNameAndIdAsync<PrivateInheritedFieldsSimpleDTO>());
		}

		[Test]
		public async Task WorkWithPublicParameterLessCtor_FieldsAsync()
		{
			await (AssertCardinalityAndSomethingAsync<PublicParameterLessCtor>());
		}

		[Test]
		public async Task WorkWithPublicParameterLessCtor_PropertiesAsync()
		{
			await (AssertCardinalityAndSomethingAsync<PublicParameterLessCtor>("select s.Name as Something from Simple s"));
		}

		[Test]
		public async Task WorksWithStructAsync()
		{
			await (AssertCardinalityAndSomethingAsync<TestStruct>());
		}

		[Test]
		public async Task WorksWithNewPropertyAsync()
		{
			await (AssertCardinalityNameAndIdAsync<NewPropertiesSimpleDTO>());
		}

		[Test]
		public async Task WorksWithManyCandidatesAsync()
		{
			using (var s = OpenSession())
			{
				var transformer = GetTransformer<NewPropertiesSimpleDTO>();
				var l = await (s.CreateSQLQuery("select id as ID, Name as NamE from Simple")
						.SetResultTransformer(transformer)
						.ListAsync<NewPropertiesSimpleDTO>());
				Assert.That(l.Count, Is.EqualTo(2));
				Assert.That(l, Has.All.Not.Null);
				Assert.That(l, Has.Some.Property("Name").EqualTo("Name1"));
				Assert.That(l, Has.Some.Property("Name").EqualTo("Name2"));
				Assert.That(l, Has.All.Property("Id").Not.Null);
				Assert.That(l, Has.All.Property("iD").Null);
				Assert.That(l, Has.All.Property("NaMe").Null);
			}
		}

		[Test]
		public void ToPropertiesInsensitivelyDuplicated_WithoutAnyProjectionsAsync()
		{
			using (var s = OpenSession())
			{
				var transformer = GetTransformer<PropertiesInsensitivelyDuplicated>();
				Assert.ThrowsAsync<AmbiguousMatchException>(() =>
				{
					return s.CreateSQLQuery("select * from Simple")
						.SetResultTransformer(transformer)
						.ListAsync<PropertiesInsensitivelyDuplicated>();
				});
			}
		}

		[Test]
		public async Task SerializationAsync()
		{
			await (AssertSerializationAsync<PublicPropertiesSimpleDTO>());
			await (AssertSerializationAsync<PrivateFieldsSimpleDTO>());
			await (AssertSerializationAsync<PublicInheritedPropertiesSimpleDTO>());
			await (AssertSerializationAsync<PrivateInheritedFieldsSimpleDTO>());
			await (AssertSerializationAsync<NewPropertiesSimpleDTO>());
		}

		enum TestEnum
		{ Value0, Value1 }

		class TestDto
		{
			private TestDto()
			{ }

			public TestDto(bool bogus) { }

			public string StringProp { get; set; }
			public int IntProp { get; set; }
			public int IntPropNull { get; set; }
			public int? IntPropNullNullable { get; set; }
			public TestEnum EnumProp { get; set; }
		}

		struct TestDtoAsStruct
		{
			public string StringProp { get; set; }
			public int IntProp { get; set; }
			public int IntPropNull { get; set; }
			public int? IntPropNullNullable { get; set; }
			public TestEnum EnumProp { get; set; }
		}

		class NoDefCtorDto
		{
			public NoDefCtorDto(bool bogus)
			{
			}
		}

		private async Task AssertCardinalityNameAndIdAsync<T>(IResultTransformer transformer = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			using (var s = OpenSession())
			{
				transformer = transformer ?? GetTransformer<T>();
				var l = await (s.CreateSQLQuery("select * from Simple")
						.SetResultTransformer(transformer)
						.ListAsync<T>(cancellationToken));
				var testClass = typeof(T).Name;
				Assert.That(l.Count, Is.EqualTo(2), testClass);
				Assert.That(l, Has.All.Not.Null, testClass);
				Assert.That(l, Has.Some.Property("Name").EqualTo("Name1"), testClass);
				Assert.That(l, Has.Some.Property("Name").EqualTo("Name2"), testClass);
				Assert.That(l, Has.All.Property("Id").Not.Null, testClass);
			}
		}

		private async Task AssertCardinalityAndSomethingAsync<T>(string queryString = "select s.Name as something from Simple s", CancellationToken cancellationToken = default(CancellationToken))
		{
			using (var s = OpenSession())
			{
				var transformer = GetTransformer<T>();
				var l = await (s.CreateSQLQuery(queryString)
						.SetResultTransformer(transformer)
						.ListAsync<T>(cancellationToken));
				var testClass = typeof(T).Name;
				Assert.That(l.Count, Is.EqualTo(2), testClass);
				Assert.That(l, Has.All.Not.Null, testClass);
				Assert.That(l, Has.Some.Property("Something").EqualTo("Name1"), testClass);
				Assert.That(l, Has.Some.Property("Something").EqualTo("Name2"), testClass);
			}
		}

		private Task AssertSerializationAsync<T>(CancellationToken cancellationToken = default(CancellationToken))
		{
			try
			{
				var transformer = GetTransformer<T>();
				var bytes = SerializationHelper.Serialize(transformer);
				transformer = (IResultTransformer) SerializationHelper.Deserialize(bytes);
				return AssertCardinalityNameAndIdAsync<T>(transformer: transformer, cancellationToken: cancellationToken);
			}
			catch (System.Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		protected IResultTransformer GetTransformer<T>()
		{
			return Transformers.AliasToBean<T>();
		}
	}
}
