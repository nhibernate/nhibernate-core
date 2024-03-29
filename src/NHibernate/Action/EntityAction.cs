using System;
using System.IO;
using System.Runtime.Serialization;
using NHibernate.Engine;
using NHibernate.Persister.Entity;
using NHibernate.Util;
using NHibernate.Impl;
using NHibernate.Persister;

namespace NHibernate.Action
{
	/// <summary>
	/// Base class for actions relating to insert/update/delete of an entity
	/// instance.
	/// </summary>
	[Serializable]
	public abstract partial class EntityAction : 
		IAsyncExecutable,
		IBeforeTransactionCompletionProcess,
		IAfterTransactionCompletionProcess,
		IComparable<EntityAction>, 
		IDeserializationCallback,
		ICacheableExecutable
	{
		private readonly string entityName;
		private readonly object id;
		private readonly object instance;
		private readonly ISessionImplementor session;

		[NonSerialized]
		private IEntityPersister persister;

		/// <summary>
		/// Instantiate an action.
		/// </summary>
		/// <param name="session">The session from which this action is coming.</param>
		/// <param name="id">The id of the entity</param>
		/// <param name="instance">The entity instance</param>
		/// <param name="persister">The entity persister</param>
		protected internal EntityAction(ISessionImplementor session, object id, object instance, IEntityPersister persister)
		{
			entityName = persister.EntityName;
			this.id = id;
			this.instance = instance;
			this.session = session;
			this.persister = persister;
		}

		/// <summary>
		/// Entity name accessor
		/// </summary>
		public string EntityName
		{
			get { return entityName; }
		}

		/// <summary>
		/// Entity Id accessor
		/// </summary>
		public object Id
		{
			get
			{
				if (id is DelayedPostInsertIdentifier)
				{
					return session.PersistenceContext.GetEntry(instance).Id;
				}
				return id;
			}
		}

		/// <summary>
		/// Entity Instance
		/// </summary>
		public object Instance
		{
			get { return instance; }
		}

		/// <summary>
		/// Session from which this action originated
		/// </summary>
		public ISessionImplementor Session
		{
			get { return session; }
		}

		/// <summary>
		/// The entity persister.
		/// </summary>
		public IEntityPersister Persister
		{
			get { return persister; }
		}

		protected internal abstract bool HasPostCommitEventListeners { get; }

		#region IExecutable Members

		public string[] QueryCacheSpaces
		{
			get
			{
				// 6.0 TODO: Use IPersister.SupportsQueryCache property once IPersister's todo is done.
				return persister.SupportsQueryCache() ? persister.PropertySpaces : null;
			}
		}

		public string[] PropertySpaces
		{
			get { return persister.PropertySpaces; }
		}

		public void BeforeExecutions()
		{
			throw new AssertionFailure("BeforeExecutions() called for non-collection action");
		}

		public abstract void Execute();

		//Since v5.2
		[Obsolete("This property is not used and will be removed in a future version.")]
		public virtual BeforeTransactionCompletionProcessDelegate BeforeTransactionCompletionProcess =>
			NeedsBeforeTransactionCompletion()
				? BeforeTransactionCompletionProcessImpl
				: default(BeforeTransactionCompletionProcessDelegate);

		//Since v5.2
		[Obsolete("This property is not used and will be removed in a future version.")]
		public virtual AfterTransactionCompletionProcessDelegate AfterTransactionCompletionProcess =>
			NeedsAfterTransactionCompletion()
				? AfterTransactionCompletionProcessImpl
				: default(AfterTransactionCompletionProcessDelegate);

		IBeforeTransactionCompletionProcess IAsyncExecutable.BeforeTransactionCompletionProcess =>
			NeedsBeforeTransactionCompletion() ? this : null;

		IAfterTransactionCompletionProcess IAsyncExecutable.AfterTransactionCompletionProcess =>
			NeedsAfterTransactionCompletion() ? this : null;

		protected virtual bool NeedsAfterTransactionCompletion()
		{
			return persister.HasCache || HasPostCommitEventListeners;
		}

		protected virtual bool NeedsBeforeTransactionCompletion()
		{
			// At the moment, there is no need to add the delegate, 
			// Subclasses can override this method and add the delegate if needed.
			return false;
		}

		protected virtual void BeforeTransactionCompletionProcessImpl()
		{
		}
		
		protected virtual void AfterTransactionCompletionProcessImpl(bool success)
		{
		}

		#endregion

		#region IComparable<EntityAction> Members

		public virtual int CompareTo(EntityAction other)
		{
			//sort first by entity name
			int roleComparison = entityName.CompareTo(other.entityName);
			if (roleComparison != 0)
			{
				return roleComparison;
			}
			//then by id
			return persister.IdentifierType.Compare(id, other.id);
		}

		#endregion

		#region IDeserializationCallback Members

		void IDeserializationCallback.OnDeserialization(object sender)
		{
			try
			{
				persister = session.Factory.GetEntityPersister(entityName);
			}
			catch (MappingException e)
			{
				throw new IOException("Unable to resolve class persister on deserialization", e);
			}
		}

		#endregion

		public override string ToString()
		{
			return StringHelper.Unqualify(GetType().FullName) + MessageHelper.InfoString(entityName, id);
		}

		public void ExecuteBeforeTransactionCompletion()
		{
			BeforeTransactionCompletionProcessImpl();
		}

		public void ExecuteAfterTransactionCompletion(bool success)
		{
			AfterTransactionCompletionProcessImpl(success);
		}
	}
}
