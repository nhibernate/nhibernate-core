namespace NHibernate.Cfg.Loquacious
{
	public interface IDbSchemaIntegrationConfiguration
	{
		IDbIntegrationConfiguration Recreating();
		IDbIntegrationConfiguration Creating();
		IDbIntegrationConfiguration Updating();
		IDbIntegrationConfiguration Validating();
	}
}