using NHibernate.Transaction;
namespace NHibernate.Cfg.Loquacious
{
	public interface ITransactionConfiguration
	{
		IDbIntegrationConfiguration Through<TFactory>() where TFactory : ITransactionFactory;
	}
}