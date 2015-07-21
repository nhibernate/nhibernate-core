using NHibernate.Criterion;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH637
{
	public class Point
	{
		private int x, y;

		public Point()
		{
		}

		public Point(int x, int y)
		{
			this.x = x;
			this.y = y;
		}

		public int X
		{
			get { return x; }
			set { x = value; }
		}

		public int Y
		{
			get { return y; }
			set { y = value; }
		}
	}

	public class PointHolder
	{
		private int id;
		private Point point;

		public virtual int Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual Point Point
		{
			get { return point; }
			set { point = value; }
		}
	}

	[TestFixture]
	public class Fixture : BugTestCase
	{
		[Test]
		public void MultiColumnBetween()
		{
			PointHolder holder = new PointHolder();
			holder.Point = new Point(20, 10);

			using (ISession s = OpenSession())
			using(ITransaction t = s.BeginTransaction())
			{
				s.Save(holder);

				PointHolder result = (PointHolder) s
				                                   	.CreateCriteria(typeof(PointHolder))
				                                   	.Add(Expression.Between("Point", new Point(19, 9), new Point(21, 11)))
				                                   	.UniqueResult();

				Assert.AreSame(holder, result);

				s.Delete(holder);
				t.Commit();
			}
		}
	}
}