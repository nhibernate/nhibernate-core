using System;
using System.Collections;
using System.Data;

using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.SqlTypes;

namespace NHibernate.Type 
{
	/// <summary>
	/// PersistentCollectionType.
	/// </summary>
	public abstract class PersistentCollectionType : AbstractType, IAssociationType	
	{
		
		private readonly string role;
		private static readonly SqlType[] NoSqlTypes = {};

		protected PersistentCollectionType(string role) 
		{
			this.role = role;
		}

		public virtual string Role 
		{
			get { return role; }
		}

		public override bool IsPersistentCollectionType 
		{
			get { return true; }
		}

		public override sealed bool Equals(object x, object y) 
		{
            // proxies? - comment in h2.0.3 also
			return x==y;
		}

		public abstract PersistentCollection Instantiate(ISessionImplementor session, CollectionPersister persister);

		public override object NullSafeGet(IDataReader rs, string name, ISessionImplementor session, object owner) 
		{
			throw new AssertionFailure("bug in PersistentCollectionType");
		}
	
		public override object NullSafeGet(IDataReader rs, string[] name, ISessionImplementor session, object owner) 
		{
			return ResolveIdentifier( Hydrate(rs, name, session, owner), session, owner );
		}

		public override void NullSafeSet(IDbCommand cmd, object value, int index, ISessionImplementor session) 
		{
		}

		public virtual object GetCollection(object id, object owner, ISessionImplementor session) 
		{
			// added the owner
			PersistentCollection collection = session.GetLoadingCollection( role, id );
			if(collection!=null) return collection.GetCachedValue(); //TODO: yuck... call another method - H2.0.3comment

			CollectionPersister persister = session.Factory.GetCollectionPersister(role);
			collection = persister.GetCachedCollection(id, owner, session);
			if(collection!=null) 
			{
				session.AddInitializedCollection(collection, persister, id);
				return collection.GetCachedValue();
			}
			else 
			{
				collection = Instantiate(session, persister);
				session.AddUninitializedCollection(collection, persister, id);
				return collection.GetInitialValue(persister.IsLazy);
			}

		}

		public override SqlType[] SqlTypes(IMapping session) 
		{
			return NoSqlTypes;
		}
	
		public override int GetColumnSpan(IMapping session) 
		{
			return 0;
		}	
	
		public override string ToXML(object value, ISessionFactoryImplementor factory) 
		{
			return (value==null) ? null : value.ToString();
		}
	
		public override object DeepCopy(object value) 
		{
			return value;
		}
	
		public override string Name 
		{
			get { return ReturnedClass.Name; }
		}

		
		/// <summary>
		/// Returns a reference to the elements in the collection.  
		/// </summary>
		/// <param name="collection">The object that holds the ICollection.</param>
		/// <returns>An ICollection of the Elements(classes) in the Collection.</returns>
		/// <remarks>
		/// By default the parameter <c>collection</c> is just cast to an ICollection.  Collections
		/// such as Maps and Sets should override this so that the Elements are returned - not a
		/// DictionaryEntry.
		/// </remarks>
		public virtual ICollection GetElementsCollection(object collection) 
		{
			return ( (ICollection)collection );
		}
	
		public override bool IsMutable 
		{
			get { return false; }
		}
	
		public override object Disassemble(object value, ISessionImplementor session) {
			return null;
			// commented out in h2.0.3 also
//			if (value==null) {
//				return null;
//			}
//			else {
//				object id = session.GetLoadedCollectionKey( (PersistentCollection) value );
//				if (id==null)
//					throw new AssertionFailure("Null collection id");
//				return id;
//			}
		}

		public override object Assemble(object cached, ISessionImplementor session, object owner) 
		{
			object id = session.GetEntityIdentifier(owner);
			if(id==null) 
			{
				throw new AssertionFailure("bug re-assembling collection reference");
			}
			return ResolveIdentifier(id, session, owner);
		}
	
		public override bool IsDirty(object old, object current, ISessionImplementor session) 
		{		
			System.Type ownerClass = session.Factory.GetCollectionPersister(role).OwnerClass;
		
			if ( !session.Factory.GetPersister(ownerClass).IsVersioned ) 
			{
				// collections don't dirty an unversioned parent entity
				return false;
			}
			else 
			{
				return base.IsDirty(old, current, session);
			}
		}

		public override bool HasNiceEquals 
		{
			get { return false; }
		}
	
		public abstract PersistentCollection Wrap(ISessionImplementor session, object collection);
	
		/**
		 * Note: return true because this type is castable to IAssociationType. Not because
		 * all collections are associations.
		 */
		public override bool IsAssociationType 
		{
			get { return true; }
		}
	
		public virtual ForeignKeyType ForeignKeyType 
		{
			get { return ForeignKeyType.ForeignKeyToParent;	}
		}
	
		public override object Hydrate(IDataReader rs, string[] name, ISessionImplementor session, object owner) 
		{
			return session.GetEntityIdentifier(owner);
		}
	
		public override object ResolveIdentifier(object value, ISessionImplementor session, object owner) 
		{
			if (value==null) 
			{
				return null;
			}
			else 
			{
				// h2.1 changed this to use sesion.GetCollection( role, value, owner ) and 
				// move the impl of GetCollection from this class to the ISession.
				return GetCollection( value, owner, session);
			}
		}
	
		public virtual bool IsArrayType 
		{
			get { return false; }
		}
	
		public abstract PersistentCollection AssembleCachedCollection(ISessionImplementor session, CollectionPersister persister, object disassembled, object owner);
	}
}