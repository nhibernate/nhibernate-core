using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHibernate.Test.CacheTest
{
	public class ReadOnly : CacheEntity
	{
		public virtual string Name { get; set; }

		public virtual ISet<ReadOnlyItem> Items { get; set; } = new HashSet<ReadOnlyItem>();
	}

	public class ReadOnlyItem : CacheEntity
	{
		public virtual ReadOnly Parent { get; set; }
	}

	public abstract class CacheEntity
	{
		public virtual int Id { get; protected set; }
	}
}
