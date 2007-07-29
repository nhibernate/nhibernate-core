using System;
using System.Collections;
using System.Xml;
using Iesi.Collections;
using NHibernate.Engine;
using NHibernate.Loader.Custom;
using NHibernate.Mapping;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Cfg
{
	public abstract class ResultSetMappingBinder
	{
		internal static ResultSetMappingDefinition BuildResultSetMappingDefinition(XmlNode resultSetElem, string path,
		                                                                            Mappings mappings)
		{
			string resultSetName = resultSetElem.Attributes["name"].Value;
			if (path != null)
			{
				resultSetName = path + '.' + resultSetName;
			}
			ResultSetMappingDefinition definition = new ResultSetMappingDefinition(resultSetName);

			int cnt = 0;
			foreach (XmlNode returnElem in resultSetElem.ChildNodes)
			{
				cnt++;
				string name = returnElem.LocalName;
				if ("return-scalar".Equals(name))
				{
					string column = XmlHelper.GetAttributeValue(returnElem, "column");
					IType type = HbmBinder.GetTypeFromXML(returnElem);
					definition.AddQueryReturn(new SQLQueryScalarReturn(column, type));
				}
				else if ("return".Equals(name))
				{
					definition.AddQueryReturn(BindReturn(returnElem, mappings, cnt));
				}
				else if ("return-join".Equals(name))
				{
					definition.AddQueryReturn(BindReturnJoin(returnElem, mappings));
				}
				else if ("load-collection".Equals(name))
				{
					definition.AddQueryReturn(BindLoadCollection(returnElem, mappings));
				}
			}
			return definition;
		}

		private static SQLQueryRootReturn BindReturn(XmlNode returnElem, Mappings mappings, int elementCount)
		{
			String alias = XmlHelper.GetAttributeValue(returnElem, "alias");
			if (StringHelper.IsEmpty(alias))
			{
				alias = "alias_" + elementCount; // hack/workaround as sqlquery impl depend on having a key.
			}

			string entityName = HbmBinder.GetEntityName(returnElem, mappings);
			if (entityName == null)
			{
				throw new MappingException("<return alias='" + alias + "'> must specify either a class or entity-name");
			}
			LockMode lockMode = GetLockMode(XmlHelper.GetAttributeValue(returnElem, "lock-mode"));

			PersistentClass pc = mappings.GetClass(ReflectHelper.ClassForName(entityName));
			IDictionary propertyResults = BindPropertyResults(alias, returnElem, pc, mappings);

			return new SQLQueryRootReturn(
				alias,
				entityName,
				propertyResults,
				lockMode
				);
		}

		private static SQLQueryJoinReturn BindReturnJoin(XmlNode returnElem, Mappings mappings)
		{
			String alias = XmlHelper.GetAttributeValue(returnElem, "alias");
			String roleAttribute = XmlHelper.GetAttributeValue(returnElem, "property");
			LockMode lockMode = GetLockMode(XmlHelper.GetAttributeValue(returnElem, "lock-mode"));
			int dot = roleAttribute.LastIndexOf('.');
			if (dot == -1)
			{
				throw new MappingException(
					"Role attribute for sql query return [alias=" + alias +
					"] not formatted correctly {owningAlias.propertyName}"
					);
			}
			string roleOwnerAlias = roleAttribute.Substring(0, dot);
			string roleProperty = roleAttribute.Substring(dot + 1);

			//FIXME: get the PersistentClass
			IDictionary propertyResults = BindPropertyResults(alias, returnElem, null, mappings);

			return new SQLQueryJoinReturn(
				alias,
				roleOwnerAlias,
				roleProperty,
				propertyResults, // TODO: bindpropertyresults(alias, returnElem)
				lockMode
				);
		}

		private static SQLQueryCollectionReturn BindLoadCollection(XmlNode returnElem, Mappings mappings)
		{
			string alias = XmlHelper.GetAttributeValue(returnElem, "alias");
			string collectionAttribute = XmlHelper.GetAttributeValue(returnElem, "role");
			LockMode lockMode = GetLockMode(XmlHelper.GetAttributeValue(returnElem, "lock-mode"));
			int dot = collectionAttribute.LastIndexOf('.');
			if (dot == -1)
			{
				throw new MappingException(
					"Collection attribute for sql query return [alias=" + alias +
					"] not formatted correctly {OwnerClassName.propertyName}"
					);
			}
			string ownerClassName = HbmBinder.GetClassNameWithoutAssembly(collectionAttribute.Substring(0, dot), mappings);
			string ownerPropertyName = collectionAttribute.Substring(dot + 1);

			//FIXME: get the PersistentClass
			IDictionary propertyResults = BindPropertyResults(alias, returnElem, null, mappings);

			return new SQLQueryCollectionReturn(
				alias,
				ownerClassName,
				ownerPropertyName,
				propertyResults,
				lockMode
				);
		}

		private static IDictionary BindPropertyResults(
			String alias, XmlNode returnElement, PersistentClass pc, Mappings mappings
			)
		{
			IDictionary propertyresults = new Hashtable();
				// maybe a concrete SQLpropertyresult type, but Map is exactly what is required at the moment

			XmlNode discriminatorResult =
				returnElement.SelectSingleNode(HbmConstants.nsReturnDiscriminator, HbmBinder.NamespaceManager);
			if (discriminatorResult != null)
			{
				ArrayList resultColumns = GetResultColumns(discriminatorResult);
				propertyresults["class"] = ArrayHelper.ToStringArray(resultColumns);
			}
			IList properties = new ArrayList();
			IList propertyNames = new ArrayList();

			foreach (
				XmlNode propertyresult in returnElement.SelectNodes(HbmConstants.nsReturnProperty, HbmBinder.NamespaceManager))
			{
				String name = XmlHelper.GetAttributeValue(propertyresult, "name");
				if (pc == null || name.IndexOf('.') == -1)
				{
					//if dotted and not load-collection nor return-join
					//regular property
					properties.Add(propertyresult);
					propertyNames.Add(name);
				}
				else
				{
					// Reorder properties
					// 1. get the parent property
					// 2. list all the properties following the expected one in the parent property
					// 3. calculate the lowest index and insert the property

					if (pc == null)
						throw new MappingException("dotted notation in <return-join> or <load-collection> not yet supported");
					int dotIndex = name.LastIndexOf('.');
					string reducedName = name.Substring(0, dotIndex);
					IValue value = pc.GetRecursiveProperty(reducedName).Value;
					ICollection parentPropIter;
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
						{
							try
							{
								parentPropIter =
									((Component) referencedPc.GetRecursiveProperty(toOne.ReferencedPropertyName).Value).PropertyCollection;
							}
							catch (InvalidCastException e)
							{
								throw new MappingException("dotted notation reference neither a component nor a many/one to one", e);
							}
						}
						else
						{
							try
							{
								parentPropIter = ((Component) referencedPc.IdentifierProperty.Value).PropertyCollection;
							}
							catch (InvalidCastException e)
							{
								throw new MappingException("dotted notation reference neither a component nor a many/one to one", e);
							}
						}
					}
					else
					{
						throw new MappingException("dotted notation reference neither a component nor a many/one to one");
					}
					bool hasFollowers = false;
					IList followers = new ArrayList();
					foreach (Mapping.Property prop in parentPropIter)
					{
						String currentPropertyName = prop.Name;
						String currentName = reducedName + '.' + currentPropertyName;
						if (hasFollowers)
						{
							followers.Add(currentName);
						}
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
					properties.Insert(index, propertyresult);
				}
			}

			ISet uniqueReturnProperty = new HashedSet();
			foreach (XmlNode propertyresult in properties)
			{
				string name = XmlHelper.GetAttributeValue(propertyresult, "name");
				if ("class".Equals(name))
				{
					throw new MappingException(
						"class is not a valid property name to use in a <return-property>, use <return-discriminator> instead"
						);
				}
				//TODO: validate existing of property with the chosen name. (secondpass )
				ArrayList allResultColumns = GetResultColumns(propertyresult);

				if (allResultColumns.Count == 0)
				{
					throw new MappingException(
						"return-property for alias " + alias +
						" must specify at least one column or return-column name"
						);
				}
				if (uniqueReturnProperty.Contains(name))
				{
					throw new MappingException(
						"duplicate return-property for property " + name +
						" on alias " + alias
						);
				}
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
				{
					propertyresults[key] = allResultColumns;
				}
				else
				{
					ArrayHelper.AddAll(intermediateResults, allResultColumns);
				}
			}

			IDictionary newPropertyResults = new Hashtable();

			foreach (DictionaryEntry entry in propertyresults)
			{
				if (entry.Value is ArrayList)
				{
					ArrayList list = (ArrayList) entry.Value;
					newPropertyResults[entry.Key] = ArrayHelper.ToStringArray(list);
				}
				else
				{
					newPropertyResults[entry.Key] = entry.Value;
				}
			}
			return newPropertyResults.Count == 0 ? CollectionHelper.EmptyMap : newPropertyResults;
		}

		private static int GetIndexOfFirstMatchingProperty(IList propertyNames, string follower)
		{
			int propertySize = propertyNames.Count;
			for (int propIndex = 0; propIndex < propertySize; propIndex++)
			{
				if (((String) propertyNames[propIndex]).StartsWith(follower))
				{
					return propIndex;
				}
			}
			return -1;
		}

		private static ArrayList GetResultColumns(XmlNode propertyresult)
		{
			String column = Unquote(XmlHelper.GetAttributeValue(propertyresult, "column"));
			ArrayList allResultColumns = new ArrayList();
			if (column != null)
			{
				allResultColumns.Add(column);
			}
			foreach (XmlNode element in propertyresult.SelectNodes(HbmConstants.nsReturnColumn, HbmBinder.NamespaceManager))
			{
				allResultColumns.Add(Unquote(XmlHelper.GetAttributeValue(element, "name")));
			}
			return allResultColumns;
		}

		private static String Unquote(String name)
		{
			if (name != null && name[0] == '`')
			{
				name = name.Substring(1, name.Length - 2);
			}
			return name;
		}

		private static LockMode GetLockMode(string lockMode)
		{
			if (lockMode == null || "read".Equals(lockMode))
			{
				return LockMode.Read;
			}
			else if ("none".Equals(lockMode))
			{
				return LockMode.None;
			}
			else if ("upgrade".Equals(lockMode))
			{
				return LockMode.Upgrade;
			}
			else if ("upgrade-nowait".Equals(lockMode))
			{
				return LockMode.UpgradeNoWait;
			}
			else if ("write".Equals(lockMode))
			{
				return LockMode.Write;
			}
				//else if ("force".equals(lockMode))
				//{
				//    return LockMode.FORCE;
				//}
			else
			{
				throw new MappingException("unknown lockmode " + lockMode);
			}
		}
	}
}