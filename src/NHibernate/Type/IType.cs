using System;
using System.Data;

using NHibernate.Sql;
using NHibernate.Engine;

namespace NHibernate.Type {
	/// <summary>
	/// Defines a mapping from a .NET type to an Generic SQL datatype.
	/// This interface is intended to be implemented by applications that need custom types.
	/// </summary>
	/// <remarks>Implementors should usually be immutable and MUST definately be threadsafe</remarks>
	public interface IType {


		// QUESTIONS
		//
		// How do we implement Serializable interface? Standard .NET pattern or other?
		// What do we do with GetReturnedClass? It returns a java.lang.Class
		// What do we do with method that required ResultSet param?
		
		/// <summary>
		/// Return true if the implementation is castable to AssociationType.
		/// This does not necessarily imply that the type actually represents an association.
		/// </summary>
		/// <returns></returns>
		bool IsAssociationType { get; }
		
		/// <summary>
		/// Is this type a collection type?
		/// </summary>
		/// <returns></returns>
		bool IsPersistentCollectionType { get; }

		/// <summary>
		/// Is this type a component type?
		/// If so, the implementation must be castable to <tt>AbstractComponentType</tt>.
		/// A component type may own collections or associations and hence must provide certain extra functionality.
		/// </summary>
		/// <returns></returns>
		bool IsComponentType	{ get; }
	
		/// <summary>
		/// Is this type an entity type?
		/// </summary>
		/// <returns></returns>
		bool IsEntityType { get; }
	
		/// <summary>
		/// Is this an "object" type,
		/// ie. a reference to a persistent entity that is not modelled as a (foreign key) association.
		/// </summary>
		/// <returns></returns>
		/// <exception cref="MappingException">MappingException</exception>
		bool IsObjectType { get; }
	
		/// <summary>
		/// Return the SQL typecodes for the columns mapped by this type.
		/// The codes are defined on NHibernate.Sql.Types.
		/// </summary>
		/// <param name="mapping">Mapping</param>
		/// <returns>Typecodes</returns>
		Types[] SqlTypes(IMapping mapping);
	
		/// <summary>
		/// How many columns are used to persist this type?
		/// </summary>
		/// <param name="mapping"></param>
		/// <returns></returns>
		/// <exception cref="MappingException">MappingException</exception>
		int GetColumnSpan(IMapping mapping);
	
		/// <summary>
		/// The class returned by <c>NullSafeGet()</c> methods. This is used to establish the
		/// class of an array of this type
		/// </summary>
		System.Type ReturnedClass { get; }
		
	
		/// <summary>
		/// Compare two instances of the class mapped by this type for persistence "equality", ie. Equality of persistent state.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		bool Equals(object x, object y);
		
		/// <summary>
		/// Should the parent be considered dirty, given both the old and current field or element value?
		/// </summary>
		/// <param name="old">the old value</param>
		/// <param name="current">the current value</param>
		/// <param name="session">the session</param>
		/// <returns>true if the field is dirty</returns>
		bool IsDirty(object old, object current, ISessionImplementor session);
	

		/// <summary>
		/// Retrieve an instance of the mapped class from an ADO.NET resultset.
		/// Implementors should handle possibility of null values.
		/// </summary>
		/// <param name="rs"></param>
		/// <param name="names"></param>
		/// <param name="session"></param>
		/// <param name="owner"></param>
		/// <returns></returns>
		object NullSafeGet(IDataReader rs, string[] names, ISessionImplementor session, object owner);

		/// <summary>
		/// Retrieve an instance of the mapped class from an ADO.NET data reader. Implementations
		/// should handle possibility of null values.
		/// </summary>
		/// <remarks>
		/// This method might be called if the type is known to be a single-column type.
		/// </remarks>
		/// <param name="rs"></param>
		/// <param name="name"></param>
		/// <param name="session"></param>
		/// <param name="owner"></param>
		/// <returns></returns>
		object NullSafeGet(IDataReader rs, string name, ISessionImplementor session, Object owner);
	
		/// <summary>
		/// Write an instance of the mapped class to a prepared statement. Implementors should
		/// handle possibility of null values.
		/// </summary>
		/// <remarks>
		/// A multi-column type should be written to parameters starting from <c>Index</c>.
		/// </remarks>
		/// <param name="st"></param>
		/// <param name="value"></param>
		/// <param name="index"></param>
		/// <param name="session"></param>
		void NullSafeSet(IDbCommand st, object value, int index, ISessionImplementor session);
	

		/// <summary>
		/// A representation of the value to be embedded in an XML element
		/// </summary>
		/// <param name="value"></param>
		/// <param name="factory"></param>
		/// <returns></returns>
		string ToXML(object value, ISessionFactoryImplementor factory);

		
		/// <summary>
		/// Returns the abbreviated name of the type.
		/// </summary>
		/// <returns>the Hibernate type name</returns>
		string Name { get; }
	
		/// <summary>
		/// Return a deep copy of the persistent state, stopping at entities and at collections.
		/// </summary>
		/// <param name="value">generally a collection element or entity field</param>
		/// <returns></returns>
		object DeepCopy(object val);
	
		/// <summary>
		/// Are objects of this type mutable?
		/// (With respect to the referencing object...
		/// entities and collections are considered immutable because they manage their own internal state.)
		/// </summary>
		/// <returns></returns>
		bool IsMutable { get; }

		/// <summary>
		/// Return a cacheable "disassembled" representation of the object.
		/// </summary>
		/// <param name="value">the value to cache</param>
		/// <param name="session">the sesion</param>
		/// <returns>the disassembled, deep cloned state</returns>
		object Disassemble(object value, ISessionImplementor session);
	
		/// <summary>
		/// Reconstruct the object from its cached "disassembled" state.
		/// </summary>
		/// <param name="cached">the disassembled state from the cache</param>
		/// <param name="session">the session</param>
		/// <param name="owner">the parent entity object</param>
		/// <returns>the object</returns>
		object Assemble(object cached, ISessionImplementor session, object owner);

		/// <summary>
		/// Does this type implement a well-behaved <c>Equals()</c> method?
		/// </summary>
		/// <remarks>
		/// Strickly, if this method returns <c>true</c> then <c>x.Equals(y)</c> implies
		/// <c>IType.Equals(x, y)</c> and alos <c>IType.Equals(x, y)</c> implies that
		/// probably <c>x.Equals(y)</c>
		/// </remarks>
		bool HasNiceEquals { get; }
	
		/// <summary>
		/// Retrieve an instance of the mapped class, or the identifier of an entity or collection
		/// from an ADO.NET result set
		/// </summary>
		/// <remarks>
		/// This is useful for 2-phase property initialization - the second phase is a call to
		/// <c>ResolveIdentifier()</c>
		/// </remarks>
		/// <param name="rs"></param>
		/// <param name="names">the column names</param>
		/// <param name="session">the session</param>
		/// <param name="owner">the parent entity</param>
		/// <returns>an identifier or actual object</returns>
		object Hydrate(IDataReader rs, string[] names, ISessionImplementor session, object owner);
	
		/// <summary>
		/// Map identifiers to entities or collections. This is the second phase of 2-phase property
		/// initialization
		/// </summary>
		/// <param name="value">an identifier or value returned by <c>Hydrate()</c></param>
		/// <param name="session">The session</param>
		/// <param name="owner">The parent entity</param>
		/// <returns></returns>
		object ResolveIdentifier(object value, ISessionImplementor session, object owner);

	}
}
