using System;
using System.Collections.Generic;
using System.Linq;
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

		protected ClassBinder(Mappings mappings, Dialect.Dialect dialect)
			: base(mappings)
		{
			this.dialect = dialect;
		}

		protected ClassBinder(ClassBinder parent)
			: base(parent.Mappings)
		{
			dialect = parent.dialect;
		}

		protected void BindClass(IEntityMapping classMapping, PersistentClass model, IDictionary<string, MetaAttribute> inheritedMetas)
		{
			// handle the lazy attribute
			// go ahead and set the lazy here, since pojo.proxy can override it.
			model.IsLazy = classMapping.UseLazy.HasValue ? classMapping.UseLazy.Value : mappings.DefaultLazy;

			// transfer an explicitly defined entity name
			string entityName = classMapping.EntityName ??
								ClassForNameChecked(classMapping.Name, mappings, "persistent class {0} not found").FullName;
			if (entityName == null)
				throw new MappingException("Unable to determine entity name");
			model.EntityName = entityName;

			BindPocoRepresentation(classMapping, model);
			BindXmlRepresentation(classMapping, model);
			BindMapRepresentation(classMapping, model);

			BindPersistentClassCommonValues(classMapping, model, inheritedMetas);
		}

		protected void BindUnionSubclasses(IEnumerable<HbmUnionSubclass> unionSubclasses, PersistentClass persistentClass, IDictionary<string, MetaAttribute> inheritedMetas)
		{
			foreach (var unionSubclass in unionSubclasses)
			{
				new UnionSubclassBinder(this).HandleUnionSubclass(persistentClass, unionSubclass, inheritedMetas);
			}
		}

		protected void BindJoinedSubclasses(IEnumerable<HbmJoinedSubclass> joinedSubclasses, PersistentClass persistentClass, IDictionary<string, MetaAttribute> inheritedMetas)
		{
			foreach (var joinedSubclass in joinedSubclasses)
			{
				new JoinedSubclassBinder(this).HandleJoinedSubclass(persistentClass, joinedSubclass, inheritedMetas);
			}
		}

		protected void BindSubclasses(IEnumerable<HbmSubclass> subclasses, PersistentClass persistentClass, IDictionary<string, MetaAttribute> inheritedMetas)
		{
			foreach (var subclass in subclasses)
			{
				new SubclassBinder(this).HandleSubclass(persistentClass, subclass, inheritedMetas);
			}
		}

		private void BindPersistentClassCommonValues(IEntityMapping classMapping, PersistentClass model, IDictionary<string, MetaAttribute> inheritedMetas)
		{
			// DISCRIMINATOR
			var discriminable = classMapping as IEntityDiscriminableMapping;
			model.DiscriminatorValue = (discriminable == null || string.IsNullOrEmpty(discriminable.DiscriminatorValue)) ? model.EntityName : discriminable.DiscriminatorValue;

			// DYNAMIC UPDATE
			model.DynamicUpdate = classMapping.DynamicUpdate;

			// DYNAMIC INSERT
			model.DynamicInsert = classMapping.DynamicInsert;

			// IMPORT
			// For entities, the EntityName is the key to find a persister
			// NH Different behavior: we are using the association between EntityName and its more certain implementation (AssemblyQualifiedName)
			// Dynamic entities have no class, reverts to EntityName. -AK
			string qualifiedName = model.MappedClass == null ? model.EntityName : model.MappedClass.AssemblyQualifiedName;
			mappings.AddImport(qualifiedName, model.EntityName);
			if (mappings.IsAutoImport && model.EntityName.IndexOf('.') > 0)
				mappings.AddImport(qualifiedName, StringHelper.Unqualify(model.EntityName));

			// BATCH SIZE
			if (classMapping.BatchSize.HasValue)
				model.BatchSize = classMapping.BatchSize.Value;

			// SELECT BEFORE UPDATE
			model.SelectBeforeUpdate = classMapping.SelectBeforeUpdate;

			// META ATTRIBUTES
			model.MetaAttributes = GetMetas(classMapping, inheritedMetas);

			// PERSISTER
			if(!string.IsNullOrEmpty(classMapping.Persister))
				model.EntityPersisterClass = ClassForNameChecked(classMapping.Persister, mappings,
																 "could not instantiate persister class: {0}");

			// CUSTOM SQL
			HandleCustomSQL(classMapping, model);
			if (classMapping.SqlLoader != null)
				model.LoaderName = classMapping.SqlLoader.queryref;

			foreach (var synchronize in classMapping.Synchronize)
			{
				model.AddSynchronizedTable(synchronize.table);
			}

			model.IsAbstract = classMapping.IsAbstract;
		}

		private void BindMapRepresentation(IEntityMapping classMapping, PersistentClass entity)
		{
			HbmTuplizer tuplizer = classMapping.Tuplizers.FirstOrDefault(tp => tp.entitymode == HbmTuplizerEntitymode.DynamicMap);
			if (tuplizer != null)
			{
				string tupClassName = FullQualifiedClassName(tuplizer.@class, mappings);
				entity.AddTuplizer(EntityMode.Map, tupClassName);
			}
		}

		private void BindXmlRepresentation(IEntityMapping classMapping, PersistentClass entity)
		{
			entity.NodeName = string.IsNullOrEmpty(classMapping.Node) ? StringHelper.Unqualify(entity.EntityName): classMapping.Node;

			HbmTuplizer tuplizer = classMapping.Tuplizers.FirstOrDefault(tp => tp.entitymode == HbmTuplizerEntitymode.Xml);
			if (tuplizer != null)
			{
				string tupClassName = FullQualifiedClassName(tuplizer.@class, mappings);
				entity.AddTuplizer(EntityMode.Xml, tupClassName);
			}
		}

		private void BindPocoRepresentation(IEntityMapping classMapping, PersistentClass entity)
		{
			string className = classMapping.Name == null
								? null
								   : ClassForNameChecked(classMapping.Name, mappings, "persistent class {0} not found").AssemblyQualifiedName;

			entity.ClassName = className;

			if (!string.IsNullOrEmpty(classMapping.Proxy))
			{
				entity.ProxyInterfaceName = ClassForNameChecked(classMapping.Proxy, mappings, "proxy class not found: {0}").AssemblyQualifiedName;
				entity.IsLazy = true;
			}
			else if (entity.IsLazy)
				entity.ProxyInterfaceName = className;

			HbmTuplizer tuplizer = classMapping.Tuplizers.FirstOrDefault(tp=> tp.entitymode == HbmTuplizerEntitymode.Poco);
			if (tuplizer != null)
			{
				string tupClassName = FullQualifiedClassName(tuplizer.@class, mappings);
				entity.AddTuplizer(EntityMode.Poco, tupClassName);
			}
		}

		protected void BindJoins(IEnumerable<HbmJoin> joins, PersistentClass persistentClass, IDictionary<string, MetaAttribute> inheritedMetas)
		{
			foreach (var hbmJoin in joins)
			{
				var join = new Join { PersistentClass = persistentClass };
				BindJoin(hbmJoin, join, inheritedMetas);
				persistentClass.AddJoin(join);
			}
		}

		private void BindJoin(HbmJoin joinMapping, Join join, IDictionary<string, MetaAttribute> inheritedMetas)
		{
			PersistentClass persistentClass = join.PersistentClass;

			// TABLENAME
			string schema = joinMapping.schema ?? mappings.SchemaName;
			string catalog = joinMapping.catalog ?? mappings.CatalogName;

			string action = "all"; // joinMapping.schemaaction ?? "all";

			string tableName = joinMapping.table;
			Table table = mappings.AddTable(schema, catalog, GetClassTableName(persistentClass, tableName), joinMapping.Subselect, false, action);
			join.Table = table;

			join.IsSequentialSelect = joinMapping.fetch == HbmJoinFetch.Select;
			join.IsInverse = joinMapping.inverse;
			join.IsOptional = joinMapping.optional;

			log.InfoFormat("Mapping class join: {0} -> {1}", persistentClass.EntityName, join.Table.Name);

			// KEY
			SimpleValue key;
			if (!String.IsNullOrEmpty(joinMapping.key.propertyref))
			{
				string propertyRef = joinMapping.key.propertyref;
				var propertyRefKey = new SimpleValue(persistentClass.Table)
					{
						IsAlternateUniqueKey = true
					};
				var property = persistentClass.GetProperty(propertyRef);
				join.RefIdProperty = property;
				//we only want one column
				var column = (Column) property.ColumnIterator.First();
				if (!column.Unique)
					throw new MappingException(
						string.Format(
							"Property {0}, on class {1} must be marked as unique to be joined to with a property-ref.",
							property.Name,
							persistentClass.ClassName));
				propertyRefKey.AddColumn(column);
				propertyRefKey.TypeName = property.Type.Name;
				key = new ReferenceDependantValue(table, propertyRefKey);
			}
			else
			{
				key = new DependantValue(table, persistentClass.Identifier);
			}

			key.ForeignKeyName = joinMapping.key.foreignkey;
			join.Key = key;
			key.IsCascadeDeleteEnabled = joinMapping.key.ondelete == HbmOndelete.Cascade;
			new ValuePropertyBinder(key, Mappings).BindSimpleValue(joinMapping.key, persistentClass.EntityName, false);

			join.CreatePrimaryKey(dialect);
			join.CreateForeignKey();

			// PROPERTIES
			new PropertiesBinder(Mappings, persistentClass, dialect).Bind(joinMapping.Properties, join.Table,
																							inheritedMetas, p => { },
																							join.AddProperty);

			// CUSTOM SQL
			HandleCustomSQL(joinMapping, join);
		}

		private void HandleCustomSQL(IEntitySqlsMapping sqlsMapping, ISqlCustomizable model)
		{
			var sqlInsert = sqlsMapping.SqlInsert;
			if (sqlInsert != null)
			{
				bool callable = sqlInsert.callableSpecified && sqlInsert.callable;
				model.SetCustomSQLInsert(sqlInsert.Text.LinesToString(), callable, GetResultCheckStyle(sqlInsert));
			}

			var sqlDelete = sqlsMapping.SqlDelete;
			if (sqlDelete != null)
			{
				bool callable = sqlDelete.callableSpecified && sqlDelete.callable;
				model.SetCustomSQLDelete(sqlDelete.Text.LinesToString(), callable, GetResultCheckStyle(sqlDelete));
			}

			var sqlUpdate = sqlsMapping.SqlUpdate;
			if (sqlUpdate != null)
			{
				bool callable = sqlUpdate.callableSpecified && sqlUpdate.callable;
				model.SetCustomSQLUpdate(sqlUpdate.Text.LinesToString(), callable, GetResultCheckStyle(sqlUpdate));
			}
		}

		protected PersistentClass GetSuperclass(string extendsName)
		{
			if (string.IsNullOrEmpty(extendsName))
			{
				throw new MappingException("'extends' attribute is not found or is empty.");
			}
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

		protected string GetClassTableName(PersistentClass model, string mappedTableName)
		{
			return string.IsNullOrEmpty(mappedTableName)
					? mappings.NamingStrategy.ClassToTableName(model.EntityName)
					: mappings.NamingStrategy.TableName(mappedTableName);
		}

		protected void BindComponent(IComponentMapping componentMapping, Component model, System.Type reflectedClass, string className,
									 string path, bool isNullable,
									 IDictionary<string, MetaAttribute> inheritedMetas)
		{
			model.RoleName = path;
			inheritedMetas = GetMetas(componentMapping as IDecoratable, inheritedMetas);
			model.MetaAttributes = inheritedMetas;
			var componentClassName = componentMapping.Class;
			if (!string.IsNullOrEmpty(componentClassName))
			{
				model.ComponentClass = ClassForNameChecked(componentClassName, mappings, "component class not found: {0}");
				model.ComponentClassName = FullQualifiedClassName(componentClassName, mappings);
				model.IsEmbedded = false;
			}
			else if (componentMapping is HbmDynamicComponent)
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
				model.IsEmbedded = true;
				if (model.Owner.HasPocoRepresentation)
				{
					model.ComponentClass = model.Owner.MappedClass;
				}
				else
				{
					model.IsDynamic = true;
				}
			}

			string nodeName = !string.IsNullOrEmpty(componentMapping.EmbeddedNode)
								? componentMapping.EmbeddedNode
								: !string.IsNullOrEmpty(componentMapping.Name) ? componentMapping.Name : model.Owner.NodeName;
			model.NodeName = nodeName;

			// Parent
			if (componentMapping.Parent != null && !string.IsNullOrEmpty(componentMapping.Parent.name))
			{
				model.ParentProperty = new Property
										{
											Name = componentMapping.Parent.name,
											PropertyAccessorName = componentMapping.Parent.access ?? mappings.DefaultAccess
										}; 
			}

			new PropertiesBinder(Mappings, model, className, path, isNullable, Mappings.Dialect).Bind(
				componentMapping.Properties, model.Table, inheritedMetas, p =>
					{ }, model.AddProperty);
		}

		protected void BindForeignKey(string foreignKey, SimpleValue value)
		{
			if (!string.IsNullOrEmpty(foreignKey))
			{
				value.ForeignKeyName = foreignKey;
			}
		}

		protected void BindAny(HbmAny node, Any model, bool isNullable)
		{
			model.IdentifierTypeName = node.idtype;
			new TypeBinder(model, Mappings).Bind(node.idtype);

			BindAnyMeta(node, model);

			new ColumnsBinder(model, Mappings).Bind(node.Columns, isNullable, null);
		}

		protected void BindAnyMeta(IAnyMapping anyMapping, Any model)
		{
			if(string.IsNullOrEmpty(anyMapping.MetaType))
			{
				return;
			}
				model.MetaType = anyMapping.MetaType;
			var metaValues = anyMapping.MetaValues;
			if (metaValues.Count == 0)
			{
				return;
			}
			IDictionary<object, string> values = new Dictionary<object, string>();
			IType metaType = TypeFactory.HeuristicType(model.MetaType);

			foreach (var metaValue in metaValues)
			{
				try
				{
					object value = ((IDiscriminatorType)metaType).StringToObject(metaValue.value);
					string entityName = GetClassName(metaValue.@class, mappings);
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
			}
			model.MetaValues = values.Count > 0 ? values : null;
		}

		protected void BindOneToOne(HbmOneToOne oneToOneMapping, OneToOne model)
		{
			model.IsConstrained = oneToOneMapping.constrained;

			model.ForeignKeyType = oneToOneMapping.constrained
									? ForeignKeyDirection.ForeignKeyFromParent
									: ForeignKeyDirection.ForeignKeyToParent;

			InitOuterJoinFetchSetting(oneToOneMapping, model);
			InitLaziness(oneToOneMapping.Lazy, model, true);
			BindForeignKey(oneToOneMapping.foreignkey, model);

			model.ReferencedPropertyName = oneToOneMapping.propertyref;

			model.ReferencedEntityName = GetEntityName(oneToOneMapping, mappings);
			model.PropertyName = oneToOneMapping.Name;
		}

		protected static ExecuteUpdateResultCheckStyle GetResultCheckStyle(HbmCustomSQL customSQL)
		{
			if (customSQL != null)
			{
				if (!customSQL.checkSpecified)
				{
					return ExecuteUpdateResultCheckStyle.Count;
				}
				switch (customSQL.check)
				{
					case HbmCustomSQLCheck.None:
						return ExecuteUpdateResultCheckStyle.None;
					case HbmCustomSQLCheck.Rowcount:
						return ExecuteUpdateResultCheckStyle.Count;
					case HbmCustomSQLCheck.Param:
						return null; // not supported
				}
			}
			return null;
		}

		protected static void InitLaziness(HbmRestrictedLaziness? restrictedLaziness, ToOne fetchable, bool defaultLazy)
		{
			// Note : fetch="join" overrides default laziness
			bool isLazyTrue = !restrictedLaziness.HasValue
								? defaultLazy && fetchable.IsLazy
								: restrictedLaziness == HbmRestrictedLaziness.Proxy;
			fetchable.IsLazy = isLazyTrue;
		}

		protected static void InitLaziness(HbmLaziness? laziness, ToOne fetchable, bool defaultLazy)
		{
			// Note : fetch="join" overrides default laziness
			bool isLazyTrue = !laziness.HasValue
													? defaultLazy && fetchable.IsLazy
													: (laziness == HbmLaziness.Proxy || laziness == HbmLaziness.NoProxy);
			fetchable.IsLazy = isLazyTrue;
			fetchable.UnwrapProxy = laziness == HbmLaziness.NoProxy;
		}

		protected void InitOuterJoinFetchSetting(HbmManyToMany manyToMany, IFetchable model)
		{
			FetchMode fetchStyle;
			bool lazy = true;

			if (!manyToMany.fetchSpecified)
			{
				if (!manyToMany.outerjoinSpecified)
				{
					//NOTE SPECIAL CASE:
					// default to join and non-lazy for the "second join"
					// of the many-to-many
					lazy = false;
					fetchStyle = FetchMode.Join;
				}
				else
				{
					fetchStyle = GetFetchStyle(manyToMany.outerjoin);
				}
			}
			else
			{
				fetchStyle = GetFetchStyle(manyToMany.fetch);
			}

			model.FetchMode = fetchStyle;
			model.IsLazy = lazy;
		}

		protected FetchMode GetFetchStyle(HbmFetchMode fetchModeMapping)
		{
			FetchMode fetchStyle;
			switch (fetchModeMapping)
			{
				case HbmFetchMode.Select:
					fetchStyle = FetchMode.Select;
					break;
				case HbmFetchMode.Join:
					fetchStyle = FetchMode.Join;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			return fetchStyle;
		}

		protected FetchMode GetFetchStyle(HbmOuterJoinStrategy outerJoinStrategyMapping)
		{
			// use old (HB 2.1) defaults if outer-join is specified
			FetchMode fetchStyle;
			switch (outerJoinStrategyMapping)
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
			return fetchStyle;
		}

		protected void InitOuterJoinFetchSetting(HbmOneToOne oneToOne, OneToOne model)
		{
			FetchMode fetchStyle;
			bool lazy = true;

			if (!oneToOne.fetchSpecified)
			{
				if (!oneToOne.outerjoinSpecified)
				{
					//NOTE SPECIAL CASE:
					// one-to-one constrained=falase cannot be proxied,
					// so default to join and non-lazy
					lazy = model.IsConstrained;
					fetchStyle = lazy ? FetchMode.Default : FetchMode.Join;
				}
				else
				{
					fetchStyle = GetFetchStyle(oneToOne.outerjoin);
				}
			}
			else
			{
				fetchStyle = GetFetchStyle(oneToOne.fetch);
			}

			model.FetchMode = fetchStyle;
			model.IsLazy = lazy;
		}

		protected static string GetEntityName(IRelationship relationship, Mappings mappings)
		{
			string entityName = string.IsNullOrEmpty(relationship.EntityName) ? null : relationship.EntityName;
			string className = string.IsNullOrEmpty(relationship.Class) ? null : relationship.Class;
			return entityName ?? (className == null ? null : StringHelper.GetFullClassname(FullQualifiedClassName(className, mappings)));
		}
	}
}
