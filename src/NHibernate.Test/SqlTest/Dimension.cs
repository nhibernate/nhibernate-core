using System;

namespace NHibernate.Test.SqlTest
{
	public class Dimension
	{
		private long id;
		private int length;
		private int width;

		public Dimension()
		{
		}

		public Dimension(int length, int width)
		{
			this.length = length;
			this.width = width;
		}

		public virtual long Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual int Length
		{
			get { return length; }
			set { length = value; }
		}

		public virtual int Width
		{
			get { return width; }
			set { width = value; }
		}
	}
}