using System;
using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Proxy;

namespace NHibernate.Intercept
{
	[Serializable]
	public abstract class AbstractFieldInterceptor : IFieldInterceptor
	{
		public static readonly object InvokeImplementation = new object();

		[NonSerialized]
		private ISessionImplementor session;
		private ISet<string> uninitializedFields;
		private readonly ISet<string> unwrapProxyFieldNames;
		private readonly HashSet<string> loadedUnwrapProxyFieldNames = new HashSet<string>();
		private readonly string entityName;
		private readonly System.Type mappedClass;

		[NonSerialized]
		private bool initializing;
		private bool isDirty;

		protected internal AbstractFieldInterceptor(ISessionImplementor session, ISet<string> uninitializedFields, ISet<string> unwrapProxyFieldNames, string entityName, System.Type mappedClass)
		{
			this.session = session;
			this.uninitializedFields = uninitializedFields;
			this.unwrapProxyFieldNames = unwrapProxyFieldNames ?? new HashSet<string>();
			this.entityName = entityName;
			this.mappedClass = mappedClass;
		}

		#region IFieldInterceptor Members

		public bool IsDirty
		{
			get { return isDirty; }
		}

		public ISessionImplementor Session
		{
			get { return session; }
			set { session = value; }
		}

		public bool IsInitialized
		{
			get { return uninitializedFields == null || uninitializedFields.Count == 0; }
		}

		public bool IsInitializedField(string field)
		{
			return !IsUninitializedProperty(field) && !IsUninitializedAssociation(field);
		}

		public void MarkDirty()
		{
			isDirty = true;
		}

		public void ClearDirty()
		{
			isDirty = false;
		}

		public string EntityName
		{
			get { return entityName; }
		}

		public System.Type MappedClass
		{
			get { return mappedClass; }
		}

		#endregion

		public ISet<string> UninitializedFields
		{
			get { return uninitializedFields; }
		}

		public bool Initializing
		{
			get { return initializing; }
		}

		public object Intercept(object target, string fieldName, object value)
		{
			// NH Specific: Hibernate only deals with lazy properties here, we deal with 
			// both lazy properties and with no-proxy. 
			if (initializing)
			{
				return InvokeImplementation;
			}

			if (IsInitializedField(fieldName))
			{
				if (value.IsProxy() && IsInitializedAssociation(fieldName))
					return InitializeOrGetAssociation((INHibernateProxy) value, fieldName);

				return value;
			}

			if (session == null)
			{
				throw new LazyInitializationException(EntityName, null, string.Format("entity with lazy properties is not associated with a session. entity-name:'{0}' property:'{1}'", EntityName, fieldName));
			}
			if (!session.IsOpen || !session.IsConnected)
			{
				throw new LazyInitializationException(EntityName, null, string.Format("session is not connected. entity-name:'{0}' property:'{1}'", EntityName, fieldName));
			}

			if (IsUninitializedProperty(fieldName))
			{
				return InitializeField(fieldName, target);
			}

			if (value.IsProxy() && IsUninitializedAssociation(fieldName))
			{
				var nhproxy = value as INHibernateProxy;
				return InitializeOrGetAssociation(nhproxy, fieldName);
			}
			return InvokeImplementation;
		}

		private bool IsInitializedAssociation(string fieldName)
		{
			return loadedUnwrapProxyFieldNames.Contains(fieldName);
		}

		private bool IsUninitializedAssociation(string fieldName)
		{
			return unwrapProxyFieldNames.Contains(fieldName) && !loadedUnwrapProxyFieldNames.Contains(fieldName);
		}

		private bool IsUninitializedProperty(string fieldName)
		{
			return uninitializedFields != null && uninitializedFields.Contains(fieldName);
		}

		private object InitializeOrGetAssociation(INHibernateProxy value, string fieldName)
		{
			if(value.HibernateLazyInitializer.IsUninitialized)
			{
				value.HibernateLazyInitializer.Initialize();
				value.HibernateLazyInitializer.Unwrap = true; // means that future Load/Get from the session will get the implementation
				loadedUnwrapProxyFieldNames.Add(fieldName);
			}
			return value.HibernateLazyInitializer.GetImplementation(session);
		}

		private object InitializeField(string fieldName, object target)
		{
			object result;
			initializing = true;
			try
			{
				var lazyPropertyInitializer = ((ILazyPropertyInitializer) session.Factory.GetEntityPersister(entityName));
				result = lazyPropertyInitializer.InitializeLazyProperty(fieldName, target, session);
			}
			finally
			{
				initializing = false;
			}
			uninitializedFields = null; //let's assume that there is only one lazy fetch group, for now!
			return result;
		}
	}
}
