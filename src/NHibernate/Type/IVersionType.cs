using System;

namespace NHibernate.Type {

	/// <summary>
	/// An IType that may be used to version data.
	/// </summary>
	public interface IVersionType : IType {

		/// <summary>
		/// Generate an initial version.
		/// Return an instance of the type.
		/// </summary>
		public object Seed { get; }

		/// <summary>
		/// Increment the version.
		/// </summary>
		/// <param name="current">the current version</param>
		/// <returns>an instance of the type</returns>
		public object Next(object current);
	}
}