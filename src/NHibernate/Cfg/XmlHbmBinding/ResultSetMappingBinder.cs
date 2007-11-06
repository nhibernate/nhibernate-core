using System;
using System.Collections;
using System.Collections.Generic;
using Iesi.Collections;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Engine;
using NHibernate.Engine.Query.Sql;
using NHibernate.Mapping;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Cfg.XmlHbmBinding
{
	public class ResultSetMappingBinder : Binder
	{
		public ResultSetMappingBinder(Mappings mappings)
			: base(mappings)
		{
		}

		public ResultSetMappingBinder(Binder parent)
			: base(parent)
		{
		}

		public ResultSetMappingDefinition Create(HbmResultSet resultSetSchema)
		{
			return Create(resultSetSchema.name, resultSetSchema.Items);
		}

		public ResultSetMappingDefinition Create(HbmSqlQuery sqlQuerySchema)
		{
			return Create(sqlQuerySchema.name, sqlQuerySchema.Items);
		}

		private ResultSetMappingDefinition Create(string name, object[] items)
		{
			ResultSetMappingDefinition definition = new ResultSetMappingDefinition(name);

			int count = 0;
			foreach (object item in items ?? new object[0])
			{
				count += 1;
				INativeSQLQueryReturn queryReturn = CreateQueryReturn(item, count);

				if (queryReturn != null)
					definition.AddQueryReturn(queryReturn);
			}

			return definition;
		}

		private INativeSQLQueryReturn CreateQueryReturn(object item, int count)
		{
			HbmLoadCollection loadCollectionSchema = item as HbmLoadCollection;
			HbmReturn returnSchema = item as HbmReturn;
			HbmReturnJoin returnJoinSchema = item as HbmReturnJoin;
			HbmReturnScalar returnScalarSchema = item as HbmReturnScalar;

			if (returnScalarSchema != null)
				return CreateScalarReturn(returnScalarSchema);

			else if (returnSchema != null)
				return CreateReturn(returnSchema, count);

			else if (returnJoinSchema != null)
				return CreateJoinReturn(returnJoinSchema);

			else if (loadCollectionSchema != null)
				return CreateLoadCollectionReturn(loadCollectionSchema);

			else
				return null;
		}

		private static INativeSQLQueryReturn CreateScalarReturn(HbmReturnScalar returnScalarSchema)
		{
			IType type = TypeFactory.HeuristicType(returnScalarSchema.type, null);

			if (type == null)
				throw new MappingException("could not interpret type: " + returnScalarSchema.type);

			return new NativeSQLQueryScalarReturn(returnScalarSchema.column, type);
		}

		private NativeSQLQueryRootReturn CreateReturn(HbmReturn returnSchema, int count)
		{
			String alias = returnSchema.alias;

			if (StringHelper.IsEmpty(alias))
				alias = "alias_" + count; // hack/workaround as sqlquery impl depend on having a key.

			string entityName = GetClassName(returnSchema.@class, mappings);

			if (entityName == null)
				throw new MappingException("<return alias='" + alias + "'> must specify either a class or entity-name");

			LockMode lockMode = GetLockMode(returnSchema.lockmode);

			PersistentClass pc = mappings.GetClass(ReflectHelper.ClassForName(entityName));
			IDictionary propertyResults =
				BindPropertyResults(alias, returnSchema.returndiscriminator, returnSchema.returnproperty, pc);

			return new NativeSQLQueryRootReturn(alias, entityName, propertyResults, lockMode);
		}

		private NativeSQLQueryJoinReturn CreateJoinReturn(HbmReturnJoin returnJoinSchema)
		{
			int dot = returnJoinSchema.property.LastIndexOf('.');

			if (dot == -1)
				throw new MappingException(
					"Role attribute for sql query return [alias=" + returnJoinSchema.alias +
						"] not formatted correctly {owningAlias.propertyName}"
					);

			string roleOwnerAlias = returnJoinSchema.property.Substring(0, dot);
			string roleProperty = returnJoinSchema.property.Substring(dot + 1);

			//FIXME: get the PersistentClass
			IDictionary propertyResults = BindPropertyResults(returnJoinSchema.alias, null, returnJoinSchema.Items, null);

			return new NativeSQLQueryJoinReturn(returnJoinSchema.alias, roleOwnerAlias, roleProperty,
				propertyResults, // TODO: bindpropertyresults(alias, returnElem)
				GetLockMode(returnJoinSchema.lockmode));
		}

		private NativeSQLQueryCollectionReturn CreateLoadCollectionReturn(HbmLoadCollection loadCollectionSchema)
		{
			int dot = loadCollectionSchema.role.LastIndexOf('.');

			if (dot == -1)
				throw new MappingException(
					"Collection attribute for sql query return [alias=" + loadCollectionSchema.alias +
						"] not formatted correctly {OwnerClassName.propertyName}"
					);

			string ownerClassName = GetClassNameWithoutAssembly(loadCollectionSchema.role.Substring(0, dot));
			string ownerPropertyName = loadCollectionSchema.role.Substring(dot + 1);

			//FIXME: get the PersistentClass
			IDictionary propertyResults = BindPropertyResults(loadCollectionSchema.alias, null, loadCollectionSchema.Items, null);

			return new NativeSQLQueryCollectionReturn(loadCollectionSchema.alias, ownerClassName, ownerPropertyName,
				propertyResults, GetLockMode(loadCollectionSchema.lockmode));
		}

		private IDictionary BindPropertyResults(string alias, HbmReturnDiscriminator discriminatorSchema,
			HbmReturnProperty[] returnProperties, PersistentClass pc)
		{
			IDictionary propertyresults = new Hashtable();
			// maybe a concrete SQLpropertyresult type, but Map is exactly what is required at the moment

			if (discriminatorSchema != null)
			{
				ArrayList resultColumns = GetResultColumns(discriminatorSchema);
				propertyresults["class"] = ArrayHelper.ToStringArray(resultColumns);
			}

			IList properties = new ArrayList();
			IList propertyNames = new ArrayList();

			foreach (HbmReturnProperty returnPropertySchema in returnProperties ?? new HbmReturnProperty[0])
			{
				String name = returnPropertySchema.name;
				if (pc == null || name.IndexOf('.') == -1)
				{
					//if dotted and not load-collection nor return-join
					//regular property
					properties.Add(returnPropertySchema);
					propertyNames.Add(name);
				}
				else
				{
					// Reorder properties
					// 1. get the parent property
					// 2. list all the properties following the expected one in the parent property
					// 3. calculate the lowest index and insert the property

					int dotIndex = name.LastIndexOf('.');
					string reducedName = name.Substring(0, dotIndex);
					IValue value = pc.GetRecursiveProperty(reducedName).Value;
					IEnumerable<Mapping.Property> parentPropIter;
					if (value is Component)
					{
						Component comp = (Component) value;
						parentPropIter = comp.PropertyCollection;
					}
					else if (value is ToOne)
					{
						ToOne toOne = (ToOne) value;
						PersistentClass referencedPc = mappings.GetClass(toOne.ReferencedEntityName);
						if (toOne.ReferencedPropertyName != null)
							try
							{
								parentPropIter =
									((Component) referencedPc.GetRecursiveProperty(toOne.ReferencedPropertyName).Value).PropertyCollection;
							}
							catch (InvalidCastException e)
							{
								throw new MappingException("dotted notation reference neither a component nor a many/one to one", e);
							}
						else
							try
							{
								parentPropIter = ((Component) referencedPc.IdentifierProperty.Value).PropertyCollection;
							}
							catch (InvalidCastException e)
							{
								throw new MappingException("dotted notation reference neither a component nor a many/one to one", e);
							}
					}
					else
						throw new MappingException("dotted notation reference neither a component nor a many/one to one");
					bool hasFollowers = false;
					IList followers = new ArrayList();
					foreach (Mapping.Property prop in parentPropIter)
					{
						String currentPropertyName = prop.Name;
						String currentName = reducedName + '.' + currentPropertyName;
						if (hasFollowers)
							followers.Add(currentName);
						if (name.Equals(currentName))
							hasFollowers = true;
					}

					int index = propertyNames.Count;
					int followersSize = followers.Count;
					for (int loop = 0; loop < followersSize; loop++)
					{
						String follower = (String) followers[loop];
						int currentIndex = GetIndexOfFirstMatchingProperty(propertyNames, follower);
						index = currentIndex != -1 && currentIndex < index ? currentIndex : index;
					}
					propertyNames.Insert(index, name);
					properties.Insert(index, returnPropertySchema);
				}
			}

			ISet uniqueReturnProperty = new HashedSet();
			foreach (HbmReturnProperty returnPropertySchema in properties)
			{
				string name = returnPropertySchema.name;
				if ("class".Equals(name))
					throw new MappingException(
						"class is not a valid property name to use in a <return-property>, use <return-discriminator> instead"
						);
				//TODO: validate existing of property with the chosen name. (secondpass )
				ArrayList allResultColumns = GetResultColumns(returnPropertySchema);

				if (allResultColumns.Count == 0)
					throw new MappingException(
						"return-property for alias " + alias +
							" must specify at least one column or return-column name"
						);
				if (uniqueReturnProperty.Contains(name))
					throw new MappingException(
						"duplicate return-property for property " + name +
							" on alias " + alias
						);
				uniqueReturnProperty.Add(name);

				// the issue here is that for <return-join/> representing an entity collection,
				// the collection element values (the property values of the associated entity)
				// are represented as 'element.{propertyname}'.  Thus the StringHelper.root()
				// here puts everything under 'element' (which additionally has significant
				// meaning).  Probably what we need to do is to something like this instead:
				//      String root = StringHelper.root( name );
				//      String key = root; // by default
				//      if ( !root.equals( name ) ) {
				//	        // we had a dot
				//          if ( !root.equals( alias ) {
				//              // the root does not apply to the specific alias
				//              if ( "elements".equals( root ) {
				//                  // we specifically have a <return-join/> representing an entity collection
				//                  // and this <return-property/> is one of that entity's properties
				//                  key = name;
				//              }
				//          }
				//      }
				// but I am not clear enough on the intended purpose of this code block, especially
				// in relation to the "Reorder properties" code block above... 
				//			String key = StringHelper.root( name );
				String key = name;
				ArrayList intermediateResults = (ArrayList) propertyresults[key];
				if (intermediateResults == null)
					propertyresults[key] = allResultColumns;
				else
					ArrayHelper.AddAll(intermediateResults, allResultColumns);
			}

			IDictionary newPropertyResults = new Hashtable();

			foreach (DictionaryEntry entry in propertyresults)
				if (entry.Value is ArrayList)
				{
					ArrayList list = (ArrayList) entry.Value;
					newPropertyResults[entry.Key] = ArrayHelper.ToStringArray(list);
				}
				else
					newPropertyResults[entry.Key] = entry.Value;
			return newPropertyResults.Count == 0 ? CollectionHelper.EmptyMap : newPropertyResults;
		}

		private static ArrayList GetResultColumns(HbmReturnProperty returnPropertySchema)
		{
			ArrayList allResultColumns = new ArrayList();
			String column = Unquote(returnPropertySchema.column);

			if (column != null)
				allResultColumns.Add(column);

			foreach (HbmReturnColumn returnColumnSchema in returnPropertySchema.returncolumn ?? new HbmReturnColumn[0])
				allResultColumns.Add(Unquote(returnColumnSchema.name));

			return allResultColumns;
		}

		private static ArrayList GetResultColumns(HbmReturnDiscriminator discriminatorSchema)
		{
			String column = Unquote(discriminatorSchema.column);
			ArrayList allResultColumns = new ArrayList();

			if (column != null)
				allResultColumns.Add(column);

			return allResultColumns;
		}

		private static LockMode GetLockMode(HbmLockMode lockMode)
		{
			switch (lockMode)
			{
				case HbmLockMode.None:
					return LockMode.None;

				case HbmLockMode.Read:
					return LockMode.Read;

				case HbmLockMode.Upgrade:
					return LockMode.Upgrade;

				case HbmLockMode.UpgradeNowait:
					return LockMode.UpgradeNoWait;

				case HbmLockMode.Write:
					return LockMode.Write;

				default:
					throw new MappingException("unknown lockMode " + lockMode);
			}
		}

		private static int GetIndexOfFirstMatchingProperty(IList propertyNames, string follower)
		{
			int propertySize = propertyNames.Count;
			for (int propIndex = 0; propIndex < propertySize; propIndex++)
				if (((String) propertyNames[propIndex]).StartsWith(follower))
					return propIndex;
			return -1;
		}

		private static String Unquote(String name)
		{
			if (name != null && name[0] == '`')
				name = name.Substring(1, name.Length - 2);
			return name;
		}

		private string GetClassNameWithoutAssembly(string unqualifiedName)
		{
			return TypeNameParser.Parse(unqualifiedName, mappings.DefaultNamespace, mappings.DefaultAssembly).Type;
		}
	}
}