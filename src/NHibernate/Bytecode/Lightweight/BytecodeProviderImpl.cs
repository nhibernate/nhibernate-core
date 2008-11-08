using System;
using NHibernate.Properties;
using NHibernate.Util;

namespace NHibernate.Bytecode.Lightweight
{
	/// <summary>
	/// Factory that generate object based on IReflectionOptimizer needed to replace the use
	/// of reflection.
	/// </summary>
	/// <remarks>
	/// Used in <see cref="NHibernate.Persister.Entity.AbstractEntityPersister"/> and
	/// <see cref="NHibernate.Type.ComponentType"/>
	/// </remarks>
	public class BytecodeProviderImpl : IBytecodeProvider, IInjectableProxyFactoryFactory
	{
		private System.Type proxyFactoryFactory;

		#region IBytecodeProvider Members

		public virtual IProxyFactoryFactory ProxyFactoryFactory
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

		/// <summary>
		/// Generate the IReflectionOptimizer object
		/// </summary>
		/// <param name="mappedClass">The target class</param>
		/// <param name="setters">Array of setters</param>
		/// <param name="getters">Array of getters</param>
		/// <returns><see langword="null" /> if the generation fails</returns>
		public IReflectionOptimizer GetReflectionOptimizer(System.Type mappedClass, IGetter[] getters, ISetter[] setters)
		{
			return new ReflectionOptimizer(mappedClass, getters, setters);
		}

		#endregion

		#region IInjectableProxyFactoryFactory Members

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