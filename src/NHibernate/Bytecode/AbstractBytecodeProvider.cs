using System;
using NHibernate.Properties;
using NHibernate.Util;

namespace NHibernate.Bytecode
{
	public abstract class AbstractBytecodeProvider : IBytecodeProvider, IInjectableProxyFactoryFactory, IInjectableCollectionTypeFactoryClass
	{
		private readonly IObjectsFactory objectsFactory = new ActivatorObjectsFactory();
		protected System.Type proxyFactoryFactory;
		private ICollectionTypeFactory collectionTypeFactory;
		private System.Type collectionTypeFactoryClass = typeof(Type.DefaultCollectionTypeFactory);

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
				return new DefaultProxyFactoryFactory();
			}
		}

		public abstract IReflectionOptimizer GetReflectionOptimizer(System.Type clazz, IGetter[] getters, ISetter[] setters);

		public virtual IObjectsFactory ObjectsFactory
		{
			get { return objectsFactory; }
		}

		public virtual ICollectionTypeFactory CollectionTypeFactory
		{
			get
			{
				if (collectionTypeFactory == null)
				{
					try
					{
						collectionTypeFactory =
							(ICollectionTypeFactory) ObjectsFactory.CreateInstance(collectionTypeFactoryClass);
					}
					catch (Exception e)
					{
						throw new HibernateByteCodeException("Failed to create an instance of CollectionTypeFactory!", e);
					}
				}
				return collectionTypeFactory;
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

		#region Implementation of IInjectableCollectionTypeFactoryClass

		public void SetCollectionTypeFactoryClass(string typeAssemblyQualifiedName)
		{
			if (string.IsNullOrEmpty(typeAssemblyQualifiedName))
			{
				throw new ArgumentNullException("typeAssemblyQualifiedName");
			}
			System.Type ctf= ReflectHelper.ClassForName(typeAssemblyQualifiedName);
			SetCollectionTypeFactoryClass(ctf);
		}

		public void SetCollectionTypeFactoryClass(System.Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (typeof(ICollectionTypeFactory).IsAssignableFrom(type) == false)
			{
				throw new HibernateByteCodeException(type.FullName + " does not implement " + typeof(ICollectionTypeFactory).FullName);
			}
			if (collectionTypeFactory != null && !collectionTypeFactoryClass.Equals(type))
			{
				throw new InvalidOperationException("CollectionTypeFactory in use, can't change it.");
			}
			collectionTypeFactoryClass = type;
		}

		#endregion
	}
}