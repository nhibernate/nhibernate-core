using System;
using NHibernate.Collection;
using NHibernate.Type;

namespace NHibernate.Impl
{
	internal class FlushVisitor : AbstractVisitor
	{
		private object _owner;

		public FlushVisitor(SessionImpl session, object owner)
			: base( session )
		{
			_owner = owner;
		}

		protected override object ProcessCollection(object collection, PersistentCollectionType type)
		{
			if( collection != null )
			{
				PersistentCollection coll;
				if( type.IsArrayType )
				{
					coll = Session.GetArrayHolder( collection );
				}
				else
				{
					coll = (PersistentCollection)collection;
				}
				Session.UpdateReachableCollection( coll, type, _owner );
			}
			return null;
		}
	}
}