using System;
using System.Collections;

using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Type;

using log4net;

namespace NHibernate.Impl
{
	internal class WrapVisitor : ProxyVisitor
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(WrapVisitor));

		private bool substitute = false;

		public bool IsSubstitutionRequired
		{
			get { return substitute; }
		}

		public WrapVisitor(SessionImpl session) : base (session) { }

		protected override object ProcessCollection(object collection, PersistentCollectionType collectionType)
		{
			if ( collection is PersistentCollection )
			{
				PersistentCollection coll = (PersistentCollection) collection;
				if ( coll.SetCurrentSession(Session) ) 
				{
					Session.ReattachCollection( coll, coll.CollectionSnapshot );
				}
				return null;

			}
			else 
			{
				return ProcessArrayOrNewCollection(collection, collectionType);
			}
		}

		private object ProcessArrayOrNewCollection(object collection, PersistentCollectionType collectionType)
		{
			if (collection == null) return null;

			CollectionPersister persister = Session.GetCollectionPersister( collectionType.Role );
			
			if ( collectionType.IsArrayType ) 
			{
				ArrayHolder ah = Session.GetArrayHolder(collection);
				if (ah == null)
				{
					ah = new ArrayHolder(Session, collection);
					Session.AddNewCollection(ah, persister);
					Session.AddArrayHolder(ah);
				}
				return null;
			}
			else 
			{
				PersistentCollection persistentCollection = collectionType.Wrap(Session, collection);
				Session.AddNewCollection(persistentCollection, persister);
				
				if ( log.IsDebugEnabled ) log.Debug( "Wrapped collection in role: " + collectionType.Role );
				
				return persistentCollection; //Force a substitution!
			}
		}

		public override void ProcessValues(object[] values, IType[] types)
		{
			for (int i = 0; i < types.Length; i++)
			{
				object result = ProcessValue( values[i], types[i] );
				if ( result != null )
				{
					substitute = true;
					values[i] = result;
				}
			}
		}

		protected override object ProcessComponent(object component, IAbstractComponentType componentType)
		{
			if (component == null) return null;

			object[] values = componentType.GetPropertyValues( component, Session );
			IType[] types = componentType.Subtypes;
			bool substituteComponent = false;
			for ( int i=0; i<types.Length; i++ ) 
			{
				object result = ProcessValue( values[i], types[i] );
				if (result != null) 
				{
					substituteComponent = true;
					values[i] = result;
				}
			}

			if (substituteComponent) 
			{
				componentType.SetPropertyValues(component, values);
			}

			return null;
		}

		public override void Process(object obj, NHibernate.Persister.IClassPersister persister)
		{
			object[] values = persister.GetPropertyValues(obj);
			IType[] types = persister.PropertyTypes;
			ProcessValues(values, types);
			if ( IsSubstitutionRequired ) persister.SetPropertyValues(obj, values);
		}

	}
}
