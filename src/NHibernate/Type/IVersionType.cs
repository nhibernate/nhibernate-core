using System.Collections;

namespace NHibernate.Type
{
	/// <summary>
	/// An <see cref="IType"/> that may be used to version data.
	/// </summary>
	public interface IVersionType : IType
	{
		/// <summary>
		/// When implemented by a class, increments the version.
		/// </summary>
		/// <param name="current">The current version</param>
		/// <returns>an instance of the <see cref="IType"/> that has been incremented.</returns>
		object Next( object current );

		/// <summary>
		/// When implemented by a class, gets an initial version.
		/// </summary>
		/// <value>Returns an instance of the <see cref="IType"/></value>
		object Seed { get; }

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
		object StringToObject( string xml );

		/// <summary>
		/// Get a comparator for the version numbers
		/// </summary>
		IComparer Comparator { get; }
	}
}