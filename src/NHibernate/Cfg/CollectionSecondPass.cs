using System;
using System.Collections;
using log4net;
using NHibernate.Mapping;

namespace NHibernate.Cfg
{
	public abstract class CollectionSecondPass : ISecondPass
	{
		private static ILog log = LogManager.GetLogger(typeof(CollectionSecondPass));
		protected Mappings mappings;
		protected Mapping.Collection collection;

		public CollectionSecondPass(Mappings mappings, Mapping.Collection collection)
		{
			this.collection = collection;
			this.mappings = mappings;
		}

		public void DoSecondPass(IDictionary persistentClasses)
		{
			if (log.IsDebugEnabled)
			{
				log.Debug("Second pass for collection: " + collection.Role);
			}
			SecondPass(persistentClasses);
			collection.CreateAllKeys();

			if (log.IsDebugEnabled)
			{
				string msg = "Mapped collection key: " + HbmBinder.Columns(collection.Key);
				if (collection.IsIndexed)
				{
					msg += ", index: " + HbmBinder.Columns(((IndexedCollection) collection).Index);
				}
				if (collection.IsOneToMany)
				{
					msg += ", one-to-many: " + collection.Element.Type.Name;
				}
				else
				{
					msg += ", element: " + HbmBinder.Columns(collection.Element);
					msg += ", type: " + collection.Element.Type.Name;
				}
				log.Debug(msg);
			}
		}

		public abstract void SecondPass(IDictionary persistentClasses);
	}
}