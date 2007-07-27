using System;
using System.Xml;

using NHibernate.Engine;
using NHibernate.Mapping;
using NHibernate.Persister.Entity;
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

		protected static void PropertiesFromXML(XmlNode node, PersistentClass model, Mappings mappings)
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

				CollectionType collectType = CollectionType.CollectionTypeFromString(name);
				IValue value = null;
				if (collectType != null)
				{
					Mapping.Collection collection =
						collectType.Create(subnode, model.Name, propertyName, model, model.MappedClass, mappings);
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
					BindComponent(subnode, (Component)value, reflectedClass, model.Name, propertyName, true, mappings);
				}
				else if ("join".Equals(name))
				{
					Join join = new Join();
					join.PersistentClass = model;
					BindJoin(subnode, join, mappings);
					model.AddJoin(join);
				}
				else if ("subclass".Equals(name))
				{
					HandleSubclass(model, mappings, subnode);
				}
				else if ("joined-subclass".Equals(name))
				{
					HandleJoinedSubclass(model, mappings, subnode);
				}
				else if ("filter".Equals(name))
				{
					ParseFilter(subnode, model, mappings);
				}
				if (value != null)
				{
					model.AddProperty(CreateProperty(value, propertyName, model.MappedClass, subnode, mappings));
				}
			}
		}

		protected static void BindSubclass(XmlNode node, Subclass model, Mappings mappings)
		{
			BindClass(node, model, mappings);

			if (model.ClassPersisterClass == null)
			{
				model.RootClazz.ClassPersisterClass = typeof(SingleTableEntityPersister);
			}

			log.Info("Mapping subclass: " + model.Name + " -> " + model.Table.Name);

			// properties
			PropertiesFromXML(node, model, mappings);
		}

		protected static void HandleJoinedSubclass(PersistentClass model, Mappings mappings, XmlNode subnode)
		{
			JoinedSubclass subclass = new JoinedSubclass(model);
			BindJoinedSubclass(subnode, subclass, mappings);
			model.AddSubclass(subclass);
			mappings.AddClass(subclass);
		}

		protected static void HandleSubclass(PersistentClass model, Mappings mappings, XmlNode subnode)
		{
			Subclass subclass = new SingleTableSubclass(model);
			BindSubclass(subnode, subclass, mappings);
			model.AddSubclass(subclass);
			mappings.AddClass(subclass);
		}

		protected static void BindJoinedSubclass(XmlNode node, JoinedSubclass model, Mappings mappings)
		{
			BindClass(node, model, mappings);

			// joined subclass
			if (model.ClassPersisterClass == null)
			{
				model.RootClazz.ClassPersisterClass = typeof(JoinedSubclassEntityPersister);
			}

			//table + schema names
			XmlAttribute schemaNode = node.Attributes["schema"];
			string schema = schemaNode == null ? mappings.SchemaName : schemaNode.Value;
			Table mytable = mappings.AddTable(schema, GetClassTableName(model, node, mappings));
			((ITableOwner)model).Table = mytable;

			log.Info("Mapping joined-subclass: " + model.Name + " -> " + model.Table.Name);

			XmlNode keyNode = node.SelectSingleNode(HbmConstants.nsKey, NamespaceManager);
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
			PropertiesFromXML(node, model, mappings);
		}

		public static void BindClass(XmlNode node, PersistentClass model, Mappings mappings)
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

			foreach (XmlNode syncNode in node.SelectNodes(HbmConstants.nsSynchronize, NamespaceManager))
			{
				model.AddSynchronizedTable(XmlHelper.GetAttributeValue(syncNode, "table"));
			}
		}

		public static void BindJoin(XmlNode node, Join join, Mappings mappings)
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
				GetClassTableName(persistentClass, node, mappings));
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

			log.Info(
				"Mapping class join: " + persistentClass.Name +
				" -> " + join.Table.Name
				);

			// KEY
			XmlNode keyNode = node.SelectSingleNode(HbmConstants.nsKey, NamespaceManager);
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
							true,
							mappings);
						break;
				}

				if (value != null)
				{
					NHibernate.Mapping.Property prop = CreateProperty(value, propertyName, persistentClass.MappedClass, subnode, mappings);
					prop.IsOptional = join.IsOptional;
					join.AddProperty(prop);
				}
			}

			// CUSTOM SQL
			HandleCustomSQL(node, join);
		}

		protected static void HandleCustomSQL(XmlNode node, PersistentClass model)
		{
			XmlNode element = node.SelectSingleNode(HbmConstants.nsSqlInsert, NamespaceManager);

			if (element != null)
			{
				bool callable = IsCallable(element);
				model.SetCustomSQLInsert(element.InnerText.Trim(), callable, GetResultCheckStyle(element, callable));
			}

			element = node.SelectSingleNode(HbmConstants.nsSqlDelete, NamespaceManager);
			if (element != null)
			{
				bool callable = IsCallable(element);
				model.SetCustomSQLDelete(element.InnerText.Trim(), callable, GetResultCheckStyle(element, callable));
			}

			element = node.SelectSingleNode(HbmConstants.nsSqlUpdate, NamespaceManager);
			if (element != null)
			{
				bool callable = IsCallable(element);
				model.SetCustomSQLUpdate(element.InnerText.Trim(), callable, GetResultCheckStyle(element, callable));
			}

			element = node.SelectSingleNode(HbmConstants.nsLoader, NamespaceManager);
			if (element != null)
			{
				model.LoaderName = XmlHelper.GetAttributeValue(element, "query-ref");
			}
		}

		protected static void HandleCustomSQL(XmlNode node, Join model)
		{
			XmlNode element = node.SelectSingleNode(HbmConstants.nsSqlInsert, NamespaceManager);
			if (element != null)
			{
				bool callable = IsCallable(element);
				model.SetCustomSQLInsert(element.InnerText.Trim(), callable, GetResultCheckStyle(element, callable));
			}

			element = node.SelectSingleNode(HbmConstants.nsSqlDelete, NamespaceManager);
			if (element != null)
			{
				bool callable = IsCallable(element);
				model.SetCustomSQLDelete(element.InnerText.Trim(), callable, GetResultCheckStyle(element, callable));
			}

			element = node.SelectSingleNode(HbmConstants.nsSqlUpdate, NamespaceManager);
			if (element != null)
			{
				bool callable = IsCallable(element);
				model.SetCustomSQLUpdate(element.InnerText.Trim(), callable, GetResultCheckStyle(element, callable));
			}
		}

		protected static OptimisticLockMode GetOptimisticLockMode(XmlAttribute olAtt)
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


		protected static PersistentClass GetSuperclass(Mappings model, XmlNode subnode)
		{
			XmlAttribute extendsAttr = subnode.Attributes["extends"];
			if (extendsAttr == null)
			{
				throw new MappingException("'extends' attribute is not found.");
			}
			String extendsValue = FullClassName(extendsAttr.Value, model);
			System.Type superclass = ClassForFullNameChecked(extendsValue,
															 "extended class not found: {0}");
			PersistentClass superModel = model.GetClass(superclass);

			if (superModel == null)
			{
				throw new MappingException("Cannot extend unmapped class: " + extendsValue);
			}
			return superModel;
		}

		protected static string GetClassTableName(PersistentClass model, XmlNode node, Mappings mappings)
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

	}
}