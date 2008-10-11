using System;
using NHibernate.Properties;
using NHibernate.Util;

namespace NHibernate.Bytecode
{
	/// <summary>
	/// A <see cref="IBytecodeProvider" /> implementation that returns
	/// <see langword="null" />, disabling reflection optimization.
	/// </summary>
	public class NullBytecodeProvider : IBytecodeProvider, IInjectableProxyFactoryFactory
	{
		private System.Type proxyFactoryFactory;

		#region IBytecodeProvider Members

		public IProxyFactoryFactory ProxyFactoryFactory
		{
			get
			{
				if (proxyFactoryFactory != null)
				{
					try
					{
						return (IProxyFactoryFactory) Activator.CreateInstance(proxyFactoryFactory);
					}
					catch (Exception e)
					{
						throw new HibernateException("Failed to create an instance of '" + proxyFactoryFactory.FullName + "'!", e);
					}
				}
				throw new HibernateException("The ProxyFactoryFactory was not configured. Initialize the 'proxyfactory.factory_class' property of the session-factory section.");
			}
		}

		public IReflectionOptimizer GetReflectionOptimizer(System.Type clazz, IGetter[] getters, ISetter[] setters)
		{
			return null;
		}

		#endregion

		#region Implementation of IInjectableProxyFactoryFactory

		public void SetProxyFactoryFactory(string typeName)
		{
			System.Type pffc;
			try
			{
				pffc = ReflectHelper.ClassForName(typeName);
			}
			catch (HibernateException he)
			{
				throw new HibernateException("Unable to load type '" + typeName + "' during configuration of proxy factory class.",
				                             he);
			}

			if (typeof (IProxyFactoryFactory).IsAssignableFrom(pffc) == false)
			{
				var he = new HibernateException(pffc.FullName + " does not implement " + typeof (IProxyFactoryFactory).FullName);
				throw he;
			}
			proxyFactoryFactory = pffc;
		}

		#endregion
	}
}