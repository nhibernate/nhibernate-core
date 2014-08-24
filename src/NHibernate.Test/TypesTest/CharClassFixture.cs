using NUnit.Framework;
using SharpTestsEx;

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
				Executing.This(()=> saved = s.Get<CharClass>(1)).Should().NotThrow();
				saved.NormalChar.Should().Be('A');
				saved.NullableChar.Should().Not.Have.Value();

				s.Delete(saved);
				s.Flush();
			}
		}
	}
}