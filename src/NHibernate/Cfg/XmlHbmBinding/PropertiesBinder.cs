using System.Collections.Generic;
using System.Linq;
using System.Xml;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping;
using System;
using NHibernate.Util;

namespace NHibernate.Cfg.XmlHbmBinding
{
	public class PropertiesBinder : ClassBinder
	{
		private readonly PersistentClass model;

		public PropertiesBinder(Mappings mappings, PersistentClass model, XmlNamespaceManager namespaceManager, Dialect.Dialect dialect)
			: base(mappings, namespaceManager, dialect)
		{
			this.model = model;
		}

		public void Bind(IEnumerable<IEntityPropertyMapping> properties, IDictionary<string, MetaAttribute> inheritedMetas)
		{
			Bind(properties, inheritedMetas, null);
		}

		public void Bind(IEnumerable<IEntityPropertyMapping> properties, IDictionary<string, MetaAttribute> inheritedMetas, Action<Property> modifier)
		{
			Action<Property> action = modifier ?? (p => { }); 
			string entityName = model.EntityName;
			Table table = model.Table;

			foreach (var entityPropertyMapping in properties)
			{
				Property property= null;

				string propertyName = entityPropertyMapping.Name;

				ICollectionPropertyMapping collectionMapping;
				HbmManyToOne manyToOneMapping;
				HbmAny anyMapping;
				HbmOneToOne oneToOneMapping;
				HbmProperty propertyMapping;
				HbmComponent componentMapping;
				HbmDynamicComponent dynamicComponentMapping;

				if ((propertyMapping = entityPropertyMapping as HbmProperty) != null)
				{
					var value = new SimpleValue(table);
					BindSimpleValue(Serialize(propertyMapping), value, true, propertyName);
					property = CreateProperty(entityPropertyMapping, model.ClassName, value, inheritedMetas);
					BindValueProperty(propertyMapping, property);
				}
				else if ((collectionMapping = entityPropertyMapping as ICollectionPropertyMapping) != null)
				{
					var collectionBinder = new CollectionBinder(Mappings, namespaceManager, dialect);

					Mapping.Collection collection = collectionBinder.Create(collectionMapping, entityName, propertyName, model,
																												model.MappedClass, inheritedMetas);

					mappings.AddCollection(collection);

					property = CreateProperty(collectionMapping, model.ClassName, collection, inheritedMetas);
					BindCollectionProperty(collectionMapping, property);
				}
				else if ((manyToOneMapping = entityPropertyMapping as HbmManyToOne) != null)
				{
					var value = new ManyToOne(table);
					BindManyToOne(Serialize(manyToOneMapping), value, propertyName, true);
					property = CreateProperty(entityPropertyMapping, model.ClassName, value, inheritedMetas);
					BindManyToOneProperty(manyToOneMapping, property);
				}
				else if ((componentMapping = entityPropertyMapping as HbmComponent) != null)
				{
					string subpath = StringHelper.Qualify(entityName, propertyName);
					var subnode = Serialize(componentMapping);
					var value = new Component(model);
					// NH: Modified from H2.1 to allow specifying the type explicitly using class attribute
					System.Type reflectedClass = GetPropertyType(subnode, model.MappedClass, propertyName);
					BindComponent(subnode, value, reflectedClass, entityName, propertyName, subpath, true, inheritedMetas);
					property = CreateProperty(entityPropertyMapping, model.ClassName, value, inheritedMetas);
					BindComponentProperty(componentMapping, property);
				}
				else if ((oneToOneMapping = entityPropertyMapping as HbmOneToOne) != null)
				{
					var value = new OneToOne(table, model);
					BindOneToOne(Serialize(oneToOneMapping), value);
					property = CreateProperty(entityPropertyMapping, model.ClassName, value, inheritedMetas);
					BindOneToOneProperty(oneToOneMapping, property);
				}
				else if ((dynamicComponentMapping = entityPropertyMapping as HbmDynamicComponent) != null)
				{
					string subpath = StringHelper.Qualify(entityName, propertyName);
					var subnode = Serialize(dynamicComponentMapping);
					var value = new Component(model);
					// NH: Modified from H2.1 to allow specifying the type explicitly using class attribute
					System.Type reflectedClass = GetPropertyType(subnode, model.MappedClass, propertyName);
					BindComponent(subnode, value, reflectedClass, entityName, propertyName, subpath, true, inheritedMetas);
					property = CreateProperty(entityPropertyMapping, model.ClassName, value, inheritedMetas);
					BindComponentProperty(dynamicComponentMapping, property);
				}
				else if ((anyMapping = entityPropertyMapping as HbmAny) != null)
				{
					var value = new Any(table);
					BindAny(Serialize(anyMapping), value, true);
					property = CreateProperty(entityPropertyMapping, model.ClassName, value, inheritedMetas);
					BindAnyProperty(anyMapping, property);
				}

				if(property != null)
				{
					action(property);
					if (log.IsDebugEnabled)
					{
						string msg = "Mapped property: " + property.Name;
						string columns = string.Join(",", property.Value.ColumnIterator.Select(c => c.Text).ToArray());
						if (columns.Length > 0)
							msg += " -> " + columns;
						if (property.Type != null)
							msg += ", type: " + property.Type.Name;
						log.Debug(msg);
					}
					model.AddProperty(property);
				}
			}
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
			var toOne = property.Value as ToOne;
			if (toOne != null)
			{
				string propertyRef = toOne.ReferencedPropertyName;
				if (propertyRef != null)
					mappings.AddUniquePropertyReference(toOne.ReferencedEntityName, propertyRef);
				toOne.CreateForeignKey();
			}
		}

		private void BindComponentProperty(HbmDynamicComponent dynamicComponentMapping, Property property)
		{
			property.IsUpdateable = dynamicComponentMapping.update;
			property.IsInsertable = dynamicComponentMapping.insert;
		}

		private void BindComponentProperty(HbmComponent componentMapping, Property property)
		{
			property.IsUpdateable = componentMapping.update;
			property.IsInsertable = componentMapping.insert;
		}

		private void BindManyToOneProperty(HbmManyToOne manyToOneMapping, Property property)
		{
			property.Cascade = manyToOneMapping.cascade ?? mappings.DefaultCascade;
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

		private void BindCollectionProperty(ICollectionPropertyMapping collectionMapping, Property property)
		{
			property.Cascade = collectionMapping.Cascade ?? mappings.DefaultCascade;
		}

		private Property CreateProperty(IEntityPropertyMapping propertyMapping, string className, IValue value, IDictionary<string, MetaAttribute> inheritedMetas)
		{
			if (string.IsNullOrEmpty(propertyMapping.Name))
			{
				throw new MappingException("A property mapping must define the name attribute [" + className + "]");
			}

			var propertyAccessorName = propertyMapping.Access ?? Mappings.DefaultAccess;

			if (!string.IsNullOrEmpty(className) && value.IsSimpleValue)
				value.SetTypeUsingReflection(className, propertyMapping.Name, propertyAccessorName);

			var property = new Property
					{
						Name = propertyMapping.Name,
						PropertyAccessorName = propertyAccessorName,
						Value = value,
						IsOptimisticLocked = propertyMapping.OptimisticKock,
						MetaAttributes = GetMetas(propertyMapping, inheritedMetas)
					};

			return property;
		}
	}
}