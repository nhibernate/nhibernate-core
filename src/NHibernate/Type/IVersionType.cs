using System.Collections;
using NHibernate.Engine;

namespace NHibernate.Type
{
	/// <summary>
	/// An <see cref="IType"/> that may be used to version data.
	/// </summary>
	public partial interface IVersionType : IType
	{
		/// <summary>
		/// When implemented by a class, increments the version.
		/// </summary>
		/// <param name="current">The current version</param>
		/// <param name="session">The current session, if available.</param>
		/// <returns>an instance of the <see cref="IType"/> that has been incremented.</returns>
		object Next(object current, ISessionImplementor session);

		/// <summary>
		/// When implemented by a class, gets an initial version.
		/// </summary>
		/// <param name="session">The current session, if available.</param>
		/// <returns>An instance of the type.</returns>
		object Seed(ISessionImplementor session);

		/// <summary>
		/// Get a comparator for the version numbers
		/// </summary>
		IComparer Comparator { get; }

		// 6.0 TODO: rename "xml" parameter as "value": it is not a xml string. The fact it generally comes from a xml
		// attribute value is irrelevant to the method behavior.
		/// <summary>
		/// Parse the string representation of a value to convert it to the .NET object.
		/// </summary>
		/// <param name="xml">A string representation.</param>
		/// <returns>The value.</returns>
		/// <remarks>Notably meant for parsing <c>unsave-value</c> mapping attribute value. Contrary to what could
		/// be expected due to its current name, <paramref name="xml"/> must be a plain string, not a xml encoded
		/// string.</remarks>
		object FromStringValue(string xml);
	}
}
