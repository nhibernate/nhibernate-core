namespace NHibernate.Mapping.ByCode
{
	public interface IManyToOneMapper : IEntityPropertyMapper, IColumnsMapper
	{
		/// <summary>
		/// Force the many-to-one to a different type than the one of the property.
		/// </summary>
		/// <param name="entityType">Mapped entity type.</param>
		/// <remarks>
		/// Useful when the property is an interface and you need the mapping to a concrete class mapped as entity.
		/// </remarks>
		void Class(System.Type entityType);

		void Cascade(Cascade cascadeStyle);
		void NotNullable(bool notnull);
		void Unique(bool unique);
		void UniqueKey(string uniquekeyName);
		void Index(string indexName);
		void Fetch(FetchKind fetchMode);
		void Formula(string formula);
		void Lazy(LazyRelation lazyRelation);
		void Update(bool consideredInUpdateQuery);
		void Insert(bool consideredInInsertQuery);
		void ForeignKey(string foreignKeyName);
		void PropertyRef(string propertyReferencedName);
		void NotFound(NotFoundMode mode);
	}
}