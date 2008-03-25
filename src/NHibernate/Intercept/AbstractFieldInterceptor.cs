using System;
using Iesi.Collections.Generic;
using NHibernate.Engine;

namespace NHibernate.Intercept
{
	[Serializable]
	public abstract class AbstractFieldInterceptor : IFieldInterceptor
	{
		[NonSerialized]
		private ISessionImplementor session;
		private ISet<string> uninitializedFields;
		private readonly string entityName;

		[NonSerialized]
		private bool initializing;
		private bool isDirty;

		protected internal AbstractFieldInterceptor(ISessionImplementor session, ISet<string> uninitializedFields, string entityName)
		{
			this.session = session;
			this.uninitializedFields = uninitializedFields;
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

		protected internal object Intercept(object target, string fieldName, object value)
		{
			if (initializing)
			{
				return value;
			}

			if (uninitializedFields != null && uninitializedFields.Contains(fieldName))
			{
				if (session == null)
				{
					throw new LazyInitializationException("entity with lazy properties is not associated with a session");
				}
				else if (!session.IsOpen || !session.IsConnected)
				{
					throw new LazyInitializationException("session is not connected");
				}

				object result;
				initializing = true;
				try
				{
					result = ((ILazyPropertyInitializer)session.Factory.GetEntityPersister(entityName)).InitializeLazyProperty(fieldName, target, session);
				}
				finally
				{
					initializing = false;
				}
				uninitializedFields = null; //let's assume that there is only one lazy fetch group, for now!
				return result;
			}
			else
			{
				return value;
			}
		}
	}
}
