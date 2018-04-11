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

		[NonSerialized]
		private System.Type _mappedClass;
		private SerializableSystemType _serializableMappedClass;

		[NonSerialized]
		private IInstantiationOptimizer _optimizer;

		[NonSerialized]
		private readonly IProxyFactory _proxyFactory;

		private readonly bool _generateFieldInterceptionProxy;

		private readonly bool _embeddedIdentifier;

		[NonSerialized]
		private ConstructorInfo _constructor;

		[NonSerialized]
		private System.Type _proxyInterface;
		private SerializableSystemType _serializableProxyInterface;

		public PocoInstantiator()
		{
		}

		public PocoInstantiator(Mapping.Component component, IInstantiationOptimizer optimizer)
		{
			_mappedClass = component.ComponentClass;
			_optimizer = optimizer;

			_proxyInterface = null;
			_embeddedIdentifier = false;

			try
			{
				_constructor = ReflectHelper.GetDefaultConstructor(_mappedClass);
			}
			catch (PropertyNotFoundException)
			{
				log.Info("no default (no-argument) constructor for class: {0} (class must be instantiated by Interceptor)", _mappedClass.FullName);
				_constructor = null;
			}
		}

		public PocoInstantiator(PersistentClass persistentClass, IInstantiationOptimizer optimizer, IProxyFactory proxyFactory, bool generateFieldInterceptionProxy)
		{
			_mappedClass = persistentClass.MappedClass;
			_proxyInterface = persistentClass.ProxyInterface;
			_embeddedIdentifier = persistentClass.HasEmbeddedIdentifier;
			_optimizer = optimizer;
			_proxyFactory = proxyFactory;
			_generateFieldInterceptionProxy = generateFieldInterceptionProxy;

			try
			{
				_constructor = ReflectHelper.GetDefaultConstructor(_mappedClass);
			}
			catch (PropertyNotFoundException)
			{
				log.Info("no default (no-argument) constructor for class: {0} (class must be instantiated by Interceptor)", _mappedClass.FullName);
				_constructor = null;
			}
		}

		#region IInstantiator Members

		public object Instantiate(object id)
		{
			bool useEmbeddedIdentifierInstanceAsEntity = _embeddedIdentifier && id != null && id.GetType().Equals(_mappedClass);
			return useEmbeddedIdentifierInstanceAsEntity ? id : Instantiate();
		}

		public object Instantiate()
		{
			if (ReflectHelper.IsAbstractClass(_mappedClass))
			{
				throw new InstantiationException("Cannot instantiate abstract class or interface: ", _mappedClass);
			}
			if (_generateFieldInterceptionProxy)
			{
				return _proxyFactory.GetFieldInterceptionProxy(GetInstance());
			}
			return GetInstance();
		}

		private object GetInstance()
		{
			if (_optimizer != null)
			{
				return _optimizer.CreateInstance();
			}
			if (_mappedClass.IsValueType)
			{
				return Cfg.Environment.BytecodeProvider.ObjectsFactory.CreateInstance(_mappedClass, true);
			}
			if (_constructor == null)
			{
				throw new InstantiationException("No default constructor for entity: ", _mappedClass);
			}
			try
			{
				return _constructor.Invoke(null);
			}
			catch (Exception e)
			{
				throw new InstantiationException("Could not instantiate entity: ", e, _mappedClass);
			}
		}

		public bool IsInstance(object obj)
		{
			return _mappedClass.IsInstanceOfType(obj) || (_proxyInterface != null && _proxyInterface.IsInstanceOfType(obj)); //this one needed only for guessEntityMode()
		}

		#endregion

		[OnSerializing]
		private void OnSerializing(StreamingContext context)
		{
			_serializableMappedClass = SerializableSystemType.Wrap(_mappedClass);
			_serializableProxyInterface = SerializableSystemType.Wrap(_proxyInterface);
		}

		#region IDeserializationCallback Members

		public void OnDeserialization(object sender)
		{
			if (_generateFieldInterceptionProxy)
			{
				throw new InvalidOperationException("IProxyFactory implementors are currently not serializable.");
			}

			_mappedClass = _serializableMappedClass?.GetSystemType();
			_proxyInterface = _serializableProxyInterface?.GetSystemType();
			_constructor = ReflectHelper.GetDefaultConstructor(_mappedClass);
		}

		#endregion

		public void SetOptimizer(IInstantiationOptimizer optimizer)
		{
			_optimizer = optimizer;
		}
	}
}
