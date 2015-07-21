using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH1301
{
	public class ClassA
	{
		private int id;
		private IList<ClassB> bCollection = new List<ClassB>();

		public virtual int Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual IList<ClassB> BCollection
		{
			get { return bCollection; }
		}
	}

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
