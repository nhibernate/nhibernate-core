using System;
using System.Reflection;
using NHibernate.Engine;
using NHibernate.Id;
using NHibernate.Mapping;
using NHibernate.Properties;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Tuple
{
	/// <summary>
	/// Responsible for generation of runtime metamodel <see cref="NHibernate.Tuple.Property" /> representations.
	/// Makes distinction between identifier, version, and other (standard) properties.
	/// </summary>
	/// <remarks>
	/// Author: Steve Ebersole
	/// </remarks>
	public class PropertyFactory
	{
		/// <summary>
		/// Generates an IdentifierProperty representation of the for a given entity mapping.
		/// </summary>
		/// <param name="mappedEntity">The mapping definition of the entity.</param>
		/// <param name="generator">The identifier value generator to use for this identifier.</param>
		/// <returns>The appropriate IdentifierProperty definition.</returns>
		public static IdentifierProperty BuildIdentifierProperty(PersistentClass mappedEntity, IIdentifierGenerator generator)
		{
			string mappedUnsavedValue = mappedEntity.Identifier.NullValue;
			IType type = mappedEntity.Identifier.Type;
			Mapping.Property property = mappedEntity.IdentifierProperty;

			IdentifierValue unsavedValue = UnsavedValueFactory.GetUnsavedIdentifierValue(mappedUnsavedValue, GetGetter(property), type, GetConstructor(mappedEntity));

			if (property == null)
			{
				// this is a virtual id property...
				return new IdentifierProperty(type, mappedEntity.HasEmbeddedIdentifier,
					mappedEntity.HasIdentifierMapper, unsavedValue, generator);
			}
			else
			{
				return new IdentifierProperty(property.Name, property.NodeName, type, mappedEntity.HasEmbeddedIdentifier, unsavedValue, generator);
			}
		}

		/// <summary>
		/// Generates a VersionProperty representation for an entity mapping given its
		/// version mapping Property.
		/// </summary>
		/// <param name="property">The version mapping Property.</param>
		/// <param name="lazyAvailable">Is property lazy loading currently available for parent class.</param>
		/// <returns>The appropriate VersionProperty definition.</returns>
		public static VersionProperty BuildVersionProperty(Mapping.Property property, bool lazyAvailable)
		{
			String mappedUnsavedValue = ((IKeyValue) property.Value).NullValue;

			VersionValue unsavedValue = UnsavedValueFactory.GetUnsavedVersionValue(
				mappedUnsavedValue,
				GetGetter(property),
				(IVersionType) property.Type,
				GetConstructor(property.PersistentClass)
				);

			bool lazy = IsLazyProperty(property, lazyAvailable);

			return new VersionProperty(
				property.Name,
				property.NodeName,
				property.Value.Type,
				lazy,
				property.IsInsertable,
				property.IsUpdateable,
				property.Generation == PropertyGeneration.Insert || property.Generation == PropertyGeneration.Always,
				property.Generation == PropertyGeneration.Always,
				property.IsOptional,
				property.IsUpdateable && !lazy,
				property.IsOptimisticLocked,
				property.CascadeStyle,
				unsavedValue,
				DetermineInsertValueGenerationType(property),
				DetermineUpdateValueGenerationType(property));
		}

		private static bool IsLazyProperty(Mapping.Property property, bool lazyAvailable)
		{
			return lazyAvailable && property.IsLazy && (!property.IsEntityRelation || property.UnwrapProxy);
		}

		/// <summary>
		/// Generate a "standard" (i.e., non-identifier and non-version) based on the given
		/// mapped property.
		/// </summary>
		/// <param name="property">The mapped property.</param>
		/// <param name="lazyAvailable">Is property lazy loading currently available.</param>
		/// <returns>The appropriate StandardProperty definition.</returns>
		public static StandardProperty BuildStandardProperty(Mapping.Property property, bool lazyAvailable)
		{
			IType type = property.Value.Type;

			// we need to dirty check collections, since they can cause an owner
			// version number increment

			// we need to dirty check many-to-ones with not-found="ignore" in order 
			// to update the cache (not the database), since in this case a null
			// entity reference can lose information

			bool alwaysDirtyCheck = type.IsAssociationType &&
			                        ((IAssociationType) type).IsAlwaysDirtyChecked;
      
			return new StandardProperty(
				property.Name,
				property.NodeName,
				type,
				IsLazyProperty(property, lazyAvailable),
				property.IsInsertable,
				property.IsUpdateable,
				IsInsertGenerated(property),
				IsUpdateGenerated(property),
				property.IsOptional,
				alwaysDirtyCheck || property.IsUpdateable,
				property.IsOptimisticLocked,
				property.CascadeStyle,
				property.Value.FetchMode,
				DetermineInsertValueGenerationType(property),
				DetermineUpdateValueGenerationType(property));
		}

		private static bool IsUpdateGenerated(Mapping.Property property)
		{
			return property.Generation == PropertyGeneration.Always;
		}

		private static bool IsInsertGenerated(Mapping.Property property)
		{
			return property.Generation == PropertyGeneration.Insert || property.Generation == PropertyGeneration.Always;
		}

		private static ValueInclusion DetermineInsertValueGenerationType(Mapping.Property mappingProperty)
		{
			if (IsInsertGenerated(mappingProperty))
			{
				return ValueInclusion.Full;
			}
			else if (mappingProperty.Value is Mapping.Component)
			{
				if (HasPartialInsertComponentGeneration((Mapping.Component) mappingProperty.Value))
				{
					return ValueInclusion.Partial;
				}
			}
			return ValueInclusion.None;
		}

		private static bool HasPartialInsertComponentGeneration(Mapping.Component component)
		{
			foreach (Mapping.Property prop in component.PropertyIterator)
			{
				if (prop.Generation == PropertyGeneration.Always || prop.Generation == PropertyGeneration.Insert)
				{
					return true;
				}
				else if (prop.Value is Mapping.Component)
				{
					if (HasPartialInsertComponentGeneration((Mapping.Component) prop.Value))
					{
						return true;
					}
				}
			}
			return false;
		}

		private static ValueInclusion DetermineUpdateValueGenerationType(Mapping.Property mappingProperty)
		{
			if (IsUpdateGenerated(mappingProperty))
			{
				return ValueInclusion.Full;
			}
			else if (mappingProperty.Value is Mapping.Component)
			{
				if (HasPartialUpdateComponentGeneration((Mapping.Component) mappingProperty.Value))
				{
					return ValueInclusion.Partial;
				}
			}
			return ValueInclusion.None;
		}

		private static bool HasPartialUpdateComponentGeneration(Mapping.Component component)
		{
			foreach (Mapping.Property prop in component.PropertyIterator)
			{
				if (prop.Generation == PropertyGeneration.Always)
				{
					return true;
				}
				else if (prop.Value is Mapping.Component)
				{
					if (HasPartialUpdateComponentGeneration((Mapping.Component) prop.Value))
					{
						return true;
					}
				}
			}
			return false;
		}

		private static ConstructorInfo GetConstructor(PersistentClass persistentClass)
		{
			if (persistentClass == null || !persistentClass.HasPocoRepresentation)
			{
				return null;
			}

			try
			{
				return ReflectHelper.GetDefaultConstructor(persistentClass.MappedClass);
			}
			catch
			{
				return null;
			}
		}

		private static IGetter GetGetter(Mapping.Property mappingProperty)
		{
			if (mappingProperty == null || !mappingProperty.PersistentClass.HasPocoRepresentation)
			{
				return null;
			}

			IPropertyAccessor pa = PropertyAccessorFactory.GetPropertyAccessor(mappingProperty, EntityMode.Poco);
			return pa.GetGetter(mappingProperty.PersistentClass.MappedClass, mappingProperty.Name);
		}
	}
}