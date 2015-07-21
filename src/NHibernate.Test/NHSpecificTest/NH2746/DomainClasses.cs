using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH2746
{
	public class T1
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
		public virtual ISet<T2> Children { get; protected set; }

		public T1()
		{
			this.Children = new HashSet<T2>();
		}
	}

	public class T2
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
		public virtual T1 Parent { get; set; }

		public T2()
		{
		}
	}
}
