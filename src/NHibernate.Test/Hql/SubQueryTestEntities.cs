using System.Collections.Generic;

namespace NHibernate.Test.Hql
{
	public class Root
	{
		public virtual int Id { get; set; }

		public virtual string RootName { get; set; }

		public virtual Branch Branch { get; set; }
	}

	public class Branch
	{
		public virtual int Id { get; set; }

		public virtual string BranchName { get; set; }

		public virtual IList<Leaf> Leafs { get; set; }
	}

	public class Leaf
	{
		public virtual int Id { get; set; }

		public virtual string LeafName { get; set; }
	}
}
