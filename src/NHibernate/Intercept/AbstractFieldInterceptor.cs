using System;
using System.Collections.Generic;
using Iesi.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Persister.Entity;
using NHibernate.Proxy;
using NHibernate.Util;

namespace NHibernate.Intercept
{
	[Serializable]
	public abstract class AbstractFieldInterceptor : IFieldInterceptor
	{
		public static readonly object InvokeImplementation = new object();

		[NonSerialized]
		private ISessionImplementor session;
		private ISet<string> uninitializedFields;
		private ISet<string> uninitializedFieldsReadOnly;
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
			this.uninitializedFieldsReadOnly = uninitializedFields != null ? new ReadOnlySet<string>(uninitializedFields) : null;
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

		// Since v5.3
		[Obsolete("Please use GetUninitializedFields extension method instead")]
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
			return Intercept(target, fieldName, value, false);
		}

		public object Intercept(object target, string fieldName, object value, bool setter)
		{
			if (setter)
			{
				if (IsUninitializedProperty(fieldName))
				{
					uninitializedFields.Remove(fieldName);
				}

				if (!unwrapProxyFieldNames.Contains(fieldName))
				{
					return value;
				}

				// When a proxy is set by the user which we know when the session is set, we should not unwrap it
				if (session != null || !value.IsProxy())
				{
					loadedUnwrapProxyFieldNames.Add(fieldName);
				}

				return value;
			}

			// NH Specific: Hibernate only deals with lazy properties here, we deal with 
			// both lazy properties and with no-proxy. 
			if (initializing)
			{
				return InvokeImplementation;
			}

			if (IsInitializedField(fieldName))
			{
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
				value = InitializeOrGetAssociation(nhproxy, fieldName);
				// Set the property value in order to be accessible when the session is closed
				var persister = session.Factory.GetEntityPersister(entityName);
				persister.SetPropertyValue(
					target,
					persister.EntityMetamodel.BytecodeEnhancementMetadata.UnwrapProxyPropertiesMetadata.GetUnwrapProxyPropertyIndex(fieldName),
					value);

				return value;
			}
			return InvokeImplementation;
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

			return result;
		}

		public ISet<string> GetUninitializedFields()
		{
			return uninitializedFieldsReadOnly ?? CollectionHelper.EmptySet<string>();
		}
	}
}
