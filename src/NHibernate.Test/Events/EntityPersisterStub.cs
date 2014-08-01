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

namespace NHibernate.Test.Events
{
    public class EntityPersisterStub : IEntityPersister
    {
        public EntityPersisterStub()
        {
            PropertyNames = new string[]{};
        }

        public ISessionFactoryImplementor Factory { get; set; }
        public string RootEntityName { get; set; }
        public string EntityName { get; set; }
        public EntityMetamodel EntityMetamodel { get; set; }
        public string[] PropertySpaces { get; set; }
        public string[] QuerySpaces { get; set; }
        public bool IsMutable { get; set; }
        public bool IsInherited { get; set; }
        public bool IsIdentifierAssignedByInsert { get; set; }

        bool IEntityPersister.IsVersioned
        {
            get { return true; }
        }

        public IVersionType VersionType { get; set; }
        public int VersionProperty { get; set; }
        public int[] NaturalIdentifierProperties { get; set; }
        public IIdentifierGenerator IdentifierGenerator { get; set; }
        public IType[] PropertyTypes { get; set; }
        public string[] PropertyNames { get; set; }
        public bool[] PropertyInsertability { get; set; }
        public ValueInclusion[] PropertyInsertGenerationInclusions { get; set; }
        public ValueInclusion[] PropertyUpdateGenerationInclusions { get; set; }
        public bool[] PropertyCheckability { get; set; }
        public bool[] PropertyNullability { get; set; }
        public bool[] PropertyVersionability { get; set; }
        public bool[] PropertyLaziness { get; set; }
        public CascadeStyle[] PropertyCascadeStyles { get; set; }
        public IType IdentifierType { get; set; }
        public string IdentifierPropertyName { get; set; }
        public bool IsCacheInvalidationRequired { get; set; }
        public bool IsLazyPropertiesCacheable { get; set; }
        public ICacheConcurrencyStrategy Cache { get; set; }
        public ICacheEntryStructure CacheEntryStructure { get; set; }
        public IClassMetadata ClassMetadata { get; set; }
        public bool IsBatchLoadable { get; set; }
        public bool IsSelectBeforeUpdateRequired { get; set; }
        public bool IsVersionPropertyGenerated { get; set; }
        public void PostInstantiate()
        {
            throw new NotImplementedException();
        }

        public bool IsSubclassEntityName(string entityName)
        {
            throw new NotImplementedException();
        }

        public bool HasProxy { get; set; }
        public bool HasCollections { get; set; }
        public bool HasMutableProperties { get; set; }
        public bool HasSubselectLoadableCollections { get; set; }
        public bool HasCascades { get; set; }
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

        public bool HasIdentifierProperty { get; set; }
        public bool CanExtractIdOutOfEntity { get; set; }
        public bool HasNaturalIdentifier { get; set; }
        public object[] GetNaturalIdentifierSnapshot(object id, ISessionImplementor session)
        {
            throw new NotImplementedException();
        }

        public bool HasLazyProperties { get; set; }
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

        public bool[] PropertyUpdateability { get; set; }
        public bool HasCache { get; set; }
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

        public bool HasInsertGeneratedProperties { get; set; }
        public bool HasUpdateGeneratedProperties { get; set; }
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

        public IEntityPersister GetSubclassEntityPersister(object instance, ISessionFactoryImplementor factory, EntityMode entityMode)
        {
            throw new NotImplementedException();
        }

        public bool? IsUnsavedVersion(object version)
        {
            throw new NotImplementedException();
        }

        bool IOptimisticCacheSource.IsVersioned
        {
            get { return true; }
        }

        public IComparer VersionComparator { get; set; }
    }
}
