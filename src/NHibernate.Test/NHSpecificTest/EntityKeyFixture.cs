using System;
using System.Collections;
using NHibernate.Persister.Entity;
using NUnit.Framework;

using NHibernate.Engine;

namespace NHibernate.Test.NHSpecificTest
{
	public class TestingClassPersister : IEntityPersister
	{
		public NHibernate.Type.IType IdentifierType
		{
			get { return NHibernateUtil.Int32; }
		}

		#region IEntityPersister Members

		public object IdentifierSpace
		{
			get
			{
				// TODO:  Add TestingClassPersister.IdentifierSpace getter implementation
				return null;
			}
		}

		public NHibernate.Metadata.IClassMetadata ClassMetadata
		{
			get
			{
				// TODO:  Add TestingClassPersister.ClassMetadata getter implementation
				return null;
			}
		}

		public bool HasCache
		{
			get
			{
				// TODO:  Add TestingClassPersister.HasCache getter implementation
				return false;
			}
		}

		public object GetCurrentVersion(object id, ISessionImplementor session)
		{
			// TODO:  Add TestingClassPersister.GetCurrentVersion implementation
			return null;
		}

		public int[] FindDirty(object[] x, object[] y, object owner, ISessionImplementor session)
		{
			// TODO:  Add TestingClassPersister.FindDirty implementation
			return null;
		}

		public bool IsBatchLoadable
		{
			get
			{
				// TODO:  Add TestingClassPersister.IsBatchLoadable getter implementation
				return false;
			}
		}

		public bool IsCacheInvalidationRequired
		{
			get
			{
				// TODO:  Add TestingClassPersister.IsCacheInvalidationRequired getter implementation
				return false;
			}
		}

		public bool[] PropertyUpdateability
		{
			get
			{
				// TODO:  Add TestingClassPersister.PropertyUpdateability getter implementation
				return null;
			}
		}

		public bool[] PropertyCheckability
		{
			get { return null; }
		}

		public bool HasCascades
		{
			get
			{
				// TODO:  Add TestingClassPersister.HasCascades getter implementation
				return false;
			}
		}

		public object Instantiate(object id)
		{
			// TODO:  Add TestingClassPersister.Instantiate implementation
			return null;
		}

		public NHibernate.Id.IIdentifierGenerator IdentifierGenerator
		{
			get
			{
				// TODO:  Add TestingClassPersister.IdentifierGenerator getter implementation
				return null;
			}
		}

		public bool[] PropertyInsertability
		{
			get
			{
				// TODO:  Add TestingClassPersister.PropertyInsertability getter implementation
				return null;
			}
		}

        public bool[] PropertyVersionability
        {
            get 
            {
                // TODO:  Add TestingClassPersister.PropertyVersionability getter implementation
                return null; 
            }
        }


		public System.Type MappedClass
		{
			get
			{
				// TODO:  Add TestingClassPersister.MappedClass getter implementation
				return null;
			}
		}

		public object Insert(object[] fields, object obj, ISessionImplementor session)
		{
			// TODO:  Add TestingClassPersister.Insert implementation
			return null;
		}

		void IEntityPersister.Insert(object id, object[] fields, object obj, ISessionImplementor session)
		{
			// TODO:  Add TestingClassPersister.NHibernate.Persister.IEntityPersister.Insert implementation
		}

		public bool IsUnsaved(object obj)
		{
			// TODO:  Add TestingClassPersister.IsUnsaved implementation
			return false;
		}

		public bool HasIdentifierPropertyOrEmbeddedCompositeIdentifier
		{
			get
			{
				// TODO:  Add TestingClassPersister.HasIdentifierPropertyOrEmbeddedCompositeIdentifier getter implementation
				return false;
			}
		}

		public object GetVersion(object obj)
		{
			// TODO:  Add TestingClassPersister.GetVersion implementation
			return null;
		}

		public NHibernate.Engine.Cascades.CascadeStyle[] PropertyCascadeStyles
		{
			get
			{
				// TODO:  Add TestingClassPersister.PropertyCascadeStyles getter implementation
				return null;
			}
		}

		public object[] PropertySpaces
		{
			get
			{
				// TODO:  Add TestingClassPersister.PropertySpaces getter implementation
				return null;
			}
		}

		public void SetPropertyValues(object obj, object[] values)
		{
			// TODO:  Add TestingClassPersister.SetPropertyValues implementation
		}

		public NHibernate.Type.IType[] PropertyTypes
		{
			get
			{
				// TODO:  Add TestingClassPersister.PropertyTypes getter implementation
				return null;
			}
		}

		public object[] GetDatabaseSnapshot(object id, object version, ISessionImplementor session)
		{
			// TODO:  Add TestingClassPersister.GetCurrentPersistentState implementation
			return null;
		}

		public bool IsIdentifierAssignedByInsert
		{
			get
			{
				// TODO:  Add TestingClassPersister.IsIdentifierAssignedByInsert getter implementation
				return false;
			}
		}

		public System.Type ConcreteProxyClass
		{
			get
			{
				// TODO:  Add TestingClassPersister.ConcreteProxyClass getter implementation
				return null;
			}
		}

		public object GetIdentifier(object obj)
		{
			// TODO:  Add TestingClassPersister.GetIdentifier implementation
			return null;
		}

		public object GetPropertyValue(object obj, string name)
		{
			// TODO:  Add TestingClassPersister.GetPropertyValue implementation
			return null;
		}

		object IEntityPersister.GetPropertyValue(object obj, int i)
		{
			// TODO:  Add TestingClassPersister.NHibernate.Persister.IEntityPersister.GetPropertyValue implementation
			return null;
		}

		public bool IsVersioned
		{
			get
			{
				// TODO:  Add TestingClassPersister.IsVersioned getter implementation
				return false;
			}
		}

		public bool HasProxy
		{
			get
			{
				// TODO:  Add TestingClassPersister.HasProxy getter implementation
				return false;
			}
		}

		public void SetIdentifier(object obj, object id)
		{
			// TODO:  Add TestingClassPersister.SetIdentifier implementation
		}

		public bool ImplementsLifecycle
		{
			get
			{
				// TODO:  Add TestingClassPersister.ImplementsLifecycle getter implementation
				return false;
			}
		}

		public object[] GetPropertyValues(object obj)
		{
			// TODO:  Add TestingClassPersister.GetPropertyValues implementation
			return null;
		}

		public string ClassName
		{
			get
			{
				// TODO:  Add TestingClassPersister.ClassName getter implementation
				return null;
			}
		}

		public int[] FindModified(object[] old, object[] current, object owner, ISessionImplementor session)
		{
			// TODO:  Add TestingClassPersister.FindModified implementation
			return null;
		}

		public bool HasIdentifierProperty
		{
			get
			{
				// TODO:  Add TestingClassPersister.HasIdentifierProperty getter implementation
				return false;
			}
		}

		public NHibernate.Type.IType GetPropertyType(string propertyName)
		{
			// TODO:  Add TestingClassPersister.GetPropertyType implementation
			return null;
		}

		public int VersionProperty
		{
			get
			{
				// TODO:  Add TestingClassPersister.VersionProperty getter implementation
				return 0;
			}
		}

		public System.Reflection.PropertyInfo ProxyIdentifierProperty
		{
			get
			{
				// TODO:  Add TestingClassPersister.ProxyIdentifierProperty getter implementation
				return null;
			}
		}

		public bool IsMutable
		{
			get
			{
				// TODO:  Add TestingClassPersister.IsMutable getter implementation
				return false;
			}
		}

		public bool HasCollections
		{
			get
			{
				// TODO:  Add TestingClassPersister.HasCollections getter implementation
				return false;
			}
		}

		public void Update(object id, object[] fields, int[] dirtyFields, bool hasDirtyCollection, object[] oldFields, object oldVersion, object obj, ISessionImplementor session)
		{
			// TODO:  Add TestingClassPersister.Update implementation
		}

		public void Delete(object id, object version, object obj, ISessionImplementor session)
		{
			// TODO:  Add TestingClassPersister.Delete implementation
		}

		public string[] PropertyNames
		{
			get
			{
				// TODO:  Add TestingClassPersister.PropertyNames getter implementation
				return null;
			}
		}

		public void SetPropertyValue(object obj, string name, object value)
		{
			// TODO:  Add TestingClassPersister.SetPropertyValue implementation
		}

		void IEntityPersister.SetPropertyValue(object obj, int i, object value)
		{
			// TODO:  Add TestingClassPersister.NHibernate.Persister.IEntityPersister.SetPropertyValue implementation
		}

		public object Load(object id, object optionalObject, LockMode lockMode, ISessionImplementor session)
		{
			// TODO:  Add TestingClassPersister.Load implementation
			return null;
		}

		public NHibernate.Cache.ICacheConcurrencyStrategy Cache
		{
			get
			{
				// TODO:  Add TestingClassPersister.Cache getter implementation
				return null;
			}
		}

		public bool ImplementsValidatable
		{
			get
			{
				// TODO:  Add TestingClassPersister.ImplementsValidatable getter implementation
				return false;
			}
		}

		public bool IsUnsavedVersion( object [ ] values )
		{
			// TODO:  Add TestingClassPersister.IsDefaultVersion implementation
			return false;
		}

		public bool[] PropertyNullability
		{
			get
			{
				// TODO:  Add TestingClassPersister.PropertyNullability getter implementation
				return null;
			}
		}

		public void PostInstantiate()
		{
			// TODO:  Add TestingClassPersister.PostInstantiate implementation
		}

		public NHibernate.Type.IVersionType VersionType
		{
			get
			{
				// TODO:  Add TestingClassPersister.VersionType getter implementation
				return null;
			}
		}

		public string IdentifierPropertyName
		{
			get
			{
				// TODO:  Add TestingClassPersister.IdentifierPropertyName getter implementation
				return null;
			}
		}

		public System.Type[] ProxyInterfaces
		{
			get
			{
				// TODO:  Add TestingClassPersister.ProxyInterfaces getter implementation
				return null;
			}
		}

		public void Lock(object id, object version, object obj, LockMode lockMode, ISessionImplementor session)
		{
			// TODO:  Add TestingClassPersister.Lock implementation
		}

		public object CreateProxy( object id, ISessionImplementor session )
		{
			return null;
		}

		public object[] QuerySpaces
		{
			get { return null; }
		}

		public ISessionFactoryImplementor Factory
		{
			get { return null; }
		}
		#endregion

    }

	[TestFixture]
	public class EntityKeyFixture
	{
		[Test, ExpectedException( typeof( ArgumentException ) )]
		public void CreateWithWrongTypeOfId()
		{
			IEntityPersister persister = new TestingClassPersister();
			EntityKey key = new EntityKey(1L, persister);
		}
	}
}
