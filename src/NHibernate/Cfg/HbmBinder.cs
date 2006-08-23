using System;
using System.Collections;
using System.Text;
using System.Xml;

using log4net;

using NHibernate.Cache;
using NHibernate.Engine;
using NHibernate.Mapping;
using NHibernate.Persister;
using NHibernate.Persister.Entity;
using NHibernate.Property;
using NHibernate.Type;
using NHibernate.Util;

using Array = NHibernate.Mapping.Array;

namespace NHibernate.Cfg
{
	public class HbmBinder
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(HbmBinder));

		// Made internal to be accessible from Cfg.CollectionSecondPass
		internal static XmlNamespaceManager nsmgr;

		internal static Dialect.Dialect dialect;

		/// <summary>
		/// Converts a partial class name into a fully qualified one
		/// </summary>
		/// <param name="className"></param>
		/// <param name="mapping"></param>
		/// <returns></returns>
		public static string FullClassName(string className, Mappings mapping)
		{
			if (className == null)
			{
				return null;
			}

			return TypeNameParser.Parse(className, mapping.DefaultNamespace, mapping.DefaultAssembly)
				.ToString();
		}

		/// <summary>
		/// Attempts to find a type by its full name. Throws a MappingException using
		/// the provided <c>errorMessage</c> in case of failure.
		/// </summary>
		/// <param name="fullName">name of the class to find</param>
		/// <param name="errorMessage">Error message to use for
		/// the <see cref="MappingException" /> in case of failure. Should contain
		/// the <c>{0}</c> formatting placeholder.</param>
		/// <returns></returns>
		/// <exception cref="MappingException">
		/// Thrown when there is an error loading the class.
		/// </exception>
		private static System.Type ClassForFullNameChecked(string fullName, string errorMessage)
		{
			try
			{
				return ReflectHelper.ClassForName(fullName);
			}
			catch (Exception e)
			{
				throw new MappingException(String.Format(errorMessage, fullName), e);
			}
		}

		/// <summary>
		/// Similar to <see cref="ClassForFullNameChecked" />, but handles short class names
		/// by calling <see cref="FullClassName" />.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="mappings"></param>
		/// <param name="errorMessage"></param>
		/// <returns></returns>
		private static System.Type ClassForNameChecked(string name, Mappings mappings, string errorMessage)
		{
			return ClassForFullNameChecked(FullClassName(name, mappings), errorMessage);
		}

		public static void BindClass(XmlNode node, PersistentClass model, Mappings mappings)
		{
			string className = node.Attributes["name"] == null ? null : FullClassName(node.Attributes["name"].Value, mappings);

			// CLASS
			model.MappedClass = ClassForFullNameChecked(className, "persistent class {0} not found");

			// PROXY INTERFACE
			XmlAttribute proxyNode = node.Attributes["proxy"];
			XmlAttribute lazyNode = node.Attributes["lazy"];
			bool lazy = lazyNode == null ?
				mappings.DefaultLazy :
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
				? false :
				"true".Equals(dynamicNode.Value);

			// DYNAMIC INSERT
			XmlAttribute insertNode = node.Attributes["dynamic-insert"];
			model.DynamicInsert = (insertNode == null) ?
				false :
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

			foreach (XmlNode syncNode in node.SelectNodes(HbmConstants.nsSynchronize, nsmgr))
			{
				model.AddSynchronizedTable(XmlHelper.GetAttributeValue(syncNode, "table"));
			}
		}

		public static void BindSubclass(XmlNode node, Subclass model, Mappings mappings)
		{
			BindClass(node, model, mappings);

			if (model.ClassPersisterClass == null)
			{
				model.RootClazz.ClassPersisterClass = typeof(SingleTableEntityPersister);
			}

			model.Table = model.Superclass.Table;

			log.Info("Mapping subclass: " + model.Name + " -> " + model.Table.Name);

			// properties
			PropertiesFromXML(node, model, mappings);
		}

		private static string GetClassTableName(PersistentClass model, XmlNode node, Mappings mappings)
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

		public static void BindJoinedSubclass(XmlNode node, Subclass model, Mappings mappings)
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
			model.Table = mytable;

			log.Info("Mapping joined-subclass: " + model.Name + " -> " + model.Table.Name);

			XmlNode keyNode = node.SelectSingleNode(HbmConstants.nsKey, nsmgr);
			SimpleValue key = new SimpleValue(mytable);
			model.Key = key;
			BindSimpleValue(keyNode, key, false, model.Name, mappings);

			model.Key.Type = model.Identifier.Type;
			model.CreatePrimaryKey(dialect);
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

		public static void BindRootClass(XmlNode node, RootClass model, Mappings mappings)
		{
			BindClass(node, model, mappings);

			//TABLENAME
			XmlAttribute schemaNode = node.Attributes["schema"];
			string schema = schemaNode == null ? mappings.SchemaName : schemaNode.Value;
			Table table = mappings.AddTable(schema, GetClassTableName(model, node, mappings));
			model.Table = table;

			log.Info("Mapping class: " + model.Name + " -> " + model.Table.Name);

			//MUTABLE
			XmlAttribute mutableNode = node.Attributes["mutable"];
			model.IsMutable = (mutableNode == null) || mutableNode.Value.Equals("true");

			//WHERE
			XmlAttribute whereNode = node.Attributes["where"];
			if (whereNode != null)
			{
				model.Where = whereNode.Value;
			}

			//CHECK
			XmlAttribute checkNode = node.Attributes["check"];
			if (checkNode != null)
			{
				table.AddCheckConstraint(checkNode.Value);
			}

			//POLYMORPHISM
			XmlAttribute polyNode = node.Attributes["polymorphism"];
			model.IsExplicitPolymorphism = (polyNode != null) && polyNode.Value.Equals("explicit");

			foreach (XmlNode subnode in node.ChildNodes)
			{
				string name = subnode.LocalName; //Name;
				string propertyName = GetPropertyName(subnode);

				//I am only concerned with elements that are from the nhibernate namespace
				if (subnode.NamespaceURI != Configuration.MappingSchemaXMLNS)
				{
					continue;
				}

				switch (name)
				{
					case "id":
						SimpleValue id = new SimpleValue(table);
						model.Identifier = id;

						if (propertyName == null)
						{
							BindSimpleValue(subnode, id, false, RootClass.DefaultIdentifierColumnName, mappings);
							if (id.Type == null)
							{
								throw new MappingException("must specify an identifier type: " + model.MappedClass.Name);
							}
							//model.IdentifierProperty = null;
						}
						else
						{
							BindSimpleValue(subnode, id, false, propertyName, mappings);
							id.SetTypeByReflection(model.MappedClass, propertyName, PropertyAccess(subnode, mappings));
							Mapping.Property prop = new Mapping.Property(id);
							BindProperty(subnode, prop, mappings);
							model.IdentifierProperty = prop;
						}

						if (id.Type.ReturnedClass.IsArray)
						{
							throw new MappingException("illegal use of an array as an identifier (arrays don't reimplement equals)");
						}

						MakeIdentifier(subnode, id, mappings);
						break;

					case "composite-id":
						Component compId = new Component(model);
						model.Identifier = compId;
						if (propertyName == null)
						{
							BindComponent(subnode, compId, null, model.Name, "id", false, mappings);
							model.HasEmbeddedIdentifier = compId.IsEmbedded;
							//model.IdentifierProperty = null;
						}
						else
						{
							System.Type reflectedClass = GetPropertyType(subnode, mappings, model.MappedClass, propertyName);
							BindComponent(subnode, compId, reflectedClass, model.Name, propertyName, false, mappings);
							Mapping.Property prop = new Mapping.Property(compId);
							BindProperty(subnode, prop, mappings);
							model.IdentifierProperty = prop;
						}
						MakeIdentifier(subnode, compId, mappings);

						System.Type compIdClass = compId.ComponentClass;
						if (!ReflectHelper.OverridesEquals(compIdClass))
						{
							throw new MappingException(
								"composite-id class must override Equals(): " + compIdClass.FullName
								);
						}

						if (!ReflectHelper.OverridesGetHashCode(compIdClass))
						{
							throw new MappingException(
								"composite-id class must override GetHashCode(): " + compIdClass.FullName
								);
						}

						// Serializability check not ported
						break;

					case "version":
					case "timestamp":
						//VERSION
						SimpleValue val = new SimpleValue(table);
						BindSimpleValue(subnode, val, false, propertyName, mappings);
						if (val.Type == null)
						{
							val.Type = (("version".Equals(name)) ? NHibernateUtil.Int32 : NHibernateUtil.Timestamp);
						}
						Mapping.Property timestampProp = new Mapping.Property(val);
						BindProperty(subnode, timestampProp, mappings);
						MakeVersion(subnode, val);
						model.Version = timestampProp;
						model.AddNewProperty(timestampProp);
						break;

					case "discriminator":
						//DISCRIMINATOR
						SimpleValue discrim = new SimpleValue(table);
						model.Discriminator = discrim;
						BindSimpleValue(subnode, discrim, false, RootClass.DefaultDiscriminatorColumnName, mappings);
						if (discrim.Type == null)
						{
							discrim.Type = NHibernateUtil.String;
							foreach (Column col in discrim.ColumnCollection)
							{
								col.Type = NHibernateUtil.String;
								break;
							}
						}
						model.IsPolymorphic = true;
						if (subnode.Attributes["force"] != null && "true".Equals(subnode.Attributes["force"].Value))
						{
							model.IsForceDiscriminator = true;
						}
						if (subnode.Attributes["insert"] != null && "false".Equals(subnode.Attributes["insert"].Value))
						{
							model.IsDiscriminatorInsertable = false;
						}
						break;

					case "jcs-cache":
					case "cache":
						string className = model.MappedClass.FullName;
						ICacheConcurrencyStrategy cache = CacheFactory.CreateCache(subnode, className, model.IsMutable);
						mappings.AddCache(className, cache);
						model.Cache = cache;

						break;
				}
			}

			model.CreatePrimaryKey(dialect);

			PropertiesFromXML(node, model, mappings);
		}

		public static void BindColumns(XmlNode node, SimpleValue model, bool isNullable, bool autoColumn, string propertyPath, Mappings mappings)
		{
			//COLUMN(S)
			XmlAttribute columnAttribute = node.Attributes["column"];
			if (columnAttribute == null)
			{
				int count = 0;
				Table table = model.Table;

				foreach (XmlNode columnElement in node.SelectNodes(HbmConstants.nsColumn, nsmgr))
				{
					Column col = new Column(model.Type, count++);
					BindColumn(columnElement, col, isNullable);

					string name = columnElement.Attributes["name"].Value;
					col.Name = mappings.NamingStrategy.ColumnName(name);
					if (table != null)
					{
						table.AddColumn(col);
					}
					//table=null -> an association, fill it in later
					model.AddColumn(col);

					//column index
					XmlAttribute indexNode = columnElement.Attributes["index"];
					if (indexNode != null && table != null)
					{
						table.GetIndex(indexNode.Value).AddColumn(col);
					}

					//column group index (although it can serve as a separate column index)
					XmlAttribute parentElementIndexAttr = node.Attributes["index"];
					if (parentElementIndexAttr != null && table != null)
					{
						table.GetIndex(parentElementIndexAttr.Value).AddColumn(col);
					}
					XmlAttribute uniqueNode = columnElement.Attributes["unique-key"];
					if (uniqueNode != null && table != null)
					{
						table.GetUniqueKey(uniqueNode.Value).AddColumn(col);
					}
				}
			}
			else
			{
				Column col = new Column(model.Type, 0);
				BindColumn(node, col, isNullable);
				col.Name = mappings.NamingStrategy.ColumnName(columnAttribute.Value);
				Table table = model.Table;
				if (table != null)
				{
					table.AddColumn(col);
				} //table=null -> an association - fill it in later
				model.AddColumn(col);
				//column group index (although can serve as a separate column index)
				XmlAttribute indexAttr = node.Attributes["index"];
				if (indexAttr != null && table != null)
				{
					table.GetIndex(indexAttr.Value).AddColumn(col);
				}
			}

			if (autoColumn && model.ColumnSpan == 0)
			{
				Column col = new Column(model.Type, 0);
				BindColumn(node, col, isNullable);
				col.Name = mappings.NamingStrategy.PropertyToColumnName(propertyPath);
				model.Table.AddColumn(col);
				model.AddColumn(col);
			}
		}

		//automatically makes a column with the default name if none is specifed by XML
		public static void BindSimpleValue(XmlNode node, SimpleValue model, bool isNullable, string path, Mappings mappings)
		{
			model.Type = GetTypeFromXML(node);

			XmlAttribute formulaNode = node.Attributes["formula"];
			if (formulaNode != null)
			{
				Formula f = new Formula();
				f.FormulaString = formulaNode.InnerText;
				model.AddFormula(f);
			}
			else
			{
				BindColumns(node, model, isNullable, true, path, mappings);
			}

			XmlAttribute fkNode = node.Attributes["foreign-key"];
			if (fkNode != null)
			{
				model.ForeignKeyName = fkNode.Value;
			}
		}

		private static string PropertyAccess(XmlNode node, Mappings mappings)
		{
			XmlAttribute accessNode = node.Attributes["access"];
			return accessNode != null ? accessNode.Value : mappings.DefaultAccess;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="node"></param>
		/// <param name="model"></param>
		/// <param name="mappings"></param>
		public static void BindProperty(XmlNode node, Mapping.Property model, Mappings mappings)
		{
			model.Name = GetPropertyName(node);
			IType type = model.Value.Type;
			if (type == null)
			{
				throw new MappingException("could not determine a property type for: " + model.Name);
			}

			model.PropertyAccessorName = PropertyAccess(node, mappings);

			XmlAttribute cascadeNode = node.Attributes["cascade"];
			model.Cascade = (cascadeNode == null) ? mappings.DefaultCascade : cascadeNode.Value;

			XmlAttribute updateNode = node.Attributes["update"];
			model.IsUpdateable = (updateNode == null) ? true : "true".Equals(updateNode.Value);

			XmlAttribute insertNode = node.Attributes["insert"];
			model.IsInsertable = (insertNode == null) ? true : "true".Equals(insertNode.Value);

			if (log.IsDebugEnabled)
			{
				string msg = "Mapped property: " + model.Name;
				string columns = Columns(model.Value);
				if (columns.Length > 0)
				{
					msg += " -> " + columns;
				}
				if (model.Type != null)
				{
					msg += ", type: " + model.Type.Name;
				}
				log.Debug(msg);
			}

			model.MetaAttributes = GetMetas(node);
		}

		public static string Columns(IValue val)
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

		/// <remarks>
		/// Called for all collections. <paramref name="containingType" /> parameter
		/// was added in NH to allow for reflection related to generic types.
		/// </remarks>
		public static void BindCollection(XmlNode node, Mapping.Collection model, string className, string path, System.Type containingType, Mappings mappings)
		{
			// ROLENAME
			model.Role = StringHelper.Qualify(className, path);
			// TODO: H3.1 has just collection.setRole(path) here - why?

			XmlAttribute inverseNode = node.Attributes["inverse"];
			if (inverseNode != null)
			{
				model.IsInverse = StringHelper.BooleanValue(inverseNode.Value);
			}

			// TODO: H3.1 - not ported: mutable, optimistic-lock

			XmlAttribute orderNode = node.Attributes["order-by"];
			if (orderNode != null)
			{
				model.OrderBy = orderNode.Value;
			}
			XmlAttribute whereNode = node.Attributes["where"];
			if (whereNode != null)
			{
				model.Where = whereNode.Value;
			}
			XmlAttribute batchNode = node.Attributes["batch-size"];
			if (batchNode != null)
			{
				model.BatchSize = Int32.Parse(batchNode.Value);
			}

			// PERSISTER
			XmlAttribute persisterNode = node.Attributes["persister"];
			if (persisterNode == null)
			{
				//persister = CollectionPersisterImpl.class;
			}
			else
			{
				model.CollectionPersisterClass = ClassForNameChecked(
					persisterNode.Value, mappings,
					"could not instantiate collection persister class: {0}");
			}

			XmlAttribute typeNode = node.Attributes["collection-type"];
			if (typeNode != null)
			{
				model.TypeName = typeNode.Value;
			}

			// FETCH STRATEGY
			InitOuterJoinFetchSetting(node, model);

			// TODO: H3.1 - fetch="subselect"

			// LAZINESS
			InitLaziness(node, model, mappings, "true", mappings.DefaultLazy);
			// TODO: H3.1 - lazy="extra"

			XmlNode oneToManyNode = node.SelectSingleNode(HbmConstants.nsOneToMany, nsmgr);
			if (oneToManyNode != null)
			{
				OneToMany oneToMany = new OneToMany(model.Owner);
				model.Element = oneToMany;
				BindOneToMany(oneToManyNode, oneToMany, mappings);
				//we have to set up the table later!! yuck
			}
			else
			{
				//TABLE
				XmlAttribute tableNode = node.Attributes["table"];
				string tableName;
				if (tableNode != null)
				{
					tableName = mappings.NamingStrategy.TableName(tableNode.Value);
				}
				else
				{
					tableName = mappings.NamingStrategy.PropertyToTableName(className, path);
				}
				XmlAttribute schemaNode = node.Attributes["schema"];
				string schema = schemaNode == null ? mappings.SchemaName : schemaNode.Value;
				model.CollectionTable = mappings.AddTable(schema, tableName);

				log.Info("Mapping collection: " + model.Role + " -> " + model.CollectionTable.Name);
			}

			//SORT
			XmlAttribute sortedAtt = node.Attributes["sort"];
			// unsorted, natural, comparator.class.name
			if (sortedAtt == null || sortedAtt.Value.Equals("unsorted"))
			{
				model.IsSorted = false;
			}
			else
			{
				model.IsSorted = true;
				if (!sortedAtt.Value.Equals("natural"))
				{
					string comparatorClassName = FullClassName(sortedAtt.Value, mappings);
					try
					{
						model.Comparer = Activator.CreateInstance(ReflectHelper.ClassForName(comparatorClassName));
					}
					catch
					{
						throw new MappingException("could not instantiate comparer class: " + comparatorClassName);
					}
				}
			}

			//ORPHAN DELETE (used for programmer error detection)
			XmlAttribute cascadeAtt = node.Attributes["cascade"];
			if (cascadeAtt != null && cascadeAtt.Value.Equals("all-delete-orphan"))
			{
				model.OrphanDelete = true;
			}

#if NET_2_0
			bool? isGeneric = null;

			XmlAttribute genericAtt = node.Attributes["generic"];
			if (genericAtt != null)
			{
				isGeneric = bool.Parse(genericAtt.Value);
			}

			System.Type collectionType = null;

			if (!isGeneric.HasValue)
			{
				collectionType = GetPropertyType(node, mappings, containingType, GetPropertyName(node));
				isGeneric = collectionType.IsGenericType;
			}

			model.IsGeneric = isGeneric ?? false;

			if (model.IsGeneric)
			{
				// Determine the generic arguments using reflection
				if (collectionType == null)
				{
					collectionType = GetPropertyType(node, mappings, containingType, GetPropertyName(node));
				}
				System.Type[] genericArguments = collectionType.GetGenericArguments();
				model.GenericArguments = genericArguments;
			}
#endif
			//set up second pass
			if (model is List)
			{
				mappings.AddSecondPass(new ListSecondPass(node, mappings, (List) model));
			}
			else if (model is Map)
			{
				mappings.AddSecondPass(new MapSecondPass(node, mappings, (Map) model));
			}
			else if (model is Set)
			{
				mappings.AddSecondPass(new SetSecondPass(node, mappings, (Set) model));
			}
			else if (model is IdentifierCollection)
			{
				mappings.AddSecondPass(new IdentifierCollectionSecondPass(node, mappings, (IdentifierCollection) model));
			}
			else
			{
				mappings.AddSecondPass(new CollectionSecondPass(node, mappings, model));
			}
		}

		private static void InitLaziness(
			XmlNode node,
			IFetchable fetchable,
			Mappings mappings,
			string proxyVal,
			bool defaultLazy)
		{
			XmlAttribute lazyNode = node.Attributes["lazy"];
			bool isLazyTrue = lazyNode == null ?
				defaultLazy && fetchable.IsLazy : //fetch="join" overrides default laziness
				lazyNode.Value.Equals(proxyVal); //fetch="join" overrides default laziness
			fetchable.IsLazy = isLazyTrue;
		}

		private static void InitLaziness(
			XmlNode node,
			ToOne fetchable,
			Mappings mappings,
			bool defaultLazy)
		{
			XmlAttribute lazyNode = node.Attributes["lazy"];
			if (lazyNode != null && "no-proxy".Equals(lazyNode.Value))
			{
				//fetchable.UnwrapProxy = true;
				fetchable.IsLazy = true;
				//TODO: better to degrade to lazy="false" if uninstrumented
			}
			else
			{
				InitLaziness(node, fetchable, mappings, "proxy", defaultLazy);
			}
		}

		public static string GetEntityName(XmlNode elem, Mappings model)
		{
			//string entityName = XmlHelper.GetAttributeValue(elem, "entity-name");
			//return entityName == null ? GetClassName( elem.Attributes[ "class" ], model ) : entityName;
			return GetClassName(elem.Attributes["class"], model);
		}

		private static string GetClassName(XmlAttribute att, Mappings model)
		{
			if (att == null)
			{
				return null;
			}
			return GetClassName(att.Value, model);
		}

		public static string GetClassName(string unqualifiedName, Mappings model)
		{
			return ClassForNameChecked(unqualifiedName, model, "unknown class {0}").AssemblyQualifiedName;
		}

		public static void BindManyToOne(XmlNode node, ManyToOne model, string defaultColumnName, bool isNullable, Mappings mappings)
		{
			BindColumns(node, model, isNullable, true, defaultColumnName, mappings);
			InitOuterJoinFetchSetting(node, model);
			InitLaziness(node, model, mappings, true);

			XmlAttribute ukName = node.Attributes["property-ref"];
			if (ukName != null)
			{
				model.ReferencedPropertyName = ukName.Value;
			}

			// TODO NH: this is sort of redundant with the code below
			model.ReferencedEntityName = GetEntityName(node, mappings);

			XmlAttribute typeNode = node.Attributes["class"];

			if (typeNode != null)
			{
				model.Type = TypeFactory.ManyToOne(
					ClassForNameChecked(typeNode.Value, mappings,
										 "could not find class: {0}"),
					model.ReferencedPropertyName);
			}

			XmlAttribute fkNode = node.Attributes["foreign-key"];
			if (fkNode != null)
			{
				model.ForeignKeyName = fkNode.Value;
			}
		}

		public static void BindAny(XmlNode node, Any model, bool isNullable, Mappings mappings)
		{
			model.IdentifierType = GetTypeFromXML(node);

			XmlAttribute metaAttribute = node.Attributes["meta-type"];
			if (metaAttribute != null)
			{
				IType metaType = TypeFactory.HeuristicType(metaAttribute.Value);
				if (metaType == null)
				{
					throw new MappingException("could not interpret meta-type");
				}
				model.MetaType = metaType;

				Hashtable values = new Hashtable();
				foreach (XmlNode metaValue in node.SelectNodes(HbmConstants.nsMetaValue, nsmgr))
				{
					try
					{
						object value = ((IDiscriminatorType) model.MetaType).FromString(metaValue.Attributes["value"].Value);
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
				}

				if (values.Count > 0)
				{
					model.MetaType = new MetaType(values, model.MetaType);
				}
			}

			BindColumns(node, model, isNullable, false, null, mappings);
		}

		public static void BindOneToOne(XmlNode node, OneToOne model, bool isNullable, Mappings mappings)
		{
			//BindColumns( node, model, isNullable, false, null, mappings );
			InitOuterJoinFetchSetting(node, model);
			InitLaziness(node, model, mappings, true);

			XmlAttribute constrNode = node.Attributes["constrained"];
			bool constrained = constrNode != null && constrNode.Value.Equals("true");
			model.IsConstrained = constrained;

			model.ForeignKeyDirection = (constrained ? ForeignKeyDirection.ForeignKeyFromParent : ForeignKeyDirection.ForeignKeyToParent);

			XmlAttribute fkNode = node.Attributes["foreign-key"];
			if (fkNode != null)
			{
				model.ForeignKeyName = fkNode.Value;
			}

			XmlAttribute ukName = node.Attributes["property-ref"];
			if (ukName != null)
			{
				model.ReferencedPropertyName = ukName.Value;
			}

			// TODO NH: this is sort of redundant with the code below
			model.ReferencedEntityName = GetEntityName(node, mappings);

			XmlAttribute classNode = node.Attributes["class"];
			if (classNode != null)
			{
				model.Type = TypeFactory.OneToOne(
					ClassForNameChecked(classNode.Value, mappings, "could not find class: {0}"),
					model.ForeignKeyDirection, model.ReferencedPropertyName);
			}
		}

		public static void BindOneToMany(XmlNode node, OneToMany model, Mappings mappings)
		{
			model.Type = (EntityType) NHibernateUtil.Entity(
				ClassForNameChecked(node.Attributes["class"].Value, mappings,
									 "associated class not found: {0}"));
		}

		public static void BindColumn(XmlNode node, Column model, bool isNullable)
		{
			XmlAttribute lengthNode = node.Attributes["length"];
			if (lengthNode != null)
			{
				model.Length = int.Parse(lengthNode.Value);
			}

			XmlAttribute nullNode = node.Attributes["not-null"];
			model.IsNullable = (nullNode != null) ? !StringHelper.BooleanValue(nullNode.Value) : isNullable;

			XmlAttribute unqNode = node.Attributes["unique"];
			model.IsUnique = unqNode != null && StringHelper.BooleanValue(unqNode.Value);

			XmlAttribute chkNode = node.Attributes["check"];
			model.CheckConstraint = chkNode != null ? chkNode.Value : string.Empty;

			XmlAttribute typeNode = node.Attributes["sql-type"];
			model.SqlType = (typeNode == null) ? null : typeNode.Value;
		}

		/// <remarks>
		/// Called for arrays and primitive arrays
		/// </remarks>
		public static void BindArray(XmlNode node, Array model, string prefix, string path, System.Type containingType, Mappings mappings)
		{
			BindCollection(node, model, prefix, path, containingType, mappings);

			XmlAttribute att = node.Attributes["element-class"];

			if (att != null)
			{
				model.ElementClass = ClassForNameChecked(att.Value, mappings,
														  "could not find element class: {0}");
			}
			else
			{
				foreach (XmlNode subnode in node.ChildNodes)
				{
					string name = subnode.LocalName; //.Name;

					//I am only concerned with elements that are from the nhibernate namespace
					if (subnode.NamespaceURI != Configuration.MappingSchemaXMLNS)
					{
						continue;
					}

					switch (name)
					{
						case "element":
							IType type = GetTypeFromXML(subnode);

							model.ElementClass = type.ReturnedClass;

							break;

						case "one-to-many":
						case "many-to-many":
						case "composite-element":
							model.ElementClass = ClassForNameChecked(
								subnode.Attributes["class"].Value, mappings,
								"element class not found: {0}");
							break;
					}
				}
			}
		}

		public static void BindComponent(XmlNode node, Component model, System.Type reflectedClass, string className, string path, bool isNullable, Mappings mappings)
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

				CollectionType collectType = CollectionType.CollectionTypeFromString(name);
				IValue value = null;
				if (collectType != null)
				{
					Mapping.Collection collection = collectType.Create(subnode, className, subpath, model.Owner, model.ComponentClass, mappings);
					mappings.AddCollection(collection);
					value = collection;
				}
				else if ("many-to-one".Equals(name) || "key-many-to-one".Equals(name))
				{
					value = new ManyToOne(model.Table);
					BindManyToOne(subnode, (ManyToOne) value, subpath, isNullable, mappings);
				}
				else if ("one-to-one".Equals(name))
				{
					value = new OneToOne(model.Table, model.Owner.Identifier);
					BindOneToOne(subnode, (OneToOne) value, isNullable, mappings);
				}
				else if ("any".Equals(name))
				{
					value = new Any(model.Table);
					BindAny(subnode, (Any) value, isNullable, mappings);
				}
				else if ("property".Equals(name) || "key-property".Equals(name))
				{
					value = new SimpleValue(model.Table);
					BindSimpleValue(subnode, (SimpleValue) value, isNullable, subpath, mappings);
				}
				else if ("component".Equals(name) || "dynamic-component".Equals(name) || "nested-composite-element".Equals(name))
				{
					System.Type subreflectedClass = model.ComponentClass == null ?
						null :
						GetPropertyType(subnode, mappings, model.ComponentClass, propertyName);
					value = (model.Owner != null) ?
						new Component(model.Owner) : // a class component
						new Component(model.Table); // a composite element
					BindComponent(subnode, (Component) value, subreflectedClass, className, subpath, isNullable, mappings);
				}
				else if ("parent".Equals(name))
				{
					model.ParentProperty = propertyName;
				}

				if (value != null)
				{
					model.AddProperty(CreateProperty(value, propertyName, model.ComponentClass, subnode, mappings));
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

		internal static IType GetTypeFromXML(XmlNode node)
		{
			IType type;
			XmlAttribute typeAttribute = node.Attributes["type"];

			if (typeAttribute == null)
			{
				typeAttribute = node.Attributes["id-type"]; //for an any
			}
			string typeName = null;
			IDictionary parameters = null;
			if (typeAttribute != null)
			{
				typeName = typeAttribute.Value;
			}
			else
			{
				XmlNode typeNode = node.SelectSingleNode(HbmConstants.nsType, nsmgr);
				if (typeNode == null)//we will have to use reflection
					return null;
				XmlAttribute nameAttribute = typeNode.Attributes["name"];//we know it exists because the schema validate it
				typeName = nameAttribute.Value;
				parameters = new Hashtable();
				foreach (XmlNode childNode in typeNode.ChildNodes)
				{
					parameters.Add(childNode.Attributes["name"].Value,
						childNode.InnerText.Trim());
				}
			}
			type = TypeFactory.HeuristicType(typeName, parameters);
			if (type == null)
			{
				throw new MappingException("could not interpret type: " + typeAttribute.Value);
			}
			return type;
		}

		private static void InitOuterJoinFetchSetting(XmlNode node, IFetchable model)
		{
			XmlAttribute fetchNode = node.Attributes["fetch"];
			FetchMode fetchStyle;
			bool lazy = true;

			if (fetchNode == null)
			{
				XmlAttribute jfNode = node.Attributes["outer-join"];
				if (jfNode == null)
				{
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
					{
						fetchStyle = FetchMode.Default;
					}
				}
				else
				{
					// use old (HB 2.1) defaults if outer-join is specified
					string eoj = jfNode.Value;
					if ("auto".Equals(eoj))
					{
						fetchStyle = FetchMode.Default;
					}
					else
					{
						bool join = "true".Equals(eoj);
						fetchStyle = join ?
							FetchMode.Join :
							FetchMode.Select;
					}
				}
			}
			else
			{
				bool join = "join".Equals(fetchNode.Value);
				fetchStyle = join ?
					FetchMode.Join :
					FetchMode.Select;
			}

			model.FetchMode = fetchStyle;
			model.IsLazy = lazy;
		}

		private static void MakeIdentifier(XmlNode node, SimpleValue model, Mappings mappings)
		{
			//GENERATOR

			XmlNode subnode = node.SelectSingleNode(HbmConstants.nsGenerator, nsmgr);
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
					parms.Add("schema", mappings.SchemaName);
				}

				parms.Add("target_table", model.Table.Name);

				foreach (Column col in model.ColumnCollection)
				{
					parms.Add("target_column", col);
					break;
				}

				foreach (XmlNode childNode in subnode.SelectNodes(HbmConstants.nsParam, nsmgr))
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

		public static void MakeVersion(XmlNode node, SimpleValue model)
		{
			// VERSION UNSAVED-VALUE
			XmlAttribute nullValueNode = node.Attributes["unsaved-value"];
			if (nullValueNode != null)
			{
				model.NullValue = nullValueNode.Value;
			}
			else
			{
				model.NullValue = null;
			}
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
					Mapping.Collection collection = collectType.Create(subnode, model.Name, propertyName, model, model.MappedClass, mappings);
					mappings.AddCollection(collection);
					value = collection;
				}
				else if ("many-to-one".Equals(name))
				{
					value = new ManyToOne(table);
					BindManyToOne(subnode, (ManyToOne) value, propertyName, true, mappings);
				}
				else if ("any".Equals(name))
				{
					value = new Any(table);
					BindAny(subnode, (Any) value, true, mappings);
				}
				else if ("one-to-one".Equals(name))
				{
					value = new OneToOne(table, model.Identifier);
					BindOneToOne(subnode, (OneToOne) value, true, mappings);
				}
				else if ("property".Equals(name))
				{
					value = new SimpleValue(table);
					BindSimpleValue(subnode, (SimpleValue) value, true, propertyName, mappings);
				}
				else if ("component".Equals(name) || "dynamic-component".Equals(name))
				{
					// NH: Modified from H2.1 to allow specifying the type explicitly using class attribute
					System.Type reflectedClass = GetPropertyType(subnode, mappings, model.MappedClass, propertyName);
					value = new Component(model);
					BindComponent(subnode, (Component) value, reflectedClass, model.Name, propertyName, true, mappings);
				}
				else if ("subclass".Equals(name))
				{
					HandleSubclass(model, mappings, subnode);
				}
				else if ("joined-subclass".Equals(name))
				{
					HandleJoinedSubclass(model, mappings, subnode);
				}
				if (value != null)
				{
					model.AddNewProperty(CreateProperty(value, propertyName, model.MappedClass, subnode, mappings));
				}
			}
		}

		private static Mapping.Property CreateProperty(IValue value, string propertyName, System.Type parentClass, XmlNode subnode, Mappings mappings)
		{
			if (parentClass != null && value.IsSimpleValue)
			{
				((SimpleValue) value).SetTypeByReflection(parentClass, propertyName, PropertyAccess(subnode, mappings));
			}

			// This is done here 'cos we might only know the type here (ugly!)
			if (value is ToOne)
			{
				ToOne toOne = (ToOne) value;
				string propertyRef = toOne.ReferencedPropertyName;
				if (propertyRef != null)
				{
					mappings.AddUniquePropertyReference(((EntityType) value.Type).AssociatedClass, propertyRef);
				}
			}

			value.CreateForeignKey();
			Mapping.Property prop = new Mapping.Property();
			prop.Value = value;
			BindProperty(subnode, prop, mappings);

			return prop;
		}

		private static void HandleJoinedSubclass(PersistentClass model, Mappings mappings, XmlNode subnode)
		{
			Subclass subclass = new Subclass(model);
			BindJoinedSubclass(subnode, subclass, mappings);
			model.AddSubclass(subclass);
			mappings.AddClass(subclass);
		}

		private static void HandleSubclass(PersistentClass model, Mappings mappings, XmlNode subnode)
		{
			Subclass subclass = new Subclass(model);
			BindSubclass(subnode, subclass, mappings);
			model.AddSubclass(subclass);
			mappings.AddClass(subclass);
		}

		/// <remarks>
		/// Called for Lists, arrays, primitive arrays
		/// </remarks>>
		public static void BindListSecondPass(XmlNode node, List model, IDictionary classes, Mappings mappings)
		{
			BindCollectionSecondPass(node, model, classes, mappings);

			XmlNode subnode = node.SelectSingleNode(HbmConstants.nsIndex, nsmgr);
			IntegerValue iv = new IntegerValue(model.CollectionTable);
			BindIntegerValue(subnode, iv, IndexedCollection.DefaultIndexColumnName, model.IsOneToMany, mappings);
			model.Index = iv;
		}

		public static void BindIdentifierCollectionSecondPass(XmlNode node, IdentifierCollection model, IDictionary persitentClasses, Mappings mappings)
		{
			BindCollectionSecondPass(node, model, persitentClasses, mappings);

			XmlNode subnode = node.SelectSingleNode(HbmConstants.nsCollectionId, nsmgr);
			SimpleValue id = new SimpleValue(model.CollectionTable);
			BindSimpleValue(subnode, id, false, IdentifierCollection.DefaultIdentifierColumnName, mappings);
			model.Identifier = id;
			MakeIdentifier(subnode, id, mappings);
		}

		/// <summary>
		/// Called for Maps
		/// </summary>
		/// <param name="node"></param>
		/// <param name="model"></param>
		/// <param name="classes"></param>
		/// <param name="mappings"></param>
		public static void BindMapSecondPass(XmlNode node, Map model, IDictionary classes, Mappings mappings)
		{
			BindCollectionSecondPass(node, model, classes, mappings);

			foreach (XmlNode subnode in node.ChildNodes)
			{
				//I am only concerned with elements that are from the nhibernate namespace
				if (subnode.NamespaceURI != Configuration.MappingSchemaXMLNS)
				{
					continue;
				}

				string name = subnode.LocalName; //.Name;

				if ("index".Equals(name))
				{
					SimpleValue value = new SimpleValue(model.CollectionTable);
					BindSimpleValue(subnode, value, model.IsOneToMany, IndexedCollection.DefaultIndexColumnName, mappings);
					model.Index = value;
					if (model.Index.Type == null)
					{
						throw new MappingException("map index element must specify a type: " + model.Role);
					}
				}
				else if ("index-many-to-many".Equals(name))
				{
					ManyToOne mto = new ManyToOne(model.CollectionTable);
					BindManyToOne(subnode, mto, IndexedCollection.DefaultIndexColumnName, model.IsOneToMany, mappings);
					model.Index = mto;
				}
				else if ("composite-index".Equals(name))
				{
					Component component = new Component(model.CollectionTable);
					BindComponent(subnode, component, null, model.Role, "index", model.IsOneToMany, mappings);
					model.Index = component;
				}
				else if ("index-many-to-any".Equals(name))
				{
					Any any = new Any(model.CollectionTable);
					BindAny(subnode, any, model.IsOneToMany, mappings);
					model.Index = any;
				}
			}
		}

		/// <remarks>
		/// Called for all collections
		/// </remarks>
		public static void BindCollectionSecondPass(XmlNode node, Mapping.Collection model, IDictionary persistentClasses, Mappings mappings)
		{
			if (model.IsOneToMany)
			{
				OneToMany oneToMany = (OneToMany) model.Element;
				System.Type assocClass = oneToMany.EntityType.AssociatedClass;
				PersistentClass persistentClass = (PersistentClass) persistentClasses[assocClass];
				if (persistentClass == null)
				{
					throw new MappingException("Association references unmapped class: " + assocClass.Name);
				}
				oneToMany.AssociatedClass = persistentClass;
				model.CollectionTable = persistentClass.Table;

				if (log.IsInfoEnabled)
				{
					log.Info("mapping collection: " + model.Role + " -> " + model.CollectionTable.Name);
				}
			}

			//CHECK
			XmlAttribute chNode = node.Attributes["check"];
			if (chNode != null)
			{
				model.CollectionTable.AddCheckConstraint(chNode.Value);
			}

			//contained elements:
			foreach (XmlNode subnode in node.ChildNodes)
			{
				//I am only concerned with elements that are from the nhibernate namespace
				if (subnode.NamespaceURI != Configuration.MappingSchemaXMLNS)
				{
					continue;
				}

				string name = subnode.LocalName; //.Name;

				if ("key".Equals(name) || "generated-key".Equals(name))
				{
					SimpleValue key = new SimpleValue(model.CollectionTable);
					BindSimpleValue(subnode, key, model.IsOneToMany, Mapping.Collection.DefaultKeyColumnName, mappings);
					key.Type = model.Owner.Identifier.Type;
					if (key.Type.ReturnedClass.IsArray)
					{
						throw new MappingException("illegal use of an array as an identifier (arrays don't reimplement Equals)");
					}
					model.Key = key;
				}
				else if ("element".Equals(name))
				{
					SimpleValue elt = new SimpleValue(model.CollectionTable);
					model.Element = elt;
					BindSimpleValue(subnode, elt, true, Mapping.Collection.DefaultElementColumnName, mappings);
				}
				else if ("many-to-many".Equals(name))
				{
					ManyToOne element = new ManyToOne(model.CollectionTable);
					model.Element = element;
					BindManyToOne(subnode, element, Mapping.Collection.DefaultElementColumnName, false, mappings);
				}
				else if ("composite-element".Equals(name))
				{
					Component element = new Component(model.CollectionTable);
					model.Element = element;
					BindComponent(subnode, element, null, model.Role, "element", true, mappings);
				}
				else if ("many-to-any".Equals(name))
				{
					Any element = new Any(model.CollectionTable);
					model.Element = element;
					BindAny(subnode, element, true, mappings);
				}
				else if ("jcs-cache".Equals(name) || "cache".Equals(name))
				{
					ICacheConcurrencyStrategy cache = CacheFactory.CreateCache(subnode, model.Role, model.Owner.IsMutable);
					mappings.AddCache(model.Role, cache);
					model.Cache = cache;
				}
			}

			// Code below is not present in H2.1, why was it added?
			if (!model.IsInverse)
			{
				if (!model.IsOneToMany) // no foreign key for a one-to-many
				{
					model.Element.CreateForeignKey();
				}

				model.Key.CreateForeignKeyOfClass(model.Owner.MappedClass);
			}
		}

		public static void BindRoot(XmlDocument doc, Mappings mappings)
		{
			XmlNode hmNode = doc.DocumentElement;
			ExtractRootAttributes(hmNode, mappings);

			nsmgr = new XmlNamespaceManager(doc.NameTable);
			// note that the prefix has absolutely nothing to do with what the user
			// selects as their prefix in the document.  It is the prefix we use to 
			// build the XPath and the nsmgr takes care of translating our prefix into
			// the user defined prefix...
			nsmgr.AddNamespace(HbmConstants.nsPrefix, Configuration.MappingSchemaXMLNS);

			foreach (XmlNode n in hmNode.SelectNodes(HbmConstants.nsClass, nsmgr))
			{
				RootClass rootclass = new RootClass();
				HbmBinder.BindRootClass(n, rootclass, mappings);
				mappings.AddClass(rootclass);
			}

			foreach (XmlNode n in hmNode.SelectNodes(HbmConstants.nsSubclass, nsmgr))
			{
				PersistentClass superModel = GetSuperclass(mappings, n);
				HandleSubclass(superModel, mappings, n);
			}

			foreach (XmlNode n in hmNode.SelectNodes(HbmConstants.nsJoinedSubclass, nsmgr))
			{
				PersistentClass superModel = GetSuperclass(mappings, n);
				HandleJoinedSubclass(superModel, mappings, n);
			}

			foreach (XmlNode n in hmNode.SelectNodes(HbmConstants.nsQuery, nsmgr))
			{
				BindNamedQuery(n, null, mappings);
			}

			foreach (XmlNode n in hmNode.SelectNodes(HbmConstants.nsSqlQuery, nsmgr))
			{
				BindNamedSQLQuery(n, null, mappings);
			}

			foreach (XmlNode n in hmNode.SelectNodes(HbmConstants.nsImport, nsmgr))
			{
				string className = FullClassName(n.Attributes["class"].Value, mappings);
				XmlAttribute renameNode = n.Attributes["rename"];
				string rename = (renameNode == null) ? StringHelper.GetClassname(className) : renameNode.Value;
				log.Debug("Import: " + rename + " -> " + className);
				mappings.AddImport(className, rename);
			}
		}

		internal static FlushMode GetFlushMode(string flushMode)
		{
			switch (flushMode)
			{
				case null:
					return FlushMode.Auto;
				// H3 has null here but FlushMode can't be null so I'm assuming Auto as default
				case "auto":
					return FlushMode.Auto;
				case "commit":
					return FlushMode.Commit;
				case "never":
					return FlushMode.Never;
				default:
					throw new MappingException("unknown flushmode " + flushMode);
			}
		}

		private static void BindNamedQuery(XmlNode queryElem, string path, Mappings mappings)
		{
			string queryName = queryElem.Attributes["name"].Value;
			if (path != null)
			{
				queryName = path + '.' + queryName;
			}
			string query = queryElem.InnerText;
			log.Debug("Named query: " + queryName + " -> " + query);

			bool cacheable = "true".Equals(XmlHelper.GetAttributeValue(queryElem, "cacheable"));
			string region = XmlHelper.GetAttributeValue(queryElem, "cache-region");
			XmlAttribute tAtt = queryElem.Attributes["timeout"];
			int timeout = tAtt == null ? -1 : int.Parse(tAtt.Value);
			XmlAttribute fsAtt = queryElem.Attributes["fetch-size"];
			int fetchSize = fsAtt == null ? -1 : int.Parse(fsAtt.Value);
			XmlAttribute roAttr = queryElem.Attributes["read-only"];
			bool readOnly = roAttr != null && "true".Equals(roAttr.Value);
			XmlAttribute cacheModeAtt = queryElem.Attributes["cache-mode"];
			string cacheMode = cacheModeAtt == null ? null : cacheModeAtt.Value;
			XmlAttribute cmAtt = queryElem.Attributes["comment"];
			string comment = cmAtt == null ? null : cmAtt.Value;

			NamedQueryDefinition namedQuery = new NamedQueryDefinition(
				query,
				cacheable,
				region,
				timeout,
				fetchSize,
				//GetFlushMode(XmlHelper.GetAttributeValue(queryElem, "flush-mode")),
				//GetCacheMode(cacheMode),
				readOnly,
				comment,
				GetParameterTypes(queryElem)
			);

			mappings.AddQuery(queryName, namedQuery);
		}

		internal static IDictionary GetParameterTypes(XmlNode queryElem)
		{
			IDictionary result = new SequencedHashMap();
			foreach (XmlElement element in queryElem.SelectNodes(HbmConstants.nsQueryParam, nsmgr))
			{
				result[XmlHelper.GetAttributeValue(element, "name")] = XmlHelper.GetAttributeValue(element, "type");
			}
			return result;
		}

		private static void BindNamedSQLQuery(XmlNode queryElem, string path, Mappings mappings)
		{
			mappings.AddSecondPass(new NamedSQLQuerySecondPass(queryElem, path, mappings));
			/*
			string qname = queryElem.Attributes["name"].Value;
			NamedSQLQuery namedQuery = new NamedSQLQuery(queryElem.InnerText);

			foreach (XmlNode returns in queryElem.SelectNodes(nsReturn, nsmgr))
			{
				string alias = returns.Attributes["alias"].Value;
				System.Type clazz = ClassForNameChecked(
					returns.Attributes["class"].Value, mappings,
					"class not found: {0} for alias " + alias);
				namedQuery.AddAliasedClass(alias, clazz);
			}

			foreach (XmlNode table in queryElem.SelectNodes(nsSynchronize, nsmgr))
			{
				namedQuery.AddSynchronizedTable(table.Attributes["table"].Value);
			}

			log.Debug("Named sql query: " + qname + " -> " + namedQuery.QueryString);
			mappings.AddSQLQuery(qname, namedQuery);
			*/
		}

		private static void ExtractRootAttributes(XmlNode hmNode, Mappings mappings)
		{
			XmlAttribute schemaNode = hmNode.Attributes["schema"];
			mappings.SchemaName = (schemaNode == null) ? null : schemaNode.Value;

			XmlAttribute dcNode = hmNode.Attributes["default-cascade"];
			mappings.DefaultCascade = (dcNode == null) ? "none" : dcNode.Value;

			XmlAttribute daNode = hmNode.Attributes["default-access"];
			mappings.DefaultAccess = (daNode == null) ? "property" : daNode.Value;

			XmlAttribute dlNode = hmNode.Attributes["default-lazy"];
			mappings.DefaultLazy = dlNode == null || dlNode.Value.Equals("true");

			XmlAttribute aiNode = hmNode.Attributes["auto-import"];
			mappings.IsAutoImport = (aiNode == null) ? true : "true".Equals(aiNode.Value);

			XmlAttribute nsNode = hmNode.Attributes["namespace"];
			mappings.DefaultNamespace = (nsNode == null) ? null : nsNode.Value;

			XmlAttribute assemblyNode = hmNode.Attributes["assembly"];
			mappings.DefaultAssembly = (assemblyNode == null) ? null : assemblyNode.Value;
		}

		private static PersistentClass GetSuperclass(Mappings model, XmlNode subnode)
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

		private class CollectionSecondPass : Cfg.CollectionSecondPass
		{
			protected XmlNode node;

			public CollectionSecondPass(XmlNode node, Mappings mappings, Mapping.Collection collection)
				: base(mappings, collection)
			{
				this.node = node;
			}

			public override void SecondPass(IDictionary persistentClasses)
			{
				HbmBinder.BindCollectionSecondPass(node, collection, persistentClasses, mappings);
			}
		}

		private class IdentifierCollectionSecondPass : CollectionSecondPass
		{
			public IdentifierCollectionSecondPass(XmlNode node, Mappings mappings, IdentifierCollection collection)
				: base(node, mappings, collection)
			{
			}

			public override void SecondPass(IDictionary persistentClasses)
			{
				HbmBinder.BindIdentifierCollectionSecondPass(node, (IdentifierCollection) collection, persistentClasses, mappings);
			}
		}

		private class MapSecondPass : CollectionSecondPass
		{
			public MapSecondPass(XmlNode node, Mappings mappings, Map collection)
				: base(node, mappings, collection)
			{
			}

			public override void SecondPass(IDictionary persistentClasses)
			{
				HbmBinder.BindMapSecondPass(node, (Map) collection, persistentClasses, mappings);
			}
		}

		private class SetSecondPass : CollectionSecondPass
		{
			public SetSecondPass(XmlNode node, Mappings mappings, Set collection)
				: base(node, mappings, collection)
			{
			}

			public override void SecondPass(IDictionary persistentClasses)
			{
				HbmBinder.BindSetSecondPass(node, (Set) collection, persistentClasses, mappings);
			}
		}

		private class ListSecondPass : CollectionSecondPass
		{
			public ListSecondPass(XmlNode node, Mappings mappings, List collection)
				: base(node, mappings, collection)
			{
			}

			public override void SecondPass(IDictionary persistentClasses)
			{
				HbmBinder.BindListSecondPass(node, (List) collection, persistentClasses, mappings);
			}
		}

		private abstract class CollectionType
		{
			private string xmlTag;
			public abstract Mapping.Collection Create(XmlNode node, string className, string path, PersistentClass owner, System.Type containingType, Mappings mappings);

			public CollectionType(string xmlTag)
			{
				this.xmlTag = xmlTag;
			}

			public override string ToString()
			{
				return xmlTag;
			}

			private static CollectionType MAP = new CollectionTypeMap("map");

			private class CollectionTypeMap : CollectionType
			{
				public CollectionTypeMap(string xmlTag)
					: base(xmlTag)
				{
				}

				public override Mapping.Collection Create(XmlNode node, string prefix, string path, PersistentClass owner, System.Type containingType, Mappings mappings)
				{
					Map map = new Map(owner);
					HbmBinder.BindCollection(node, map, prefix, path, containingType, mappings);
					return map;
				}
			}

			private static CollectionType SET = new CollectionTypeSet("set");

			private class CollectionTypeSet : CollectionType
			{
				public CollectionTypeSet(string xmlTag)
					: base(xmlTag)
				{
				}

				public override Mapping.Collection Create(XmlNode node, string prefix, string path, PersistentClass owner, System.Type containingType, Mappings mappings)
				{
					Set setCollection = new Set(owner);
					HbmBinder.BindCollection(node, setCollection, prefix, path, containingType, mappings);
					return setCollection;
				}
			}

			private static CollectionType LIST = new CollectionTypeList("list");

			private class CollectionTypeList : CollectionType
			{
				public CollectionTypeList(string xmlTag)
					: base(xmlTag)
				{
				}

				public override Mapping.Collection Create(XmlNode node, string prefix, string path, PersistentClass owner, System.Type containingType, Mappings mappings)
				{
					List list = new List(owner);
					HbmBinder.BindCollection(node, list, prefix, path, containingType, mappings);
					return list;
				}
			}

			private static CollectionType BAG = new CollectionTypeBag("bag");

			private class CollectionTypeBag : CollectionType
			{
				public CollectionTypeBag(string xmlTag)
					: base(xmlTag)
				{
				}

				public override Mapping.Collection Create(XmlNode node, string prefix, string path, PersistentClass owner, System.Type containingType, Mappings mappings)
				{
					Bag bag = new Bag(owner);
					HbmBinder.BindCollection(node, bag, prefix, path, containingType, mappings);
					return bag;
				}

			}

			private static CollectionType IDBAG = new CollectionTypeIdBag("idbag");

			private class CollectionTypeIdBag : CollectionType
			{
				public CollectionTypeIdBag(string xmlTag)
					: base(xmlTag)
				{
				}

				public override Mapping.Collection Create(XmlNode node, string prefix, string path, PersistentClass owner, System.Type containingType, Mappings mappings)
				{
					IdentifierBag bag = new IdentifierBag(owner);
					HbmBinder.BindCollection(node, bag, prefix, path, containingType, mappings);
					return bag;
				}

			}

			private static CollectionType ARRAY = new CollectionTypeArray("array");

			private class CollectionTypeArray : CollectionType
			{
				public CollectionTypeArray(string xmlTag)
					: base(xmlTag)
				{
				}

				public override Mapping.Collection Create(XmlNode node, string prefix, string path, PersistentClass owner, System.Type containingType, Mappings mappings)
				{
					Array array = new Array(owner);
					HbmBinder.BindArray(node, array, prefix, path, containingType, mappings);
					return array;
				}
			}

			private static CollectionType PRIMITIVE_ARRAY = new CollectionTypePrimitiveArray("primitive-array");

			private class CollectionTypePrimitiveArray : CollectionType
			{
				public CollectionTypePrimitiveArray(string xmlTag)
					: base(xmlTag)
				{
				}

				public override Mapping.Collection Create(XmlNode node, string prefix, string path, PersistentClass owner, System.Type containingType, Mappings mappings)
				{
					PrimitiveArray array = new PrimitiveArray(owner);
					HbmBinder.BindArray(node, array, prefix, path, containingType, mappings);
					return array;
				}
			}

			private static Hashtable Instances = new Hashtable();

			static CollectionType()
			{
				Instances.Add(MAP.ToString(), MAP);
				Instances.Add(BAG.ToString(), BAG);
				Instances.Add(IDBAG.ToString(), IDBAG);
				Instances.Add(SET.ToString(), SET);
				Instances.Add(LIST.ToString(), LIST);
				Instances.Add(ARRAY.ToString(), ARRAY);
				Instances.Add(PRIMITIVE_ARRAY.ToString(), PRIMITIVE_ARRAY);
			}

			public static CollectionType CollectionTypeFromString(string xmlTagName)
			{
				return (CollectionType) Instances[xmlTagName];
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

		private static IDictionary GetMetas(XmlNode node)
		{
			IDictionary map = new Hashtable();

			foreach (XmlNode metaNode in node.SelectNodes(HbmConstants.nsMeta, nsmgr))
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

		public static void BindIntegerValue(XmlNode node, IntegerValue model, string defaultColumnName, bool isNullable, Mappings mappings)
		{
			BindSimpleValue(node, model, isNullable, defaultColumnName, mappings);

			if (model.ColumnCollection.Count > 1)
			{
				log.Error("This shouldn't happen, check BindIntegerValue");
			}
			foreach (Column col in model.ColumnCollection)
			{
				col.Type = NHibernateUtil.Int32;
				col.TypeIndex = 0;
				break;
			}
		}

		private static System.Type GetPropertyType(XmlNode definingNode, Mappings mappings,
			System.Type containingType, string propertyName)
		{
			if (definingNode.Attributes["class"] != null)
			{
				return ClassForNameChecked(definingNode.Attributes["class"].Value, mappings,
					"could not find class: {0}");
			}
			else if (containingType == null)
			{
				return null;
			}

			string access = PropertyAccess(definingNode, mappings);

			return ReflectHelper.ReflectedPropertyClass(containingType, propertyName, access);
		}

		public static void BindSetSecondPass(XmlNode node, Set model, IDictionary persistentClasses, Mappings mappings)
		{
			BindCollectionSecondPass(node, model, persistentClasses, mappings);

			if (!model.IsOneToMany)
			{
				model.CreatePrimaryKey();
			}
		}

		private static string GetPropertyName(XmlNode node)
		{
			if (node.Attributes != null)
			{
				XmlAttribute propertyNameNode = node.Attributes["name"];
				return (propertyNameNode == null) ? null : propertyNameNode.Value;
			}
			return null;
		}

		private static void HandleCustomSQL(XmlNode node, PersistentClass model)
		{
			XmlNode element = node.SelectSingleNode(HbmConstants.nsSqlInsert, nsmgr);

			if (element != null)
			{
				bool callable = IsCallable(element);
				model.SetCustomSQLInsert(element.InnerText.Trim(), callable, GetResultCheckStyle(element, callable));
			}

			element = node.SelectSingleNode(HbmConstants.nsSqlDelete, nsmgr);
			if (element != null)
			{
				bool callable = IsCallable(element);
				model.SetCustomSQLDelete(element.InnerText.Trim(), callable, GetResultCheckStyle(element, callable));
			}

			element = node.SelectSingleNode(HbmConstants.nsSqlUpdate, nsmgr);
			if (element != null)
			{
				bool callable = IsCallable(element);
				model.SetCustomSQLUpdate(element.InnerText.Trim(), callable, GetResultCheckStyle(element, callable));
			}

			element = node.SelectSingleNode(HbmConstants.nsLoader, nsmgr);
			if (element != null)
			{
				model.LoaderName = XmlHelper.GetAttributeValue(element, "query-ref");
			}
		}

		// TODO: uncomment when Join implemented
		//private static void handleCustomSQL(Element node, Join model)
		//{
		//    Element element = node.element("sql-insert");
		//    if (element != null)
		//    {
		//        boolean callable = isCallable(element);
		//        model.setCustomSQLInsert(element.getTextTrim(), callable, getResultCheckStyle(element, callable));
		//    }

		//    element = node.element("sql-delete");
		//    if (element != null)
		//    {
		//        boolean callable = isCallable(element);
		//        model.setCustomSQLDelete(element.getTextTrim(), callable, getResultCheckStyle(element, callable));
		//    }

		//    element = node.element("sql-update");
		//    if (element != null)
		//    {
		//        boolean callable = isCallable(element);
		//        model.setCustomSQLUpdate(element.getTextTrim(), callable, getResultCheckStyle(element, callable));
		//    }
		//}

		private static void HandleCustomSQL(XmlNode node, Mapping.Collection model)
		{
			XmlNode element = node.SelectSingleNode(HbmConstants.nsSqlInsert, nsmgr);

			if (element != null)
			{
				bool callable = IsCallable(element);
				model.SetCustomSQLInsert(element.InnerText.Trim(), callable, GetResultCheckStyle(element, callable));
			}

			element = node.SelectSingleNode(HbmConstants.nsSqlDelete, nsmgr);
			if (element != null)
			{
				bool callable = IsCallable(element);
				model.SetCustomSQLDelete(element.InnerText.Trim(), callable, GetResultCheckStyle(element, callable));
			}

			element = node.SelectSingleNode(HbmConstants.nsSqlUpdate, nsmgr);
			if (element != null)
			{
				bool callable = IsCallable(element);
				model.SetCustomSQLUpdate(element.InnerText.Trim(), callable, GetResultCheckStyle(element, callable));
			}

			element = node.SelectSingleNode(HbmConstants.nsSqlDeleteAll, nsmgr);
			if (element != null)
			{
				bool callable = IsCallable(element);
				model.SetCustomSQLDeleteAll(element.InnerText.Trim(), callable, GetResultCheckStyle(element, callable));
			}
		}

		private static bool IsCallable(XmlNode element)
		{
			XmlAttribute attrib = element.Attributes["callable"];
			return attrib != null && "true".Equals(attrib.Value);
		}

		private static ExecuteUpdateResultCheckStyle GetResultCheckStyle(XmlNode element, bool callable)
		{
			XmlAttribute attr = element.Attributes["check"];
			if (attr == null)
			{
				// use COUNT as the default.  This mimics the old behavior, although
				// NONE might be a better option moving forward in the case of callable
				return ExecuteUpdateResultCheckStyle.Count;
			}
			return ExecuteUpdateResultCheckStyle.Parse(attr.Value);
		}
	}
}