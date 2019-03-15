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
using NHibernate.Bytecode.Lightweight;

namespace NHibernate.Tuple.Entity
{

	/// <summary> An <see cref="IEntityTuplizer"/> specific to the POCO entity mode. </summary>
	public class PocoEntityTuplizer : AbstractEntityTuplizer
	{
		private static readonly INHibernateLogger log = NHibernateLogger.For(typeof(PocoEntityTuplizer));
		private readonly System.Type mappedClass;
		private readonly System.Type proxyInterface;
		private readonly bool islifecycleImplementor;
		private readonly bool isValidatableImplementor;
		[NonSerialized]
		private IReflectionOptimizer optimizer;
		private readonly IProxyValidator proxyValidator;
		private readonly IBytecodeEnhancementMetadata _enhancementMetadata;
		[NonSerialized]
		private bool isBytecodeProviderImpl; // 6.0 TODO: remove

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
				optimizer = Cfg.Environment.BytecodeProvider.GetReflectionOptimizer(mappedClass, getters, setters, idGetter, idSetter);
				isBytecodeProviderImpl = Cfg.Environment.BytecodeProvider is BytecodeProviderImpl;
			}
		}
		public PocoEntityTuplizer(EntityMetamodel entityMetamodel, PersistentClass mappedEntity)
			: base(entityMetamodel, mappedEntity)
		{
			mappedClass = mappedEntity.MappedClass;
			proxyInterface = mappedEntity.ProxyInterface;
			islifecycleImplementor = typeof(ILifecycle).IsAssignableFrom(mappedClass);
			isValidatableImplementor = typeof(IValidatable).IsAssignableFrom(mappedClass);
			_enhancementMetadata = EntityMetamodel.BytecodeEnhancementMetadata;

			SetReflectionOptimizer();

			Instantiator = BuildInstantiator(mappedEntity);

			ClearOptimizerWhenUsingCustomAccessors();

			proxyValidator = Cfg.Environment.BytecodeProvider.ProxyFactoryFactory.ProxyValidator;
		}

		public override System.Type ConcreteProxyClass
		{
			get { return proxyInterface; }
		}

		public override bool IsInstrumented => _enhancementMetadata.EnhancedForLazyLoading;

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
				log.Debug("Create Instantiator without optimizer for:{0}", persistentClass.MappedClass.FullName);
				return new PocoEntityInstantiator(EntityMetamodel, persistentClass, null, ProxyFactory);
			}
			else
			{
				log.Debug("Create Instantiator using optimizer for:{0}", persistentClass.MappedClass.FullName);
				return new PocoEntityInstantiator(EntityMetamodel, persistentClass, optimizer.InstantiationOptimizer, ProxyFactory);
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
			if (log.IsErrorEnabled() && needAccesorCheck)
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
				log.Warn(he, "could not create proxy factory for:{0}", EntityName);
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
					log.Error("Getters of lazy classes cannot be final: {0}.{1}", persistentClass.MappedClass.FullName,
						              property.Name);
				}
				method = property.GetSetter(clazz).Method;
				if (!proxyValidator.IsProxeable(method))
				{
					log.Error("Setters of lazy classes cannot be final: {0}.{1}", persistentClass.MappedClass.FullName,
						              property.Name);
				}
			}
		}

		protected virtual IProxyFactory BuildProxyFactoryInternal(PersistentClass @class, IGetter getter, ISetter setter)
		{
			return Cfg.Environment.BytecodeProvider.ProxyFactoryFactory.BuildProxyFactory();
		}

		public override void AfterInitialize(object entity, ISessionImplementor session)
		{
			if (IsInstrumented)
			{
				var interceptor = _enhancementMetadata.ExtractInterceptor(entity);
				if (interceptor == null)
				{
					interceptor = _enhancementMetadata.InjectInterceptor(entity, session);
				}
				else
				{
					interceptor.Session = session;
				}

				interceptor?.ClearDirty();
			}
		}

		public override object GetPropertyValue(object entity, int i)
		{
			if (isBytecodeProviderImpl && optimizer?.AccessOptimizer != null)
			{
				return optimizer.AccessOptimizer.GetPropertyValue(entity, i);
			}

			return base.GetPropertyValue(entity, i);
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
				return _enhancementMetadata.HasAnyUninitializedLazyProperties(entity);
			}
			else
			{
				return false;
			}
		}

		internal override ISet<string> GetUninitializedLazyProperties(object entity)
		{
			if (!EntityMetamodel.HasLazyProperties)
			{
				return CollectionHelper.EmptySet<string>();
			}

			return _enhancementMetadata.GetUninitializedLazyProperties(entity);
		}

		public override bool IsLifecycleImplementor
		{
			get { return islifecycleImplementor; }
		}

		public override void SetPropertyValue(object entity, int i, object value)
		{
			if (isBytecodeProviderImpl && optimizer?.AccessOptimizer != null)
			{
				optimizer.AccessOptimizer.SetPropertyValue(entity, i, value);
				return;
			}

			base.SetPropertyValue(entity, i, value);
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

		protected override object GetIdentifierPropertyValue(object entity)
		{
			if (isBytecodeProviderImpl && optimizer?.AccessOptimizer != null)
			{
				return optimizer.AccessOptimizer.GetSpecializedPropertyValue(entity);
			}

			return base.GetIdentifierPropertyValue(entity);
		}

		protected override void SetIdentifierPropertyValue(object entity, object value)
		{
			if (isBytecodeProviderImpl && optimizer?.AccessOptimizer != null)
			{
				optimizer.AccessOptimizer.SetSpecializedPropertyValue(entity, value);
				return;
			}

			base.SetIdentifierPropertyValue(entity, value);
		}
	}
}
