using System.Collections;
using NHibernate.Engine;

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

		object FromStringValue(string xml);
	}
}
