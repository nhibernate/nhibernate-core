using NUnit.Framework;
using System.Linq;

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

		[Test]
		public void ParameterTypeForAnsiCharInLinq()
		{
			using (var logSpy = new SqlLogSpy())
			using (var session = OpenSession())
			{
				var result = (from e in session.Query<CharClass>()
							where e.AnsiChar == 'B'
							select e).ToList();
				
				Assert.That(logSpy.GetWholeLog(), Does.Contain("Type: AnsiString"));
			}
		}

		[Test]
		public void ParameterTypeForCharInAnsiStringInLinq()
		{
			using (var logSpy = new SqlLogSpy())
			using (var session = OpenSession())
			{
				var result = (from e in session.Query<CharClass>()
							where e.AnsiString[0] == 'P'
							select e).ToList();

				Assert.That(logSpy.GetWholeLog(), Does.Contain("Type: AnsiString"));
			}
		}
	}
}
