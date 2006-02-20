using System;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Persister.Collection;
using NHibernate.Type;

namespace NHibernate.Impl
{
	/// <summary>
	/// When an entity is passed to <c>Update()</c>, all its collections must be
	/// inspected and:
	/// <list type="number">
	///		<item>
	///			<description>
	///			Associate any uninitialized PersistentCollections with this Session.
	///			</description>
	///		</item>
	///		<item>
	///			<description>
	///			Associate any initialized PersistentCollections with this Session, using the
	///			existing snapshot.
	///			</description>
	///		</item>
	///		<item>
	///			<description>
	///			Execute a collection removal (SQL DELETE) for each null collection property
	///			or "new" collection.
	///			</description>
	///		</item>
	/// </list>
	/// </summary>
	internal class OnUpdateVisitor : ReattachVisitor
	{
		public OnUpdateVisitor(SessionImpl session, object key)
			: base( session, key )
		{
		}

		protected override object ProcessCollection(object collection, PersistentCollectionType type)
		{
			ICollectionPersister persister = Session.GetCollectionPersister( type.Role );

			if( collection is IPersistentCollection )
			{
				IPersistentCollection wrapper = (IPersistentCollection)collection;

				if( wrapper.SetCurrentSession( Session ) )
				{
					//a "detached" collection!
					ICollectionSnapshot snapshot = wrapper.CollectionSnapshot;

					if( !SessionImpl.IsOwnerUnchanged( snapshot, persister, Key ) )
					{
						// if the collection belonged to a different entity,
						// clean up the existing state of the collection
						Session.RemoveCollection( persister, Key );
					}

					Session.ReattachCollection( wrapper, snapshot );
				}
				else
				{
					// a collection loaded in the current session
					// can not possibly be the collection belonging
					// to the entity passed to update()
					Session.RemoveCollection( persister, Key );
				}
			}
			else
			{
				// null or brand new collection
				// this will also (inefficiently) handle arrays, which have
				// no snapshot, so we can't do any better
				Session.RemoveCollection( persister, Key );
				//processArrayOrNewCollection(collection, type);
			}

			return null;
		}
	}
}