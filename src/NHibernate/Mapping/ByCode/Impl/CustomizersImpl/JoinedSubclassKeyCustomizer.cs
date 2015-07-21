using System;
using System.Linq.Expressions;
using System.Reflection;

namespace NHibernate.Mapping.ByCode.Impl.CustomizersImpl
{
	public class JoinedSubclassKeyCustomizer<TEntity> : IKeyMapper<TEntity>
		where TEntity : class
	{
		public JoinedSubclassKeyCustomizer(ICustomizersHolder customizersHolder)
		{
			CustomizersHolder = customizersHolder;
		}

		public ICustomizersHolder CustomizersHolder { get; private set; }

		#region Implementation of IKeyMapper<TEntity>

		public void Column(Action<IColumnMapper> columnMapper)
		{
			CustomizersHolder.AddCustomizer(typeof (TEntity), (IJoinedSubclassAttributesMapper m) => m.Key(x => x.Column(columnMapper)));
		}

		public void Columns(params Action<IColumnMapper>[] columnMapper)
		{
			CustomizersHolder.AddCustomizer(typeof (TEntity), (IJoinedSubclassAttributesMapper m) => m.Key(x => x.Columns(columnMapper)));
		}

		public void Column(string columnName)
		{
			CustomizersHolder.AddCustomizer(typeof (TEntity), (IJoinedSubclassAttributesMapper m) => m.Key(x => x.Column(columnName)));
		}

		public void OnDelete(OnDeleteAction deleteAction)
		{
			CustomizersHolder.AddCustomizer(typeof (TEntity), (IJoinedSubclassAttributesMapper m) => m.Key(x => x.OnDelete(deleteAction)));
		}

		public void PropertyRef<TProperty>(Expression<Func<TEntity, TProperty>> propertyGetter)
		{
			MemberInfo member = TypeExtensions.DecodeMemberAccessExpression(propertyGetter);
			CustomizersHolder.AddCustomizer(typeof (TEntity), (IJoinedSubclassAttributesMapper m) => m.Key(x => x.PropertyRef(member)));
		}

		public void Update(bool consideredInUpdateQuery)
		{
			CustomizersHolder.AddCustomizer(typeof (TEntity), (IJoinedSubclassAttributesMapper m) => m.Key(x => x.Update(consideredInUpdateQuery)));
		}

		public void ForeignKey(string foreignKeyName)
		{
			CustomizersHolder.AddCustomizer(typeof (TEntity), (IJoinedSubclassAttributesMapper m) => m.Key(x => x.ForeignKey(foreignKeyName)));
		}

		public void NotNullable(bool notnull)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IJoinedSubclassAttributesMapper m) => m.Key(x => x.NotNullable(notnull)));
		}

		public void Unique(bool unique)
		{
			CustomizersHolder.AddCustomizer(typeof(TEntity), (IJoinedSubclassAttributesMapper m) => m.Key(x => x.Unique(unique)));
		}

		#endregion
	}
}