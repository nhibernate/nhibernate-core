namespace NHibernate.Mapping.ByCode
{
	public interface IComposedIdMapper : IMinimalPlainPropertyContainerMapper {}

	public interface IComponentAsIdAttributesMapper : IAccessorPropertyMapper
	{
		/// <summary>
		/// Force the component to a different type than the one of the property.
		/// </summary>
		/// <param name="componentType">Mapped component type.</param>
		/// <remarks>
		/// Useful when the property is an interface and you need the mapping to a concrete class mapped as component.
		/// </remarks>
		void Class(System.Type componentType);

		void UnsavedValue(UnsavedValueType unsavedValueType);
	}

	public interface IComponentAsIdMapper : IComponentAsIdAttributesMapper, IMinimalPlainPropertyContainerMapper { }

	public interface IComposedIdMapper<TEntity> : IMinimalPlainPropertyContainerMapper<TEntity> where TEntity : class { }

	public interface IComponentAsIdAttributesMapper<TComponent> : IAccessorPropertyMapper
	{
		void Class<TConcrete>() where TConcrete : TComponent;

		void UnsavedValue(UnsavedValueType unsavedValueType);
	}

	public interface IComponentAsIdMapper<TComponent> : IComponentAsIdAttributesMapper<TComponent>, IMinimalPlainPropertyContainerMapper<TComponent>
	{ }

}