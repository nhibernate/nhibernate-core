using System.Reflection;

namespace NHibernate.Mapping.ByCode.Impl.CustomizersImpl
{
	public class NaturalIdCustomizer<TEntity> : PropertyContainerCustomizer<TEntity>, IBasePlainPropertyContainerMapper<TEntity> where TEntity : class
	{
		public NaturalIdCustomizer(IModelExplicitDeclarationsHolder explicitDeclarationsHolder, ICustomizersHolder customizersHolder)
			: base(explicitDeclarationsHolder, customizersHolder, null) {}

		protected override void RegisterPropertyMapping<TProperty>(System.Linq.Expressions.Expression<System.Func<TEntity, TProperty>> property, System.Action<IPropertyMapper> mapping)
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(property);
			MemberInfo memberOf = TypeExtensions.DecodeMemberAccessExpressionOf(property);
			ExplicitDeclarationsHolder.AddAsNaturalId(member);
			ExplicitDeclarationsHolder.AddAsNaturalId(memberOf);
			base.RegisterPropertyMapping(property, mapping);
		}

		protected override void RegisterNoVisiblePropertyMapping(string notVisiblePropertyOrFieldName, System.Action<IPropertyMapper> mapping)
		{
			MemberInfo member = typeof(TEntity).GetPropertyOrFieldMatchingName(notVisiblePropertyOrFieldName);
			MemberInfo memberOf = member.GetMemberFromReflectedType(typeof(TEntity));

			ExplicitDeclarationsHolder.AddAsNaturalId(member);
			ExplicitDeclarationsHolder.AddAsNaturalId(memberOf);
			base.RegisterNoVisiblePropertyMapping(notVisiblePropertyOrFieldName, mapping);
		}

		protected override void RegisterComponentMapping<TComponent>(System.Linq.Expressions.Expression<System.Func<TEntity, TComponent>> property, System.Action<IComponentMapper<TComponent>> mapping)
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(property);
			MemberInfo memberOf = TypeExtensions.DecodeMemberAccessExpressionOf(property);
			ExplicitDeclarationsHolder.AddAsNaturalId(member);
			ExplicitDeclarationsHolder.AddAsNaturalId(memberOf);
			base.RegisterComponentMapping(property, mapping);
		}

		protected override void RegisterAnyMapping<TProperty>(System.Linq.Expressions.Expression<System.Func<TEntity, TProperty>> property, System.Type idTypeOfMetaType, System.Action<IAnyMapper> mapping)
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(property);
			MemberInfo memberOf = TypeExtensions.DecodeMemberAccessExpressionOf(property);
			ExplicitDeclarationsHolder.AddAsNaturalId(member);
			ExplicitDeclarationsHolder.AddAsNaturalId(memberOf);
			base.RegisterAnyMapping(property, idTypeOfMetaType, mapping);
		}

		protected override void RegisterManyToOneMapping<TProperty>(System.Linq.Expressions.Expression<System.Func<TEntity, TProperty>> property, System.Action<IManyToOneMapper> mapping)
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(property);
			MemberInfo memberOf = TypeExtensions.DecodeMemberAccessExpressionOf(property);
			ExplicitDeclarationsHolder.AddAsNaturalId(member);
			ExplicitDeclarationsHolder.AddAsNaturalId(memberOf);
			base.RegisterManyToOneMapping(property, mapping);
		}
	}
}