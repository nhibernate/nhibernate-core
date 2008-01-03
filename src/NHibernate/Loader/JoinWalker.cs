using System;
using System.Collections;
using System.Text;
using Iesi.Collections;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Persister.Collection;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Util;
using System.Collections.Generic;

namespace NHibernate.Loader
{
	public class JoinWalker
	{
		private readonly ISessionFactoryImplementor factory;
		protected readonly IList associations = new ArrayList();
		private readonly ISet visitedAssociationKeys = new HashedSet();
		private readonly IDictionary<string, IFilter> enabledFilters;

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

		protected JoinWalker(ISessionFactoryImplementor factory, IDictionary<string, IFilter> enabledFilters)
		{
			this.factory = factory;
			this.enabledFilters = enabledFilters;
		}

		/// <summary>
		/// Add on association (one-to-one, many-to-one, or a collection) to a list
		/// of associations to be fetched by outerjoin (if necessary)
		/// </summary>
		private void AddAssociationToJoinTreeIfNecessary(
			IAssociationType type,
			string[] aliasedLhsColumns,
			string alias,
			string path,
			int currentDepth,
			JoinType joinType)
		{
			if (joinType >= JoinType.InnerJoin)
			{
				AddAssociationToJoinTree(
					type,
					aliasedLhsColumns,
					alias,
					path,
					currentDepth,
					joinType
					);
			}
		}

		/// <summary>
		/// Add on association (one-to-one, many-to-one, or a collection) to a list
		/// of associations to be fetched by outerjoin
		/// </summary>
		private void AddAssociationToJoinTree(
			IAssociationType type,
			string[] aliasedLhsColumns,
			string alias,
			string path,
			int currentDepth,
			JoinType joinType)
		{
			IJoinable joinable = type.GetAssociatedJoinable(Factory);

			string subalias = GenerateTableAlias(
				associations.Count + 1, //before adding to collection!
				path,
				joinable
				);

			OuterJoinableAssociation assoc = new OuterJoinableAssociation(
				type,
				alias,
				aliasedLhsColumns,
				subalias,
				joinType,
				Factory,
				enabledFilters
				);
			assoc.ValidateJoin(path);
			associations.Add(assoc);

			int nextDepth = currentDepth + 1;

			if (!joinable.IsCollection)
			{
				if (joinable is IOuterJoinLoadable)
				{
					WalkEntityTree(
						(IOuterJoinLoadable) joinable,
						subalias,
						path,
						nextDepth
						);
				}
			}
			else
			{
				if (joinable is IQueryableCollection)
				{
					WalkCollectionTree(
						(IQueryableCollection) joinable,
						subalias,
						path,
						nextDepth
						);
				}
			}
		}

		/// <summary>
		/// For an entity class, return a list of associations to be fetched by outerjoin
		/// </summary>
		protected void WalkEntityTree(IOuterJoinLoadable persister, string alias)
		{
			WalkEntityTree(persister, alias, "", 0);
		}

		/// <summary>
		/// For a collection role, return a list of associations to be fetched by outerjoin
		/// </summary>
		protected void WalkCollectionTree(IQueryableCollection persister, string alias)
		{
			WalkCollectionTree(persister, alias, "", 0);
			//TODO: when this is the entry point, we should use an INNER_JOIN for fetching the many-to-many elements!
		}

		/// <summary>
		/// For a collection role, return a list of associations to be fetched by outerjoin
		/// </summary>
		private void WalkCollectionTree(IQueryableCollection persister, string alias, string path, int currentDepth)
		{
			if (persister.IsOneToMany)
			{
				WalkEntityTree(
					(IOuterJoinLoadable) persister.ElementPersister,
					alias,
					path,
					currentDepth
					);
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

					JoinType joinType = GetJoinType(
						associationType,
						persister.FetchMode,
						path,
						persister.TableName,
						lhsColumns,
						!useInnerJoin,
						currentDepth - 1,
						null //operations which cascade as far as the collection also cascade to collection elements
						);

					AddAssociationToJoinTreeIfNecessary(
						associationType,
						aliasedLhsColumns,
						alias,
						path,
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
						currentDepth
						);
				}
			}
		}

		private void WalkEntityAssociationTree(
			IAssociationType associationType,
			IOuterJoinLoadable persister,
			int propertyNumber,
			string alias,
			string path,
			bool nullable,
			int currentDepth)
		{
			string[] aliasedLhsColumns = JoinHelper.GetAliasedLHSColumnNames(
				associationType, alias, propertyNumber, persister, Factory);
			string[] lhsColumns = JoinHelper.GetLHSColumnNames(
				associationType, propertyNumber, persister, Factory);
			string lhsTable = JoinHelper.GetLHSTableName(associationType, propertyNumber, persister);

			string subpath = SubPath(path, persister.GetSubclassPropertyName(propertyNumber));

			JoinType joinType = GetJoinType(
				associationType,
				persister.GetFetchMode(propertyNumber),
				subpath,
				lhsTable,
				lhsColumns,
				nullable,
				currentDepth,
				persister.GetCascadeStyle(propertyNumber)
				);

			AddAssociationToJoinTreeIfNecessary(
				associationType,
				aliasedLhsColumns,
				alias,
				subpath,
				currentDepth,
				joinType
				);
		}

		/// <summary>
		/// For an entity class, add to a list of associations to be fetched
		/// by outerjoin
		/// </summary>
		private void WalkEntityTree(
			IOuterJoinLoadable persister,
			string alias,
			string path,
			int currentDepth)
		{
			int n = persister.CountSubclassProperties();
			for (int i = 0; i < n; i++)
			{
				IType type = persister.GetSubclassPropertyType(i);
				if (type.IsAssociationType)
				{
					WalkEntityAssociationTree(
						(IAssociationType) type,
						persister,
						i,
						alias,
						path,
						persister.IsSubclassPropertyNullable(i),
						currentDepth
						);
				}
				else if (type.IsComponentType)
				{
					WalkComponentTree(
						(IAbstractComponentType) type,
						i,
						0,
						persister,
						alias,
						SubPath(path, persister.GetSubclassPropertyName(i)),
						currentDepth
						);
				}
			}
		}

		/// <summary>
		/// For a component, add to a list of associations to be fetched by outerjoin
		/// </summary>
		private void WalkComponentTree(
			IAbstractComponentType componentType,
			int propertyNumber,
			int begin,
			IOuterJoinLoadable persister,
			string alias,
			string path,
			int currentDepth)
		{
			IType[] types = componentType.Subtypes;
			string[] propertyNames = componentType.PropertyNames;
			for (int i = 0; i < types.Length; i++)
			{
				if (types[i].IsAssociationType)
				{
					IAssociationType associationType = (IAssociationType) types[i];
					string[] aliasedLhsColumns = JoinHelper.GetAliasedLHSColumnNames(
						associationType, alias, propertyNumber, begin, persister, Factory
						);
					string[] lhsColumns = JoinHelper.GetLHSColumnNames(
						associationType, propertyNumber, begin, persister, Factory
						);
					string lhsTable = JoinHelper.GetLHSTableName(
						associationType, propertyNumber, persister
						);
					string subpath = SubPath(path, propertyNames[i]);
					bool[] propertyNullability = componentType.PropertyNullability;

					JoinType joinType = GetJoinType(
						associationType,
						componentType.GetFetchMode(i),
						subpath,
						lhsTable,
						lhsColumns,
						propertyNullability == null || propertyNullability[i],
						currentDepth,
						componentType.GetCascadeStyle(i)
						);

					AddAssociationToJoinTreeIfNecessary(
						associationType,
						aliasedLhsColumns,
						alias,
						subpath,
						currentDepth,
						joinType
						);
				}
				else if (types[i].IsComponentType)
				{
					string subpath = SubPath(path, propertyNames[i]);

					WalkComponentTree(
						(IAbstractComponentType) types[i],
						propertyNumber,
						begin,
						persister,
						alias,
						subpath,
						currentDepth
						);
				}
				begin += types[i].GetColumnSpan(Factory);
			}
		}

		/// <summary>
		/// For a composite element, add to a list of associations to be fetched by outerjoin
		/// </summary>
		private void WalkCompositeElementTree(
			IAbstractComponentType compositeType,
			string[] cols,
			IQueryableCollection persister,
			string alias,
			string path,
			int currentDepth)
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

					JoinType joinType = GetJoinType(
						associationType,
						compositeType.GetFetchMode(i),
						subpath,
						persister.TableName,
						lhsColumns,
						propertyNullability == null || propertyNullability[i],
						currentDepth,
						compositeType.GetCascadeStyle(i)
						);

					AddAssociationToJoinTreeIfNecessary(
						associationType,
						aliasedLhsColumns,
						alias,
						subpath,
						currentDepth,
						joinType
						);
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
						currentDepth
						);
				}
				begin += length;
			}
		}

		/// <summary>
		/// Extend the path by the given property name
		/// </summary>
		private static string SubPath(string path, string property)
		{
			if (path == null || path.Length == 0)
			{
				return property;
			}
			else
			{
				return StringHelper.Qualify(path, property);
			}
		}

		/// <summary>
		/// Get the join type (inner, outer, etc) or -1 if the
		/// association should not be joined. Override on
		/// subclasses.
		/// </summary>
		protected virtual JoinType GetJoinType(
			IAssociationType type,
			FetchMode config,
			String path,
			String lhsTable,
			String[] lhsColumns,
			bool nullable,
			int currentDepth,
			Cascades.CascadeStyle cascadeStyle)
		{
			if (!IsJoinedFetchEnabled(type, config, cascadeStyle))
			{
				return JoinType.None;
			}

			if (IsTooDeep(currentDepth) || IsTooManyCollections())
			{
				return JoinType.None;
			}

			bool dupe = IsDuplicateAssociation(lhsTable, lhsColumns, type);

			if (dupe)
			{
				return JoinType.None;
			}

			return GetJoinType(nullable, currentDepth);
		}

		/// <summary>
		/// Use an inner join if it is a non-null association and this
		/// is the "first" join in a series
		/// </summary>
		protected JoinType GetJoinType(bool nullable, int currentDepth)
		{
			//TODO: this is too conservative; if all preceding joins were 
			//      also inner joins, we could use an inner join here
			return !nullable && currentDepth == 0 ?
			       JoinType.InnerJoin :
			       JoinType.LeftOuterJoin;
		}

		protected virtual bool IsTooDeep(int currentDepth)
		{
			int maxFetchDepth = Factory.MaximumFetchDepth;
			return maxFetchDepth >= 0 && currentDepth >= maxFetchDepth;
		}

		protected virtual bool IsTooManyCollections()
		{
			return false;
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
							EntityType entityType = type as EntityType;
							IEntityPersister persister = factory.GetEntityPersister(entityType.GetAssociatedEntityName());
							return !persister.HasProxy;
						}
						else
						{
							return false;
						}

					default:
						throw new ArgumentOutOfRangeException("config", config, "Unknown OJ strategy " + config.ToString());
				}
			}
		}

		/// <summary>
		/// Override on subclasses to enable or suppress joining
		/// of certain association types
		/// </summary>
		protected virtual bool IsJoinedFetchEnabled(IAssociationType type, FetchMode config,
		                                            Cascades.CascadeStyle cascadeStyle)
		{
			return type.IsEntityType && IsJoinedFetchEnabledInMapping(config, type);
		}

		protected virtual string GenerateTableAlias(int n, string path, IJoinable joinable)
		{
			return StringHelper.GenerateAlias(joinable.Name, n);
		}

		protected virtual string GenerateRootAlias(string description)
		{
			return StringHelper.GenerateAlias(description, 0);
		}

		/// <summary>
		/// Used to detect circularities in the joined graph, note that
		/// this method is side-effecty
		/// </summary>
		protected virtual bool IsDuplicateAssociation(
			string foreignKeyTable,
			string[] foreignKeyColumns)
		{
			AssociationKey associationKey = new AssociationKey(foreignKeyColumns, foreignKeyTable);
			return !visitedAssociationKeys.Add(associationKey);
		}

		/// <summary>
		/// Used to detect circularities in the joined graph, note that
		/// this method is side-effecty
		/// </summary>
		protected virtual bool IsDuplicateAssociation(
			string lhsTable,
			string[] lhsColumnNames,
			IAssociationType type)
		{
			string foreignKeyTable;
			string[] foreignKeyColumns;

			if (type.ForeignKeyDirection == ForeignKeyDirection.ForeignKeyFromParent)
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
		private class AssociationKey
		{
			private String[] columns;
			private String table;

			public AssociationKey(String[] columns, String table)
			{
				this.columns = columns;
				this.table = table;
			}

			public override bool Equals(object other)
			{
				AssociationKey that = (AssociationKey) other;
				return that.table.Equals(table) && CollectionHelper.CollectionEquals(columns, that.columns);
			}

			public override int GetHashCode()
			{
				return table.GetHashCode();
			}
		}

		/// <summary>
		/// Should we join this association?
		/// </summary>
		protected bool IsJoinable(
			JoinType joinType,
			ISet visitedAssociationKeys,
			string lhsTable,
			string[] lhsColumnNames,
			IAssociationType type,
			int depth
			)
		{
			if (joinType < JoinType.InnerJoin) return false;
			if (joinType == JoinType.InnerJoin) return true;
			return !IsTooDeep(depth) && !IsDuplicateAssociation(lhsTable, lhsColumnNames, type);
		}

		protected string OrderBy(IList associations, string orderBy)
		{
			string fullOrderBy = OrderBy(associations);

			if (fullOrderBy.Length == 0)
			{
				fullOrderBy = orderBy;
			}
			else if (orderBy.Length != 0)
			{
				fullOrderBy = fullOrderBy + ", " + orderBy;
			}

			return fullOrderBy;
		}

		/// <summary>
		/// Generate a sequence of <c>LEFT OUTER JOIN</c> clauses for the given associations.
		/// </summary>
		protected JoinFragment MergeOuterJoins(IList associations)
		{
			JoinFragment outerjoin = Dialect.CreateOuterJoinFragment();

			OuterJoinableAssociation last = null;
			foreach (OuterJoinableAssociation oj in associations)
			{
				//TODO H3:
				if (last != null && last.IsManyToManyWith(oj))
				{
					oj.AddManyToManyJoin(outerjoin, (IQueryableCollection) last.Joinable);
				}
				else
				{
					oj.AddJoins(outerjoin);
				}

				last = oj;
			}

			return outerjoin;
		}

		/// <summary>
		/// Count the number of instances of IJoinable which are actually
		/// also instances of ILoadable, or are one-to-many associations
		/// </summary>
		protected static int CountEntityPersisters(IList associations)
		{
			int result = 0;
			foreach (OuterJoinableAssociation oj in associations)
			{
				if (oj.Joinable.ConsumesEntityAlias())
				{
					result++;
				}
			}

			return result;
		}

		/// <summary>
		/// Count the number of instances of <see cref="IJoinable" /> which
		/// are actually also instances of <see cref="IPersistentCollection" />
		/// which are being fetched by outer join
		/// </summary>
		protected static int CountCollectionPersisters(IList associations)
		{
			int result = 0;

			foreach (OuterJoinableAssociation oj in associations)
			{
				if (oj.JoinType == JoinType.LeftOuterJoin && oj.Joinable.IsCollection)
				{
					result++;
				}
			}
			return result;
		}

		/// <summary>
		/// Get the order by string required for collection fetching
		/// </summary>
		protected static string OrderBy(IList associations)
		{
			StringBuilder buf = new StringBuilder();

			OuterJoinableAssociation last = null;
			foreach (OuterJoinableAssociation oj in associations)
			{
				if (oj.JoinType == JoinType.LeftOuterJoin)
				{
					if (oj.Joinable.IsCollection)
					{
						IQueryableCollection queryableCollection = (IQueryableCollection) oj.Joinable;
						if (queryableCollection.HasOrdering)
						{
							string orderByString = queryableCollection.GetSQLOrderByString(oj.RHSAlias);
							buf.Append(orderByString).Append(", ");
						}
					}
					else
					{
						// it might still need to apply a collection ordering based on a
						// many-to-many defined order-by...
						if (last != null && last.Joinable.IsCollection)
						{
							IQueryableCollection queryableCollection = (IQueryableCollection) last.Joinable;
							if (queryableCollection.IsManyToMany && last.IsManyToManyWith(oj))
							{
								if (queryableCollection.HasManyToManyOrdering)
								{
									string orderByString = queryableCollection.GetManyToManyOrderByString(oj.RHSAlias);
									buf.Append(orderByString).Append(", ");
								}
							}
						}
					}
				}
				last = oj;
			}

			if (buf.Length > 0)
			{
				buf.Length = buf.Length - 2;
			}

			return buf.ToString();
		}

		/// <summary>
		/// Render the where condition for a (batch) load by identifier / collection key
		/// </summary>
		protected SqlStringBuilder WhereString(string alias, string[] columnNames, IType type, int batchSize)
		{
			if (columnNames.Length == 1)
			{
				InFragment inf = new InFragment().SetColumn(alias, columnNames[0]);

				for (int i = 0; i < batchSize; i++)
				{
					inf.AddValue(Parameter.Placeholder);
				}

				return new SqlStringBuilder(inf.ToFragmentString());
			}
			else
			{
				Parameter[] columnParameters = Parameter.GenerateParameters(columnNames.Length);
				ConditionalFragment byId = new ConditionalFragment()
					.SetTableAlias(alias)
					.SetCondition(columnNames, columnParameters);

				SqlStringBuilder whereString = new SqlStringBuilder();

				if (batchSize == 1)
				{
					// if no batch, use "foo = ? and bar = ?"
					whereString.Add(byId.ToSqlStringFragment());
				}
				else
				{
					// if a composite key, use "( (foo = ? and bar = ?) or (foo = ? and bar = ?) )" for batching
					whereString.Add(StringHelper.OpenParen); // TODO: unnecessary for databases with ANSI-style joins
					DisjunctionFragment df = new DisjunctionFragment();
					for (int i = 0; i < batchSize; i++)
					{
						df.AddCondition(byId);
					}
					whereString.Add(df.ToFragmentString());
					whereString.Add(StringHelper.ClosedParen); // TODO: unnecessary for databases with ANSI-style joins
				}

				return whereString;
			}
		}

		protected void InitPersisters(IList associations, LockMode lockMode)
		{
			int joins = CountEntityPersisters(associations);
			int collections = CountCollectionPersisters(associations);

			collectionOwners = collections == 0 ? null : new int[collections];
			collectionPersisters = collections == 0 ? null : new ICollectionPersister[collections];
			collectionSuffixes = BasicLoader.GenerateSuffixes(joins + 1, collections);

			persisters = new ILoadable[joins];
			aliases = new String[joins];
			owners = new int[joins];
			ownerAssociationTypes = new EntityType[joins];
			lockModeArray = ArrayHelper.FillArray(lockMode, joins);

			int i = 0;
			int j = 0;

			foreach (OuterJoinableAssociation oj in associations)
			{
				if (!oj.IsCollection)
				{
					persisters[i] = (ILoadable) oj.Joinable;
					aliases[i] = oj.RHSAlias;
					owners[i] = oj.GetOwner(associations);
					ownerAssociationTypes[i] = (EntityType) oj.JoinableType;
					i++;
				}
				else
				{
					IQueryableCollection collPersister = (IQueryableCollection) oj.Joinable;

					if (oj.JoinType == JoinType.LeftOuterJoin)
					{
						//it must be a collection fetch
						collectionPersisters[j] = collPersister;
						collectionOwners[j] = oj.GetOwner(associations);
						j++;
					}

					if (collPersister.IsOneToMany)
					{
						persisters[i] = (ILoadable) collPersister.ElementPersister;
						aliases[i] = oj.RHSAlias;
						i++;
					}
				}
			}

			if (ArrayHelper.IsAllNegative(owners))
			{
				owners = null;
			}
			if (collectionOwners != null && ArrayHelper.IsAllNegative(collectionOwners))
			{
				collectionOwners = null;
			}
		}

		/// <summary>
		/// Generate a select list of columns containing all properties of the entity classes
		/// </summary>
		public string SelectString(IList associations)
		{
			if (associations.Count == 0)
			{
				return string.Empty;
			}
			else
			{
				SqlStringBuilder buf = new SqlStringBuilder(associations.Count * 3);
				buf.Add(StringHelper.CommaSpace);

				int entityAliasCount = 0;
				int collectionAliasCount = 0;

				for (int i = 0; i < associations.Count; i++)
				{
					OuterJoinableAssociation join = (OuterJoinableAssociation) associations[i];
					OuterJoinableAssociation next = (i == associations.Count - 1)
					                                	? null
					                                	: (OuterJoinableAssociation) associations[i + 1];

					IJoinable joinable = join.Joinable;
					string entitySuffix = (suffixes == null || entityAliasCount >= suffixes.Length)
					                      	? null
					                      	: suffixes[entityAliasCount];

					string collectionSuffix = (suffixes == null || collectionAliasCount >= collectionSuffixes.Length)
					                          	? null
					                          	: collectionSuffixes[collectionAliasCount];

					string selectFragment = joinable.SelectFragment(
						next == null ? null : next.Joinable,
						next == null ? null : next.RHSAlias,
						join.RHSAlias,
						entitySuffix,
						collectionSuffix,
						join.JoinType == JoinType.LeftOuterJoin
						);

					buf.Add(selectFragment);

					if (joinable.ConsumesEntityAlias())
					{
						entityAliasCount++;
					}

					if (joinable.ConsumesCollectionAlias() && join.JoinType == JoinType.LeftOuterJoin)
					{
						collectionAliasCount++;
					}

					if (i < associations.Count - 1 &&
					    !selectFragment.Trim().Equals(string.Empty))
					{
						buf.Add(StringHelper.CommaSpace);
					}
				}

				return buf.ToSqlString().ToString();
			}
		}
	}
}