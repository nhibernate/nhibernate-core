using System;

namespace NHibernate.Test.NHSpecificTest.NH607
{
	public abstract class PersistentObject
	{
		private int id;

		public PersistentObject()
		{
			this.id = 0;
		}

		public virtual int Id
		{
			get { return id; }
			set { id = value; }
		}
	}
}