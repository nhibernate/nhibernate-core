using System;
using System.Collections.Generic;
using NHibernate.Cfg;
using NHibernate.Event.Default;
using NHibernate.Util;

namespace NHibernate.Event
{
	/// <summary> 
	/// A convience holder for all defined session event listeners.
	/// </summary>
	[Serializable]
	public class EventListeners
	{
        private readonly List<object> initializedListeners = new List<object>();

		private static readonly IDictionary<ListenerType, System.Type> eventInterfaceFromType =
			new Dictionary<ListenerType, System.Type>(28);

		static EventListeners()
		{
			eventInterfaceFromType[ListenerType.Autoflush] = typeof (IAutoFlushEventListener);
			eventInterfaceFromType[ListenerType.Merge] = typeof (IMergeEventListener);
			eventInterfaceFromType[ListenerType.Create] = typeof (IPersistEventListener);
			eventInterfaceFromType[ListenerType.CreateOnFlush] = typeof (IPersistEventListener);
			eventInterfaceFromType[ListenerType.Delete] = typeof (IDeleteEventListener);
			eventInterfaceFromType[ListenerType.DirtyCheck] = typeof (IDirtyCheckEventListener);
			eventInterfaceFromType[ListenerType.Evict] = typeof (IEvictEventListener);
			eventInterfaceFromType[ListenerType.Flush] = typeof (IFlushEventListener);
			eventInterfaceFromType[ListenerType.FlushEntity] = typeof (IFlushEntityEventListener);
			eventInterfaceFromType[ListenerType.Load] = typeof (ILoadEventListener);
			eventInterfaceFromType[ListenerType.LoadCollection] = typeof (IInitializeCollectionEventListener);
			eventInterfaceFromType[ListenerType.Lock] = typeof (ILockEventListener);
			eventInterfaceFromType[ListenerType.Refresh] = typeof (IRefreshEventListener);
			eventInterfaceFromType[ListenerType.Replicate] = typeof (IReplicateEventListener);
			eventInterfaceFromType[ListenerType.SaveUpdate] = typeof (ISaveOrUpdateEventListener);
			eventInterfaceFromType[ListenerType.Save] = typeof (ISaveOrUpdateEventListener);
			eventInterfaceFromType[ListenerType.Update] = typeof (ISaveOrUpdateEventListener);
			eventInterfaceFromType[ListenerType.PreLoad] = typeof (IPreLoadEventListener);
			eventInterfaceFromType[ListenerType.PreUpdate] = typeof (IPreUpdateEventListener);
			eventInterfaceFromType[ListenerType.PreDelete] = typeof (IPreDeleteEventListener);
			eventInterfaceFromType[ListenerType.PreInsert] = typeof (IPreInsertEventListener);
			eventInterfaceFromType[ListenerType.PreCollectionRecreate] = typeof (IPreCollectionRecreateEventListener);
			eventInterfaceFromType[ListenerType.PreCollectionRemove] = typeof (IPreCollectionRemoveEventListener);
			eventInterfaceFromType[ListenerType.PreCollectionUpdate] = typeof (IPreCollectionUpdateEventListener);
			eventInterfaceFromType[ListenerType.PostLoad] = typeof (IPostLoadEventListener);
			eventInterfaceFromType[ListenerType.PostUpdate] = typeof (IPostUpdateEventListener);
			eventInterfaceFromType[ListenerType.PostDelete] = typeof (IPostDeleteEventListener);
			eventInterfaceFromType[ListenerType.PostInsert] = typeof (IPostInsertEventListener);
			eventInterfaceFromType[ListenerType.PostCommitUpdate] = typeof (IPostUpdateEventListener);
			eventInterfaceFromType[ListenerType.PostCommitDelete] = typeof (IPostDeleteEventListener);
			eventInterfaceFromType[ListenerType.PostCommitInsert] = typeof (IPostInsertEventListener);
			eventInterfaceFromType[ListenerType.PostCollectionRecreate] = typeof (IPostCollectionRecreateEventListener);
			eventInterfaceFromType[ListenerType.PostCollectionRemove] = typeof (IPostCollectionRemoveEventListener);
			eventInterfaceFromType[ListenerType.PostCollectionUpdate] = typeof (IPostCollectionUpdateEventListener);
			eventInterfaceFromType = new UnmodifiableDictionary<ListenerType, System.Type>(eventInterfaceFromType);
		}

		private ILoadEventListener[] loadEventListeners = new ILoadEventListener[] {new DefaultLoadEventListener()};

		private ISaveOrUpdateEventListener[] saveOrUpdateEventListeners = new ISaveOrUpdateEventListener[]
		                                                                  	{new DefaultSaveOrUpdateEventListener()};

		private IMergeEventListener[] mergeEventListeners = new IMergeEventListener[] {new DefaultMergeEventListener()};

		private IPersistEventListener[] persistEventListeners = new IPersistEventListener[]
		                                                        	{new DefaultPersistEventListener()};

		private IPersistEventListener[] persistOnFlushEventListeners = new IPersistEventListener[]
		                                                               	{new DefaultPersistOnFlushEventListener()};

		private IReplicateEventListener[] replicateEventListeners = new IReplicateEventListener[]
		                                                            	{new DefaultReplicateEventListener()};

		private IDeleteEventListener[] deleteEventListeners = new IDeleteEventListener[] {new DefaultDeleteEventListener()};

		private IAutoFlushEventListener[] autoFlushEventListeners = new IAutoFlushEventListener[]
		                                                            	{new DefaultAutoFlushEventListener()};

		private IDirtyCheckEventListener[] dirtyCheckEventListeners = new IDirtyCheckEventListener[]
		                                                              	{new DefaultDirtyCheckEventListener()};

		private IFlushEventListener[] flushEventListeners = new IFlushEventListener[] {new DefaultFlushEventListener()};
		private IEvictEventListener[] evictEventListeners = new IEvictEventListener[] {new DefaultEvictEventListener()};
		private ILockEventListener[] lockEventListeners = new ILockEventListener[] {new DefaultLockEventListener()};

		private IRefreshEventListener[] refreshEventListeners = new IRefreshEventListener[]
		                                                        	{new DefaultRefreshEventListener()};

		private IFlushEntityEventListener[] flushEntityEventListeners = new IFlushEntityEventListener[]
		                                                                	{new DefaultFlushEntityEventListener()};

		private IInitializeCollectionEventListener[] initializeCollectionEventListeners =
			new IInitializeCollectionEventListener[] {new DefaultInitializeCollectionEventListener()};

		private IPostLoadEventListener[] postLoadEventListeners = new IPostLoadEventListener[]
		                                                          	{new DefaultPostLoadEventListener()};

		private IPreLoadEventListener[] preLoadEventListeners = new IPreLoadEventListener[]
		                                                        	{new DefaultPreLoadEventListener()};

		private IPreDeleteEventListener[] preDeleteEventListeners = new IPreDeleteEventListener[] {};
		private IPreUpdateEventListener[] preUpdateEventListeners = new IPreUpdateEventListener[] {};
		private IPreInsertEventListener[] preInsertEventListeners = new IPreInsertEventListener[] {};
		private IPostDeleteEventListener[] postDeleteEventListeners = new IPostDeleteEventListener[] {};
		private IPostUpdateEventListener[] postUpdateEventListeners = new IPostUpdateEventListener[] {};
		private IPostInsertEventListener[] postInsertEventListeners = new IPostInsertEventListener[] {};
		private IPostDeleteEventListener[] postCommitDeleteEventListeners = new IPostDeleteEventListener[] {};
		private IPostUpdateEventListener[] postCommitUpdateEventListeners = new IPostUpdateEventListener[] {};
		private IPostInsertEventListener[] postCommitInsertEventListeners = new IPostInsertEventListener[] {};

		private IPreCollectionRecreateEventListener[] preCollectionRecreateEventListeners =
			new IPreCollectionRecreateEventListener[] {};

		private IPostCollectionRecreateEventListener[] postCollectionRecreateEventListeners =
			new IPostCollectionRecreateEventListener[] {};

		private IPreCollectionRemoveEventListener[] preCollectionRemoveEventListeners =
			new IPreCollectionRemoveEventListener[] {};

		private IPostCollectionRemoveEventListener[] postCollectionRemoveEventListeners =
			new IPostCollectionRemoveEventListener[] {};

		private IPreCollectionUpdateEventListener[] preCollectionUpdateEventListeners =
			new IPreCollectionUpdateEventListener[] {};

		private IPostCollectionUpdateEventListener[] postCollectionUpdateEventListeners =
			new IPostCollectionUpdateEventListener[] {};

		private ISaveOrUpdateEventListener[] saveEventListeners = new ISaveOrUpdateEventListener[]
		                                                          	{new DefaultSaveEventListener()};

		private ISaveOrUpdateEventListener[] updateEventListeners = new ISaveOrUpdateEventListener[]
		                                                            	{new DefaultUpdateEventListener()};

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

		public IPreCollectionRecreateEventListener[] PreCollectionRecreateEventListeners
		{
			get { return preCollectionRecreateEventListeners; }
			set
			{
				if (value != null)
				{
					preCollectionRecreateEventListeners = value;
				}
			}
		}

		public IPostCollectionRecreateEventListener[] PostCollectionRecreateEventListeners
		{
			get { return postCollectionRecreateEventListeners; }
			set
			{
				if (value != null)
				{
					postCollectionRecreateEventListeners = value;
				}
			}
		}

		public IPreCollectionRemoveEventListener[] PreCollectionRemoveEventListeners
		{
			get { return preCollectionRemoveEventListeners; }
			set
			{
				if (value != null)
				{
					preCollectionRemoveEventListeners = value;
				}
			}
		}

		public IPostCollectionRemoveEventListener[] PostCollectionRemoveEventListeners
		{
			get { return postCollectionRemoveEventListeners; }
			set
			{
				if (value != null)
				{
					postCollectionRemoveEventListeners = value;
				}
			}
		}

		public IPreCollectionUpdateEventListener[] PreCollectionUpdateEventListeners
		{
			get { return preCollectionUpdateEventListeners; }
			set
			{
				if (value != null)
				{
					preCollectionUpdateEventListeners = value;
				}
			}
		}

		public IPostCollectionUpdateEventListener[] PostCollectionUpdateEventListeners
		{
			get { return postCollectionUpdateEventListeners; }
			set
			{
				if (value != null)
				{
					postCollectionUpdateEventListeners = value;
				}
			}
		}

		public System.Type GetListenerClassFor(ListenerType type)
		{
			System.Type result;
			if (!eventInterfaceFromType.TryGetValue(type, out result))
			{
				throw new MappingException("Unrecognized listener type [" + type + "]");
			}

			return result;
		}

		/// <summary> 
		/// Call <see cref="IInitializable.Initialize(Configuration)"/> on any listeners that implement 
		/// <see cref="IInitializable"/>.
		/// </summary>
		/// <seealso cref="IInitializable"/>
		public virtual void InitializeListeners(Configuration cfg)
		{
			InitializeListeners(cfg, loadEventListeners);
			InitializeListeners(cfg, saveOrUpdateEventListeners);
			InitializeListeners(cfg, mergeEventListeners);
			InitializeListeners(cfg, persistEventListeners);
			InitializeListeners(cfg, persistOnFlushEventListeners);
			InitializeListeners(cfg, replicateEventListeners);
			InitializeListeners(cfg, deleteEventListeners);
			InitializeListeners(cfg, autoFlushEventListeners);
			InitializeListeners(cfg, dirtyCheckEventListeners);
			InitializeListeners(cfg, flushEventListeners);
			InitializeListeners(cfg, evictEventListeners);
			InitializeListeners(cfg, lockEventListeners);
			InitializeListeners(cfg, refreshEventListeners);
			InitializeListeners(cfg, flushEntityEventListeners);
			InitializeListeners(cfg, initializeCollectionEventListeners);
			InitializeListeners(cfg, postLoadEventListeners);
			InitializeListeners(cfg, preLoadEventListeners);
			InitializeListeners(cfg, preDeleteEventListeners);
			InitializeListeners(cfg, preUpdateEventListeners);
			InitializeListeners(cfg, preInsertEventListeners);
			InitializeListeners(cfg, postDeleteEventListeners);
			InitializeListeners(cfg, postUpdateEventListeners);
			InitializeListeners(cfg, postInsertEventListeners);
			InitializeListeners(cfg, postCommitDeleteEventListeners);
			InitializeListeners(cfg, postCommitUpdateEventListeners);
			InitializeListeners(cfg, postCommitInsertEventListeners);
			InitializeListeners(cfg, saveEventListeners);
			InitializeListeners(cfg, updateEventListeners);

			InitializeListeners(cfg, preCollectionRecreateEventListeners);
			InitializeListeners(cfg, postCollectionRecreateEventListeners);
			InitializeListeners(cfg, preCollectionRemoveEventListeners);
			InitializeListeners(cfg, postCollectionRemoveEventListeners);
			InitializeListeners(cfg, preCollectionUpdateEventListeners);
			InitializeListeners(cfg, postCollectionUpdateEventListeners);
		}

		private void InitializeListeners(Configuration cfg, object[] list)
		{
		    initializedListeners.AddRange(list);
			foreach (object i in list)
			{
				IInitializable initializable = i as IInitializable;
				if (initializable != null)
				{
					initializable.Initialize(cfg);
				}
			}
		}

		public EventListeners ShallowCopy()
		{
			// todo-events Not ported
			return this;
		}

		public void DestroyListeners()
		{
			try
			{
				foreach (object i in initializedListeners)
				{
					var destructible = i as IDestructible;
					if (destructible != null)
					{
						destructible.Cleanup();
					}
					else
					{
						var disposable = i as IDisposable;
						if (disposable != null)
						{
							disposable.Dispose();
						}						
					}
				}
			}
			catch (Exception e)
			{
				throw new HibernateException("could not destruct/dispose listeners", e);
			}
		}
	}
}