namespace NHibernate.Mapping.ByCode
{
	public interface IDynamicComponentAttributesMapper : IEntityPropertyMapper
	{
		void Update(bool consideredInUpdateQuery);
		void Insert(bool consideredInInsertQuery);
		void Unique(bool unique);
	}

	public interface IDynamicComponentMapper : IDynamicComponentAttributesMapper, IPropertyContainerMapper { }

	public interface IDynamicComponentAttributesMapper<TComponent> : IEntityPropertyMapper
	{
		void Update(bool consideredInUpdateQuery);
		void Insert(bool consideredInInsertQuery);
		void Unique(bool unique);
	}

	public interface IDynamicComponentMapper<TComponent> : IDynamicComponentAttributesMapper<TComponent>, IPropertyContainerMapper<TComponent>
	{ }

}