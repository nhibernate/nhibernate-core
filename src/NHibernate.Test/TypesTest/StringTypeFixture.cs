using System.Linq;
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
	}
}
