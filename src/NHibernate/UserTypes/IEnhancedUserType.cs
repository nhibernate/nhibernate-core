namespace NHibernate.UserTypes
{
	/// <summary>
	/// A custom type that may function as an identifier or discriminator
	/// type, or may be marshalled to and from an XML document.
	/// </summary>
	public interface IEnhancedUserType : IUserType
	{
		/// <summary>
		/// Parse a string representation of this value, as it appears
		/// in an XML document.
		/// </summary>
		object FromXMLString(string xml);

		/// <summary>
		/// Return an SQL literal representation of the value
		/// </summary>
		string ObjectToSQLString(object value);

		/// <summary> 
		/// Return a string representation of this value, as it
		/// should appear in an XML document
		/// </summary>
		string ToXMLString(object value);
	}
}