using System;

namespace NHibernate.Test.NHSpecificTest.NH898
{
	public class ClassB : ClassBParent
	{
		private ClassA a;
		public virtual ClassA A
		{
			get { return a; }
			set { a = value; }
		}
	}
}
