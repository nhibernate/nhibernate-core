using System.Linq;
using NUnit.Framework;

namespace NHibernate.Test.Linq
{
	[TestFixture]
	public class BinaryExpressionOrdererTests : LinqTestCase
	{
		[Test]
		public void ValuePropertySwapsToPropertyValue()
		{
			var query = (from user in db.Users
						 where ("ayende" == user.Name)
						 select user).ToList();
			Assert.AreEqual(1, query.Count);
		}

		[Test]
		public void PropertyValueDoesntSwaps()
		{
            var query = (from user in db.Users
						 where (user.Name == "ayende")
						 select user).ToList();
			Assert.AreEqual(1, query.Count);
		}

		[Test]
		public void PropertyPropertyDoesntSwap()
		{
            var query = (from user in db.Users
						 where (user.Name == user.Name)
						 select user).ToList();
			Assert.AreEqual(3, query.Count);
		}

        [Test]
		public void EqualsSwapsToEquals()
		{
            var query = (from user in db.Users
						 where ("ayende" == user.Name)
						 select user).ToList();
			Assert.AreEqual(1, query.Count);
		}

		[Test]
		public void NotEqualsSwapsToNotEquals()
		{
            var query = (from user in db.Users
						 where ("ayende" != user.Name)
						 select user).ToList();
			Assert.AreEqual(2, query.Count);
		}

		[Test]
		public void GreaterThanSwapsToLessThan()
		{
            var query = (from user in db.Users
						 where (3 > user.Id)
						 select user).ToList();
			Assert.AreEqual(2, query.Count);
		}

		[Test]
		public void GreaterThanOrEqualToSwapsToLessThanOrEqualTo()
		{
            var query = (from user in db.Users
						 where (2 >= user.Id)
						 select user).ToList();
			Assert.AreEqual(2, query.Count);
		}

		[Test]
		public void LessThanSwapsToGreaterThan()
		{
            var query = (from user in db.Users
						 where (1 < user.Id)
						 select user).ToList();
			Assert.AreEqual(2, query.Count);
		}

		[Test]
		public void LessThanOrEqualToSwapsToGreaterThanOrEqualTo()
		{
            var query = (from user in db.Users
						 where (2 <= user.Id)
						 select user).ToList();
			Assert.AreEqual(2, query.Count);
		}

		[Test]
		public void ValuePropertySwapsToPropertyValueUsingEqualsFromConstant()
		{
			// check NH-2440
			var query = (from user in db.Users
									 where ("ayende".Equals(user.Name))
									 select user).ToList();
			Assert.AreEqual(1, query.Count);
		}

		[Test]
		public void ValuePropertySwapsToPropertyValueUsingEqualsToConstant()
		{
			// check NH-2440
			var query = (from user in db.Users
									 where (user.Name.Equals("ayende"))
									 select user).ToList();
			Assert.AreEqual(1, query.Count);
		}
	}
}