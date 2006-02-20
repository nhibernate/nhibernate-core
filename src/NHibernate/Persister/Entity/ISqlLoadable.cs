using System;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate.Persister.Entity
{
	/// <summary>
	/// A class persister that supports queries expressed in the platform native SQL dialect.
	/// </summary>
	public interface ISqlLoadable : ILoadable
	{
		/// <summary>
		/// Returns the column alias names used to persist/query the numbered property of the class or a subclass (optional operation).
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="suffix"></param>
		/// <returns></returns>
		string[] GetSubclassPropertyColumnAliases( string propertyName, string suffix );

		/// <summary>
		/// All columns to select, when loading.
		/// </summary>
		/// <param name="alias"></param>
		/// <param name="suffix"></param>
		/// <returns></returns>
		SqlString SelectFragment( string alias, string suffix );

		/// <summary>
		/// Get the type
		/// </summary>
		IType Type { get; }
	}
}
