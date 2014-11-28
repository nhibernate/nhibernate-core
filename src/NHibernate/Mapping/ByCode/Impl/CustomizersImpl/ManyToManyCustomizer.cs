using System;

namespace NHibernate.Mapping.ByCode.Impl.CustomizersImpl
{
	public class ManyToManyCustomizer : IManyToManyMapper
	{
		private readonly ICustomizersHolder customizersHolder;
		private readonly PropertyPath propertyPath;

		public ManyToManyCustomizer(IModelExplicitDeclarationsHolder explicitDeclarationsHolder, PropertyPath propertyPath, ICustomizersHolder customizersHolder)
		{
			if (explicitDeclarationsHolder == null)
			{
				throw new ArgumentNullException("explicitDeclarationsHolder");
			}
			explicitDeclarationsHolder.AddAsManyToManyItemRelation(propertyPath.LocalMember);
			this.propertyPath = propertyPath;
			this.customizersHolder = customizersHolder;
		}

		#region IManyToManyMapper Members

		public void Column(Action<IColumnMapper> columnMapper)
		{
			customizersHolder.AddCustomizer(propertyPath, (IManyToManyMapper x) => x.Column(columnMapper));
		}

		public void Columns(params Action<IColumnMapper>[] columnMapper)
		{
			customizersHolder.AddCustomizer(propertyPath, (IManyToManyMapper x) => x.Columns(columnMapper));
		}

		public void Column(string name)
		{
			customizersHolder.AddCustomizer(propertyPath, (IManyToManyMapper x) => x.Column(name));
		}

		public void Class(System.Type entityType)
		{
			customizersHolder.AddCustomizer(propertyPath, (IManyToManyMapper x) => x.Class(entityType));
		}

		public void EntityName(string entityName)
		{
			customizersHolder.AddCustomizer(propertyPath, (IManyToManyMapper x) => x.EntityName(entityName));
		}

		public void NotFound(NotFoundMode mode)
		{
			customizersHolder.AddCustomizer(propertyPath, (IManyToManyMapper x) => x.NotFound(mode));
		}

		public void Formula(string formula)
		{
			customizersHolder.AddCustomizer(propertyPath, (IManyToManyMapper x) => x.Formula(formula));
		}

		public void Lazy(LazyRelation lazyRelation)
		{
			customizersHolder.AddCustomizer(propertyPath, (IManyToManyMapper x) => x.Lazy(lazyRelation));
		}

		public void ForeignKey(string foreignKeyName)
		{
			customizersHolder.AddCustomizer(propertyPath, (IManyToManyMapper x) => x.ForeignKey(foreignKeyName));
		}

		public void Where(string sqlWhereClause)
		{
			customizersHolder.AddCustomizer(propertyPath, (IManyToManyMapper x) => x.Where(sqlWhereClause));
		}

		#endregion
	}
}