using System;
using Iesi.Collections.Generic;
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
		private ISet<string> unwrapProxyFieldNames;
		private readonly string entityName;

		[NonSerialized]
		private bool initializing;
		private bool isDirty;

		protected internal AbstractFieldInterceptor(ISessionImplementor session, ISet<string> uninitializedFields, ISet<string> unwrapProxyFieldNames, string entityName)
		{
			this.session = session;
			this.uninitializedFields = uninitializedFields;
			this.unwrapProxyFieldNames = unwrapProxyFieldNames;
			this.entityName = entityName;
		}

		#region IFieldInterceptor Members

		public bool IsDirty
		{
			get { return isDirty; }
		}

		public void SetSession(ISessionImplementor session)
		{
			this.session = session;
		}

		public bool IsInitialized
		{
			get { return uninitializedFields == null || uninitializedFields.Count == 0; }
		}

		public bool IsInitializedField(string field)
		{
			return uninitializedFields == null || !uninitializedFields.Contains(field);
		}

		public void MarkDirty()
		{
			isDirty = true;
		}

		public void ClearDirty()
		{
			isDirty = false;
		}

		#endregion

		public ISet<string> UninitializedFields
		{
			get { return uninitializedFields; }
		}

		public string EntityName
		{
			get { return entityName; }
		}

		public bool Initializing
		{
			get { return initializing; }
		}

		public object Intercept(object target, string fieldName, object value)
		{
			if (initializing)
				return InvokeImplementation;

			if (session == null)
			{
				throw new LazyInitializationException("entity with lazy properties is not associated with a session");
			}
			if (!session.IsOpen || !session.IsConnected)
			{
				throw new LazyInitializationException("session is not connected");
			}

			if (uninitializedFields != null && uninitializedFields.Contains(fieldName))
			{
				return InitializeField(fieldName, target);
			}
			if (value is INHibernateProxy && unwrapProxyFieldNames != null && unwrapProxyFieldNames.Contains(fieldName))
			{
				return InitializeOrGetAssociation((INHibernateProxy)value);
			}
			return InvokeImplementation;
		}

		private object InitializeOrGetAssociation(INHibernateProxy value)
		{
			if(value.HibernateLazyInitializer.IsUninitialized)
			{
				value.HibernateLazyInitializer.Initialize();
				var association = value.HibernateLazyInitializer.GetImplementation(session);
				//var narrowedProxy = session.PersistenceContext.ProxyFor(association);
				// we set the narrowed impl here to be able to get it back in the future
				value.HibernateLazyInitializer.SetImplementation(association);
				var entityPersister = session.GetEntityPersister(value.HibernateLazyInitializer.EntityName, value);
				var key = new EntityKey(value.HibernateLazyInitializer.Identifier,
				                              entityPersister,
				                              session.EntityMode);
				session.PersistenceContext.RemoveProxy(key);
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
