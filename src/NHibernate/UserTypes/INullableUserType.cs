using System;

namespace NHibernate.UserTypes
{
	/// <summary>
	/// A custom type with certain not-<c>null</c> values represented as <c>NULL</c>
	/// in the database.
	/// </summary>
	/// <remarks>Implementing this interface is useful if a property of the type
	/// is used in a class with <c>dynamic-update</c> or <c>dynamic-insert</c>
	/// set to <c>true</c>.
	/// </remarks>
	public interface INullableUserType
	{
		/// <summary>
		/// Determines whether the specified value is represented as <c>NULL</c> in the database.
		/// </summary>
		/// <param name="value">The value, may be <c>null</c>.</param>
		/// <returns>
		/// <c>true</c> if the specified value is represented as <c>NULL</c> in the database;
		/// otherwise, <c>false</c>.
		/// </returns>
		bool IsDatabaseNull(object value);
	}
}
