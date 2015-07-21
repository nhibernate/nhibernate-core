using System;

namespace NHibernate.Test.NHSpecificTest.NH898
{
	public abstract class ClassBParent
	{
		private int id;
		public virtual int Id
		{
			get { return id; }
			set { id = value; }
		}
	}
}
