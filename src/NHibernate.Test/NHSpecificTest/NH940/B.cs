using System;

namespace NHibernate.Test.NHSpecificTest.NH940
{
	public class B
	{
		private long id;

		public virtual long Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual void Execute()
		{
			throw new MyException();
		}
	}
}