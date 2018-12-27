using System;
using System.Collections.Generic;
using NHibernate.Bytecode;
using NHibernate.Engine;
using NHibernate.Intercept;
using NHibernate.Mapping;
using NHibernate.Util;

namespace NHibernate.Tuple.Entity
{
	/// <summary>
	/// Author: Steve Ebersole
	/// </summary>
	public class BytecodeEnhancementMetadataPocoImpl : IBytecodeEnhancementMetadata
	{
		private readonly System.Type _entityType;

		public static IBytecodeEnhancementMetadata From(
			PersistentClass persistentClass,
			ICollection<LazyPropertyDescriptor> lazyPropertyDescriptors,
			ICollection<UnwrapProxyPropertyDescriptor> unwrapProxyPropertyDescriptors)
		{
			var mappedClass = persistentClass.MappedClass;
			var enhancedForLazyLoading = lazyPropertyDescriptors?.Count > 0 || unwrapProxyPropertyDescriptors?.Count > 0;

			// We have to check all subclasses if any of them is enhanced for lazy loading as the
			// root class will be enhanced when any of the subclasses is enhanced, even if it
			// does not have any lazy properties.
			// If we do not check the subclasses, where the root-entity has no lazy properties
			// we will eager-load/double-load those properties (NH2488).
			if (!enhancedForLazyLoading)
			{
				foreach (var persistentSubclass in persistentClass.SubclassClosureIterator)
				{
					enhancedForLazyLoading |= IsEnhancedForLazyLoading(persistentSubclass);
					if (enhancedForLazyLoading)
					{
						break;
					}
				}
			}

			var lazyPropertiesMetadata = enhancedForLazyLoading
				? LazyPropertiesMetadata.From(persistentClass.EntityName, lazyPropertyDescriptors)
				: LazyPropertiesMetadata.NonEnhanced(persistentClass.EntityName);

			var unwrapProxyPropertiesMetadata = enhancedForLazyLoading
				? UnwrapProxyPropertiesMetadata.From(persistentClass.EntityName, unwrapProxyPropertyDescriptors)
				: UnwrapProxyPropertiesMetadata.NonEnhanced(persistentClass.EntityName);

			return new BytecodeEnhancementMetadataPocoImpl(
				persistentClass.EntityName,
				mappedClass,
				enhancedForLazyLoading,
				lazyPropertiesMetadata,
				unwrapProxyPropertiesMetadata
			);
		}

		/// <summary>
		/// Check for a <see cref="PersistentClass"/> if is enhanced for lazy loading.
		/// NOTE: The logic was taken from <see cref="EntityMetamodel"/>.
		/// </summary>
		/// <param name="persistentClass">The persistent class to check.</param>
		/// <returns>Whether the persistent class is enhanced for lazy loading or not.</returns>
		private static bool IsEnhancedForLazyLoading(PersistentClass persistentClass)
		{
			var lazyAvailable = persistentClass.HasPocoRepresentation
			                     && FieldInterceptionHelper.IsInstrumented(persistentClass.MappedClass);

			// NH: WARNING if we have to disable lazy/unproxy properties we have to do it in the whole process.
			var lazy = persistentClass.IsLazy && (!persistentClass.HasPocoRepresentation ||
			                                      !ReflectHelper.IsFinalClass(persistentClass.ProxyInterface));
			lazyAvailable &= lazy; // <== Disable lazy properties if the class is marked with lazy=false
			if (!lazyAvailable)
			{
				return false;
			}

			foreach (var prop in persistentClass.PropertyIterator)
			{
				// NH: A lazy property is a simple property marked with lazy=true
				var islazyProperty = prop.IsLazy && (!prop.IsEntityRelation || prop.UnwrapProxy);
				// NH: A Relation (in this case many-to-one or one-to-one) marked as "no-proxy"
				var isUnwrapProxy = prop.UnwrapProxy;

				if (islazyProperty || isUnwrapProxy)
				{
					return true;
				}
			}

			return false;
		}

		public BytecodeEnhancementMetadataPocoImpl(
			string entityName,
			System.Type entityType,
			bool enhancedForLazyLoading,
			LazyPropertiesMetadata lazyPropertiesMetadata,
			UnwrapProxyPropertiesMetadata unwrapProxyPropertiesMetadata)
		{
			EntityName = entityName;
			_entityType = entityType;
			EnhancedForLazyLoading = enhancedForLazyLoading;
			LazyPropertiesMetadata = lazyPropertiesMetadata;
			UnwrapProxyPropertiesMetadata = unwrapProxyPropertiesMetadata;
		}

		/// <inheritdoc />
		public string EntityName { get; }

		/// <inheritdoc />
		public bool EnhancedForLazyLoading { get; }

		/// <inheritdoc />
		public LazyPropertiesMetadata LazyPropertiesMetadata { get; }

		/// <inheritdoc />
		public UnwrapProxyPropertiesMetadata UnwrapProxyPropertiesMetadata { get; }

		/// <inheritdoc />
		public IFieldInterceptor InjectInterceptor(object entity, bool lazyPropertiesAreUnfetched, ISessionImplementor session)
		{
			if (!EnhancedForLazyLoading)
			{
				throw new NotInstrumentedException($"Entity class [{_entityType}] is not enhanced for lazy loading");
			}

			if (!(entity is IFieldInterceptorAccessor fieldInterceptorAccessor))
			{
				return null; // Can happen when a saved entity is refreshed within the same session NH2860
			}

			if (entity.GetType().BaseType != _entityType)
			{
				throw new ArgumentException(
					$"Passed entity instance [{entity}] is not of expected type [{EntityName}]");
			}

			var fieldInterceptorImpl = new DefaultFieldInterceptor(
				session,
				LazyPropertiesMetadata.HasLazyProperties && lazyPropertiesAreUnfetched
					? new HashSet<string>(LazyPropertiesMetadata.LazyPropertyNames)
					: null,
				UnwrapProxyPropertiesMetadata.UnwrapProxyPropertyNames,
				EntityName,
				_entityType);
			fieldInterceptorAccessor.FieldInterceptor = fieldInterceptorImpl;

			return fieldInterceptorImpl;
		}

		/// <inheritdoc />
		public IFieldInterceptor ExtractInterceptor(object entity)
		{
			if (!EnhancedForLazyLoading)
			{
				throw new NotInstrumentedException($"Entity class [{_entityType}] is not enhanced for lazy loading");
			}

			if (!(entity is IFieldInterceptorAccessor fieldInterceptorAccessor))
			{
				return null;
			}

			var interceptor = fieldInterceptorAccessor.FieldInterceptor;
			if (interceptor == null)
			{
				return null;
			}

			if (_entityType != interceptor.MappedClass)
			{
				throw new ArgumentException(
					$"Passed entity instance [{entity}] is not of expected type [{EntityName}]");
			}

			return fieldInterceptorAccessor.FieldInterceptor;
		}

		/// <inheritdoc />
		public ISet<string> GetUninitializedLazyProperties(object entity)
		{
			var interceptor = LazyPropertiesMetadata.HasLazyProperties ? ExtractInterceptor(entity) : null;
			return interceptor?.GetUninitializedFields() ?? CollectionHelper.EmptySet<string>();
		}

		/// <inheritdoc />
		public ISet<string> GetUninitializedLazyProperties(object[] entityState)
		{
			if (!LazyPropertiesMetadata.HasLazyProperties)
			{
				return CollectionHelper.EmptySet<string>();
			}

			var uninitializedProperties = new HashSet<string>();
			foreach (var propertyDescriptor in LazyPropertiesMetadata.LazyPropertyDescriptors)
			{
				if (entityState[propertyDescriptor.PropertyIndex] == LazyPropertyInitializer.UnfetchedProperty)
				{
					uninitializedProperties.Add(propertyDescriptor.Name);
				}
			}

			return uninitializedProperties;
		}
	}
}
