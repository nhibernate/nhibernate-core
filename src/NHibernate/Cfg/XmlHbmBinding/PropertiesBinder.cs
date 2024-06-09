using System.Collections.Generic;
using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping;
using System;
using NHibernate.Util;

namespace NHibernate.Cfg.XmlHbmBinding
{
	public class PropertiesBinder : ClassBinder
	{
		private readonly PersistentClass persistentClass;
		private readonly Component component;
		private readonly string entityName;
		private readonly System.Type mappedClass;
		private readonly bool componetDefaultNullable;
		private readonly string propertyBasePath;

		//Since v5.2
		[Obsolete("Please use constructor without a dialect parameter.")]
		public PropertiesBinder(Mappings mappings, PersistentClass persistentClass, Dialect.Dialect dialect)
			: this(mappings, persistentClass)
		{
		}
		
		//Since v5.2
		[Obsolete("Please use constructor without dialect parameter")]
		public PropertiesBinder(Mappings mappings, Component component, string className, string path, bool isNullable, Dialect.Dialect dialect)
			: this(mappings, component, className, path, isNullable)
		{
		}

		public PropertiesBinder(Mappings mappings, PersistentClass persistentClass)
			: base(mappings)
		{
			this.persistentClass = persistentClass;
			entityName = persistentClass.EntityName;
			propertyBasePath = entityName;
			mappedClass = persistentClass.MappedClass;
			componetDefaultNullable = true;
			component = null;
		}

		public PropertiesBinder(Mappings mappings, Component component, string className, string path, bool isNullable)
			: base(mappings)
		{
			persistentClass = component.Owner;
			this.component = component;
			entityName = className;
			mappedClass = component.ComponentClass;
			propertyBasePath = path;
			componetDefaultNullable = isNullable;
		}

		public void Bind(IEnumerable<IEntityPropertyMapping> properties, IDictionary<string, MetaAttribute> inheritedMetas)
		{
			Bind(properties, inheritedMetas, p => { });
		}

		public void Bind(IEnumerable<IEntityPropertyMapping> properties, IDictionary<string, MetaAttribute> inheritedMetas, Action<Property> modifier)
		{
			Bind(properties, persistentClass.Table, inheritedMetas, modifier, p => persistentClass.AddProperty(p));
		}

		public void Bind(IEnumerable<IEntityPropertyMapping> properties, Table table, IDictionary<string, MetaAttribute> inheritedMetas, Action<Property> modifier, Action<Property> addToModelAction)
		{
			if (table == null)
			{
				throw new ArgumentNullException("table");
			}
			if (modifier == null)
			{
				throw new ArgumentNullException("modifier");
			}
			if (addToModelAction == null)
			{
				throw new ArgumentNullException("addToModelAction");
			}

			foreach (var entityPropertyMapping in properties)
			{
				Property property = null;

				string propertyName = entityPropertyMapping.Name;

				if (entityPropertyMapping is HbmProperty propertyMapping)
				{
					var value = new SimpleValue(table);
					new ValuePropertyBinder(value, Mappings).BindSimpleValue(propertyMapping, propertyName, true);
					property = CreateProperty(entityPropertyMapping, mappedClass, value, inheritedMetas);
					BindValueProperty(propertyMapping, property);
				}
				else if (entityPropertyMapping is ICollectionPropertiesMapping collectionMapping)
				{
					var collectionBinder = new CollectionBinder(Mappings);
					string propertyPath = propertyName == null ? null : StringHelper.Qualify(propertyBasePath, propertyName);

					Mapping.Collection collection = collectionBinder.Create(collectionMapping, entityName, propertyPath, persistentClass,
																												mappedClass, inheritedMetas);

					mappings.AddCollection(collection);

					property = CreateProperty(collectionMapping, mappedClass, collection, inheritedMetas);
					BindCollectionProperty(collectionMapping, property);
				}
				else if (entityPropertyMapping is HbmProperties propertiesMapping)
				{
					var subpath = propertyName == null ? null : StringHelper.Qualify(propertyBasePath, propertyName);
					var value = CreateNewComponent(table);
					BindComponent(propertiesMapping, value, null, entityName, subpath, componetDefaultNullable, inheritedMetas);
					property = CreateProperty(entityPropertyMapping, mappedClass, value, inheritedMetas);
					BindComponentProperty(propertiesMapping, property, value);
				}
				else if (entityPropertyMapping is HbmManyToOne manyToOneMapping)
				{
					var value = new ManyToOne(table);
					BindManyToOne(manyToOneMapping, value, propertyName, true);
					property = CreateProperty(entityPropertyMapping, mappedClass, value, inheritedMetas);
					BindManyToOneProperty(manyToOneMapping, property);
				}
				else if (entityPropertyMapping is HbmComponent componentMapping)
				{
					string subpath = propertyName == null ? null : StringHelper.Qualify(propertyBasePath, propertyName);
					var value = CreateNewComponent(table);
					// NH: Modified from H2.1 to allow specifying the type explicitly using class attribute
					System.Type reflectedClass = mappedClass == null ? null : GetPropertyType(componentMapping.Class, mappedClass, propertyName, componentMapping.Access);
					BindComponent(componentMapping, value, reflectedClass, entityName, subpath, componetDefaultNullable, inheritedMetas);
					property = CreateProperty(entityPropertyMapping, mappedClass, value, inheritedMetas);
					BindComponentProperty(componentMapping, property, value);
				}
				else if (entityPropertyMapping is HbmOneToOne oneToOneMapping)
				{
					var value = new OneToOne(table, persistentClass);
					BindOneToOne(oneToOneMapping, value);
					property = CreateProperty(entityPropertyMapping, mappedClass, value, inheritedMetas);
					BindOneToOneProperty(oneToOneMapping, property);
				}
				else if (entityPropertyMapping is HbmDynamicComponent dynamicComponentMapping)
				{
					string subpath = propertyName == null ? null : StringHelper.Qualify(propertyBasePath, propertyName);
					var value = CreateNewComponent(table);
					// NH: Modified from H2.1 to allow specifying the type explicitly using class attribute
					System.Type reflectedClass = mappedClass == null ? null : GetPropertyType(dynamicComponentMapping.Class, mappedClass, propertyName, dynamicComponentMapping.Access);
					BindComponent(dynamicComponentMapping, value, reflectedClass, entityName, subpath, componetDefaultNullable, inheritedMetas);
					property = CreateProperty(entityPropertyMapping, mappedClass, value, inheritedMetas);
					BindComponentProperty(dynamicComponentMapping, property, value);
				}
				else if (entityPropertyMapping is HbmAny anyMapping)
				{
					var value = new Any(table);
					BindAny(anyMapping, value, true);
					property = CreateProperty(entityPropertyMapping, mappedClass, value, inheritedMetas);
					BindAnyProperty(anyMapping, property);
				}
				else if (entityPropertyMapping is HbmNestedCompositeElement nestedCompositeElementMapping)
				{
					if (component == null)
					{
						throw new AssertionFailure("Nested Composite Element without a owner component.");
					}
					string subpath = propertyName == null ? null : StringHelper.Qualify(propertyBasePath, propertyName);
					var value = CreateNewComponent(table);
					// NH: Modified from H2.1 to allow specifying the type explicitly using class attribute
					System.Type reflectedClass = mappedClass == null ? null : GetPropertyType(nestedCompositeElementMapping.Class, mappedClass, propertyName, nestedCompositeElementMapping.access);
					BindComponent(nestedCompositeElementMapping, value, reflectedClass, entityName, subpath, componetDefaultNullable, inheritedMetas);
					property = CreateProperty(entityPropertyMapping, mappedClass, value, inheritedMetas);
				}
				else if (entityPropertyMapping is HbmKeyProperty keyPropertyMapping)
				{
					var value = new SimpleValue(table);
					new ValuePropertyBinder(value, Mappings).BindSimpleValue(keyPropertyMapping, propertyName, componetDefaultNullable);
					property = CreateProperty(entityPropertyMapping, mappedClass, value, inheritedMetas);
				}
				else if (entityPropertyMapping is HbmKeyManyToOne keyManyToOneMapping)
				{
					var value = new ManyToOne(table);
					BindKeyManyToOne(keyManyToOneMapping, value, propertyName, componetDefaultNullable);
					property = CreateProperty(entityPropertyMapping, mappedClass, value, inheritedMetas);
				}

				if (property != null)
				{
					modifier(property);
					property.LogMapped(log);
					addToModelAction(property);
				}
			}			
		}

		private Component CreateNewComponent(Table table)
		{
			// Manage nested components
			return component != null ? new Component(component) : new Component(table, persistentClass);
		}

		private System.Type GetPropertyType(string classMapping, System.Type containingType, string propertyName, string propertyAccess)
		{
			if(!string.IsNullOrEmpty(classMapping))
				return ClassForNameChecked(classMapping, mappings, "could not find class: {0}");
			else if (containingType == null)
				return null;

			string access = GetPropertyAccessorName(propertyAccess);

			return ReflectHelper.ReflectedPropertyClass(containingType, propertyName, access);
		}

		private void BindKeyManyToOne(HbmKeyManyToOne keyManyToOneMapping, ManyToOne model, string defaultColumnName, bool isNullable)
		{
			new ValuePropertyBinder(model, Mappings).BindSimpleValue(keyManyToOneMapping, defaultColumnName, isNullable);
			InitLaziness(keyManyToOneMapping.Lazy, model, true);

			model.ReferencedEntityName = GetEntityName(keyManyToOneMapping, mappings);
			model.IsIgnoreNotFound = keyManyToOneMapping.NotFoundMode == HbmNotFoundMode.Ignore;

			BindForeignKey(keyManyToOneMapping.foreignkey, model);
		}

		private void BindManyToOne(HbmManyToOne manyToOneMapping, ManyToOne model, string defaultColumnName, bool isNullable)
		{
			new ValuePropertyBinder(model, Mappings).BindSimpleValue(manyToOneMapping, defaultColumnName, isNullable);
			InitOuterJoinFetchSetting(manyToOneMapping, model);
			InitLaziness(manyToOneMapping.Lazy, model, true);

			var ukName = !string.IsNullOrEmpty(manyToOneMapping.propertyref) ? manyToOneMapping.propertyref : null;
			if (ukName != null)
				model.ReferencedPropertyName = ukName;

			model.ReferencedEntityName = GetEntityName(manyToOneMapping, mappings);
			model.IsIgnoreNotFound = manyToOneMapping.NotFoundMode == HbmNotFoundMode.Ignore;
			model.PropertyName = manyToOneMapping.Name;

			if (ukName != null && !model.IsIgnoreNotFound)
			{
				AddManyToOneSecondPass(model);
			}
			
			if (manyToOneMapping.unique)
			{
				model.IsLogicalOneToOne = true;
			}

			BindForeignKey(manyToOneMapping.foreignkey, model);
		}

		private void InitOuterJoinFetchSetting(HbmManyToOne manyToOneMapping, ManyToOne model)
		{
			FetchMode fetchStyle = !manyToOneMapping.fetchSpecified
															? (!manyToOneMapping.outerjoinSpecified ? FetchMode.Default : GetFetchStyle(manyToOneMapping.outerjoin))
															: GetFetchStyle(manyToOneMapping.fetch);

			model.FetchMode = fetchStyle;
		}

		private void AddManyToOneSecondPass(ManyToOne manyToOne)
		{
			mappings.AddSecondPass(manyToOne.CreatePropertyRefConstraints);
		}

		private void BindValueProperty(HbmProperty propertyMapping, Property property)
		{
			property.IsUpdateable = propertyMapping.updateSpecified ? propertyMapping.update:true;
			property.IsInsertable = propertyMapping.insertSpecified ? propertyMapping.insert:true;
			PropertyGeneration generation;
			switch (propertyMapping.generated)
			{
				case HbmPropertyGeneration.Never:
					generation = PropertyGeneration.Never;
					break;
				case HbmPropertyGeneration.Insert:
					generation = PropertyGeneration.Insert;
					break;
				case HbmPropertyGeneration.Always:
					generation = PropertyGeneration.Always;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			property.Generation = generation;

			if (generation == PropertyGeneration.Always || generation == PropertyGeneration.Insert)
			{
				// generated properties can *never* be insertable...
				if (propertyMapping.insertSpecified && property.IsInsertable)
				{
					// the user specifically supplied insert="true",
					// which constitutes an illegal combo
					throw new MappingException("cannot specify both insert=\"true\" and generated=\"" + generation
					                           + "\" for property: " + propertyMapping.Name);
				}
				property.IsInsertable = false;
			}
			if (generation == PropertyGeneration.Always)
			{
				// properties generated on update can never be updateable...
				if (propertyMapping.updateSpecified && property.IsUpdateable)
				{
					// the user specifically supplied update="true",
					// which constitutes an illegal combo
					throw new MappingException("cannot specify both update=\"true\" and generated=\"" + generation
					                           + "\" for property: " + propertyMapping.Name);
				}
				property.IsUpdateable = false;
			}
		}

		private void BindAnyProperty(HbmAny anyMapping, Property property)
		{
			property.Cascade = anyMapping.cascade ?? mappings.DefaultCascade;
			property.IsUpdateable = anyMapping.update;
			property.IsInsertable = anyMapping.insert;
		}

		private void BindOneToOneProperty(HbmOneToOne oneToOneMapping, Property property)
		{
			property.Cascade = oneToOneMapping.cascade ?? mappings.DefaultCascade;
			property.UnwrapProxy = oneToOneMapping.Lazy == HbmLaziness.NoProxy;
			var toOne = property.Value as ToOne;
			if (toOne != null)
			{
				string propertyRef = toOne.ReferencedPropertyName;
				if (propertyRef != null)
					mappings.AddUniquePropertyReference(toOne.ReferencedEntityName, propertyRef);
				toOne.CreateForeignKey();
			}
		}

		private void BindComponentProperty(HbmDynamicComponent dynamicComponentMapping, Property property, Component model)
		{
			property.IsUpdateable = dynamicComponentMapping.update;
			property.IsInsertable = dynamicComponentMapping.insert;
			if (dynamicComponentMapping.unique)
			{
				model.Owner.Table.CreateUniqueKey(model.ColumnIterator.OfType<Column>().ToList());
			}
		}

		private void BindComponentProperty(HbmProperties propertiesMapping, Property property, Component model)
		{
			property.IsUpdateable = propertiesMapping.update;
			property.IsInsertable = propertiesMapping.insert;
			if (propertiesMapping.unique)
			{
				model.Owner.Table.CreateUniqueKey(model.ColumnIterator.OfType<Column>().ToList());
			}
		}

		private void BindComponentProperty(HbmComponent componentMapping, Property property, Component model)
		{
			property.IsUpdateable = componentMapping.update;
			property.IsInsertable = componentMapping.insert;
			if (componentMapping.unique)
			{
				model.Owner.Table.CreateUniqueKey(model.ColumnIterator.OfType<Column>().ToList());
			}
			HbmTuplizer[] tuplizers = componentMapping.tuplizer;
			if (tuplizers != null)
			{
				foreach (var tuplizer in tuplizers)
				{
					var mode = tuplizer.entitymode.ToEntityMode();
					var tuplizerClassName = FullQualifiedClassName(tuplizer.@class, mappings);
					model.AddTuplizer(mode, tuplizerClassName);
				}
			}
		}

		private void BindManyToOneProperty(HbmManyToOne manyToOneMapping, Property property)
		{
			property.Cascade = manyToOneMapping.cascade ?? mappings.DefaultCascade;
			property.UnwrapProxy = manyToOneMapping.Lazy == HbmLaziness.NoProxy;
			property.IsUpdateable = manyToOneMapping.update;
			property.IsInsertable = manyToOneMapping.insert;
			var toOne = property.Value as ToOne;
			if (toOne != null)
			{
				string propertyRef = toOne.ReferencedPropertyName;
				if (propertyRef != null)
					mappings.AddUniquePropertyReference(toOne.ReferencedEntityName, propertyRef);
				toOne.CreateForeignKey();
			}
		}

		private void BindCollectionProperty(ICollectionPropertiesMapping collectionMapping, Property property)
		{
			property.Cascade = collectionMapping.Cascade ?? mappings.DefaultCascade;
		}

		private Property CreateProperty(IEntityPropertyMapping propertyMapping, System.Type propertyOwnerType, SimpleValue value, IDictionary<string, MetaAttribute> inheritedMetas)
		{
			var type = propertyOwnerType?.UnwrapIfNullable();
			if (string.IsNullOrEmpty(propertyMapping.Name))
				throw new MappingException("A property mapping must define the name attribute [" + type + "]");

			var propertyAccessorName = GetPropertyAccessorName(propertyMapping.Access);

			if (type != null)
				value.SetTypeUsingReflection(type, propertyMapping.Name, propertyAccessorName);

			return new Property
			{
				Name = propertyMapping.Name,
				PropertyAccessorName = propertyAccessorName,
				Value = value,
				IsLazy = propertyMapping.IsLazyProperty,
				LazyGroup = propertyMapping.GetLazyGroup(),
				IsOptimisticLocked = propertyMapping.OptimisticLock,
				MetaAttributes = GetMetas(propertyMapping, inheritedMetas)
			};
		}
		
		private Property CreateProperty(IEntityPropertyMapping propertyMapping, System.Type propertyOwnerType, Mapping.Collection value, IDictionary<string, MetaAttribute> inheritedMetas)
		{
			if (string.IsNullOrEmpty(propertyMapping.Name))
				throw new MappingException("A property mapping must define the name attribute [" + propertyOwnerType + "]");

			var propertyAccessorName = GetPropertyAccessorName(propertyMapping.Access);

			return new Property
			{
				Name = propertyMapping.Name,
				PropertyAccessorName = propertyAccessorName,
				Value = value,
				IsLazy = propertyMapping.IsLazyProperty,
				LazyGroup = propertyMapping.GetLazyGroup(),
				IsOptimisticLocked = propertyMapping.OptimisticLock,
				MetaAttributes = GetMetas(propertyMapping, inheritedMetas)
			};
		}

		private string GetPropertyAccessorName(string propertyMappedAccessor)
		{
			return propertyMappedAccessor ?? Mappings.DefaultAccess;
		}
	}
}
