using System;
using System.Reflection;
using System.Runtime.Serialization;

using NHibernate.Bytecode;
using NHibernate.Mapping;
using NHibernate.Proxy;
using NHibernate.Util;

namespace NHibernate.Tuple
{
	/// <summary> Defines a POCO-based instantiator for use from the tuplizers.</summary>
	[Serializable]
	public class PocoInstantiator : IInstantiator, IDeserializationCallback
	{
		private static readonly INHibernateLogger log = NHibernateLogger.For(typeof(PocoInstantiator));

		private readonly System.Type mappedClass;

		[NonSerialized]
		private IInstantiationOptimizer optimizer;

		private readonly IProxyFactory proxyFactory; // 6.0 TODO: remove

		private readonly bool generateFieldInterceptionProxy; // 6.0 TODO: remove

		private readonly bool embeddedIdentifier;

		private readonly bool _isAbstract;

		[NonSerialized]
		private ConstructorInfo constructor;

		private readonly System.Type proxyInterface; // 6.0 TODO: remove

		// Since 5.3
		[Obsolete("This constructor has no more usage in NHibernate and will be removed in a future version.")]
		public PocoInstantiator()
		{
		}

		public PocoInstantiator(Mapping.Component component, IInstantiationOptimizer optimizer)
		{
			mappedClass = component.ComponentClass;
			this.optimizer = optimizer;

			proxyInterface = null;
			embeddedIdentifier = false;

			try
			{
				constructor = ReflectHelper.GetDefaultConstructor(mappedClass);
			}
			catch (PropertyNotFoundException)
			{
				log.Info("no default (no-argument) constructor for class: {0} (class must be instantiated by Interceptor)", mappedClass.FullName);
				constructor = null;
			}
		}

		// Since 5.3
		[Obsolete("Use PocoEntityInstantiator class instead.")]
		public PocoInstantiator(PersistentClass persistentClass, IInstantiationOptimizer optimizer, IProxyFactory proxyFactory, bool generateFieldInterceptionProxy)
			: this(persistentClass.MappedClass, optimizer, persistentClass.HasEmbeddedIdentifier)
		{
			proxyInterface = persistentClass.ProxyInterface;
			this.proxyFactory = proxyFactory;
			this.generateFieldInterceptionProxy = generateFieldInterceptionProxy;
		}

		public PocoInstantiator(System.Type mappedClass, IInstantiationOptimizer optimizer, bool embeddedIdentifier)
		{
			this.mappedClass = mappedClass;
			this.optimizer = optimizer;
			this.embeddedIdentifier = embeddedIdentifier;
			_isAbstract = ReflectHelper.IsAbstractClass(mappedClass);

			try
			{
				constructor = ReflectHelper.GetDefaultConstructor(mappedClass);
			}
			catch (PropertyNotFoundException)
			{
				log.Info("no default (no-argument) constructor for class: {0} (class must be instantiated by Interceptor)", mappedClass.FullName);
				constructor = null;
			}
		}

		#region IInstantiator Members

		public object Instantiate(object id)
		{
			bool useEmbeddedIdentifierInstanceAsEntity = embeddedIdentifier && id != null && id.GetType().Equals(mappedClass);
			return useEmbeddedIdentifierInstanceAsEntity ? id : Instantiate();
		}

		public object Instantiate()
		{
			if (_isAbstract)
			{
				throw new InstantiationException("Cannot instantiate abstract class or interface: ", mappedClass);
			}
			// 6.0 TODO: Remove if statement
			if (generateFieldInterceptionProxy)
			{
				return proxyFactory.GetFieldInterceptionProxy(CreateInstance);
			}
			return CreateInstance();
		}

		protected virtual object CreateInstance()
		{
			if (optimizer != null)
			{
				return optimizer.CreateInstance();
			}
			if (mappedClass.IsValueType)
			{
				return Activator.CreateInstance(mappedClass, true);
			}
			if (constructor == null)
			{
				throw new InstantiationException("No default constructor for entity: ", mappedClass);
			}
			try
			{
				return constructor.Invoke(null);
			}
			catch (Exception e)
			{
				throw new InstantiationException("Could not instantiate entity: ", e, mappedClass);
			}
		}

		public virtual bool IsInstance(object obj)
		{
			// 6.0 TODO: Remove the proxyInterface check
			return mappedClass.IsInstanceOfType(obj) || (proxyInterface != null && proxyInterface.IsInstanceOfType(obj)); //this one needed only for guessEntityMode()
		}

		#endregion

		#region IDeserializationCallback Members

		public void OnDeserialization(object sender)
		{
			constructor = ReflectHelper.GetDefaultConstructor(mappedClass);
		}

		#endregion

		public void SetOptimizer(IInstantiationOptimizer optimizer)
		{
			this.optimizer = optimizer;
		}
	}
}
