namespace NHibernate.Mapping.ByCode
{
	// 6.0 TODO: inherit from IColumnsAndFormulasMapper
	public interface IManyToManyMapper : IColumnsMapper
	{
		void Class(System.Type entityType);
		void EntityName(string entityName);
		void NotFound(NotFoundMode mode);
		// 6.0 TODO: remove once inhertied from IColumnsAndFormulasMapper
		void Formula(string formula);
		void Lazy(LazyRelation lazyRelation);
		void ForeignKey(string foreignKeyName);
		void Where(string sqlWhereClause);
	}
}
