using System;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Persister.Collection;
using NHibernate.Type;

namespace NHibernate.Impl
{
	/// <summary>
	/// When a transient entity is passed to <see cref="ISession.Lock" />, we must inspect all its collections and
	/// 1. associate any uninitialized PersistentCollections with this session
	/// 2. associate any initialized PersistentCollections with this session, using the
	///    existing snapshot
	/// 3. throw an exception for each "new" collection
	/// </summary>
	internal class OnLockVisitor : ReattachVisitor
	{
		public OnLockVisitor(SessionImpl session, object key)
			: base( session, key )
		{
		}

		protected override object ProcessCollection(object collection, PersistentCollectionType type)
		{
			ICollectionPersister persister = Session.GetCollectionPersister( type.Role );

			if( collection == null )
			{
				// Do nothing
			}
			else if( collection is IPersistentCollection )
			{
				IPersistentCollection coll = (IPersistentCollection)collection;

				if( coll.SetCurrentSession( Session ) )
				{
					ICollectionSnapshot snapshot = coll.CollectionSnapshot;
					if( SessionImpl.IsOwnerUnchanged( snapshot, persister, this.Key ) )
					{
						// a "detached" collection that originally belonged to the same entity
						if( snapshot.Dirty )
						{
							throw new HibernateException( "reassociated object has dirty collection" );
						}
						Session.ReattachCollection( coll, snapshot );
					}
					else
					{
						// a "detached" collection that belonged to a different entity
						throw new HibernateException( "reassociated object has dirty collection reference" );
					}
				}
				else
				{
					// a collection loaded in the current session
					// can not possibly be the collection belonging
					// to the entity passed to update()
					throw new HibernateException( "reassociated object has dirty collection reference" );
				}
			}
			else
			{
				// brand new collection
				//TODO: or an array!! we can't lock objects with arrays now??
				throw new HibernateException( "reassociated object has dirty collection reference" );
			}

			return null;
		}

	}
}