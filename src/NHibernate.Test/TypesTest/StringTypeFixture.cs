using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Type;
using NUnit.Framework;

namespace NHibernate.Test.TypesTest
{
	[TestFixture]
	public class StringTypeFixture : TypeFixtureBase
	{
		protected override string TypeName
		{
			get { return "String"; }
		}

		protected override void OnTearDown()
		{
			using (var s = OpenSession())
			{
				s.CreateQuery("delete from StringClass").ExecuteUpdate();
			}
		}

		[Test]
		public void InsertNullValue()
		{
			using (ISession s = OpenSession())
			{
				StringClass b = new StringClass();
				b.StringValue = null;
				s.Save(b);
				s.Flush();
			}

			using (ISession s = OpenSession())
			{
				StringClass b = (StringClass) s.CreateCriteria(typeof(StringClass)).UniqueResult();
				Assert.That(b.StringValue, Is.Null);
			}
		}

		[Test]
		public void InsertUnicodeValue()
		{
			const string unicode = "길동 최고 新闻 地图 ます プル éèêëîïôöõàâäåãçùûü бджзй αβ ខគឃ ضذخ";
			using (var s = OpenSession())
			{
				var b = new StringClass { StringValue = unicode };
				s.Save(b);
				s.Flush();
			}

			using (var s = OpenSession())
			{
				var b = s.Query<StringClass>().Single();
				Assert.That(b.StringValue, Is.EqualTo(unicode));
			}
		}

		[Test]
		public void IsCaseSensitive()
		{
			Assert.That(NHibernateUtil.String.IsEqual("ABC", "abc"), Is.False, "Equality");
			Assert.That(NHibernateUtil.String.GetHashCode("ABC"), Is.Not.EqualTo(NHibernateUtil.String.GetHashCode("abc")), "Hashcode");
		}

		[Test]
		public void IsCaseInsensitiveWithInsensitiveComparer()
		{
			var originalComparer = AbstractStringType.DefaultComparer;
			AbstractStringType.DefaultComparer = StringComparer.OrdinalIgnoreCase;
			try
			{
				Assert.That(NHibernateUtil.String.IsEqual("ABC", "abc"), Is.True, "Equality");
				Assert.That(
					NHibernateUtil.String.GetHashCode("ABC"),
					Is.EqualTo(NHibernateUtil.String.GetHashCode("abc")),
					"Hashcode");
			}
			finally
			{
				AbstractStringType.DefaultComparer = originalComparer;
			}
		}

		[Test]
		public void CanParseParameters()
		{
			Assert.Multiple(
				() =>
				{
					var type = TypeFactory.HeuristicType(
						"string",
						new Dictionary<string, string>
						{
							{ AbstractStringType.ComparerCultureParameterName, "Current" },
							{ AbstractStringType.CaseSensitiveParameterName, "false" }
						});
					Assert.That(type.IsEqual("ABC", "abc"), Is.True, "Current CI Equality");
					Assert.That(type.GetHashCode("ABC"), Is.EqualTo(type.GetHashCode("abc")), "Current CI Hashcode");
					Assert.That(type, Is.Not.EqualTo(NHibernateUtil.String), "A cached instance has been returned");

					type = TypeFactory.HeuristicType(
						"string",
						new Dictionary<string, string>
						{
							{ AbstractStringType.ComparerCultureParameterName, "Invariant" }
						});
					Assert.That(type.IsEqual("ABCI", "abci"), Is.False, "Invariant Equality");
					Assert.That(type.GetHashCode("ABCI"), Is.Not.EqualTo(type.GetHashCode("abci")), "Invariant Hashcode");

					type = TypeFactory.HeuristicType(
						"string",
						new Dictionary<string, string>
						{
							{ AbstractStringType.ComparerCultureParameterName, "Ordinal" },
							{ AbstractStringType.CaseSensitiveParameterName, "false" }
						});
					Assert.That(type.IsEqual("ABCI", "abci"), Is.True, "Ordinal CI Equality");
					Assert.That(type.GetHashCode("ABCI"), Is.EqualTo(type.GetHashCode("abci")), "Ordinal CI Hashcode");

					type = TypeFactory.HeuristicType(
						"string",
						new Dictionary<string, string>
						{
							{ AbstractStringType.CaseSensitiveParameterName, "false" }
						});
					Assert.That(type.IsEqual("ABCI", "abci"), Is.True, "CI Equality");
					Assert.That(type.GetHashCode("ABCI"), Is.EqualTo(type.GetHashCode("abci")), "CI Hashcode");

					type = TypeFactory.HeuristicType(
						"string",
						new Dictionary<string, string>
						{
							{ AbstractStringType.ComparerCultureParameterName, "tr" },
							{ AbstractStringType.CaseSensitiveParameterName, "false" }
						});
					Assert.That(type.IsEqual("ABC", "abc"), Is.True, "Turkish CI ABC Equality");
					Assert.That(type.GetHashCode("ABC"), Is.EqualTo(type.GetHashCode("abc")), "Turkish CI ABC Hashcode");
					Assert.That(type.IsEqual("İ", "i"), Is.True, "Turkish CI İi Equality");
					Assert.That(type.GetHashCode("İ"), Is.EqualTo(type.GetHashCode("i")), "Turkish CI İi Hashcode");
					Assert.That(type.IsEqual("I", "ı"), Is.True, "Turkish CI Iı Equality");
					Assert.That(type.GetHashCode("I"), Is.EqualTo(type.GetHashCode("ı")), "Turkish CI Iı Hashcode");
					// In Turkish, the i dot is meaningful and İ is not equivalent to ı, I is not equivalent to i.
					Assert.That(type.IsEqual("İ", "ı"), Is.False, "Turkish CI İı Equality");
					Assert.That(type.GetHashCode("İ"), Is.Not.EqualTo(type.GetHashCode("ı")), "Turkish CI İı Hashcode");
					Assert.That(type.IsEqual("I", "i"), Is.False, "Turkish CI Ii Equality");
					Assert.That(type.GetHashCode("I"), Is.Not.EqualTo(type.GetHashCode("i")), "Turkish CI Ii Hashcode");
				});
		}
	}
}
