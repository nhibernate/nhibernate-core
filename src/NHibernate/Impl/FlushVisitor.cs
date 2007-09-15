using System;
using log4net;
using NHibernate.Collection;
using NHibernate.Type;

namespace NHibernate.Impl
{
	internal class FlushVisitor : AbstractVisitor
	{
		private object _owner;
		private static readonly ILog log = LogManager.GetLogger(typeof(AbstractVisitor));

		public FlushVisitor(SessionImpl session, object owner)
			: base(session)
		{
			_owner = owner;
		}

		protected override object ProcessCollection(object collection, CollectionType type)
		{
			if (log.IsDebugEnabled)
			{
				log.Debug(string.Format("Processing collection for role {0}", type.Role));
			}

			if (collection != null)
			{
				IPersistentCollection coll;
				if (type.IsArrayType)
				{
					coll = Session.GetCollectionHolder(collection);
				}
				else
				{
					coll = (IPersistentCollection) collection;
				}
				Session.UpdateReachableCollection(coll, type, _owner);
			}
			return null;
		}
	}
}