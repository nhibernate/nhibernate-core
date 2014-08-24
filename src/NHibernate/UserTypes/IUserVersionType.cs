using System;
using System.Collections;
using NHibernate.Engine;

namespace NHibernate.UserTypes
{
	/// <summary>
	/// A user type that may be used for a version property.
	/// </summary>
	public interface IUserVersionType : IUserType, IComparer
	{
		/// <summary>
		/// Generate an initial version.
		/// </summary>
		/// <param name="session">The session from which this request originates.  May be
		/// null; currently this only happens during startup when trying to determine
		/// the "unsaved value" of entities.</param>
		/// <returns>an instance of the type</returns>
		object Seed(ISessionImplementor session);

		/// <summary>
		/// Increment the version.
		/// </summary>
		/// <param name="current">The session from which this request originates.</param>
		/// <param name="session">the current version</param>
		/// <returns>an instance of the type</returns>
		object Next(object current, ISessionImplementor session);
	}
}