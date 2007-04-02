using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate.Util;

namespace NHibernate.Search.Impl
{
	public class SearchInterceptor : EmptyInterceptor
	{
		private Dictionary<ITransaction, List<LuceneWork>> syncronizations = new Dictionary<ITransaction, List<LuceneWork>>();
		List<object> entitiesToAddOnPostFlush = new List<object>();
		private ISession session;
		private SearchFactory searchFactory;

		public ISession Session
		{
			get { return session; }
			set { session = value; }
		}

		public override void SetSession(ISession session)
		{
			if(this.session!=null)
			{
				throw new InvalidOperationException(
					"You cannot use the same instance of SearchInterceptor for more than a single session, use a new instnace whenver you open a session");
			}
			this.session = session;
			searchFactory = SearchFactory.GetSearchFactory(session);
		}

		public SearchFactory SearchFactory
		{
			get { return searchFactory; }
		}

		public override void PostFlush(System.Collections.ICollection entities)
		{
			ArrayList searchableEntities = new ArrayList(entities);
			foreach (object entity in entitiesToAddOnPostFlush)
			{
				AssertEntityExistsInCollection(entity, searchableEntities);
				object id = session.GetIdentifier(entity);
				RegisterIndexing(entity, id, WorkType.Add);
			}
			entitiesToAddOnPostFlush.Clear();
		}

		private static void AssertEntityExistsInCollection(object entity, ArrayList searchableEntities)
		{
			if (searchableEntities.Contains(entity) == false)
				throw new HibernateException(
					string.Format("An entity [{0}] with no ID was previosuly saved, but was not found in the entities collection post flush.", entity.GetType().FullName));
		}

		public override bool OnFlushDirty(object entity, object id, object[] currentState, object[] previousState, string[] propertyNames, NHibernate.Type.IType[] types)
		{
			RegisterIndexing(entity, id, WorkType.Update);
			return base.OnFlushDirty(entity, id, currentState, previousState, propertyNames, types);
		}

		public override void OnDelete(object entity, object id, object[] state, string[] propertyNames, NHibernate.Type.IType[] types)
		{
			RegisterIndexing(entity, id, WorkType.Delete);
			base.OnDelete(entity, id, state, propertyNames, types);
		}

		public override bool OnSave(object entity, object id, object[] state, string[] propertyNames, NHibernate.Type.IType[] types)
		{
			if(id==null)//if we have an identity ID, the ID is not known until after the flush
				RegisterForAdditionOnPostFlush(entity);
			else
				RegisterIndexing(entity, id, WorkType.Add);
			return base.OnSave(entity, id, state, propertyNames, types);
		}

		private void RegisterForAdditionOnPostFlush(object entity)
		{
			entitiesToAddOnPostFlush.Add(entity);	
		}

		private void RegisterIndexing(object entity, object id, WorkType workType)
		{
			SearchFactory searchFactory = SearchFactory.GetSearchFactory(session);
			searchFactory.PerformWork(entity, id, session, workType);
		}

		public void RegisterSyncronization(ITransaction transaction, List<LuceneWork> work)
		{
			if (syncronizations.ContainsKey(transaction) == false)
				syncronizations.Add(transaction, new List<LuceneWork>());
			syncronizations[transaction].AddRange(work);
		}

		public override void AfterTransactionCompletion(ITransaction tx)
		{
			base.AfterTransactionCompletion(tx);
			List<LuceneWork> queue;
			if (syncronizations.TryGetValue(tx, out queue) == false)
				return;
			if (tx.WasCommitted)
			{
				SearchFactory.GetSearchFactory(session)
					.ExecuteQueueImmediate(queue);
			}
			queue.Clear();
			syncronizations.Remove(tx);
		}
	}
}