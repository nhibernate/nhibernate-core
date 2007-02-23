using System;

namespace NHibernate.UserTypes
{
	/// <summary>
	/// A custom type with certain not-<see langword="null" /> values represented as <see langword="null" />
	/// in the database.
	/// </summary>
	/// <remarks>Implementing this interface is useful if a property of the type
	/// is used in a class with <c>dynamic-update</c> or <c>dynamic-insert</c>
	/// set to <see langword="true" />.
	/// </remarks>
	public interface INullableUserType
	{
		/// <summary>
		/// Determines whether the specified value is represented as <see langword="null" /> in the database.
		/// </summary>
		/// <param name="value">The value, may be <see langword="null" />.</param>
		/// <returns>
		/// <see langword="true" /> if the specified value is represented as <see langword="null" /> in the database;
		/// otherwise, <see langword="false" />.
		/// </returns>
		bool IsDatabaseNull(object value);
	}
}