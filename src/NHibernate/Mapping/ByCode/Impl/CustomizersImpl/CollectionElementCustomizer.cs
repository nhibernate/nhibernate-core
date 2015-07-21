using System;
using NHibernate.Type;

namespace NHibernate.Mapping.ByCode.Impl.CustomizersImpl
{
	public class CollectionElementCustomizer : IElementMapper
	{
		private readonly PropertyPath propertyPath;

		public CollectionElementCustomizer(PropertyPath propertyPath, ICustomizersHolder customizersHolder)
		{
			this.propertyPath = propertyPath;
			CustomizersHolder = customizersHolder;
		}

		public ICustomizersHolder CustomizersHolder { get; private set; }

		#region IElementMapper Members

		public void Column(Action<IColumnMapper> columnMapper)
		{
			CustomizersHolder.AddCustomizer(propertyPath, (IElementMapper e) => e.Column(columnMapper));
		}

		public void Columns(params Action<IColumnMapper>[] columnMapper)
		{
			CustomizersHolder.AddCustomizer(propertyPath, (IElementMapper e) => e.Columns(columnMapper));
		}

		public void Column(string name)
		{
			CustomizersHolder.AddCustomizer(propertyPath, (IElementMapper e) => e.Column(name));
		}

		public void Type(IType persistentType)
		{
			CustomizersHolder.AddCustomizer(propertyPath, (IElementMapper e) => e.Type(persistentType));
		}

		public void Type<TPersistentType>()
		{
			CustomizersHolder.AddCustomizer(propertyPath, (IElementMapper e) => e.Type<TPersistentType>());
		}

		public void Type<TPersistentType>(object parameters)
		{
			CustomizersHolder.AddCustomizer(propertyPath, (IElementMapper e) => e.Type<TPersistentType>(parameters));
		}

		public void Type(System.Type persistentType, object parameters)
		{
			CustomizersHolder.AddCustomizer(propertyPath, (IElementMapper e) => e.Type(persistentType, parameters));
		}

		public void Length(int length)
		{
			CustomizersHolder.AddCustomizer(propertyPath, (IElementMapper e) => e.Length(length));
		}

		public void Precision(short precision)
		{
			CustomizersHolder.AddCustomizer(propertyPath, (IElementMapper e) => e.Precision(precision));
		}

		public void Scale(short scale)
		{
			CustomizersHolder.AddCustomizer(propertyPath, (IElementMapper e) => e.Scale(scale));
		}

		public void NotNullable(bool notnull)
		{
			CustomizersHolder.AddCustomizer(propertyPath, (IElementMapper e) => e.NotNullable(notnull));
		}

		public void Unique(bool unique)
		{
			CustomizersHolder.AddCustomizer(propertyPath, (IElementMapper e) => e.Unique(unique));
		}

		public void Formula(string formula)
		{
			CustomizersHolder.AddCustomizer(propertyPath, (IElementMapper e) => e.Formula(formula));
		}

		#endregion
	}
}