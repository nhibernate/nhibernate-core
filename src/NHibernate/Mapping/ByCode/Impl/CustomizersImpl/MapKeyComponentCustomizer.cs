using System;
using System.Linq.Expressions;
using System.Reflection;

namespace NHibernate.Mapping.ByCode.Impl.CustomizersImpl
{
	public class MapKeyComponentCustomizer<TKey> : IComponentMapKeyMapper<TKey>
	{
		private readonly ICustomizersHolder customizersHolder;
		private readonly PropertyPath propertyPath;

		public MapKeyComponentCustomizer(IModelExplicitDeclarationsHolder explicitDeclarationsHolder, PropertyPath propertyPath, ICustomizersHolder customizersHolder)
		{
			this.propertyPath = propertyPath;
			this.customizersHolder = customizersHolder;
		}

		#region IComponentMapKeyMapper<TKey> Members

		public void Property<TProperty>(Expression<Func<TKey, TProperty>> property, Action<IPropertyMapper> mapping)
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(property);
			customizersHolder.AddCustomizer(new PropertyPath(propertyPath, member), mapping);
		}

		public void Property<TProperty>(Expression<Func<TKey, TProperty>> property)
		{
			Property(property, x => { });
		}

		public void ManyToOne<TProperty>(Expression<Func<TKey, TProperty>> property, Action<IManyToOneMapper> mapping) where TProperty : class
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(property);
			customizersHolder.AddCustomizer(new PropertyPath(propertyPath, member), mapping);
		}

		#endregion
	}
}