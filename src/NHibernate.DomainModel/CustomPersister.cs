using System;
using System.Collections;
using NHibernate.Cache;
using NHibernate.Cache.Entry;
using NHibernate.Engine;
using NHibernate.Event;
using NHibernate.Id;
using NHibernate.Mapping;
using NHibernate.Metadata;
using NHibernate.Persister.Entity;
using NHibernate.Tuple.Entity;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.DomainModel
{
	/// <summary>
	/// Summary description for CustomPersister.
	/// </summary>
	public class CustomPersister : IEntityPersister
	{
		private static readonly Hashtable Instances = new Hashtable();
		private static readonly IIdentifierGenerator Generator = new UUIDHexGenerator();

		private static readonly IType[] Types = new IType[] { NHibernateUtil.String };
		private static readonly string[] Names = new string[] { "name" };
		private static readonly bool[] Mutability = new bool[] { true };
		private static readonly bool[] Nullability = new bool[] { true };
		private readonly ISessionFactoryImplementor factory;

		public CustomPersister(PersistentClass model, ICacheConcurrencyStrategy cache, ISessionFactoryImplementor factory,
													 IMapping mapping)
		{
			this.factory = factory;
		}

		private static void CheckEntityMode(EntityMode entityMode)
		{
			if (EntityMode.Poco != entityMode)
			{
				throw new ArgumentOutOfRangeException("entityMode", "Unhandled EntityMode : " + entityMode);
			}
		}

		#region IEntityPersister Members

		public ISessionFactoryImplementor Factory
		{
			get { return factory; }
		}

		public string RootEntityName
		{
			get { return "CUSTOMS"; }
		}

		public string EntityName
		{
			get { return typeof (Custom).FullName; }
		}

		public EntityMetamodel EntityMetamodel
		{
			get { return null; }
		}

		public string[] PropertySpaces
		{
			get { return new string[] { "CUSTOMS" }; }
		}

		public string[] QuerySpaces
		{
			get { return new string[] { "CUSTOMS" }; }
		}

		public bool IsMutable
		{
			get { return true; }
		}

		public bool IsInherited
		{
			get { return false; }
		}

		public bool IsIdentifierAssignedByInsert
		{
			get { return false; }
		}

		#region IOptimisticCacheSource Members

		public bool IsVersioned
		{
			get { return false; }
		}

		bool IEntityPersister.IsVersioned
		{
			get { return IsVersioned; }
		}

		public IVersionType VersionType
		{
			get { return null; }
		}

		public int VersionProperty
		{
			get { return 0; }
		}

		public int[] NaturalIdentifierProperties
		{
			get { return null; }
		}

		public IIdentifierGenerator IdentifierGenerator
		{
			get { return Generator; }
		}

		public IType[] PropertyTypes
		{
			get { return Types; }
		}

		public string[] PropertyNames
		{
			get { return Names; }
		}

		public bool[] PropertyInsertability
		{
			get { return Mutability; }
		}

		public ValueInclusion[] PropertyInsertGenerationInclusions
		{
			get { return new ValueInclusion[0]; }
		}

		public ValueInclusion[] PropertyUpdateGenerationInclusions
		{
			get { return new ValueInclusion[0]; }
		}

		public bool[] PropertyCheckability
		{
			get { return Mutability; }
		}

		public bool[] PropertyNullability
		{
			get { return Nullability; }
		}

		public bool[] PropertyVersionability
		{
			get { return Mutability; }
		}

		public bool[] PropertyLaziness
		{
			get { return null; }
		}

		public CascadeStyle[] PropertyCascadeStyles
		{
			get { return null; }
		}

		public IType IdentifierType
		{
			get { return NHibernateUtil.String; }
		}

		public string IdentifierPropertyName
		{
			get { return "Id"; }
		}

		public bool IsCacheInvalidationRequired
		{
			get { return false; }
		}

		public bool IsLazyPropertiesCacheable
		{
			get { return true; }
		}

		public ICacheConcurrencyStrategy Cache
		{
			get { return null; }
		}

		public ICacheEntryStructure CacheEntryStructure
		{
			get { return new UnstructuredCacheEntry(); }
		}

		public IClassMetadata ClassMetadata
		{
			get { return null; }
		}

		public bool IsBatchLoadable
		{
			get { return false; }
		}

		public bool IsSelectBeforeUpdateRequired
		{
			get { return false; }
		}

		public bool IsVersionPropertyGenerated
		{
			get { return false; }
		}

		public void PostInstantiate()
		{
		}

		public bool IsSubclassEntityName(string entityName)
		{
			return typeof (Custom).FullName.Equals(entityName);
		}

		public bool HasProxy
		{
			get { return false; }
		}

		public bool HasCollections
		{
			get { return false; }
		}

		public bool HasMutableProperties
		{
			get { return false; }
		}

		public bool HasSubselectLoadableCollections
		{
			get { return false; }
		}

		public bool HasCascades
		{
			get { return false; }
		}

		public IType GetPropertyType(string propertyName)
		{
			throw new NotSupportedException();
		}

		public int[] FindDirty(object[] currentState, object[] previousState, object entity, ISessionImplementor session)
		{
			if (!EqualsHelper.Equals(currentState[0], previousState[0]))
			{
				return new int[] { 0 };
			}
			else
			{
				return null;
			}
		}

		public int[] FindModified(object[] old, object[] current, object entity, ISessionImplementor session)
		{
			if (!EqualsHelper.Equals(old[0], current[0]))
			{
				return new int[] { 0 };
			}
			else
			{
				return null;
			}
		}

		public bool HasIdentifierProperty
		{
			get { return true; }
		}

		public bool CanExtractIdOutOfEntity
		{
			get { return true; }
		}

		public bool HasNaturalIdentifier
		{
			get { return false; }
		}

		public object[] GetNaturalIdentifierSnapshot(object id, ISessionImplementor session)
		{
			return null;
		}

		public bool HasLazyProperties
		{
			get { return false; }
		}

		public object Load(object id, object optionalObject, LockMode lockMode, ISessionImplementor session)
		{
			// fails when optional object is supplied
			Custom clone = null;
			Custom obj = (Custom)Instances[id];
			if (obj != null)
			{
				clone = (Custom)obj.Clone();
				TwoPhaseLoad.AddUninitializedEntity(new EntityKey(id, this, session.EntityMode), clone, this, LockMode.None, false,
				                                    session);
				TwoPhaseLoad.PostHydrate(this, id, new String[] {obj.Name}, clone, LockMode.None, false, session);
				TwoPhaseLoad.InitializeEntity(clone, false, session, new PreLoadEvent((IEventSource) session),
				                              new PostLoadEvent((IEventSource) session));
			}
			return clone;
		}

		public void Lock(object id, object version, object obj, LockMode lockMode, ISessionImplementor session)
		{
			throw new NotSupportedException();
		}

		public void Insert(object id, object[] fields, object obj, ISessionImplementor session)
		{
			Instances[id] = ((Custom)obj).Clone();
		}

		public object Insert(object[] fields, object obj, ISessionImplementor session)
		{
			throw new NotSupportedException();
		}

		public void Delete(object id, object version, object obj, ISessionImplementor session)
		{
			Instances.Remove(id);
		}

		public void Update(object id, object[] fields, int[] dirtyFields, bool hasDirtyCollection, object[] oldFields,
		                   object oldVersion, object obj, object rowId, ISessionImplementor session)
		{
			Instances[id] = ((Custom)obj).Clone();
		}

		public bool[] PropertyUpdateability
		{
			get { return Mutability; }
		}

		public bool HasCache
		{
			get { return false; }
		}

		public object[] GetDatabaseSnapshot(object id, ISessionImplementor session)
		{
			return null;
		}

		public object GetCurrentVersion(object id, ISessionImplementor session)
		{
			return Instances[id];
		}

		public object ForceVersionIncrement(object id, object currentVersion, ISessionImplementor session)
		{
			return null;
		}

		public EntityMode? GuessEntityMode(object obj)
		{
			if (!IsInstance(obj, EntityMode.Poco))
			{
				return null;
			}
			else
			{
				return EntityMode.Poco;
			}
		}

		public bool IsInstrumented(EntityMode entityMode)
		{
			return false;
		}

		public bool HasInsertGeneratedProperties
		{
			get { return false; }
		}

		public bool HasUpdateGeneratedProperties
		{
			get { return false; }
		}

		public void AfterInitialize(object entity, bool lazyPropertiesAreUnfetched, ISessionImplementor session)
		{
		}

		public void AfterReassociate(object entity, ISessionImplementor session)
		{
		}

		public object CreateProxy(object id, ISessionImplementor session)
		{
			throw new NotSupportedException("no proxy for this class");
		}

		public bool? IsTransient(object obj, ISessionImplementor session)
		{
			return ((Custom) obj).Id == null;
		}

		public object[] GetPropertyValuesToInsert(object obj, IDictionary mergeMap, ISessionImplementor session)
		{
			return GetPropertyValues(obj, session.EntityMode);
		}

		public void ProcessInsertGeneratedProperties(object id, object entity, object[] state, ISessionImplementor session)
		{
		}

		public void ProcessUpdateGeneratedProperties(object id, object entity, object[] state, ISessionImplementor session)
		{
		}

		public System.Type GetMappedClass(EntityMode entityMode)
		{
			CheckEntityMode(entityMode);
			return typeof(Custom);
		}

		public bool ImplementsLifecycle(EntityMode entityMode)
		{
			CheckEntityMode(entityMode);
			return false;
		}

		public bool ImplementsValidatable(EntityMode entityMode)
		{
			CheckEntityMode(entityMode);
			return false;
		}

		public System.Type GetConcreteProxyClass(EntityMode entityMode)
		{
			CheckEntityMode(entityMode);
			return typeof (Custom);
		}

		public void SetPropertyValues(object obj, object[] values, EntityMode entityMode)
		{
			CheckEntityMode(entityMode);
			SetPropertyValue(obj, 0, values[0], entityMode);
		}

		public void SetPropertyValue(object obj, int i, object value, EntityMode entityMode)
		{
			CheckEntityMode(entityMode);
			((Custom) obj).Name = (string) value;
		}

		public object[] GetPropertyValues(object obj, EntityMode entityMode)
		{
			CheckEntityMode(entityMode);
			Custom c = (Custom) obj;
			return new Object[] {c.Name};
		}

		public object GetPropertyValue(object obj, int i, EntityMode entityMode)
		{
			CheckEntityMode(entityMode);
			return ((Custom)obj).Name;
		}

		public object GetPropertyValue(object obj, string name, EntityMode entityMode)
		{
			CheckEntityMode(entityMode);
			return ((Custom)obj).Name;
		}

		public object GetIdentifier(object obj, EntityMode entityMode)
		{
			CheckEntityMode(entityMode);
			return ((Custom)obj).Id;
		}

		public void SetIdentifier(object obj, object id, EntityMode entityMode)
		{
			CheckEntityMode(entityMode);
			((Custom) obj).Id = (string) id;
		}

		public object GetVersion(object obj, EntityMode entityMode)
		{
			CheckEntityMode(entityMode);
			return null;
		}

		public object Instantiate(object id, EntityMode entityMode)
		{
			CheckEntityMode(entityMode);
			Custom c = new Custom();
			c.Id = (string)id;
			return c;
		}

		public bool IsInstance(object entity, EntityMode entityMode)
		{
			CheckEntityMode(entityMode);
			return entity is Custom;
		}

		public bool HasUninitializedLazyProperties(object obj, EntityMode entityMode)
		{
			CheckEntityMode(entityMode);
			return false;
		}

		public void ResetIdentifier(object entity, object currentId, object currentVersion, EntityMode entityMode)
		{
			CheckEntityMode(entityMode);
			((Custom)entity).Id = (string)currentId;
		}

		public IEntityPersister GetSubclassEntityPersister(object instance, ISessionFactoryImplementor factory,
		                                                   EntityMode entityMode)
		{
			CheckEntityMode(entityMode);
			return this;
		}

		public bool? IsUnsavedVersion(object version)
		{
			return false;
		}

		#endregion

		public IComparer VersionComparator
		{
			get { return null; }
		}

		#endregion
	}
}
