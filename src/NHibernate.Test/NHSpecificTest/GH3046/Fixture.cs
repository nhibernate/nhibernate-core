using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH3046
{
	// Only the mapping needs a change for this test case. It is fetched by the fixture namespace, so it will use the mapping defined here.
	[TestFixture]
	public class Fixture : GH2549.Fixture
	{
	}
}
