using System;
using System.Collections;
using System.Text;
using System.Xml;

using NHibernate.Cfg.MappingSchema;
using NHibernate.Engine;
using NHibernate.Mapping;
using NHibernate.Property;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Cfg.XmlHbmBinding
{
	public abstract class ClassBinder : Binder
	{
		protected readonly Dialect.Dialect dialect;
		protected readonly XmlNamespaceManager namespaceManager;

		public ClassBinder(Binder parent, XmlNamespaceManager namespaceManager, Dialect.Dialect dialect)
			: base(parent)
		{
			this.dialect = dialect;
			this.namespaceManager = namespaceManager;
		}

		public ClassBinder(ClassBinder parent)
			: base(parent)
		{
			dialect = parent.dialect;
			namespaceManager = parent.namespaceManager;
		}

		protected void PropertiesFromXML(XmlNode node, PersistentClass model)
		{
			Table table = model.Table;

			foreach (XmlNode subnode in node.ChildNodes)
			{
				//I am only concerned with elements that are from the nhibernate namespace
				if (subnode.NamespaceURI != Configuration.MappingSchemaXMLNS)
					continue;

				string name = subnode.LocalName; //.Name;
				string propertyName = GetPropertyName(subnode);

				IValue value = null;
				CollectionBinder collectionBinder = new CollectionBinder(this);
				if (collectionBinder.CanCreate(name))
				{
					Mapping.Collection collection = collectionBinder.Create(name, subnode, model.Name,
						propertyName, model, model.MappedClass);

					mappings.AddCollection(collection);
					value = collection;
				}
				else if ("many-to-one".Equals(name))
				{
					value = new ManyToOne(table);
					BindManyToOne(subnode, (ManyToOne) value, propertyName, true);
				}
				else if ("any".Equals(name))
				{
					value = new Any(table);
					BindAny(subnode, (Any) value, true);
				}
				else if ("one-to-one".Equals(name))
				{
					value = new OneToOne(table, model.Identifier);
					BindOneToOne(subnode, (OneToOne) value);
				}
				else if ("property".Equals(name))
				{
					value = new SimpleValue(table);
					BindSimpleValue(subnode, (SimpleValue) value, true, propertyName);
				}
				else if ("component".Equals(name) || "dynamic-component".Equals(name))
				{
					// NH: Modified from H2.1 to allow specifying the type explicitly using class attribute
					System.Type reflectedClass = GetPropertyType(subnode, model.MappedClass, propertyName);
					value = new Component(model);
					BindComponent(subnode, (Component) value, reflectedClass, model.Name, propertyName, true);
				}
				else if ("join".Equals(name))
				{
					Join join = new Join();
					join.PersistentClass = model;
					BindJoin(subnode, join);
					model.AddJoin(join);
				}
				else if ("subclass".Equals(name))
					new SubclassBinder(this).HandleSubclass(model, subnode);

				else if ("joined-subclass".Equals(name))
					new JoinedSubclassBinder(this).HandleJoinedSubclass(model, subnode);

				else if ("union-subclass".Equals(name))
					new UnionSubclassBinder(this).HandleUnionSubclass(model, subnode);

				else if ("filter".Equals(name))
					ParseFilter(subnode, model);

				if (value != null)
					model.AddProperty(CreateProperty(value, propertyName, model.MappedClass, subnode));
			}
		}

		protected void BindClass(XmlNode node, PersistentClass model)
		{
			string className = node.Attributes["name"] == null ? null : FullClassName(node.Attributes["name"].Value, mappings);

			// CLASS
			model.MappedClass = ClassForFullNameChecked(className, "persistent class {0} not found");

			string entityName = node.Attributes["entity-name"] == null ? null : node.Attributes["name"].Value;
			if (entityName == null)
				entityName = className;
			if (entityName == null)
			{
				throw new MappingException("Unable to determine entity name");
			}
			model.EntityName = entityName;

			// PROXY INTERFACE
			XmlAttribute proxyNode = node.Attributes["proxy"];
			XmlAttribute lazyNode = node.Attributes["lazy"];
			bool lazy = lazyNode == null ? mappings.DefaultLazy : "true".Equals(lazyNode.Value);

			// go ahead and set the lazy here, since pojo.proxy can override it.
			model.IsLazy = lazy;

			if (proxyNode != null)
			{
				model.ProxyInterface = ClassForNameChecked(proxyNode.Value, mappings, "proxy class not found: {0}");
				model.IsLazy = true;
			}
			else if (model.IsLazy)
				model.ProxyInterface = model.MappedClass;

			// DISCRIMINATOR
			XmlAttribute discriminatorNode = node.Attributes["discriminator-value"];
			model.DiscriminatorValue = (discriminatorNode == null) ? model.Name : discriminatorNode.Value;

			// DYNAMIC UPDATE
			XmlAttribute dynamicNode = node.Attributes["dynamic-update"];
			model.DynamicUpdate = (dynamicNode == null) ? false : "true".Equals(dynamicNode.Value);

			// DYNAMIC INSERT
			XmlAttribute insertNode = node.Attributes["dynamic-insert"];
			model.DynamicInsert = (insertNode == null) ? false : "true".Equals(insertNode.Value);

			// IMPORT

			// we automatically want to add an import of the Assembly Qualified Name (includes version, 
			// culture, public-key) to the className supplied in the hbm.xml file.  The most common use-case
			// will have it contain the "FullClassname, AssemblyName", it might contain version, culture, 
			// public key, etc...) but should not assume it does.
			mappings.AddImport(model.MappedClass.AssemblyQualifiedName, StringHelper.GetFullClassname(className));

			// if we are supposed to auto-import the Class then add an import to get from the Classname
			// to the Assembly Qualified Class Name
			if (mappings.IsAutoImport)
				mappings.AddImport(model.MappedClass.AssemblyQualifiedName, StringHelper.GetClassname(className));

			// BATCH SIZE
			XmlAttribute batchNode = node.Attributes["batch-size"];
			if (batchNode != null)
				model.BatchSize = int.Parse(batchNode.Value);

			// SELECT BEFORE UPDATE
			XmlAttribute sbuNode = node.Attributes["select-before-update"];
			if (sbuNode != null)
				model.SelectBeforeUpdate = "true".Equals(sbuNode.Value);

			// OPTIMISTIC LOCK MODE
			XmlAttribute olNode = node.Attributes["optimistic-lock"];
			model.OptimisticLockMode = GetOptimisticLockMode(olNode);

			// META ATTRIBUTES
			model.MetaAttributes = GetMetas(node);

			// PERSISTER
			XmlAttribute persisterNode = node.Attributes["persister"];
			if (persisterNode == null)
			{
				//persister = typeof( EntityPersister );
			}
			else
				model.ClassPersisterClass =
					ClassForNameChecked(persisterNode.Value, mappings, "could not instantiate persister class: {0}");

			// CUSTOM SQL
			HandleCustomSQL(node, model);

			foreach (XmlNode syncNode in node.SelectNodes(HbmConstants.nsSynchronize, namespaceManager))
				model.AddSynchronizedTable(XmlHelper.GetAttributeValue(syncNode, "table"));

			bool? isAbstract = null;
			XmlAttribute abstractNode = node.Attributes["abstract"];
			if (abstractNode != null)
			{
				if ("true".Equals(abstractNode.Value) || "1".Equals(abstractNode.Value))
					isAbstract = true;
				else if ("false".Equals(abstractNode.Value) || "0".Equals(abstractNode.Value))
					isAbstract = false;
			}
			model.IsAbstract = isAbstract;
		}

		private void BindJoin(XmlNode node, Join join)
		{
			PersistentClass persistentClass = join.PersistentClass;
			String path = persistentClass.Name;

			// TABLENAME

			XmlAttribute schemaNode = node.Attributes["schema"];
			string schema = schemaNode == null
				?
					mappings.SchemaName
				: schemaNode.Value;

			Table table = mappings.AddTable(schema, GetClassTableName(persistentClass, node), false);
			join.Table = table;

			XmlAttribute fetchNode = node.Attributes["fetch"];
			if (fetchNode != null)
				join.IsSequentialSelect = "select".Equals(fetchNode.Value);

			XmlAttribute invNode = node.Attributes["inverse"];
			if (invNode != null)
				join.IsInverse = "true".Equals(invNode.Value);

			XmlAttribute nullNode = node.Attributes["optional"];
			if (nullNode != null)
				join.IsOptional = "true".Equals(nullNode.Value);

			log.InfoFormat("Mapping class join: {0} -> {1}", persistentClass.Name, join.Table.Name);

			// KEY
			XmlNode keyNode = node.SelectSingleNode(HbmConstants.nsKey, namespaceManager);
			SimpleValue key = new DependentValue(table, persistentClass.Identifier);
			join.Key = key;
			// key.SetCascadeDeleteEnabled("cascade".Equals(keyNode.Attributes["on-delete"].Value));
			BindSimpleValue(keyNode, key, false, persistentClass.Name);
			// TODO: not sure if the following if-block is correct
			if (key.Type == null)
				key.Type = persistentClass.Identifier.Type;

			join.CreatePrimaryKey(dialect);
			join.CreateForeignKey();

			// PROPERTIES
			//PropertiesFromXML(node, persistentClass, mappings);
			foreach (XmlNode subnode in node.ChildNodes)
			{
				string name = subnode.Name;
				XmlAttribute nameAttribute = subnode.Attributes["name"];
				string propertyName = nameAttribute == null ? null : nameAttribute.Value;

				IValue value = null;
				switch (name)
				{
					case "many-to-one":
						value = new ManyToOne(table);
						BindManyToOne(subnode, (ManyToOne) value, propertyName, true);
						break;
					case "any":
						value = new Any(table);
						BindAny(subnode, (Any) value, true);
						break;
					case "property":
						value = new SimpleValue(table);
						BindSimpleValue(subnode, (SimpleValue) value, true, propertyName);
						break;
					case "component":
					case "dynamic-component":
						string subpath = StringHelper.Qualify(path, propertyName);
						value = new Component(join);
						BindComponent(
							subnode,
							(Component) value,
							join.PersistentClass.MappedClass,
							propertyName,
							subpath,
							true);
						break;
				}

				if (value != null)
				{
					Mapping.Property prop = CreateProperty(value, propertyName, persistentClass.MappedClass, subnode);
					prop.IsOptional = join.IsOptional;
					join.AddProperty(prop);
				}
			}

			// CUSTOM SQL
			HandleCustomSQL(node, join);
		}

		private void HandleCustomSQL(XmlNode node, PersistentClass model)
		{
			XmlNode element = node.SelectSingleNode(HbmConstants.nsSqlInsert, namespaceManager);

			if (element != null)
			{
				bool callable = IsCallable(element);
				model.SetCustomSQLInsert(element.InnerText.Trim(), callable, GetResultCheckStyle(element, callable));
			}

			element = node.SelectSingleNode(HbmConstants.nsSqlDelete, namespaceManager);
			if (element != null)
			{
				bool callable = IsCallable(element);
				model.SetCustomSQLDelete(element.InnerText.Trim(), callable, GetResultCheckStyle(element, callable));
			}

			element = node.SelectSingleNode(HbmConstants.nsSqlUpdate, namespaceManager);
			if (element != null)
			{
				bool callable = IsCallable(element);
				model.SetCustomSQLUpdate(element.InnerText.Trim(), callable, GetResultCheckStyle(element, callable));
			}

			element = node.SelectSingleNode(HbmConstants.nsLoader, namespaceManager);
			if (element != null)
				model.LoaderName = XmlHelper.GetAttributeValue(element, "query-ref");
		}

		private void HandleCustomSQL(XmlNode node, Join model)
		{
			XmlNode element = node.SelectSingleNode(HbmConstants.nsSqlInsert, namespaceManager);
			if (element != null)
			{
				bool callable = IsCallable(element);
				model.SetCustomSQLInsert(element.InnerText.Trim(), callable, GetResultCheckStyle(element, callable));
			}

			element = node.SelectSingleNode(HbmConstants.nsSqlDelete, namespaceManager);
			if (element != null)
			{
				bool callable = IsCallable(element);
				model.SetCustomSQLDelete(element.InnerText.Trim(), callable, GetResultCheckStyle(element, callable));
			}

			element = node.SelectSingleNode(HbmConstants.nsSqlUpdate, namespaceManager);
			if (element != null)
			{
				bool callable = IsCallable(element);
				model.SetCustomSQLUpdate(element.InnerText.Trim(), callable, GetResultCheckStyle(element, callable));
			}
		}

		private static OptimisticLockMode GetOptimisticLockMode(XmlAttribute olAtt)
		{
			if (olAtt == null)
				return OptimisticLockMode.Version;

			string olMode = olAtt.Value;

			if (olMode == null || "version".Equals(olMode))
				return OptimisticLockMode.Version;
			else if ("dirty".Equals(olMode))
				return OptimisticLockMode.Dirty;
			else if ("all".Equals(olMode))
				return OptimisticLockMode.All;
			else if ("none".Equals(olMode))
				return OptimisticLockMode.None;
			else
				throw new MappingException("Unsupported optimistic-lock style: " + olMode);
		}

		protected PersistentClass GetSuperclass(XmlNode subnode)
		{
			XmlAttribute extendsAttr = subnode.Attributes["extends"];
			if (extendsAttr == null)
				throw new MappingException("'extends' attribute is not found.");
			String extendsValue = FullClassName(extendsAttr.Value, mappings);
			System.Type superclass = ClassForFullNameChecked(extendsValue,
				"extended class not found: {0}");
			PersistentClass superModel = mappings.GetClass(superclass);

			if (superModel == null)
				throw new MappingException("Cannot extend unmapped class: " + extendsValue);
			return superModel;
		}

		protected string GetClassTableName(PersistentClass model, XmlNode node)
		{
			XmlAttribute tableNameNode = node.Attributes["table"];
			if (tableNameNode == null)
				return mappings.NamingStrategy.ClassToTableName(model.Name);
			else
				return mappings.NamingStrategy.TableName(tableNameNode.Value);
		}

		protected void MakeIdentifier(XmlNode node, SimpleValue model)
		{
			//GENERATOR

			XmlNode subnode = node.SelectSingleNode(HbmConstants.nsGenerator, namespaceManager);
			if (subnode != null)
			{
				if (subnode.Attributes["class"] == null)
					throw new MappingException("no class given for generator");

				model.IdentifierGeneratorStrategy = subnode.Attributes["class"].Value;

				IDictionary parms = new Hashtable();

				// NOTE: While fixing NH-1061, a couple of lines similar to the following
				// were added to ClassIdBinder.GetGeneratorProperties().  It looks like
				// we may need it here too.  But I don't want to put it in just yet.
				/* if (model.Table.Schema != null)
				    parms.Add("schema", model.Table.Schema);
				else */
				if (mappings.SchemaName != null)
					parms.Add("schema", dialect.QuoteForSchemaName(mappings.SchemaName));

				parms.Add("target_table", model.Table.GetQuotedName(dialect));

				foreach (Column col in model.ColumnCollection)
				{
					parms.Add("target_column", col);
					break;
				}

				foreach (XmlNode childNode in subnode.SelectNodes(HbmConstants.nsParam, namespaceManager))
					parms.Add(
						childNode.Attributes["name"].Value,
						childNode.InnerText
						);

				model.IdentifierGeneratorProperties = parms;
			}

			model.Table.SetIdentifierValue(model);

			//unsaved-value
			XmlAttribute nullValueNode = node.Attributes["unsaved-value"];
			if (nullValueNode != null)
				model.NullValue = nullValueNode.Value;
			else if (model.IdentifierGeneratorStrategy == "assigned")
				// TODO: H3 has model.setNullValue("undefined") here, but
				// NH doesn't (yet) allow "undefined" for id unsaved-value,
				// so we use "null" here
				model.NullValue = "null";
			else
				model.NullValue = null;
		}

		protected void BindComponent(XmlNode node, Component model, System.Type reflectedClass,
			string className, string path, bool isNullable)
		{
			XmlAttribute classNode = node.Attributes["class"];

			if ("dynamic-component".Equals(node.Name))
			{
				model.IsEmbedded = false;
				model.IsDynamic = true;
			}
			else if (classNode != null)
			{
				model.ComponentClass = ClassForNameChecked(
					classNode.Value, mappings,
					"component class not found: {0}");
				model.IsEmbedded = false;
			}
			else if (reflectedClass != null)
			{
				model.ComponentClass = reflectedClass;
				model.IsEmbedded = false;
			}
			else
			{
				// an "embedded" component (ids only)
				model.ComponentClass = model.Owner.MappedClass;
				model.IsEmbedded = true;
			}

			foreach (XmlNode subnode in node.ChildNodes)
			{
				//I am only concerned with elements that are from the nhibernate namespace
				if (subnode.NamespaceURI != Configuration.MappingSchemaXMLNS)
					continue;

				string name = subnode.LocalName; //.Name;
				string propertyName = GetPropertyName(subnode);
				string subpath = propertyName == null ? null : StringHelper.Qualify(path, propertyName);

				IValue value = null;

				CollectionBinder binder = new CollectionBinder(this);

				if (binder.CanCreate(name))
				{
					Mapping.Collection collection = binder.Create(name, subnode, className,
						subpath, model.Owner, model.ComponentClass);

					mappings.AddCollection(collection);
					value = collection;
				}
				else if ("many-to-one".Equals(name) || "key-many-to-one".Equals(name))
				{
					value = new ManyToOne(model.Table);
					BindManyToOne(subnode, (ManyToOne) value, subpath, isNullable);
				}
				else if ("one-to-one".Equals(name))
				{
					value = new OneToOne(model.Table, model.Owner.Identifier);
					BindOneToOne(subnode, (OneToOne) value);
				}
				else if ("any".Equals(name))
				{
					value = new Any(model.Table);
					BindAny(subnode, (Any) value, isNullable);
				}
				else if ("property".Equals(name) || "key-property".Equals(name))
				{
					value = new SimpleValue(model.Table);
					BindSimpleValue(subnode, (SimpleValue) value, isNullable, subpath);
				}
				else if ("component".Equals(name) || "dynamic-component".Equals(name) || "nested-composite-element".Equals(name))
				{
					System.Type subreflectedClass = model.ComponentClass == null
						?
							null
						:
							GetPropertyType(subnode, model.ComponentClass, propertyName);
					value = (model.Owner != null)
						?
							new Component(model.Owner)
						: // a class component
						new Component(model.Table); // a composite element
					BindComponent(subnode, (Component) value, subreflectedClass, className, subpath, isNullable);
				}
				else if ("parent".Equals(name))
					model.ParentProperty = propertyName;

				if (value != null)
					model.AddProperty(CreateProperty(value, propertyName, model.ComponentClass, subnode));
			}

			int span = model.PropertySpan;
			string[] names = new string[span];
			IType[] types = new IType[span];
			bool[] nullabilities = new bool[span];
			Cascades.CascadeStyle[] cascade = new Cascades.CascadeStyle[span];
			FetchMode[] joinedFetch = new FetchMode[span];

			int i = 0;
			foreach (Mapping.Property prop in model.PropertyCollection)
			{
				names[i] = prop.Name;
				types[i] = prop.Type;
				nullabilities[i] = prop.IsNullable;
				cascade[i] = prop.CascadeStyle;
				joinedFetch[i] = prop.Value.FetchMode;
				i++;
			}

			IType componentType;
			if (model.IsDynamic)
				componentType = new DynamicComponentType(names, types, nullabilities, joinedFetch, cascade);
			else
			{
				IGetter[] getters = new IGetter[span];
				ISetter[] setters = new ISetter[span];
				bool foundCustomAccessor = false;
				i = 0;
				foreach (Mapping.Property prop in model.PropertyCollection)
				{
					setters[i] = prop.GetSetter(model.ComponentClass);
					getters[i] = prop.GetGetter(model.ComponentClass);
					if (!prop.IsBasicPropertyAccessor)
						foundCustomAccessor = true;
					i++;
				}

				componentType = new ComponentType(
					model.ComponentClass,
					names,
					getters,
					setters,
					foundCustomAccessor,
					types,
					nullabilities,
					joinedFetch,
					cascade,
					model.ParentProperty);
			}
			model.Type = componentType;
		}

		protected Mapping.Property CreateProperty(IValue value, string propertyName, System.Type parentClass,
			XmlNode subnode)
		{
			if (parentClass != null && value.IsSimpleValue)
				((SimpleValue) value).SetTypeByReflection(parentClass, propertyName, PropertyAccess(subnode));

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
			BindProperty(subnode, prop);

			return prop;
		}

		protected void BindProperty(XmlNode node, Mapping.Property property)
		{
			string propName = XmlHelper.GetAttributeValue(node, "name");
			property.Name = propName;
			IType type = property.Value.Type;
			if (type == null)
				throw new MappingException("could not determine a property type for: " + property.Name);

			property.PropertyAccessorName = PropertyAccess(node);

			XmlAttribute cascadeNode = node.Attributes["cascade"];
			property.Cascade = (cascadeNode == null) ? mappings.DefaultCascade : cascadeNode.Value;

			XmlAttribute updateNode = node.Attributes["update"];
			property.IsUpdateable = (updateNode == null) ? true : "true".Equals(updateNode.Value);

			XmlAttribute insertNode = node.Attributes["insert"];
			property.IsInsertable = (insertNode == null) ? true : "true".Equals(insertNode.Value);

			XmlAttribute optimisticLockNode = node.Attributes["optimistic-lock"];
			property.IsOptimisticLocked = (optimisticLockNode == null) ? true : "true".Equals(optimisticLockNode.Value);

			XmlAttribute generatedNode = node.Attributes["generated"];
			string generationName = generatedNode == null ? null : generatedNode.Value;
			PropertyGeneration generation = ParsePropertyGeneration(generationName);
			property.Generation = generation;

			if (generation == PropertyGeneration.Always || generation == PropertyGeneration.Insert)
			{
				// generated properties can *never* be insertable...
				if (property.IsInsertable)
					if (insertNode == null)
						// insertable simply because that is the user did not specify
						// anything; just override it
						property.IsInsertable = false;
					else
						// the user specifically supplied insert="true",
						// which constitutes an illegal combo
						throw new MappingException(
							"cannot specify both insert=\"true\" and generated=\"" + generationName +
								"\" for property: " + propName);

				// properties generated on update can never be updateable...
				if (property.IsUpdateable && generation == PropertyGeneration.Always)
					if (updateNode == null)
						// updateable only because the user did not specify 
						// anything; just override it
						property.IsUpdateable = false;
					else
						// the user specifically supplied update="true",
						// which constitutes an illegal combo
						throw new MappingException(
							"cannot specify both update=\"true\" and generated=\"" + generationName +
								"\" for property: " + propName);
			}

			if (log.IsDebugEnabled)
			{
				string msg = "Mapped property: " + property.Name;
				string columns = Columns(property.Value);
				if (columns.Length > 0)
					msg += " -> " + columns;
				if (property.Type != null)
					msg += ", type: " + property.Type.Name;
				log.Debug(msg);
			}

			property.MetaAttributes = GetMetas(node);
		}

		protected static PropertyGeneration ParsePropertyGeneration(string name)
		{
			switch (name)
			{
				case "insert":
					return PropertyGeneration.Insert;
				case "always":
					return PropertyGeneration.Always;
				default:
					return PropertyGeneration.Never;
			}
		}

		protected static string Columns(IValue val)
		{
			StringBuilder columns = new StringBuilder();
			bool first = true;
			foreach (ISelectable col in val.ColumnCollection)
			{
				if (first)
					first = false;
				else
					columns.Append(", ");
				columns.Append(col.Text);
			}
			return columns.ToString();
		}

		//automatically makes a column with the default name if none is specifed by XML
		protected void BindSimpleValue(XmlNode node, SimpleValue model, bool isNullable, string path)
		{
			model.Type = GetTypeFromXML(node);
			//BindSimpleValueType(node, model, mappings);

			XmlAttribute formulaNode = node.Attributes["formula"];
			if (formulaNode != null)
			{
				Formula f = new Formula();
				f.FormulaString = formulaNode.InnerText;
				model.AddFormula(f);
			}
			else
				BindColumns(node, model, isNullable, true, path);

			XmlAttribute fkNode = node.Attributes["foreign-key"];
			if (fkNode != null)
				model.ForeignKeyName = fkNode.Value;
		}

		protected void BindManyToOne(XmlNode node, ManyToOne model, string defaultColumnName, bool isNullable)
		{
			BindColumns(node, model, isNullable, true, defaultColumnName);
			InitOuterJoinFetchSetting(node, model);
			InitLaziness(node, model, true);

			XmlAttribute ukName = node.Attributes["property-ref"];
			if (ukName != null)
				model.ReferencedPropertyName = ukName.Value;

			// TODO NH: this is sort of redundant with the code below
			model.ReferencedEntityName = GetEntityName(node, mappings);

			string notFound = XmlHelper.GetAttributeValue(node, "not-found");
			model.IsIgnoreNotFound = "ignore".Equals(notFound);

			XmlAttribute typeNode = node.Attributes["class"];

			if (typeNode != null)
				model.Type = TypeFactory.ManyToOne(
					ClassForNameChecked(typeNode.Value, mappings,
						"could not find class: {0}"),
					model.ReferencedPropertyName,
					model.IsLazy,
					model.IsIgnoreNotFound);

			XmlAttribute fkNode = node.Attributes["foreign-key"];
			if (fkNode != null)
				model.ForeignKeyName = fkNode.Value;
		}

		protected void BindAny(XmlNode node, Any model, bool isNullable)
		{
			model.IdentifierType = GetTypeFromXML(node);

			XmlAttribute metaAttribute = node.Attributes["meta-type"];
			if (metaAttribute != null)
			{
				IType metaType = TypeFactory.HeuristicType(metaAttribute.Value);
				if (metaType == null)
					throw new MappingException("could not interpret meta-type");
				model.MetaType = metaType;

				Hashtable values = new Hashtable();
				foreach (XmlNode metaValue in node.SelectNodes(HbmConstants.nsMetaValue, namespaceManager))
					try
					{
						object value = model.MetaType.FromString(metaValue.Attributes["value"].Value);
						System.Type clazz = ReflectHelper.ClassForName(FullClassName(metaValue.Attributes["class"].Value, mappings));
						values[value] = clazz;
					}
					catch (InvalidCastException)
					{
						throw new MappingException("meta-type was not an IDiscriminatorType: " + metaType.Name);
					}
					catch (HibernateException he)
					{
						throw new MappingException("could not interpret meta-value", he);
					}
					catch (TypeLoadException cnfe)
					{
						throw new MappingException("meta-value class not found", cnfe);
					}

				if (values.Count > 0)
					model.MetaType = new MetaType(values, model.MetaType);
			}

			BindColumns(node, model, isNullable, false, null);
		}

		private void BindOneToOne(XmlNode node, OneToOne model)
		{
			//BindColumns( node, model, isNullable, false, null, mappings );
			InitOuterJoinFetchSetting(node, model);
			InitLaziness(node, model, true);

			XmlAttribute constrNode = node.Attributes["constrained"];
			bool constrained = constrNode != null && constrNode.Value.Equals("true");
			model.IsConstrained = constrained;

			model.ForeignKeyDirection = (constrained
				? ForeignKeyDirection.ForeignKeyFromParent
				: ForeignKeyDirection.ForeignKeyToParent);

			XmlAttribute fkNode = node.Attributes["foreign-key"];
			if (fkNode != null)
				model.ForeignKeyName = fkNode.Value;

			XmlAttribute ukName = node.Attributes["property-ref"];
			if (ukName != null)
				model.ReferencedPropertyName = ukName.Value;

			// TODO NH: this is sort of redundant with the code below
			model.ReferencedEntityName = GetEntityName(node, mappings);

			XmlAttribute classNode = node.Attributes["class"];
			XmlAttribute nameNode = node.Attributes["name"];
			if (classNode != null && nameNode != null)
				model.Type = TypeFactory.OneToOne(
					ClassForNameChecked(classNode.Value, mappings, "could not find class: {0}"),
					model.ForeignKeyDirection,
					model.ReferencedPropertyName,
					model.IsLazy,
					nameNode.Value
					);
		}

		protected IDictionary GetMetas(XmlNode node)
		{
			IDictionary map = new Hashtable();

			foreach (XmlNode metaNode in node.SelectNodes(HbmConstants.nsMeta, namespaceManager))
			{
				string name = metaNode.Attributes["attribute"].Value;
				MetaAttribute meta = (MetaAttribute) map[name];
				if (meta == null)
				{
					meta = new MetaAttribute();
					map[name] = meta;
				}
				meta.AddValue(metaNode.InnerText);
			}

			return map;
		}

		protected System.Type GetPropertyType(XmlNode definingNode, System.Type containingType, string propertyName)
		{
			if (definingNode.Attributes["class"] != null)
				return ClassForNameChecked(definingNode.Attributes["class"].Value, mappings,
					"could not find class: {0}");
			else if (containingType == null)
				return null;

			string access = PropertyAccess(definingNode);

			return ReflectHelper.ReflectedPropertyClass(containingType, propertyName, access);
		}

		protected static bool IsCallable(XmlNode element)
		{
			XmlAttribute attrib = element.Attributes["callable"];
			return attrib != null && "true".Equals(attrib.Value);
		}

		protected static ExecuteUpdateResultCheckStyle GetResultCheckStyle(XmlNode element, bool callable)
		{
			XmlAttribute attr = element.Attributes["check"];
			if (attr == null)
				// use COUNT as the default.  This mimics the old behavior, although
				// NONE might be a better option moving forward in the case of callable
				return ExecuteUpdateResultCheckStyle.Count;
			return ExecuteUpdateResultCheckStyle.Parse(attr.Value);
		}

		protected void ParseFilter(XmlNode filterElement, IFilterable filterable)
		{
			string name = GetPropertyName(filterElement);
			if(name.IndexOf('.') > -1)
				throw new MappingException("Filter name can't contain the character '.'(point): " + name);
			string condition = filterElement.InnerText;
			if (condition == null || StringHelper.IsEmpty(condition.Trim()))
				if (filterElement.Attributes != null)
				{
					XmlAttribute propertyNameNode = filterElement.Attributes["condition"];
					condition = (propertyNameNode == null) ? null : propertyNameNode.Value;
				}

			//TODO: bad implementation, cos it depends upon ordering of mapping doc
			//      fixing this requires that Collection/PersistentClass gain access
			//      to the Mappings reference from Configuration (or the filterDefinitions
			//      map directly) sometime during Configuration.buildSessionFactory
			//      (after all the types/filter-defs are known and before building
			//      persisters).
			if (StringHelper.IsEmpty(condition))
				condition = mappings.GetFilterDefinition(name).DefaultFilterCondition;
			if (condition == null)
				throw new MappingException("no filter condition found for filter: " + name);
			log.Debug("Applying filter [" + name + "] as [" + condition + "]");
			filterable.AddFilter(name, condition);
		}

		protected void BindColumns(XmlNode node, SimpleValue model, bool isNullable, bool autoColumn,
			string propertyPath)
		{
			Table table = model.Table;
			//COLUMN(S)
			XmlAttribute columnAttribute = node.Attributes["column"];
			if (columnAttribute == null)
			{
				int count = 0;

				foreach (XmlNode columnElement in node.SelectNodes(HbmConstants.nsColumn, namespaceManager))
				{
					Column col = new Column(model.Type, count++);
					BindColumn(columnElement, col, isNullable);

					string name = columnElement.Attributes["name"].Value;
					col.Name = mappings.NamingStrategy.ColumnName(name);
					if (table != null)
						table.AddColumn(col);
					//table=null -> an association, fill it in later
					model.AddColumn(col);

					//column index
					BindIndex(columnElement.Attributes["index"], table, col);
					//column group index (although it can serve as a separate column index)
					BindIndex(node.Attributes["index"], table, col);

					BindUniqueKey(columnElement.Attributes["unique-key"], table, col);
					BindUniqueKey(node.Attributes["unique-key"], table, col);
				}
			}
			else
			{
				Column col = new Column(model.Type, 0);
				BindColumn(node, col, isNullable);
				col.Name = mappings.NamingStrategy.ColumnName(columnAttribute.Value);
				if (table != null)
					table.AddColumn(col);
				model.AddColumn(col);
				//column group index (although can serve as a separate column index)
				BindIndex(node.Attributes["index"], table, col);
				BindUniqueKey(node.Attributes["unique-key"], table, col);
			}

			if (autoColumn && model.ColumnSpan == 0)
			{
				Column col = new Column(model.Type, 0);
				BindColumn(node, col, isNullable);
				col.Name = mappings.NamingStrategy.PropertyToColumnName(propertyPath);
				model.Table.AddColumn(col);
				model.AddColumn(col);
				//column group index (although can serve as a separate column index)
				BindIndex(node.Attributes["index"], table, col);
				BindUniqueKey(node.Attributes["unique-key"], table, col);
			}
		}

		protected static void BindColumn(XmlNode node, Column model, bool isNullable)
		{
			XmlAttribute lengthNode = node.Attributes["length"];
			if (lengthNode != null)
				model.Length = int.Parse(lengthNode.Value);

			XmlAttribute nullNode = node.Attributes["not-null"];
			model.IsNullable = (nullNode != null) ? !StringHelper.BooleanValue(nullNode.Value) : isNullable;

			XmlAttribute unqNode = node.Attributes["unique"];
			model.IsUnique = unqNode != null && StringHelper.BooleanValue(unqNode.Value);

			XmlAttribute chkNode = node.Attributes["check"];
			model.CheckConstraint = chkNode != null ? chkNode.Value : string.Empty;

			XmlAttribute typeNode = node.Attributes["sql-type"];
			model.SqlType = (typeNode == null) ? null : typeNode.Value;
		}

		protected static void BindIndex(XmlAttribute indexAttribute, Table table, Column column)
		{
			if (indexAttribute != null && table != null)
			{
				StringTokenizer tokens = new StringTokenizer(indexAttribute.Value, ", ");
				foreach (string token in tokens)
					table.GetIndex(token).AddColumn(column);
			}
		}

		protected static void BindUniqueKey(XmlAttribute uniqueKeyAttribute, Table table, Column column)
		{
			if (uniqueKeyAttribute != null && table != null)
			{
				StringTokenizer tokens = new StringTokenizer(uniqueKeyAttribute.Value, ", ");
				foreach (string token in tokens)
					table.GetUniqueKey(token).AddColumn(column);
			}
		}

		protected static void InitLaziness(XmlNode node, ToOne fetchable, bool defaultLazy)
		{
			XmlAttribute lazyNode = node.Attributes["lazy"];
			if (lazyNode != null && "no-proxy".Equals(lazyNode.Value))
				//fetchable.UnwrapProxy = true;
				fetchable.IsLazy = true;
				//TODO: better to degrade to lazy="false" if uninstrumented
			else
				InitLaziness(node, fetchable, "proxy", defaultLazy);
		}

		protected static void InitLaziness(XmlNode node, IFetchable fetchable, string proxyVal, bool defaultLazy)
		{
			XmlAttribute lazyNode = node.Attributes["lazy"];
			bool isLazyTrue = lazyNode == null
				?
					defaultLazy && fetchable.IsLazy
				: //fetch="join" overrides default laziness
				lazyNode.Value.Equals(proxyVal); //fetch="join" overrides default laziness
			fetchable.IsLazy = isLazyTrue;
		}

		protected static void InitOuterJoinFetchSetting(XmlNode node, IFetchable model)
		{
			XmlAttribute fetchNode = node.Attributes["fetch"];
			FetchMode fetchStyle;
			bool lazy = true;

			if (fetchNode == null)
			{
				XmlAttribute jfNode = node.Attributes["outer-join"];
				if (jfNode == null)
					if ("many-to-many".Equals(node.Name))
					{
						//NOTE SPECIAL CASE:
						// default to join and non-lazy for the "second join"
						// of the many-to-many
						lazy = false;
						fetchStyle = FetchMode.Join;
					}
					else if ("one-to-one".Equals(node.Name))
					{
						//NOTE SPECIAL CASE:
						// one-to-one constrained=falase cannot be proxied,
						// so default to join and non-lazy
						lazy = ((OneToOne) model).IsConstrained;
						fetchStyle = lazy ? FetchMode.Default : FetchMode.Join;
					}
					else
						fetchStyle = FetchMode.Default;
				else
				{
					// use old (HB 2.1) defaults if outer-join is specified
					string eoj = jfNode.Value;
					if ("auto".Equals(eoj))
						fetchStyle = FetchMode.Default;
					else
					{
						bool join = "true".Equals(eoj);
						fetchStyle = join
							?
								FetchMode.Join
							:
								FetchMode.Select;
					}
				}
			}
			else
			{
				bool join = "join".Equals(fetchNode.Value);
				fetchStyle = join
					?
						FetchMode.Join
					:
						FetchMode.Select;
			}

			model.FetchMode = fetchStyle;
			model.IsLazy = lazy;
		}

		protected string PropertyAccess(XmlNode node)
		{
			XmlAttribute accessNode = node.Attributes["access"];
			return accessNode != null ? accessNode.Value : mappings.DefaultAccess;
		}

		protected IType GetTypeFromXML(XmlNode node)
		{
			IType type;

			IDictionary parameters = null;

			XmlAttribute typeAttribute = node.Attributes["type"];
			if (typeAttribute == null)
				typeAttribute = node.Attributes["id-type"]; //for an any
			string typeName;
			if (typeAttribute != null)
				typeName = typeAttribute.Value;
			else
			{
				XmlNode typeNode = node.SelectSingleNode(HbmConstants.nsType, namespaceManager);
				if (typeNode == null) //we will have to use reflection
					return null;
				XmlAttribute nameAttribute = typeNode.Attributes["name"]; //we know it exists because the schema validate it
				typeName = nameAttribute.Value;
				parameters = new Hashtable();
				foreach (XmlNode childNode in typeNode.ChildNodes)
					parameters.Add(childNode.Attributes["name"].Value,
						childNode.InnerText.Trim());
			}
			type = TypeFactory.HeuristicType(typeName, parameters);
			if (type == null)
				throw new MappingException("could not interpret type: " + typeAttribute.Value);
			return type;
		}

		private static string GetEntityName(XmlNode elem, Mappings model)
		{
			// TODO: H3.2 Implement real entityName (look at IEntityPersister for feature)
			//string entityName = XmlHelper.GetAttributeValue(elem, "entity-name");
			//return entityName == null ? GetClassName( elem.Attributes[ "class" ], model ) : entityName;
			XmlAttribute att = elem.Attributes["class"];

			if (att == null)
				return null;

			return GetClassName(att.Value, model);
		}

		protected XmlNodeList SelectNodes(XmlNode node, string xpath)
		{
			return node.SelectNodes(xpath, namespaceManager);
		}

		protected static string GetPropertyName(XmlNode node)
		{
			if (node.Attributes != null)
			{
				XmlAttribute propertyNameNode = node.Attributes["name"];
				return (propertyNameNode == null) ? null : propertyNameNode.Value;
			}
			return null;
		}

		protected static void LogMappedProperty(Mapping.Property property)
		{
			if (log.IsDebugEnabled)
			{
				string msg = "Mapped property: " + property.Name;
				string columns = Columns(property.Value);

				if (columns.Length > 0)
					msg += " -> " + columns;

				if (property.Type != null)
					msg += ", type: " + property.Type.Name;

				log.Debug(msg);
			}
		}

		protected static void BindIndex(string indexAttribute, Table table, Column column)
		{
			if (indexAttribute != null && table != null)
			{
				StringTokenizer tokens = new StringTokenizer(indexAttribute, ", ");
				foreach (string token in tokens)
					table.GetIndex(token).AddColumn(column);
			}
		}

		protected static void BindUniqueKey(string uniqueKeyAttribute, Table table, Column column)
		{
			if (uniqueKeyAttribute != null && table != null)
			{
				StringTokenizer tokens = new StringTokenizer(uniqueKeyAttribute, ", ");
				foreach (string token in tokens)
					table.GetUniqueKey(token).AddColumn(column);
			}
		}

		protected static void BindColumn(HbmColumn columnSchema, Column column, bool isNullable)
		{
			if (columnSchema.length != null)
				column.Length = int.Parse(columnSchema.length);

			column.IsNullable = columnSchema.notnullSpecified ? !columnSchema.notnull : isNullable;
			column.IsUnique = columnSchema.uniqueSpecified && columnSchema.unique;
			column.CheckConstraint = columnSchema.check ?? string.Empty;
			column.SqlType = columnSchema.sqltype;
		}
	}
}
