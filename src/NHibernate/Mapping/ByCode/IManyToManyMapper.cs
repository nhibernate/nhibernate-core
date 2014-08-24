namespace NHibernate.Mapping.ByCode
{
	public interface IManyToManyMapper : IColumnsMapper
	{
		void Class(System.Type entityType);
		void EntityName(string entityName);
		void NotFound(NotFoundMode mode);
		void Formula(string formula);
		void Lazy(LazyRelation lazyRelation);
		void ForeignKey(string foreignKeyName);
		void Where(string sqlWhereClause);
	}
}