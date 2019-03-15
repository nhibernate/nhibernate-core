using System.Data.Common;
using NHibernate.Engine;
using NHibernate.SqlTypes;

namespace NHibernate.UserTypes
{
	/// <summary>
	/// The interface to be implemented by user-defined types.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The interface abstracts user code from future changes to the <see cref="Type.IType"/> interface,
	/// simplifies the implementation of custom types and hides certain "internal interfaces" from
	/// user code.
	/// </para>
	/// <para>
	/// Implementers must declare a public default constructor.
	/// </para>
	/// <para>
	/// The actual class mapped by a <c>IUserType</c> may be just about anything.
	/// </para>
	/// <para>
	/// For ensuring cacheability, <see cref="Assemble" /> and
	/// <see cref="Disassemble" /> must provide conversion to/from a cacheable
	/// representation.
	/// </para>
	/// <para>
	/// Alternatively, custom types could implement <see cref="Type.IType"/> directly or extend one of the
	/// abstract classes in <c>NHibernate.Type</c>. This approach risks more future incompatible changes
	/// to classes or interfaces in the package.
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
		bool Equals(object x, object y);

		/// <summary>
		/// Get a hashcode for the instance, consistent with persistence "equality"
		/// </summary>
		int GetHashCode(object x);

		/// <summary>
		/// Retrieve an instance of the mapped class from an ADO resultset.
		/// Implementors should handle possibility of null values.
		/// </summary>
		/// <param name="rs">a DbDataReader</param>
		/// <param name="names">column names</param>
		/// <param name="session">The session for which the operation is done. Allows access to
		/// <c>Factory.Dialect</c> and <c>Factory.ConnectionProvider.Driver</c> for adjusting to
		/// database or data provider capabilities.</param>
		/// <param name="owner">the containing entity</param>
		/// <returns>The value.</returns>
		/// <exception cref="HibernateException">HibernateException</exception>
		object NullSafeGet(DbDataReader rs, string[] names, ISessionImplementor session, object owner);

		/// <summary>
		/// Write an instance of the mapped class to a prepared statement.
		/// Implementors should handle possibility of null values.
		/// A multi-column type should be written to parameters starting from index.
		/// </summary>
		/// <param name="cmd">a DbCommand</param>
		/// <param name="value">the object to write</param>
		/// <param name="index">command parameter index</param>
		/// <param name="session">The session for which the operation is done. Allows access to
		/// <c>Factory.Dialect</c> and <c>Factory.ConnectionProvider.Driver</c> for adjusting to
		/// database or data provider capabilities.</param>
		/// <exception cref="HibernateException">HibernateException</exception>
		void NullSafeSet(DbCommand cmd, object value, int index, ISessionImplementor session);

		/// <summary>
		/// Return a deep copy of the persistent state, stopping at entities and at collections.
		/// </summary>
		/// <param name="value">Generally a collection element or entity field value mapped as this user type.</param>
		/// <returns>A copy.</returns>
		object DeepCopy(object value);

		/// <summary>
		/// Are objects of this type mutable?
		/// </summary>
		bool IsMutable { get; }

		/// <summary>
		/// During merge, replace the existing (<paramref name="target" />) value in the entity
		/// we are merging to with a new (<paramref name="original" />) value from the detached
		/// entity we are merging. For immutable objects, or null values, it is safe to simply
		/// return the first parameter. For mutable objects, it is safe to return a copy of the
		/// first parameter. For objects with component values, it might make sense to
		/// recursively replace component values.
		/// </summary>
		/// <param name="original">the value from the detached entity being merged</param>
		/// <param name="target">the value in the managed entity</param>
		/// <param name="owner">the managed entity</param>
		/// <returns>the value to be merged</returns>
		object Replace(object original, object target, object owner);

		/// <summary>
		/// Reconstruct an object from the cacheable representation. At the very least this
		/// method should perform a deep copy if the type is mutable. See
		/// <see cref="Disassemble"/>. (Optional operation if the second level cache is not used.)
		/// </summary>
		/// <param name="cached">The cacheable representation.</param>
		/// <param name="owner">The owner of the cached object.</param>
		/// <returns>A reconstructed object from the cachable representation.</returns>
		object Assemble(object cached, object owner);

		/// <summary>
		/// Transform the object into its cacheable representation. At the very least this
		/// method should perform a deep copy if the type is mutable. That may not be enough
		/// for some implementations, however; for example, associations must be cached as
		/// identifier values. (Optional operation if the second level cache is not used.)
		/// Second level cache implementations may have additional requirements, like the
		/// cacheable representation being binary serializable.
		/// </summary>
		/// <param name="value">The object to be cached.</param>
		/// <returns>A cacheable representation of the object.</returns>
		object Disassemble(object value);
	}
}
