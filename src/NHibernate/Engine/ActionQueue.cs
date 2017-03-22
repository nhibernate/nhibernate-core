using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NHibernate.Action;
using NHibernate.Cache;
using NHibernate.Impl;
using NHibernate.Type;

namespace NHibernate.Engine
{
	/// <summary> 
	/// Responsible for maintaining the queue of actions related to events.
	/// <para>
	/// The ActionQueue holds the DML operations queued as part of a session's
	/// transactional-write-behind semantics. DML operations are queued here
	/// until a flush forces them to be executed against the database. 
	/// </para>
	/// </summary>
	[Serializable]
	public class ActionQueue
	{
		private static readonly IInternalLogger log = LoggerProvider.LoggerFor(typeof(ActionQueue));
		private const int InitQueueListSize = 5;

		private ISessionImplementor session;

		// Object insertions, updates, and deletions have list semantics because
		// they must happen in the right order so as to respect referential
		// integrity
		private readonly List<IExecutable> insertions;
		private readonly List<EntityDeleteAction> deletions;
		private readonly List<EntityUpdateAction> updates;
		// Actually the semantics of the next three are really "Bag"
		// Note that, unlike objects, collection insertions, updates,
		// deletions are not really remembered between flushes. We
		// just re-use the same Lists for convenience.
		private readonly List<CollectionRecreateAction> collectionCreations;
		private readonly List<CollectionUpdateAction> collectionUpdates;
		private readonly List<CollectionRemoveAction> collectionRemovals;

		private readonly AfterTransactionCompletionProcessQueue afterTransactionProcesses;
		private readonly BeforeTransactionCompletionProcessQueue beforeTransactionProcesses;

		public ActionQueue(ISessionImplementor session)
		{
			this.session = session;
			insertions = new List<IExecutable>(InitQueueListSize);
			deletions = new List<EntityDeleteAction>(InitQueueListSize);
			updates = new List<EntityUpdateAction>(InitQueueListSize);

			collectionCreations = new List<CollectionRecreateAction>(InitQueueListSize);
			collectionUpdates = new List<CollectionUpdateAction>(InitQueueListSize);
			collectionRemovals = new List<CollectionRemoveAction>(InitQueueListSize);

			afterTransactionProcesses = new AfterTransactionCompletionProcessQueue(session);
			beforeTransactionProcesses = new BeforeTransactionCompletionProcessQueue(session);
		}

		public virtual void Clear()
		{
			updates.Clear();
			insertions.Clear();
			deletions.Clear();

			collectionCreations.Clear();
			collectionRemovals.Clear();
			collectionUpdates.Clear();
		}

		public void AddAction(EntityInsertAction action)
		{
			insertions.Add(action);
		}

		public void AddAction(EntityDeleteAction action)
		{
			deletions.Add(action);
		}

		public void AddAction(EntityUpdateAction action)
		{
			updates.Add(action);
		}

		public void AddAction(CollectionRecreateAction action)
		{
			collectionCreations.Add(action);
		}

		public void AddAction(CollectionRemoveAction action)
		{
			collectionRemovals.Add(action);
		}

		public void AddAction(CollectionUpdateAction action)
		{
			collectionUpdates.Add(action);
		}

		public void AddAction(EntityIdentityInsertAction insert)
		{
			insertions.Add(insert);
		}

		public void AddAction(BulkOperationCleanupAction cleanupAction)
		{
			RegisterCleanupActions(cleanupAction);
		}

		public void RegisterProcess(AfterTransactionCompletionProcessDelegate process)
		{
			afterTransactionProcesses.Register(process);
		}

		public void RegisterProcess(BeforeTransactionCompletionProcessDelegate process)
		{
			beforeTransactionProcesses.Register(process);
		}

		private void ExecuteActions(IList list)
		{
			int size = list.Count;
			for (int i = 0; i < size; i++)
				Execute((IExecutable)list[i]);

			list.Clear();
			session.Batcher.ExecuteBatch();
		}

		public void Execute(IExecutable executable)
		{
			try
			{
				executable.Execute();
			}
			finally
			{
				RegisterCleanupActions(executable);
			}
		}

		private void RegisterCleanupActions(IExecutable executable)
		{
			beforeTransactionProcesses.Register(executable.BeforeTransactionCompletionProcess);
			if (session.Factory.Settings.IsQueryCacheEnabled)
			{
				string[] spaces = executable.PropertySpaces;
				afterTransactionProcesses.AddSpacesToInvalidate(spaces);
				session.Factory.UpdateTimestampsCache.PreInvalidate(spaces);
			}
			afterTransactionProcesses.Register(executable.AfterTransactionCompletionProcess);
		}

		/// <summary> 
		/// Perform all currently queued entity-insertion actions.
		/// </summary>
		public void ExecuteInserts()
		{
			ExecuteActions(insertions);
		}

		/// <summary> 
		/// Perform all currently queued actions. 
		/// </summary>
		public void ExecuteActions()
		{
			ExecuteActions(insertions);
			ExecuteActions(updates);
			ExecuteActions(collectionRemovals);
			ExecuteActions(collectionUpdates);
			ExecuteActions(collectionCreations);
			ExecuteActions(deletions);
		}

		private void PrepareActions(IList queue)
		{
			foreach (IExecutable executable in queue)
				executable.BeforeExecutions();
		}

		/// <summary>
		/// Prepares the internal action queues for execution.  
		/// </summary>
		public void PrepareActions()
		{
			PrepareActions(collectionRemovals);
			PrepareActions(collectionUpdates);
			PrepareActions(collectionCreations);
		}

		/// <summary>
		/// Execute any registered <see cref="BeforeTransactionCompletionProcessDelegate" />
		/// </summary>
		public void BeforeTransactionCompletion()
		{
			beforeTransactionProcesses.BeforeTransactionCompletion();
		}

		/// <summary> 
		/// Performs cleanup of any held cache softlocks.
		/// </summary>
		/// <param name="success">Was the transaction successful.</param>
		public void AfterTransactionCompletion(bool success)
		{
			afterTransactionProcesses.AfterTransactionCompletion(success);
		}

		/// <summary> 
		/// Check whether the given tables/query-spaces are to be executed against
		/// given the currently queued actions. 
		/// </summary>
		/// <param name="tables">The table/query-spaces to check. </param>
		/// <returns> True if we contain pending actions against any of the given tables; false otherwise.</returns>
		public virtual bool AreTablesToBeUpdated(ISet<string> tables)
		{
			return
				AreTablesToUpdated(updates, tables)
				|| AreTablesToUpdated(insertions, tables)
				|| AreTablesToUpdated(deletions, tables)
				|| AreTablesToUpdated(collectionUpdates, tables)
				|| AreTablesToUpdated(collectionCreations, tables)
				|| AreTablesToUpdated(collectionRemovals, tables);
		}

		/// <summary> 
		/// Check whether any insertion or deletion actions are currently queued. 
		/// </summary>
		/// <returns> True if insertions or deletions are currently queued; false otherwise.</returns>
		public bool AreInsertionsOrDeletionsQueued
		{
			get { return (insertions.Count > 0 || deletions.Count > 0); }
		}

		private static bool AreTablesToUpdated(IList executables, ICollection<string> tablespaces)
		{
			foreach (IExecutable exec in executables)
			{
				var spaces = exec.PropertySpaces;
				foreach (string o in spaces)
				{
					if (tablespaces.Contains(o))
					{
						if (log.IsDebugEnabled)
							log.Debug("changes must be flushed to space: " + o);

						return true;
					}
				}
			}
			return false;
		}

		public int CollectionRemovalsCount
		{
			get { return collectionRemovals.Count; }
		}

		public int CollectionUpdatesCount
		{
			get { return collectionUpdates.Count; }
		}

		public int CollectionCreationsCount
		{
			get { return collectionCreations.Count; }
		}

		public int DeletionsCount
		{
			get { return deletions.Count; }
		}

		public int UpdatesCount
		{
			get { return updates.Count; }
		}

		public int InsertionsCount
		{
			get { return insertions.Count; }
		}

		public void SortCollectionActions()
		{
			if (session.Factory.Settings.IsOrderUpdatesEnabled)
			{
				//sort the updates by fk
				collectionCreations.Sort();
				collectionUpdates.Sort();
				collectionRemovals.Sort();
			}
		}

		public void SortActions()
		{
			if (session.Factory.Settings.IsOrderUpdatesEnabled)
			{
				//sort the updates by pk
				updates.Sort();
			}
			if (session.Factory.Settings.IsOrderInsertsEnabled)
			{
				SortInsertActions();
			}
		}

		//Order the {@link #insertions} queue such that we group inserts
		//against the same entity together (without violating constraints).  The
		//original order is generated by cascade order, which in turn is based on
		//the directionality of foreign-keys. So even though we will be changing
		//the ordering here, we need to make absolutely certain that we do not
		//circumvent this FK ordering to the extent of causing constraint
		//violations
		private void SortInsertActions()
		{
			new InsertActionSorter(this).Sort();
		}

		public IList<EntityDeleteAction> CloneDeletions()
		{
			return new List<EntityDeleteAction>(deletions);
		}

		public void ClearFromFlushNeededCheck(int previousCollectionRemovalSize)
		{
			collectionCreations.Clear();
			collectionUpdates.Clear();
			updates.Clear();
			// collection deletions are a special case since update() can add
			// deletions of collections not loaded by the session.
			for (int i = collectionRemovals.Count - 1; i >= previousCollectionRemovalSize; i--)
			{
				collectionRemovals.RemoveAt(i);
			}
		}

		public bool HasBeforeTransactionActions()
		{
			return beforeTransactionProcesses.HasActions;
		}

		public bool HasAfterTransactionActions()
		{
			return afterTransactionProcesses.HasActions;
		}

		public bool HasAnyQueuedActions
		{
			get
			{
				return
					updates.Count > 0
					|| insertions.Count > 0
					|| deletions.Count > 0
					|| collectionUpdates.Count > 0
					|| collectionRemovals.Count > 0
					|| collectionCreations.Count > 0;
			}
		}

		public override string ToString()
		{
			// todo-events : use the helper for the collections
			return new StringBuilder()
				.Append("ActionQueue[insertions=")
				.Append(insertions)
				.Append(" updates=")
				.Append(updates)
				.Append(" deletions=")
				.Append(deletions)
				.Append(" collectionCreations=")
				.Append(collectionCreations)
				.Append(" collectionRemovals=")
				.Append(collectionRemovals)
				.Append(" collectionUpdates=")
				.Append(collectionUpdates)
				.Append("]").ToString();
		}

		[Serializable]
		private class BeforeTransactionCompletionProcessQueue
		{
			private ISessionImplementor session;
			private IList<BeforeTransactionCompletionProcessDelegate> processes = new List<BeforeTransactionCompletionProcessDelegate>();

			public bool HasActions
			{
				get { return processes.Count > 0; }
			}

			public BeforeTransactionCompletionProcessQueue(ISessionImplementor session)
			{
				this.session = session;
			}

			public void Register(BeforeTransactionCompletionProcessDelegate process)
			{
				if (process == null)
				{
					return;
				}
				processes.Add(process);
			}

			public void BeforeTransactionCompletion()
			{
				int size = processes.Count;
				for (int i = 0; i < size; i++)
				{
					try
					{
						BeforeTransactionCompletionProcessDelegate process = processes[i];
						process();
					}
					catch (HibernateException e)
					{
						throw e;
					}
					catch (Exception e)
					{
						throw new AssertionFailure("Unable to perform BeforeTransactionCompletion callback", e);
					}
				}
				processes.Clear();
			}
		}

		[Serializable]
		private class AfterTransactionCompletionProcessQueue
		{
			private ISessionImplementor session;
			private HashSet<string> querySpacesToInvalidate = new HashSet<string>();
			private IList<AfterTransactionCompletionProcessDelegate> processes = new List<AfterTransactionCompletionProcessDelegate>(InitQueueListSize * 3);

			public bool HasActions
			{
				get { return processes.Count > 0; }
			}

			public AfterTransactionCompletionProcessQueue(ISessionImplementor session)
			{
				this.session = session;
			}

			public void AddSpacesToInvalidate(string[] spaces)
			{
				if (spaces == null)
				{
					return;
				}
				for (int i = 0, max = spaces.Length; i < max; i++)
				{
					this.AddSpaceToInvalidate(spaces[i]);
				}
			}

			public void AddSpaceToInvalidate(string space)
			{
				querySpacesToInvalidate.Add(space);
			}

			public void Register(AfterTransactionCompletionProcessDelegate process)
			{
				if (process == null)
				{
					return;
				}
				processes.Add(process);
			}

			public void AfterTransactionCompletion(bool success)
			{
				int size = processes.Count;

				for (int i = 0; i < size; i++)
				{
					try
					{
						AfterTransactionCompletionProcessDelegate process = processes[i];
						process(success);
					}
					catch (CacheException e)
					{
						log.Error("could not release a cache lock", e);
						// continue loop
					}
					catch (Exception e)
					{
						throw new AssertionFailure("Unable to perform AfterTransactionCompletion callback", e);
					}
				}
				processes.Clear();

				if (session.Factory.Settings.IsQueryCacheEnabled)
				{
					session.Factory.UpdateTimestampsCache.Invalidate(querySpacesToInvalidate.ToArray());
				}
				querySpacesToInvalidate.Clear();
			}
		}

		[Serializable]
		private class InsertActionSorter
		{
			private readonly ActionQueue _actionQueue;

			[Serializable]
			private class BatchIdentifier : IEquatable<BatchIdentifier>
			{
				public string EntityName { get; }
				public ISet<string> ParentEntityNames { get; } = new HashSet<string>();
				public ISet<string> ChildEntityNames { get; } = new HashSet<string>();

				public BatchIdentifier(string entityName)
				{
					EntityName = entityName;
				}

				public override bool Equals(object obj)
				{
					var other = obj as BatchIdentifier;

					return Equals(other);
				}

				public bool Equals(BatchIdentifier other)
				{
					if (ReferenceEquals(this, other))
					{
						return true;
					}
					if (other == null)
					{
						return false;
					}
					return EntityName == other.EntityName;
				}

				public override int GetHashCode()
				{
					return EntityName.GetHashCode();
				}
			}

			// _orderedBatches and _entityNameBatchIdentifier replace _latestBatch from Hibernate implementation.
			private readonly List<BatchIdentifier> _orderedBatches = new List<BatchIdentifier>();
			private readonly Dictionary<string, BatchIdentifier> _entityNameBatchIdentifier = new Dictionary<string, BatchIdentifier>();

			// the map of batch numbers to EntityInsertAction lists
			private readonly Dictionary<BatchIdentifier, List<EntityInsertAction>> _actionBatches = new Dictionary<BatchIdentifier, List<EntityInsertAction>>();

			public InsertActionSorter(ActionQueue actionQueue)
			{
				_actionQueue = actionQueue;
			}

			public void Sort()
			{
				// the list of entity names that indicate the batch number
				foreach (EntityInsertAction action in _actionQueue.insertions)
				{
					// the entity associated with the current action.
					var currentEntity = action.Instance;

					BatchIdentifier batchIdentifier;
					if (!_entityNameBatchIdentifier.TryGetValue(action.EntityName, out batchIdentifier))
					{
						batchIdentifier = new BatchIdentifier(action.EntityName);
						_entityNameBatchIdentifier[action.EntityName] = batchIdentifier;
						_orderedBatches.Add(batchIdentifier);
					}
					AddParentChildEntityNames(action, batchIdentifier);
					AddToBatch(batchIdentifier, action);
				}
				_actionQueue.insertions.Clear();

				for (var i = 0; i < _orderedBatches.Count; i++)
				{
					var batchIdentifier = _orderedBatches[i];
					var entityName = batchIdentifier.EntityName;

					//Make sure that child entries are not before parents
					for (var j = i - 1; j >= 0; j--)
					{
						var prevBatchIdentifier = _orderedBatches[j];
						if (prevBatchIdentifier.ParentEntityNames.Contains(entityName))
						{
							_orderedBatches.RemoveAt(i);
							_orderedBatches.Insert(j, batchIdentifier);
						}
					}

					//Make sure that parent entries are not after children
					for (var j = i + 1; j < _orderedBatches.Count; j++)
					{
						var nextBatchIdentifier = _orderedBatches[j];
						//Take care of unidirectional @OneToOne associations but exclude bidirectional @ManyToMany
						if (nextBatchIdentifier.ChildEntityNames.Contains(entityName) &&
							!batchIdentifier.ChildEntityNames.Contains(nextBatchIdentifier.EntityName))
						{
							_orderedBatches.RemoveAt(i);
							_orderedBatches.Insert(j, batchIdentifier);
						}
					}
				}

				// now rebuild the insertions list. There is a batch for each entry in the batches list.
				foreach (var batchIdentifier in _orderedBatches)
				{
					var batch = _actionBatches[batchIdentifier];
					_actionQueue.insertions.AddRange(batch);
				}
			}

			/// <summary>
			/// Add parent and child entity names so that we know how to rearrange dependencies
			/// </summary>
			/// <param name="action">The action being sorted</param>
			/// <param name="batchIdentifier">The batch identifier of the entity affected by the action</param>
			private void AddParentChildEntityNames(EntityInsertAction action, BatchIdentifier batchIdentifier)
			{
				var propertyValues = action.State;
				var propertyTypes = action.Persister.ClassMetadata?.PropertyTypes;
				if (propertyTypes == null)
				{
					log.WarnFormat("Entity {0} persister does not provide meta-data, its batch ordering may be wrong and cause a failure.",
						batchIdentifier.EntityName);
					return;
				}

				for (var i = 0; i < propertyValues.Length; i++)
				{
					var value = propertyValues[i];
					var type = propertyTypes[i];
					if (type.IsEntityType && value != null)
					{
						var entityType = (EntityType)type;
						var entityName = entityType.Name;
						// Non constrained one-to-one means this is a child end.
						if (entityType.IsOneToOne && entityType.IsNullable)
						{
							batchIdentifier.ChildEntityNames.Add(entityName);
						}
						else
						{
							batchIdentifier.ParentEntityNames.Add(entityName);
						}
					}
					else if (type.IsCollectionType && value != null)
					{
						var collectionType = (CollectionType)type;
						var sessionFactory = action.Session.Factory;
						if (collectionType.GetElementType(sessionFactory).IsEntityType)
						{
							var entityName = collectionType.GetAssociatedEntityName(sessionFactory);
							batchIdentifier.ChildEntityNames.Add(entityName);
						}
					}
				}
			}

			private void AddToBatch(BatchIdentifier batchIdentifier, EntityInsertAction action)
			{
				List<EntityInsertAction> actions;

				if (!_actionBatches.TryGetValue(batchIdentifier, out actions))
				{
					actions = new List<EntityInsertAction>();
					_actionBatches[batchIdentifier] = actions;
				}

				actions.Add(action);
			}
		}
	}
}
