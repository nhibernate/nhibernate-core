using System;

namespace NHibernate.Test.NHSpecificTest.NH980
{
	public class IdOnly
	{
		private int id;

		public virtual int Id
		{
			get { return id; }
			set { id = value; }
		}
	}
}
