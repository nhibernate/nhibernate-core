namespace NHibernate.Type
{
	/// <summary>
	/// An <see cref="IType"/> that may be used as an identifier.
	/// </summary>
	public interface IIdentifierType : IType
	{
		/// <summary>
		/// When implemented by a class, converts the xml string from the 
		/// mapping file to the .NET object.
		/// </summary>
		/// <param name="xml">The value of <c>discriminator-value</c> or <c>unsaved-value</c> attribute.</param>
		/// <returns>The string converted to the object.</returns>
		/// <remarks>
		/// This method needs to be able to handle any string.  It should not just 
		/// call System.Type.Parse without verifying that it is a parsable value
		/// for the System.Type.
		/// </remarks>
		object StringToObject(string xml);
	}
}