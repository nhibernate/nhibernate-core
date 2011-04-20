namespace NHibernate.Mapping.ByCode
{
	public interface IDynamicComponentAttributesMapper : IEntityPropertyMapper
	{
		void Update(bool consideredInUpdateQuery);
		void Insert(bool consideredInInsertQuery);
	}

	public interface IDynamicComponentMapper : IDynamicComponentAttributesMapper, IPropertyContainerMapper { }

	public interface IDynamicComponentAttributesMapper<TComponent> : IEntityPropertyMapper
	{
		void Update(bool consideredInUpdateQuery);
		void Insert(bool consideredInInsertQuery);
	}

	public interface IDynamicComponentMapper<TComponent> : IDynamicComponentAttributesMapper<TComponent>, IPropertyContainerMapper<TComponent> where TComponent : class { }

}