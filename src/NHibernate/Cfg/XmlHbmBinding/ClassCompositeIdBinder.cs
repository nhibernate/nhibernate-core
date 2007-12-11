using System.Collections.Generic;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Engine;
using NHibernate.Mapping;
using NHibernate.Property;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Cfg.XmlHbmBinding
{
	public class ClassCompositeIdBinder : ClassBinder
	{
		public ClassCompositeIdBinder(ClassBinder parent)
			: base(parent)
		{
		}

		public void BindCompositeId(HbmCompositeId idSchema, PersistentClass rootClass)
		{
			if (idSchema == null)
				return;

			Component compositeId = new Component(rootClass);
			rootClass.Identifier = compositeId;

			if (idSchema.name == null)
			{
				BindComponent(compositeId, null, "id", idSchema);
				rootClass.HasEmbeddedIdentifier = compositeId.IsEmbedded;
			}
			else
			{
				System.Type reflectedClass = GetPropertyType(rootClass.MappedClass,
					idSchema.name, idSchema);

				BindComponent(compositeId, reflectedClass, idSchema.name, idSchema);

				Mapping.Property prop = new Mapping.Property(compositeId);
				BindProperty(prop, idSchema);
				rootClass.IdentifierProperty = prop;
			}

			compositeId.Table.SetIdentifierValue(compositeId);
			compositeId.NullValue = GetXmlEnumAttribute(idSchema.unsavedvalue);

			System.Type compIdClass = compositeId.ComponentClass;
			if (!ReflectHelper.OverridesEquals(compIdClass))
				throw new MappingException(
					"composite-id class must override Equals(): " + compIdClass.FullName
					);

			if (!ReflectHelper.OverridesGetHashCode(compIdClass))
				throw new MappingException(
					"composite-id class must override GetHashCode(): " + compIdClass.FullName
					);
			// Serializability check not ported
		}

		private void BindComponent(Component compositeId, System.Type reflectedClass, string path,
			HbmCompositeId idSchema)
		{
			if (idSchema.@class != null)
			{
				compositeId.ComponentClass = ClassForNameChecked(idSchema.@class, mappings,
					"component class not found: {0}");

				compositeId.IsEmbedded = false;
			}
			else if (reflectedClass != null)
			{
				compositeId.ComponentClass = reflectedClass;
				compositeId.IsEmbedded = false;
			}
			else
			{
				// an "embedded" component (ids only)
				compositeId.ComponentClass = compositeId.Owner.MappedClass;
				compositeId.IsEmbedded = true;
			}

			foreach (object item in idSchema.Items ?? new object[0])
			{
				HbmKeyManyToOne keyManyToOneSchema = item as HbmKeyManyToOne;
				HbmKeyProperty keyPropertySchema = item as HbmKeyProperty;

				if (keyManyToOneSchema != null)
				{
					ManyToOne manyToOne = new ManyToOne(compositeId.Table);

					string propertyName = keyManyToOneSchema.name == null
						? null
						: StringHelper.Qualify(path, keyManyToOneSchema.name);

					BindManyToOne(keyManyToOneSchema, manyToOne, propertyName, false);

					Mapping.Property property = CreateProperty(manyToOne, keyManyToOneSchema.name,
						compositeId.ComponentClass, keyManyToOneSchema);

					compositeId.AddProperty(property);
				}
				else if (keyPropertySchema != null)
				{
					SimpleValue value = new SimpleValue(compositeId.Table);

					string propertyName = keyPropertySchema.name == null
						? null
						: StringHelper.Qualify(path, keyPropertySchema.name);

					BindSimpleValue(keyPropertySchema, value, false, propertyName);

					Mapping.Property property = CreateProperty(value, keyPropertySchema.name,
						compositeId.ComponentClass, keyPropertySchema);

					compositeId.AddProperty(property);
				}
			}

			int span = compositeId.PropertySpan;
			string[] names = new string[span];
			IType[] types = new IType[span];
			bool[] nullabilities = new bool[span];
			Cascades.CascadeStyle[] cascade = new Cascades.CascadeStyle[span];
			FetchMode[] joinedFetch = new FetchMode[span];

			int i = 0;
			foreach (Mapping.Property prop in compositeId.PropertyIterator)
			{
				names[i] = prop.Name;
				types[i] = prop.Type;
				nullabilities[i] = prop.IsNullable;
				cascade[i] = prop.CascadeStyle;
				joinedFetch[i] = prop.Value.FetchMode;
				i++;
			}

			IType componentType;

			if (compositeId.IsDynamic)
				componentType = new DynamicComponentType(names, types, nullabilities, joinedFetch, cascade);
			else
			{
				IGetter[] getters = new IGetter[span];
				ISetter[] setters = new ISetter[span];
				bool foundCustomAccessor = false;
				i = 0;
				foreach (Mapping.Property prop in compositeId.PropertyIterator)
				{
					setters[i] = prop.GetSetter(compositeId.ComponentClass);
					getters[i] = prop.GetGetter(compositeId.ComponentClass);
					if (!prop.IsBasicPropertyAccessor)
						foundCustomAccessor = true;
					i++;
				}

				componentType = new ComponentType(compositeId.ComponentClass, names, getters, setters,
					foundCustomAccessor, types, nullabilities, joinedFetch, cascade,
					compositeId.ParentProperty);
			}

			compositeId.Type = componentType;
		}

		private void BindProperty(Mapping.Property property, HbmCompositeId idSchema)
		{
			property.Name = idSchema.name;

			if (property.Value.Type == null)
				throw new MappingException("could not determine a property type for: " + property.Name);

			property.PropertyAccessorName = idSchema.access ?? mappings.DefaultAccess;
			property.Cascade = mappings.DefaultCascade;
			property.IsUpdateable = true;
			property.IsInsertable = true;
			property.IsOptimisticLocked = true;
			property.Generation = PropertyGeneration.Never;
			property.MetaAttributes = new Dictionary<string, MetaAttribute>();

			LogMappedProperty(property);
		}

		private System.Type GetPropertyType(System.Type containingType, string propertyName,
			HbmCompositeId idSchema)
		{
			if (idSchema.@class != null)
				return ClassForNameChecked(idSchema.@class, mappings, "could not find class: {0}");

			else if (containingType == null)
				return null;

			else
			{
				string access = idSchema.access ?? mappings.DefaultAccess;
				return ReflectHelper.ReflectedPropertyClass(containingType, propertyName, access);
			}
		}

		private void BindManyToOne(HbmKeyManyToOne keyManyToOneSchema, ManyToOne manyToOne, string defaultColumnName,
			bool isNullable)
		{
			BindColumns(keyManyToOneSchema, manyToOne, isNullable, true, defaultColumnName);

			manyToOne.FetchMode = FetchMode.Default;
			manyToOne.IsLazy = !keyManyToOneSchema.lazySpecified
				? manyToOne.IsLazy
				: keyManyToOneSchema.lazy == HbmRestrictedLaziness.Proxy;

			// TODO NH: this is sort of redundant with the code below
			manyToOne.ReferencedEntityName = GetEntityName(keyManyToOneSchema, mappings);
			manyToOne.IsIgnoreNotFound = false;

			string typeNode = keyManyToOneSchema.@class;

			if (typeNode != null)
				manyToOne.Type = TypeFactory.ManyToOne(ClassForNameChecked(typeNode, mappings,
					"could not find class: {0}"),
					manyToOne.ReferencedPropertyName,
					manyToOne.IsLazy,
					manyToOne.IsIgnoreNotFound);

			if (keyManyToOneSchema.foreignkey != null)
				manyToOne.ForeignKeyName = keyManyToOneSchema.foreignkey;
		}

		private static string GetEntityName(HbmKeyManyToOne keyManyToOneSchema, Mappings mappings)
		{
			if (keyManyToOneSchema.@class == null)
				return null;

			return GetClassName(keyManyToOneSchema.@class, mappings);
		}

		private void BindColumns(HbmKeyManyToOne keyManyToOneSchema, SimpleValue model, bool isNullable,
			bool autoColumn, string propertyPath)
		{
			Table table = model.Table;
			//COLUMN(S)
			if (keyManyToOneSchema.column1 == null)
			{
				int count = 0;

				foreach (HbmColumn columnSchema in keyManyToOneSchema.column ?? new HbmColumn[0])
				{
					Column col = new Column(model.Type, count++);
					BindColumn(columnSchema, col, isNullable);

					string name = columnSchema.name;
					col.Name = mappings.NamingStrategy.ColumnName(name);
					if (table != null)
						table.AddColumn(col);
					//table=null -> an association, fill it in later
					model.AddColumn(col);

					//column index
					BindIndex(columnSchema.index, table, col);
					//column group index (although it can serve as a separate column index)

					BindUniqueKey(columnSchema.uniquekey, table, col);
				}
			}
			else
			{
				Column col = new Column(model.Type, 0);
				BindColumn(col, isNullable);
				col.Name = mappings.NamingStrategy.ColumnName(keyManyToOneSchema.column1);
				if (table != null)
					table.AddColumn(col);
				model.AddColumn(col);
				//column group index (although can serve as a separate column index)
			}

			if (autoColumn && model.ColumnSpan == 0)
			{
				Column col = new Column(model.Type, 0);
				BindColumn(col, isNullable);
				col.Name = mappings.NamingStrategy.PropertyToColumnName(propertyPath);
				model.Table.AddColumn(col);
				model.AddColumn(col);
				//column group index (although can serve as a separate column index)
			}
		}

		private static void BindColumn(Column model, bool isNullable)
		{
			model.IsNullable = isNullable;
			model.IsUnique = false;
			model.CheckConstraint = string.Empty;
			model.SqlType = null;
		}

		private Mapping.Property CreateProperty(ToOne value, string propertyName, System.Type parentClass,
			HbmKeyManyToOne keyManyToOneSchema)
		{
			if (parentClass != null && value.IsSimpleValue)
				value.SetTypeByReflection(parentClass, propertyName, keyManyToOneSchema.access ?? mappings.DefaultAccess);

			string propertyRef = value.ReferencedPropertyName;
			if (propertyRef != null)
				mappings.AddUniquePropertyReference(((EntityType) value.Type).AssociatedClass, propertyRef);

			value.CreateForeignKey();
			Mapping.Property prop = new Mapping.Property();
			prop.Value = value;
			BindProperty(keyManyToOneSchema, prop);

			return prop;
		}

		private void BindProperty(HbmKeyManyToOne keyManyToOneSchema, Mapping.Property property)
		{
			property.Name = keyManyToOneSchema.name;

			if (property.Value.Type == null)
				throw new MappingException("could not determine a property type for: " + property.Name);

			property.PropertyAccessorName = keyManyToOneSchema.access ?? mappings.DefaultAccess;
			property.Cascade = mappings.DefaultCascade;
			property.IsUpdateable = true;
			property.IsInsertable = true;
			property.IsOptimisticLocked = true;
			property.Generation = PropertyGeneration.Never;
			property.MetaAttributes = new Dictionary<string, MetaAttribute>();

			LogMappedProperty(property);
		}

		private void BindSimpleValue(HbmKeyProperty keyPropertySchema, SimpleValue model, bool isNullable, string path)
		{
			model.Type = GetType(keyPropertySchema);
			BindColumns(keyPropertySchema, model, isNullable, true, path);
		}

		private static IType GetType(HbmKeyProperty keyPropertySchema)
		{
			if (keyPropertySchema.type == null)
				return null;

			string typeName = keyPropertySchema.type;
			IType type = TypeFactory.HeuristicType(typeName, null);

			if (type == null)
				throw new MappingException("could not interpret type: " + keyPropertySchema.type);

			return type;
		}

		private void BindColumns(HbmKeyProperty keyPropertySchema, SimpleValue model, bool isNullable, bool autoColumn,
			string propertyPath)
		{
			Table table = model.Table;

			if (keyPropertySchema.column1 == null)
			{
				int count = 0;

				foreach (HbmColumn columnSchema in keyPropertySchema.column ?? new HbmColumn[0])
				{
					Column col = new Column(model.Type, count++);
					BindColumn(columnSchema, col, isNullable);

					col.Name = mappings.NamingStrategy.ColumnName(columnSchema.name);
					if (table != null)
						table.AddColumn(col);
					//table=null -> an association, fill it in later
					model.AddColumn(col);

					//column index
					BindIndex(columnSchema.index, table, col);
					//column group index (although it can serve as a separate column index)

					BindUniqueKey(columnSchema.uniquekey, table, col);
				}
			}
			else
			{
				Column col = new Column(model.Type, 0);
				BindColumn(keyPropertySchema, col, isNullable);
				col.Name = mappings.NamingStrategy.ColumnName(keyPropertySchema.column1);
				if (table != null)
					table.AddColumn(col);
				model.AddColumn(col);
				//column group index (although can serve as a separate column index)
			}

			if (autoColumn && model.ColumnSpan == 0)
			{
				Column col = new Column(model.Type, 0);
				BindColumn(keyPropertySchema, col, isNullable);
				col.Name = mappings.NamingStrategy.PropertyToColumnName(propertyPath);
				model.Table.AddColumn(col);
				model.AddColumn(col);
				//column group index (although can serve as a separate column index)
			}
		}

		private static void BindColumn(HbmKeyProperty keyPropertySchema, Column model, bool isNullable)
		{
			if (keyPropertySchema.length != null)
				model.Length = int.Parse(keyPropertySchema.length);

			model.IsNullable = isNullable;
			model.IsUnique = false;
			model.CheckConstraint = string.Empty;
			model.SqlType = null;
		}

		private Mapping.Property CreateProperty(SimpleValue value, string propertyName, System.Type parentClass,
			HbmKeyProperty keyPropertySchema)
		{
			if (parentClass != null && value.IsSimpleValue)
				value.SetTypeByReflection(parentClass, propertyName, keyPropertySchema.access ?? mappings.DefaultAccess);

			// This is done here 'cos we might only know the type here (ugly!)
			if (value is ToOne)
			{
				ToOne toOne = (ToOne) value;
				string propertyRef = toOne.ReferencedPropertyName;
				if (propertyRef != null)
					mappings.AddUniquePropertyReference(((EntityType) value.Type).AssociatedClass, propertyRef);
			}

			value.CreateForeignKey();
			Mapping.Property prop = new Mapping.Property();
			prop.Value = value;
			BindProperty(keyPropertySchema, prop);

			return prop;
		}

		private void BindProperty(HbmKeyProperty keyPropertySchema, Mapping.Property property)
		{
			property.Name = keyPropertySchema.name;

			if (property.Value.Type == null)
				throw new MappingException("could not determine a property type for: " + property.Name);

			property.PropertyAccessorName = keyPropertySchema.access ?? mappings.DefaultAccess;
			property.Cascade = mappings.DefaultCascade;
			property.IsUpdateable = true;
			property.IsInsertable = true;
			property.IsOptimisticLocked = true;
			property.Generation = PropertyGeneration.Never;
			property.MetaAttributes = new Dictionary<string, MetaAttribute>();
			LogMappedProperty(property);
		}
	}
}