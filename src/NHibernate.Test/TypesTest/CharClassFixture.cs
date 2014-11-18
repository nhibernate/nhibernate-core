using NUnit.Framework;

namespace NHibernate.Test.TypesTest
{
	public class CharClassFixture : TypeFixtureBase
	{
		protected override string TypeName
		{
			get { return "Char"; }
		}

		[Test]
		public void ReadWrite()
		{
			var basic = new CharClass{Id=1,NormalChar = 'A'};

			using (var s = OpenSession())
			{
				s.Save(basic);
				s.Flush();
			}

			using (var s = OpenSession())
			{
				CharClass saved= null;
				Assert.That(() => saved = s.Get<CharClass>(1), Throws.Nothing);
				Assert.That(saved.NormalChar, Is.EqualTo('A'));
				Assert.That(saved.NullableChar, Is.Null);

				s.Delete(saved);
				s.Flush();
			}
		}
	}
}