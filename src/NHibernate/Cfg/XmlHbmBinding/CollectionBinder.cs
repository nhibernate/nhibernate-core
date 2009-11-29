using System;
using System.Collections.Generic;
using System.Xml;

using NHibernate.Mapping;
using NHibernate.Type;
using NHibernate.Util;

using Array = NHibernate.Mapping.Array;
using NHibernate.Cfg.MappingSchema;

namespace NHibernate.Cfg.XmlHbmBinding
{
	public class CollectionBinder : ClassBinder
	{
		private static readonly HashSet<System.Type> SupportedCollections = new HashSet<System.Type>
		                                                                    	{
		                                                                    		typeof (HbmMap),
		                                                                    		typeof (HbmBag),
		                                                                    		typeof (HbmIdbag),
		                                                                    		typeof (HbmSet),
		                                                                    		typeof (HbmList),
		                                                                    		typeof (HbmArray),
		                                                                    		typeof (HbmPrimitiveArray)
		                                                                    	};
		private readonly IDictionary<string, CreateCollectionCommand> createCollectionCommands =
			new Dictionary<string, CreateCollectionCommand>();

		public static bool IsSupported(System.Type type)
		{
			return SupportedCollections.Contains(type);
		}

		public CollectionBinder(ClassBinder parent)
			: base(parent)
		{
			CreateCommandCollection();
		}

		private void CreateCommandCollection()
		{
			createCollectionCommands.Add("map", CreateMap);
			createCollectionCommands.Add("bag", CreateBag);
			createCollectionCommands.Add("idbag", CreateIdentifierBag);
			createCollectionCommands.Add("set", CreateSet);
			createCollectionCommands.Add("list", CreateList);
			createCollectionCommands.Add("array", CreateArray);
			createCollectionCommands.Add("primitive-array", CreatePrimitiveArray);
		}

		public bool CanCreate(string xmlTagName)
		{
			return createCollectionCommands.ContainsKey(xmlTagName);
		}

		public Mapping.Collection Create(ICollectionPropertyMapping collectionMapping, string className,
			string path, PersistentClass owner, System.Type containingType, IDictionary<string, MetaAttribute> inheritedMetas)
		{
			var collectionType = collectionMapping.GetType();
			if (collectionType == typeof (HbmBag))
			{
				return createCollectionCommands["bag"](Serialize(typeof (HbmBag), collectionMapping), className, path, owner,
				                                       containingType, inheritedMetas);
			}
			else if (collectionType == typeof (HbmSet))
			{
				return createCollectionCommands["set"](Serialize(typeof(HbmSet), collectionMapping), className, path, owner,
				                                       containingType, inheritedMetas);
			}
			else if (collectionType == typeof (HbmList))
			{
				return createCollectionCommands["list"](Serialize(typeof(HbmList), collectionMapping), className, path, owner,
				                                       containingType, inheritedMetas);
			}
			else if (collectionType == typeof (HbmMap))
			{
				return createCollectionCommands["map"](Serialize(typeof(HbmMap), collectionMapping), className, path, owner,
				                                       containingType, inheritedMetas);
			}
			else if (collectionType == typeof (HbmIdbag))
			{
				return createCollectionCommands["idbag"](Serialize(typeof(HbmIdbag), collectionMapping), className, path, owner,
				                                       containingType, inheritedMetas);
			}
			else if (collectionType == typeof (HbmArray))
			{
				return createCollectionCommands["array"](Serialize(typeof(HbmArray), collectionMapping), className, path, owner,
				                                       containingType, inheritedMetas);
			}
			else if (collectionType == typeof (HbmPrimitiveArray))
			{
				return createCollectionCommands["primitive-array"](Serialize(typeof(HbmPrimitiveArray), collectionMapping), className, path, owner,
				                                       containingType, inheritedMetas);
			}
			throw new MappingException("Not supported collection mapping element:" + collectionType);
		}


		public Mapping.Collection Create(string xmlTagName, XmlNode node, string className,
			string path, PersistentClass owner, System.Type containingType, IDictionary<string, MetaAttribute> inheritedMetas)
		{
			CreateCollectionCommand command = createCollectionCommands[xmlTagName];
			return command(node, className, path, owner, containingType, inheritedMetas);
		}

		private Mapping.Collection CreateMap(XmlNode node, string prefix, string path,
			PersistentClass owner, System.Type containingType, IDictionary<string, MetaAttribute> inheritedMetas)
		{
			Map map = new Map(owner);
			BindCollection(node, map, prefix, path, containingType, inheritedMetas);
			return map;
		}

		private Mapping.Collection CreateSet(XmlNode node, string prefix, string path,
			PersistentClass owner, System.Type containingType, IDictionary<string, MetaAttribute> inheritedMetas)
		{
			Set setCollection = new Set(owner);
			BindCollection(node, setCollection, prefix, path, containingType, inheritedMetas);
			return setCollection;
		}

		private Mapping.Collection CreateList(XmlNode node, string prefix, string path,
			PersistentClass owner, System.Type containingType, IDictionary<string, MetaAttribute> inheritedMetas)
		{
			List list = new List(owner);
			BindCollection(node, list, prefix, path, containingType, inheritedMetas);
			return list;
		}

		private Mapping.Collection CreateBag(XmlNode node, string prefix, string path,
			PersistentClass owner, System.Type containingType, IDictionary<string, MetaAttribute> inheritedMetas)
		{
			Bag bag = new Bag(owner);
			BindCollection(node, bag, prefix, path, containingType, inheritedMetas);
			return bag;
		}

		private Mapping.Collection CreateIdentifierBag(XmlNode node, string prefix, string path,
			PersistentClass owner, System.Type containingType, IDictionary<string, MetaAttribute> inheritedMetas)
		{
			IdentifierBag bag = new IdentifierBag(owner);
			BindCollection(node, bag, prefix, path, containingType, inheritedMetas);
			return bag;
		}

		private Mapping.Collection CreateArray(XmlNode node, string prefix, string path,
			PersistentClass owner, System.Type containingType, IDictionary<string, MetaAttribute> inheritedMetas)
		{
			Array array = new Array(owner);
			BindArray(node, array, prefix, path, containingType, inheritedMetas);
			return array;
		}

		private Mapping.Collection CreatePrimitiveArray(XmlNode node, string prefix, string path,
			PersistentClass owner, System.Type containingType, IDictionary<string, MetaAttribute> inheritedMetas)
		{
			PrimitiveArray array = new PrimitiveArray(owner);
			BindArray(node, array, prefix, path, containingType,inheritedMetas);
			return array;
		}

		/// <remarks>
		/// Called for all collections. <paramref name="containingType" /> parameter
		/// was added in NH to allow for reflection related to generic types.
		/// </remarks>
		private void BindCollection(XmlNode node, Mapping.Collection model, string className,
			string path, System.Type containingType, IDictionary<string, MetaAttribute> inheritedMetas)
		{
			// ROLENAME
			model.Role = StringHelper.Qualify(className, path);
			// TODO: H3.1 has just collection.setRole(path) here - why?

			XmlAttribute inverseNode = node.Attributes["inverse"];
			if (inverseNode != null)
				model.IsInverse = StringHelper.BooleanValue(inverseNode.Value);

			// TODO: H3.1 - not ported: mutable
			XmlAttribute olNode = node.Attributes["optimistic-lock"];
			model.IsOptimisticLocked = olNode == null || "true".Equals(olNode.Value);

			XmlAttribute orderNode = node.Attributes["order-by"];
			if (orderNode != null)
				model.OrderBy = orderNode.Value;
			XmlAttribute whereNode = node.Attributes["where"];
			if (whereNode != null)
				model.Where = whereNode.Value;
			XmlAttribute batchNode = node.Attributes["batch-size"];
			if (batchNode != null)
				model.BatchSize = int.Parse(batchNode.Value);

			// PERSISTER
			XmlAttribute persisterNode = node.Attributes["persister"];
			if (persisterNode == null)
			{
				//persister = CollectionPersisterImpl.class;
			}
			else
				model.CollectionPersisterClass = ClassForNameChecked(
					persisterNode.Value, mappings,
					"could not instantiate collection persister class: {0}");

			XmlAttribute typeNode = node.Attributes["collection-type"];
			if (typeNode != null)
			{
				string typeName = typeNode.Value;
				TypeDef typeDef = mappings.GetTypeDef(typeName);
				if (typeDef != null)
				{
					model.TypeName = typeDef.TypeClass;
					model.TypeParameters = typeDef.Parameters;
				}
				else
				{
					model.TypeName = FullQualifiedClassName(typeName, mappings);
				}
			}

			// FETCH STRATEGY
			InitOuterJoinFetchSetting(node, model);

			if ("subselect".Equals(XmlHelper.GetAttributeValue(node, "fetch")))
			{
				model.IsSubselectLoadable = true;
				model.Owner.HasSubselectLoadableCollections = true;
			}

			// LAZINESS
			InitLaziness(node, model, "true", mappings.DefaultLazy);
			XmlAttribute lazyNode = node.Attributes["lazy"];
			if (lazyNode != null && "extra".Equals(lazyNode.Value))
			{
				model.IsLazy = true;
				model.ExtraLazy = true;
			}

			XmlNode oneToManyNode = node.SelectSingleNode(HbmConstants.nsOneToMany, namespaceManager);
			if (oneToManyNode != null)
			{
				OneToMany oneToMany = new OneToMany(model.Owner);
				model.Element = oneToMany;
				BindOneToMany(oneToManyNode, oneToMany);
				//we have to set up the table later!! yuck
			}
			else
			{
				//TABLE
				XmlAttribute tableNode = node.Attributes["table"];
				string tableName;
				if (tableNode != null)
					tableName = mappings.NamingStrategy.TableName(tableNode.Value);
				else
					tableName = mappings.NamingStrategy.PropertyToTableName(className, path);
				XmlAttribute schemaNode = node.Attributes["schema"];
				string schema = schemaNode == null ? mappings.SchemaName : schemaNode.Value;
				XmlAttribute catalogNode = node.Attributes["catalog"];
				string catalog = catalogNode == null ? mappings.CatalogName : catalogNode.Value;

				XmlAttribute actionNode = node.Attributes["schema-action"];
				string action = actionNode == null ? "all" : actionNode.Value;
                
				model.CollectionTable = mappings.AddTable(schema, catalog, tableName, null, false, action);

				log.InfoFormat("Mapping collection: {0} -> {1}", model.Role, model.CollectionTable.Name);
			}

			//SORT
			XmlAttribute sortedAtt = node.Attributes["sort"];
			// unsorted, natural, comparator.class.name
			if (sortedAtt == null || sortedAtt.Value.Equals("unsorted"))
				model.IsSorted = false;
			else
			{
				model.IsSorted = true;
				if (!sortedAtt.Value.Equals("natural"))
				{
					string comparatorClassName = FullQualifiedClassName(sortedAtt.Value, mappings);
					model.ComparerClassName = comparatorClassName;
				}
			}

			//ORPHAN DELETE (used for programmer error detection)
			XmlAttribute cascadeAtt = node.Attributes["cascade"];
			if (cascadeAtt != null && cascadeAtt.Value.IndexOf("delete-orphan") >= 0)
				model.HasOrphanDelete = true;

			bool? isGeneric = null;

			XmlAttribute genericAtt = node.Attributes["generic"];
			if (genericAtt != null)
				isGeneric = bool.Parse(genericAtt.Value);

			System.Type collectionType = null;

			if (!isGeneric.HasValue && containingType != null)
			{
				collectionType = GetPropertyType(node, containingType, GetPropertyName(node));
				isGeneric = collectionType.IsGenericType;
			}

			model.IsGeneric = isGeneric ?? false;

			if (model.IsGeneric)
			{
				// Determine the generic arguments using reflection
				if (collectionType == null)
					collectionType = GetPropertyType(node, containingType, GetPropertyName(node));
				System.Type[] genericArguments = collectionType.GetGenericArguments();
				model.GenericArguments = genericArguments;
			}

			HandleCustomSQL(node, model);

			//set up second pass
			if (model is List)
				AddListSecondPass(node, (List)model, inheritedMetas);

			else if (model is Map)
				AddMapSecondPass(node, (Map)model, inheritedMetas);

			else if (model is Set)
				AddSetSecondPass(node, (Set)model, inheritedMetas);

			else if (model is IdentifierCollection)
				AddIdentifierCollectionSecondPass(node, (IdentifierCollection)model, inheritedMetas);

			else
				AddCollectionSecondPass(node, model, inheritedMetas);

			foreach (XmlNode filter in node.SelectNodes(HbmConstants.nsFilter, namespaceManager))
				ParseFilter(filter, model);

			XmlNode loader = node.SelectSingleNode(HbmConstants.nsLoader, namespaceManager);
			if (loader != null)
				model.LoaderName = XmlHelper.GetAttributeValue(loader, "query-ref");

			XmlNode key = node.SelectSingleNode(HbmConstants.nsKey, namespaceManager);
			if (key != null)
				model.ReferencedPropertyName = XmlHelper.GetAttributeValue(key, "property-ref");

		}

		/// <remarks>
		/// Called for arrays and primitive arrays
		/// </remarks>
		private void BindArray(XmlNode node, Array model, string prefix, string path,
			System.Type containingType, IDictionary<string, MetaAttribute> inheritedMetas)
		{
			BindCollection(node, model, prefix, path, containingType, inheritedMetas);

			XmlAttribute att = node.Attributes["element-class"];

			if (att != null)
				model.ElementClassName = GetQualifiedClassName(att.Value, mappings);
			else
				foreach (XmlNode subnode in node.ChildNodes)
				{
					// TODO NH: mmm.... the code below, maybe, must be resolved by SecondPass (not here)
					string name = subnode.LocalName; //.Name;

					//I am only concerned with elements that are from the nhibernate namespace
					if (subnode.NamespaceURI != MappingSchemaXMLNS)
						continue;

					switch (name)
					{
						case "element":
							string typeName;
							XmlAttribute typeAttribute = subnode.Attributes["type"];
							if (typeAttribute != null)
								typeName = typeAttribute.Value;
							else
								throw new MappingException("type for <element> was not defined");
							IType type = TypeFactory.HeuristicType(typeName, null);
							if (type == null)
								throw new MappingException("could not interpret type: " + typeName);

							model.ElementClassName = type.ReturnedClass.AssemblyQualifiedName;
							break;

						case "one-to-many":
						case "many-to-many":
						case "composite-element":
							model.ElementClassName = GetQualifiedClassName(subnode.Attributes["class"].Value, mappings);
							break;
					}
				}
		}

		private void AddListSecondPass(XmlNode node, List model, IDictionary<string, MetaAttribute> inheritedMetas)
		{
			mappings.AddSecondPass(delegate(IDictionary<string, PersistentClass> persistentClasses)
				{
					PreCollectionSecondPass(model);
					BindListSecondPass(node, model, persistentClasses, inheritedMetas);
					PostCollectionSecondPass(model);
				});
		}

		private void AddMapSecondPass(XmlNode node, Map model, IDictionary<string, MetaAttribute> inheritedMetas)
		{
			mappings.AddSecondPass(delegate(IDictionary<string, PersistentClass> persistentClasses)
				{
					PreCollectionSecondPass(model);
					BindMapSecondPass(node, model, persistentClasses, inheritedMetas);
					PostCollectionSecondPass(model);
				});
		}

		private void AddSetSecondPass(XmlNode node, Set model, IDictionary<string, MetaAttribute> inheritedMetas)
		{
			mappings.AddSecondPass(delegate(IDictionary<string, PersistentClass> persistentClasses)
				{
					PreCollectionSecondPass(model);
					BindSetSecondPass(node, model, persistentClasses, inheritedMetas);
					PostCollectionSecondPass(model);
				});
		}

		private void AddIdentifierCollectionSecondPass(XmlNode node, IdentifierCollection model, IDictionary<string, MetaAttribute> inheritedMetas)
		{
			mappings.AddSecondPass(delegate(IDictionary<string, PersistentClass> persistentClasses)
				{
					PreCollectionSecondPass(model);
					BindIdentifierCollectionSecondPass(node, model, persistentClasses, inheritedMetas);
					PostCollectionSecondPass(model);
				});
		}

		private void AddCollectionSecondPass(XmlNode node, Mapping.Collection model, IDictionary<string, MetaAttribute> inheritedMetas)
		{
			mappings.AddSecondPass(delegate(IDictionary<string, PersistentClass> persistentClasses)
				{
					PreCollectionSecondPass(model);
					BindCollectionSecondPass(node, model, persistentClasses, inheritedMetas);
					PostCollectionSecondPass(model);
				});
		}

		private void HandleCustomSQL(XmlNode node, Mapping.Collection model)
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

		private static void PreCollectionSecondPass(Mapping.Collection collection)
		{
			if (log.IsDebugEnabled)
				log.Debug("Second pass for collection: " + collection.Role);
		}

		private static void PostCollectionSecondPass(Mapping.Collection collection)
		{
			collection.CreateAllKeys();

			if (log.IsDebugEnabled)
			{
				string msg = "Mapped collection key: " + Columns(collection.Key);
				if (collection.IsIndexed)
					msg += ", index: " + Columns(((IndexedCollection)collection).Index);
				if (collection.IsOneToMany)
					msg += ", one-to-many: " + collection.Element.Type.Name;
				else
				{
					msg += ", element: " + Columns(collection.Element);
					msg += ", type: " + collection.Element.Type.Name;
				}
				log.Debug(msg);
			}
		}

		private void BindListSecondPass(XmlNode node, List model,
			IDictionary<string, PersistentClass> persistentClasses, IDictionary<string, MetaAttribute> inheritedMetas)
		{
			BindCollectionSecondPass(node, model, persistentClasses, inheritedMetas);

			XmlNode subnode = node.SelectSingleNode(HbmConstants.nsListIndex, namespaceManager);
			if (subnode == null) { subnode = node.SelectSingleNode(HbmConstants.nsIndex, namespaceManager); }
			SimpleValue iv = new SimpleValue(model.CollectionTable);
			BindIntegerValue(subnode, iv, IndexedCollection.DefaultIndexColumnName, model.IsOneToMany);
			model.Index = iv;

			string baseIndex = XmlHelper.GetAttributeValue(subnode, "base");
			if (baseIndex != null) { model.BaseIndex = Convert.ToInt32(baseIndex); }
		}

		private void BindOneToMany(XmlNode node, OneToMany model)
		{
			model.ReferencedEntityName = GetEntityName(node, mappings);

			string notFound = XmlHelper.GetAttributeValue(node, "not-found");
			model.IsIgnoreNotFound = "ignore".Equals(notFound);
		}

		private void BindIdentifierCollectionSecondPass(XmlNode node, IdentifierCollection model,
			IDictionary<string, PersistentClass> persitentClasses, IDictionary<string, MetaAttribute> inheritedMetas)
		{
			BindCollectionSecondPass(node, model, persitentClasses, inheritedMetas);

			XmlNode subnode = node.SelectSingleNode(HbmConstants.nsCollectionId, namespaceManager);
			SimpleValue id = new SimpleValue(model.CollectionTable);
			BindSimpleValue(subnode, id, false, IdentifierCollection.DefaultIdentifierColumnName);
			model.Identifier = id;
			MakeIdentifier(subnode, id);
		}

		private void BindIntegerValue(XmlNode node, SimpleValue model, string defaultColumnName,
			bool isNullable)
		{
			BindSimpleValue(node, model, isNullable, defaultColumnName);

			if (model.ColumnSpan > 1)
				log.Error("This shouldn't happen, check BindIntegerValue");
			model.TypeName = NHibernateUtil.Int32.Name;
		}

		private void BindSetSecondPass(XmlNode node, Set model,
			IDictionary<string, PersistentClass> persistentClasses, IDictionary<string, MetaAttribute> inheritedMetas)
		{
			BindCollectionSecondPass(node, model, persistentClasses, inheritedMetas);

			if (!model.IsOneToMany)
				model.CreatePrimaryKey();
		}

		/// <summary>
		/// Called for Maps
		/// </summary>
		private void BindMapSecondPass(XmlNode node, Map model,
			IDictionary<string, PersistentClass> persistentClasses, IDictionary<string, MetaAttribute> inheritedMetas)
		{
			BindCollectionSecondPass(node, model, persistentClasses, inheritedMetas);

			foreach (XmlNode subnode in node.ChildNodes)
			{
				//I am only concerned with elements that are from the nhibernate namespace
				if (subnode.NamespaceURI != MappingSchemaXMLNS)
					continue;

				string name = subnode.LocalName; //.Name;

				if ("index".Equals(name) || "map-key".Equals(name))
				{
					SimpleValue value = new SimpleValue(model.CollectionTable);
					BindSimpleValue(subnode, value, model.IsOneToMany, IndexedCollection.DefaultIndexColumnName);
					model.Index = value;
					if (model.Index.Type == null)
						throw new MappingException("map index element must specify a type: " + model.Role);
				}
				else if ("index-many-to-many".Equals(name) || "map-key-many-to-many".Equals(name))
				{
					ManyToOne mto = new ManyToOne(model.CollectionTable);
					BindManyToOne(subnode, mto, IndexedCollection.DefaultIndexColumnName, model.IsOneToMany);
					model.Index = mto;
				}
				else if ("composite-index".Equals(name) || "composite-map-key".Equals(name))
				{
					Component component = new Component(model);
					BindComponent(subnode, component, null, null, null, model.Role + ".index", model.IsOneToMany,inheritedMetas);
					model.Index = component;
				}
				else if ("index-many-to-any".Equals(name))
				{
					Any any = new Any(model.CollectionTable);
					BindAny(subnode, any, model.IsOneToMany);
					model.Index = any;
				}
			}
		}

		/// <remarks>
		/// Called for all collections
		/// </remarks>
		private void BindCollectionSecondPass(XmlNode node, Mapping.Collection model,
			IDictionary<string, PersistentClass> persistentClasses, IDictionary<string, MetaAttribute> inheritedMetas)
		{
			if (model.IsOneToMany)
			{
				OneToMany oneToMany = (OneToMany)model.Element;
				string associatedEntityName = oneToMany.ReferencedEntityName;
				PersistentClass persistentClass;
				if (persistentClasses.TryGetValue(associatedEntityName, out persistentClass) == false)
					throw new MappingException("Association references unmapped class: " + associatedEntityName);
				oneToMany.AssociatedClass = persistentClass;
				model.CollectionTable = persistentClass.Table;

				if (log.IsInfoEnabled)
					log.Info("mapping collection: " + model.Role + " -> " + model.CollectionTable.Name);
			}

			//CHECK
			XmlAttribute chNode = node.Attributes["check"];
			if (chNode != null)
				model.CollectionTable.AddCheckConstraint(chNode.Value);

			//contained elements:
			foreach (XmlNode subnode in node.ChildNodes)
			{
				//I am only concerned with elements that are from the nhibernate namespace
				if (subnode.NamespaceURI != MappingSchemaXMLNS)
					continue;

				string name = subnode.LocalName; //.Name;

				if ("key".Equals(name) || "generated-key".Equals(name))
				{
					string propRef = model.ReferencedPropertyName;
					IKeyValue keyValue;
					if (propRef == null)
					{
						keyValue = model.Owner.Identifier;
					}
					else
					{
						keyValue = (IKeyValue)model.Owner.GetProperty(propRef).Value;
					}
					DependantValue key = new DependantValue(model.CollectionTable, keyValue);
					if (subnode.Attributes["on-delete"] != null)
						key.IsCascadeDeleteEnabled = "cascade".Equals(subnode.Attributes["on-delete"].Value);
					BindSimpleValue(subnode, key, model.IsOneToMany, Mapping.Collection.DefaultKeyColumnName);
					if (key.Type.ReturnedClass.IsArray)
						throw new MappingException("illegal use of an array as an identifier (arrays don't reimplement Equals)");
					model.Key = key;


					XmlAttribute notNull = subnode.Attributes["not-null"];
					key.SetNullable(notNull == null || IsFalse(notNull.Value));
					XmlAttribute updateable = subnode.Attributes["update"];
					key.SetUpdateable(updateable == null || IsTrue(updateable.Value));
				}
				else if ("element".Equals(name))
				{
					var elt = new SimpleValue(model.CollectionTable);
					model.Element = elt;
					if(model.IsGeneric)
					{
						switch (model.GenericArguments.Length)
						{
							case 1:
								// a collection with a generic type parameter
								elt.TypeName = model.GenericArguments[0].AssemblyQualifiedName;
								break;
							case 2:
								// a map (IDictionary) with 2 parameters
								elt.TypeName = model.GenericArguments[1].AssemblyQualifiedName; 
								break;
						}
					}
					BindSimpleValue(subnode, elt, true, Mapping.Collection.DefaultElementColumnName);
				}
				else if ("many-to-many".Equals(name))
				{
					ManyToOne element = new ManyToOne(model.CollectionTable);
					model.Element = element;
					BindManyToOne(subnode, element, Mapping.Collection.DefaultElementColumnName, false);
					BindManyToManySubelements(model, subnode);
				}
				else if ("composite-element".Equals(name))
				{
					Component element = new Component(model);
					model.Element = element;
					BindComponent(subnode, element, null, null, null, model.Role+ ".element", true, inheritedMetas);
				}
				else if ("many-to-any".Equals(name))
				{
					Any element = new Any(model.CollectionTable);
					model.Element = element;
					BindAny(subnode, element, true);
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

		private void BindManyToManySubelements(Mapping.Collection collection, XmlNode manyToManyNode)
		{
			// Bind the where
			XmlAttribute where = manyToManyNode.Attributes["where"];
			string whereCondition = where == null ? null : where.Value;
			collection.ManyToManyWhere = whereCondition;

			// Bind the order-by
			XmlAttribute order = manyToManyNode.Attributes["order-by"];
			string orderFragment = order == null ? null : order.Value;
			collection.ManyToManyOrdering = orderFragment;

			// Bind the filters
			if ((manyToManyNode.SelectSingleNode(HbmConstants.nsFilter, namespaceManager) != null ||
				whereCondition != null) &&
					collection.FetchMode == FetchMode.Join &&
						collection.Element.FetchMode != FetchMode.Join)
				throw new MappingException(
					"many-to-many defining filter or where without join fetching " +
						"not valid within collection using join fetching [" + collection.Role + "]"
					);
			foreach (XmlNode filterElement in manyToManyNode.SelectNodes(HbmConstants.nsFilter, namespaceManager))
			{
				string name = XmlHelper.GetAttributeValue(filterElement, "name");
				string condition = filterElement.InnerText.Trim();
				if (StringHelper.IsEmpty(condition))
					condition = XmlHelper.GetAttributeValue(filterElement, "condition");
				if (StringHelper.IsEmpty(condition))
					condition = mappings.GetFilterDefinition(name).DefaultFilterCondition;
				if (condition == null)
					throw new MappingException("no filter condition found for filter: " + name);
				log.Debug(
					"Applying many-to-many filter [" + name +
						"] as [" + condition +
							"] to role [" + collection.Role + "]"
					);
				collection.AddManyToManyFilter(name, condition);
			}
		}

		private delegate Mapping.Collection CreateCollectionCommand(XmlNode node, string className,
			string path, PersistentClass owner, System.Type containingType, IDictionary<string, MetaAttribute> inheritedMetas);
	}
}
