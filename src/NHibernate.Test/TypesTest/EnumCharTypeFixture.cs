using System.Collections;
using NHibernate.Type;
using NUnit.Framework;

namespace NHibernate.Test.TypesTest
{
	[TestFixture]
	public class EnumCharTypeFixture : TypeFixtureBase
	{
		protected override string TypeName
		{
			get { return "EnumChar"; }
		}

		protected override void OnSetUp()
		{

			EnumCharClass basic = new EnumCharClass();
			basic.Id = 1;
			basic.EnumValue = SampleCharEnum.Dimmed;

			EnumCharClass basic2 = new EnumCharClass();
			basic2.Id = 2;
			basic2.EnumValue = SampleCharEnum.On;

			ISession s = OpenSession();
			s.Save(basic);
			s.Save(basic2);
			s.Flush();
			s.Close();
		}

		protected override void OnTearDown()
		{
			ISession s = OpenSession();
			s.Delete("from EnumCharClass");
			s.Delete("from EnumCharBaz");
			s.Flush();
			s.Close();
		}

		[Test]
		public void ReadFromLoad()
		{
			using (ISession s = OpenSession())
			{
				EnumCharClass basic = (EnumCharClass) s.Load(typeof (EnumCharClass), 1);
				Assert.AreEqual(SampleCharEnum.Dimmed, basic.EnumValue);

				EnumCharClass basic2 = (EnumCharClass) s.Load(typeof (EnumCharClass), 2);
				Assert.AreEqual(SampleCharEnum.On, basic2.EnumValue);
			}
		}

		[Test]
		public void ReadFromQueryUsingValue()
		{
			using (ISession s = OpenSession())
			{
				IList results;
				IQuery q = s.CreateQuery("from EnumCharClass as ecc where ecc.EnumValue=:value");

				q.SetParameter("value", SampleCharEnum.On,new EnumCharType<SampleCharEnum>());
				results = q.List();

				Assert.AreEqual(1, results.Count, "only 1 was 'On'");

				q.SetParameter("value", SampleCharEnum.Off, new EnumCharType<SampleCharEnum>());
				results = q.List();

				Assert.AreEqual(0, results.Count, "should not be any in the 'Off' status");
			}
		}

		[Test]
		public void ReadFromQueryUsingString()
		{
			using (ISession s = OpenSession())
			{
				IList results;
				IQuery q = s.CreateQuery("from EnumCharClass as ecc where ecc.EnumValue=:value");

				q.SetString("value", "N");
				results = q.List();

				Assert.AreEqual(1, results.Count, "only 1 was \"N\" string");

				q.SetString("value", "F");
				results = q.List();

				Assert.AreEqual(0, results.Count, "should not be any in the \"F\" string");
			}
		}

		[Test]
		public void ReadFromQueryUsingChar()
		{
			using (ISession s = OpenSession())
			{
				IList results;
				IQuery q = s.CreateQuery("from EnumCharClass as ecc where ecc.EnumValue=:value");

				q.SetCharacter("value", 'N');
				results = q.List();

				Assert.AreEqual(1, results.Count, "only 1 was 'N' char");

				q.SetCharacter("value", 'F');
				results = q.List();

				Assert.AreEqual(0, results.Count, "should not be any in the 'F' char");
			}
		}

		[Test]
		public void CanBeUsedAsDiscriminator()
		{
			EnumCharFoo foo = new EnumCharFoo();
			EnumCharBar bar = new EnumCharBar();

			foo.Id = 1;
			bar.Id = 2;

			using (ISession s = OpenSession())
			{
				s.Save(foo);
				s.Save(bar);
				s.Flush();
			}

			using (ISession s = OpenSession())
			{
				s.Load<EnumCharFoo>(1);
				s.Load<EnumCharBar>(2);

				EnumCharBaz baz;

				baz = s.Load<EnumCharBaz>(1);
				Assert.AreEqual(SampleCharEnum.Dimmed, baz.Type);

				baz = s.Load<EnumCharBaz>(2);
				Assert.AreEqual(SampleCharEnum.Off, baz.Type);
			}
		}
	}
}