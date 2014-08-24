using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Mapping;
using NHibernate.Type;
using NHibernate.Util;

using Array = NHibernate.Mapping.Array;
using NHibernate.Cfg.MappingSchema;

namespace NHibernate.Cfg.XmlHbmBinding
{
	public class CollectionBinder : ClassBinder
	{
		public CollectionBinder(Mappings mappings, Dialect.Dialect dialect) : base(mappings, dialect)
		{
		}

		public Mapping.Collection Create(ICollectionPropertiesMapping collectionMapping, string className,
			string propertyFullPath, PersistentClass owner, System.Type containingType, IDictionary<string, MetaAttribute> inheritedMetas)
		{
			var collectionType = collectionMapping.GetType();
			if (collectionType == typeof (HbmBag))
			{
				return CreateBag((HbmBag)collectionMapping, className, propertyFullPath, owner, containingType, inheritedMetas);
			}
			else if (collectionType == typeof (HbmSet))
			{
				return CreateSet((HbmSet)collectionMapping, className, propertyFullPath, owner, containingType, inheritedMetas);
			}
			else if (collectionType == typeof (HbmList))
			{
				return CreateList((HbmList)collectionMapping, className, propertyFullPath, owner, containingType, inheritedMetas);
			}
			else if (collectionType == typeof (HbmMap))
			{
				return CreateMap((HbmMap)collectionMapping, className, propertyFullPath, owner, containingType, inheritedMetas);
			}
			else if (collectionType == typeof (HbmIdbag))
			{
				return CreateIdentifierBag((HbmIdbag)collectionMapping, className, propertyFullPath, owner, containingType, inheritedMetas);
			}
			else if (collectionType == typeof (HbmArray))
			{
				return CreateArray((HbmArray)collectionMapping, className, propertyFullPath, owner, containingType, inheritedMetas);
			}
			else if (collectionType == typeof (HbmPrimitiveArray))
			{
				return CreatePrimitiveArray((HbmPrimitiveArray)collectionMapping, className, propertyFullPath, owner, containingType, inheritedMetas);
			}
			throw new MappingException("Not supported collection mapping element:" + collectionType);
		}

		private Mapping.Collection CreateMap(HbmMap mapMapping, string prefix, string path,
			PersistentClass owner, System.Type containingType, IDictionary<string, MetaAttribute> inheritedMetas)
		{
			var map = new Map(owner);
			BindCollection(mapMapping, map, prefix, path, containingType, inheritedMetas);
			AddMapSecondPass(mapMapping, map, inheritedMetas);
			return map;
		}

		private Mapping.Collection CreateSet(HbmSet setMapping, string prefix, string path,
			PersistentClass owner, System.Type containingType, IDictionary<string, MetaAttribute> inheritedMetas)
		{
			var setCollection = new Set(owner);
			BindCollection(setMapping, setCollection, prefix, path, containingType, inheritedMetas);
			AddSetSecondPass(setMapping, setCollection, inheritedMetas);
			return setCollection;
		}

		private Mapping.Collection CreateList(HbmList listMapping, string prefix, string path,
			PersistentClass owner, System.Type containingType, IDictionary<string, MetaAttribute> inheritedMetas)
		{
			var list = new List(owner);
			BindCollection(listMapping, list, prefix, path, containingType, inheritedMetas);
			AddListSecondPass(listMapping, list, inheritedMetas);
			return list;
		}

		private Mapping.Collection CreateBag(HbmBag bagMapping, string prefix, string path,
			PersistentClass owner, System.Type containingType, IDictionary<string, MetaAttribute> inheritedMetas)
		{
			var bag = new Bag(owner);
			BindCollection(bagMapping, bag, prefix, path, containingType, inheritedMetas);
			AddCollectionSecondPass(bagMapping, bag, inheritedMetas);
			return bag;
		}

		private Mapping.Collection CreateIdentifierBag(HbmIdbag idbagMapping, string prefix, string path,
			PersistentClass owner, System.Type containingType, IDictionary<string, MetaAttribute> inheritedMetas)
		{
			var idbag = new IdentifierBag(owner);
			BindCollection(idbagMapping, idbag, prefix, path, containingType, inheritedMetas);
			AddIdentifierCollectionSecondPass(idbagMapping, idbag, inheritedMetas);
			return idbag;
		}

		private Mapping.Collection CreateArray(HbmArray arrayMapping, string prefix, string path,
			PersistentClass owner, System.Type containingType, IDictionary<string, MetaAttribute> inheritedMetas)
		{
			var array = new Array(owner);
			BindArray(arrayMapping, array, prefix, path, containingType, inheritedMetas);
			AddArraySecondPass(arrayMapping, array, inheritedMetas);
			return array;
		}

		private Mapping.Collection CreatePrimitiveArray(HbmPrimitiveArray primitiveArrayMapping, string prefix, string path,
			PersistentClass owner, System.Type containingType, IDictionary<string, MetaAttribute> inheritedMetas)
		{
			var array = new PrimitiveArray(owner);
			BindPrimitiveArray(primitiveArrayMapping, array, prefix, path, containingType,inheritedMetas);
			AddPrimitiveArraySecondPass(primitiveArrayMapping, array, inheritedMetas);
			return array;
		}

		private void BindPrimitiveArray(HbmPrimitiveArray arrayMapping, PrimitiveArray model, string prefix, string path, System.Type containingType, IDictionary<string, MetaAttribute> inheritedMetas)
		{
			BindCollection(arrayMapping, model, prefix, path, containingType, inheritedMetas);

			var element = arrayMapping.ElementRelationship as HbmElement;
			if (element != null)
			{
				string typeName;
				var typeAttribute = element.Type;
				if (typeAttribute != null)
					typeName = typeAttribute.name;
				else
					throw new MappingException("type for <element> was not defined");
				IType type = TypeFactory.HeuristicType(typeName, null);
				if (type == null)
					throw new MappingException("could not interpret type: " + typeName);

				model.ElementClassName = type.ReturnedClass.AssemblyQualifiedName;
			}
		}

		/// <remarks>
		/// Called for all collections. <paramref name="containingType" /> parameter
		/// was added in NH to allow for reflection related to generic types.
		/// </remarks>
		private void BindCollection(ICollectionPropertiesMapping collectionMapping, Mapping.Collection model, string className,
			string path, System.Type containingType, IDictionary<string, MetaAttribute> inheritedMetas)
		{
			// ROLENAME
			model.Role = path;

			model.IsInverse = collectionMapping.Inverse;
			model.IsMutable = collectionMapping.Mutable;
			model.IsOptimisticLocked = collectionMapping.OptimisticLock;
			model.OrderBy = collectionMapping.OrderBy;
			model.Where = collectionMapping.Where;
			if (collectionMapping.BatchSize.HasValue)
				model.BatchSize = collectionMapping.BatchSize.Value;

			// PERSISTER
			if (!string.IsNullOrEmpty(collectionMapping.PersisterQualifiedName))
			{
				model.CollectionPersisterClass = ClassForNameChecked(collectionMapping.PersisterQualifiedName, mappings,
																	 "could not instantiate collection persister class: {0}");
			}

			if(!string.IsNullOrEmpty(collectionMapping.CollectionType))
			{
				TypeDef typeDef = mappings.GetTypeDef(collectionMapping.CollectionType);
				if (typeDef != null)
				{
					model.TypeName = typeDef.TypeClass;
					model.TypeParameters = typeDef.Parameters;
				}
				else
				{
					model.TypeName = FullQualifiedClassName(collectionMapping.CollectionType, mappings);
				}
			}

			// FETCH STRATEGY
			InitOuterJoinFetchSetting(collectionMapping, model);

			// LAZINESS
			InitLaziness(collectionMapping, model);

			var oneToManyMapping = collectionMapping.ElementRelationship as HbmOneToMany;
			if (oneToManyMapping != null)
			{
				var oneToMany = new OneToMany(model.Owner);
				model.Element = oneToMany;
				BindOneToMany(oneToManyMapping, oneToMany);
				//we have to set up the table later!! yuck
			}
			else
			{
				//TABLE
				string tableName = !string.IsNullOrEmpty(collectionMapping.Table)
									? mappings.NamingStrategy.TableName(collectionMapping.Table)
									: mappings.NamingStrategy.PropertyToTableName(className, path);

				string schema = string.IsNullOrEmpty(collectionMapping.Schema) ? mappings.SchemaName : collectionMapping.Schema;
				string catalog = string.IsNullOrEmpty(collectionMapping.Catalog) ? mappings.CatalogName : collectionMapping.Catalog;

				// TODO NH : add schema-action to the xsd
				model.CollectionTable = mappings.AddTable(schema, catalog, tableName, collectionMapping.Subselect, false, "all");

				log.InfoFormat("Mapping collection: {0} -> {1}", model.Role, model.CollectionTable.Name);
			}

			//SORT
			var sortedAtt = collectionMapping.Sort;
			// unsorted, natural, comparator.class.name
			if (string.IsNullOrEmpty(sortedAtt) || sortedAtt.Equals("unsorted"))
				model.IsSorted = false;
			else
			{
				model.IsSorted = true;
				if (!sortedAtt.Equals("natural"))
				{
					string comparatorClassName = FullQualifiedClassName(sortedAtt, mappings);
					model.ComparerClassName = comparatorClassName;
				}
			}

			//ORPHAN DELETE (used for programmer error detection)
			var cascadeAtt = collectionMapping.Cascade;
			if (!string.IsNullOrEmpty(cascadeAtt) && cascadeAtt.IndexOf("delete-orphan") >= 0)
				model.HasOrphanDelete = true;

			// GENERIC
			bool? isGeneric = collectionMapping.Generic;
			System.Type collectionType = null;
			if (!isGeneric.HasValue && containingType != null)
			{
				collectionType = GetPropertyType(containingType, collectionMapping.Name, collectionMapping.Access);
				isGeneric = collectionType.IsGenericType;
			}

			model.IsGeneric = isGeneric.GetValueOrDefault();

			if (model.IsGeneric)
			{
				// Determine the generic arguments using reflection
				if (collectionType == null)
					collectionType = GetPropertyType(containingType, collectionMapping.Name, collectionMapping.Access);
				System.Type[] genericArguments = collectionType.GetGenericArguments();
				model.GenericArguments = genericArguments;
			}

			// CUSTOM SQL
			HandleCustomSQL(collectionMapping, model);
			if (collectionMapping.SqlLoader != null)
				model.LoaderName = collectionMapping.SqlLoader.queryref;

			new FiltersBinder(model, Mappings).Bind(collectionMapping.Filters);

			var key = collectionMapping.Key;
			if (key != null)
				model.ReferencedPropertyName = key.propertyref;
		}

		private void InitLaziness(ICollectionPropertiesMapping collectionMapping, Mapping.Collection fetchable)
		{
			var lazyMapping = collectionMapping.Lazy;
			if(!lazyMapping.HasValue)
			{
				fetchable.IsLazy = mappings.DefaultLazy;
				fetchable.ExtraLazy = false;
				return;
			}

			switch (lazyMapping.Value)
			{
				case HbmCollectionLazy.True:
					fetchable.IsLazy = true;
					break;
				case HbmCollectionLazy.False:
					fetchable.IsLazy = false;
					fetchable.ExtraLazy = false;
					break;
				case HbmCollectionLazy.Extra:
					fetchable.IsLazy = true;
					fetchable.ExtraLazy = true;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void InitOuterJoinFetchSetting(ICollectionPropertiesMapping collectionMapping, Mapping.Collection model)
		{
			FetchMode fetchStyle = FetchMode.Default;

			if (!collectionMapping.FetchMode.HasValue)
			{
				if (collectionMapping.OuterJoin.HasValue)
				{
					// use old (HB 2.1) defaults if outer-join is specified
					switch (collectionMapping.OuterJoin.Value)
					{
						case HbmOuterJoinStrategy.Auto:
							fetchStyle = FetchMode.Default;
							break;
						case HbmOuterJoinStrategy.True:
							fetchStyle = FetchMode.Join;
							break;
						case HbmOuterJoinStrategy.False:
							fetchStyle = FetchMode.Select;
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
				}
			}
			else
			{
				switch (collectionMapping.FetchMode.Value)
				{
					case HbmCollectionFetchMode.Select:
						fetchStyle = FetchMode.Select;
						break;
					case HbmCollectionFetchMode.Join:
						fetchStyle = FetchMode.Join;
						break;
					case HbmCollectionFetchMode.Subselect:
						fetchStyle = FetchMode.Select;
						model.IsSubselectLoadable = true;
						model.Owner.HasSubselectLoadableCollections = true;
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}

			model.FetchMode = fetchStyle;
		}

		/// <remarks>
		/// Called for arrays and primitive arrays
		/// </remarks>
		private void BindArray(HbmArray arrayMapping, Array model, string prefix, string path,
			System.Type containingType, IDictionary<string, MetaAttribute> inheritedMetas)
		{
			BindCollection(arrayMapping, model, prefix, path, containingType, inheritedMetas);

			var att = arrayMapping.elementclass;

			if (att != null)
				model.ElementClassName = GetQualifiedClassName(att, mappings);
			else
			{
				HbmElement element;
				HbmOneToMany oneToMany;
				HbmManyToMany manyToMany;
				HbmCompositeElement compositeElement;
				if((element = arrayMapping.ElementRelationship as HbmElement) != null)
				{
					string typeName;
					var typeAttribute = element.Type;
					if (typeAttribute != null)
						typeName = typeAttribute.name;
					else
						throw new MappingException("type for <element> was not defined");
					IType type = TypeFactory.HeuristicType(typeName, null);
					if (type == null)
						throw new MappingException("could not interpret type: " + typeName);

					model.ElementClassName = type.ReturnedClass.AssemblyQualifiedName;
				}
				else if((oneToMany = arrayMapping.ElementRelationship as HbmOneToMany) != null)
				{
					model.ElementClassName = GetQualifiedClassName(oneToMany.@class, mappings);
				}
				else if ((manyToMany = arrayMapping.ElementRelationship as HbmManyToMany) != null)
				{
					model.ElementClassName = GetQualifiedClassName(manyToMany.@class, mappings);
				}
				else if ((compositeElement = arrayMapping.ElementRelationship as HbmCompositeElement) != null)
				{
					model.ElementClassName = GetQualifiedClassName(compositeElement.@class, mappings);
				}
			}
		}

		private void AddListSecondPass(HbmList listMapping, List model, IDictionary<string, MetaAttribute> inheritedMetas)
		{
			mappings.AddSecondPass(delegate(IDictionary<string, PersistentClass> persistentClasses)
				{
					PreCollectionSecondPass(model);
					BindListSecondPass(listMapping, model, persistentClasses, inheritedMetas);
					PostCollectionSecondPass(model);
				});
		}

		private void AddArraySecondPass(HbmArray arrayMapping, Array model, IDictionary<string, MetaAttribute> inheritedMetas)
		{
			mappings.AddSecondPass(delegate(IDictionary<string, PersistentClass> persistentClasses)
			{
				PreCollectionSecondPass(model);
				BindArraySecondPass(arrayMapping, model, persistentClasses, inheritedMetas);
				PostCollectionSecondPass(model);
			});
		}

		private void AddPrimitiveArraySecondPass(HbmPrimitiveArray primitiveArrayMapping, Array model, IDictionary<string, MetaAttribute> inheritedMetas)
		{
			mappings.AddSecondPass(delegate(IDictionary<string, PersistentClass> persistentClasses)
			{
				PreCollectionSecondPass(model);
				BindPrimitiveArraySecondPass(primitiveArrayMapping, model, persistentClasses, inheritedMetas);
				PostCollectionSecondPass(model);
			});
		}

		private void AddMapSecondPass(HbmMap mapMapping, Map model, IDictionary<string, MetaAttribute> inheritedMetas)
		{
			mappings.AddSecondPass(delegate(IDictionary<string, PersistentClass> persistentClasses)
				{
					PreCollectionSecondPass(model);
					BindMapSecondPass(mapMapping, model, persistentClasses, inheritedMetas);
					PostCollectionSecondPass(model);
				});
		}

		private void AddSetSecondPass(HbmSet setMapping, Set model, IDictionary<string, MetaAttribute> inheritedMetas)
		{
			mappings.AddSecondPass(delegate(IDictionary<string, PersistentClass> persistentClasses)
				{
					PreCollectionSecondPass(model);
					BindSetSecondPass(setMapping, model, persistentClasses, inheritedMetas);
					PostCollectionSecondPass(model);
				});
		}

		private void AddIdentifierCollectionSecondPass(HbmIdbag idbagMapping, IdentifierCollection model, IDictionary<string, MetaAttribute> inheritedMetas)
		{
			mappings.AddSecondPass(delegate(IDictionary<string, PersistentClass> persistentClasses)
				{
					PreCollectionSecondPass(model);
					BindIdentifierCollectionSecondPass(idbagMapping, model, persistentClasses, inheritedMetas);
					PostCollectionSecondPass(model);
				});
		}

		private void AddCollectionSecondPass(ICollectionPropertiesMapping collectionMapping, Mapping.Collection model, IDictionary<string, MetaAttribute> inheritedMetas)
		{
			mappings.AddSecondPass(delegate(IDictionary<string, PersistentClass> persistentClasses)
				{
					PreCollectionSecondPass(model);
					BindCollectionSecondPass(collectionMapping, model, persistentClasses, inheritedMetas);
					PostCollectionSecondPass(model);
				});
		}

		private void HandleCustomSQL(ICollectionSqlsMapping collection, Mapping.Collection model)
		{
			var sqlInsert = collection.SqlInsert;
			if (sqlInsert != null)
			{
				bool callable = sqlInsert.callableSpecified && sqlInsert.callable;
				model.SetCustomSQLInsert(sqlInsert.Text.LinesToString(), callable, GetResultCheckStyle(sqlInsert));
			}

			var sqlDelete = collection.SqlDelete;
			if (sqlDelete != null)
			{
				bool callable = sqlDelete.callableSpecified && sqlDelete.callable;
				model.SetCustomSQLDelete(sqlDelete.Text.LinesToString(), callable, GetResultCheckStyle(sqlDelete));
			}

			var sqlUpdate = collection.SqlUpdate;
			if (sqlUpdate != null)
			{
				bool callable = sqlUpdate.callableSpecified && sqlUpdate.callable;
				model.SetCustomSQLUpdate(sqlUpdate.Text.LinesToString(), callable, GetResultCheckStyle(sqlUpdate));
			}

			var sqlDeleteAll = collection.SqlDeleteAll;
			if (sqlDeleteAll != null)
			{
				bool callable = sqlDeleteAll.callableSpecified && sqlDeleteAll.callable;
				model.SetCustomSQLDeleteAll(sqlDeleteAll.Text.LinesToString(), callable, GetResultCheckStyle(sqlDeleteAll));
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
				string msg = "Mapped collection key: " + string.Join(",", collection.Key.ColumnIterator.Select(c => c.Text).ToArray());
				if (collection.IsIndexed)
					msg += ", index: " + string.Join(",", ((IndexedCollection)collection).Index.ColumnIterator.Select(c => c.Text).ToArray());
				if (collection.IsOneToMany)
					msg += ", one-to-many: " + collection.Element.Type.Name;
				else
				{
					msg += ", element: " + string.Join(",", collection.Element.ColumnIterator.Select(c => c.Text).ToArray());
					msg += ", type: " + collection.Element.Type.Name;
				}
				log.Debug(msg);
			}
		}

		private void BindListSecondPass(HbmList listMapping, List model, IDictionary<string, PersistentClass> persistentClasses, IDictionary<string, MetaAttribute> inheritedMetas)
		{
			BindCollectionSecondPass(listMapping, model, persistentClasses, inheritedMetas);

			// Index
			BindCollectionIndex(listMapping, model);
			if (listMapping.ListIndex != null && !string.IsNullOrEmpty(listMapping.ListIndex.@base))
			{
				model.BaseIndex = Convert.ToInt32(listMapping.ListIndex.@base);
			}

			if (NeedBackref(model))
			{
				string entityName = ((OneToMany)model.Element).ReferencedEntityName;
				PersistentClass referenced = mappings.GetClass(entityName);
				var ib = new IndexBackref();
				ib.Name = '_' + model.OwnerEntityName + "." + listMapping.Name + "IndexBackref";
				ib.IsUpdateable = false;
				ib.IsSelectable = false;
				ib.CollectionRole = model.Role;
				ib.EntityName = model.Owner.EntityName;
				ib.Value = model.Index;
				referenced.AddProperty(ib);
			}
		}

		private void BindArraySecondPass(HbmArray arrayMapping, List model,
			IDictionary<string, PersistentClass> persistentClasses, IDictionary<string, MetaAttribute> inheritedMetas)
		{
			BindCollectionSecondPass(arrayMapping, model, persistentClasses, inheritedMetas);

			// Index
			BindCollectionIndex(arrayMapping, model);
			if (arrayMapping.ListIndex != null && !string.IsNullOrEmpty(arrayMapping.ListIndex.@base))
			{
				model.BaseIndex = Convert.ToInt32(arrayMapping.ListIndex.@base);
			}
		}

		private void BindPrimitiveArraySecondPass(HbmPrimitiveArray primitiveArrayMapping, List model,
			IDictionary<string, PersistentClass> persistentClasses, IDictionary<string, MetaAttribute> inheritedMetas)
		{
			BindCollectionSecondPass(primitiveArrayMapping, model, persistentClasses, inheritedMetas);

			// Index
			BindCollectionIndex(primitiveArrayMapping, model);
			if (primitiveArrayMapping.ListIndex != null && !string.IsNullOrEmpty(primitiveArrayMapping.ListIndex.@base))
			{
				model.BaseIndex = Convert.ToInt32(primitiveArrayMapping.ListIndex.@base);
			}
		}

		private void BindCollectionIndex(IIndexedCollectionMapping listMapping, IndexedCollection model)
		{
			SimpleValue iv = null;
			if (listMapping.ListIndex != null)
			{
				iv = new SimpleValue(model.CollectionTable);
				new ValuePropertyBinder(iv, Mappings).BindSimpleValue(listMapping.ListIndex,
																	  IndexedCollection.DefaultIndexColumnName, model.IsOneToMany);
			}
			else if (listMapping.Index != null)
			{
				iv = new SimpleValue(model.CollectionTable);
				listMapping.Index.type = NHibernateUtil.Int32.Name;
				new ValuePropertyBinder(iv, Mappings).BindSimpleValue(listMapping.Index, IndexedCollection.DefaultIndexColumnName,
																	  model.IsOneToMany);
			}
			if (iv != null)
			{
				if (iv.ColumnSpan > 1)
					log.Error("This shouldn't happen, check BindIntegerValue");
				model.Index = iv;
			}
		}

		private void BindOneToMany(HbmOneToMany oneToManyMapping, OneToMany model)
		{
			model.ReferencedEntityName = GetEntityName(oneToManyMapping, mappings);

			model.IsIgnoreNotFound = oneToManyMapping.NotFoundMode == HbmNotFoundMode.Ignore;
		}

		private void BindIdentifierCollectionSecondPass(HbmIdbag idbagMapping, IdentifierCollection model,
			IDictionary<string, PersistentClass> persitentClasses, IDictionary<string, MetaAttribute> inheritedMetas)
		{
			BindCollectionSecondPass(idbagMapping, model, persitentClasses, inheritedMetas);

			var id = new SimpleValue(model.CollectionTable);
			new ValuePropertyBinder(id, Mappings).BindSimpleValue(idbagMapping.collectionid, IdentifierCollection.DefaultIdentifierColumnName);
			model.Identifier = id;
			new IdGeneratorBinder(Mappings).BindGenerator(id, idbagMapping.collectionid.generator);
			id.Table.SetIdentifierValue(id);
		}

		private void BindSetSecondPass(HbmSet setMapping, Set model,
			IDictionary<string, PersistentClass> persistentClasses, IDictionary<string, MetaAttribute> inheritedMetas)
		{
			BindCollectionSecondPass(setMapping, model, persistentClasses, inheritedMetas);

			if (!model.IsOneToMany)
				model.CreatePrimaryKey();
		}

		/// <summary>
		/// Called for Maps
		/// </summary>
		private void BindMapSecondPass(HbmMap mapMapping, Map model,
			IDictionary<string, PersistentClass> persistentClasses, IDictionary<string, MetaAttribute> inheritedMetas)
		{
			BindCollectionSecondPass(mapMapping, model, persistentClasses, inheritedMetas);

			HbmIndex indexMapping;
			HbmMapKey mapKeyMapping;

			HbmIndexManyToMany indexManyToManyMapping;
			HbmMapKeyManyToMany mapKeyManyToManyMapping;

			HbmCompositeIndex compositeIndexMapping;
			HbmCompositeMapKey compositeMapKeyMapping;

			HbmIndexManyToAny indexManyToAnyMapping;

			if ((indexMapping = mapMapping.Item as HbmIndex) != null)
			{
				var value = new SimpleValue(model.CollectionTable);
				new ValuePropertyBinder(value, Mappings).BindSimpleValue(indexMapping, IndexedCollection.DefaultIndexColumnName,
																		 model.IsOneToMany);
				model.Index = value;
				if (string.IsNullOrEmpty(model.Index.TypeName))
					throw new MappingException("map index element must specify a type: " + model.Role);
			}
			else if ((mapKeyMapping = mapMapping.Item as HbmMapKey) != null)
			{
				var value = new SimpleValue(model.CollectionTable);
				new ValuePropertyBinder(value, Mappings).BindSimpleValue(mapKeyMapping, IndexedCollection.DefaultIndexColumnName,
																																 model.IsOneToMany);
				model.Index = value;
				if (string.IsNullOrEmpty(model.Index.TypeName))
					throw new MappingException("map index element must specify a type: " + model.Role);
			}
			else if ((indexManyToManyMapping = mapMapping.Item as HbmIndexManyToMany) != null)
			{
				var manyToOne = new ManyToOne(model.CollectionTable);
				BindIndexManyToMany(indexManyToManyMapping, manyToOne, IndexedCollection.DefaultIndexColumnName, model.IsOneToMany);
				model.Index = manyToOne;
			}
			else if ((mapKeyManyToManyMapping = mapMapping.Item as HbmMapKeyManyToMany) != null)
			{
				var manyToOne = new ManyToOne(model.CollectionTable);
				BindMapKeyManyToMany(mapKeyManyToManyMapping, manyToOne, IndexedCollection.DefaultIndexColumnName, model.IsOneToMany);
				model.Index = manyToOne;
			}
			else if ((compositeIndexMapping = mapMapping.Item as HbmCompositeIndex) != null)
			{
				var component = new Component(model);
				BindComponent(compositeIndexMapping, component, null, null, model.Role + ".index", model.IsOneToMany, inheritedMetas);
				model.Index = component;
			}
			else if ((compositeMapKeyMapping = mapMapping.Item as HbmCompositeMapKey) != null)
			{
				var component = new Component(model);
				BindComponent(compositeMapKeyMapping, component, null, null, model.Role + ".index", model.IsOneToMany, inheritedMetas);
				model.Index = component;
			}
			else if ((indexManyToAnyMapping = mapMapping.Item as HbmIndexManyToAny) != null)
			{
				var any = new Any(model.CollectionTable);
				BindIndexManyToAny(indexManyToAnyMapping, any, model.IsOneToMany);
				model.Index = any;				
			}

			bool indexIsFormula = model.Index.ColumnIterator.Any(x=> x.IsFormula);
			if (NeedBackref(model) && !indexIsFormula)
			{
				string entityName = ((OneToMany)model.Element).ReferencedEntityName;
				PersistentClass referenced = mappings.GetClass(entityName);
				var ib = new IndexBackref();
				ib.Name = '_' + model.OwnerEntityName + "." + mapMapping.Name + "IndexBackref";
				ib.IsUpdateable = false;
				ib.IsSelectable = false;
				ib.CollectionRole = model.Role;
				ib.EntityName = model.Owner.EntityName;
				ib.Value = model.Index;
				referenced.AddProperty(ib);
			}
		}

		private void BindIndexManyToAny(HbmIndexManyToAny indexManyToAnyMapping, Any any, bool isNullable)
		{
			any.IdentifierTypeName = indexManyToAnyMapping.idtype;
			new TypeBinder(any, Mappings).Bind(indexManyToAnyMapping.idtype);
			BindAnyMeta(indexManyToAnyMapping, any);
			new ColumnsBinder(any, Mappings).Bind(indexManyToAnyMapping.Columns, isNullable,
																						() =>
																						new HbmColumn
																						{
																							name = mappings.NamingStrategy.PropertyToColumnName(indexManyToAnyMapping.column1)
																						});
		}

		private void BindMapKeyManyToMany(HbmMapKeyManyToMany mapKeyManyToManyMapping, ManyToOne model, string defaultColumnName, bool isNullable)
		{
			new ValuePropertyBinder(model, Mappings).BindSimpleValue(mapKeyManyToManyMapping, defaultColumnName, isNullable);

			model.ReferencedEntityName = GetEntityName(mapKeyManyToManyMapping, mappings);

			BindForeignKey(mapKeyManyToManyMapping.foreignkey, model);
		}

		private void BindIndexManyToMany(HbmIndexManyToMany indexManyToManyMapping, ManyToOne model, string defaultColumnName, bool isNullable)
		{
			new ValuePropertyBinder(model, Mappings).BindSimpleValue(indexManyToManyMapping, defaultColumnName, isNullable);

			model.ReferencedEntityName = GetEntityName(indexManyToManyMapping, mappings);

			BindForeignKey(indexManyToManyMapping.foreignkey, model);
		}

		/// <remarks>
		/// Called for all collections
		/// </remarks>
		private void BindCollectionSecondPass(ICollectionPropertiesMapping collectionMapping, Mapping.Collection model,
			IDictionary<string, PersistentClass> persistentClasses, IDictionary<string, MetaAttribute> inheritedMetas)
		{
			if (model.IsOneToMany)
			{
				var oneToMany = (OneToMany)model.Element;
				string associatedEntityName = oneToMany.ReferencedEntityName;
				PersistentClass persistentClass;
				if (persistentClasses.TryGetValue(associatedEntityName, out persistentClass) == false)
					throw new MappingException("Association references unmapped class: " + associatedEntityName);
				oneToMany.AssociatedClass = persistentClass;
				model.CollectionTable = persistentClass.Table;
				if (model.IsInverse && persistentClass.JoinClosureSpan > 0)
				{
					// NH: bidirectional one-to-many with a class splitted in more tables; have to find in which table is the inverse side
					foreach (var joined in persistentClass.JoinClosureIterator)
					{
						if (collectionMapping.Key.Columns.Select(x=> x.name).All(x => joined.Table.ColumnIterator.Select(jc=> jc.Name).Contains(x)))
						{
							model.CollectionTable = joined.Table;
							break;
						}
					}
				}

				if (log.IsInfoEnabled)
					log.Info("mapping collection: " + model.Role + " -> " + model.CollectionTable.Name);
			}

			//CHECK
			if (!string.IsNullOrEmpty(collectionMapping.Check))
			{
				model.CollectionTable.AddCheckConstraint(collectionMapping.Check);
			}
			BindKey(collectionMapping.Key, model);

			//contained elements:
			HbmCompositeElement compositeElementMapping;
			HbmElement elementMapping;
			HbmManyToAny manyToAnyMapping;
			HbmManyToMany manyToManyMapping;

			if ((elementMapping = collectionMapping.ElementRelationship as HbmElement) != null)
			{
				BindElement(elementMapping, model);
			}
			else if ((manyToManyMapping = collectionMapping.ElementRelationship as HbmManyToMany) != null)
			{
				BindManyToMany(manyToManyMapping, model);
			}
			else if ((compositeElementMapping = collectionMapping.ElementRelationship as HbmCompositeElement) != null)
			{
				BindCompositeElement(compositeElementMapping, model, inheritedMetas);
			}
			else if ((manyToAnyMapping = collectionMapping.ElementRelationship as HbmManyToAny) != null)
			{
				BindManyToAny(manyToAnyMapping, model);
			}
			
			BindCache(collectionMapping.Cache, model);

			if (NeedBackref(model))
			{
				// for non-inverse one-to-many, with a not-null fk, add a backref!
				string entityName = ((OneToMany)model.Element).ReferencedEntityName;
				PersistentClass referenced = mappings.GetClass(entityName);
				var prop = new Backref();
				prop.Name = '_' + model.OwnerEntityName + "." + collectionMapping.Name + "Backref";
				prop.IsUpdateable = false;
				prop.IsSelectable = false;
				prop.CollectionRole = model.Role;
				prop.EntityName = model.Owner.EntityName;
				prop.Value = model.Key;
				referenced.AddProperty(prop);
			}
		}

		private void BindManyToMany(HbmManyToMany manyToManyMapping, Mapping.Collection model)
		{
			var manyToMany = new ManyToOne(model.CollectionTable);
			model.Element = manyToMany;
			new ValuePropertyBinder(manyToMany, Mappings).BindSimpleValue(manyToManyMapping,
																	   Mapping.Collection.DefaultElementColumnName, false);
			InitOuterJoinFetchSetting(manyToManyMapping, manyToMany);
			var restrictedLaziness = manyToManyMapping.lazySpecified ? manyToManyMapping.lazy : (HbmRestrictedLaziness?) null;
			InitLaziness(restrictedLaziness, manyToMany, true);

			if(!string.IsNullOrEmpty(manyToManyMapping.propertyref))
			{
				manyToMany.ReferencedPropertyName = manyToManyMapping.propertyref;
			}

			manyToMany.ReferencedEntityName = GetEntityName(manyToManyMapping, mappings);

			manyToMany.IsIgnoreNotFound = manyToManyMapping.NotFoundMode == HbmNotFoundMode.Ignore;

			if(!string.IsNullOrEmpty(manyToManyMapping.foreignkey))
			{
				manyToMany.ForeignKeyName = manyToManyMapping.foreignkey;
			}
			BindManyToManySubelements(manyToManyMapping, model);
		}

		private void BindCompositeElement(HbmCompositeElement compositeElementMapping, Mapping.Collection model, IDictionary<string, MetaAttribute> inheritedMetas)
		{
			var component = new Component(model);
			model.Element = component;
			BindComponent(compositeElementMapping, component, null, null, model.Role + ".element", true, inheritedMetas);
		}

		private void BindManyToAny(HbmManyToAny manyToAnyMapping, Mapping.Collection model)
		{
			var any = new Any(model.CollectionTable);
			model.Element = any;
			any.IdentifierTypeName = manyToAnyMapping.idtype;
			new TypeBinder(any, Mappings).Bind(manyToAnyMapping.idtype);
			BindAnyMeta(manyToAnyMapping, any);
			new ColumnsBinder(any, Mappings).Bind(manyToAnyMapping.Columns, true,
												  () =>
												  new HbmColumn
													{
														name = mappings.NamingStrategy.PropertyToColumnName(manyToAnyMapping.column1)
													});
		}

		private void BindElement(HbmElement elementMapping, Mapping.Collection model)
		{
			var element = new SimpleValue(model.CollectionTable);
			model.Element = element;
			if (model.IsGeneric)
			{
				switch (model.GenericArguments.Length)
				{
					case 1:
						// a collection with a generic type parameter
						element.TypeName = model.GenericArguments[0].AssemblyQualifiedName;
						break;
					case 2:
						// a map (IDictionary) with 2 parameters
						element.TypeName = model.GenericArguments[1].AssemblyQualifiedName;
						break;
				}
			}
			new ValuePropertyBinder(element, Mappings).BindSimpleValue(elementMapping, Mapping.Collection.DefaultKeyColumnName, true);
		}

		private static void BindCache(HbmCache cacheSchema, Mapping.Collection collection)
		{
			if (cacheSchema != null)
			{
				collection.CacheConcurrencyStrategy = cacheSchema.usage.ToCacheConcurrencyStrategy();
				collection.CacheRegionName = cacheSchema.region;
			}
		}

		private void BindKey(HbmKey keyMapping, Mapping.Collection model)
		{
			if (keyMapping == null)
			{
				return;
			}
			string propRef = model.ReferencedPropertyName;
			IKeyValue keyValue;
			if (propRef == null)
			{
				keyValue = model.Owner.Identifier;
			}
			else
			{
				keyValue = (IKeyValue) model.Owner.GetProperty(propRef).Value;
			}
			var key = new DependantValue(model.CollectionTable, keyValue)
						{IsCascadeDeleteEnabled = keyMapping.ondelete == HbmOndelete.Cascade};

			new ValuePropertyBinder(key, Mappings).BindSimpleValue(keyMapping, Mapping.Collection.DefaultKeyColumnName, model.IsOneToMany);

			if (key.Type.ReturnedClass.IsArray)
				throw new MappingException("illegal use of an array as an identifier (arrays don't reimplement Equals)");
			model.Key = key;

			key.SetNullable(!keyMapping.IsNullable.HasValue || keyMapping.IsNullable.Value);
			key.SetUpdateable(!keyMapping.IsUpdatable.HasValue || keyMapping.IsUpdatable.Value);
			BindForeignKey(keyMapping.foreignkey, key);
		}

		private void BindManyToManySubelements(HbmManyToMany manyToManyMapping, Mapping.Collection collection)
		{
			// Bind the where
			string whereCondition = string.IsNullOrEmpty(manyToManyMapping.where) ? null : manyToManyMapping.where;
			collection.ManyToManyWhere = whereCondition;

			// Bind the order-by
			string orderFragment = string.IsNullOrEmpty(manyToManyMapping.orderby) ? null : manyToManyMapping.orderby;
			collection.ManyToManyOrdering = orderFragment;

			// Bind the filters
			var filters = manyToManyMapping.filter;
			var hasFilters = filters != null && filters.Length > 0;

			if ((hasFilters || whereCondition != null) && collection.FetchMode == FetchMode.Join
				&& collection.Element.FetchMode != FetchMode.Join)
			{
				throw new MappingException(
					string.Format(
						"many-to-many defining filter or where without join fetching not valid within collection using join fetching [{0}]",
						collection.Role));

			}

			new FiltersBinder(collection, Mappings).Bind(filters, collection.AddManyToManyFilter);
		}

		private System.Type GetPropertyType(System.Type containingType, string propertyName, string propertyAccess)
		{
			string access = propertyAccess ?? Mappings.DefaultAccess;

			return containingType == null ? null : ReflectHelper.ReflectedPropertyClass(containingType, propertyName, access);
		}

		private static bool NeedBackref(Mapping.Collection model)
		{
			return model.IsOneToMany && !model.Key.IsNullable && !model.IsInverse;
		}
	}
}
