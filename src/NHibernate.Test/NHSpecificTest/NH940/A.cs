using System;

namespace NHibernate.Test.NHSpecificTest.NH940
{
	public class A
	{
		private long id;
		private B b;

		public virtual long Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual B B
		{
			get { return b; }
			set { b = value; }
		}

		public virtual void Execute()
		{
			B.Execute();
		}
	}
}