using System;

namespace NHibernate.Test.NHSpecificTest.NH898
{
	public class ClassA
	{
		private int id;
		private ClassB b;
		public virtual int Id
		{
			get { return id; }
			set { id = value; }
		}
		public virtual ClassB B
		{
			get { return b; }
			set { b = value; }
		}
	}
}
