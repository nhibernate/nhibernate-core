using System;
using System.Collections;

using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.Impl
{
	internal class OnUpdateVisitor : ReattachVisitor
	{
		public OnUpdateVisitor(SessionImpl session, object key)
			: base (session, key)
		{
		}

		protected override object ProcessCollection(object collection, PersistentCollectionType type)
		{
			CollectionPersister persister = Session.GetCollectionPersister( type.Role );
		
			if ( collection is PersistentCollection ) 
			{
				PersistentCollection wrapper = (PersistentCollection) collection;

				if ( wrapper.SetCurrentSession(Session) ) 
				{
					//a "detached" collection!
					ICollectionSnapshot snapshot = wrapper.CollectionSnapshot;
				
					if ( !SessionImpl.IsOwnerUnchanged(snapshot, persister, Key) ) 
					{
						// if the collection belonged to a different entity,
						// clean up the existing state of the collection
						Session.RemoveCollection(persister, Key);
					}
				
					Session.ReattachCollection(wrapper, snapshot);
				}
				else 
				{
					// a collection loaded in the current session
					// can not possibly be the collection belonging
					// to the entity passed to update()
					Session.RemoveCollection(persister, Key);
				}
			}
			else 
			{
				// null or brand new collection
				// this will also (inefficiently) handle arrays, which have
				// no snapshot, so we can't do any better
				Session.RemoveCollection(persister, Key);
				//processArrayOrNewCollection(collection, type);
			}

			return null;
		}
	}
}
