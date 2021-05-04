using System.Collections.Generic;

namespace NHibernate.Test.SecondLevelCacheTests
{
	public class NeverItem
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
		public virtual string Description { get; set; }

		public virtual IList<NeverChildItem> Childrens { get; set; } = new List<NeverChildItem>();
	}

	public class NeverChildItem
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }

		public virtual NeverItem Parent { get; set; }
	}
}
