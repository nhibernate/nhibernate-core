using System;
using System.Collections;
using System.Data;

using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Sql;
using NHibernate.SqlTypes;

namespace NHibernate.Type {

	/// <summary>
	/// A specific PersistentCollectionType for a Role.
	/// </summary>
	public abstract class PersistentCollectionType : AbstractType, IAssociationType	{
		
		private readonly string role;
		private static readonly SqlType[] NoSqlTypes = {};

		public PersistentCollectionType(string role) {
			this.role = role;
		}

		public virtual string Role {
			get { return role; }
		}

		public override bool IsPersistentCollectionType {
			get { return true; }
		}

		public override sealed bool Equals(object x, object y) {
            // proxies?
			return x==y;
		}

		public abstract PersistentCollection Instantiate(ISessionImplementor session, CollectionPersister persister);

		public override object NullSafeGet(IDataReader rs, string name, ISessionImplementor session, object owner) {
			throw new AssertionFailure("bug in PersistentCollectionType");
		}
	
		/// <summary>
		/// Returns a fully initialized Collection - be careful when calling this because it might
		/// open another DataReader!!!
		/// </summary>
		/// <param name="rs"></param>
		/// <param name="name"></param>
		/// <param name="session"></param>
		/// <param name="owner"></param>
		/// <returns></returns>
		public override object NullSafeGet(IDataReader rs, string[] name, ISessionImplementor session, object owner) {
			return ResolveIdentifier( Hydrate(rs, name, session, owner), session, owner );
		}

		/// <summary>
		/// Gets a Collection without opening a DataReader.  
		/// </summary>
		/// <param name="rs"></param>
		/// <param name="name"></param>
		/// <param name="session"></param>
		/// <param name="owner"></param>
		/// <param name="partOfComponent"></param>
		/// <returns>
		/// The Collection returned from here is a lazy-load Collection regardless of what the map says.  To
		/// load this Collection partOfComponent must be true and then ResolveIdentifier must be called.  This 
		/// method is only intended to be used by ComponentType to solve the problem of Getting a Collection
		/// opens a second DataReader.
		/// </returns>
		public object NullSafeGet(IDataReader rs, string[] name, ISessionImplementor session, object owner, bool partOfComponent) 
		{
			object id = session.GetEntityIdentifier(owner);
			PersistentCollection collection = session.GetLoadingCollection(role, id);
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
				
				// hard coding in lazy here because we don't want it to load during it's Get - just
				// initialize the Collection class as if it is being lazy loaded - we'll get back to
				// loading it during ResolveIdentifier...
				collection.GetInitialValue(true);

				// if we get to here then we have just created a lazy loaded (ie - uninitialized )Collection that might 
				// be a part of a Component.  If it is part of a component then we need to mark it as
				// needing to be a part of the batch that gets ResolveIdentifier called where it will be Initialized
				// according to the IsLazy property of the Persister
				if(partOfComponent) 
				{
					session.AddUnresolvedComponentCollection(id, role, collection);
				}

				return collection;
			}
		}

		public virtual object GetCollection(object id, object owner, ISessionImplementor session) {
			PersistentCollection collection = session.GetLoadingCollection(role, id);
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

		public override void NullSafeSet(IDbCommand cmd, object value, int index, ISessionImplementor session) {
		}

		public override SqlType[] SqlTypes(IMapping session) {
			return NoSqlTypes;
		}
	
		public override int GetColumnSpan(IMapping session) {
			return 0;
		}	
	
		public override string ToXML(object value, ISessionFactoryImplementor factory) {
			return (value==null) ? null : value.ToString();
		}
	
		public override object DeepCopy(object value) {
			return value;
		}
	
		public override string Name {
			get { return ReturnedClass.Name; }
		}

		//Is it correct?
		//Was:
		//public Iterator getElementsIterator(Object collection) {
		//	return ( (java.util.Collection) collection ).iterator();
		//}
		public ICollection GetElementsCollection(object collection) {
			return ( (ICollection )collection );
		}
	
		public override bool IsMutable {
			get { return false; }
		}
	
		public override object Disassemble(object value, ISessionImplementor session) {
			return null;
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

		public override object Assemble(object cached, ISessionImplementor session, object owner) {
			object id = session.GetEntityIdentifier(owner);
			if(id==null) throw new AssertionFailure("bug re-assembling collection reference");
			return ResolveIdentifier(id, session, owner);
			//return ResolveIdentifier(cached, session, owner);
		}
	
		public override bool IsDirty(object old, object current, ISessionImplementor session) {
		
			System.Type ownerClass = session.Factory.GetCollectionPersister(role).OwnerClass;
		
			if ( !session.Factory.GetPersister(ownerClass).IsVersioned ) {
				// collections don't dirty an unversioned parent entity
				return false;
			}
			else {
				return base.IsDirty(old, current, session);
			}
		}

		public override bool HasNiceEquals {
			get { return false; }
		}
	
		public abstract PersistentCollection Wrap(ISessionImplementor session, object collection);
	
		/**
		 * Note: return true because this type is castable to IAssociationType. Not because
		 * all collections are associations.
		 */
		public override bool IsAssociationType {
			get { return true; }
		}
	
		public virtual ForeignKeyType ForeignKeyType {
			get { return ForeignKeyType.ForeignKeyToParent;	}
		}
	
		public override object Hydrate(IDataReader rs, string[] name, ISessionImplementor session, object owner) {
			return session.GetEntityIdentifier(owner);
		}
	
		/// <summary>
		/// 
		/// </summary>
		/// <param name="value">The id of the owner.</param>
		/// <param name="session"></param>
		/// <param name="owner"></param>
		/// <returns></returns>
		/// <remarks>
		/// </remarks>
		public override object ResolveIdentifier(object value, ISessionImplementor session, object owner) {
			if (value==null) {
				return null;
			}
			else {
				return GetCollection( value, owner, session);
			}
		}
	
		/// <summary>
		/// Resolves Collection that might be part of a Component.
		/// </summary>
		/// <param name="value">The id of the owner.</param>
		/// <param name="session">The current Session.</param>
		/// <param name="owner">The owner of the collection.</param>
		/// <param name="partOfComponent">Indicates if this Collection is a part of a Component.</param>
		/// <returns>A fully initialized collection according to its Persister's IsLazy property.</returns>
		public object ResolveIdentifier(object value, ISessionImplementor session, object owner, bool partOfComponent) 
		{
			if(partOfComponent==false) return ResolveIdentifier(value, session, owner);
			
			// check to see if this Collection is part of a Component and it has already been Instantiated
			// and just needs to GetInitialValue because it has already been Instantiated and Added to the 
			// Session in NullSafeGet
			object id = value;
			
			PersistentCollection collection = session.GetUnresolvedComponentCollection(id, role);
			
			if(collection==null) 
			{
				// when the collection is null that means it is not an UnresolvedComponentCollection
				// so we can let ResolveIdentifier get it however it needs to.
				return ResolveIdentifier(id, session, owner);
			}
			else 
			{
				// we already have a collection 
				CollectionPersister persister = session.Factory.GetCollectionPersister(role);
				collection.GetInitialValue(persister.IsLazy);
				
				// we have resolved the Collection in the Component so remove it from the unresolved
				session.RemoveUnresolvedComponentCollection(id, role);

				return collection;
			}

			
			
		}

		public virtual bool IsArrayType {
			get { return false; }
		}
	
		public abstract PersistentCollection AssembleCachedCollection(ISessionImplementor session, CollectionPersister persister, object disassembled, object owner);
	}
}