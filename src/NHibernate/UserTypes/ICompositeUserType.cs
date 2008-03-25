using System;
using System.Data;
using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.UserTypes
{
	/// <summary>
	/// A UserType that may be dereferenced in a query.
	/// This interface allows a custom type to define "properties".
	/// These need not necessarily correspond to physical .NET style properties.
	/// 
	/// A ICompositeUserType may be used in almost every way 
	/// that a component may be used. It may even contain many-to-one
	/// associations.
	/// 
	/// Implementors must be immutable and must declare a public
	/// default constructor.
	/// 
	/// Unlike UserType, cacheability does not depend upon
	/// serializability. Instead, Assemble() and 
	/// Disassemble() provide conversion to/from a cacheable
	/// representation.
	/// </summary>
	public interface ICompositeUserType
	{
		/// <summary>
		/// Get the "property names" that may be used in a query. 
		/// </summary>
		string[] PropertyNames { get; }

		/// <summary>
		/// Get the corresponding "property types"
		/// </summary>
		IType[] PropertyTypes { get; }

		/// <summary>
		/// Get the value of a property
		/// </summary>
		/// <param name="component">an instance of class mapped by this "type"</param>
		/// <param name="property"></param>
		/// <returns>the property value</returns>
		object GetPropertyValue(object component, int property);

		/// <summary>
		/// Set the value of a property
		/// </summary>
		/// <param name="component">an instance of class mapped by this "type"</param>
		/// <param name="property"></param>
		/// <param name="value">the value to set</param>
		void SetPropertyValue(object component, int property, object value);

		/// <summary>
		/// The class returned by NullSafeGet().
		/// </summary>
		System.Type ReturnedClass { get; }

		/// <summary>
		/// Compare two instances of the class mapped by this type for persistence
		/// "equality", ie. equality of persistent state.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		bool Equals(Object x, Object y);

		/// <summary>
		/// Get a hashcode for the instance, consistent with persistence "equality"
		/// </summary>
		int GetHashCode(object x);

		/// <summary>
		/// Retrieve an instance of the mapped class from a IDataReader. Implementors
		/// should handle possibility of null values.
		/// </summary>
		/// <param name="dr">IDataReader</param>
		/// <param name="names">the column names</param>
		/// <param name="session"></param>
		/// <param name="owner">the containing entity</param>
		/// <returns></returns>
		object NullSafeGet(IDataReader dr, string[] names, ISessionImplementor session, object owner);

		/// <summary>
		/// Write an instance of the mapped class to a prepared statement.
		/// Implementors should handle possibility of null values.
		/// A multi-column type should be written to parameters starting from index.
		/// </summary>
		/// <param name="cmd"></param>
		/// <param name="value"></param>
		/// <param name="index"></param>
		/// <param name="session"></param>
		void NullSafeSet(IDbCommand cmd, object value, int index, ISessionImplementor session);

		/// <summary>
		/// Return a deep copy of the persistent state, stopping at entities and at collections.
		/// </summary>
		/// <param name="value">generally a collection element or entity field</param>
		/// <returns></returns>
		object DeepCopy(object value);

		/// <summary>
		/// Are objects of this type mutable?
		/// </summary>
		bool IsMutable { get; }

		/// <summary>
		/// Transform the object into its cacheable representation.
		/// At the very least this method should perform a deep copy.
		/// That may not be enough for some implementations, method should perform a deep copy. That may not be enough for some implementations, however; for example, associations must be cached as identifier values. (optional operation)
		/// </summary>
		/// <param name="value">the object to be cached</param>
		/// <param name="session"></param>
		/// <returns></returns>
		object Disassemble(object value, ISessionImplementor session);

		/// <summary>
		/// Reconstruct an object from the cacheable representation.
		/// At the very least this method should perform a deep copy. (optional operation)
		/// </summary>
		/// <param name="cached">the object to be cached</param>
		/// <param name="session"></param>
		/// <param name="owner"></param>
		/// <returns></returns>
		object Assemble(object cached, ISessionImplementor session, object owner);

		/// <summary>
		/// During merge, replace the existing (target) value in the entity we are merging to
		/// with a new (original) value from the detached entity we are merging. For immutable
		/// objects, or null values, it is safe to simply return the first parameter. For
		/// mutable objects, it is safe to return a copy of the first parameter. However, since
		/// composite user types often define component values, it might make sense to recursively 
		/// replace component values in the target object.
		/// </summary>
		object Replace(object original, object target, ISessionImplementor session, object owner);
	}
}