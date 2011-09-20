using System;

namespace NHibernate
{
	/// <summary>
	/// Represents a fetch strategy for lazy properties
	/// </summary>
	/// <remarks>
	/// This is used together with the <see cref="ICriteria"/> API to specify
	/// runtime fetching strategies.
	/// <para>
	/// For Hql queries, use the <c>fetch all properties</c> clause instead.
	/// </para>
	/// </remarks>
	[Serializable]
	public enum LazyPropertyFetchMode
	{
		/// <summary>
		/// The default fetch mode, which is to fetch lazy properties separately
		/// </summary>
		Default,

		/// <summary>
		/// Eager fetch lazy properties
		/// </summary>
		Select,
	}
}
