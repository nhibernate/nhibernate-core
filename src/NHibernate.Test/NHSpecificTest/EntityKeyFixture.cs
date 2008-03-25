using System;
using System.Collections;
using NHibernate.Cache;
using NHibernate.Cache.Entry;
using NHibernate.Engine;
using NHibernate.Id;
using NHibernate.Metadata;
using NHibernate.Persister.Entity;
using NHibernate.Tuple.Entity;
using NHibernate.Type;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest
{

	public class TestingClassPersister : IEntityPersister
	{
		public IType IdentifierType
		{
			get { return NHibernateUtil.Int32; }
		}

		// NOTE:
		// IdentifierType is what we need for this test.
		// other properties with a sort of implementation are :
		// RootEntityName, EntityName, IsBatchLoadable, Factory

		#region IEntityPersister Members

		public ISessionFactoryImplementor Factory
		{
			get { return null; }
		}

		public string RootEntityName
		{
			get { return null; }
		}

		public string EntityName
		{
			get { return null; }
		}

		public EntityMetamodel EntityMetamodel
		{
			get { throw new NotImplementedException(); }
		}

		public string[] PropertySpaces
		{
			get { throw new NotImplementedException(); }
		}

		public string[] QuerySpaces
		{
			get { throw new NotImplementedException(); }
		}

		public bool IsMutable
		{
			get { throw new NotImplementedException(); }
		}

		public bool IsInherited
		{
			get { throw new NotImplementedException(); }
		}

		public bool IsIdentifierAssignedByInsert
		{
			get { throw new NotImplementedException(); }
		}

		#region IOptimisticCacheSource Members

		public bool IsVersioned
		{
			get { throw new NotImplementedException(); }
		}

		bool IEntityPersister.IsVersioned
		{
			get { throw new NotImplementedException(); }
		}

		public IVersionType VersionType
		{
			get { throw new NotImplementedException(); }
		}

		public int VersionProperty
		{
			get { throw new NotImplementedException(); }
		}

		public int[] NaturalIdentifierProperties
		{
			get { throw new NotImplementedException(); }
		}

		public IIdentifierGenerator IdentifierGenerator
		{
			get { throw new NotImplementedException(); }
		}

		public IType[] PropertyTypes
		{
			get { throw new NotImplementedException(); }
		}

		public string[] PropertyNames
		{
			get { throw new NotImplementedException(); }
		}

		public bool[] PropertyInsertability
		{
			get { throw new NotImplementedException(); }
		}

		public ValueInclusion[] PropertyInsertGenerationInclusions
		{
			get { throw new NotImplementedException(); }
		}

		public ValueInclusion[] PropertyUpdateGenerationInclusions
		{
			get { throw new NotImplementedException(); }
		}

		public bool[] PropertyCheckability
		{
			get { throw new NotImplementedException(); }
		}

		public bool[] PropertyNullability
		{
			get { throw new NotImplementedException(); }
		}

		public bool[] PropertyVersionability
		{
			get { throw new NotImplementedException(); }
		}

		public bool[] PropertyLaziness
		{
			get { throw new NotImplementedException(); }
		}

		public CascadeStyle[] PropertyCascadeStyles
		{
			get { throw new NotImplementedException(); }
		}

		public string IdentifierPropertyName
		{
			get { throw new NotImplementedException(); }
		}

		public bool IsCacheInvalidationRequired
		{
			get { throw new NotImplementedException(); }
		}

		public bool IsLazyPropertiesCacheable
		{
			get { throw new NotImplementedException(); }
		}

		public ICacheConcurrencyStrategy Cache
		{
			get { throw new NotImplementedException(); }
		}

		public ICacheEntryStructure CacheEntryStructure
		{
			get { throw new NotImplementedException(); }
		}

		public IClassMetadata ClassMetadata
		{
			get { throw new NotImplementedException(); }
		}

		public bool IsBatchLoadable
		{
			get { return false; }
		}

		public bool IsSelectBeforeUpdateRequired
		{
			get { throw new NotImplementedException(); }
		}

		public bool IsVersionPropertyGenerated
		{
			get { throw new NotImplementedException(); }
		}

		public void PostInstantiate()
		{
			throw new NotImplementedException();
		}

		public bool IsSubclassEntityName(string entityName)
		{
			throw new NotImplementedException();
		}

		public bool HasProxy
		{
			get { throw new NotImplementedException(); }
		}

		public bool HasCollections
		{
			get { throw new NotImplementedException(); }
		}

		public bool HasMutableProperties
		{
			get { throw new NotImplementedException(); }
		}

		public bool HasSubselectLoadableCollections
		{
			get { throw new NotImplementedException(); }
		}

		public bool HasCascades
		{
			get { throw new NotImplementedException(); }
		}

		public IType GetPropertyType(string propertyName)
		{
			throw new NotImplementedException();
		}

		public int[] FindDirty(object[] currentState, object[] previousState, object entity, ISessionImplementor session)
		{
			throw new NotImplementedException();
		}

		public int[] FindModified(object[] old, object[] current, object entity, ISessionImplementor session)
		{
			throw new NotImplementedException();
		}

		public bool HasIdentifierProperty
		{
			get { throw new NotImplementedException(); }
		}

		public bool CanExtractIdOutOfEntity
		{
			get { throw new NotImplementedException(); }
		}

		public bool HasNaturalIdentifier
		{
			get { throw new NotImplementedException(); }
		}

		public object[] GetNaturalIdentifierSnapshot(object id, ISessionImplementor session)
		{
			throw new NotImplementedException();
		}

		public bool HasLazyProperties
		{
			get { throw new NotImplementedException(); }
		}

		public object Load(object id, object optionalObject, LockMode lockMode, ISessionImplementor session)
		{
			throw new NotImplementedException();
		}

		public void Lock(object id, object version, object obj, LockMode lockMode, ISessionImplementor session)
		{
			throw new NotImplementedException();
		}

		public void Insert(object id, object[] fields, object obj, ISessionImplementor session)
		{
			throw new NotImplementedException();
		}

		public object Insert(object[] fields, object obj, ISessionImplementor session)
		{
			throw new NotImplementedException();
		}

		public void Delete(object id, object version, object obj, ISessionImplementor session)
		{
			throw new NotImplementedException();
		}

		public void Update(object id, object[] fields, int[] dirtyFields, bool hasDirtyCollection, object[] oldFields,
		                   object oldVersion, object obj, object rowId, ISessionImplementor session)
		{
			throw new NotImplementedException();
		}

		public bool[] PropertyUpdateability
		{
			get { throw new NotImplementedException(); }
		}

		public bool HasCache
		{
			get { throw new NotImplementedException(); }
		}

		public object[] GetDatabaseSnapshot(object id, ISessionImplementor session)
		{
			throw new NotImplementedException();
		}

		public object GetCurrentVersion(object id, ISessionImplementor session)
		{
			throw new NotImplementedException();
		}

		public object ForceVersionIncrement(object id, object currentVersion, ISessionImplementor session)
		{
			throw new NotImplementedException();
		}

		public EntityMode? GuessEntityMode(object obj)
		{
			throw new NotImplementedException();
		}

		public bool IsInstrumented(EntityMode entityMode)
		{
			throw new NotImplementedException();
		}

		public bool HasInsertGeneratedProperties
		{
			get { throw new NotImplementedException(); }
		}

		public bool HasUpdateGeneratedProperties
		{
			get { throw new NotImplementedException(); }
		}

		public void AfterInitialize(object entity, bool lazyPropertiesAreUnfetched, ISessionImplementor session)
		{
			throw new NotImplementedException();
		}

		public void AfterReassociate(object entity, ISessionImplementor session)
		{
			throw new NotImplementedException();
		}

		public object CreateProxy(object id, ISessionImplementor session)
		{
			throw new NotImplementedException();
		}

		public bool? IsTransient(object obj, ISessionImplementor session)
		{
			throw new NotImplementedException();
		}

		public object[] GetPropertyValuesToInsert(object obj, IDictionary mergeMap, ISessionImplementor session)
		{
			throw new NotImplementedException();
		}

		public void ProcessInsertGeneratedProperties(object id, object entity, object[] state, ISessionImplementor session)
		{
			throw new NotImplementedException();
		}

		public void ProcessUpdateGeneratedProperties(object id, object entity, object[] state, ISessionImplementor session)
		{
			throw new NotImplementedException();
		}

		public System.Type GetMappedClass(EntityMode entityMode)
		{
			throw new NotImplementedException();
		}

		public bool ImplementsLifecycle(EntityMode entityMode)
		{
			throw new NotImplementedException();
		}

		public bool ImplementsValidatable(EntityMode entityMode)
		{
			throw new NotImplementedException();
		}

		public System.Type GetConcreteProxyClass(EntityMode entityMode)
		{
			throw new NotImplementedException();
		}

		public void SetPropertyValues(object obj, object[] values, EntityMode entityMode)
		{
			throw new NotImplementedException();
		}

		public void SetPropertyValue(object obj, int i, object value, EntityMode entityMode)
		{
			throw new NotImplementedException();
		}

		public object[] GetPropertyValues(object obj, EntityMode entityMode)
		{
			throw new NotImplementedException();
		}

		public object GetPropertyValue(object obj, int i, EntityMode entityMode)
		{
			throw new NotImplementedException();
		}

		public object GetPropertyValue(object obj, string name, EntityMode entityMode)
		{
			throw new NotImplementedException();
		}

		public object GetIdentifier(object obj, EntityMode entityMode)
		{
			throw new NotImplementedException();
		}

		public void SetIdentifier(object obj, object id, EntityMode entityMode)
		{
			throw new NotImplementedException();
		}

		public object GetVersion(object obj, EntityMode entityMode)
		{
			throw new NotImplementedException();
		}

		public object Instantiate(object id, EntityMode entityMode)
		{
			throw new NotImplementedException();
		}

		public bool IsInstance(object entity, EntityMode entityMode)
		{
			throw new NotImplementedException();
		}

		public bool HasUninitializedLazyProperties(object obj, EntityMode entityMode)
		{
			throw new NotImplementedException();
		}

		public void ResetIdentifier(object entity, object currentId, object currentVersion, EntityMode entityMode)
		{
			throw new NotImplementedException();
		}

		public IEntityPersister GetSubclassEntityPersister(object instance, ISessionFactoryImplementor factory,
		                                                   EntityMode entityMode)
		{
			throw new NotImplementedException();
		}

		public bool? IsUnsavedVersion(object version)
		{
			throw new NotImplementedException();
		}

		#endregion

		public IComparer VersionComparator
		{
			get { throw new NotImplementedException(); }
		}

		#endregion
	}

	[TestFixture]
	public class EntityKeyFixture
	{
		[Test, ExpectedException(typeof(ArgumentException))]
		public void CreateWithWrongTypeOfId()
		{
			IEntityPersister persister = new TestingClassPersister();
			EntityKey key = new EntityKey(1L, persister, EntityMode.Poco);
		}
	}
}
