using System.Linq;
using NUnit.Framework;

namespace NHibernate.Test.Linq
{
	[TestFixture]
    [Ignore]
	public class MethodCallTests : LinqTestCase
	{
		[Test]
		public void CanExecuteAny()
		{
			bool result = db.Users.Any();
			Assert.IsTrue(result);
		}

		[Test]
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
