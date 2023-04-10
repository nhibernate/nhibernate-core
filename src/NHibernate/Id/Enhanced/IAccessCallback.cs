namespace NHibernate.Id.Enhanced
{
	/// <summary>
	/// Contract for providing callback access to an <see cref="IDatabaseStructure"/>,
	/// typically from the <see cref="IOptimizer"/>.
	/// </summary>
	public partial interface IAccessCallback
	{
		/// <summary>
		/// Retrieve the next value from the underlying source.
		/// </summary>
		long GetNextValue();

		/// <summary>
		/// Obtain the tenant identifier (multi-tenancy), if one, associated with this callback.
		/// </summary>
		string GetTenantIdentifier();
	}
}
