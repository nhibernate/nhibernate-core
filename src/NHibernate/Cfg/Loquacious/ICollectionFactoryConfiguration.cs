using System;
using NHibernate.Bytecode;
namespace NHibernate.Cfg.Loquacious
{
	//Since 5.3
	[Obsolete("Replaced by direct class usage")]
	public interface ICollectionFactoryConfiguration
	{
		IFluentSessionFactoryConfiguration Through<TCollecionsFactory>() where TCollecionsFactory : ICollectionTypeFactory;
	}
}
