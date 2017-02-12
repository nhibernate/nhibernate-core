using System;
using System.Linq.Expressions;
using System.Reflection;

namespace NHibernate.Mapping.ByCode
{
	public enum OnDeleteAction
	{
		NoAction,
		Cascade,
	}

	public interface IKeyMapper : IColumnsMapper
	{
		void OnDelete(OnDeleteAction deleteAction);
		void PropertyRef(MemberInfo property);
		void Update(bool consideredInUpdateQuery);
		void NotNullable(bool notnull);
		void Unique(bool unique);

		/// <summary>
		/// Set the Foreign-Key name
		/// </summary>
		/// <param name="foreignKeyName">The name of the Foreign-Key</param>
		/// <remarks>
		/// Where the <paramref name="foreignKeyName"/> is "none" or <see cref="string.Empty"/> or all white-spaces the FK won't be created.
		/// Use null to reset the default NHibernate's behavior.
		/// </remarks>
		void ForeignKey(string foreignKeyName);
	}

	public interface IKeyMapper<TEntity> : IColumnsMapper
	{
		void OnDelete(OnDeleteAction deleteAction);
		void PropertyRef<TProperty>(Expression<Func<TEntity, TProperty>> propertyGetter);
		void Update(bool consideredInUpdateQuery);
		void ForeignKey(string foreignKeyName);
		void NotNullable(bool notnull);
		void Unique(bool unique);
	}
}