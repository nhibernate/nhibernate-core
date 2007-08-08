using System;
using NHibernate.Cfg.ConfigurationSchema;

namespace NHibernate.Cfg
{
	public interface IHibernateConfiguration
	{
		BytecodeProviderType ByteCodeProviderType { get; }
		bool UseReflectionOptimizer { get; }
		SessionFactoryConfiguration SessionFactory { get; }
	}
}
