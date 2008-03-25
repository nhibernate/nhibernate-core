using System;

namespace NHibernate.Test.NHSpecificTest.NH523
{
	public class ClassB
	{
		private int id;

		public virtual int Id
		{
			get { return id; }
			set { id = value; }
		}
	}
}