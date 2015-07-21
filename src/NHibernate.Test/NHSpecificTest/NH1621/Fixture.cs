using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1621
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		ISession session;

		public override string BugNumber
		{
			get { return "NH1621"; }
		}

		[Test]
		public void QueryUsingReadonlyProperty()
		{
			using (session = OpenSession())
			{
				Nums nums1 = new Nums {ID = 1, NumA = 1, NumB = 2};
				session.Save(nums1);

				Nums nums2 = new Nums {ID = 2, NumA = 2, NumB = 2 };
				session.Save(nums2);

				Nums nums3 = new Nums {ID = 3, NumA = 5, NumB = 2 };
				session.Save(nums3);

				session.Flush();
				session.Clear();

				var nums = session.CreateQuery("from Nums b where b.Sum > 4").List<Nums>();

				Assert.That(nums.Count, Is.EqualTo(1));
				Assert.That(nums[0].Sum, Is.EqualTo(7));

				session.Delete("from Nums");
				session.Flush();
				session.Close();
			}
		}
	}
}
