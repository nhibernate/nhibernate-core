using System;

namespace NHibernate {

	/// <summary>
	/// Represents an enumerated type
	/// </summary>
	/// <remarks>
	/// Implementors of <c>IPersistentEnum</c> are enumerated types persisted to the database
	/// as <c>SMALLINT</c>s. As well as implementing <c>ToInt()</c>, a <c>IPersistentEnum</c>
	/// must also provide a static method with the signature:
	/// <code>
	///		public static PersistentEnum FromInt(int i)
	/// </code>
	/// </remarks>
	public interface IPersistentEnum {
		int ToInt();
	}
}
