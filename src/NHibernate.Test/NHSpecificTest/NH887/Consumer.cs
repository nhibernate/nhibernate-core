using System;

namespace NHibernate.Test.NHSpecificTest.NH887
{
	public class Consumer
	{
		private int id;
		private Child child;

		public virtual int Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual Child Child
		{
			get { return child; }
			set { child = value; }
		}
	}
}