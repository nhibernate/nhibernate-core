using System.Reflection;

namespace NHibernate.Mapping.ByCode.Impl.CustomizersImpl
{
	public class ComposedIdCustomizer<TEntity> : PropertyContainerCustomizer<TEntity>, IComposedIdMapper<TEntity> where TEntity : class
	{
		public ComposedIdCustomizer(IModelExplicitDeclarationsHolder explicitDeclarationsHolder, ICustomizersHolder customizersHolder)
			: base(explicitDeclarationsHolder, customizersHolder, null) {}

		protected override void RegisterPropertyMapping<TProperty>(System.Linq.Expressions.Expression<System.Func<TEntity, TProperty>> property, System.Action<IPropertyMapper> mapping)
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpressionOf(property);
			ExplicitDeclarationsHolder.AddAsPartOfComposedId(member);
			base.RegisterPropertyMapping(property, mapping);
		}

		protected override void RegisterManyToOneMapping<TProperty>(System.Linq.Expressions.Expression<System.Func<TEntity, TProperty>> property, System.Action<IManyToOneMapper> mapping)
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpressionOf(property);
			ExplicitDeclarationsHolder.AddAsPartOfComposedId(member);
			base.RegisterManyToOneMapping(property, mapping);
		}
	}
}