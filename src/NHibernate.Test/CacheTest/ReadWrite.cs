using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHibernate.Test.CacheTest
{
	public class ReadWrite : CacheEntity
	{
		public virtual string Name { get; set; }

		public virtual ISet<ReadWriteItem> Items { get; set; } = new HashSet<ReadWriteItem>();
	}

	public class ReadWriteItem : CacheEntity
	{
		public virtual ReadWrite Parent { get; set; }
	}
}
