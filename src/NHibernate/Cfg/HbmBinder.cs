using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;

using log4net;

using NHibernate.Cfg.XmlHbmBinding;
using NHibernate.Engine;
using NHibernate.Mapping;
using NHibernate.Property;
using NHibernate.Type;
using NHibernate.Util;

using Array=NHibernate.Mapping.Array;

namespace NHibernate.Cfg
{
	public class HbmBinder
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(HbmBinder));

		// Made internal to be accessible from Cfg.CollectionSecondPass
		private static XmlNamespaceManager namespaceManager;

		internal static XmlNamespaceManager NamespaceManager
		{
			get { return namespaceManager; }
		}

		internal static Dialect.Dialect dialect;

		/// <summary>
		/// Converts a partial class name into a fully qualified one
		/// </summary>
		/// <param name="className"></param>
		/// <param name="mapping"></param>
		/// <returns></returns>
		protected static string FullClassName(string className, Mappings mapping)
		{
			if (className == null)
			{
				return null;
			}

			return TypeNameParser.Parse(className, mapping.DefaultNamespace, mapping.DefaultAssembly)
				.ToString();
		}

		/// <summary>
		/// Attempts to find a type by its full name. Throws a <see cref="MappingException" />
		/// using the provided <paramref name="errorMessage" /> in case of failure.
		/// </summary>
		/// <param name="fullName">name of the class to find</param>
		/// <param name="errorMessage">Error message to use for
		/// the <see cref="MappingException" /> in case of failure. Should contain
		/// the <c>{0}</c> formatting placeholder.</param>
		/// <returns>A <see cref="System.Type" /> instance.</returns>
		/// <exception cref="MappingException">
		/// Thrown when there is an error loading the class.
		/// </exception>
		protected static System.Type ClassForFullNameChecked(string fullName, string errorMessage)
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
		protected static System.Type ClassForNameChecked(string name, Mappings mappings, string errorMessage)
		{
			return ClassForFullNameChecked(FullClassName(name, mappings), errorMessage);
		}

		private static void BindColumns(XmlNode node, SimpleValue model, bool isNullable, bool autoColumn, string propertyPath,
		                               Mappings mappings)
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
					{
						table.AddColumn(col);
					}
					//table=null -> an association, fill it in later
					model.AddColumn(col);

					//column index
					BindIndex(columnElement.Attributes["index"], table, col, mappings);
					//column group index (although it can serve as a separate column index)
					BindIndex(node.Attributes["index"], table, col, mappings);

					BindUniqueKey(columnElement.Attributes["unique-key"], table, col, mappings);
					BindUniqueKey(node.Attributes["unique-key"], table, col, mappings);
				}
			}
			else
			{
				Column col = new Column(model.Type, 0);
				BindColumn(node, col, isNullable);
				col.Name = mappings.NamingStrategy.ColumnName(columnAttribute.Value);
				if (table != null)
				{
					table.AddColumn(col);
				} //table=null -> an association - fill it in later
				model.AddColumn(col);
				//column group index (although can serve as a separate column index)
				BindIndex(node.Attributes["index"], table, col, mappings);
				BindUniqueKey(node.Attributes["unique-key"], table, col, mappings);
			}

			if (autoColumn && model.ColumnSpan == 0)
			{
				Column col = new Column(model.Type, 0);
				BindColumn(node, col, isNullable);
				col.Name = mappings.NamingStrategy.PropertyToColumnName(propertyPath);
				model.Table.AddColumn(col);
				model.AddColumn(col);
				//column group index (although can serve as a separate column index)
				BindIndex(node.Attributes["index"], table, col, mappings);
				BindUniqueKey(node.Attributes["unique-key"], table, col, mappings);
			}
		}

		private static void BindIndex(XmlAttribute indexAttribute, Table table, Column column, Mappings mappings)
		{
			if (indexAttribute != null && table != null)
			{
				StringTokenizer tokens = new StringTokenizer(indexAttribute.Value, ", ");
				foreach (string token in tokens)
				{
					table.GetIndex(token).AddColumn(column);
				}
			}
		}

		private static void BindUniqueKey(XmlAttribute uniqueKeyAttribute, Table table, Column column, Mappings mappings)
		{
			if (uniqueKeyAttribute != null && table != null)
			{
				StringTokenizer tokens = new StringTokenizer(uniqueKeyAttribute.Value, ", ");
				foreach (string token in tokens)
				{
					table.GetUniqueKey(token).AddColumn(column);
				}
			}
		}

		//automatically makes a column with the default name if none is specifed by XML
		protected static void BindSimpleValue(XmlNode node, SimpleValue model, bool isNullable, string path, Mappings mappings)
		{
			model.Type = GetTypeFromXML(node);
			//BindSimpleValueType(node, model, mappings);

			BindColumnsOrFormula(node, model, path, isNullable, mappings);

			XmlAttribute fkNode = node.Attributes["foreign-key"];
			if (fkNode != null)
			{
				model.ForeignKeyName = fkNode.Value;
			}
		}

		protected static string PropertyAccess(XmlNode node, Mappings mappings)
		{
			XmlAttribute accessNode = node.Attributes["access"];
			return accessNode != null ? accessNode.Value : mappings.DefaultAccess;
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

		protected static void BindProperty(XmlNode node, Mapping.Property property, Mappings mappings)
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

		internal static string Columns(IValue val)
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
		private static void BindCollection(XmlNode node, Mapping.Collection model, string className, string path,
		                                  System.Type containingType, Mappings mappings)
		{
			// ROLENAME
			model.Role = StringHelper.Qualify(className, path);
			// TODO: H3.1 has just collection.setRole(path) here - why?

			XmlAttribute inverseNode = node.Attributes["inverse"];
			if (inverseNode != null)
			{
				model.IsInverse = StringHelper.BooleanValue(inverseNode.Value);
			}

			// TODO: H3.1 - not ported: mutable
			XmlAttribute olNode = node.Attributes["optimistic-lock"];
			model.IsOptimisticLocked = olNode == null || "true".Equals(olNode.Value);

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

			if ("subselect".Equals(XmlHelper.GetAttributeValue(node, "fetch")))
			{
				model.IsSubselectLoadable = true;
				model.Owner.HasSubselectLoadableCollections = true;
			}

			// LAZINESS
			InitLaziness(node, model, mappings, "true", mappings.DefaultLazy);
			// TODO: H3.1 - lazy="extra"

			XmlNode oneToManyNode = node.SelectSingleNode(HbmConstants.nsOneToMany, namespaceManager);
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

			HandleCustomSQL(node, model);

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

			foreach (XmlNode filter in node.SelectNodes(HbmConstants.nsFilter, namespaceManager))
			{
				ParseFilter(filter, model, mappings);
			}

			XmlNode loader = node.SelectSingleNode(HbmConstants.nsLoader, namespaceManager);
			if (loader != null)
			{
				model.LoaderName = XmlHelper.GetAttributeValue(loader, "query-ref");
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
			bool isLazyTrue = lazyNode == null
			                  	?
			                  defaultLazy && fetchable.IsLazy
			                  	: //fetch="join" overrides default laziness
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

		protected static string GetEntityName(XmlNode elem, Mappings model)
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

		public static string GetClassNameWithoutAssembly(string unqualifiedName, Mappings model)
		{
			return TypeNameParser.Parse(unqualifiedName, model.DefaultNamespace, model.DefaultAssembly).Type;
		}

		public static string GetClassName(string unqualifiedName, Mappings model)
		{
			return ClassForNameChecked(unqualifiedName, model, "unknown class {0}").AssemblyQualifiedName;
			//return TypeNameParser.Parse(unqualifiedName, model.DefaultNamespace, model.DefaultAssembly).ToString();
		}

		private static void BindColumnsOrFormula(XmlNode node, SimpleValue simpleValue, string path, bool isNullable, Mappings mappings)
		{
			XmlAttribute formulaNode = node.Attributes["formula"];
			if (formulaNode != null)
			{
				Formula f = new Formula();
				f.FormulaString = formulaNode.InnerText;
				simpleValue.AddFormula(f);
			}
			else
			{
				BindColumns(node, simpleValue, isNullable, true, path, mappings);
			}
		}

		public static void BindManyToOne(XmlNode node, ManyToOne model, string defaultColumnName, bool isNullable,
		                                 Mappings mappings)
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

			string notFound = XmlHelper.GetAttributeValue(node, "not-found");
			model.IsIgnoreNotFound = "ignore".Equals(notFound);

			XmlAttribute typeNode = node.Attributes["class"];

			if (typeNode != null)
			{
				model.Type = TypeFactory.ManyToOne(
					ClassForNameChecked(typeNode.Value, mappings,
					                    "could not find class: {0}"),
					model.ReferencedPropertyName,
					model.IsLazy,
					model.IsIgnoreNotFound);
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
				foreach (XmlNode metaValue in node.SelectNodes(HbmConstants.nsMetaValue, namespaceManager))
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

			model.ForeignKeyDirection = (constrained
			                             	? ForeignKeyDirection.ForeignKeyFromParent
			                             	: ForeignKeyDirection.ForeignKeyToParent);

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
					model.ForeignKeyDirection,
					model.ReferencedPropertyName,
					model.IsLazy
					);
			}
		}

		public static void BindOneToMany(XmlNode node, OneToMany model, Mappings mappings)
		{
			model.ReferencedEntityName = ClassForNameChecked(node.Attributes["class"].Value, mappings,
			                                                 "associated class not found: {0}");

			string notFound = XmlHelper.GetAttributeValue(node, "not-found");
			model.IsIgnoreNotFound = "ignore".Equals(notFound);
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
		public static void BindArray(XmlNode node, Array model, string prefix, string path, System.Type containingType,
		                             Mappings mappings)
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

		public static void BindComponent(XmlNode node, Component model, System.Type reflectedClass, string className,
		                                 string path, bool isNullable, Mappings mappings)
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
					Mapping.Collection collection =
						collectType.Create(subnode, className, subpath, model.Owner, model.ComponentClass, mappings);
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

			IDictionary parameters = null;
			
			XmlAttribute typeAttribute = node.Attributes["type"];
			if (typeAttribute == null)
			{
				typeAttribute = node.Attributes["id-type"]; //for an any
			}
			string typeName = null;
			if (typeAttribute != null)
			{
				typeName = typeAttribute.Value;
			}
			else
			{
				XmlNode typeNode = node.SelectSingleNode(HbmConstants.nsType, namespaceManager);
				if (typeNode == null) //we will have to use reflection
					return null;
				XmlAttribute nameAttribute = typeNode.Attributes["name"]; //we know it exists because the schema validate it
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

		protected static void MakeIdentifier(XmlNode node, SimpleValue model, Mappings mappings)
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

		protected static Mapping.Property CreateProperty(IValue value, string propertyName, System.Type parentClass,
		                                               XmlNode subnode, Mappings mappings)
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

		/// <remarks>
		/// Called for Lists, arrays, primitive arrays
		/// </remarks>>
		public static void BindListSecondPass(XmlNode node, List model, IDictionary<System.Type, PersistentClass> classes, Mappings mappings)
		{
			BindCollectionSecondPass(node, model, classes, mappings);

			XmlNode subnode = node.SelectSingleNode(HbmConstants.nsIndex, namespaceManager);
			IntegerValue iv = new IntegerValue(model.CollectionTable);
			BindIntegerValue(subnode, iv, IndexedCollection.DefaultIndexColumnName, model.IsOneToMany, mappings);
			model.Index = iv;
		}

		public static void BindIdentifierCollectionSecondPass(XmlNode node, IdentifierCollection model,
		                                                      IDictionary<System.Type, PersistentClass> persitentClasses, Mappings mappings)
		{
			BindCollectionSecondPass(node, model, persitentClasses, mappings);

			XmlNode subnode = node.SelectSingleNode(HbmConstants.nsCollectionId, namespaceManager);
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
		public static void BindMapSecondPass(XmlNode node, Map model, IDictionary<System.Type, PersistentClass> classes, Mappings mappings)
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
		public static void BindCollectionSecondPass(XmlNode node, Mapping.Collection model, IDictionary<System.Type, PersistentClass> persistentClasses,
		                                            Mappings mappings)
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
					BindManyToManySubelements(model, subnode, mappings);
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
					XmlAttribute usageNode = subnode.Attributes["usage"];
					model.CacheConcurrencyStrategy = (usageNode != null) ? usageNode.Value : null;
					XmlAttribute regionNode = subnode.Attributes["region"];
					model.CacheRegionName = (regionNode != null) ? regionNode.Value : null;
				}
			}
		}

		public static void BindRoot(XmlDocument doc, Mappings mappings)
		{
			// note that the prefix has absolutely nothing to do with what the user
			// selects as their prefix in the document.  It is the prefix we use to 
			// build the XPath and the nsmgr takes care of translating our prefix into
			// the user defined prefix...
			namespaceManager = new XmlNamespaceManager(doc.NameTable);
			namespaceManager.AddNamespace(HbmConstants.nsPrefix, Configuration.MappingSchemaXMLNS);

			new MappingRootBinder(mappings, namespaceManager).Bind(doc.DocumentElement);
		}


		private class CollectionSecondPass : Cfg.CollectionSecondPass
		{
			protected XmlNode node;

			public CollectionSecondPass(XmlNode node, Mappings mappings, Mapping.Collection collection)
				: base(mappings, collection)
			{
				this.node = node;
			}

			public override void SecondPass(IDictionary<System.Type, PersistentClass> persistentClasses)
			{
				BindCollectionSecondPass(node, collection, persistentClasses, mappings);
			}
		}

		private class IdentifierCollectionSecondPass : CollectionSecondPass
		{
			public IdentifierCollectionSecondPass(XmlNode node, Mappings mappings, IdentifierCollection collection)
				: base(node, mappings, collection)
			{
			}

			public override void SecondPass(IDictionary<System.Type, PersistentClass> persistentClasses)
			{
				BindIdentifierCollectionSecondPass(node, (IdentifierCollection) collection, persistentClasses, mappings);
			}
		}

		private class MapSecondPass : CollectionSecondPass
		{
			public MapSecondPass(XmlNode node, Mappings mappings, Map collection)
				: base(node, mappings, collection)
			{
			}

			public override void SecondPass(IDictionary<System.Type, PersistentClass> persistentClasses)
			{
				BindMapSecondPass(node, (Map) collection, persistentClasses, mappings);
			}
		}

		private class SetSecondPass : CollectionSecondPass
		{
			public SetSecondPass(XmlNode node, Mappings mappings, Set collection)
				: base(node, mappings, collection)
			{
			}

			public override void SecondPass(IDictionary<System.Type, PersistentClass> persistentClasses)
			{
				BindSetSecondPass(node, (Set) collection, persistentClasses, mappings);
			}
		}

		private class ListSecondPass : CollectionSecondPass
		{
			public ListSecondPass(XmlNode node, Mappings mappings, List collection)
				: base(node, mappings, collection)
			{
			}

			public override void SecondPass(IDictionary<System.Type, PersistentClass> persistentClasses)
			{
				BindListSecondPass(node, (List) collection, persistentClasses, mappings);
			}
		}

		protected abstract class CollectionType
		{
			private string xmlTag;

			public abstract Mapping.Collection Create(XmlNode node, string className, string path, PersistentClass owner,
			                                          System.Type containingType, Mappings mappings);

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

				public override Mapping.Collection Create(XmlNode node, string prefix, string path, PersistentClass owner,
				                                          System.Type containingType, Mappings mappings)
				{
					Map map = new Map(owner);
					BindCollection(node, map, prefix, path, containingType, mappings);
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

				public override Mapping.Collection Create(XmlNode node, string prefix, string path, PersistentClass owner,
				                                          System.Type containingType, Mappings mappings)
				{
					Set setCollection = new Set(owner);
					BindCollection(node, setCollection, prefix, path, containingType, mappings);
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

				public override Mapping.Collection Create(XmlNode node, string prefix, string path, PersistentClass owner,
				                                          System.Type containingType, Mappings mappings)
				{
					List list = new List(owner);
					BindCollection(node, list, prefix, path, containingType, mappings);
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

				public override Mapping.Collection Create(XmlNode node, string prefix, string path, PersistentClass owner,
				                                          System.Type containingType, Mappings mappings)
				{
					Bag bag = new Bag(owner);
					BindCollection(node, bag, prefix, path, containingType, mappings);
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

				public override Mapping.Collection Create(XmlNode node, string prefix, string path, PersistentClass owner,
				                                          System.Type containingType, Mappings mappings)
				{
					IdentifierBag bag = new IdentifierBag(owner);
					BindCollection(node, bag, prefix, path, containingType, mappings);
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

				public override Mapping.Collection Create(XmlNode node, string prefix, string path, PersistentClass owner,
				                                          System.Type containingType, Mappings mappings)
				{
					Array array = new Array(owner);
					BindArray(node, array, prefix, path, containingType, mappings);
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

				public override Mapping.Collection Create(XmlNode node, string prefix, string path, PersistentClass owner,
				                                          System.Type containingType, Mappings mappings)
				{
					PrimitiveArray array = new PrimitiveArray(owner);
					BindArray(node, array, prefix, path, containingType, mappings);
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

		protected static IDictionary GetMetas(XmlNode node)
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

		public static void BindIntegerValue(XmlNode node, IntegerValue model, string defaultColumnName, bool isNullable,
		                                    Mappings mappings)
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

		protected static System.Type GetPropertyType(XmlNode definingNode, Mappings mappings,
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

		public static void BindSetSecondPass(XmlNode node, Set model, IDictionary<System.Type, PersistentClass> persistentClasses, Mappings mappings)
		{
			BindCollectionSecondPass(node, model, persistentClasses, mappings);

			if (!model.IsOneToMany)
			{
				model.CreatePrimaryKey();
			}
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

		private static void HandleCustomSQL(XmlNode node, Mapping.Collection model)
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

			element = node.SelectSingleNode(HbmConstants.nsSqlDeleteAll, namespaceManager);
			if (element != null)
			{
				bool callable = IsCallable(element);
				model.SetCustomSQLDeleteAll(element.InnerText.Trim(), callable, GetResultCheckStyle(element, callable));
			}
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
			{
				// use COUNT as the default.  This mimics the old behavior, although
				// NONE might be a better option moving forward in the case of callable
				return ExecuteUpdateResultCheckStyle.Count;
			}
			return ExecuteUpdateResultCheckStyle.Parse(attr.Value);
		}

		protected static void ParseFilter(XmlNode filterElement, IFilterable filterable, Mappings model)
		{
			string name = GetPropertyName(filterElement);
			string condition = filterElement.InnerText;
			if (condition == null || StringHelper.IsEmpty(condition.Trim()))
			{
				if (filterElement.Attributes != null)
				{
					XmlAttribute propertyNameNode = filterElement.Attributes["condition"];
					condition = (propertyNameNode == null) ? null : propertyNameNode.Value;
				}
			}

			//TODO: bad implementation, cos it depends upon ordering of mapping doc
			//      fixing this requires that Collection/PersistentClass gain access
			//      to the Mappings reference from Configuration (or the filterDefinitions
			//      map directly) sometime during Configuration.buildSessionFactory
			//      (after all the types/filter-defs are known and before building
			//      persisters).
			if (StringHelper.IsEmpty(condition))
			{
				condition = model.GetFilterDefinition(name).DefaultFilterCondition;
			}
			if (condition == null)
			{
				throw new MappingException("no filter condition found for filter: " + name);
			}
			log.Debug("Applying filter [" + name + "] as [" + condition + "]");
			filterable.AddFilter(name, condition);
		}

		private static void BindManyToManySubelements(Mapping.Collection collection,
		                                              XmlNode manyToManyNode,
		                                              Mappings model)
		{
			// Bind the where
			XmlAttribute where = manyToManyNode.Attributes["where"];
			String whereCondition = where == null ? null : where.Value;
			collection.ManyToManyWhere = whereCondition;

			// Bind the order-by
			XmlAttribute order = manyToManyNode.Attributes["order-by"];
			String orderFragment = order == null ? null : order.Value;
			collection.ManyToManyOrdering = orderFragment;

			// Bind the filters
			if ((manyToManyNode.SelectSingleNode(HbmConstants.nsFilter, namespaceManager) != null || whereCondition != null) &&
			    collection.FetchMode == FetchMode.Join &&
			    collection.Element.FetchMode != FetchMode.Join)
			{
				throw new MappingException(
					"many-to-many defining filter or where without join fetching " +
					"not valid within collection using join fetching [" + collection.Role + "]"
					);
			}
			foreach (XmlNode filterElement in manyToManyNode.SelectNodes(HbmConstants.nsFilter, namespaceManager))
			{
				string name = XmlHelper.GetAttributeValue(filterElement, "name");
				string condition = filterElement.InnerText.Trim();
				if (StringHelper.IsEmpty(condition))
					condition = XmlHelper.GetAttributeValue(filterElement, "condition");
				if (StringHelper.IsEmpty(condition))
				{
					condition = model.GetFilterDefinition(name).DefaultFilterCondition;
				}
				if (condition == null)
				{
					throw new MappingException("no filter condition found for filter: " + name);
				}
				log.Debug(
					"Applying many-to-many filter [" + name +
					"] as [" + condition +
					"] to role [" + collection.Role + "]"
					);
				collection.AddManyToManyFilter(name, condition);
			}
		}
	}
}
