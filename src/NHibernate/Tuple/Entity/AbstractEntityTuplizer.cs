using System.Collections;
using System.Collections.Generic;

using NHibernate.Engine;
using NHibernate.Id;
using NHibernate.Intercept;
using NHibernate.Mapping;
using NHibernate.Properties;
using NHibernate.Proxy;
using NHibernate.Type;

namespace NHibernate.Tuple.Entity
{
	/// <summary> Support for tuplizers relating to entities. </summary>
	public abstract class AbstractEntityTuplizer : IEntityTuplizer
	{
		private static readonly IInternalLogger log = LoggerProvider.LoggerFor(typeof(AbstractEntityTuplizer));
		private readonly EntityMetamodel entityMetamodel;
		private readonly IGetter idGetter;
		private readonly ISetter idSetter;

		protected int propertySpan;
		protected IGetter[] getters;
		protected ISetter[] setters;
		protected bool hasCustomAccessors;
		private readonly IProxyFactory proxyFactory;
		private readonly IAbstractComponentType identifierMapperType;

		/// <summary> Constructs a new AbstractEntityTuplizer instance. </summary>
		/// <param name="entityMetamodel">The "interpreted" information relating to the mapped entity. </param>
		/// <param name="mappingInfo">The parsed "raw" mapping data relating to the given entity. </param>
		protected AbstractEntityTuplizer(EntityMetamodel entityMetamodel, PersistentClass mappingInfo)
		{
			this.entityMetamodel = entityMetamodel;

			if (!entityMetamodel.IdentifierProperty.IsVirtual)
			{
				idGetter = BuildPropertyGetter(mappingInfo.IdentifierProperty, mappingInfo);
				idSetter = BuildPropertySetter(mappingInfo.IdentifierProperty, mappingInfo);
			}
			else
			{
				idGetter = null;
				idSetter = null;
			}

			propertySpan = entityMetamodel.PropertySpan;

			getters = new IGetter[propertySpan];
			setters = new ISetter[propertySpan];

			bool foundCustomAccessor = false;
			int i = 0;
			foreach (Mapping.Property property in mappingInfo.PropertyClosureIterator)
			{
				getters[i] = BuildPropertyGetter(property, mappingInfo);
				setters[i] = BuildPropertySetter(property, mappingInfo);
				if (!property.IsBasicPropertyAccessor)
					foundCustomAccessor = true;
				i++;				
			}
			if (log.IsDebugEnabled)
			{
				log.DebugFormat("{0} accessors found for entity: {1}", foundCustomAccessor ? "Custom" : "No custom",
				                mappingInfo.EntityName);
			}
			hasCustomAccessors = foundCustomAccessor;

			//NH-1587
			//instantiator = BuildInstantiator(mappingInfo);

			if (entityMetamodel.IsLazy)
			{
				proxyFactory = BuildProxyFactory(mappingInfo, idGetter, idSetter);
				if (proxyFactory == null)
				{
					entityMetamodel.IsLazy = false;
				}
			}
			else
			{
				proxyFactory = null;
			}

			Mapping.Component mapper = mappingInfo.IdentifierMapper;
			identifierMapperType = mapper == null ? null : (IAbstractComponentType)mapper.Type;
		}

		#region IEntityTuplizer Members

		public virtual bool IsLifecycleImplementor
		{
			get { return false; }
		}

		public virtual bool IsValidatableImplementor
		{
			get { return false; }
		}

		public abstract System.Type ConcreteProxyClass { get; }

		public abstract bool IsInstrumented { get; }

		public object Instantiate(object id)
		{
			object result = Instantiator.Instantiate(id);
			if (id != null)
			{
				SetIdentifier(result, id);
			}
			return result;
		}

		public object GetIdentifier(object entity)
		{
			object id;
			if (entityMetamodel.IdentifierProperty.IsEmbedded)
			{
				id = entity;
			}
			else
			{
				if (idGetter == null)
				{
					if (identifierMapperType == null)
					{
						throw new HibernateException("The class has no identifier property: " + EntityName);
					}
					else
					{
						ComponentType copier = (ComponentType)entityMetamodel.IdentifierProperty.Type;
						id = copier.Instantiate(EntityMode);
						copier.SetPropertyValues(id, identifierMapperType.GetPropertyValues(entity, EntityMode), EntityMode);
					}
				}
				else
				{
					id = idGetter.Get(entity);
				}
			}

			return id;
		}

		public void SetIdentifier(object entity, object id)
		{
			if (entityMetamodel.IdentifierProperty.IsEmbedded)
			{
				if (entity != id)
				{
					IAbstractComponentType copier = (IAbstractComponentType)entityMetamodel.IdentifierProperty.Type;
					copier.SetPropertyValues(entity, copier.GetPropertyValues(id, EntityMode), EntityMode);
				}
			}
			else if (idSetter != null)
			{
				idSetter.Set(entity, id);
			}
		}

		public void ResetIdentifier(object entity, object currentId, object currentVersion)
		{
			if (!(entityMetamodel.IdentifierProperty.IdentifierGenerator is Assigned))
			{
				//reset the id
				object result = entityMetamodel.IdentifierProperty.UnsavedValue.GetDefaultValue(currentId);
				SetIdentifier(entity, result);
				//reset the version
				VersionProperty versionProperty = entityMetamodel.VersionProperty;
				if (entityMetamodel.IsVersioned)
				{
					SetPropertyValue(entity, entityMetamodel.VersionPropertyIndex, versionProperty.UnsavedValue.GetDefaultValue(currentVersion));
				}
			}
		}

		public object GetVersion(object entity)
		{
			if (!entityMetamodel.IsVersioned)
				return null;
			return getters[entityMetamodel.VersionPropertyIndex].Get(entity);
		}

		public void SetPropertyValue(object entity, int i, object value)
		{
			setters[i].Set(entity, value);
		}

		public void SetPropertyValue(object entity, string propertyName, object value)
		{
			setters[entityMetamodel.GetPropertyIndex(propertyName)].Set(entity, value);
		}

		public virtual object[] GetPropertyValuesToInsert(object entity, IDictionary mergeMap, ISessionImplementor session)
		{
			int span = entityMetamodel.PropertySpan;
			object[] result = new object[span];

			for (int j = 0; j < span; j++)
			{
				result[j] = getters[j].GetForInsert(entity, mergeMap, session);
			}
			return result;
		}

		public object GetPropertyValue(object entity, string propertyPath)
		{
			int loc = propertyPath.IndexOf('.');
			string basePropertyName = loc > 0 ? propertyPath.Substring(0, (loc) - (0)) : propertyPath;

			int index = entityMetamodel.GetPropertyIndex(basePropertyName);
			object baseValue = GetPropertyValue(entity, index);
			if (loc > 0)
			{
				ComponentType type = (ComponentType)entityMetamodel.PropertyTypes[index];
				return GetComponentValue(type, baseValue, propertyPath.Substring(loc + 1));
			}
			else
			{
				return baseValue;
			}
		}

		public virtual void AfterInitialize(object entity, bool lazyPropertiesAreUnfetched, ISessionImplementor session)
		{
		}

		public bool HasProxy
		{
			get { return entityMetamodel.IsLazy; }
		}

		public object CreateProxy(object id, ISessionImplementor session)
		{
			return ProxyFactory.GetProxy(id, session);
		}

		public virtual bool HasUninitializedLazyProperties(object entity)
		{
			// the default is to simply not lazy fetch properties for now...
			return false;
		}

		#endregion

		#region ITuplizer Members

		public abstract System.Type MappedClass { get; }

		public virtual object[] GetPropertyValues(object entity)
		{
			bool getAll = ShouldGetAllProperties(entity);
			int span = entityMetamodel.PropertySpan;
			object[] result = new object[span];

			for (int j = 0; j < span; j++)
			{
				StandardProperty property = entityMetamodel.Properties[j];
				if (getAll || !property.IsLazy)
				{
					result[j] = getters[j].Get(entity);
				}
				else
				{
					result[j] = LazyPropertyInitializer.UnfetchedProperty;
				}
			}
			return result;
		}

		public virtual void SetPropertyValues(object entity, object[] values)
		{
			bool setAll = !entityMetamodel.HasLazyProperties;

			for (int j = 0; j < entityMetamodel.PropertySpan; j++)
			{
				if (setAll || !Equals(LazyPropertyInitializer.UnfetchedProperty, values[j]))
				{
					setters[j].Set(entity, values[j]);
				}
			}
		}

		public virtual object GetPropertyValue(object entity, int i)
		{
			return getters[i].Get(entity);
		}

		public object Instantiate()
		{
			return Instantiate(null);
		}

		public bool IsInstance(object obj)
		{
			return Instantiator.IsInstance(obj);
		}

		#endregion

		/// <summary> Return the entity-mode handled by this tuplizer instance. </summary>
		public abstract EntityMode EntityMode { get;}

		protected virtual IInstantiator Instantiator { get; set; }

		/// <summary>Retrieves the defined entity-name for the tuplized entity. </summary>
		protected virtual string EntityName
		{
			get { return entityMetamodel.Name; }
		}

		/// <summary> 
		/// Retrieves the defined entity-names for any subclasses defined for this entity. 
		/// </summary>
		protected virtual ISet<string> SubclassEntityNames
		{
			get { return entityMetamodel.SubclassEntityNames; }
		}

		/// <summary> Build an appropriate Getter for the given property. </summary>
		/// <param name="mappedProperty">The property to be accessed via the built Getter. </param>
		/// <param name="mappedEntity">The entity information regarding the mapped entity owning this property. </param>
		/// <returns> An appropriate Getter instance. </returns>
		protected abstract IGetter BuildPropertyGetter(Mapping.Property mappedProperty, PersistentClass mappedEntity);

		/// <summary> Build an appropriate Setter for the given property. </summary>
		/// <param name="mappedProperty">The property to be accessed via the built Setter. </param>
		/// <param name="mappedEntity">The entity information regarding the mapped entity owning this property. </param>
		/// <returns> An appropriate Setter instance. </returns>
		protected abstract ISetter BuildPropertySetter(Mapping.Property mappedProperty, PersistentClass mappedEntity);

		/// <summary> Build an appropriate Instantiator for the given mapped entity. </summary>
		/// <param name="mappingInfo">The mapping information regarding the mapped entity. </param>
		/// <returns> An appropriate Instantiator instance. </returns>
		protected abstract IInstantiator BuildInstantiator(PersistentClass mappingInfo);

		/// <summary> Build an appropriate ProxyFactory for the given mapped entity. </summary>
		/// <param name="mappingInfo">The mapping information regarding the mapped entity. </param>
		/// <param name="idGetter">The constructed Getter relating to the entity's id property. </param>
		/// <param name="idSetter">The constructed Setter relating to the entity's id property. </param>
		/// <returns> An appropriate ProxyFactory instance. </returns>
		protected abstract IProxyFactory BuildProxyFactory(PersistentClass mappingInfo, IGetter idGetter, ISetter idSetter);

		/// <summary> Extract a component property value. </summary>
		/// <param name="type">The component property types. </param>
		/// <param name="component">The component instance itself. </param>
		/// <param name="propertyPath">The property path for the property to be extracted. </param>
		/// <returns> The property value extracted. </returns>
		protected virtual object GetComponentValue(ComponentType type, object component, string propertyPath)
		{
			int loc = propertyPath.IndexOf('.');
			string basePropertyName = loc > 0 ? propertyPath.Substring(0, (loc) - (0)) : propertyPath;

			string[] propertyNames = type.PropertyNames;
			int index = 0;
			for (; index < propertyNames.Length; index++)
			{
				if (basePropertyName.Equals(propertyNames[index]))
					break;
			}
			if (index == propertyNames.Length)
			{
				throw new MappingException("component property not found: " + basePropertyName);
			}

			object baseValue = type.GetPropertyValue(component, index, EntityMode);

			if (loc > 0)
			{
				ComponentType subtype = (ComponentType)type.Subtypes[index];
				return GetComponentValue(subtype, baseValue, propertyPath.Substring(loc + 1));
			}
			else
			{
				return baseValue;
			}
		}

		protected virtual IProxyFactory ProxyFactory
		{
			get { return proxyFactory; }
		}

		protected virtual bool ShouldGetAllProperties(object entity)
		{
			return !HasUninitializedLazyProperties(entity);
		}

		protected EntityMetamodel EntityMetamodel
		{
			get { return entityMetamodel; }
		}

	}
}
