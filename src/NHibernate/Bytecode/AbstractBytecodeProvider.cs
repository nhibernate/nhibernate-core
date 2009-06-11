using System;
using NHibernate.Properties;
using NHibernate.Util;

namespace NHibernate.Bytecode
{
	public abstract class AbstractBytecodeProvider : IBytecodeProvider, IInjectableProxyFactoryFactory
	{
		private readonly IObjectsFactory objectsFactory = new ActivatorObjectsFactory();
		protected System.Type proxyFactoryFactory;
		private ICollectionTypeFactory collectionTypeFactory;

		#region IBytecodeProvider Members

		public virtual IProxyFactoryFactory ProxyFactoryFactory
		{
			get
			{
				if (proxyFactoryFactory != null)
				{
					try
					{
						return (IProxyFactoryFactory) ObjectsFactory.CreateInstance(proxyFactoryFactory);
					}
					catch (Exception e)
					{
						throw new HibernateByteCodeException("Failed to create an instance of '" + proxyFactoryFactory.FullName + "'!", e);
					}
				}

				throw new ProxyFactoryFactoryNotConfiguredException();
			}
		}

		public abstract IReflectionOptimizer GetReflectionOptimizer(System.Type clazz, IGetter[] getters, ISetter[] setters);

		public virtual IObjectsFactory ObjectsFactory
		{
			get { return objectsFactory; }
		}

		public ICollectionTypeFactory CollectionTypeFactory
		{
			get
			{
				if (collectionTypeFactory == null)
				{
					try
					{
						collectionTypeFactory =
							(ICollectionTypeFactory) ObjectsFactory.CreateInstance(typeof (Type.DefaultCollectionTypeFactory));
					}
					catch (Exception e)
					{
						throw new HibernateByteCodeException("Failed to create an instance of CollectionTypeFactory!", e);
					}
				}
				return collectionTypeFactory;
			}
			protected set
			{
				if(value == null)
				{
					throw new InvalidOperationException("The CollectionTypeFactory can't be null.");
				}
				collectionTypeFactory = value;
			}
		}

		#endregion

		#region IInjectableProxyFactoryFactory Members

		public virtual void SetProxyFactoryFactory(string typeName)
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

			if (typeof(IProxyFactoryFactory).IsAssignableFrom(pffc) == false)
			{
				var he = new HibernateByteCodeException(pffc.FullName + " does not implement " + typeof(IProxyFactoryFactory).FullName);
				throw he;
			}
			proxyFactoryFactory = pffc;
		}

		#endregion
	}
}