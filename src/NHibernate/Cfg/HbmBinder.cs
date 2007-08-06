using System;
using System.Collections;
using System.Text;
using System.Xml;

using log4net;

using NHibernate.Cfg.XmlHbmBinding;
using NHibernate.Engine;
using NHibernate.Mapping;
using NHibernate.Property;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Cfg
{
	public class HbmBinder
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(HbmBinder));

		// Made internal to be accessible from Cfg.CollectionSecondPass
		private static XmlNamespaceManager namespaceManager;

		protected internal static Dialect.Dialect dialect;

		protected internal static XmlNamespaceManager NamespaceManager
		{
			get { return namespaceManager; }
			set { namespaceManager = value; }
		}

		protected internal static IType GetTypeFromXML(XmlNode node)
		{
			IType type;

			IDictionary parameters = null;
			
			XmlAttribute typeAttribute = node.Attributes["type"];
			if (typeAttribute == null)
			{
				typeAttribute = node.Attributes["id-type"]; //for an any
			}
			string typeName;
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

		protected internal static string GetEntityName(XmlNode elem, Mappings model)
		{
			//string entityName = XmlHelper.GetAttributeValue(elem, "entity-name");
			//return entityName == null ? GetClassName( elem.Attributes[ "class" ], model ) : entityName;
			return GetClassName(elem.Attributes["class"], model);
		}

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

		protected static void BindManyToOne(XmlNode node, ManyToOne model, string defaultColumnName, bool isNullable,
		                                 Mappings mappings)
		{
			BindColumns(node, model, isNullable, true, defaultColumnName, mappings);
			InitOuterJoinFetchSetting(node, model);
			InitLaziness(node, model, true);

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

		protected static void BindAny(XmlNode node, Any model, bool isNullable, Mappings mappings)
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
				}

				if (values.Count > 0)
				{
					model.MetaType = new MetaType(values, model.MetaType);
				}
			}

			BindColumns(node, model, isNullable, false, null, mappings);
		}

		protected static void BindOneToOne(XmlNode node, OneToOne model, bool isNullable, Mappings mappings)
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

		protected static string GetPropertyName(XmlNode node)
		{
			if (node.Attributes != null)
			{
				XmlAttribute propertyNameNode = node.Attributes["name"];
				return (propertyNameNode == null) ? null : propertyNameNode.Value;
			}
			return null;
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
				{
					table.AddColumn(col);
				} //table=null -> an association - fill it in later
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

		private static void BindIndex(XmlAttribute indexAttribute, Table table, Column column)
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

		private static void BindUniqueKey(XmlAttribute uniqueKeyAttribute, Table table, Column column)
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

		private static void InitLaziness(XmlNode node, ToOne fetchable, bool defaultLazy)
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
				InitLaziness(node, fetchable, "proxy", defaultLazy);
			}
		}

		private static string GetClassName(XmlAttribute att, Mappings model)
		{
			if (att == null)
			{
				return null;
			}
			return GetClassName(att.Value, model);
		}

		private static string GetClassName(string unqualifiedName, Mappings model)
		{
			return ClassForNameChecked(unqualifiedName, model, "unknown class {0}").AssemblyQualifiedName;
			//return TypeNameParser.Parse(unqualifiedName, model.DefaultNamespace, model.DefaultAssembly).ToString();
		}

		private static void BindColumnsOrFormula(XmlNode node, SimpleValue simpleValue, string path, bool isNullable,
			Mappings mappings)
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

		private static void BindColumn(XmlNode node, Column model, bool isNullable)
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

		protected static void InitOuterJoinFetchSetting(XmlNode node, IFetchable model)
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
	}
}
