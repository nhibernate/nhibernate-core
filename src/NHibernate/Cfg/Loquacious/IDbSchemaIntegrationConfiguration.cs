using System;

namespace NHibernate.Cfg.Loquacious
{
	//Since 5.3
	[Obsolete("Replaced by direct class usage")]
	public interface IDbSchemaIntegrationConfiguration
	{
		IDbIntegrationConfiguration Recreating();
		IDbIntegrationConfiguration Creating();
		IDbIntegrationConfiguration Updating();
		IDbIntegrationConfiguration Validating();
	}
}
