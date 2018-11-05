namespace NHibernate.Type
{
	/// <summary>
	/// An <see cref="IType"/> that may be used as an identifier.
	/// </summary>
	public interface IIdentifierType : IType
	{
		// 6.0 TODO: rename "xml" parameter as "value": it is not a xml string. The fact it generally comes from a xml
		// attribute value is irrelevant to the method behavior.
		/// <summary>
		/// Parse the string representation of a value to convert it to the .NET object.
		/// </summary>
		/// <param name="xml">A string representation.</param>
		/// <returns>The string converted to the object.</returns>
		/// <remarks>
		/// <para>This method needs to be able to handle any string. It should not just
		/// call System.Type.Parse without verifying that it is a parsable value
		/// for the System.Type.</para> 
		/// <para>Notably meant for parsing <c>discriminator-value</c> or <c>unsaved-value</c> mapping attribute value.
		/// Contrary to what could be expected due to its current name, <paramref name="xml"/> must be a plain string,
		/// not n xml encoded string.</para>
		/// </remarks>
		object StringToObject(string xml);
	}
}
