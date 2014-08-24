using System.Collections.Generic;
using NUnit.Framework;
using NHibernate.Criterion;

namespace NHibernate.Test.NHSpecificTest.NH2546
{
	[TestFixture]
	public class SetCommandParameterSizesFalseFixture : BugTestCase
	{
		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return dialect is Dialect.MsSql2008Dialect;
		}

		protected override void OnSetUp()
		{
			using (ISession session = Sfi.OpenSession())
			{
				session.Persist(new Student() { StringTypeWithLengthDefined = "Julian Maughan" });
				session.Persist(new Student() { StringTypeWithLengthDefined = "Bill Clinton" });
				session.Flush();
			}
		}
		
		protected override void OnTearDown()
		{
			using (ISession session = Sfi.OpenSession())
			{
				session.CreateQuery("delete from Student").ExecuteUpdate();
				session.Flush();
			}
			base.OnTearDown();
		}

		[Test]
		public void LikeExpressionWithinDefinedTypeSize()
		{
			using (ISession session = Sfi.OpenSession())
			{
				ICriteria criteria = session
					.CreateCriteria<Student>()
					.Add(Restrictions.Like("StringTypeWithLengthDefined", "Julian%"));
				
				IList<Student> list = criteria.List<Student>();
				
				Assert.That(list.Count, Is.EqualTo(1));
			}
		}
		
		[Test]
		public void LikeExpressionExceedsDefinedTypeSize()
		{
			// In this case we are forcing the usage of LikeExpression class where the length of the associated property is ignored
			using (ISession session = Sfi.OpenSession())
			{
				ICriteria criteria = session
					.CreateCriteria<Student>()
					.Add(Restrictions.Like("StringTypeWithLengthDefined", "[a-z][a-z][a-z]ian%", MatchMode.Exact, null));
				
				IList<Student> list = criteria.List<Student>();
				
				Assert.That(list.Count, Is.EqualTo(1));
			}
		}
	}
}
