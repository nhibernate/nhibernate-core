using NHibernate.Criterion;
using NUnit.Framework;

namespace NHibernate.Test.ExpressionTest
{
	[TestFixture]
	public class RestrictionsFixture
	{
		[Test]
		public void LikeShouldContainsMatch()
		{
			ICriterion c = Restrictions.Like("Name", "n", MatchMode.Anywhere, null);
			Assert.That(c, Is.InstanceOf<LikeExpression>());
			var likeExpression = (LikeExpression) c;
			Assert.That(likeExpression.ToString(), Is.EqualTo("Name like %n%"));
		}
	}
}