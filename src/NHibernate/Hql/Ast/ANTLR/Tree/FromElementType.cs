using System;
using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Param;
using NHibernate.Persister.Collection;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Util;
using IQueryable = NHibernate.Persister.Entity.IQueryable;

namespace NHibernate.Hql.Ast.ANTLR.Tree
{
	/// <summary>
	/// Delegate that handles the type and join sequence information for a FromElement.
	/// Author: josh
	/// Ported by: Steve Strong
	/// </summary>
	[CLSCompliant(false)]
	public class FromElementType
	{
		private static readonly INHibernateLogger Log = NHibernateLogger.For(typeof(FromElementType));

		private readonly FromElement _fromElement;
		private readonly IEntityPersister _persister;
		private readonly IPropertyMapping _propertyMapping;
		private readonly IType _type;
		private IQueryableCollection _queryableCollection;
		private CollectionPropertyMapping _collectionPropertyMapping;
		private JoinSequence _joinSequence;
		private IParameterSpecification _indexCollectionSelectorParamSpec;
		private string _collectionSuffix;

		public FromElementType(FromElement fromElement, IEntityPersister persister, EntityType entityType)
			: this(fromElement, persister, null, entityType)
		{
		}

		internal FromElementType(FromElement fromElement, IEntityPersister persister, IPropertyMapping propertyMapping, IType type)
		{
			_fromElement = fromElement;
			_persister = persister;
			_type = type;
			_propertyMapping = propertyMapping ?? _persister as IPropertyMapping;

			var queryable = persister as IQueryable;
			if (queryable != null)
				fromElement.Text = queryable.TableName + " " + fromElement.TableAlias;
		}

		protected FromElementType(FromElement fromElement)
		{
			_fromElement = fromElement;
		}

		public IEntityPersister EntityPersister
		{
			get { return _persister; }
		}

		private string TableAlias
		{
			get { return _fromElement.TableAlias; }
		}

		private string CollectionTableAlias
		{
			get { return _fromElement.CollectionTableAlias; }
		}

		public virtual IType DataType
		{
			get
			{
				if (_persister == null && _queryableCollection != null)
				{
					return _queryableCollection.Type;
				}

				return _type;
			}
		}

		public IType SelectType
		{
			get
			{
				if (!(_type is EntityType entityType))
				{
					return null;
				}

				bool shallow = _fromElement.FromClause.Walker.IsShallowQuery;
				return TypeFactory.ManyToOne(entityType.GetAssociatedEntityName(), shallow);
			}
		}

		public string CollectionSuffix
		{
			get { return _collectionSuffix; }
			set { _collectionSuffix = value; }
		}

		public string EntitySuffix { get; set; }

		public IParameterSpecification IndexCollectionSelectorParamSpec
		{
			get { return _indexCollectionSelectorParamSpec; }
			set { _indexCollectionSelectorParamSpec = value; }
		}

		public JoinSequence JoinSequence
		{
			get
			{
				if (_joinSequence != null)
				{
					return _joinSequence;
				}

				// Class names in the FROM clause result in a JoinSequence (the old FromParser does this).
				var joinable = _persister as IJoinable;
				if (joinable != null)
				{
					// the delete and update statements created here will never be executed when IsMultiTable is true,
					// only the where clause will be used by MultiTableUpdateExecutor/MultiTableDeleteExecutor. In that case
					// we have to use the alias from the persister.
					var useAlias = _fromElement.UseTableAliases || _fromElement.Queryable.IsMultiTable;

					return _fromElement.SessionFactoryHelper.CreateJoinSequence().SetRoot(joinable, useAlias ? TableAlias : string.Empty);
				}
				
				return null; // TODO: Should this really return null?  If not, figure out something better to do here.
			}
			set { _joinSequence = value; }
		}

		/// <summary>
		/// Returns the identifier select SQL fragment.
		/// </summary>
		/// <param name="size">The total number of returned types.</param>
		/// <param name="k">The sequence of the current returned type.</param>
		/// <returns>the identifier select SQL fragment.</returns>
		// Since v5.4
		[Obsolete("Use GetIdentifierSelectFragment method instead.")]
		public string RenderIdentifierSelect(int size, int k)
		{
			return GetIdentifierSelectFragment(GetSuffix(size, k))?.ToSqlStringFragment(false) ?? string.Empty;
		}

		/// <summary>
		/// Gets the identifier select fragment.
		/// </summary>
		/// <param name="suffix">The column suffix.</param>
		/// <returns>The identifier select fragment.</returns>
		public SelectFragment GetIdentifierSelectFragment(string suffix)
		{
			return GetIdentifierSelectFragment(suffix, _fromElement.ParentFromElement?.TableAlias ?? TableAlias);
		}

		internal SelectFragment GetIdentifierSelectFragment(string suffix, string alias)
		{
			CheckInitialized();

			// Render the identifier select fragment using the table alias.
			if (_fromElement.FromClause.IsScalarSubQuery)
			{
				var queryable = Queryable;
				if (queryable == null)
					return null;

				return new SelectFragment(queryable.Factory.Dialect)
				       .AddColumns(alias, queryable.IdentifierColumnNames);
			}
			else if (_fromElement is JoinSubqueryFromElement joinSubquery)
			{
				var columns = joinSubquery.PropertyMapping.GetIdentifiersColumns(alias);
				return columns.Count > 0 
					? new SelectFragment(_fromElement.Walker.SessionFactoryHelper.Factory.Dialect).AddColumns(columns)
					: null;
			}
			else
			{
				var queryable = Queryable;
				if (queryable == null)
					throw new QueryException("not an entity");

				return queryable.GetIdentifierSelectFragment(alias, suffix)
				                .UseAliasesAsColumns(_fromElement.ParentFromElement != null);
			}
		}

		/// <summary>
		/// Render the identifier select, but in a 'scalar' context (i.e. generate the column alias).
		/// </summary>
		/// <param name="i">the sequence of the returned type</param>
		/// <returns>the identifier select with the column alias.</returns>
		// Since v5.4
		[Obsolete("Use GetScalarIdentifierSelectFragment method instead.")]
		public virtual string RenderScalarIdentifierSelect(int i)
		{
			return GetScalarIdentifierSelectFragment(i, NameGenerator.ScalarName).ToSqlStringFragment(false);
		}

		/// <summary>
		/// Gets the identifier select fragment, but in a 'scalar' context (i.e. generate the column alias).
		/// </summary>
		/// <param name="i">The sequence of the returned type</param>
		/// <param name="aliasCreator">A function to generate aliases.</param>
		/// <returns>The identifier select fragment.</returns>
		public virtual SelectFragment GetScalarIdentifierSelectFragment(int i, Func<int,int, string> aliasCreator)
		{
			CheckInitialized();
			var cols = GetPropertyMapping(Persister.Entity.EntityPersister.EntityID).ToColumns(TableAlias, Persister.Entity.EntityPersister.EntityID);
			var factory = Queryable?.Factory ?? QueryableCollection?.Factory ?? throw new QueryException("not an entity or collection");
			var fragment = new SelectFragment(factory.Dialect);
			// For property references generate <tablealias>.<columnname> as <projectionalias>
			for (var j = 0; j < cols.Length; j++)
			{
				fragment.AddColumn(null, cols[j], aliasCreator(i, j));
			}

			return fragment;
		}

		/// <summary>
		/// Returns the property select SQL fragment.
		/// </summary>
		/// <param name="size">The total number of returned types.</param>
		/// <param name="k">The sequence of the current returned type.</param>
		/// <param name="allProperties"></param>
		/// <returns>the property select SQL fragment.</returns>
		// Since v5.4
		[Obsolete("This method has no more usage in NHibernate and will be removed in a future version.")]
		public string RenderPropertySelect(int size, int k, bool allProperties)
		{
			return GetPropertiesSelectFragment(GetSuffix(size, k), null, allProperties, TableAlias).ToSqlStringFragment();
		}

		/// <summary>
		/// Gets the properties select fragment.
		/// </summary>
		/// <param name="suffix">The column suffix.</param>
		/// <param name="allProperties">Whether to include all lazy properties.</param>
		/// <param name="alias">The alias for the columns.</param>
		/// <returns>The properties select fragment.</returns>
		internal SelectFragment GetPropertiesSelectFragment(string suffix, bool allProperties, string alias)
		{
			return GetPropertiesSelectFragment(suffix, null, allProperties, alias);
		}

		// Since v5.4
		[Obsolete("This method has no more usage in NHibernate and will be removed in a future version.")]
		public string RenderPropertySelect(int size, int k, string[] fetchLazyProperties)
		{
			return GetPropertiesSelectFragment(GetSuffix(size, k), fetchLazyProperties, false, TableAlias).ToSqlStringFragment();
		}

		/// <summary>
		/// Gets the properties select fragment.
		/// </summary>
		/// <param name="suffix">The column suffix.</param>
		/// <param name="fetchLazyProperties">Lazy properties to be included.</param>
		/// <param name="alias">The alias for the columns.</param>
		/// <returns>The properties select fragment.</returns>
		internal SelectFragment GetPropertiesSelectFragment(string suffix, string[] fetchLazyProperties, string alias)
		{
			return GetPropertiesSelectFragment(suffix, fetchLazyProperties, false, alias);
		}

		/// <summary>
		/// Gets the properties select fragment.
		/// </summary>
		/// <param name="suffix">The column suffix.</param>
		/// <param name="fetchLazyProperties">Lazy properties to be included.</param>
		/// <param name="allProperties">Whether to include all lazy properties.</param>
		/// <param name="alias">The alias for the columns.</param>
		/// <returns>The properties select fragment.</returns>
		private SelectFragment GetPropertiesSelectFragment(string suffix, string[] fetchLazyProperties, bool allProperties, string alias)
		{
			CheckInitialized();

			if (_fromElement is JoinSubqueryFromElement joinSubquery)
			{
				return new SelectFragment(_fromElement.Walker.SessionFactoryHelper.Factory.Dialect)
					.AddColumns(joinSubquery.PropertyMapping.GetPropertiesColumns(alias));
			}

			// Use the old method when fetchProperties is null to prevent any breaking changes
			// 6.0 TODO: simplify condition by removing the fetchProperties part
			var fragment = fetchLazyProperties == null || allProperties
				? Queryable?.GetPropertiesSelectFragment(alias, suffix, allProperties)
				: Queryable?.GetPropertiesSelectFragment(alias, suffix, fetchLazyProperties);

			return fragment?.UseAliasesAsColumns(_fromElement.ParentFromElement != null);
		}

		// Since v5.4
		[Obsolete("Use GetCollectionSelectFragment method instead.")]
		public string RenderCollectionSelectFragment(int size, int k)
		{
			return GetCollectionSelectFragment(GenerateSuffix(size, k))?.ToSqlStringFragment(false) ?? string.Empty;
		}

		/// <summary>
		/// Gets the collection select fragment.
		/// </summary>
		/// <param name="suffix">The column suffix.</param>
		/// <returns>The collection select fragment</returns>
		public SelectFragment GetCollectionSelectFragment(string suffix)
		{
			if (_queryableCollection == null)
				return null;

			if (_collectionSuffix == null)
				_collectionSuffix = suffix;

			return _queryableCollection.GetSelectFragment(_fromElement.ParentFromElement?.TableAlias ?? CollectionTableAlias, _collectionSuffix)
			                           .UseAliasesAsColumns(_fromElement.ParentFromElement != null);
		}

		// Since v5.4
		[Obsolete("Use GetValueCollectionSelectFragment method instead.")]
		public string RenderValueCollectionSelectFragment(int size, int k)
		{
			return GetValueCollectionSelectFragment(GenerateSuffix(size, k))?.ToSqlStringFragment(false) ?? string.Empty;
		}

		/// <summary>
		/// Gets the value collection select fragment.
		/// </summary>
		/// <param name="suffix">The column suffix.</param>
		/// <returns>The value collection select fragment</returns>
		public SelectFragment GetValueCollectionSelectFragment(string suffix)
		{
			if (_queryableCollection == null)
				return null;

			if (_collectionSuffix == null)
				_collectionSuffix = suffix;

			return _queryableCollection.GetSelectFragment(_fromElement.ParentFromElement?.TableAlias ?? TableAlias, _collectionSuffix)
			                           .UseAliasesAsColumns(_fromElement.ParentFromElement != null);
		}

		public bool IsEntity
		{
			get { return _persister != null; }
		}

		public bool IsCollectionOfValuesOrComponents
		{
			get
			{
				if (_persister != null)
					return false;

				if (_queryableCollection == null)
					return false;
				
				return !_queryableCollection.ElementType.IsEntityType;
			}
		}

		public virtual IPropertyMapping GetPropertyMapping(string propertyName)
		{
			CheckInitialized();

			if (_queryableCollection == null)
			{
				// Not a collection?
				return _propertyMapping;
			}

			// If the property is a special collection property name, return a CollectionPropertyMapping.
			if (CollectionProperties.IsCollectionProperty(propertyName))
			{
				if (_collectionPropertyMapping == null)
				{
					_collectionPropertyMapping = new CollectionPropertyMapping(_queryableCollection);
				}
				return _collectionPropertyMapping;
			}
			if (_queryableCollection.ElementType.IsAnyType)
			{
				// collection of <many-to-any/> mappings...
				// used to circumvent the component-collection check below...
				return _queryableCollection;
			}

			if (_queryableCollection.ElementType.IsComponentType)
			{
				// Collection of components.
				if (propertyName == Persister.Entity.EntityPersister.EntityID)
				{
					return (IPropertyMapping)_queryableCollection.OwnerEntityPersister;
				}
			}

			return _queryableCollection;
		}

		/// <summary>
		/// Returns the type of a property, given it's name (the last part) and the full path.
		/// </summary>
		/// <param name="propertyName">The last part of the full path to the property.</param>
		/// <param name="propertyPath">The full property path.</param>
		/// <returns>The type</returns>
		public virtual IType GetPropertyType(string propertyName, string propertyPath)
		{
			CheckInitialized();

			IType type = null;

			// If this is an entity and the property is the identifier property, then use getIdentifierType().
			//      Note that the propertyName.equals( propertyPath ) checks whether we have a component
			//      key reference, where the component class property name is the same as the
			//      entity id property name; if the two are not equal, this is the case and
			//      we'd need to "fall through" to using the property mapping.
			if (_persister != null && (propertyName == propertyPath) && propertyName == _persister.IdentifierPropertyName)
			{
				type = _persister.IdentifierType;
			}
			else
			{	// Otherwise, use the property mapping.
				IPropertyMapping mapping = GetPropertyMapping(propertyName);
				type = mapping.ToType(propertyPath);
			}
			if (type == null)
			{
				throw new MappingException("Property " + propertyName + " does not exist in " +
						((_queryableCollection == null) ? "class" : "collection") + " "
						+ ((_queryableCollection == null) ? _fromElement.ClassName : _queryableCollection.Role));
			}
			return type;
		}

		public string[] ToColumns(string tableAlias, string path, bool inSelect)
		{
			return ToColumns(tableAlias, path, inSelect, false);
		}

		/// <summary>
		/// Returns the Hibernate queryable implementation for the HQL class.
		/// </summary>
		public IQueryable Queryable
		{
			get { return _persister as IQueryable; }
		}

		public virtual IQueryableCollection QueryableCollection
		{
			get { return _queryableCollection; }
			set
			{
				if (_queryableCollection != null)
				{
					throw new InvalidOperationException("QueryableCollection is already defined for " + this + "!");
				}
				_queryableCollection = value;
				if (!_queryableCollection.IsOneToMany)
				{
					// For many-to-many joins, use the tablename from the queryable collection for the default text.
					_fromElement.Text = _queryableCollection.TableName + " " + TableAlias;
				}
			}
		}

		public string[] ToColumns(string tableAlias, string path, bool inSelect, bool forceAlias)
		{
			CheckInitialized();
			IPropertyMapping propertyMapping = GetPropertyMapping(path);

			// If this from element is a collection and the path is a collection property (maxIndex, etc.) then
			// generate a sub-query.
			if (!inSelect && _queryableCollection != null && CollectionProperties.IsCollectionProperty(path))
			{
				IDictionary<string, IFilter> enabledFilters = _fromElement.Walker.EnabledFilters;

				string subquery = CollectionSubqueryFactory.CreateCollectionSubquery(
						_joinSequence,
						enabledFilters,
						propertyMapping.ToColumns(tableAlias, path)
				);
				if (Log.IsDebugEnabled())
				{
					Log.Debug("toColumns({0},{1}) : subquery = {2}", tableAlias, path, subquery);
				}
				return new [] { "(" + subquery + ")" };
			}
			else
			{
				if (forceAlias)
				{
					return propertyMapping.ToColumns(tableAlias, path);
				}
				else if (_fromElement.Walker.StatementType == HqlSqlWalker.SELECT)
				{
					return propertyMapping.ToColumns(tableAlias, path);
				}
				else if (_fromElement.Walker.CurrentClauseType == HqlSqlWalker.SELECT)
				{
					return propertyMapping.ToColumns(tableAlias, path);
				}
				else if (_fromElement.Walker.IsSubQuery)
				{
					// We already know it's subqery for DML query.
					// If this FROM_ELEMENT represents a correlation to the outer-most query we must use real table name
					// for UPDATE(typically in a SET clause)/DELETE queries unless it's multi-table reference inside top level where clause
					// (as this subquery will actually be used in the "id select" phase of that multi-table executor)
					var useAlias = _fromElement.Walker.StatementType == HqlSqlWalker.INSERT
						|| (IsMultiTable && _fromElement.Walker.CurrentTopLevelClauseType == HqlSqlWalker.WHERE);

					if (!useAlias && IsCorrelation)
						return propertyMapping.ToColumns(ExtractTableName(), path);

					return propertyMapping.ToColumns(tableAlias, path);
				}
				else
				{
					string[] columns = propertyMapping.ToColumns(path);
					Log.Info("Using non-qualified column reference [{0} -> ({1})]", path, ArrayHelper.ToString(columns));
					return columns;
				}
			}
		}

		private static string GetSuffix(int size, int sequence)
		{
			return GenerateSuffix(size, sequence);
		}

		private static string GenerateSuffix(int size, int k)
		{
			String suffix = size == 1 ? "" : k.ToString() + '_';
			return suffix;
		}

		private void CheckInitialized()
		{
			_fromElement.CheckInitialized();
		}

		private bool IsCorrelation
		{
			get
			{
				FromClause top = _fromElement.Walker.GetFinalFromClause();
				return _fromElement.FromClause != _fromElement.Walker.CurrentFromClause &&
					   _fromElement.FromClause == top;
			}
		}

		private bool IsMultiTable
		{
			get
			{
				// should be safe to only ever expect EntityPersister references here
				return _fromElement.Queryable != null &&
					   _fromElement.Queryable.IsMultiTable;
			}
		}

		private string ExtractTableName()
		{
			// should be safe to only ever expect EntityPersister references here
			return _fromElement.Queryable.TableName;
		}
	}
}
