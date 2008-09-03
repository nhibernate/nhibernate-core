using System.Collections;
using Iesi.Collections;
using NHibernate.Engine;
using NHibernate.Persister.Entity;

namespace NHibernate.Event
{
	public interface IEventSource : ISessionImplementor, ISession
	{
		/// <summary> Get the ActionQueue for this session</summary>
		ActionQueue ActionQueue { get;}

		/// <summary> 
		/// Instantiate an entity instance, using either an interceptor,
		/// or the given persister
		/// </summary>
		object Instantiate(IEntityPersister persister, object id);

		/// <summary> Force an immediate flush</summary>
		void ForceFlush(EntityEntry e);

		/// <summary> Cascade merge an entity instance</summary>
		void Merge(string entityName, object obj, IDictionary copiedAlready);

		/// <summary> Cascade persist an entity instance</summary>
		void Persist(string entityName, object obj, IDictionary createdAlready);

		/// <summary> Cascade persist an entity instance during the flush process</summary>
		void PersistOnFlush(string entityName, object obj, IDictionary copiedAlready);

		/// <summary> Cascade refresh an entity instance</summary>
		void Refresh(object obj, IDictionary refreshedAlready);

		/// <summary> Cascade copy an entity instance</summary>
		void SaveOrUpdateCopy(string entityName, object obj, IDictionary copiedAlready);

		/// <summary> Cascade delete an entity instance</summary>
		void Delete(string entityName, object child, bool isCascadeDeleteEnabled, ISet transientEntities);
	}

}
