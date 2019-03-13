using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NHibernate.Transform;
using NHibernate.Util;
using NUnit.Framework;

namespace NHibernate.Test.TransformTests
{
	[TestFixture]
	public class AliasToBeanResultTransformerFixture : TestCase
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
		public void WorkWithOutPublicParameterLessCtor()
		{
			AssertCardinalityAndSomething<WithOutPublicParameterLessCtor>();
		}

		[Test]
		public void ToPublicProperties_WithoutAnyProjections()
		{
			AssertCardinalityNameAndId<PublicPropertiesSimpleDTO>();
		}

		[Test]
		public void ToPrivateFields_WithoutAnyProjections()
		{
			AssertCardinalityNameAndId<PrivateFieldsSimpleDTO>();
		}

		[Test]
		public void ToInheritedPublicProperties_WithoutProjections()
		{
			AssertCardinalityNameAndId<PublicInheritedPropertiesSimpleDTO>();
		}

		[Test]
		public void ToInheritedPrivateFields_WithoutProjections()
		{
			AssertCardinalityNameAndId<PrivateInheritedFieldsSimpleDTO>();
		}

		[Test]
		public void WorkWithPublicParameterLessCtor_Fields()
		{
			AssertCardinalityAndSomething<PublicParameterLessCtor>();
		}

		[Test]
		public void WorkWithPublicParameterLessCtor_Properties()
		{
			AssertCardinalityAndSomething<PublicParameterLessCtor>("select s.Name as Something from Simple s");
		}

		[Test]
		public void WorksWithStruct()
		{
			AssertCardinalityAndSomething<TestStruct>();
		}

		[Test]
		public void WorksWithNewProperty()
		{
			AssertCardinalityNameAndId<NewPropertiesSimpleDTO>();
		}

		[Test]
		public void WorksWithManyCandidates()
		{
			using (var s = OpenSession())
			{
				var transformer = Transformers.AliasToBean<NewPropertiesSimpleDTO>();
				var l = s.CreateSQLQuery("select id as ID, Name as NamE from Simple")
						.SetResultTransformer(transformer)
						.List<NewPropertiesSimpleDTO>();
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
		public void ToPropertiesInsensitivelyDuplicated_WithoutAnyProjections()
		{
			using (var s = OpenSession())
			{
				var transformer = Transformers.AliasToBean<PropertiesInsensitivelyDuplicated>();
				Assert.Throws<AmbiguousMatchException>(() =>
				{
					s.CreateSQLQuery("select * from Simple")
						.SetResultTransformer(transformer)
						.List<PropertiesInsensitivelyDuplicated>();
				});
			}
		}

		[Test]
		public void Serialization()
		{
			AssertSerialization<PublicPropertiesSimpleDTO>();
			AssertSerialization<PrivateFieldsSimpleDTO>();
			AssertSerialization<PublicInheritedPropertiesSimpleDTO>();
			AssertSerialization<PrivateInheritedFieldsSimpleDTO>();
			AssertSerialization<NewPropertiesSimpleDTO>();
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

		[Test]
		public void TupleConversion()
		{
			var o = new TestDto(true)
			{
				IntProp = 1,
				IntPropNull = 0,
				StringProp = "hello",
				IntPropNullNullable = null,
				EnumProp = TestEnum.Value1,
			};
			string nullMarker = "NULL";
			var testData = new Dictionary<string, object>
			{
				{nameof(o.IntProp), o.IntProp},
				{nullMarker, decimal.MaxValue},
				{nameof(o.IntPropNull).ToLowerInvariant(), null},
				{string.Empty, new object()},
				{nameof(o.IntPropNullNullable).ToLowerInvariant(), null},
				{nameof(o.EnumProp), 1},
				{nameof(o.StringProp), o.StringProp},
			};
			var aliases = testData.Keys.Select(k => k == nullMarker ? null : k).ToArray();

			var tuple = testData.Values.ToArray();

			var actual = (TestDto) Transformers.AliasToBean<TestDto>().TransformTuple(tuple, aliases);
			var actualStruct = (TestDtoAsStruct) Transformers.AliasToBean<TestDtoAsStruct>().TransformTuple(tuple, aliases);
			Assert.That(actual.IntProp, Is.EqualTo(o.IntProp));
			Assert.That(actual.IntPropNull, Is.EqualTo(o.IntPropNull));
			Assert.That(actual.StringProp, Is.EqualTo(o.StringProp));
			Assert.That(actual.IntPropNullNullable, Is.EqualTo(o.IntPropNullNullable));
			Assert.That(actual.EnumProp, Is.EqualTo(o.EnumProp));
		}

		[Test]
		public void ThrowUserFriendlyException()
		{
			var o = new TestDto(true) { };
			
			string nullMarker = "NULL";
			var testData = new Dictionary<string, object>
			{
				{nameof(o.IntProp), "hello"},
			};
			var aliases = testData.Keys.Select(k => k == nullMarker ? null : k).ToArray();
			var tuple = testData.Values.ToArray();

			var ex = Assert.Throws<System.InvalidCastException>(() => Transformers.AliasToBean<TestDto>().TransformTuple(tuple, aliases));
			Assert.That(ex, Has.Message.Contains(nameof(o.IntProp)));
			var ex2 = Assert.Throws<System.InvalidCastException>(() => Transformers.AliasToBean<TestDtoAsStruct>().TransformTuple(tuple, aliases));
			Assert.That(ex2, Has.Message.Contains(nameof(o.IntProp)));
		}

		class NoDefCtorDto
		{
			public NoDefCtorDto(bool bogus)
			{
			}
		}

		[Test]
		public void ThrowsForClassWithoutDefaultCtor()
		{
			Assert.That(() => Transformers.AliasToBean<NoDefCtorDto>().TransformTuple(new object[0], new string[0]), Throws.ArgumentException);
		}

		private void AssertCardinalityNameAndId<T>(IResultTransformer transformer = null)
		{
			using (var s = OpenSession())
			{
				transformer = transformer ?? Transformers.AliasToBean<T>();
				var l = s.CreateSQLQuery("select * from Simple")
						.SetResultTransformer(transformer)
						.List<T>();
				var testClass = typeof(T).Name;
				Assert.That(l.Count, Is.EqualTo(2), testClass);
				Assert.That(l, Has.All.Not.Null, testClass);
				Assert.That(l, Has.Some.Property("Name").EqualTo("Name1"), testClass);
				Assert.That(l, Has.Some.Property("Name").EqualTo("Name2"), testClass);
				Assert.That(l, Has.All.Property("Id").Not.Null, testClass);
			}
		}

		private void AssertCardinalityAndSomething<T>(string queryString = "select s.Name as something from Simple s")
		{
			using (var s = OpenSession())
			{
				var transformer = Transformers.AliasToBean<T>();
				var l = s.CreateSQLQuery(queryString)
						.SetResultTransformer(transformer)
						.List<T>();
				var testClass = typeof(T).Name;
				Assert.That(l.Count, Is.EqualTo(2), testClass);
				Assert.That(l, Has.All.Not.Null, testClass);
				Assert.That(l, Has.Some.Property("Something").EqualTo("Name1"), testClass);
				Assert.That(l, Has.Some.Property("Something").EqualTo("Name2"), testClass);
			}
		}

		private void AssertSerialization<T>()
		{
			var transformer = Transformers.AliasToBean<T>();
			var bytes = SerializationHelper.Serialize(transformer);
			transformer = (IResultTransformer) SerializationHelper.Deserialize(bytes);
			AssertCardinalityNameAndId<T>(transformer: transformer);
		}
	}
}
