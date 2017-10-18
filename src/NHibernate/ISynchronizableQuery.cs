using System.Collections.Generic;

namespace NHibernate
{
	public interface ISynchronizableQuery<out T> where T : ISynchronizableQuery<T>
	{
		T AddSynchronizedQuerySpace(string querySpace);
		T AddSynchronizedEntityName(string entityName);
		T AddSynchronizedEntityClass(System.Type entityType);
		IReadOnlyCollection<string> GetSynchronizedQuerySpaces();
	}
}
