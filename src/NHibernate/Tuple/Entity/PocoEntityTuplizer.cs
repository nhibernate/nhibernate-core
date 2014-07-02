using System;
using System.Collections.Generic;
using System.Reflection;

using NHibernate.Bytecode;
using NHibernate.Classic;
using NHibernate.Engine;
using NHibernate.Intercept;
using NHibernate.Mapping;
using NHibernate.Properties;
using NHibernate.Proxy;
using NHibernate.Type;
using NHibernate.Util;
using System.Runtime.Serialization;

namespace NHibernate.Tuple.Entity
{

	/// <summary> An <see cref="IEntityTuplizer"/> specific to the POCO entity mode. </summary>
	public class PocoEntityTuplizer : AbstractEntityTuplizer
	{
		private static readonly IInternalLogger log = LoggerProvider.LoggerFor(typeof(PocoEntityTuplizer));
		private readonly System.Type mappedClass;
		private readonly System.Type proxyInterface;
		private readonly bool islifecycleImplementor;
		private readonly bool isValidatableImplementor;
		private readonly HashSet<string> lazyPropertyNames = new HashSet<string>();
		private readonly HashSet<string> unwrapProxyPropertyNames = new HashSet<string>();
		[NonSerialized]
		private IReflectionOptimizer optimizer;
		private readonly IProxyValidator proxyValidator;

		[OnDeserialized]
		internal void OnDeserialized(StreamingContext context)
		{
			SetReflectionOptimizer();

			if (optimizer != null)
			{
				// Also set the InstantiationOptimizer on the deserialized PocoInstantiator.
				((PocoInstantiator)Instantiator).SetOptimizer(optimizer.InstantiationOptimizer);
			}

			ClearOptimizerWhenUsingCustomAccessors();
		}
		protected void SetReflectionOptimizer()
		{
			if (Cfg.Environment.UseReflectionOptimizer)
			{
				// NH different behavior fo NH-1587
				optimizer = Cfg.Environment.BytecodeProvider.GetReflectionOptimizer(mappedClass, getters, setters);
			}
		}
		public PocoEntityTuplizer(EntityMetamodel entityMetamodel, PersistentClass mappedEntity)
			: base(entityMetamodel, mappedEntity)
		{
			mappedClass = mappedEntity.MappedClass;
			proxyInterface = mappedEntity.ProxyInterface;
			islifecycleImplementor = typeof(ILifecycle).IsAssignableFrom(mappedClass);
			isValidatableImplementor = typeof(IValidatable).IsAssignableFrom(mappedClass);

			foreach (Mapping.Property property in mappedEntity.PropertyClosureIterator)
			{
				if (property.IsLazy)
					lazyPropertyNames.Add(property.Name);
				if (property.UnwrapProxy)
					unwrapProxyPropertyNames.Add(property.Name);
			}
			SetReflectionOptimizer();

			Instantiator = BuildInstantiator(mappedEntity);

			ClearOptimizerWhenUsingCustomAccessors();

			proxyValidator = Cfg.Environment.BytecodeProvider.ProxyFactoryFactory.ProxyValidator;
		}

		public override System.Type ConcreteProxyClass
		{
			get { return proxyInterface; }
		}

		public override bool IsInstrumented
		{
			get 
			{
				// NH: we can't really check for EntityMetamodel.HasLazyProperties and/or EntityMetamodel.HasUnwrapProxyForProperties here
				// because this property is used even where subclasses has lazy-properties.
				// Checking it here, where the root-entity has no lazy properties we will eager-load/double-load those properties.
				return FieldInterceptionHelper.IsInstrumented(MappedClass);
			}
		}

		public override System.Type MappedClass
		{
			get { return mappedClass; }
		}

		protected override IGetter BuildPropertyGetter(Mapping.Property mappedProperty, PersistentClass mappedEntity)
		{
			return mappedProperty.GetGetter(mappedEntity.MappedClass);
		}

		protected override ISetter BuildPropertySetter(Mapping.Property mappedProperty, PersistentClass mappedEntity)
		{
			return mappedProperty.GetSetter(mappedEntity.MappedClass);
		}

		protected override IInstantiator BuildInstantiator(PersistentClass persistentClass)
		{
			if (optimizer == null)
			{
				log.Debug("Create Instantiator without optimizer for:" + persistentClass.MappedClass.FullName);
				return new PocoInstantiator(persistentClass, null, ProxyFactory, EntityMetamodel.HasLazyProperties || EntityMetamodel.HasUnwrapProxyForProperties);
			}
			else
			{
				log.Debug("Create Instantiator using optimizer for:" + persistentClass.MappedClass.FullName);
				return new PocoInstantiator(persistentClass, optimizer.InstantiationOptimizer, ProxyFactory, EntityMetamodel.HasLazyProperties || EntityMetamodel.HasUnwrapProxyForProperties);
			}
		}

		protected override IProxyFactory BuildProxyFactory(PersistentClass persistentClass, IGetter idGetter,
		                                                            ISetter idSetter)
		{
			bool needAccesorCheck = true; // NH specific (look the comment below)

			// determine the id getter and setter methods from the proxy interface (if any)
			// determine all interfaces needed by the resulting proxy
			var proxyInterfaces = new HashSet<System.Type> {typeof (INHibernateProxy)};

			System.Type _mappedClass = persistentClass.MappedClass;
			System.Type _proxyInterface = persistentClass.ProxyInterface;

			if (_proxyInterface != null && !_mappedClass.Equals(_proxyInterface))
			{
				if (!_proxyInterface.IsInterface)
				{
					throw new MappingException("proxy must be either an interface, or the class itself: " + EntityName);
				}
				needAccesorCheck = false; // NH (the proxy is an interface all properties can be overridden)
				proxyInterfaces.Add(_proxyInterface);
			}

			if (_mappedClass.IsInterface)
			{
				needAccesorCheck = false; // NH (the mapped class is an interface all properties can be overridden)
				proxyInterfaces.Add(_mappedClass);
			}

			foreach (Subclass subclass in persistentClass.SubclassIterator)
			{
				System.Type subclassProxy = subclass.ProxyInterface;
				System.Type subclassClass = subclass.MappedClass;
				if (subclassProxy != null && !subclassClass.Equals(subclassProxy))
				{
					if (!subclassProxy.IsInterface)
					{
						throw new MappingException("proxy must be either an interface, or the class itself: " + subclass.EntityName);
					}
					proxyInterfaces.Add(subclassProxy);
				}
			}

			/* 
			 * NH Different Implementation (for Error logging):
			 * - Check if the logger is enabled
			 * - Don't need nothing to check if the mapped-class or proxy is an interface
			 */
			if (log.IsErrorEnabled && needAccesorCheck)
			{
				LogPropertyAccessorsErrors(persistentClass);
			}
			/**********************************************************/

			MethodInfo idGetterMethod = idGetter == null ? null : idGetter.Method;
			MethodInfo idSetterMethod = idSetter == null ? null : idSetter.Method;

			MethodInfo proxyGetIdentifierMethod = idGetterMethod == null || _proxyInterface == null ? null :
				ReflectHelper.TryGetMethod(_proxyInterface, idGetterMethod);

			MethodInfo proxySetIdentifierMethod = idSetterMethod == null || _proxyInterface == null ? null :
				ReflectHelper.TryGetMethod(_proxyInterface, idSetterMethod);

			IProxyFactory pf = BuildProxyFactoryInternal(persistentClass, idGetter, idSetter);
			try
			{
				pf.PostInstantiate(EntityName, _mappedClass, proxyInterfaces, proxyGetIdentifierMethod, proxySetIdentifierMethod,
				                   persistentClass.HasEmbeddedIdentifier ? (IAbstractComponentType) persistentClass.Identifier.Type: null);
			}
			catch (HibernateException he)
			{
				log.Warn("could not create proxy factory for:" + EntityName, he);
				pf = null;
			}
			return pf;
		}

		private void LogPropertyAccessorsErrors(PersistentClass persistentClass)
		{
			if (proxyValidator == null)
			{
				return;
			}

			// This method work when Environment.UseProxyValidator is off
			System.Type clazz = persistentClass.MappedClass;
			foreach (Mapping.Property property in persistentClass.PropertyIterator)
			{
				MethodInfo method = property.GetGetter(clazz).Method;
				if (!proxyValidator.IsProxeable(method))
				{
					log.Error(
						string.Format("Getters of lazy classes cannot be final: {0}.{1}", persistentClass.MappedClass.FullName,
						              property.Name));
				}
				method = property.GetSetter(clazz).Method;
				if (!proxyValidator.IsProxeable(method))
				{
					log.Error(
						string.Format("Setters of lazy classes cannot be final: {0}.{1}", persistentClass.MappedClass.FullName,
						              property.Name));
				}
			}
		}

		protected virtual IProxyFactory BuildProxyFactoryInternal(PersistentClass @class, IGetter getter, ISetter setter)
		{
			return Cfg.Environment.BytecodeProvider.ProxyFactoryFactory.BuildProxyFactory();
		}

		public override void AfterInitialize(object entity, bool lazyPropertiesAreUnfetched, ISessionImplementor session)
		{
			if (IsInstrumented && (EntityMetamodel.HasLazyProperties || EntityMetamodel.HasUnwrapProxyForProperties))
			{
				HashSet<string> lazyProps = lazyPropertiesAreUnfetched && EntityMetamodel.HasLazyProperties ? lazyPropertyNames : null;
				//TODO: if we support multiple fetch groups, we would need
				//      to clone the set of lazy properties!
				FieldInterceptionHelper.InjectFieldInterceptor(entity, EntityName, this.MappedClass ,lazyProps, unwrapProxyPropertyNames, session);
			}
		}

		public override object[] GetPropertyValues(object entity)
		{
			if (ShouldGetAllProperties(entity) && optimizer != null && optimizer.AccessOptimizer != null)
			{
				return GetPropertyValuesWithOptimizer(entity);
			}
			else
			{
				return base.GetPropertyValues(entity);
			}
		}

		private object[] GetPropertyValuesWithOptimizer(object entity)
		{
			return optimizer.AccessOptimizer.GetPropertyValues(entity);
		}

		public override object[] GetPropertyValuesToInsert(object entity, System.Collections.IDictionary mergeMap, ISessionImplementor session)
		{
			if (ShouldGetAllProperties(entity) && optimizer != null && optimizer.AccessOptimizer != null)
			{
				return GetPropertyValuesWithOptimizer(entity);
			}
			else
			{
				return base.GetPropertyValuesToInsert(entity, mergeMap, session);
			}
		}

		public override bool HasUninitializedLazyProperties(object entity)
		{
			if (EntityMetamodel.HasLazyProperties)
			{
				IFieldInterceptor callback = FieldInterceptionHelper.ExtractFieldInterceptor(entity);
				return callback != null && !callback.IsInitialized;
			}
			else
			{
				return false;
			}
		}

		public override bool IsLifecycleImplementor
		{
			get { return islifecycleImplementor; }
		}

		public override void SetPropertyValues(object entity, object[] values)
		{
			if (!EntityMetamodel.HasLazyProperties && optimizer != null && optimizer.AccessOptimizer != null)
			{
				SetPropertyValuesWithOptimizer(entity, values);
			}
			else
			{
				base.SetPropertyValues(entity, values);
			}
		}

		private void SetPropertyValuesWithOptimizer(object entity, object[] values)
		{
			try
			{
				optimizer.AccessOptimizer.SetPropertyValues(entity, values);
			}
			catch (InvalidCastException e)
			{
				throw new PropertyAccessException(e, "Invalid Cast (check your mapping for property type mismatches);", true,
				                                  entity.GetType());
			}
		}

		public override bool IsValidatableImplementor
		{
			get { return isValidatableImplementor; }
		}

		public override EntityMode EntityMode
		{
			get { return EntityMode.Poco; }
		}

		protected void ClearOptimizerWhenUsingCustomAccessors()
		{
			if (hasCustomAccessors)
			{
				optimizer = null;
			}
		}
	}
}
