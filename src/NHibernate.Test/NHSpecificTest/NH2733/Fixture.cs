using System;
using System.Linq.Expressions;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2733
{
	public interface ISpecification
	{
		Expression Expression { get; }
	}

	public interface ISpecification<T> : ISpecification
	{
		Expression<Func<T, bool>> Predicate { get; }
	}

	public class Specification<T>
	{
		public Expression<Func<T, bool>> Predicate { get; protected set; }

		public Expression Expression
		{
			get
			{
				return Predicate;
			}
		}

		public Specification(Expression<Func<T, bool>> predicate)
		{
			Predicate = predicate;
		}
	}

	[TestFixture]
	public class Fixture : BugTestCase
	{
		Item Item = null;

		protected override void OnSetUp()
		{
			using (ISession session = Sfi.OpenSession())
			{
				var item = new Item();
				session.Persist(item);
				session.Flush();
			}
		}

		protected override void OnTearDown()
		{
			using (ISession session = Sfi.OpenSession())
			{
				session.CreateQuery("delete from Item").ExecuteUpdate();
				session.Flush();
			}
			base.OnTearDown();
		}

		public static Expression<Func<Item, bool>> GetExpression(DateTime startTime)
		{
			return item => item.Details.StartTime == startTime;
		}

		[Test]
		public void CanUseExpressionForWhere()
		{
			using (ISession session = Sfi.OpenSession())
			{
				IQueryOver<Item, Item> query = session.QueryOver(() => Item);

				var start = DateTime.UtcNow;

				query
					.Where(GetExpression(start));

				query.List();
			}
		}
	}
}
