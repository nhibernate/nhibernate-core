using System;

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
		public bool IsAssociationType { get; }
		
		/// <summary>
		/// Is this type a collection type?
		/// </summary>
		/// <returns></returns>
		public bool IsPersistentCollectionType { get; }

		/// <summary>
		/// Is this type a component type?
		/// If so, the implementation must be castable to <tt>AbstractComponentType</tt>.
		/// A component type may own collections or associations and hence must provide certain extra functionality.
		/// </summary>
		/// <returns></returns>
		public bool IsComponentType	{ get; }
	
		/// <summary>
		/// Is this type an entity type?
		/// </summary>
		/// <returns></returns>
		public bool IsEntityType { get; }
	
		/// <summary>
		/// Is this an "object" type,
		/// ie. a reference to a persistent entity that is not modelled as a (foreign key) association.
		/// </summary>
		/// <returns></returns>
		/// <exception cref="MappingException">MappingException</exception>
		public boolean IsObjectType { get; }
	
		/// <summary>
		/// Return the SQL typecodes for the columns mapped by this type.
		/// The codes are defined on NHibernate.Sql.Types.
		/// </summary>
		/// <param name="mapping">Mapping</param>
		/// <returns>Typecodes</returns>
		public Types[] SqlTypes(IMapping mapping);
	
		/// <summary>
		/// How many columns are used to persist this type?
		/// </summary>
		/// <param name="mapping"></param>
		/// <returns></returns>
		/// <exception cref="MappingException">MappingException</exception>
		public int GetColumnSpan(IMapping mapping);
	
		/* TODO:
		
		 * The class returned by <tt>nullSafeGet()</tt> methods. This is used to establish 
		 * the class of an array of this type.
		 *
		 * @return Class
		 
		public Class GetReturnedClass();
		*/
	
		/// <summary>
		/// Compare two instances of the class mapped by this type for persistence "equality", ie. Equality of persistent state.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		/// <exception cref="HibernateException">HibernateException</exception>
		public bool Equals(object x, object y);
		
		/// <summary>
		/// Should the parent be considered dirty, given both the old and current field or element value?
		/// </summary>
		/// <param name="old">the old value</param>
		/// <param name="current">the current value</param>
		/// <param name="session"></param>
		/// <returns>true if the field is dirty</returns>
		public bool IsDirty(object old, object current, ISessionImplementor session);
	
		/* TODO:
		 
		 * @see Type#hydrate(ResultSet, String[], SessionImplementor, Object) alternative, 2-phase property initialization
		 * @param rs
		 * @param names the column names
		 * @param session
		 * @param owner the parent entity
		 * @return Object
		 * @throws HibernateException
		 * @throws SQLException
		 

		/// <summary>
		/// Retrieve an instance of the mapped class from a JDBC resultset.
		/// Implementors should handle possibility of null values.
		/// </summary>
		/// <param name="rs"></param>
		/// <param name="names"></param>
		/// <param name="session"></param>
		/// <param name="owner"></param>
		/// <returns></returns>
		public object NullSafeGet(ResultSet rs, string[] names, ISessionImplementor session, object owner);
	

		 * Retrieve an instance of the mapped class from a JDBC resultset. Implementations
		 * should handle possibility of null values. This method might be called if the
		 * type is known to be a single-column type.
		 *
		 * @param rs
		 * @param name the column name
		 * @param session
		 * @param owner the parent entity
		 * @return Object
		 * @throws HibernateException
		 * @throws SQLException

		public Object nullSafeGet(ResultSet rs, String name, SessionImplementor session, Object owner) throws HibernateException, SQLException;
	

		 * Write an instance of the mapped class to a prepared statement. Implementors
		 * should handle possibility of null values. A multi-column type should be written
		 * to parameters starting from <tt>index</tt>.
		 *
		 * @param st
		 * @param value the object to write
		 * @param index statement parameter index
		 * @param session
		 * @throws HibernateException
		 * @throws SQLException

		public void nullSafeSet(PreparedStatement st, Object value, int index, SessionImplementor session) throws HibernateException, SQLException;
	

		 * A representation of the value to be embedded in an XML element.
		 *
		 * @param value
		 * @param factory
		 * @return String
		 * @throws HibernateException
		 
		public String toXML(Object value, SessionFactoryImplementor factory) throws HibernateException;
		
		*/
		
		/// <summary>
		/// Returns the abbreviated name of the type.
		/// </summary>
		/// <returns>the Hibernate type name</returns>
		public string Name { get; }
	
		/// <summary>
		/// Return a deep copy of the persistent state, stopping at entities and at collections.
		/// </summary>
		/// <param name="value">generally a collection element or entity field</param>
		/// <returns></returns>
		public object DeepCopy(object value);
	
		/// <summary>
		/// Are objects of this type mutable?
		/// (With respect to the referencing object...
		/// entities and collections are considered immutable because they manage their own internal state.)
		/// </summary>
		/// <returns></returns>
		public bool IsMutable { get; }
	
		/**TODO:
		 * Return a cacheable "disassembled" representation of the object.
		 * @param value the value to cache
		 * @param session the session
		 * @return the disassembled, deep cloned state
		
		public Serializable disassemble(Object value, SessionImplementor session) throws HibernateException;
	
		 *
		 * Reconstruct the object from its cached "disassembled" state.
		 * @param cached the disassembled state from the cache
		 * @param session the session
		 * @param owner the parent entity object
		 * @return the the object
		
		public Object assemble(Serializable cached, SessionImplementor session, Object owner) throws HibernateException, SQLException;
	
		 *
		 * Does this type implement a well-behaved <tt>equals()</tt> method?
		 * (ie. one that is consistent with <tt>Type.equals()</tt>.) Strictly,
		 * if this method returns <tt>true</tt> then <tt>x.equals(y)</tt> implies
		 * <tt>Type.equals(x, y)</tt> and also <tt>Type.equals(x, y)</tt> implies
		 * that <b>probably</b> <tt>x.equals(y)</tt>
		 * @see java.lang.Object#equals(java.lang.Object)
		 * @see Type#equals(java.lang.Object, java.lang.Object)
		
		public boolean hasNiceEquals();
	
		 *
		 * Retrieve an instance of the mapped class, or the identifier of an entity or collection, from a JDBC resultset.
		 * This is useful for 2-phase property initialization - the second phase is a call to <tt>resolveIdentifier()</tt>.
		 * @see Type#resolveIdentifier(Object, SessionImplementor)
		 * @param rs
		 * @param names the column names
		 * @param session the session
		 * @param owner the parent entity
		 * @return Object an identifier or actual value
		 * @throws HibernateException
		 * @throws SQLException
	
		public Object hydrate(ResultSet rs, String[] names, SessionImplementor session, Object owner) throws HibernateException, SQLException;
	
		 *
		 * Map identifiers to entities or collections. This is the second phase of 2-phase property initialization.
		 * @see Type#hydrate(ResultSet, String[], SessionImplementor, Object)
		 * @param value an identifier or value returned by <tt>hydrate()</tt>
		 * @param owner the parent entity
		 * @param session the session
		 * @return the given value, or the value associated with the identifier
		 * @throws HibernateException
		 * @throws SQLException
		
		public Object resolveIdentifier(Object value, SessionImplementor session, Object owner) throws HibernateException, SQLException;
		 */
	}
}
