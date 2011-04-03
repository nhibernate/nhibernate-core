using System.Reflection;

namespace NHibernate.Mapping.ByCode.Impl.CustomizersImpl
{
	public class NaturalIdCustomizer<TEntity> : PropertyContainerCustomizer<TEntity>, IBasePlainPropertyContainerMapper<TEntity> where TEntity : class
	{
		public NaturalIdCustomizer(IModelExplicitDeclarationsHolder explicitDeclarationsHolder, ICustomizersHolder customizersHolder)
			: base(explicitDeclarationsHolder, customizersHolder, null) {}

		public override void Property<TProperty>(System.Linq.Expressions.Expression<System.Func<TEntity, TProperty>> property, System.Action<IPropertyMapper> mapping)
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(property);
			MemberInfo memberOf = TypeExtensions.DecodeMemberAccessExpressionOf(property);
			ExplicitDeclarationsHolder.AddAsNaturalId(member);
			ExplicitDeclarationsHolder.AddAsNaturalId(memberOf);
			base.Property(property, mapping);
		}

		public override void Property(FieldInfo member, System.Action<IPropertyMapper> mapping)
		{
			ExplicitDeclarationsHolder.AddAsNaturalId(member);
			base.Property(member, mapping);
		}

		public override void Component<TComponent>(System.Linq.Expressions.Expression<System.Func<TEntity, TComponent>> property, System.Action<IComponentMapper<TComponent>> mapping)
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(property);
			MemberInfo memberOf = TypeExtensions.DecodeMemberAccessExpressionOf(property);
			ExplicitDeclarationsHolder.AddAsNaturalId(member);
			ExplicitDeclarationsHolder.AddAsNaturalId(memberOf);
			base.Component(property, mapping);
		}

		public override void Any<TProperty>(System.Linq.Expressions.Expression<System.Func<TEntity, TProperty>> property, System.Type idTypeOfMetaType, System.Action<IAnyMapper> mapping)
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(property);
			MemberInfo memberOf = TypeExtensions.DecodeMemberAccessExpressionOf(property);
			ExplicitDeclarationsHolder.AddAsNaturalId(member);
			ExplicitDeclarationsHolder.AddAsNaturalId(memberOf);
			base.Any(property, idTypeOfMetaType, mapping);
		}

		public override void ManyToOne<TProperty>(System.Linq.Expressions.Expression<System.Func<TEntity, TProperty>> property)
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(property);
			MemberInfo memberOf = TypeExtensions.DecodeMemberAccessExpressionOf(property);
			ExplicitDeclarationsHolder.AddAsNaturalId(member);
			ExplicitDeclarationsHolder.AddAsNaturalId(memberOf);
			base.ManyToOne(property);
		}

		public override void ManyToOne<TProperty>(System.Linq.Expressions.Expression<System.Func<TEntity, TProperty>> property, System.Action<IManyToOneMapper> mapping)
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(property);
			MemberInfo memberOf = TypeExtensions.DecodeMemberAccessExpressionOf(property);
			ExplicitDeclarationsHolder.AddAsNaturalId(member);
			ExplicitDeclarationsHolder.AddAsNaturalId(memberOf);
			base.ManyToOne(property, mapping);
		}
	}
}