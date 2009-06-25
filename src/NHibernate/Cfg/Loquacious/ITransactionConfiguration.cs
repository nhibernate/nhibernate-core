using NHibernate.Transaction;
namespace NHibernate.Cfg.Loquacious
{
	public interface ITransactionConfiguration
	{
		IDbIntegrationConfiguration Trough<TFactory>() where TFactory : ITransactionFactory;
	}
}