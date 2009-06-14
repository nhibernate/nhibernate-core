using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Engine;
using NHibernate.Mapping;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Cfg.XmlHbmBinding
{
	public abstract class ClassBinder : Binder
	{
		protected readonly Dialect.Dialect dialect;
		protected readonly XmlNamespaceManager namespaceManager;

		protected ClassBinder(Binder parent, XmlNamespaceManager namespaceManager, Dialect.Dialect dialect)
			: base(parent)
		{
			this.dialect = dialect;
			this.namespaceManager = namespaceManager;
		}

		protected ClassBinder(ClassBinder parent)
			: base(parent)
		{
			dialect = parent.dialect;
			namespaceManager = parent.namespaceManager;
		}

		protected void PropertiesFromXML(XmlNode node, PersistentClass model, IDictionary<string, MetaAttribute> inheritedMetas)
		{
			PropertiesFromXML(node, model, inheritedMetas, null, true, true, false);
		}

		protected void PropertiesFromXML(XmlNode node, PersistentClass model, IDictionary<string, MetaAttribute> inheritedMetas, UniqueKey uniqueKey, bool mutable, bool nullable, bool naturalId)
		{
			string entityName = model.EntityName;

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
					Mapping.Collection collection = collectionBinder.Create(name, subnode, entityName, propertyName, model,
					                                                        model.MappedClass, inheritedMetas);

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
					value = new OneToOne(table, model);
					BindOneToOne(subnode, (OneToOne) value);
				}
				else if ("property".Equals(name))
				{
					value = new SimpleValue(table);
					BindSimpleValue(subnode, (SimpleValue) value, true, propertyName);
				}
				else if ("component".Equals(name) || "dynamic-component".Equals(name))
				{
					string subpath = StringHelper.Qualify(entityName, propertyName);
					// NH: Modified from H2.1 to allow specifying the type explicitly using class attribute
					System.Type reflectedClass = GetPropertyType(subnode, model.MappedClass, propertyName);
					value = new Component(model);
					BindComponent(subnode, (Component) value, reflectedClass, entityName, propertyName, subpath, true, inheritedMetas);
				}
				else if ("join".Equals(name))
				{
					Join join = new Join();
					join.PersistentClass = model;
					BindJoin(subnode, join, inheritedMetas);
					model.AddJoin(join);
				}
				else if ("subclass".Equals(name))
					new SubclassBinder(this).HandleSubclass(model, subnode, inheritedMetas);

				else if ("joined-subclass".Equals(name))
					new JoinedSubclassBinder(this).HandleJoinedSubclass(model, subnode, inheritedMetas);

				else if ("union-subclass".Equals(name))
					new UnionSubclassBinder(this).HandleUnionSubclass(model, subnode, inheritedMetas);

				else if ("filter".Equals(name))
					ParseFilter(subnode, model);
				else if ("natural-id".Equals(name))
				{
					UniqueKey uk = new UniqueKey();
					uk.Name = "_UniqueKey";
					uk.Table = table;
					//by default, natural-ids are "immutable" (constant)

					bool mutableId = false;
					if (subnode.Attributes["mutable"] != null)
					{
						mutableId = "true".Equals(subnode.Attributes["mutable"]);						
					}

					PropertiesFromXML(subnode, model, inheritedMetas, uk, mutableId, false, true);
					table.AddUniqueKey(uk);
				}

				if (value != null)
				{
					Property property = CreateProperty(value, propertyName, model.ClassName, subnode, inheritedMetas);
					if (!mutable)
						property.IsUpdateable = false;
					if (naturalId)
						property.IsNaturalIdentifier = true;
					model.AddProperty(property);
					if (uniqueKey != null)
						uniqueKey.AddColumns(new SafetyEnumerable<Column>(property.ColumnIterator));
				}
			}
		}

		protected void BindClass(XmlNode node, IDecoratable classMapping, PersistentClass model, IDictionary<string, MetaAttribute> inheritedMetas)
		{
			// transfer an explicitly defined entity name
			// handle the lazy attribute
			XmlAttribute lazyNode = node.Attributes["lazy"];
			bool lazy = lazyNode == null ? mappings.DefaultLazy : "true".Equals(lazyNode.Value);
			// go ahead and set the lazy here, since pojo.proxy can override it.
			model.IsLazy = lazy;

			string entityName = (node.Attributes["entity-name"] != null ? node.Attributes["entity-name"].Value : null)
			                    ??
			                    ClassForNameChecked(node.Attributes["name"].Value, mappings, "persistent class {0} not found").
			                    	FullName;
			if (entityName == null)
			{
				throw new MappingException("Unable to determine entity name");
			}
			model.EntityName = entityName;

			BindPocoRepresentation(node, model);
			BindXmlRepresentation(node, model);
			BindMapRepresentation(node, model);

			BindPersistentClassCommonValues(node, classMapping, model, inheritedMetas);
		}

		private void BindPersistentClassCommonValues(XmlNode node, IDecoratable classMapping, PersistentClass model, IDictionary<string, MetaAttribute> inheritedMetas)
		{
			// DISCRIMINATOR
			XmlAttribute discriminatorNode = node.Attributes["discriminator-value"];
			model.DiscriminatorValue = (discriminatorNode == null) ? model.EntityName : discriminatorNode.Value;

			// DYNAMIC UPDATE
			XmlAttribute dynamicNode = node.Attributes["dynamic-update"];
			model.DynamicUpdate = (dynamicNode == null) ? false : "true".Equals(dynamicNode.Value);

			// DYNAMIC INSERT
			XmlAttribute insertNode = node.Attributes["dynamic-insert"];
			model.DynamicInsert = (insertNode == null) ? false : "true".Equals(insertNode.Value);

			// IMPORT
			// For entities, the EntityName is the key to find a persister
			// NH Different behavior: we are using the association between EntityName and its more certain implementation (AssemblyQualifiedName)
			// Dynamic entities have no class, reverts to EntityName. -AK
			string qualifiedName = model.MappedClass == null ? model.EntityName : model.MappedClass.AssemblyQualifiedName;
			mappings.AddImport(qualifiedName, model.EntityName);
			if (mappings.IsAutoImport && model.EntityName.IndexOf('.') > 0)
				mappings.AddImport(qualifiedName, StringHelper.Unqualify(model.EntityName));

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
			model.MetaAttributes = classMapping != null
			                       	? GetMetas(classMapping, inheritedMetas)
			                       	: GetMetas(node.SelectNodes(HbmConstants.nsMeta, namespaceManager), inheritedMetas);

			// PERSISTER
			XmlAttribute persisterNode = node.Attributes["persister"];
			if (persisterNode == null)
			{
				//persister = typeof( EntityPersister );
			}
			else
				model.EntityPersisterClass = ClassForNameChecked(persisterNode.Value, mappings,
				                                                 "could not instantiate persister class: {0}");

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

		private void BindMapRepresentation(XmlNode node, PersistentClass entity)
		{
			XmlNode tuplizer = LocateTuplizerDefinition(node, EntityMode.Map);
			if (tuplizer != null)
			{
				string tupClassName = FullQualifiedClassName(tuplizer.Attributes["class"].Value, mappings);
				entity.AddTuplizer(EntityMode.Map, tupClassName);
			}
		}

		private void BindXmlRepresentation(XmlNode node, PersistentClass entity)
		{
			string nodeName = null;
			XmlAttribute nodeAtt = node.Attributes["node"];
			if(nodeAtt != null)
				nodeName = nodeAtt.Value;
			if (nodeName == null)
				nodeName = StringHelper.Unqualify(entity.EntityName);
			entity.NodeName = nodeName;

			XmlNode tuplizer = LocateTuplizerDefinition(node, EntityMode.Xml);
			if (tuplizer != null)
			{
				string tupClassName = FullQualifiedClassName(tuplizer.Attributes["class"].Value, mappings);
				entity.AddTuplizer(EntityMode.Xml, tupClassName);
			}
		}

		private void BindPocoRepresentation(XmlNode node, PersistentClass entity)
		{
			string className = node.Attributes["name"] == null
			                   	? null
			                   	: ClassForNameChecked(node.Attributes["name"].Value, mappings, "persistent class {0} not found").
			                   	  	AssemblyQualifiedName;

			entity.ClassName = className;

			XmlAttribute proxyNode = node.Attributes["proxy"];
			if (proxyNode != null)
			{
				entity.ProxyInterfaceName = ClassForNameChecked(proxyNode.Value, mappings, "proxy class not found: {0}").AssemblyQualifiedName;
				entity.IsLazy = true;
			}
			else if (entity.IsLazy)
				entity.ProxyInterfaceName = className;

			XmlNode tuplizer = LocateTuplizerDefinition(node, EntityMode.Poco);
			if (tuplizer != null)
			{
				string tupClassName = FullQualifiedClassName(tuplizer.Attributes["class"].Value, mappings);
				entity.AddTuplizer(EntityMode.Poco, tupClassName);
			}
		}

		private XmlNode LocateTuplizerDefinition(XmlNode container, EntityMode mode)
		{
			string modeToFind = EntityModeHelper.ToString(mode);
			foreach (XmlNode node in container.SelectNodes(HbmConstants.nsTuplizer, namespaceManager))
			{
				if (modeToFind.Equals(node.Attributes["entity-mode"].Value))
					return node;
			}
			return null;
		}

		private void BindJoin(XmlNode node, Join join, IDictionary<string, MetaAttribute> inheritedMetas)
		{
			PersistentClass persistentClass = join.PersistentClass;
			String path = persistentClass.EntityName;

			// TABLENAME

			XmlAttribute schemaNode = node.Attributes["schema"];
			string schema = schemaNode == null ? mappings.SchemaName : schemaNode.Value;
			XmlAttribute catalogNode = node.Attributes["catalog"];
			string catalog = catalogNode == null ? mappings.CatalogName : catalogNode.Value;

			XmlAttribute actionNode = node.Attributes["schema-action"];
			string action = actionNode == null ? "all" : actionNode.Value;

			Table table = mappings.AddTable(schema, catalog, GetClassTableName(persistentClass, node), null, false, action);
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

			log.InfoFormat("Mapping class join: {0} -> {1}", persistentClass.EntityName, join.Table.Name);

			// KEY
			XmlNode keyNode = node.SelectSingleNode(HbmConstants.nsKey, namespaceManager);
			SimpleValue key = new DependantValue(table, persistentClass.Identifier);
			join.Key = key;
			if (keyNode.Attributes["on-delete"] != null)
				key.IsCascadeDeleteEnabled = "cascade".Equals(keyNode.Attributes["on-delete"].Value);
			BindSimpleValue(keyNode, key, false, persistentClass.EntityName);

			join.CreatePrimaryKey(dialect);
			join.CreateForeignKey();

			// PROPERTIES
			//PropertiesFromXML(node, persistentClass, mappings);
			foreach (XmlNode subnode in node.ChildNodes)
			{
				//I am only concerned with elements that are from the nhibernate namespace
				if (subnode.NamespaceURI != Configuration.MappingSchemaXMLNS)
					continue;

				string name = subnode.Name;
				XmlAttribute nameAttribute = subnode.Attributes["name"];
				string propertyName = nameAttribute == null ? null : nameAttribute.Value;
				IValue value = null;
				var collectionBinder = new CollectionBinder(this);
				if (collectionBinder.CanCreate(name))
				{
					Mapping.Collection collection = collectionBinder.Create(name, subnode, persistentClass.EntityName, propertyName,
					                                                        persistentClass, persistentClass.MappedClass,
					                                                        inheritedMetas);

					mappings.AddCollection(collection);
					value = collection;
				}
				else
				{
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
							BindComponent(subnode, (Component) value, join.PersistentClass.MappedClass, join.PersistentClass.ClassName,
							              propertyName, subpath, true, inheritedMetas);
							break;
					}
				}
				if (value != null)
				{
					var prop = CreateProperty(value, propertyName, persistentClass.MappedClass.AssemblyQualifiedName, subnode,
					                          inheritedMetas);
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

		private static Versioning.OptimisticLock GetOptimisticLockMode(XmlAttribute olAtt)
		{
			if (olAtt == null)
				return Versioning.OptimisticLock.Version;

			string olMode = olAtt.Value;

			if (olMode == null || "version".Equals(olMode))
				return Versioning.OptimisticLock.Version;
			else if ("dirty".Equals(olMode))
				return Versioning.OptimisticLock.Dirty;
			else if ("all".Equals(olMode))
				return Versioning.OptimisticLock.All;
			else if ("none".Equals(olMode))
				return Versioning.OptimisticLock.None;
			else
				throw new MappingException("Unsupported optimistic-lock style: " + olMode);
		}

		protected PersistentClass GetSuperclass(XmlNode subnode)
		{
			XmlAttribute extendsAttr = subnode.Attributes["extends"];
			if (extendsAttr == null)
			{
				throw new MappingException("'extends' attribute is not found.");
			}
			string extendsName = extendsAttr.Value;
			PersistentClass superModel = mappings.GetClass(extendsName);
			if(superModel == null)
			{
				string qualifiedExtendsName = FullClassName(extendsName, mappings);
				superModel = mappings.GetClass(qualifiedExtendsName);
			}
			if (superModel == null)
			{
				throw new MappingException("Cannot extend unmapped class: " + extendsName);
			}
			return superModel;
		}

		protected string GetClassTableName(PersistentClass model, XmlNode node)
		{
			XmlAttribute tableNameNode = node.Attributes["table"];
			if (tableNameNode == null)
				return mappings.NamingStrategy.ClassToTableName(model.EntityName);
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

				Dictionary<string, string> parms = new Dictionary<string, string>();

				// NOTE: While fixing NH-1061, a couple of lines similar to the following
				// were added to ClassIdBinder.GetGeneratorProperties().  It looks like
				// we may need it here too.  But I don't want to put it in just yet.
				/* if (model.Table.Schema != null)
				    parms.Add("schema", model.Table.Schema);
				else */
				if (mappings.SchemaName != null)
					parms.Add(Id.PersistentIdGeneratorParmsNames.Schema, dialect.QuoteForSchemaName(mappings.SchemaName));

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
			{
				model.NullValue = nullValueNode.Value;
			}
			else
			{
				model.NullValue = "assigned".Equals(model.IdentifierGeneratorStrategy) ? "undefined" : null;
			}
		}

		protected void BindComponent(XmlNode node, Component model, System.Type reflectedClass,
			string className, string parentProperty, string path, bool isNullable,
			IDictionary<string, MetaAttribute> inheritedMetas)
		{
			bool isIdentifierMapper = false;

			model.RoleName = path;

			inheritedMetas = GetMetas(node.SelectNodes(HbmConstants.nsMeta, namespaceManager), inheritedMetas);
			model.MetaAttributes = inheritedMetas;

			XmlAttribute classNode = node.Attributes["class"];

			if (classNode != null)
			{
				model.ComponentClass = ClassForNameChecked(
					classNode.Value, mappings,
					"component class not found: {0}");
				model.ComponentClassName = FullQualifiedClassName(classNode.Value, mappings);
				model.IsEmbedded = false;
			}
			else if ("dynamic-component".Equals(node.Name))
			{
				model.IsEmbedded = false;
				model.IsDynamic = true;
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

			string nodeName = (GetAttributeValue(node, "node") ?? GetAttributeValue(node, "name")) ?? model.Owner.NodeName;
			model.NodeName = nodeName;

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
					Mapping.Collection collection = binder.Create(name, subnode, className, subpath, model.Owner, model.ComponentClass,
					                                              inheritedMetas);

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
					value = new OneToOne(model.Table, model.Owner);
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
					value = new Component(model);
					BindComponent(subnode, (Component) value, subreflectedClass, className, propertyName, subpath, isNullable, inheritedMetas);
				}
				else if ("parent".Equals(name))
					model.ParentProperty = propertyName;

				if (value != null)
				{
					Property property = CreateProperty(value, propertyName, model.ComponentClassName, subnode, inheritedMetas);
					if (isIdentifierMapper)
					{
						property.IsInsertable = false;
						property.IsUpdateable = false;
					}
					model.AddProperty(property);
				}
			}
		}

		protected Property CreateProperty(IValue value, string propertyName, string className,
			XmlNode subnode, IDictionary<string, MetaAttribute> inheritedMetas)
		{
			if (string.IsNullOrEmpty(propertyName))
			{
				throw new MappingException(subnode.LocalName + " mapping must defined a name attribute [" + className + "]");
			}

			if (!string.IsNullOrEmpty(className) && value.IsSimpleValue)
				value.SetTypeUsingReflection(className, propertyName, PropertyAccess(subnode));

			// This is done here 'cos we might only know the type here (ugly!)
			var toOne = value as ToOne;
			if (toOne != null)
			{
				string propertyRef = toOne.ReferencedPropertyName;
				if (propertyRef != null)
					mappings.AddUniquePropertyReference(toOne.ReferencedEntityName, propertyRef);
			}

			value.CreateForeignKey();
			var prop = new Property {Value = value};
			BindProperty(subnode, prop, inheritedMetas);

			return prop;
		}

		protected void BindProperty(XmlNode node, Property property, IDictionary<string, MetaAttribute> inheritedMetas)
		{
			string propName = XmlHelper.GetAttributeValue(node, "name");
			property.Name = propName;
			//IType type = property.Value.Type;
			//if (type == null)
			//  throw new MappingException("could not determine a property type for: " + property.Name);

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

			property.MetaAttributes = GetMetas(node.SelectNodes(HbmConstants.nsMeta, namespaceManager), inheritedMetas);
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
			foreach (ISelectable col in val.ColumnIterator)
			{
				if (first)
					first = false;
				else
					columns.Append(", ");
				columns.Append(col.Text);
			}
			return columns.ToString();
		}

		//automatically makes a column with the default name if none is specified by XML
		protected void BindSimpleValue(XmlNode node, SimpleValue model, bool isNullable, string path)
		{
			BindSimpleValueType(node, model);

			BindColumnsOrFormula(node, model, path, isNullable);

			XmlAttribute fkNode = node.Attributes["foreign-key"];
			if (fkNode != null)
				model.ForeignKeyName = fkNode.Value;
		}

		private void BindSimpleValueType(XmlNode node, SimpleValue simpleValue)
		{
			string typeName = null;
			string originalTypeName = null;

			Dictionary<string, string> parameters = new Dictionary<string, string>();

			XmlAttribute typeNode = node.Attributes["type"];
			if (typeNode == null)
				typeNode = node.Attributes["id-type"]; //for an any
			if (typeNode != null)
				typeName = typeNode.Value;

			XmlNode typeChild = node.SelectSingleNode(HbmConstants.nsType, namespaceManager);
			if (typeName == null && typeChild != null)
			{
				originalTypeName = typeChild.Attributes["name"].Value;
				// NH: allow className completing it with assembly+namespace of the mapping doc.
				typeName = FullQualifiedClassName(originalTypeName, mappings);
				foreach (XmlNode childNode in typeChild.ChildNodes)
					parameters.Add(childNode.Attributes["name"].Value, childNode.InnerText.Trim());
			}

			BindTypeDef(typeName, originalTypeName, parameters, simpleValue);
		}

		protected void BindTypeDef(string typeName, string originalTypeName, Dictionary<string, string> parameters, SimpleValue simpleValue)
		{
			TypeDef typeDef = originalTypeName == null ? mappings.GetTypeDef(typeName) : mappings.GetTypeDef(originalTypeName);
			if (typeDef != null)
			{
				typeName = FullQualifiedClassName(typeDef.TypeClass, mappings);
				// parameters on the property mapping should
				// override parameters in the typedef
				Dictionary<string, string> allParameters = new Dictionary<string, string>(typeDef.Parameters);
				ArrayHelper.AddAll<string, string>(allParameters, parameters);
				parameters = allParameters;
			}

			if (!(parameters.Count == 0))
				simpleValue.TypeParameters = parameters;

			if (typeName != null)
			{
				if(NeedQualifiedClassName(typeName))
				{
					typeName = FullQualifiedClassName(typeName, mappings);
				}
				simpleValue.TypeName = typeName;
			}
		}

		private void BindColumnsOrFormula(XmlNode node, SimpleValue simpleValue, string path, bool isNullable)
		{
			var formula = GetFormula(node);
			if (formula != null)
			{
				var f = new Formula { FormulaString = formula };
				simpleValue.AddFormula(f);
			}
			else
				BindColumns(node, simpleValue, isNullable, true, path);
		}

		protected string GetFormula(XmlNode node)
		{
			XmlAttribute attribute = node.Attributes["formula"];
			if (attribute != null)
			{
				return attribute.InnerText;
			}
			else
			{
				var fcn = node.SelectSingleNode(HbmConstants.nsFormula, namespaceManager);
				return fcn != null && !string.IsNullOrEmpty(fcn.InnerText) ? fcn.InnerText : null;
			}
		}

		private void AddManyToOneSecondPass(ManyToOne manyToOne)
		{
			mappings.AddSecondPass(delegate(IDictionary<string, PersistentClass> persistentClasses)
			                       	{ manyToOne.CreatePropertyRefConstraints(persistentClasses); });
		}

		protected void BindManyToOne(XmlNode node, ManyToOne model, string defaultColumnName, bool isNullable)
		{
			BindColumnsOrFormula(node, model, defaultColumnName, isNullable);
			InitOuterJoinFetchSetting(node, model);
			InitLaziness(node, model, true);

			XmlAttribute ukName = node.Attributes["property-ref"];
			if (ukName != null)
				model.ReferencedPropertyName = ukName.Value;

			model.ReferencedEntityName = GetEntityName(node, mappings);

			string notFound = XmlHelper.GetAttributeValue(node, "not-found");
			model.IsIgnoreNotFound = "ignore".Equals(notFound);

			if (ukName != null && !model.IsIgnoreNotFound)
			{
				if (!"many-to-many".Equals(node.Name))
				{
					AddManyToOneSecondPass(model);
				}
			}

			XmlAttribute fkNode = node.Attributes["foreign-key"];
			if (fkNode != null)
				model.ForeignKeyName = fkNode.Value;
		}

		protected void BindAny(XmlNode node, Any model, bool isNullable)
		{
			IType idt = GetTypeFromXML(node);
			if (idt != null)
				model.IdentifierTypeName = idt.Name;

			XmlAttribute metaAttribute = node.Attributes["meta-type"];
			if (metaAttribute != null)
			{
				model.MetaType = metaAttribute.Value;
				XmlNodeList metaValues = node.SelectNodes(HbmConstants.nsMetaValue, namespaceManager);
				if (metaValues != null && metaValues.Count > 0)
				{
					IDictionary<object, string> values = new Dictionary<object, string>();
					IType metaType = TypeFactory.HeuristicType(model.MetaType);
					foreach (XmlNode metaValue in metaValues)
						try
						{
							object value = ((IDiscriminatorType)metaType).StringToObject(metaValue.Attributes["value"].Value);
							string entityName = GetClassName(metaValue.Attributes["class"].Value, mappings);
							values[value] = entityName;
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
					model.MetaValues = values.Count > 0 ? values : null;
				}
			}

			BindColumns(node, model, isNullable, false, null);
		}

		private void BindOneToOne(XmlNode node, OneToOne model)
		{
			//BindColumns( node, model, isNullable, false, null, mappings );

			XmlAttribute constrNode = node.Attributes["constrained"];
			bool constrained = constrNode != null && constrNode.Value.Equals("true");
			model.IsConstrained = constrained;

			model.ForeignKeyType = (constrained
				? ForeignKeyDirection.ForeignKeyFromParent
				: ForeignKeyDirection.ForeignKeyToParent);

			InitOuterJoinFetchSetting(node, model);
			InitLaziness(node, model, true);

			XmlAttribute fkNode = node.Attributes["foreign-key"];
			if (fkNode != null)
				model.ForeignKeyName = fkNode.Value;

			XmlAttribute ukName = node.Attributes["property-ref"];
			if (ukName != null)
				model.ReferencedPropertyName = ukName.Value;

			model.ReferencedEntityName = GetEntityName(node, mappings);
			model.PropertyName = node.Attributes["name"].Value;
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
					Column col = new Column();
					col.Value = model;
					col.TypeIndex = count++;
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
				Column col = new Column();
				col.Value = model;
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
				Column col = new Column();
				col.Value = model;
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
			XmlAttribute scaleNode = node.Attributes["scale"];
			if (scaleNode != null)
				model.Scale = int.Parse(scaleNode.Value);
			XmlAttribute precNode = node.Attributes["precision"];
			if (precNode != null)
				model.Precision = int.Parse(precNode.Value);

			XmlAttribute nullNode = node.Attributes["not-null"];
			model.IsNullable = (nullNode != null) ? !StringHelper.BooleanValue(nullNode.Value) : isNullable;

			XmlAttribute unqNode = node.Attributes["unique"];
			model.IsUnique = unqNode != null && StringHelper.BooleanValue(unqNode.Value);

			XmlAttribute chkNode = node.Attributes["check"];
			model.CheckConstraint = chkNode != null ? chkNode.Value : string.Empty;

			XmlAttribute typeNode = node.Attributes["sql-type"];
			model.SqlType = (typeNode == null) ? null : typeNode.Value;

			XmlAttribute defaAttribute = node.Attributes["default"];
			model.DefaultValue = (defaAttribute == null) ? null : defaAttribute.Value;
		}

		protected static void BindIndex(XmlAttribute indexAttribute, Table table, Column column)
		{
			if (indexAttribute != null && table != null)
			{
				var tokens = new StringTokenizer(indexAttribute.Value, ",", false);
				foreach (string token in tokens)
					table.GetOrCreateIndex(token.Trim()).AddColumn(column);
			}
		}

		protected static void BindUniqueKey(XmlAttribute uniqueKeyAttribute, Table table, Column column)
		{
			if (uniqueKeyAttribute != null && table != null)
			{
				var tokens = new StringTokenizer(uniqueKeyAttribute.Value, ",", false);
				foreach (string token in tokens)
					table.GetOrCreateUniqueKey(token.Trim()).AddColumn(column);
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

			IDictionary<string, string> parameters = null;

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
				parameters = new Dictionary<string, string>();
				foreach (XmlNode childNode in typeNode.ChildNodes)
					parameters.Add(childNode.Attributes["name"].Value,
						childNode.InnerText.Trim());
			}
			type = TypeFactory.HeuristicType(typeName, parameters);
			if (type == null)
				throw new MappingException("could not interpret type: " + typeAttribute.Value);
			return type;
		}

		protected static string GetEntityName(XmlNode elem, Mappings model)
		{
			string entityName = XmlHelper.GetAttributeValue(elem, "entity-name");
			string className = XmlHelper.GetAttributeValue(elem, "class");
			entityName = entityName
			             ?? (className == null ? null : StringHelper.GetFullClassname(FullQualifiedClassName(className, model)));

			return entityName;
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
				var tokens = new StringTokenizer(indexAttribute, ",", false);
				foreach (string token in tokens)
					table.GetOrCreateIndex(token.Trim()).AddColumn(column);
			}
		}

		protected static void BindUniqueKey(string uniqueKeyAttribute, Table table, Column column)
		{
			if (uniqueKeyAttribute != null && table != null)
			{
				var tokens = new StringTokenizer(uniqueKeyAttribute, ",", false);
				foreach (string token in tokens)
					table.GetOrCreateUniqueKey(token.Trim()).AddColumn(column);
			}
		}

		protected void BindColumn(HbmColumn columnSchema, Column column, bool isNullable)
		{
			column.Name = mappings.NamingStrategy.ColumnName(columnSchema.name);

			if (columnSchema.length != null)
				column.Length = int.Parse(columnSchema.length);
			if (columnSchema.scale != null)
				column.Scale = int.Parse(columnSchema.scale);
			if (columnSchema.precision != null)
				column.Precision = int.Parse(columnSchema.precision);

			column.IsNullable = columnSchema.notnullSpecified ? !columnSchema.notnull : isNullable;
			column.IsUnique = columnSchema.uniqueSpecified && columnSchema.unique;
			column.CheckConstraint = columnSchema.check ?? string.Empty;
			column.SqlType = columnSchema.sqltype;
			column.DefaultValue = columnSchema.@default;
			if (columnSchema.comment != null)
				column.Comment = string.Concat(columnSchema.comment.Text).Trim();
		}
	}
}
