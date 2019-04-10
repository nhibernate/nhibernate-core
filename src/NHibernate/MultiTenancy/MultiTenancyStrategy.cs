namespace NHibernate.MultiTenancy
{
	/// <summary>
	/// Strategy for multi-tenancy
	/// <seealso cref="Cfg.Environment.MultiTenancy"/>
	/// </summary>
	public enum MultiTenancyStrategy
	{
		/// <summary>
		/// No multi-tenancy
		/// </summary>
		None,

//		/// <summary>
//		/// Multi-tenancy implemented by use of discriminator columns.
//		/// </summary>
//		Discriminator,
//
//		/// <summary>
//		/// Multi-tenancy implemented as separate schemas.
//		/// </summary>
//		Schema,

		/// <summary>
		/// Multi-tenancy implemented as separate database per tenant.
		/// </summary>
		Database,
	}
}
