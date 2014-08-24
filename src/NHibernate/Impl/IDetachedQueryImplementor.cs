using NHibernate;

namespace NHibernate.Impl
{
	/// <summary>
	/// Interface for DetachedQuery implementors.
	/// </summary>
	/// <remarks>
	/// When you are working with queries in "detached mode" you may need some additional services like clone, 
	/// copy of parameters from another query and so on.
	/// </remarks>
	public interface IDetachedQueryImplementor
	{
		/// <summary>
		/// Copy all properties to a given <see cref="IDetachedQuery"/>.
		/// </summary>
		/// <param name="destination">The given <see cref="IDetachedQuery"/>.</param>
		/// <remarks>
		/// Usually the implementation use <see cref="IDetachedQuery"/> to set properties to the <paramref name="destination"/>.
		/// This mean that existing properties are merged/overriden.
		/// </remarks>
		void CopyTo(IDetachedQuery destination);

		/// <summary>
		/// Set only parameters to a given <see cref="IDetachedQuery"/>.
		/// </summary>
		/// <param name="destination">The given <see cref="IDetachedQuery"/>.</param>
		/// <remarks>
		/// Existing parameters are merged/overriden.
		/// </remarks>
		void SetParametersTo(IDetachedQuery destination);

		/// <summary>
		/// Override all properties reading new values from a given <see cref="IDetachedQueryImplementor"/>.
		/// </summary>
		/// <param name="origin">The given origin.</param>
		void OverrideInfoFrom(IDetachedQueryImplementor origin);

		/// <summary>
		/// Override all parameters reading new values from a given <see cref="IDetachedQueryImplementor"/>.
		/// </summary>
		/// <param name="origin">The given origin.</param>
		void OverrideParametersFrom(IDetachedQueryImplementor origin);
	}
}