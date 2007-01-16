using System.Data;
using NHibernate.SqlTypes;

namespace NHibernate
{
	/// <summary>
	/// The inteface to be implemented by user-defined types.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The inteface abstracts user code from future changes to the <see cref="Type.IType"/> inteface,
	/// simplifies the implementation of custom types and hides certain "internal interfaces from
	/// user code.
	/// </para>
	/// <para>
	/// Implemenators must be immutable and must declare a public default constructor.
	/// </para>
	/// <para>
	/// The actual class mapped by a <c>IUserType</c> may be just about anything. However, if it is to
	/// be cacheble by a persistent cache, it must be serializable.
	/// </para>
	/// <para>
	/// Alternatively, custom types could implement <see cref="Type.IType"/> directly or extend one of the
	/// abstract classes in <c>NHibernate.Type</c>. This approach risks future incompatible changes
	/// to classes or intefaces in the package.
	/// </para>
	/// </remarks>
	public interface IUserType
	{
		/// <summary>
		/// The SQL types for the columns mapped by this type. 
		/// </summary>
		SqlType[] SqlTypes { get; }

		/// <summary>
		/// The type returned by <c>NullSafeGet()</c>
		/// </summary>
		System.Type ReturnedType { get; }

		/// <summary>
		/// Compare two instances of the class mapped by this type for persistent "equality"
		/// ie. equality of persistent state
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		bool Equals( object x, object y );

		/// <summary>
		/// Get a hashcode for the instance, consistent with persistence "equality"
		/// </summary>
		int GetHashCode(object x);

		/// <summary>
		/// Retrieve an instance of the mapped class from a JDBC resultset.
		/// Implementors should handle possibility of null values.
		/// </summary>
		/// <param name="rs">a IDataReader</param>
		/// <param name="names">column names</param>
		/// <param name="owner">the containing entity</param>
		/// <returns></returns>
		/// <exception cref="HibernateException">HibernateException</exception>
//		/// <exception cref="SQLException">SQLException</exception>
		object NullSafeGet( IDataReader rs, string[] names, object owner );

		/// <summary>
		/// Write an instance of the mapped class to a prepared statement.
		/// Implementors should handle possibility of null values.
		/// A multi-column type should be written to parameters starting from index.
		/// </summary>
		/// <param name="cmd">a IDbCommand</param>
		/// <param name="value">the object to write</param>
		/// <param name="index">command parameter index</param>
		/// <exception cref="HibernateException">HibernateException</exception>
//		/// <exception cref="SQLException">SQLException</exception>
		void NullSafeSet( IDbCommand cmd, object value, int index );

		/// <summary>
		/// Return a deep copy of the persistent state, stopping at entities and at collections.
		/// </summary>
		/// <param name="value">generally a collection element or entity field</param>
		/// <returns>a copy</returns>
		object DeepCopy( object value );

		/// <summary>
		/// Are objects of this type mutable?
		/// </summary>
		bool IsMutable { get; }
	}
}
