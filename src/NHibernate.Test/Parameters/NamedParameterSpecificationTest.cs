using NHibernate.Param;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.Parameters
{
	public class NamedParameterSpecificationTest
	{
		[Test]
		public void WhenHasSameNameThenSameHashCode()
		{
			var expected = (new NamedParameterSpecification(1, 0, "nhlist")).GetHashCode();
			(new NamedParameterSpecification(1, 0, "nhlist")).GetHashCode().Should().Be.EqualTo(expected);
		}

		[Test]
		public void WhenHasNoSameNameThenNoSameHashCode()
		{
			var expected = (new NamedParameterSpecification(1, 0, "nHlist")).GetHashCode();
			(new NamedParameterSpecification(1, 0, "nhlist")).GetHashCode().Should().Not.Be.EqualTo(expected);
		}

		[Test]
		public void WhenHasSameNameThenAreEquals()
		{
			var expected = (new NamedParameterSpecification(1, 0, "nhlist"));
			(new NamedParameterSpecification(1, 0, "nhlist")).Should().Be.EqualTo(expected);
		}

		[Test]
		public void WhenHasNoSameNameThenAreNotEquals()
		{
			var expected = (new NamedParameterSpecification(1, 0, "nHlist"));
			(new NamedParameterSpecification(1, 0, "nhlist")).Should().Not.Be.EqualTo(expected);
		}
	}
}