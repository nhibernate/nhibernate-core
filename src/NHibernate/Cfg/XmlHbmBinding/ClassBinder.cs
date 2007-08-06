using System;
using System.Collections;
using System.Text;
using System.Xml;

using NHibernate.Engine;
using NHibernate.Mapping;
using NHibernate.Persister.Entity;
using NHibernate.Property;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Cfg.XmlHbmBinding
{
	public abstract class ClassBinder : Binder
	{
		public ClassBinder(Mappings mappings, XmlNamespaceManager namespaceManager)
			: base(mappings, namespaceManager)
		{
		}

		public ClassBinder(Binder parent)
			: base(parent)
		{
		}

		protected void PropertiesFromXML(XmlNode node, PersistentClass model)
		{
			Table table = model.Table;

			foreach (XmlNode subnode in node.ChildNodes)
			{
				//I am only concerned with elements that are from the nhibernate namespace
				if (subnode.NamespaceURI != Configuration.MappingSchemaXMLNS)
				{
					continue;
				}

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
					BindManyToOne(subnode, (ManyToOne)value, propertyName, true, mappings);
				}
				else if ("any".Equals(name))
				{
					value = new Any(table);
					BindAny(subnode, (Any)value, true, mappings);
				}
				else if ("one-to-one".Equals(name))
				{
					value = new OneToOne(table, model.Identifier);
					BindOneToOne(subnode, (OneToOne)value, true, mappings);
				}
				else if ("property".Equals(name))
				{
					value = new SimpleValue(table);
					BindSimpleValue(subnode, (SimpleValue)value, true, propertyName, mappings);
				}
				else if ("component".Equals(name) || "dynamic-component".Equals(name))
				{
					// NH: Modified from H2.1 to allow specifying the type explicitly using class attribute
					System.Type reflectedClass = GetPropertyType(subnode, mappings, model.MappedClass, propertyName);
					value = new Component(model);
					BindComponent(subnode, (Component)value, reflectedClass, model.Name, propertyName, true);
				}
				else if ("join".Equals(name))
				{
					Join join = new Join();
					join.PersistentClass = model;
					BindJoin(subnode, join);
					model.AddJoin(join);
				}
				else if ("subclass".Equals(name))
				{
					HandleSubclass(model, subnode);
				}
				else if ("joined-subclass".Equals(name))
				{
					HandleJoinedSubclass(model, subnode);
				}
				else if ("filter".Equals(name))
				{
					ParseFilter(subnode, model, mappings);
				}
				if (value != null)
				{
					model.AddProperty(CreateProperty(value, propertyName, model.MappedClass, subnode));
				}
			}
		}

		private void BindSubclass(XmlNode node, Subclass model)
		{
			BindClass(node, model);

			if (model.ClassPersisterClass == null)
			{
				model.RootClazz.ClassPersisterClass = typeof(SingleTableEntityPersister);
			}

			LogInfo("Mapping subclass: {0} -> {1}", model.Name, model.Table.Name);

			// properties
			PropertiesFromXML(node, model);
		}

		protected void HandleJoinedSubclass(PersistentClass model, XmlNode subnode)
		{
			JoinedSubclass subclass = new JoinedSubclass(model);
			BindJoinedSubclass(subnode, subclass);
			model.AddSubclass(subclass);
			mappings.AddClass(subclass);
		}

		protected void HandleSubclass(PersistentClass model, XmlNode subnode)
		{
			Subclass subclass = new SingleTableSubclass(model);
			BindSubclass(subnode, subclass);
			model.AddSubclass(subclass);
			mappings.AddClass(subclass);
		}

		private void BindJoinedSubclass(XmlNode node, JoinedSubclass model)
		{
			BindClass(node, model);

			// joined subclass
			if (model.ClassPersisterClass == null)
			{
				model.RootClazz.ClassPersisterClass = typeof(JoinedSubclassEntityPersister);
			}

			//table + schema names
			XmlAttribute schemaNode = node.Attributes["schema"];
			string schema = schemaNode == null ? mappings.SchemaName : schemaNode.Value;
			Table mytable = mappings.AddTable(schema, GetClassTableName(model, node));
			((ITableOwner)model).Table = mytable;

			LogInfo("Mapping joined-subclass: {0} -> {1}", model.Name, model.Table.Name);

			XmlNode keyNode = node.SelectSingleNode(HbmConstants.nsKey, namespaceManager);
			SimpleValue key = new DependentValue(mytable, model.Identifier);
			model.Key = key;
			BindSimpleValue(keyNode, key, false, model.Name, mappings);
			model.Key.Type = model.Identifier.Type;

			model.CreatePrimaryKey(dialect);

			if (!model.IsJoinedSubclass)
			{
				throw new MappingException(
					"Cannot map joined-subclass " + model.Name + " to table " +
					model.Table.Name + ", the same table as its base class.");
			}

			model.CreateForeignKey();

			// CHECK
			XmlAttribute chNode = node.Attributes["check"];
			if (chNode != null)
			{
				mytable.AddCheckConstraint(chNode.Value);
			}

			// properties
			PropertiesFromXML(node, model);
		}

		protected void BindClass(XmlNode node, PersistentClass model)
		{
			string className = node.Attributes["name"] == null ? null : FullClassName(node.Attributes["name"].Value, mappings);

			// CLASS
			model.MappedClass = ClassForFullNameChecked(className, "persistent class {0} not found");

			// PROXY INTERFACE
			XmlAttribute proxyNode = node.Attributes["proxy"];
			XmlAttribute lazyNode = node.Attributes["lazy"];
			bool lazy = lazyNode == null
							?
						mappings.DefaultLazy
							:
						"true".Equals(lazyNode.Value);

			// go ahead and set the lazy here, since pojo.proxy can override it.
			model.IsLazy = lazy;

			if (proxyNode != null)
			{
				model.ProxyInterface = ClassForNameChecked(proxyNode.Value, mappings,
														   "proxy class not found: {0}");
				model.IsLazy = true;
			}
			else if (model.IsLazy)
			{
				model.ProxyInterface = model.MappedClass;
			}

			// DISCRIMINATOR
			XmlAttribute discriminatorNode = node.Attributes["discriminator-value"];
			model.DiscriminatorValue = (discriminatorNode == null)
										? model.Name
										: discriminatorNode.Value;

			// DYNAMIC UPDATE
			XmlAttribute dynamicNode = node.Attributes["dynamic-update"];
			model.DynamicUpdate = (dynamicNode == null)
									? false
									:
								  "true".Equals(dynamicNode.Value);

			// DYNAMIC INSERT
			XmlAttribute insertNode = node.Attributes["dynamic-insert"];
			model.DynamicInsert = (insertNode == null)
									?
								  false
									:
								  "true".Equals(insertNode.Value);

			// IMPORT

			// we automatically want to add an import of the Assembly Qualified Name (includes version, 
			// culture, public-key) to the className supplied in the hbm.xml file.  The most common use-case
			// will have it contain the "FullClassname, AssemblyName", it might contain version, culture, 
			// public key, etc...) but should not assume it does.
			mappings.AddImport(model.MappedClass.AssemblyQualifiedName, StringHelper.GetFullClassname(className));

			// if we are supposed to auto-import the Class then add an import to get from the Classname
			// to the Assembly Qualified Class Name
			if (mappings.IsAutoImport)
			{
				mappings.AddImport(model.MappedClass.AssemblyQualifiedName, StringHelper.GetClassname(className));
			}

			// BATCH SIZE
			XmlAttribute batchNode = node.Attributes["batch-size"];
			if (batchNode != null)
			{
				model.BatchSize = int.Parse(batchNode.Value);
			}

			// SELECT BEFORE UPDATE
			XmlAttribute sbuNode = node.Attributes["select-before-update"];
			if (sbuNode != null)
			{
				model.SelectBeforeUpdate = "true".Equals(sbuNode.Value);
			}

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
			{
				model.ClassPersisterClass = ClassForNameChecked(
					persisterNode.Value, mappings,
					"could not instantiate persister class: {0}");
			}

			// CUSTOM SQL
			HandleCustomSQL(node, model);

			foreach (XmlNode syncNode in node.SelectNodes(HbmConstants.nsSynchronize, namespaceManager))
			{
				model.AddSynchronizedTable(XmlHelper.GetAttributeValue(syncNode, "table"));
			}
		}

		private void BindJoin(XmlNode node, Join join)
		{
			PersistentClass persistentClass = join.PersistentClass;
			String path = persistentClass.Name;

			// TABLENAME

			XmlAttribute schemaNode = node.Attributes["schema"];
			string schema = schemaNode == null ?
				mappings.SchemaName : schemaNode.Value;

			Table primaryTable = persistentClass.Table;
			Table table = mappings.AddTable(
				schema,
				GetClassTableName(persistentClass, node));
			join.Table = table;

			XmlAttribute fetchNode = node.Attributes["fetch"];
			if (fetchNode != null)
			{
				join.IsSequentialSelect = "select".Equals(fetchNode.Value);
			}

			XmlAttribute invNode = node.Attributes["inverse"];
			if (invNode != null)
			{
				join.IsInverse = "true".Equals(invNode.Value);
			}

			XmlAttribute nullNode = node.Attributes["optional"];
			if (nullNode != null)
			{
				join.IsOptional = "true".Equals(nullNode.Value);
			}

			LogInfo("Mapping class join: {0} -> {1}", persistentClass.Name, join.Table.Name);

			// KEY
			XmlNode keyNode = node.SelectSingleNode(HbmConstants.nsKey, namespaceManager);
			SimpleValue key = new DependentValue(table, persistentClass.Identifier);
			join.Key = key;
			// key.SetCascadeDeleteEnabled("cascade".Equals(keyNode.Attributes["on-delete"].Value));
			BindSimpleValue(keyNode, key, false, persistentClass.Name, mappings);
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
						BindManyToOne(subnode, (ManyToOne)value, propertyName, true, mappings);
						break;
					case "any":
						value = new Any(table);
						BindAny(subnode, (Any)value, true, mappings);
						break;
					case "property":
						value = new SimpleValue(table);
						BindSimpleValue(subnode, (SimpleValue)value, true, propertyName, mappings);
						break;
					case "component":
					case "dynamic-component":
						string subpath = StringHelper.Qualify(path, propertyName);
						value = new Component(join);
						BindComponent(
							subnode,
							(Component)value,
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
			{
				model.LoaderName = XmlHelper.GetAttributeValue(element, "query-ref");
			}
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
			{
				return OptimisticLockMode.Version;
			}

			string olMode = olAtt.Value;

			if (olMode == null || "version".Equals(olMode))
			{
				return OptimisticLockMode.Version;
			}
			else if ("dirty".Equals(olMode))
			{
				return OptimisticLockMode.Dirty;
			}
			else if ("all".Equals(olMode))
			{
				return OptimisticLockMode.All;
			}
			else if ("none".Equals(olMode))
			{
				return OptimisticLockMode.None;
			}
			else
			{
				throw new MappingException("Unsupported optimistic-lock style: " + olMode);
			}
		}

		protected PersistentClass GetSuperclass(XmlNode subnode)
		{
			XmlAttribute extendsAttr = subnode.Attributes["extends"];
			if (extendsAttr == null)
			{
				throw new MappingException("'extends' attribute is not found.");
			}
			String extendsValue = FullClassName(extendsAttr.Value, mappings);
			System.Type superclass = ClassForFullNameChecked(extendsValue,
															 "extended class not found: {0}");
			PersistentClass superModel = mappings.GetClass(superclass);

			if (superModel == null)
			{
				throw new MappingException("Cannot extend unmapped class: " + extendsValue);
			}
			return superModel;
		}

		protected string GetClassTableName(PersistentClass model, XmlNode node)
		{
			XmlAttribute tableNameNode = node.Attributes["table"];
			if (tableNameNode == null)
			{
				return mappings.NamingStrategy.ClassToTableName(model.Name);
			}
			else
			{
				return mappings.NamingStrategy.TableName(tableNameNode.Value);
			}
		}

		protected void BindSimpleValue(XmlNode node, SimpleValue simpleValue, bool isNullable, string path)
		{
			BindSimpleValue(node, simpleValue, isNullable, path, mappings);
		}

		protected void MakeIdentifier(XmlNode node, SimpleValue model)
		{
			//GENERATOR

			XmlNode subnode = node.SelectSingleNode(HbmConstants.nsGenerator, namespaceManager);
			if (subnode != null)
			{
				if (subnode.Attributes["class"] == null)
				{
					throw new MappingException("no class given for generator");
				}

				model.IdentifierGeneratorStrategy = subnode.Attributes["class"].Value;

				IDictionary parms = new Hashtable();

				if (mappings.SchemaName != null)
				{
					parms.Add("schema", dialect.QuoteForSchemaName(mappings.SchemaName));
				}

				parms.Add("target_table", model.Table.GetQuotedName(dialect));

				foreach (Column col in model.ColumnCollection)
				{
					parms.Add("target_column", col);
					break;
				}

				foreach (XmlNode childNode in subnode.SelectNodes(HbmConstants.nsParam, namespaceManager))
				{
					parms.Add(
						childNode.Attributes["name"].Value,
						childNode.InnerText
						);
				}

				model.IdentifierGeneratorProperties = parms;
			}

			model.Table.SetIdentifierValue(model);

			//unsaved-value
			XmlAttribute nullValueNode = node.Attributes["unsaved-value"];
			if (nullValueNode != null)
			{
				model.NullValue = nullValueNode.Value;
			}
			else
			{
				if (model.IdentifierGeneratorStrategy == "assigned")
				{
					// TODO: H3 has model.setNullValue("undefined") here, but
					// NH doesn't (yet) allow "undefined" for id unsaved-value,
					// so we use "null" here
					model.NullValue = "null";
				}
				else
				{
					model.NullValue = null;
				}
			}
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
				{
					continue;
				}

				string name = subnode.LocalName; //.Name;
				string propertyName = GetPropertyName(subnode);
				string subpath = propertyName == null ? null : StringHelper.Qualify(path, propertyName);

				IValue value = null;

				CollectionBinder binder = new CollectionBinder(mappings, namespaceManager);

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
					BindManyToOne(subnode, (ManyToOne)value, subpath, isNullable, mappings);
				}
				else if ("one-to-one".Equals(name))
				{
					value = new OneToOne(model.Table, model.Owner.Identifier);
					BindOneToOne(subnode, (OneToOne)value, isNullable, mappings);
				}
				else if ("any".Equals(name))
				{
					value = new Any(model.Table);
					BindAny(subnode, (Any)value, isNullable, mappings);
				}
				else if ("property".Equals(name) || "key-property".Equals(name))
				{
					value = new SimpleValue(model.Table);
					BindSimpleValue(subnode, (SimpleValue)value, isNullable, subpath, mappings);
				}
				else if ("component".Equals(name) || "dynamic-component".Equals(name) || "nested-composite-element".Equals(name))
				{
					System.Type subreflectedClass = model.ComponentClass == null
														?
													null
														:
													GetPropertyType(subnode, mappings, model.ComponentClass, propertyName);
					value = (model.Owner != null)
								?
							new Component(model.Owner)
								: // a class component
							new Component(model.Table); // a composite element
					BindComponent(subnode, (Component)value, subreflectedClass, className, subpath, isNullable);
				}
				else if ("parent".Equals(name))
				{
					model.ParentProperty = propertyName;
				}

				if (value != null)
				{
					model.AddProperty(CreateProperty(value, propertyName, model.ComponentClass, subnode));
				}
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
			{
				componentType = new DynamicComponentType(names, types, nullabilities, joinedFetch, cascade);
			}
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
					{
						foundCustomAccessor = true;
					}
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
			{
				((SimpleValue)value).SetTypeByReflection(parentClass, propertyName, PropertyAccess(subnode, mappings));
			}

			// This is done here 'cos we might only know the type here (ugly!)
			if (value is ToOne)
			{
				ToOne toOne = (ToOne)value;
				string propertyRef = toOne.ReferencedPropertyName;
				if (propertyRef != null)
				{
					mappings.AddUniquePropertyReference(((EntityType)value.Type).AssociatedClass, propertyRef);
				}
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
			{
				throw new MappingException("could not determine a property type for: " + property.Name);
			}

			property.PropertyAccessorName = PropertyAccess(node, mappings);

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
				{
					if (insertNode == null)
					{
						// insertable simply because that is the user did not specify
						// anything; just override it
						property.IsInsertable = false;
					}
					else
					{
						// the user specifically supplied insert="true",
						// which constitutes an illegal combo
						throw new MappingException(
								"cannot specify both insert=\"true\" and generated=\"" + generationName +
								"\" for property: " + propName);
					}
				}

				// properties generated on update can never be updateable...
				if (property.IsUpdateable && generation == PropertyGeneration.Always)
				{
					if (updateNode == null)
					{
						// updateable only because the user did not specify 
						// anything; just override it
						property.IsUpdateable = false;
					}
					else
					{
						// the user specifically supplied update="true",
						// which constitutes an illegal combo
						throw new MappingException(
								"cannot specify both update=\"true\" and generated=\"" + generationName +
								"\" for property: " + propName);
					}
				}
			}


			if (log.IsDebugEnabled)
			{
				string msg = "Mapped property: " + property.Name;
				string columns = Columns(property.Value);
				if (columns.Length > 0)
				{
					msg += " -> " + columns;
				}
				if (property.Type != null)
				{
					msg += ", type: " + property.Type.Name;
				}
				log.Debug(msg);
			}

			property.MetaAttributes = GetMetas(node);
		}

		private static PropertyGeneration ParsePropertyGeneration(string name)
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
				{
					first = false;
				}
				else
				{
					columns.Append(", ");
				}
				columns.Append(col.Text);
			}
			return columns.ToString();
		}
	}
}