using System;

namespace NHibernate.Mapping.ByCode.Impl.CustomizersImpl
{
	public class OneToManyCustomizer : IOneToManyMapper
	{
		private readonly ICustomizersHolder customizersHolder;
		private readonly PropertyPath propertyPath;

		public OneToManyCustomizer(IModelExplicitDeclarationsHolder explicitDeclarationsHolder, PropertyPath propertyPath, ICustomizersHolder customizersHolder)
		{
			if (explicitDeclarationsHolder == null)
			{
				throw new ArgumentNullException("explicitDeclarationsHolder");
			}
			explicitDeclarationsHolder.AddAsOneToManyRelation(propertyPath.LocalMember);
			this.propertyPath = propertyPath;
			this.customizersHolder = customizersHolder;
		}

		#region IOneToManyMapper Members

		public void Class(System.Type entityType)
		{
			customizersHolder.AddCustomizer(propertyPath, (IOneToManyMapper x) => x.Class(entityType));
		}

		public void EntityName(string entityName)
		{
			customizersHolder.AddCustomizer(propertyPath, (IOneToManyMapper x) => x.EntityName(entityName));
		}

		public void NotFound(NotFoundMode mode)
		{
			customizersHolder.AddCustomizer(propertyPath, (IOneToManyMapper x) => x.NotFound(mode));
		}

		#endregion
	}
}