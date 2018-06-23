using System;

namespace NHibernate.UserTypes
{
	/// <summary>
	/// A custom type that may function as an identifier or discriminator
	/// type, or may be marshalled to and from an XML document.
	/// </summary>
	public interface IEnhancedUserType : IUserType
	{
		/// <summary>
		/// Parse a string representation of this value.
		/// </summary>
		// 6.0 TODO: rename "FromString(string value)".
		// Since 5.2
		[Obsolete("This method was not used for parsing xml strings, but instead used for parsing already de-encoded " +
			"strings originating from xml (so indeed, plain text strings). It will be renamed \"FromString\" in a " +
			"future version. Implement a \"object FromString(string value)\" without any xml de-coding, called by your" +
			"\"object FromXMLString(string xml)\", in order to get ready for the rename.")]
		object FromXMLString(string xml);

		/// <summary>
		/// Return an SQL literal representation of the value
		/// </summary>
		string ObjectToSQLString(object value);

		/// <summary> 
		/// Return a string representation of this value. It does not need to be xml encoded.
		/// </summary>
		// 6.0 TODO: rename "ToString".
		// Since 5.2
		[Obsolete("This method was not used in a xml context, but instead just used for logs. It will be renamed " +
			"\"ToString\" in a future version. Implement a \"string ToString(object value)\", called by your " +
			"\"string ToXMLString(object value)\", in order to get ready for the rename.")]
		string ToXMLString(object value);
	}
}
