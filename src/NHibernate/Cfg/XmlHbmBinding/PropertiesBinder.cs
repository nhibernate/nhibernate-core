using System.Collections.Generic;
using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping;
using System;
using NHibernate.Util;
using Array = System.Array;

namespace NHibernate.Cfg.XmlHbmBinding
{
	public class PropertiesBinder : ClassBinder
	{
		private readonly PersistentClass persistentClass;
		private readonly Component component;
		private readonly string entityName;
		private readonly System.Type mappedClass;
		private readonly string className;
		private readonly bool componetDefaultNullable;
		private readonly string propertyBasePath;

		public PropertiesBinder(Mappings mappings, PersistentClass persistentClass, Dialect.Dialect dialect)
			: base(mappings, dialect)
		{
			this.persistentClass = persistentClass;
			entityName = persistentClass.EntityName;
			propertyBasePath = entityName;
			className = persistentClass.ClassName;
			mappedClass = persistentClass.MappedClass;
			componetDefaultNullable = true;
			component = null;
		}

		public PropertiesBinder(Mappings mappings, Component component, string className, string path, bool isNullable, Dialect.Dialect dialect)
			: base(mappings, dialect)
		{
			persistentClass = component.Owner;
			this.component = component;
			entityName = className;
			this.className = component.ComponentClassName;
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

				ICollectionPropertiesMapping collectionMapping;
				HbmManyToOne manyToOneMapping;
				HbmAny anyMapping;
				HbmOneToOne oneToOneMapping;
				HbmProperty propertyMapping;
				HbmComponent componentMapping;
				HbmDynamicComponent dynamicComponentMapping;
				HbmNestedCompositeElement nestedCompositeElementMapping;
				HbmKeyProperty keyPropertyMapping;
				HbmKeyManyToOne keyManyToOneMapping;
				HbmProperties propertiesMapping;

				if ((propertyMapping = entityPropertyMapping as HbmProperty) != null)
				{
					var value = new SimpleValue(table);
					new ValuePropertyBinder(value, Mappings).BindSimpleValue(propertyMapping, propertyName, true);
					property = CreateProperty(entityPropertyMapping, className, value, inheritedMetas);
					BindValueProperty(propertyMapping, property);
				}
				else if ((collectionMapping = entityPropertyMapping as ICollectionPropertiesMapping) != null)
				{
					var collectionBinder = new CollectionBinder(Mappings, dialect);
					string propertyPath = propertyName == null ? null : StringHelper.Qualify(propertyBasePath, propertyName);

					Mapping.Collection collection = collectionBinder.Create(collectionMapping, entityName, propertyPath, persistentClass,
																												mappedClass, inheritedMetas);

					mappings.AddCollection(collection);

					property = CreateProperty(collectionMapping, className, collection, inheritedMetas);
					BindCollectionProperty(collectionMapping, property);
				}
				else if ((propertiesMapping = entityPropertyMapping as HbmProperties) != null)
				{
					var subpath = propertyName == null ? null : StringHelper.Qualify(propertyBasePath, propertyName);
					var value = CreateNewComponent(table);
					BindComponent(propertiesMapping, value, null, entityName, subpath, componetDefaultNullable, inheritedMetas);
					property = CreateProperty(entityPropertyMapping, className, value, inheritedMetas);
					BindComponentProperty(propertiesMapping, property, value);
				}
				else if ((manyToOneMapping = entityPropertyMapping as HbmManyToOne) != null)
				{
					var value = new ManyToOne(table);
					BindManyToOne(manyToOneMapping, value, propertyName, true);
					property = CreateProperty(entityPropertyMapping, className, value, inheritedMetas);
					BindManyToOneProperty(manyToOneMapping, property);
				}
				else if ((componentMapping = entityPropertyMapping as HbmComponent) != null)
				{
					string subpath = propertyName == null ? null : StringHelper.Qualify(propertyBasePath, propertyName);
					var value = CreateNewComponent(table);
					// NH: Modified from H2.1 to allow specifying the type explicitly using class attribute
					System.Type reflectedClass = mappedClass == null ? null : GetPropertyType(componentMapping.Class, mappedClass, propertyName, componentMapping.Access);
					BindComponent(componentMapping, value, reflectedClass, entityName, subpath, componetDefaultNullable, inheritedMetas);
					property = CreateProperty(entityPropertyMapping, className, value, inheritedMetas);
					BindComponentProperty(componentMapping, property, value);
				}
				else if ((oneToOneMapping = entityPropertyMapping as HbmOneToOne) != null)
				{
					var value = new OneToOne(table, persistentClass);
					BindOneToOne(oneToOneMapping, value);
					property = CreateProperty(entityPropertyMapping, className, value, inheritedMetas);
					BindOneToOneProperty(oneToOneMapping, property);
				}
				else if ((dynamicComponentMapping = entityPropertyMapping as HbmDynamicComponent) != null)
				{
					string subpath = propertyName == null ? null : StringHelper.Qualify(propertyBasePath, propertyName);
					var value = CreateNewComponent(table);
					// NH: Modified from H2.1 to allow specifying the type explicitly using class attribute
					System.Type reflectedClass = mappedClass == null ? null : GetPropertyType(dynamicComponentMapping.Class, mappedClass, propertyName, dynamicComponentMapping.Access);
					BindComponent(dynamicComponentMapping, value, reflectedClass, entityName, subpath, componetDefaultNullable, inheritedMetas);
					property = CreateProperty(entityPropertyMapping, className, value, inheritedMetas);
					BindComponentProperty(dynamicComponentMapping, property, value);
				}
				else if ((anyMapping = entityPropertyMapping as HbmAny) != null)
				{
					var value = new Any(table);
					BindAny(anyMapping, value, true);
					property = CreateProperty(entityPropertyMapping, className, value, inheritedMetas);
					BindAnyProperty(anyMapping, property);
				}
				else if ((nestedCompositeElementMapping = entityPropertyMapping as HbmNestedCompositeElement) != null)
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
					property = CreateProperty(entityPropertyMapping, className, value, inheritedMetas);
				}
				else if ((keyPropertyMapping = entityPropertyMapping as HbmKeyProperty) != null)
				{
					var value = new SimpleValue(table);
					new ValuePropertyBinder(value, Mappings).BindSimpleValue(keyPropertyMapping, propertyName, componetDefaultNullable);
					property = CreateProperty(entityPropertyMapping, className, value, inheritedMetas);
				}
				else if ((keyManyToOneMapping = entityPropertyMapping as HbmKeyManyToOne) != null)
				{
					var value = new ManyToOne(table);
					BindKeyManyToOne(keyManyToOneMapping, value, propertyName, componetDefaultNullable);
					property = CreateProperty(entityPropertyMapping, className, value, inheritedMetas);
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
				else
				{
					property.IsInsertable = false;
				}

				// properties generated on update can never be updateable...
				if (propertyMapping.updateSpecified && property.IsUpdateable && generation == PropertyGeneration.Always)
				{
					// the user specifically supplied update="true",
					// which constitutes an illegal combo
					throw new MappingException("cannot specify both update=\"true\" and generated=\"" + generation
					                           + "\" for property: " + propertyMapping.Name);
				}
				else
				{
					property.IsUpdateable = false;
				}
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
				Array.ForEach(tuplizers.Select(tuplizer => new
				                                           	{
				                                           		TuplizerClassName = FullQualifiedClassName(tuplizer.@class, mappings),
				                                           		Mode = tuplizer.entitymode.ToEntityMode()
				                                           	}).ToArray(),
				              x => model.AddTuplizer(x.Mode, x.TuplizerClassName));
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

		private Property CreateProperty(IEntityPropertyMapping propertyMapping, string propertyOwnerClassName, IValue value, IDictionary<string, MetaAttribute> inheritedMetas)
		{
			if (string.IsNullOrEmpty(propertyMapping.Name))
			{
				throw new MappingException("A property mapping must define the name attribute [" + propertyOwnerClassName + "]");
			}

			var propertyAccessorName = GetPropertyAccessorName(propertyMapping.Access);

			if (!string.IsNullOrEmpty(propertyOwnerClassName) && value.IsSimpleValue)
				value.SetTypeUsingReflection(propertyOwnerClassName, propertyMapping.Name, propertyAccessorName);

			var property = new Property
					{
						Name = propertyMapping.Name,
						PropertyAccessorName = propertyAccessorName,
						Value = value,
                        IsLazy = propertyMapping.IsLazyProperty,
						IsOptimisticLocked = propertyMapping.OptimisticLock,
						MetaAttributes = GetMetas(propertyMapping, inheritedMetas)
					};

			return property;
		}

		private string GetPropertyAccessorName(string propertyMappedAccessor)
		{
			return propertyMappedAccessor ?? Mappings.DefaultAccess;
		}
	}
}