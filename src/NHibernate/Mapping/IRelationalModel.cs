using NHibernate.Engine;

namespace NHibernate.Mapping
{
	/// <summary>
	/// Operations to create/drop the mapping element in the database.
	/// </summary>
	public interface IRelationalModel
	{
		/// <summary>
		/// When implemented by a class, generates the SQL string to create 
		/// the mapping element in the database.
		/// </summary>
		/// <param name="dialect">The <see cref="Dialect.Dialect"/> to use for SQL rules.</param>
		/// <param name="p"></param>
		/// <returns>
		/// A string that contains the SQL to create an object.
		/// </returns>
		string SqlCreateString( Dialect.Dialect dialect, IMapping p );

		/// <summary>
		/// When implemented by a class, generates the SQL string to drop 
		/// the mapping element from the database.
		/// </summary>
		/// <param name="dialect">The <see cref="Dialect.Dialect"/> to use for SQL rules.</param>
		/// <returns>
		/// A string that contains the SQL to drop an object.
		/// </returns>
		string SqlDropString( Dialect.Dialect dialect );
	}
}