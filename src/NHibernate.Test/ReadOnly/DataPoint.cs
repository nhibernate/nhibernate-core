using System;

namespace NHibernate.Test.ReadOnly
{
	[Serializable]
	public class DataPoint
	{
		private long id;
		private decimal x;
		private decimal y;
		private string description;
		
		public virtual long Id
		{
			get { return id; }
			set { id = value; }
		}
		
		public virtual decimal X
		{
			get { return x; }
			set { x = value; }
		}
		
		public virtual decimal Y
		{
			get { return y; }
			set { y = value; }
		}
		
		public virtual string Description
		{
			get { return description; }
			set { description = value; }
		}
		
		public DataPoint()
		{
		}
		
		public DataPoint(decimal x, decimal y, string description)
		{
			this.x = x;
			this.y = y;
			this.description = description;
		}
	}
}
