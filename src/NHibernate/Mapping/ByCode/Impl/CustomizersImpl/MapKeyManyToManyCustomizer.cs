using System;

namespace NHibernate.Mapping.ByCode.Impl.CustomizersImpl
{
	public class MapKeyManyToManyCustomizer : IMapKeyManyToManyMapper
	{
		private readonly ICustomizersHolder customizersHolder;
		private readonly PropertyPath propertyPath;

		public MapKeyManyToManyCustomizer(IModelExplicitDeclarationsHolder explicitDeclarationsHolder, PropertyPath propertyPath, ICustomizersHolder customizersHolder)
		{
			if (explicitDeclarationsHolder == null)
			{
				throw new ArgumentNullException("explicitDeclarationsHolder");
			}
			explicitDeclarationsHolder.AddAsManyToManyKeyRelation(propertyPath.LocalMember);
			this.propertyPath = propertyPath;
			this.customizersHolder = customizersHolder;
		}

		#region IMapKeyManyToManyMapper Members

		public void Column(Action<IColumnMapper> columnMapper)
		{
			customizersHolder.AddCustomizer(propertyPath, (IMapKeyManyToManyMapper x) => x.Column(columnMapper));
		}

		public void Columns(params Action<IColumnMapper>[] columnMapper)
		{
			customizersHolder.AddCustomizer(propertyPath, (IMapKeyManyToManyMapper x) => x.Columns(columnMapper));
		}

		public void Column(string name)
		{
			customizersHolder.AddCustomizer(propertyPath, (IMapKeyManyToManyMapper x) => x.Column(name));
		}

		public void ForeignKey(string foreignKeyName)
		{
			customizersHolder.AddCustomizer(propertyPath, (IMapKeyManyToManyMapper x) => x.ForeignKey(foreignKeyName));
		}

		public void Formula(string formula)
		{
			customizersHolder.AddCustomizer(propertyPath, (IMapKeyManyToManyMapper x) => x.Formula(formula));
		}

		#endregion
	}
}