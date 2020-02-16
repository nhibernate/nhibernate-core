using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Persister.Collection;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Loader
{
	public class JoinWalker
	{
		private readonly ISessionFactoryImplementor factory;
		protected readonly IList<OuterJoinableAssociation> associations = new List<OuterJoinableAssociation>();
		private readonly HashSet<AssociationKey> visitedAssociationKeys = new HashSet<AssociationKey>();
		private readonly IDictionary<string, IFilter> enabledFilters;
		private readonly IDictionary<string, IFilter> enabledFiltersForManyToOne;
		private static readonly Regex aliasRegex = new Regex(@"[\w]+(?=\.)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		private string[] suffixes;
		private string[] collectionSuffixes;
		private ILoadable[] persisters;
		private int[] owners;
		private EntityType[] ownerAssociationTypes;
		private ICollectionPersister[] collectionPersisters;
		private int[] collectionOwners;
		private string[] aliases;
		private LockMode[] lockModeArray;
		private SqlString sql;

		public string[] CollectionSuffixes
		{
			get { return collectionSuffixes; }
			set { collectionSuffixes = value; }
		}

		public LockMode[] LockModeArray
		{
			get { return lockModeArray; }
			set { lockModeArray = value; }
		}

		public string[] Suffixes
		{
			get { return suffixes; }
			set { suffixes = value; }
		}

		public string[] Aliases
		{
			get { return aliases; }
			set { aliases = value; }
		}

		public bool[] EagerPropertyFetches { get; set; }
		public bool[] ChildFetchEntities { get; set; }

		public int[] CollectionOwners
		{
			get { return collectionOwners; }
			set { collectionOwners = value; }
		}

		public ICollectionPersister[] CollectionPersisters
		{
			get { return collectionPersisters; }
			set { collectionPersisters = value; }
		}

		public EntityType[] OwnerAssociationTypes
		{
			get { return ownerAssociationTypes; }
			set { ownerAssociationTypes = value; }
		}

		public int[] Owners
		{
			get { return owners; }
			set { owners = value; }
		}

		public ILoadable[] Persisters
		{
			get { return persisters; }
			set { persisters = value; }
		}

		public SqlString SqlString
		{
			get { return sql; }
			set { sql = value; }
		}

		protected ISessionFactoryImplementor Factory
		{
			get { return factory; }
		}

		protected Dialect.Dialect Dialect
		{
			get { return factory.Dialect; }
		}

		protected IDictionary<string, IFilter> EnabledFilters
		{
			get { return enabledFilters; }
		}

		protected virtual bool IsTooManyCollections
		{
			get { return false; }
		}

		//Since v5.3
		[Obsolete("This class is not used and will be removed in a future version.")]
		public class DependentAlias
		{
			public string Alias { get; set; }
			public string[] DependsOn { get; set; }
		}

		protected JoinWalker(ISessionFactoryImplementor factory, IDictionary<string, IFilter> enabledFilters)
		{
			this.factory = factory;
			this.enabledFilters = enabledFilters;
			enabledFiltersForManyToOne = FilterHelper.GetEnabledForManyToOne(enabledFilters);
		}

		/// <summary>
		/// Add on association (one-to-one, many-to-one, or a collection) to a list
		/// of associations to be fetched by outerjoin (if necessary)
		/// </summary>
		private void AddAssociationToJoinTreeIfNecessary(IAssociationType type, string[] aliasedLhsColumns,
			string alias, string path, string pathAlias, int currentDepth, JoinType joinType)
		{
			if (joinType >= JoinType.InnerJoin)
			{
				AddAssociationToJoinTree(type, aliasedLhsColumns, alias, path, pathAlias, currentDepth, joinType);
			}
		}

		// Since v5.2
		[Obsolete("Use or override the overload taking a pathAlias additional parameter")]
		protected virtual SqlString GetWithClause(string path)
		{
			return SqlString.Empty;
		}

		protected virtual SqlString GetWithClause(string path, string pathAlias)
		{
			// 6.0 TODO: inline the call
#pragma warning disable 618
			return GetWithClause(path);
#pragma warning restore 618
		}

		/// <summary>
		/// Add on association (one-to-one, many-to-one, or a collection) to a list
		/// of associations to be fetched by outerjoin
		/// </summary>
		private void AddAssociationToJoinTree(IAssociationType type, string[] aliasedLhsColumns, string alias,
			string path, string pathAlias, int currentDepth, JoinType joinType)
		{
			IJoinable joinable = type.GetAssociatedJoinable(Factory);

			string subalias = GenerateTableAlias(associations.Count + 1, path, pathAlias, joinable);
			var qc = joinable.IsCollection ? (IQueryableCollection) joinable : null;

			var assoc =
				new OuterJoinableAssociation(
					type,
					alias,
					aliasedLhsColumns,
					subalias,
					joinType,
					//for many-to-many with clause is applied with OuterJoinableAssociation created for entity persister so simply skip it here
					qc?.IsManyToMany == true ? null : GetWithClause(path, pathAlias),
					Factory,
					enabledFilters,
					GetSelectMode(path));
			assoc.ValidateJoin(path);
			AddAssociation(assoc);

			int nextDepth = currentDepth + 1;

			if (qc == null)
			{
				IOuterJoinLoadable pjl = joinable as IOuterJoinLoadable;
				if (pjl != null)
					WalkEntityTree(pjl, subalias, path, nextDepth);
			}
			else
			{
				WalkCollectionTree(qc, subalias, path, pathAlias, nextDepth);
			}
		}

		protected  virtual SelectMode GetSelectMode(string path)
		{
			return SelectMode.Undefined;
		}

		private struct DependentAlias2
		{
			public DependentAlias2(string alias, ICollection<string> dependsOn)
			{
				Alias = alias;
				DependsOn = dependsOn;
			}

			public string Alias { get; }
			public ICollection<string> DependsOn { get; }
		}

		/// <summary>
		/// Returns list of indexes in sorted order
		/// </summary>
		private static int[] GetTopologicalSortOrder(IList<DependentAlias2> fields)
		{
			TopologicalSorter g = new TopologicalSorter(fields.Count);
			Dictionary<string, int> indexes = new Dictionary<string, int>(fields.Count, StringComparer.OrdinalIgnoreCase);

			// add vertices
			for (int i = 0; i < fields.Count; i++)
			{
				indexes[fields[i].Alias] = g.AddVertex(i);
			}

			// add edges
			for (int i = 0; i < fields.Count; i++)
			{
				var dependentFields = fields[i].DependsOn;
				if (dependentFields != null)
				{
					foreach (var dependentField in dependentFields)
					{
						if (indexes.TryGetValue(dependentField, out var end))
						{
							g.AddEdge(i, end);
						}
					}
				}
			}

			return g.Sort();
		}

		private static List<DependentAlias2> GetDependentAliases(IList<OuterJoinableAssociation> associations)
		{
			var dependentAliases = new List<DependentAlias2>(associations.Count);
			foreach (var association in associations)
			{
				dependentAliases.Add(new DependentAlias2(association.RHSAlias, GetDependsOn(association)));
			}

			return dependentAliases;
		}

		private static HashSet<string> GetDependsOn(OuterJoinableAssociation association)
		{
			if (SqlStringHelper.IsEmpty(association.On))
				return null;

			var dependencies = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			foreach (Match match in aliasRegex.Matches(association.On.ToString()))
			{
				string alias = match.Value;
				if (string.Equals(alias, association.RHSAlias, StringComparison.OrdinalIgnoreCase))
					continue;

				dependencies.Add(alias);
			}

			return dependencies;
		}

		/// <summary>
		/// Adds an association
		/// </summary>
		private void AddAssociation(OuterJoinableAssociation association)
		{
			associations.Add(association);
		}

		/// <summary>
		/// For an entity class, return a list of associations to be fetched by outerjoin
		/// </summary>
		protected void WalkEntityTree(IOuterJoinLoadable persister, string alias)
		{
			WalkEntityTree(persister, alias, string.Empty, 0);
		}

		/// <summary>
		/// For a collection role, return a list of associations to be fetched by outerjoin
		/// </summary>
		protected void WalkCollectionTree(IQueryableCollection persister, string alias)
		{
			WalkCollectionTree(persister, alias, string.Empty, string.Empty, 0);
			//TODO: when this is the entry point, we should use an INNER_JOIN for fetching the many-to-many elements!
		}

		/// <summary>
		/// For a collection role, return a list of associations to be fetched by outerjoin
		/// </summary>
		private void WalkCollectionTree(IQueryableCollection persister, string alias, string path, string pathAlias, int currentDepth)
		{
			if (persister.IsOneToMany)
			{
				WalkEntityTree((IOuterJoinLoadable)persister.ElementPersister, alias, path, currentDepth);
			}
			else
			{
				IType type = persister.ElementType;
				if (type.IsAssociationType)
				{
					// a many-to-many
					// decrement currentDepth here to allow join across the association table
					// without exceeding MAX_FETCH_DEPTH (i.e. the "currentDepth - 1" bit)
					IAssociationType associationType = (IAssociationType) type;
					string[] aliasedLhsColumns = persister.GetElementColumnNames(alias);
					string[] lhsColumns = persister.ElementColumnNames;

					// if the current depth is 0, the root thing being loaded is the
					// many-to-many collection itself.  Here, it is alright to use
					// an inner join...
					bool useInnerJoin = currentDepth == 0;

					var joinType =
						GetJoinType(
							associationType,
							persister.FetchMode,
							path,
							pathAlias,
							persister.TableName,
							lhsColumns,
							!useInnerJoin,
							currentDepth - 1,
							null);

					AddAssociationToJoinTreeIfNecessary(
						associationType,
						aliasedLhsColumns,
						alias,
						path,
						pathAlias,
						currentDepth - 1,
						joinType);
				}
				else if (type.IsComponentType)
				{
					WalkCompositeElementTree(
						(IAbstractComponentType) type,
						persister.ElementColumnNames,
						persister,
						alias,
						path,
						currentDepth);
				}
			}
		}

		internal void AddExplicitEntityJoinAssociation(
			IOuterJoinLoadable persister,
			string tableAlias,
			JoinType joinType,
			string path,
			string pathAlias)
		{
			OuterJoinableAssociation assoc =
				new OuterJoinableAssociation(
					persister.EntityType,
					string.Empty,
					Array.Empty<string>(),
					tableAlias,
					joinType,
					GetWithClause(path, pathAlias),
					Factory,
					enabledFilters,
					GetSelectMode(path));
			AddAssociation(assoc);
		}

		private void WalkEntityAssociationTree(IAssociationType associationType, IOuterJoinLoadable persister,
											   int propertyNumber, string alias, string path, bool nullable, int currentDepth,
											   ILhsAssociationTypeSqlInfo associationTypeSQLInfo)
		{
			string[] aliasedLhsColumns = associationTypeSQLInfo.GetAliasedColumnNames(associationType, 0);
			string[] lhsColumns = associationTypeSQLInfo.GetColumnNames(associationType, 0);
			string lhsTable = associationTypeSQLInfo.GetTableName(associationType);

			string subpath = SubPath(path, persister.GetSubclassPropertyName(propertyNumber));

			// Obtain children aliases for the current path and alias
			var subPathAliases = GetChildAliases(alias, subpath);
			foreach (var subPathAlias in subPathAliases)
			{
				var joinType = GetJoinType(
					associationType,
					persister.GetFetchMode(propertyNumber),
					subpath,
					subPathAlias,
					lhsTable,
					lhsColumns,
					nullable,
					currentDepth,
					persister.GetCascadeStyle(propertyNumber));

				AddAssociationToJoinTreeIfNecessary(
					associationType,
					aliasedLhsColumns,
					alias,
					subpath,
					subPathAlias,
					currentDepth,
					joinType);
			}
		}

		/// <summary>
		/// For an entity class, add to a list of associations to be fetched
		/// by outerjoin
		/// </summary>
		protected virtual void WalkEntityTree(IOuterJoinLoadable persister, string alias, string path, int currentDepth)
		{
			int n = persister.CountSubclassProperties();
			for (int i = 0; i < n; i++)
			{
				IType type = persister.GetSubclassPropertyType(i);
				ILhsAssociationTypeSqlInfo associationTypeSQLInfo = JoinHelper.GetLhsSqlInfo(alias, i, persister, Factory);
				if (type.IsAssociationType)
				{
					WalkEntityAssociationTree((IAssociationType) type, persister, i, alias, path,
											  persister.IsSubclassPropertyNullable(i), currentDepth, associationTypeSQLInfo);
				}
				else if (type.IsComponentType)
				{
					WalkComponentTree((IAbstractComponentType) type, 0, alias, SubPath(path, persister.GetSubclassPropertyName(i)),
									  currentDepth, associationTypeSQLInfo);
				}
			}
		}

		/// <summary>
		/// For a component, add to a list of associations to be fetched by outerjoin
		/// </summary>
		protected void WalkComponentTree(IAbstractComponentType componentType, int begin, string alias, string path,
										 int currentDepth, ILhsAssociationTypeSqlInfo associationTypeSQLInfo)
		{
			IType[] types = componentType.Subtypes;
			string[] propertyNames = componentType.PropertyNames;
			for (int i = 0; i < types.Length; i++)
			{
				if (types[i].IsAssociationType)
				{
					var associationType = (IAssociationType) types[i];

					string[] aliasedLhsColumns = associationTypeSQLInfo.GetAliasedColumnNames(associationType, begin);
					string[] lhsColumns = associationTypeSQLInfo.GetColumnNames(associationType, begin);
					string lhsTable = associationTypeSQLInfo.GetTableName(associationType);

					string subpath = SubPath(path, propertyNames[i]);
					bool[] propertyNullability = componentType.PropertyNullability;

					// Obtain related aliases to the current path
					var subPathAliases = GetChildAliases(alias, subpath);
					foreach (var subPathAlias in subPathAliases)
					{
						var joinType = GetJoinType(
							associationType,
							componentType.GetFetchMode(i),
							subpath,
							subPathAlias,
							lhsTable,
							lhsColumns,
							propertyNullability == null || propertyNullability[i],
							currentDepth,
							componentType.GetCascadeStyle(i));

						AddAssociationToJoinTreeIfNecessary(
							associationType,
							aliasedLhsColumns,
							alias,
							subpath,
							subPathAlias,
							currentDepth,
							joinType);
					}
				}
				else if (types[i].IsComponentType)
				{
					string subpath = SubPath(path, propertyNames[i]);

					WalkComponentTree((IAbstractComponentType) types[i], begin, alias, subpath, currentDepth, associationTypeSQLInfo);
				}
				begin += types[i].GetColumnSpan(Factory);
			}
		}

		/// <summary>
		/// For a composite element, add to a list of associations to be fetched by outerjoin
		/// </summary>
		private void WalkCompositeElementTree(IAbstractComponentType compositeType, string[] cols,
			IQueryableCollection persister, string alias, string path, int currentDepth)
		{
			IType[] types = compositeType.Subtypes;
			string[] propertyNames = compositeType.PropertyNames;
			int begin = 0;
			for (int i = 0; i < types.Length; i++)
			{
				int length = types[i].GetColumnSpan(factory);
				string[] lhsColumns = ArrayHelper.Slice(cols, begin, length);

				if (types[i].IsAssociationType)
				{
					IAssociationType associationType = types[i] as IAssociationType;

					// simple, because we can't have a one-to-one or collection
					// (or even a property-ref) in a composite element:
					string[] aliasedLhsColumns = StringHelper.Qualify(alias, lhsColumns);
					string subpath = SubPath(path, propertyNames[i]);
					bool[] propertyNullability = compositeType.PropertyNullability;

					var subPathAliases = GetChildAliases(alias, subpath);
					foreach (var subPathAlias in subPathAliases)
					{
						var joinType =
							GetJoinType(
								associationType,
								compositeType.GetFetchMode(i),
								subpath,
								subPathAlias,
								persister.TableName,
								lhsColumns,
								propertyNullability == null || propertyNullability[i],
								currentDepth,
								compositeType.GetCascadeStyle(i));

						AddAssociationToJoinTreeIfNecessary(
							associationType,
							aliasedLhsColumns,
							alias,
							subpath,
							subPathAlias,
							currentDepth,
							joinType);
					}
				}
				else if (types[i].IsComponentType)
				{
					string subpath = SubPath(path, propertyNames[i]);
					WalkCompositeElementTree(
						(IAbstractComponentType) types[i],
						lhsColumns,
						persister,
						alias,
						subpath,
						currentDepth);
				}
				begin += length;
			}
		}

		/// <summary>
		/// Extend the path by the given property name
		/// </summary>
		protected static string SubPath(string path, string property)
		{
			if (string.IsNullOrEmpty(property))
				return path;
			return string.IsNullOrEmpty(path) ? property : StringHelper.Qualify(path, property);
		}

		/// <summary>
		/// Get the join type (inner, outer, etc) or -1 if the
		/// association should not be joined. Override on
		/// subclasses.
		/// </summary>
		// Since v5.2
		[Obsolete("Use or override the overload taking a pathAlias additional parameter")]
		protected virtual JoinType GetJoinType(IAssociationType type, FetchMode config, string path, string lhsTable,
			string[] lhsColumns, bool nullable, int currentDepth, CascadeStyle cascadeStyle)
		{
			if (!IsJoinedFetchEnabled(type, config, cascadeStyle))
				return JoinType.None;

			if (IsTooDeep(currentDepth) || (type.IsCollectionType && IsTooManyCollections))
				return JoinType.None;

			bool dupe = IsDuplicateAssociation(lhsTable, lhsColumns, type);
			if (dupe)
				return JoinType.None;

			return GetJoinType(nullable, currentDepth);
		}

		/// <summary>
		/// Get the join type (inner, outer, etc) or -1 if the
		/// association should not be joined. Override on
		/// subclasses.
		/// </summary>
		protected virtual JoinType GetJoinType(IAssociationType type, FetchMode config, string path, string pathAlias,
			string lhsTable, string[] lhsColumns, bool nullable, int currentDepth, CascadeStyle cascadeStyle)
		{
			// 6.0 TODO: inline the call
#pragma warning disable 618
			return GetJoinType(type, config, path, lhsTable, lhsColumns, nullable, currentDepth, cascadeStyle);
#pragma warning restore 618
		}

		// By default, multiple aliases for a child are not supported. There is only one and its value
		// does not matter for default implementation.
		private static readonly IReadOnlyCollection<string> DefaultChildAliases = new[] { string.Empty };

		/// <summary>
		/// Returns the child criteria aliases for a parent SQL alias and a child path.
		/// </summary>
		protected virtual IReadOnlyCollection<string> GetChildAliases(string parentSqlAlias, string childPath)
		{
			return DefaultChildAliases;
		}

		/// <summary>
		/// Use an inner join if it is a non-null association and this
		/// is the "first" join in a series
		/// </summary>
		protected JoinType GetJoinType(bool nullable, int currentDepth)
		{
			//TODO: this is too conservative; if all preceding joins were 
			//      also inner joins, we could use an inner join here
			return !nullable && currentDepth == 0 ? JoinType.InnerJoin : JoinType.LeftOuterJoin;
		}

		protected virtual bool IsTooDeep(int currentDepth)
		{
			int maxFetchDepth = Factory.Settings.MaximumFetchDepth;
			return maxFetchDepth >= 0 && currentDepth >= maxFetchDepth;
		}

		/// <summary>
		/// Does the mapping, and Hibernate default semantics, specify that
		/// this association should be fetched by outer joining
		/// </summary>
		protected bool IsJoinedFetchEnabledInMapping(FetchMode config, IAssociationType type)
		{
			if (!type.IsEntityType && !type.IsCollectionType)
			{
				return false;
			}
			else
			{
				switch (config)
				{
					case FetchMode.Join:
						return true;

					case FetchMode.Select:
						return false;

					case FetchMode.Default:
						if (type.IsEntityType)
						{
							//TODO: look at the owning property and check that it 
							//      isn't lazy (by instrumentation)
							EntityType entityType = (EntityType)type;
							IEntityPersister persister = factory.GetEntityPersister(entityType.GetAssociatedEntityName());
							return !persister.HasProxy;
						}
						else
						{
							return false;
						}

					default:
						throw new ArgumentOutOfRangeException("config", config, "Unknown OJ strategy " + config);
				}
			}
		}

		/// <summary>
		/// Override on subclasses to enable or suppress joining
		/// of certain association types
		/// </summary>
		protected virtual bool IsJoinedFetchEnabled(IAssociationType type, FetchMode config,
																								CascadeStyle cascadeStyle)
		{
			return type.IsEntityType && IsJoinedFetchEnabledInMapping(config, type);
		}

		// Since v5.2
		[Obsolete("Use or override the overload taking a pathAlias additional parameter")]
		protected virtual string GenerateTableAlias(int n, string path, IJoinable joinable)
		{
			return StringHelper.GenerateAlias(joinable.Name, n);
		}

		protected virtual string GenerateTableAlias(int n, string path, string pathAlias, IJoinable joinable)
		{
			// 6.0 TODO: inline the call
#pragma warning disable 618
			return GenerateTableAlias(n, path, joinable);
#pragma warning restore 618
		}

		protected virtual string GenerateRootAlias(string description)
		{
			return StringHelper.GenerateAlias(description, 0);
		}

		/// <summary>
		/// Used to detect circularities in the joined graph, note that
		/// this method is side-effecty
		/// </summary>
		protected virtual bool IsDuplicateAssociation(string foreignKeyTable, string[] foreignKeyColumns)
		{
			AssociationKey associationKey = new AssociationKey(foreignKeyColumns, foreignKeyTable);
			return !visitedAssociationKeys.Add(associationKey);
		}

		/// <summary>
		/// Used to detect circularities in the joined graph, note that
		/// this method is side-effecty
		/// </summary>
		protected virtual bool IsDuplicateAssociation(string lhsTable, string[] lhsColumnNames, IAssociationType type)
		{
			string foreignKeyTable;
			string[] foreignKeyColumns;

			if (type.ForeignKeyDirection.Equals(ForeignKeyDirection.ForeignKeyFromParent))
			{
				foreignKeyTable = lhsTable;
				foreignKeyColumns = lhsColumnNames;
			}
			else
			{
				foreignKeyTable = type.GetAssociatedJoinable(Factory).TableName;
				foreignKeyColumns = JoinHelper.GetRHSColumnNames(type, Factory);
			}

			return IsDuplicateAssociation(foreignKeyTable, foreignKeyColumns);
		}

		/// <summary>
		/// Uniquely identifier a foreign key, so that we don't
		/// join it more than once, and create circularities
		/// </summary>
		protected sealed class AssociationKey
		{
			private readonly string[] columns;
			private readonly string table;
			private readonly int hashCode;

			public AssociationKey(string[] columns, string table)
			{
				this.columns = columns;
				this.table = table;
				hashCode = table.GetHashCode();
			}

			public override bool Equals(object other)
			{
				AssociationKey that = other as AssociationKey;
				if (that == null)
					return false;

				return that.table.Equals(table) && CollectionHelper.SequenceEquals<string>(columns, that.columns);
			}

			public override int GetHashCode()
			{
				return hashCode;
			}
		}

		/// <summary>
		/// Should we join this association?
		/// </summary>
		protected bool IsJoinable(JoinType joinType, ISet<AssociationKey> visitedAssociationKeys, string lhsTable,
			string[] lhsColumnNames, IAssociationType type, int depth)
		{
			if (joinType < JoinType.InnerJoin) return false;
			if (joinType == JoinType.InnerJoin) return true;

			int maxFetchDepth = Factory.Settings.MaximumFetchDepth;
			bool tooDeep = maxFetchDepth >= 0 && depth >= maxFetchDepth;

			return !tooDeep && !IsDuplicateAssociation(lhsTable, lhsColumnNames, type);
		}

		protected SqlString OrderBy(IList<OuterJoinableAssociation> associations, SqlString orderBy)
		{
			return MergeOrderings(OrderBy(associations), orderBy);
		}

		protected SqlString OrderBy(IList<OuterJoinableAssociation> associations, string orderBy)
		{
			return MergeOrderings(OrderBy(associations), new SqlString(orderBy));
		}

		protected SqlString MergeOrderings(SqlString ass, SqlString orderBy)
		{
			if (ass.Length == 0)
				return orderBy;
			else if (orderBy.Length == 0)
				return ass;
			else
				return ass.Append(StringHelper.CommaSpace, orderBy);
		}

		protected SqlString MergeOrderings(string ass, SqlString orderBy) {
			return this.MergeOrderings(new SqlString(ass), orderBy);
		}

		protected SqlString MergeOrderings(string ass, string orderBy) {
			return this.MergeOrderings(new SqlString(ass), new SqlString(orderBy));
		}

		/// <summary>
		/// Generate a sequence of <c>LEFT OUTER JOIN</c> clauses for the given associations.
		/// </summary>
		protected JoinFragment MergeOuterJoins(IList<OuterJoinableAssociation> associations)
		{
			JoinFragment outerjoin = Dialect.CreateOuterJoinFragment();

			var sortedAssociations = GetSortedAssociations(associations);
			OuterJoinableAssociation last = null;
			foreach (OuterJoinableAssociation oj in sortedAssociations)
			{
				if (last != null && last.IsManyToManyWith(oj))
				{
					oj.AddManyToManyJoin(outerjoin, (IQueryableCollection) last.Joinable);
				}
				else
				{
					oj.AddJoins(outerjoin);
					// NH Different behavior : NH1179 and NH1293
					// Apply filters in Many-To-One association
					if (enabledFiltersForManyToOne.Count > 0)
					{
						string manyToOneFilterFragment = oj.Joinable.FilterFragment(oj.RHSAlias, enabledFiltersForManyToOne);
						bool joinClauseDoesNotContainsFilterAlready =
							outerjoin.ToFromFragmentString.IndexOfCaseInsensitive(manyToOneFilterFragment) == -1;
						if (joinClauseDoesNotContainsFilterAlready)
						{
							// Ensure that the join condition is added to the join, not the where clause.
							// Adding the condition to the where clause causes left joins to become inner joins.
							outerjoin.AddFromFragmentString(new SqlString(manyToOneFilterFragment));
						}
					}
				}
				last = oj;
			}

			return outerjoin;
		}

		private static IList<OuterJoinableAssociation> GetSortedAssociations(IList<OuterJoinableAssociation> associations)
		{
			if (associations.Count < 2)
				return associations;

			var fields = GetDependentAliases(associations);
			if (!fields.Exists(a => a.DependsOn?.Count > 0))
				return associations;

			var indexes = GetTopologicalSortOrder(fields);
			var sortedAssociations = new List<OuterJoinableAssociation>(associations.Count);
			for (int index = indexes.Length - 1; index >= 0; index--)
			{
				sortedAssociations.Add(associations[indexes[index]]);
			}

			return sortedAssociations;
		}

		/// <summary>
		/// Count the number of instances of IJoinable which are actually
		/// also instances of ILoadable, or are one-to-many associations
		/// </summary>
		protected static int CountEntityPersisters(IList<OuterJoinableAssociation> associations)
		{
			int result = 0;
			foreach (OuterJoinableAssociation oj in associations)
			{
				if (oj.Joinable.ConsumesEntityAlias() && oj.SelectMode != SelectMode.JoinOnly)
					result++;
			}

			return result;
		}

		/// <summary>
		/// Count the number of instances of <see cref="IJoinable" /> which
		/// are actually also instances of <see cref="IPersistentCollection" />
		/// which are being fetched by outer join
		/// </summary>
		protected static int CountCollectionPersisters(IList<OuterJoinableAssociation> associations)
		{
			int result = 0;

			foreach (OuterJoinableAssociation oj in associations)
			{
				if (oj.ShouldFetchCollectionPersister())
					result++;
			}
			return result;
		}

		/// <summary>
		/// Get the order by string required for collection fetching
		/// </summary>
		protected SqlString OrderBy(IList<OuterJoinableAssociation> associations)
		{
			SqlStringBuilder buf = new SqlStringBuilder();

			OuterJoinableAssociation last = null;
			foreach (OuterJoinableAssociation oj in associations)
			{
				if (oj.ShouldFetchCollectionPersister())
				{
					IQueryableCollection queryableCollection = (IQueryableCollection) oj.Joinable;
					if (queryableCollection.HasOrdering)
					{
						string orderByString = queryableCollection.GetSQLOrderByString(oj.RHSAlias);
						buf.Add(orderByString).Add(StringHelper.CommaSpace);
					}
				}
				else if (!oj.IsCollection && last?.ShouldFetchCollectionPersister() == true)
				{
					// it might still need to apply a collection ordering based on a
					// many-to-many defined order-by...
					IQueryableCollection queryableCollection = (IQueryableCollection) last.Joinable;
					if (queryableCollection.IsManyToMany && last.IsManyToManyWith(oj))
					{
						if (queryableCollection.HasManyToManyOrdering)
						{
							string orderByString = queryableCollection.GetManyToManyOrderByString(oj.RHSAlias);
							buf.Add(orderByString).Add(StringHelper.CommaSpace);
						}
					}
				}
				last = oj;
			}

			if (buf.Count > 0) {
				buf.RemoveAt(buf.Count-1);
			}

			return buf.ToSqlString();
		}

		protected virtual string GenerateAliasForColumn(string rootAlias, string column)
		{
			return rootAlias;
		}

		/// <summary>
		/// Render the where condition for a (batch) load by identifier / collection key
		/// </summary>
		protected SqlStringBuilder WhereString(string alias, string[] columnNames, int batchSize)
		{
			if (columnNames.Length == 1)
			{
				// if not a composite key, use "foo in (?, ?, ?)" for batching
				// if no batch, and not a composite key, use "foo = ?"
				string tableAlias = GenerateAliasForColumn(alias, columnNames[0]);
				InFragment inf = new InFragment().SetColumn(tableAlias, columnNames[0]);

				for (int i = 0; i < batchSize; i++)
					inf.AddValue(Parameter.Placeholder);

				return new SqlStringBuilder(inf.ToFragmentString());
			}
			else
			{
				var fragments = new ConditionalFragment[batchSize];
				for (int i = 0; i < batchSize; i++)
				{
					fragments[i] = new ConditionalFragment()
						.SetTableAlias(alias)
						.SetCondition(columnNames, Parameter.GenerateParameters(columnNames.Length));
				}

				var whereString = new SqlStringBuilder();

				if (fragments.Length == 1)
				{
					// if no batch, use "foo = ? and bar = ?"
					whereString.Add(fragments[0].ToSqlStringFragment());
				}
				else
				{
					// if batching, use "( (foo = ? and bar = ?) or (foo = ? and bar = ?) )"
					var df = new DisjunctionFragment(fragments);

					whereString.Add(StringHelper.OpenParen);
					whereString.Add(df.ToFragmentString());
					whereString.Add(StringHelper.ClosedParen);
				}

				return whereString;
			}
		}

		protected void InitPersisters(IList<OuterJoinableAssociation> associations, LockMode lockMode)
		{
			int joins = CountEntityPersisters(associations);
			int collections = CountCollectionPersisters(associations);

			collectionOwners = collections == 0 ? null : new int[collections];
			collectionPersisters = collections == 0 ? null : new ICollectionPersister[collections];
			collectionSuffixes = BasicLoader.GenerateSuffixes(joins + 1, collections);

			persisters = new ILoadable[joins];
			EagerPropertyFetches = new bool[joins];
			ChildFetchEntities = new bool[joins];
			aliases = new String[joins];
			owners = new int[joins];
			ownerAssociationTypes = new EntityType[joins];
			lockModeArray = ArrayHelper.Fill(lockMode, joins);

			int i = 0;
			int j = 0;

			foreach (OuterJoinableAssociation oj in associations)
			{
				if (oj.SelectMode == SelectMode.JoinOnly)
					continue;

				if (!oj.IsCollection)
				{
					owners[i] = oj.GetOwner(associations);
					ownerAssociationTypes[i] = (EntityType) oj.JoinableType;

					FillEntityPersisterProperties(i, oj, (ILoadable) oj.Joinable);
					i++;
				}
				else
				{
					IQueryableCollection collPersister = (IQueryableCollection)oj.Joinable;
					if (oj.ShouldFetchCollectionPersister())
					{
						//it must be a collection fetch
						collectionPersisters[j] = collPersister;
						collectionOwners[j] = oj.GetOwner(associations);
						j++;
					}

					if (collPersister.IsOneToMany)
					{
						FillEntityPersisterProperties(i, oj, (ILoadable) collPersister.ElementPersister);
						i++;
					}
				}
			}

			if (ArrayHelper.IsAllNegative(owners))
				owners = null;

			if (collectionOwners != null && ArrayHelper.IsAllNegative(collectionOwners))
				collectionOwners = null;
		}

		private void FillEntityPersisterProperties(int i, OuterJoinableAssociation oj, ILoadable persister)
		{
			persisters[i] = persister;
			aliases[i] = oj.RHSAlias;
			EagerPropertyFetches[i] = oj.SelectMode == SelectMode.FetchLazyProperties;
			ChildFetchEntities[i] = oj.SelectMode == SelectMode.ChildFetch;
		}

		/// <summary>
		/// Generate a select list of columns containing all properties of the entity classes
		/// </summary>
		public string SelectString(IList<OuterJoinableAssociation> associations)
		{
			if (associations.Count == 0)
			{
				return string.Empty;
			}
			else
			{
				SqlStringBuilder buf = new SqlStringBuilder(associations.Count * 3);

				int entityAliasCount = 0;
				int collectionAliasCount = 0;

				for (int i = 0; i < associations.Count; i++)
				{
					OuterJoinableAssociation join = associations[i];
					OuterJoinableAssociation next = (i == associations.Count - 1) ? null : associations[i + 1];

					IJoinable joinable = join.Joinable;
					string entitySuffix = (suffixes == null || entityAliasCount >= suffixes.Length) ? null : suffixes[entityAliasCount];

					string collectionSuffix = (collectionSuffixes == null || collectionAliasCount >= collectionSuffixes.Length)
																			? null
																			: collectionSuffixes[collectionAliasCount];

					string selectFragment = 
						GetSelectFragment(join, entitySuffix, collectionSuffix, next);

					if (!string.IsNullOrWhiteSpace(selectFragment))
					{
						buf.Add(StringHelper.CommaSpace)
							.Add(selectFragment);
					}
					if (joinable.ConsumesEntityAlias() && join.SelectMode != SelectMode.JoinOnly)
						entityAliasCount++;

					if (joinable.ConsumesCollectionAlias() && join.ShouldFetchCollectionPersister())
						collectionAliasCount++;
				}

				return buf.ToSqlString().ToString();
			}
		}

		protected static string GetSelectFragment(OuterJoinableAssociation join, string entitySuffix, string collectionSuffix, OuterJoinableAssociation next = null)
		{
			switch (join.SelectMode)
			{
				case SelectMode.Undefined:
				case SelectMode.Fetch:
#pragma warning disable 618
					return join.Joinable.SelectFragment(
						next?.Joinable,
						next?.RHSAlias,
						join.RHSAlias,
						entitySuffix,
						collectionSuffix,
						join.ShouldFetchCollectionPersister());
#pragma warning restore 618

				case SelectMode.FetchLazyProperties:
					return ReflectHelper.CastOrThrow<ISupportSelectModeJoinable>(join.Joinable, "fetch lazy propertie")
						.SelectFragment(
							next?.Joinable,
							next?.RHSAlias,
							join.RHSAlias,
							entitySuffix,
							collectionSuffix,
							join.ShouldFetchCollectionPersister(),
							true);
				
				case SelectMode.ChildFetch:
					return ReflectHelper.CastOrThrow<ISupportSelectModeJoinable>(join.Joinable, "child fetch select mode").IdentifierSelectFragment(join.RHSAlias, entitySuffix);

				case SelectMode.JoinOnly:
					return string.Empty;
				default:
					throw new ArgumentOutOfRangeException(nameof(join.SelectMode), $"{join.SelectMode} is unexpected.");
			}
		}
	}
}
