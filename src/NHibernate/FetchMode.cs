using System;

namespace NHibernate
{
	/// <summary>
	/// Represents a fetching strategy.
	/// </summary>
	/// <remarks>
	/// This is used together with the <see cref="ICriteria"/> API to specify
	/// runtime fetching strategies.
	/// <para>
	/// For Hql queries, use the <c>FETCH</c> keyword instead.
	/// </para>
	/// </remarks>
	[ Serializable ]
	public enum FetchMode
	{
		/// <summary>
		/// Default to the setting configured in the mapping file.
		/// </summary>
		Default = 0,
		/// <summary>
		/// Fetch lazily.  Equivalent to <c>outer-join="false"</c>
		/// </summary>
		Lazy = 1,
		/// <summary>
		/// Fetch eagerly, using an outer join.  Equivalent to <c>outer-join="true"</c>
		/// </summary>
		Eager = 2
	}
}