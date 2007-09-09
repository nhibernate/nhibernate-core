using System;
using System.Collections.Generic;
using NHibernate.Cfg;

namespace NHibernate.Event
{
	/// <summary> 
	/// A convience holder for all defined session event listeners.
	/// </summary>
	[Serializable]
	public class EventListeners
	{
		private static readonly IDictionary<string, System.Type> eventInterfaceFromType =
			new Dictionary<string, System.Type>(28);
		static EventListeners()
		{
			eventInterfaceFromType["auto-flush"] = typeof(IAutoFlushEventListener);
			eventInterfaceFromType["merge"] = typeof(IMergeEventListener);
			eventInterfaceFromType["create"] = typeof(IPersistEventListener);
			eventInterfaceFromType["create-onflush"] = typeof(IPersistEventListener);
			eventInterfaceFromType["delete"] = typeof(IDeleteEventListener);
			eventInterfaceFromType["dirty-check"] = typeof(IDirtyCheckEventListener);
			eventInterfaceFromType["evict"] = typeof(IEvictEventListener);
			eventInterfaceFromType["flush"] = typeof(IFlushEventListener);
			eventInterfaceFromType["flush-entity"] = typeof(IFlushEntityEventListener);
			eventInterfaceFromType["load"] = typeof(ILoadEventListener);
			eventInterfaceFromType["load-collection"] = typeof(IInitializeCollectionEventListener);
			eventInterfaceFromType["lock"] = typeof(ILockEventListener);
			eventInterfaceFromType["refresh"] = typeof(IRefreshEventListener);
			eventInterfaceFromType["replicate"] = typeof(IReplicateEventListener);
			eventInterfaceFromType["save-update"] = typeof(ISaveOrUpdateEventListener);
			eventInterfaceFromType["save"] = typeof(ISaveOrUpdateEventListener);
			eventInterfaceFromType["update"] = typeof(ISaveOrUpdateEventListener);
			eventInterfaceFromType["pre-load"] = typeof(IPreLoadEventListener);
			eventInterfaceFromType["pre-update"] = typeof(IPreUpdateEventListener);
			eventInterfaceFromType["pre-delete"] = typeof(IPreDeleteEventListener);
			eventInterfaceFromType["pre-insert"] = typeof(IPreInsertEventListener);
			eventInterfaceFromType["post-load"] = typeof(IPostLoadEventListener);
			eventInterfaceFromType["post-update"] = typeof(IPostUpdateEventListener);
			eventInterfaceFromType["post-delete"] = typeof(IPostDeleteEventListener);
			eventInterfaceFromType["post-insert"] = typeof(IPostInsertEventListener);
			eventInterfaceFromType["post-commit-update"] = typeof(IPostUpdateEventListener);
			eventInterfaceFromType["post-commit-delete"] = typeof(IPostDeleteEventListener);
			eventInterfaceFromType["post-commit-insert"] = typeof(IPostInsertEventListener);
		}

		private ILoadEventListener[] loadEventListeners = new ILoadEventListener[] { }; // todo-events { new DefaultLoadEventListener() };
		private ISaveOrUpdateEventListener[] saveOrUpdateEventListeners = new ISaveOrUpdateEventListener[] { }; // todo-events { new DefaultSaveOrUpdateEventListener() };
		private IMergeEventListener[] mergeEventListeners = new IMergeEventListener[] { }; // todo-events { new DefaultMergeEventListener() };
		private IPersistEventListener[] persistEventListeners = new IPersistEventListener[] { }; // todo-events { new DefaultPersistEventListener() };
		private IPersistEventListener[] persistOnFlushEventListeners = new IPersistEventListener[] { }; // todo-events { new DefaultPersistOnFlushEventListener() };
		private IReplicateEventListener[] replicateEventListeners = new IReplicateEventListener[] { }; // todo-events { new DefaultReplicateEventListener() };
		private IDeleteEventListener[] deleteEventListeners = new IDeleteEventListener[] { }; // todo-events { new DefaultDeleteEventListener() };
		private IAutoFlushEventListener[] autoFlushEventListeners = new IAutoFlushEventListener[] { }; // todo-events { new DefaultAutoFlushEventListener() };
		private IDirtyCheckEventListener[] dirtyCheckEventListeners = new IDirtyCheckEventListener[] { }; // todo-events { new DefaultDirtyCheckEventListener() };
		private IFlushEventListener[] flushEventListeners = new IFlushEventListener[] { }; // todo-events { new DefaultFlushEventListener() };
		private IEvictEventListener[] evictEventListeners = new IEvictEventListener[] { }; // todo-events { new DefaultEvictEventListener() };
		private ILockEventListener[] lockEventListeners = new ILockEventListener[] { }; // todo-events { new DefaultLockEventListener() };
		private IRefreshEventListener[] refreshEventListeners = new IRefreshEventListener[] { }; // todo-events { new DefaultRefreshEventListener() };
		private IFlushEntityEventListener[] flushEntityEventListeners = new IFlushEntityEventListener[] { }; // todo-events { new DefaultFlushEntityEventListener() };
		private IInitializeCollectionEventListener[] initializeCollectionEventListeners = new IInitializeCollectionEventListener[] { }; // todo-events { new DefaultInitializeCollectionEventListener() };

		private IPostLoadEventListener[] postLoadEventListeners = new IPostLoadEventListener[] { }; // todo-events { new DefaultPostLoadEventListener() };
		private IPreLoadEventListener[] preLoadEventListeners = new IPreLoadEventListener[] { }; // todo-events { new DefaultPreLoadEventListener() };

		private IPreDeleteEventListener[] preDeleteEventListeners = new IPreDeleteEventListener[] { };
		private IPreUpdateEventListener[] preUpdateEventListeners = new IPreUpdateEventListener[] { };
		private IPreInsertEventListener[] preInsertEventListeners = new IPreInsertEventListener[] { };
		private IPostDeleteEventListener[] postDeleteEventListeners = new IPostDeleteEventListener[] { };
		private IPostUpdateEventListener[] postUpdateEventListeners = new IPostUpdateEventListener[] { };
		private IPostInsertEventListener[] postInsertEventListeners = new IPostInsertEventListener[] { };
		private IPostDeleteEventListener[] postCommitDeleteEventListeners = new IPostDeleteEventListener[] { };
		private IPostUpdateEventListener[] postCommitUpdateEventListeners = new IPostUpdateEventListener[] { };
		private IPostInsertEventListener[] postCommitInsertEventListeners = new IPostInsertEventListener[] { };

		private ISaveOrUpdateEventListener[] saveEventListeners = new ISaveOrUpdateEventListener[] { }; // todo-events { new DefaultSaveEventListener() };
		private ISaveOrUpdateEventListener[] updateEventListeners = new ISaveOrUpdateEventListener[] { }; // todo-events { new DefaultUpdateEventListener() };

		public ILoadEventListener[] LoadEventListeners
		{
			get { return loadEventListeners; }
			set { loadEventListeners = value; }
		}

		public ISaveOrUpdateEventListener[] SaveOrUpdateEventListeners
		{
			get { return saveOrUpdateEventListeners; }
			set
			{
				if (value != null)
				{
					saveOrUpdateEventListeners = value;
				}
			}
		}

		public IMergeEventListener[] MergeEventListeners
		{
			get { return mergeEventListeners; }
			set
			{
				if (value != null)
				{
					mergeEventListeners = value;
				}
			}
		}

		public IPersistEventListener[] PersistEventListeners
		{
			get { return persistEventListeners; }
			set
			{
				if (value != null)
				{
					persistEventListeners = value;
				}
			}
		}

		public IPersistEventListener[] PersistOnFlushEventListeners
		{
			get { return persistOnFlushEventListeners; }
			set
			{
				if (value != null)
				{
					persistOnFlushEventListeners = value;
				}
			}
		}

		public IReplicateEventListener[] ReplicateEventListeners
		{
			get { return replicateEventListeners; }
			set
			{
				if (value != null)
				{
					replicateEventListeners = value;
				}
			}
		}

		public IDeleteEventListener[] DeleteEventListeners
		{
			get { return deleteEventListeners; }
			set
			{
				if (value != null)
				{
					deleteEventListeners = value;
				}
			}
		}

		public IAutoFlushEventListener[] AutoFlushEventListeners
		{
			get { return autoFlushEventListeners; }
			set
			{
				if (value != null)
				{
					autoFlushEventListeners = value;
				}
			}
		}

		public IDirtyCheckEventListener[] DirtyCheckEventListeners
		{
			get { return dirtyCheckEventListeners; }
			set
			{
				if (value != null)
				{
					dirtyCheckEventListeners = value;
				}
			}
		}

		public IFlushEventListener[] FlushEventListeners
		{
			get { return flushEventListeners; }
			set
			{
				if (value != null)
				{
					flushEventListeners = value;
				}
			}
		}

		public IEvictEventListener[] EvictEventListeners
		{
			get { return evictEventListeners; }
			set
			{
				if (value != null)
				{
					evictEventListeners = value;
				}
			}
		}

		public ILockEventListener[] LockEventListeners
		{
			get { return lockEventListeners; }
			set
			{
				if (value != null)
				{
					lockEventListeners = value;
				}
			}
		}

		public IRefreshEventListener[] RefreshEventListeners
		{
			get { return refreshEventListeners; }
			set
			{
				if (value != null)
				{
					refreshEventListeners = value;
				}
			}
		}

		public IFlushEntityEventListener[] FlushEntityEventListeners
		{
			get { return flushEntityEventListeners; }
			set
			{
				if (value != null)
				{
					flushEntityEventListeners = value;
				}
			}
		}

		public IInitializeCollectionEventListener[] InitializeCollectionEventListeners
		{
			get { return initializeCollectionEventListeners; }
			set
			{
				if (value != null)
				{
					initializeCollectionEventListeners = value;
				}
			}
		}

		public IPostLoadEventListener[] PostLoadEventListeners
		{
			get { return postLoadEventListeners; }
			set
			{
				if (value != null)
				{
					postLoadEventListeners = value;
				}
			}
		}

		public IPreLoadEventListener[] PreLoadEventListeners
		{
			get { return preLoadEventListeners; }
			set
			{
				if (value != null)
				{
					preLoadEventListeners = value;
				}
			}
		}

		public IPreDeleteEventListener[] PreDeleteEventListeners
		{
			get { return preDeleteEventListeners; }
			set
			{
				if (value != null)
				{
					preDeleteEventListeners = value;
				}
			}
		}

		public IPreUpdateEventListener[] PreUpdateEventListeners
		{
			get { return preUpdateEventListeners; }
			set
			{
				if (value != null)
				{
					preUpdateEventListeners = value;
				}
			}
		}

		public IPreInsertEventListener[] PreInsertEventListeners
		{
			get { return preInsertEventListeners; }
			set
			{
				if (value != null)
				{
					preInsertEventListeners = value;
				}
			}
		}

		public IPostDeleteEventListener[] PostDeleteEventListeners
		{
			get { return postDeleteEventListeners; }
			set
			{
				if (value != null)
				{
					postDeleteEventListeners = value;
				}
			}
		}

		public IPostUpdateEventListener[] PostUpdateEventListeners
		{
			get { return postUpdateEventListeners; }
			set
			{
				if (value != null)
				{
					postUpdateEventListeners = value;
				}
			}
		}

		public IPostInsertEventListener[] PostInsertEventListeners
		{
			get { return postInsertEventListeners; }
			set
			{
				if (value != null)
				{
					postInsertEventListeners = value;
				}
			}
		}

		public IPostDeleteEventListener[] PostCommitDeleteEventListeners
		{
			get { return postCommitDeleteEventListeners; }
			set
			{
				if (value != null)
				{
					postCommitDeleteEventListeners = value;
				}
			}
		}

		public IPostUpdateEventListener[] PostCommitUpdateEventListeners
		{
			get { return postCommitUpdateEventListeners; }
			set
			{
				if (value != null)
				{
					postCommitUpdateEventListeners = value;
				}
			}
		}

		public IPostInsertEventListener[] PostCommitInsertEventListeners
		{
			get { return postCommitInsertEventListeners; }
			set
			{
				if (value != null)
				{
					postCommitInsertEventListeners = value;
				}
			}
		}

		public ISaveOrUpdateEventListener[] SaveEventListeners
		{
			get { return saveEventListeners; }
			set
			{
				if (value != null)
				{
					saveEventListeners = value;
				}
			}
		}

		public ISaveOrUpdateEventListener[] UpdateEventListeners
		{
			get { return updateEventListeners; }
			set
			{
				if (value != null)
				{
					updateEventListeners = value;
				}
			}
		}

		public System.Type GetListenerClassFor(string type)
		{
			if (!eventInterfaceFromType.ContainsKey(type))
				throw new MappingException("Unrecognized listener type [" + type + "]");

			return eventInterfaceFromType[type];
		}

		/// <summary> 
		/// Call <see cref="IInitializable.Initialize(Configuration)"/> on any listeners that implement 
		/// <see cref="IInitializable"/>.
		/// </summary>
		/// <seealso cref="IInitializable"/>
		public virtual void InitializeListeners(Configuration cfg)
		{
			foreach (IInitializable initializable in loadEventListeners)
				initializable.Initialize(cfg);
			foreach (IInitializable initializable in saveOrUpdateEventListeners)
				initializable.Initialize(cfg);
			foreach (IInitializable initializable in mergeEventListeners)
				initializable.Initialize(cfg);
			foreach (IInitializable initializable in persistEventListeners)
				initializable.Initialize(cfg);
			foreach (IInitializable initializable in persistOnFlushEventListeners)
				initializable.Initialize(cfg);
			foreach (IInitializable initializable in replicateEventListeners)
				initializable.Initialize(cfg);
			foreach (IInitializable initializable in deleteEventListeners)
				initializable.Initialize(cfg);
			foreach (IInitializable initializable in autoFlushEventListeners)
				initializable.Initialize(cfg);
			foreach (IInitializable initializable in dirtyCheckEventListeners)
				initializable.Initialize(cfg);
			foreach (IInitializable initializable in flushEventListeners)
				initializable.Initialize(cfg);
			foreach (IInitializable initializable in evictEventListeners)
				initializable.Initialize(cfg);
			foreach (IInitializable initializable in lockEventListeners)
				initializable.Initialize(cfg);
			foreach (IInitializable initializable in refreshEventListeners)
				initializable.Initialize(cfg);
			foreach (IInitializable initializable in flushEntityEventListeners)
				initializable.Initialize(cfg);
			foreach (IInitializable initializable in initializeCollectionEventListeners)
				initializable.Initialize(cfg);
			foreach (IInitializable initializable in postLoadEventListeners)
				initializable.Initialize(cfg);
			foreach (IInitializable initializable in preLoadEventListeners)
				initializable.Initialize(cfg);
			foreach (IInitializable initializable in preDeleteEventListeners)
				initializable.Initialize(cfg);
			foreach (IInitializable initializable in preUpdateEventListeners)
				initializable.Initialize(cfg);
			foreach (IInitializable initializable in preInsertEventListeners)
				initializable.Initialize(cfg);
			foreach (IInitializable initializable in postDeleteEventListeners)
				initializable.Initialize(cfg);
			foreach (IInitializable initializable in postUpdateEventListeners)
				initializable.Initialize(cfg);
			foreach (IInitializable initializable in postInsertEventListeners)
				initializable.Initialize(cfg);
			foreach (IInitializable initializable in postCommitDeleteEventListeners)
				initializable.Initialize(cfg);
			foreach (IInitializable initializable in postCommitUpdateEventListeners)
				initializable.Initialize(cfg);
			foreach (IInitializable initializable in postCommitInsertEventListeners)
				initializable.Initialize(cfg);
			foreach (IInitializable initializable in saveEventListeners)
				initializable.Initialize(cfg);
			foreach (IInitializable initializable in updateEventListeners)
				initializable.Initialize(cfg);
		}
	}
}
