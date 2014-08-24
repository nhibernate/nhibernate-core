using System;
using NHibernate.Type;

namespace NHibernate.Mapping.ByCode.Impl.CustomizersImpl
{
	public class MapKeyCustomizer : IMapKeyMapper
	{
		private readonly ICustomizersHolder customizersHolder;
		private readonly PropertyPath propertyPath;

		public MapKeyCustomizer(PropertyPath propertyPath, ICustomizersHolder customizersHolder)
		{
			this.propertyPath = propertyPath;
			this.customizersHolder = customizersHolder;
		}

		#region IMapKeyMapper Members

		public void Column(Action<IColumnMapper> columnMapper)
		{
			customizersHolder.AddCustomizer(propertyPath, (IMapKeyMapper x) => x.Column(columnMapper));
		}

		public void Columns(params Action<IColumnMapper>[] columnMapper)
		{
			customizersHolder.AddCustomizer(propertyPath, (IMapKeyMapper x) => x.Columns(columnMapper));
		}

		public void Column(string name)
		{
			customizersHolder.AddCustomizer(propertyPath, (IMapKeyMapper x) => x.Column(name));
		}

		public void Type(IType persistentType)
		{
			customizersHolder.AddCustomizer(propertyPath, (IMapKeyMapper x) => x.Type(persistentType));
		}

		public void Type<TPersistentType>()
		{
			customizersHolder.AddCustomizer(propertyPath, (IMapKeyMapper x) => x.Type<TPersistentType>());
		}

		public void Type(System.Type persistentType)
		{
			customizersHolder.AddCustomizer(propertyPath, (IMapKeyMapper x) => x.Type(persistentType));
		}

		public void Length(int length)
		{
			customizersHolder.AddCustomizer(propertyPath, (IMapKeyMapper x) => x.Length(length));
		}

		public void Formula(string formula)
		{
			customizersHolder.AddCustomizer(propertyPath, (IMapKeyMapper x) => x.Formula(formula));
		}

		#endregion
	}
}