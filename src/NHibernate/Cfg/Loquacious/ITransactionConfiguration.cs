using System;
using NHibernate.Transaction;
namespace NHibernate.Cfg.Loquacious
{
	//Since 5.3
	[Obsolete("Replaced by direct class usage")]
	public interface ITransactionConfiguration
	{
		IDbIntegrationConfiguration Through<TFactory>() where TFactory : ITransactionFactory;
	}
}
