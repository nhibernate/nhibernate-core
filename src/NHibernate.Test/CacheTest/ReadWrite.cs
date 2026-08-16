using System.Collections.Generic;

namespace NHibernate.Test.CacheTest
{
	public class ReadWrite : NamedCacheEntity
	{
		public virtual ISet<ReadWriteItem> Items { get; set; } = new HashSet<ReadWriteItem>();
	}

	public class ReadWriteItem : CacheEntity
	{
		public virtual ReadWrite Parent { get; set; }
	}
}
