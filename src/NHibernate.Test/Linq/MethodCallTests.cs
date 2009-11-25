using System.Linq;
using NUnit.Framework;

namespace NHibernate.Test.Linq
{
	[TestFixture]
	public class MethodCallTests : LinqTestCase
	{
		[Test]
        [Ignore("Waiting for re-linq support")]
		public void CanExecuteAny()
		{
			bool result = db.Users.Any();
			Assert.IsTrue(result);
		}

		[Test]
        [Ignore("Waiting for re-linq support")]
        public void CanExecuteAnyWithArguments()
		{
			bool result = db.Users.Any(u => u.Name == "user-does-not-exist");
			Assert.IsFalse(result);
		}

		[Test]
		public void CanExecuteCountWithOrderByArguments()
		{
			var query = db.Users.OrderBy(u => u.Name);
			int count = query.Count();
			Assert.AreEqual(3, count);
		}
	}
}
