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
						throw new HibernateByteCodeException("Failed to create an instance of '" + proxyFactoryFactory.FullName + "'!", e);
					}
				}
				throw new ProxyFactoryFactoryNotConfiguredException();
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
			catch (Exception he)
			{
				throw new UnableToLoadProxyFactoryFactoryException(typeName, he);
			}

			if (typeof (IProxyFactoryFactory).IsAssignableFrom(pffc) == false)
			{
				var he = new HibernateByteCodeException(pffc.FullName + " does not implement " + typeof(IProxyFactoryFactory).FullName);
				throw he;
			}
			proxyFactoryFactory = pffc;
		}

		#endregion
	}
}