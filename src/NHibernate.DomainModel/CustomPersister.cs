using System;
using System.Collections;

using NHibernate.Cache;
using NHibernate.Metadata;
using NHibernate.Engine;
using NHibernate.Id;
using NHibernate.Impl;
using NHibernate.Mapping;
using NHibernate.Persister;
using NHibernate.Type;

namespace NHibernate.DomainModel
{
	/// <summary>
	/// Summary description for CustomPersister.
	/// </summary>
	public class CustomPersister : IClassPersister
	{

		private static readonly Hashtable Instances = new Hashtable();
		private static readonly IIdentifierGenerator Generator = new CounterGenerator();

		private static readonly IType[] Types = new IType[] { NHibernateUtil.String };
		private static readonly string[] Names = new string[] { "name" };
		private static readonly bool[] Mutability = new bool[] { true };

		public CustomPersister(PersistentClass model, ISessionFactory factory )
		{
		}

		#region IClassPersister Members

		public object IdentifierSpace
		{
			get { return "CUSTOMS"; }
		}

		public IClassMetadata ClassMetadata
		{
			get { return null; }
		}

		public bool HasCache
		{
			get { return false; }
		}

		public int[] FindDirty(object[] x, object[] y, object owner, ISessionImplementor session)
		{
			if( x[0].Equals(y[0])==false) 
			{
				return new int[] {0};
			}
			else 
			{
				return null;
			}
		}

		public bool[] PropertyUpdateability
		{
			get  { return Mutability; }
		}

		public bool HasCascades
		{
			get { return false; }
		}

		public object Instantiate(object id)
		{
			Custom c = new Custom();
			c.Id = (long)id;
			return c;
		}

		public IIdentifierGenerator IdentifierGenerator
		{
			get { return Generator; }
		}

		public bool[] PropertyInsertability
		{
			get  { return Mutability; }
		}

		public System.Type MappedClass
		{
			get { return typeof(Custom); }
		}

		public object Insert(object[] fields, object obj, ISessionImplementor session)
		{
			throw new NotSupportedException("CustomPersister.Insert withou Id is not supported.");
		}

		public void Insert(object id, object[] fields, object obj, ISessionImplementor session)
		{
			Instances[id] = ((Custom)obj).Clone();
		}

		public bool IsUnsaved(object id)
		{
			return (long)id==0;
		}

		public bool HasIdentifierPropertyOrEmbeddedCompositeIdentifier
		{
			get { return true;}
		}

		public object GetVersion(object obj)
		{
			 return null;
		}

		public Cascades.CascadeStyle[] PropertyCascadeStyles
		{
			get { return null; }
		}

		public object[] PropertySpaces
		{
			get { return new string[] { "CUSTOMS" };  }
		}

		public void SetPropertyValues(object obj, object[] values)
		{
			Custom c = (Custom)obj;
			c.Name = (string)values[0];
		}

		public IType[] PropertyTypes
		{
			get { return Types; }
		}

		public bool IsIdentifierAssignedByInsert
		{
			get { return false; }
		}

		public System.Type ConcreteProxyClass
		{
			get { return typeof(Custom); }
		}

		public object GetIdentifier(object obj)
		{
			return (long)( (Custom)obj).Id;
		}

		public object GetPropertyValue(object obj, int i)
		{
			return ( (Custom)obj).Name;
		}

		public bool IsVersioned
		{
			get { return false; }
		}

		public bool HasProxy
		{
			get { return false; }
		}

		public void SetIdentifier(object obj, object id)
		{
			( (Custom)obj).Id = (long)id;
		}

		public bool ImplementsLifecycle
		{
			get { return false; }
		}

		public object[] GetPropertyValues(object obj)
		{
			Custom c = (Custom)obj;
			return new object[] {c.Name};
		}

		public string ClassName
		{
			get { return typeof(Custom).FullName; }
		}

		public bool HasIdentifierProperty
		{
			get { return false; }
		}

		public int VersionProperty
		{
			get { return 0; }
		}

		public IType IdentifierType
		{
			get { return NHibernateUtil.Int64; }
		}

		public System.Reflection.PropertyInfo ProxyIdentifierProperty
		{
			get { return null; }
		}

		public bool IsMutable
		{
			get { return true; }
		}

		public bool HasCollections
		{
			get { return false; }
		}

		public void Update(object id, object[] fields, int[] dirtyFields, object oldVersion, object obj, ISessionImplementor session)
		{
			Instances[id] = ( (Custom)obj).Clone();
		}

		public void Delete(object id, object version, object obj, ISessionImplementor session)
		{
			Instances.Remove(id);
		}

		public string[] PropertyNames
		{
			get { return Names; }
		}

		public void SetPropertyValue(object obj, int i, object value)
		{
			( (Custom)obj).Name = (string)value;
		}

		public object Load(object id, object optionalObject, LockMode lockMode, ISessionImplementor session)
		{
			Custom clone = null;
			Custom obj = (Custom) Instances[id];
			if(obj!=null) 
			{
				clone = (Custom)obj.Clone();
				session.AddUninitializedEntity( new Key(id, this), clone, LockMode.None );
				session.PostHydrate( this, id, new string[] {obj.Name}, clone, LockMode.None );
				session.InitializeEntity(clone);
			}
			return clone;

		}

		public ICacheConcurrencyStrategy Cache
		{
			get { return null; }
		}

		public bool ImplementsValidatable
		{
			get { return false; }
		}

		public void PostInstantiate(ISessionFactoryImplementor factory)
		{
		}

		public IVersionType VersionType
		{
			get { return null; }
		}

		public string IdentifierPropertyName
		{
			get { return "Id"; }
		}

		public System.Type[] ProxyInterfaces
		{
			get { return null; }
		}

		public void Lock(object id, object version, object obj, LockMode lockMode, ISessionImplementor session)
		{
			throw new NotSupportedException("CustomPersister.Lock is not implemented");
		}

		#endregion
	}
}
