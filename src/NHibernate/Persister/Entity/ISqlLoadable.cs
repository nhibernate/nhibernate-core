using NHibernate.Type;

namespace NHibernate.Persister.Entity
{
	/// <summary>
	/// A class persister that supports queries expressed in the platform native SQL dialect.
	/// </summary>
	public interface ISqlLoadable : ILoadable
	{
		/// <summary>
		/// Get the type
		/// </summary>
		IType Type { get; }

		/// <summary>
		/// Returns the column alias names used to persist/query the numbered property of the class or a subclass (optional operation).
		/// </summary>
		string[] GetSubclassPropertyColumnAliases(string propertyName, string suffix);

		/// <summary> 
		/// Return the column names used to persist/query the named property of the class or a subclass (optional operation).
		/// </summary>
		string[] GetSubclassPropertyColumnNames(string propertyName);

		/// <summary>
		/// All columns to select, when loading.
		/// </summary>
		string SelectFragment(string alias, string suffix);
	}
}