using System;

namespace NHibernate {
	/// <summary>
	/// The inteface to be implemented by user-defined types.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The inteface abstracts user code from future changes to the <c>HibernateType</c> inteface,
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
	/// Alternatively, custom types could implement <c>HibernateType</c> directly or extend one of the
	/// abstract classes in <c>NHibernate.Type</c>. This approach risks future incompatible changes
	/// to classes or intefaces in the package.
	/// </para>
	/// </remarks>
	public interface IUserType {
		
		/// <summary>
		/// The SQL type codes for the columns mapped by this type. The codes are defined on
		/// <c></c>
		/// </summary>
		int[] SqlTypes { get; }
		
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

		//TODO:Finish user type

	}
}
