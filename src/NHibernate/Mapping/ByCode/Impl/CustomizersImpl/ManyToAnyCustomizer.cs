using System;
using NHibernate.Type;

namespace NHibernate.Mapping.ByCode.Impl.CustomizersImpl
{
	public class ManyToAnyCustomizer : IManyToAnyMapper
	{
		private readonly ICustomizersHolder customizersHolder;
		private readonly PropertyPath propertyPath;

		public ManyToAnyCustomizer(IModelExplicitDeclarationsHolder explicitDeclarationsHolder, PropertyPath propertyPath, ICustomizersHolder customizersHolder)
		{
			if (explicitDeclarationsHolder == null)
			{
				throw new ArgumentNullException("explicitDeclarationsHolder");
			}
			explicitDeclarationsHolder.AddAsManyToAnyRelation(propertyPath.LocalMember);
			this.propertyPath = propertyPath;
			this.customizersHolder = customizersHolder;
		}

		public void MetaType(IType metaType)
		{
			customizersHolder.AddCustomizer(propertyPath, (IManyToAnyMapper x) => x.MetaType(metaType));
		}

		public void MetaType<TMetaType>()
		{
			customizersHolder.AddCustomizer(propertyPath, (IManyToAnyMapper x) => x.MetaType<TMetaType>());
		}

		public void MetaType(System.Type metaType)
		{
			customizersHolder.AddCustomizer(propertyPath, (IManyToAnyMapper x) => x.MetaType(metaType));
		}

		public void IdType(IType idType)
		{
			customizersHolder.AddCustomizer(propertyPath, (IManyToAnyMapper x) => x.IdType(idType));
		}

		public void IdType<TIdType>()
		{
			customizersHolder.AddCustomizer(propertyPath, (IManyToAnyMapper x) => x.IdType<TIdType>());
		}

		public void IdType(System.Type idType)
		{
			customizersHolder.AddCustomizer(propertyPath, (IManyToAnyMapper x) => x.IdType(idType));
		}

		public void Columns(Action<IColumnMapper> idColumnMapping, Action<IColumnMapper> classColumnMapping)
		{
			customizersHolder.AddCustomizer(propertyPath, (IManyToAnyMapper x) => x.Columns(idColumnMapping, classColumnMapping));
		}

		public void MetaValue(object value, System.Type entityType)
		{
			customizersHolder.AddCustomizer(propertyPath, (IManyToAnyMapper x) => x.MetaValue(value, entityType));
		}
	}
}