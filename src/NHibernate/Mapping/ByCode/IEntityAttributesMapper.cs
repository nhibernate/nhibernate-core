using NHibernate.Persister.Entity;

namespace NHibernate.Mapping.ByCode
{
	public interface IEntityAttributesMapper
	{
		void EntityName(string value);
		void Proxy(System.Type proxy);
		void Lazy(bool value);
		void DynamicUpdate(bool value);
		void DynamicInsert(bool value);
		void BatchSize(int value);
		void SelectBeforeUpdate(bool value);
		void Persister<T>() where T : IEntityPersister;
		void Synchronize(params string[] table);
	}
}