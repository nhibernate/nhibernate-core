using NHibernate.Engine;

namespace NHibernate.Persister
{
	/// <summary>
	/// Describes a class that may be loaded via a unique key.
	/// </summary>
	public interface IUniqueKeyLoadable
	{
		/// <summary>
		/// Load an instance of the persistent class, by a unique key other than the primary key.
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="uniqueKey"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		object LoadByUniqueKey( string propertyName, object uniqueKey, ISessionImplementor session );

		/// <summary>
		/// Get the columns of the unique key.
		/// </summary>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		string[] GetUniqueKeyColumnNames( string propertyName );
	}
}
