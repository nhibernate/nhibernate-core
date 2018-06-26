using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Engine;

namespace NHibernate.Cache.Entry
{
	public class StructuredCollectionCacheEntry : ICacheEntryStructure
	{
		public virtual object Structure(object item)
		{
			var entry = (CollectionCacheEntry)item;
			return new List<object>((object[])entry.DisassembledState);
		}

		public virtual object Destructure(object item, ISessionFactoryImplementor factory)
		{
			var collection = item as IEnumerable;
			var objects = collection != null
							  ? collection.Cast<object>().ToArray()
							  : Array.Empty<object>();
			return new CollectionCacheEntry {DisassembledState = objects};
		}
	}
}
