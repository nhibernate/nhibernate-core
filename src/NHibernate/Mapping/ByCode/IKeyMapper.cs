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

		/// <summary>
		/// Set the Foreing-Key name
		/// </summary>
		/// <param name="foreingKeyName">The name of the Foreing-Key</param>
		/// <remarks>
		/// Where the <paramref name="foreingKeyName"/> is "none" or <see cref="string.Empty"/> or all white-spaces the FK won't be created.
		/// Use null to reset the default NHibernate's behavior.
		/// </remarks>
		void ForeignKey(string foreingKeyName);
	}

	public interface IKeyMapper<TEntity> : IColumnsMapper where TEntity : class
	{
		void OnDelete(OnDeleteAction deleteAction);
		void PropertyRef<TProperty>(Expression<Func<TEntity, TProperty>> propertyGetter);
		void Update(bool consideredInUpdateQuery);
		void ForeignKey(string foreingKeyName);
	}
}