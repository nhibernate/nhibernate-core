using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Antlr.Runtime;

using NHibernate.Hql.Ast.ANTLR.Util;
using NHibernate.Param;
using NHibernate.Persister.Collection;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Util;
using IQueryable = NHibernate.Persister.Entity.IQueryable;

namespace NHibernate.Hql.Ast.ANTLR.Tree
{
	[CLSCompliant(false)]
	public class FromElement : HqlSqlWalkerNode, IDisplayableNode, IParameterContainer
	{
		private static readonly IInternalLogger Log = LoggerProvider.LoggerFor(typeof(FromElement));

		private bool _isAllPropertyFetch;
		private FromElementType _elementType;
		private string _tableAlias;
		private string _classAlias;
		private string _className;
		private string _collectionTableAlias;
		private FromClause _fromClause;
		private string[] _columns;
		private FromElement _origin;
		private bool _useFromFragment;
		private bool _useWhereFragment = true;
		private bool _includeSubclasses = true;
		private readonly List<FromElement> _destinations = new List<FromElement>();
		private bool _dereferencedBySubclassProperty;
		private bool _dereferencedBySuperclassProperty;
		private bool _collectionJoin;
		private string _role;
		private bool _initialized;
		private SqlString _withClauseFragment;
		private string _withClauseJoinAlias;
		private bool _filter;
		private IToken _token;

		public FromElement(IToken token) : base(token)
		{
			_token= token;
		}

		/// <summary>
		/// Constructor form used to initialize <see cref="ComponentJoin"/>.
		/// </summary>
		/// <param name="fromClause">The FROM clause to which this element belongs.</param>
		/// <param name="origin">The origin (LHS) of this element.</param>
		/// <param name="alias">The alias applied to this element.</param>
		protected FromElement(FromClause fromClause,FromElement origin,string alias):this(origin._token)
		{
			_fromClause = fromClause;
			_origin = origin;
			_classAlias = alias;
			_tableAlias = origin.TableAlias;
			base.Initialize(fromClause.Walker);
		}

		protected void InitializeComponentJoin(FromElementType elementType)
		{
			_elementType = elementType;
			_fromClause.RegisterFromElement(this);
			_initialized = true;
		}

		public void SetAllPropertyFetch(bool fetch)
		{
			_isAllPropertyFetch = fetch;
		}

		public void SetWithClauseFragment(String withClauseJoinAlias, SqlString withClauseFragment)
		{
			_withClauseJoinAlias = withClauseJoinAlias;
			_withClauseFragment = withClauseFragment;
		}

		public Engine.JoinSequence JoinSequence
		{
			get { return _elementType.JoinSequence; }
			set { _elementType.JoinSequence = value; }
		}

		public string[] Columns
		{
			get { return _columns; }
			set { _columns = value; }
		}

		public bool IsEntity
		{
			get { return _elementType.IsEntity; }
		}

		public bool IsFromOrJoinFragment
		{
			get { return Type == HqlSqlWalker.FROM_FRAGMENT || Type == HqlSqlWalker.JOIN_FRAGMENT; }
		}

		public bool IsAllPropertyFetch
		{
			get { return _isAllPropertyFetch; }
			set { _isAllPropertyFetch = value; }
		}

		public virtual bool IsImpliedInFromClause
		{
			get { return false; }  // Since this is an explicit FROM element, it can't be implied in the FROM clause.
		}

		public bool IsFetch
		{
			get { return _fetch; }
		}

		public bool Filter
		{
			set { _filter = value; }
		}

		public bool IsFilter
		{
			get { return _filter; }
		}

		public IParameterSpecification[] GetEmbeddedParameters()
		{
			return _embeddedParameters.ToArray();
		}

		public bool HasEmbeddedParameters
		{
			get { return _embeddedParameters != null && _embeddedParameters.Count > 0; }
		}

		public IParameterSpecification IndexCollectionSelectorParamSpec
		{
			get { return _elementType.IndexCollectionSelectorParamSpec; }
			set
			{
				if (value == null)
				{
					if (_elementType.IndexCollectionSelectorParamSpec != null)
					{
						_embeddedParameters.Remove(_elementType.IndexCollectionSelectorParamSpec);
						_elementType.IndexCollectionSelectorParamSpec = null;
					}
				}
				else
				{
					_elementType.IndexCollectionSelectorParamSpec = value;
					AddEmbeddedParameter(value);
				}
			}
		}

		/// <summary>
		/// Returns true if this FromElement was implied by a path, or false if this FROM element is explicitly declared in
		/// the FROM clause.
		/// </summary>
		public virtual bool IsImplied
		{
			get { return false; } // This is an explicit FROM element.
		}

		public bool IsDereferencedBySuperclassOrSubclassProperty
		{
			get 
			{
				return _dereferencedBySubclassProperty || _dereferencedBySuperclassProperty;
			}
		}

		public bool IsDereferencedBySubclassProperty
		{
			get { return _dereferencedBySubclassProperty; }
		}

		public IEntityPersister EntityPersister
		{
			get { return _elementType.EntityPersister; }
		}

		public override IType DataType
		{
			get
			{
				return _elementType.DataType;
			}
			set
			{
				base.DataType = value;
			}
		}

		public string TableAlias
		{
			get { return _tableAlias; }
		}

		private string TableName
		{
			get
			{
				IQueryable queryable = Queryable;
				return (queryable != null) ? queryable.TableName : "{none}";
			}
		}

		public string ClassAlias
		{
			get { return _classAlias; }
		}

		public string ClassName
		{
			get { return _className; }
		}

		public FromClause FromClause
		{
			get { return _fromClause; }
		}

		public IQueryable Queryable
		{
			get { return _elementType.Queryable; }
		}

		public IQueryableCollection QueryableCollection
		{
			get { return _elementType.QueryableCollection; }
			set { _elementType.QueryableCollection = value; }
		}

		public string CollectionTableAlias
		{
			get { return _collectionTableAlias; }
			set { _collectionTableAlias = value; }
		}

		public bool CollectionJoin
		{
			get { return _collectionJoin; }
			set { _collectionJoin = value; }
		}

		public string CollectionSuffix
		{
			get { return _elementType.CollectionSuffix; }
			set { _elementType.CollectionSuffix = value; }
		}

		public IType SelectType
		{
			get { return _elementType.SelectType; }
		}

		public bool IsCollectionOfValuesOrComponents
		{
			get { return _elementType.IsCollectionOfValuesOrComponents; }
		}

		public bool IsCollectionJoin
		{
			get { return _collectionJoin; }
		}

		public void SetRole(string role)
		{
			_role = role;
		}

		public FromElement Origin
		{
			get { return _origin; }
		}

		public FromElement RealOrigin
		{
			get
			{
				if (_origin == null)
				{
					return null;
				}
				if (String.IsNullOrEmpty(_origin.Text))
				{
					return _origin.RealOrigin;
				}
				return _origin;
			}
		}

		public SqlString WithClauseFragment
		{
			get { return _withClauseFragment; }
		}

		public string WithClauseJoinAlias
		{
			get { return _withClauseJoinAlias; }
		}

		/// <summary>
		/// Returns the identifier select SQL fragment.
		/// </summary>
		/// <param name="size">The total number of returned types.</param>
		/// <param name="k">The sequence of the current returned type.</param>
		/// <returns>the identifier select SQL fragment.</returns>
		public string RenderIdentifierSelect(int size, int k)
		{
			return _elementType.RenderIdentifierSelect(size, k);
		}

		/// <summary>
		/// Returns the property select SQL fragment.
		/// </summary>
		/// <param name="size">The total number of returned types.</param>
		/// <param name="k">The sequence of the current returned type.</param>
		/// <returns>the property select SQL fragment.</returns>
		public string RenderPropertySelect(int size, int k)
		{
			return _elementType.RenderPropertySelect(size, k, IsAllPropertyFetch);
		}

		public override SqlString RenderText(Engine.ISessionFactoryImplementor sessionFactory)
		{
			var result = SqlString.Parse(Text);
			// query-parameter = the parameter specified in the NHibernate query
			// sql-parameter = real parameter/s inside the final SQL
			// here is where we suppose the SqlString has all sql-parameters in sequence for a given query-parameter.
			// This happen when the query-parameter spans multiple columns (components,custom-types and so on).
			if (HasEmbeddedParameters)
			{
				var parameters = result.GetParameters().ToArray();
				var sqlParameterPos = 0;
				var paramTrackers = _embeddedParameters.SelectMany(specification => specification.GetIdsForBackTrack(sessionFactory));
				foreach (var paramTracker in paramTrackers)
				{
					parameters[sqlParameterPos++].BackTrack = paramTracker;
				}
			}
			return result;
		}

		public string RenderCollectionSelectFragment(int size, int k)
		{
			return _elementType.RenderCollectionSelectFragment(size, k);
		}

		public string RenderValueCollectionSelectFragment(int size, int k)
		{
			return _elementType.RenderValueCollectionSelectFragment(size, k);
		}

		public void SetIndexCollectionSelectorParamSpec(IParameterSpecification indexCollectionSelectorParamSpec)
		{
			if (indexCollectionSelectorParamSpec == null)
			{
				if (_elementType.IndexCollectionSelectorParamSpec != null)
				{
					_embeddedParameters.Remove(_elementType.IndexCollectionSelectorParamSpec);
					_elementType.IndexCollectionSelectorParamSpec = null;
				}
			}
			else
			{
				_elementType.IndexCollectionSelectorParamSpec = indexCollectionSelectorParamSpec;
				AddEmbeddedParameter(indexCollectionSelectorParamSpec);
			}
		}

		public virtual void SetImpliedInFromClause(bool flag)
		{
			throw new InvalidOperationException("Explicit FROM elements can't be implied in the FROM clause!");
		}

		public virtual bool IncludeSubclasses
		{
			get { return _includeSubclasses; }
			set
			{
				if (IsDereferencedBySuperclassOrSubclassProperty)
				{
					if (!_includeSubclasses && Log.IsInfoEnabled)
					{
						Log.Info("attempt to disable subclass-inclusions", new Exception("stack-trace source"));
					}
				}
				_includeSubclasses = value;
			}
		}

		public virtual bool InProjectionList
		{
			get { return !IsImplied && IsFromOrJoinFragment; }
			set 
			{ 
				// Do nothing, eplicit from elements are *always* in the projection list. 
			}
		}

		private bool _fetch;
		public bool Fetch
		{
			get { return _fetch; }
			set
			{
				_fetch = value;
				// Fetch can't be used with scroll() or iterate().
				if (_fetch && Walker.IsShallowQuery)
				{
					throw new QueryException(LiteralProcessor.ErrorCannotFetchWithIterate);
				}
			}
		}

		/// <summary>
		/// Render the identifier select, but in a 'scalar' context (i.e. generate the column alias).
		/// </summary>
		/// <param name="i">the sequence of the returned type</param>
		/// <returns>the identifier select with the column alias.</returns>
		public string RenderScalarIdentifierSelect(int i)
		{
			return _elementType.RenderScalarIdentifierSelect(i);
		}

		public bool UseFromFragment
		{
			get
			{
				CheckInitialized();
				// If it's not implied or it is implied and it's a many to many join where the target wasn't found.
				return !IsImplied || _useFromFragment;
			}
			set { _useFromFragment = value; }
		}

		public bool UseWhereFragment
		{
			get { return _useWhereFragment;}
			set { _useWhereFragment = value; }
		}

		public string[] ToColumns(string tableAlias, string path, bool inSelect)
		{
			return _elementType.ToColumns(tableAlias, path, inSelect);
		}

		public string[] ToColumns(string tableAlias, string path, bool inSelect, bool forceAlias)
		{
			return _elementType.ToColumns(tableAlias, path, inSelect, forceAlias);
		}

		public IPropertyMapping GetPropertyMapping(string propertyName)
		{
			return _elementType.GetPropertyMapping(propertyName);
		}

		public IType GetPropertyType(string propertyName, string propertyPath)
		{
			return _elementType.GetPropertyType(propertyName, propertyPath);
		}

		public virtual string GetIdentityColumn()
		{
			CheckInitialized();
			string table = TableAlias;

			if (table == null)
			{
				throw new InvalidOperationException("No table alias for node " + this);
			}
			string[] cols;
			string propertyName;
			if (EntityPersister != null && EntityPersister.EntityMetamodel != null
					&& EntityPersister.EntityMetamodel.HasNonIdentifierPropertyNamedId)
			{
				propertyName = EntityPersister.IdentifierPropertyName;
			}
			else if (IsCollectionOfValuesOrComponents)
			{
				propertyName = CollectionPropertyNames.Elements;
			}
			else
			{
				propertyName = NHibernate.Persister.Entity.EntityPersister.EntityID;
			}
			if (Walker.StatementType == HqlSqlWalker.SELECT)
			{
				cols = GetPropertyMapping(propertyName).ToColumns(table, propertyName);
			}
			else
			{
				cols = GetPropertyMapping(propertyName).ToColumns(propertyName);
			}
			string result = StringHelper.Join(", ", cols);

			// There used to be code here that added parentheses if the number of columns was greater than one.
			// This was causing invalid queries like select (c1, c2) from x.  I couldn't think of a reason that
			// parentheses would be wanted around a list of columns, so I removed them.
			return result;
		}

		public void HandlePropertyBeingDereferenced(IType propertySource, string propertyName)
		{
			if (QueryableCollection != null && CollectionProperties.IsCollectionProperty(propertyName))
			{
				// propertyName refers to something like collection.size...
				return;
			}
			if (propertySource.IsComponentType)
			{
				// property name is a sub-path of a component...
				return;
			}

			IQueryable persister = Queryable;

			if (persister != null)
			{
				try
				{
					Declarer propertyDeclarer = persister.GetSubclassPropertyDeclarer(propertyName);

					if (Log.IsInfoEnabled)
					{
						Log.Info("handling property dereference [" + persister.EntityName + " (" + ClassAlias + ") -> " + propertyName + " (" + propertyDeclarer + ")]");
					}
					if (propertyDeclarer == Declarer.SubClass)
					{
						_dereferencedBySubclassProperty = true;
						_includeSubclasses = true;
					}
					else if (propertyDeclarer == Declarer.SuperClass)
					{
						_dereferencedBySuperclassProperty = true;
					}
				}
				catch (QueryException)
				{
					// ignore it; the incoming property could not be found so we
					// cannot be sure what to do here.  At the very least, the
					// safest is to simply not apply any dereference toggling...
				}
			}
		}

		public void SetOrigin(FromElement origin, bool manyToMany)
		{
			_origin = origin;
			origin.AddDestination(this);

			if (origin.FromClause == FromClause)
			{
				// TODO: Figure out a better way to get the FROM elements in a proper tree structure.
				// If this is not the destination of a many-to-many, add it as a child of the origin.
				if (manyToMany)
				{
					origin.AddSibling(this);
				}
				else
				{
					if (!Walker.IsInFrom && !Walker.IsInSelect)
					{
						FromClause.AddChild(this);
					}
					else
					{
						origin.AddChild(this);
					}
				}
			}
			else if (!Walker.IsInFrom)
			{
				// HHH-276 : implied joins in a subselect where clause - The destination needs to be added
				// to the destination's from clause.
				FromClause.AddChild(this);	// Not sure if this is will fix everything, but it works.
			}
			else
			{
				// Otherwise, the destination node was implied by the FROM clause and the FROM clause processor
				// will automatically add it in the right place.
			}
		}

		public void SetIncludeSubclasses(bool includeSubclasses)
		{
			if (IsDereferencedBySuperclassOrSubclassProperty)
			{
				if (!includeSubclasses && Log.IsInfoEnabled)
				{
					Log.Info("attempt to disable subclass-inclusions", new Exception("stack-trace source"));
				}
			}
			_includeSubclasses = includeSubclasses;
		}

		public virtual string GetDisplayText()
		{
			StringBuilder buf = new StringBuilder();
			buf.Append("FromElement{");
			AppendDisplayText(buf);
			buf.Append("}");
			return buf.ToString();
		}

		public void InitializeCollection(FromClause fromClause, string classAlias, string tableAlias)
		{
			DoInitialize(fromClause, tableAlias, null, classAlias, null, null);
			_initialized = true;
		}

		public void InitializeEntity(FromClause fromClause,
									string className,
									IEntityPersister persister,
									EntityType type,
									string classAlias,
									string tableAlias)
		{
			DoInitialize(fromClause, tableAlias, className, classAlias, persister, type);
			_initialized = true;
		}

		public void CheckInitialized()
		{
			if (!_initialized)
			{
				throw new InvalidOperationException("FromElement has not been initialized!");
			}
		}

		protected void AppendDisplayText(StringBuilder buf)
		{
			buf.Append(IsImplied ? (
					IsImpliedInFromClause ? "implied in FROM clause" : "implied")
					: "explicit");
			buf.Append(",").Append(IsCollectionJoin ? "collection join" : "not a collection join");
			buf.Append(",").Append(_fetch ? "fetch join" : "not a fetch join");
			buf.Append(",").Append(IsAllPropertyFetch ? "fetch all properties" : "fetch non-lazy properties");
			buf.Append(",classAlias=").Append(ClassAlias);
			buf.Append(",role=").Append(_role);
			buf.Append(",tableName=").Append(TableName);
			buf.Append(",tableAlias=").Append(TableAlias);
			FromElement origin = RealOrigin;
			buf.Append(",origin=").Append(origin == null ? "null" : origin.Text);
			buf.Append(",colums={");
			if (_columns != null)
			{
				for (int i = 0; i < _columns.Length; i++)
				{
					buf.Append(_columns[i]);
					if (i < _columns.Length)
					{
						buf.Append(" ");
					}
				}
			}
			buf.Append(",className=").Append(_className);
			buf.Append("}");
		}

		private void AddDestination(FromElement fromElement)
		{
			_destinations.Add(fromElement);
		}

		private void DoInitialize(FromClause fromClause, string tableAlias, string className, string classAlias,
								  IEntityPersister persister, EntityType type)
		{
			if (_initialized)
			{
				throw new InvalidOperationException("Already initialized!!");
			}
			_fromClause = fromClause;
			_tableAlias = tableAlias;
			_className = className;
			_classAlias = classAlias;
			_elementType = new FromElementType(this, persister, type);

			// Register the FromElement with the FROM clause, now that we have the names and aliases.
			fromClause.RegisterFromElement(this);

			if (Log.IsDebugEnabled)
			{
				Log.Debug(fromClause + " :  " + className + " ("
						+ (classAlias ?? "no alias") + ") -> " + tableAlias);
			}
		}

		// ParameterContainer impl ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		private List<IParameterSpecification> _embeddedParameters;

		public void AddEmbeddedParameter(IParameterSpecification specification)
		{
			if (_embeddedParameters == null)
			{
				_embeddedParameters = new List<IParameterSpecification>();
			}
			_embeddedParameters.Add(specification);
		}

	}
}
