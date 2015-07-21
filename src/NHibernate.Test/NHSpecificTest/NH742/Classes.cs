using System;

namespace NHibernate.Test.NHSpecificTest.NH742
{
	public class Point
	{
		private int x, y;

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

	public class SomeClass
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
}