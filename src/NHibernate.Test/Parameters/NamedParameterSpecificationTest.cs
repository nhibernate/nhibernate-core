using NHibernate.Param;
using NUnit.Framework;

namespace NHibernate.Test.Parameters
{
	public class NamedParameterSpecificationTest
	{
		[Test]
		public void WhenHasSameNameThenSameHashCode()
		{
			var expected = (new NamedParameterSpecification(1, 0, "nhlist")).GetHashCode();
			Assert.That((new NamedParameterSpecification(1, 0, "nhlist")).GetHashCode(), Is.EqualTo(expected));
		}

		[Test]
		public void WhenHasNoSameNameThenNoSameHashCode()
		{
			var expected = (new NamedParameterSpecification(1, 0, "nHlist")).GetHashCode();
			Assert.That((new NamedParameterSpecification(1, 0, "nhlist")).GetHashCode(), Is.Not.EqualTo(expected));
		}

		[Test]
		public void WhenHasSameNameThenAreEquals()
		{
			var expected = (new NamedParameterSpecification(1, 0, "nhlist"));
			Assert.That((new NamedParameterSpecification(1, 0, "nhlist")), Is.EqualTo(expected));
		}

		[Test]
		public void WhenHasNoSameNameThenAreNotEquals()
		{
			var expected = (new NamedParameterSpecification(1, 0, "nHlist"));
			Assert.That((new NamedParameterSpecification(1, 0, "nhlist")), Is.Not.EqualTo(expected));
		}
	}
}