using System;
using System.Reflection;
using System.Runtime.Serialization;
using log4net;
using NHibernate.Bytecode;
using NHibernate.Mapping;
using NHibernate.Util;

namespace NHibernate.Tuple
{
	/// <summary> Defines a POCO-based instantiator for use from the tuplizers.</summary>
	[Serializable]
	public class PocoInstantiator : IInstantiator, IDeserializationCallback
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(PocoInstantiator));
		
		private readonly System.Type mappedClass;
		
		[NonSerialized]
		private readonly IInstantiationOptimizer optimizer;

		private readonly bool embeddedIdentifier;
		
		[NonSerialized]
		private ConstructorInfo constructor;
		
		private readonly System.Type proxyInterface;

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
				log.Info(string.Format("no default (no-argument) constructor for class: {0} (class must be instantiated by Interceptor)", mappedClass.FullName));
				constructor = null;
			}
		}

		public PocoInstantiator(PersistentClass persistentClass, IInstantiationOptimizer optimizer)
		{
			mappedClass = persistentClass.MappedClass;
			proxyInterface = persistentClass.ProxyInterface;
			embeddedIdentifier = persistentClass.HasEmbeddedIdentifier;
			this.optimizer = optimizer;

			try
			{
				constructor = ReflectHelper.GetDefaultConstructor(mappedClass);
			}
			catch (PropertyNotFoundException)
			{
				log.Info(string.Format("no default (no-argument) constructor for class: {0} (class must be instantiated by Interceptor)", mappedClass.FullName));
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
			if (ReflectHelper.IsAbstractClass(mappedClass))
			{
				throw new InstantiationException("Cannot instantiate abstract class or interface: ", mappedClass);
			}
			else if (optimizer != null)
			{
				return optimizer.CreateInstance();
			}
			else if (mappedClass.IsValueType)
			{
				return Cfg.Environment.BytecodeProvider.ObjectsFactory.CreateInstance(mappedClass, true);
			}
			else if (constructor == null)
			{
				throw new InstantiationException("No default constructor for entity: ", mappedClass);
			}
			else
			{
				try
				{
					return constructor.Invoke(null);
				}
				catch (Exception e)
				{
					throw new InstantiationException("Could not instantiate entity: ", e, mappedClass);
				}
			}
		}

		public bool IsInstance(object obj)
		{
			return mappedClass.IsInstanceOfType(obj) || (proxyInterface != null && proxyInterface.IsInstanceOfType(obj)); //this one needed only for guessEntityMode()
		}

		#endregion

		#region IDeserializationCallback Members

		public void OnDeserialization(object sender)
		{
			constructor = ReflectHelper.GetDefaultConstructor(mappedClass);
		}

		#endregion
	}
}
