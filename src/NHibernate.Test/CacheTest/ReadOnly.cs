using System.Collections.Generic;

namespace NHibernate.Test.CacheTest
{
	public class ReadOnly : NamedCacheEntity
	{
		public virtual ISet<ReadOnlyItem> Items { get; set; } = new HashSet<ReadOnlyItem>();
	}

	public class ReadOnlyItem : CacheEntity
	{
		public virtual ReadOnly Parent { get; set; }
	}
}
