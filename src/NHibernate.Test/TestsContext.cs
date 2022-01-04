using NUnit.Framework;

namespace NHibernate.Test
{
	[SetUpFixture]
	public class TestsContext : TestsContextBase
	{
		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			//Everything is done in TestsContextBase static ctor 
		}
	}
}
