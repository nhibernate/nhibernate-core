using System;

namespace NHibernate.Type {

	/// <summary>
	/// An <see cref="IType"/> that may be used to version data.
	/// </summary>
	public interface IVersionType : IType {

		/// <summary>
		/// When implemented by a class, gets an initial version.
		/// </summary>
		/// <value>Returns an instance of the <see cref="IType"/></value>
		object Seed { get; }

		/// <summary>
		/// When implemented by a class, increments the version.
		/// </summary>
		/// <param name="current">The current version</param>
		/// <returns>an instance of the <see cref="IType"/> that has been incremented.</returns>
		object Next(object current);
	}
}