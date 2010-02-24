using System.Linq;
using NUnit.Framework;

namespace NHibernate.Test.Linq
{
	[TestFixture]
	public class MethodCallTests : LinqTestCase
	{
		[Test]
		public void CanExecuteAny()
		{
			var result = db.Users.Any();
			Assert.IsTrue(result);
		}

		[Test]
        public void CanExecuteAnyWithArguments()
		{
			var result = db.Users.Any(u => u.Name == "user-does-not-exist");
			Assert.IsFalse(result);
		}

		[Test]
		public void CanExecuteCountWithOrderByArguments()
		{
			var query = db.Users.OrderBy(u => u.Name);
			var count = query.Count();
			Assert.AreEqual(3, count);
		}
	}
}
