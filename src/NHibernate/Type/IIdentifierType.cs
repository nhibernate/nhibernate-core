using System;

namespace NHibernate.Type {

	/// <summary>
	/// An IType that may be used as an identifier.
	/// </summary>
	public interface IIdentifierType : IType {

		/// <summary>
		/// Convert the value from the mapping file to a Java object.
		/// </summary>
		/// <param name="xml">the value of <code>discriminator-value</code> or <code>unsaved-value</code> attribute</param>
		/// <returns></returns>
		public object StringToObject(string xml);
	}
}
