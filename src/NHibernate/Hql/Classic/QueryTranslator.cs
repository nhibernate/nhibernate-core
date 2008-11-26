using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Iesi.Collections;
using Iesi.Collections.Generic;
using log4net;

using NHibernate.Engine;
using NHibernate.Hql.Util;
using NHibernate.Impl;
using NHibernate.Loader;
using NHibernate.Persister.Collection;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;
using NHibernate.Transform;
using NHibernate.Type;
using NHibernate.Util;
using NHibernate.Dialect.Function;
using System.Collections.Specialized;
using System.Collections.Generic;

namespace NHibernate.Hql.Classic
{
	/// <summary> 
	/// An instance of <c>QueryTranslator</c> translates a Hibernate query string to SQL.
	/// </summary>
	public class QueryTranslator : BasicLoader, IFilterTranslator
	{
		private static readonly string[] NoReturnAliases = new string[] {};

		private readonly string queryIdentifier;
		private readonly string queryString;

		private readonly IDictionary<string, string> typeMap = new LinkedHashMap<string, string>();
		private readonly IDictionary<string, string> collections = new LinkedHashMap<string, string>();
		private IList<string> returnedTypes = new List<string>();
		private readonly IList<string> fromTypes = new List<string>();
		private readonly IList<IType> scalarTypes = new List<IType>();
		private readonly IDictionary<string, List<int>> namedParameters = new Dictionary<string, List<int>>();
		private readonly IDictionary<string, string> aliasNames = new Dictionary<string, string>();
		private readonly IDictionary<string, string> oneToOneOwnerNames = new Dictionary<string, string>();
		private readonly IDictionary<string, IAssociationType> uniqueKeyOwnerReferences = new Dictionary<string, IAssociationType>();
		private readonly IDictionary<string, IPropertyMapping> decoratedPropertyMappings = new Dictionary<string, IPropertyMapping>();

		private readonly IList<SqlString> scalarSelectTokens = new List<SqlString>();
		private readonly IList<SqlString> whereTokens = new List<SqlString>();
		private readonly IList<SqlString> havingTokens = new List<SqlString>();
		private readonly IDictionary<string, JoinSequence> joins = new LinkedHashMap<string, JoinSequence>();
		private readonly IList<SqlString> orderByTokens = new List<SqlString>();
		private readonly IList<SqlString> groupByTokens = new List<SqlString>();
		private readonly ISet<string> querySpaces = new HashedSet<string>();
		private readonly ISet<string> entitiesToFetch = new HashedSet<string>();

		private readonly IDictionary<string, string> pathAliases = new Dictionary<string, string>();
		private readonly IDictionary<string, JoinSequence> pathJoins = new Dictionary<string, JoinSequence>();

		private IQueryable[] persisters;
		private int[] owners;
		private EntityType[] ownerAssociationTypes;
		private string[] names;
		private bool[] includeInSelect;
		private int selectLength;
		private IType[] returnTypes;
		private IType[] actualReturnTypes;
		private string[][] scalarColumnNames;
		private IDictionary<string, string> tokenReplacements;
		private int nameCount = 0;
		private int parameterCount = 0;
		private bool distinct = false;
		private bool compiled;
		private SqlString sqlString;
		private System.Type holderClass;
		private ConstructorInfo holderConstructor;
		private bool hasScalars;
		private bool shallowQuery;
		private QueryTranslator superQuery;

		private class FetchedCollections
		{
			private int count = 0;
			private List<ICollectionPersister> persisters;
			private List<string> names;
			private List<string> ownerNames;
			private List<string> suffixes;

			// True if one of persisters represents a collection that is not safe to use in multiple join scenario.
			// Currently every collection except bag is safe.
			private bool hasUnsafeCollection = false;

			private List<int> ownerColumns;

			private static string GenerateSuffix(int count)
			{
				return count + "__";
			}

			private static bool IsUnsafe(ICollectionPersister collectionPersister)
			{
				return collectionPersister.CollectionType is BagType
				       || collectionPersister.CollectionType is IdentifierBagType;
			}

			public void Add(string name, ICollectionPersister collectionPersister, string ownerName)
			{
				if (persisters == null)
				{
					persisters = new List<ICollectionPersister>(2);
					names = new List<string>(2);
					ownerNames = new List<string>(2);
					suffixes = new List<string>(2);
				}

				count++;

				hasUnsafeCollection = hasUnsafeCollection || IsUnsafe(collectionPersister);

				// NH : This constraint is present in BasicLoader.PostInstantiate
				// The constraint here break some tests ported from H3.2 
				// where is possible the use of "left join fetch"
				//if (count > 1 && hasUnsafeCollection)
				//{
				//  // The comment only mentions a bag since I don't want to confuse users.
				//  throw new QueryException("Cannot fetch multiple collections in a single query if one of them is a bag");
				//}

				names.Add(name);
				persisters.Add(collectionPersister);
				ownerNames.Add(ownerName);
				suffixes.Add(GenerateSuffix(count - 1));
			}

			public int[] CollectionOwners
			{
				get
				{
					if (ownerColumns == null)
					{
						return null;
					}
					return ownerColumns.ToArray();
				}
			}

			public ICollectionPersister[] CollectionPersisters
			{
				get
				{
					if (persisters == null)
					{
						return null;
					}
					return persisters.ToArray();
				}
			}

			public string[] CollectionSuffixes
			{
				get
				{
					if (suffixes == null)
					{
						return null;
					}
					return suffixes.ToArray();
				}
			}

			public void AddSelectFragmentString(QuerySelect sql)
			{
				if (persisters == null)
				{
					return;
				}

				for (int i = 0; i < count; i++)
				{
					sql.AddSelectFragmentString(new SqlString(
						((IQueryableCollection) persisters[i]).SelectFragment(
							(string) names[i], (string) suffixes[i])));
				}
			}

			public void AddOrderBy(QuerySelect sql)
			{
				if (persisters == null)
				{
					return;
				}

				for (int i = 0; i < count; i++)
				{
					IQueryableCollection persister = (IQueryableCollection) persisters[i];
					if (persister.HasOrdering)
					{
						sql.AddOrderBy(persister.GetSQLOrderByString(names[i]));
					}
				}
			}

			public void InitializeCollectionOwnerColumns(IList<string> returnedTypes)
			{
				if (count == 0)
				{
					return;
				}

				ownerColumns = new List<int>(count);
				for (int i = 0; i < count; i++)
				{
					string ownerName = ownerNames[i];
					// This is quite slow in theory but there should only be a few collections
					// so it shouldn't be a problem in practice.
					int ownerIndex = returnedTypes.IndexOf(ownerName);
					ownerColumns.Add(ownerIndex);
				}
			}
		}

		private readonly FetchedCollections fetchedCollections = new FetchedCollections();

		private string[] suffixes;

		private IDictionary<string, IFilter> enabledFilters;

		private static readonly ILog log = LogManager.GetLogger(typeof(QueryTranslator));

		/// <summary> Construct a query translator </summary>
		/// <param name="queryIdentifier">
		/// A unique identifier for the query of which this
		/// translation is part; typically this is the original, user-supplied query string.
		/// </param>
		/// <param name="queryString">
		/// The "preprocessed" query string; at the very least
		/// already processed by {@link org.hibernate.hql.QuerySplitter}.
		/// </param>
		/// <param name="enabledFilters">Any enabled filters.</param>
		/// <param name="factory">The session factory. </param>
		public QueryTranslator(string queryIdentifier, string queryString, IDictionary<string, IFilter> enabledFilters, ISessionFactoryImplementor factory)
			: base(factory)
		{
			this.queryIdentifier = queryIdentifier;
			this.queryString = queryString;
			this.enabledFilters = enabledFilters;
		}

		/// <summary> 
		/// Construct a query translator
		/// </summary>
		public QueryTranslator(ISessionFactoryImplementor factory, string queryString, IDictionary<string, IFilter> enabledFilters)
			: this(queryString, queryString, enabledFilters, factory) {}

		/// <summary>
		/// Compile a subquery
		/// </summary>
		/// <param name="superquery"></param>
		protected internal void Compile(QueryTranslator superquery)
		{
			tokenReplacements = superquery.tokenReplacements;
			superQuery = superquery;
			shallowQuery = true;
			enabledFilters = superquery.EnabledFilters;

			Compile();
		}

		/// <summary>
		/// Compile a "normal" query. This method may be called multiple
		/// times. Subsequent invocations are no-ops.
		/// </summary>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void Compile(IDictionary<string,string> replacements, bool scalar)
		{
			if (!Compiled)
			{
				tokenReplacements = replacements;
				shallowQuery = scalar;

				Compile();
			}
		}

		/// <summary>
		/// Compile a filter. This method may be called multiple
		/// times. Subsequent invocations are no-ops.
		/// </summary>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void Compile(string collectionRole, IDictionary<string, string> replacements, bool scalar)
		{
			if (!Compiled)
			{
				AddFromAssociation("this", collectionRole);
				Compile(replacements, scalar);
			}
		}

		/// <summary> 
		/// Compile the query (generate the SQL).
		/// </summary>
		protected void Compile()
		{
			log.Debug("compiling query");
			try
			{
				ParserHelper.Parse(
					new PreprocessingParser(tokenReplacements),
					queryString,
					ParserHelper.HqlSeparators,
					this);
				RenderSql();
			}
			catch (QueryException qe)
			{
				qe.QueryString = queryString;
				throw;
			}
			catch (MappingException)
			{
				throw;
			}
			catch (Exception e)
			{
				log.Debug("unexpected query compilation problem", e);
				QueryException qe = new QueryException("Incorrect query syntax", e);
				qe.QueryString = queryString;
				throw qe;
			}

			PostInstantiate();

			compiled = true;
		}

		/// <summary>
		/// Persisters for the return values of a <c>List</c> style query
		/// </summary>
		/// <remarks>
		/// The <c>Persisters</c> stored by QueryTranslator have to be <see cref="IQueryable"/>.  The
		/// <c>setter</c> will attempt to cast the <c>ILoadable</c> array passed in into an 
		/// <c>IQueryable</c> array.
		/// </remarks>
		protected internal override ILoadable[] EntityPersisters
		{
			get { return persisters; }
		}

		/// <summary>
		///Types of the return values of an <c>Enumerate()</c> style query.
		///Return an array of <see cref="IType" />s.
		/// </summary>
		public virtual IType[] ReturnTypes
		{
			get { return returnTypes; }
		}

		internal virtual IType[] ActualReturnTypes
		{
			get { return actualReturnTypes; }
		}

		public virtual string[][] ScalarColumnNames
		{
			get { return scalarColumnNames; }
		}

		private static void LogQuery(string hql, string sql)
		{
			if (log.IsDebugEnabled)
			{
				log.Debug("HQL: " + hql);
				log.Debug("SQL: " + sql);
			}
		}

		internal void SetAliasName(string alias, string name)
		{
			aliasNames.Add(alias, name);
		}

		internal string GetAliasName(string alias)
		{
			string name;
			if (!aliasNames.TryGetValue(alias, out name))
			{
				if (superQuery != null)
					name = superQuery.GetAliasName(alias);
				else
					name = alias;
			}
			return name;
		}

		internal string Unalias(string path)
		{
			string alias = StringHelper.Root(path);
			string name = GetAliasName(alias);
			if (name != null)
			{
				return name + path.Substring(alias.Length);
			}
			else
			{
				return path;
			}
		}

		public void AddEntityToFetch(string name, string oneToOneOwnerName, IAssociationType ownerAssociationType)
		{
			AddEntityToFetch(name);
			if (oneToOneOwnerName != null)
			{
				oneToOneOwnerNames[name] = oneToOneOwnerName;
			}
			if (ownerAssociationType != null)
			{
				uniqueKeyOwnerReferences[name] = ownerAssociationType;
			}
		}

		public void AddEntityToFetch(string name)
		{
			entitiesToFetch.Add(name);
		}

		/// <summary></summary>
		[CLSCompliant(false)]
		// TODO: Work out why this causes an error in 1.1 - the variable sqlString is private so we're only exposing one name
		protected internal override SqlString SqlString
		{
			// this needs internal access because the WhereParser needs to be able to "get" it.
			get { return sqlString; }
		}

		private int NextCount()
		{
			return (superQuery == null) ? nameCount++ : superQuery.nameCount++;
		}

		internal string CreateNameFor(string type)
		{
			string name = StringHelper.UnqualifyEntityName(type); // NH: the default EntityName is the FullName of the class
			return StringHelper.GenerateAlias(name, NextCount());
		}

		internal string CreateNameForCollection(string role)
		{
			return StringHelper.GenerateAlias(role, NextCount());
		}

		internal string GetType(string name)
		{
			string type;
			if (!typeMap.TryGetValue(name, out type) && superQuery != null)
			{
				type = superQuery.GetType(name);
			}
			return type;
		}

		internal string GetRole(string name)
		{
			string role;
			if (!collections.TryGetValue(name, out role) && superQuery != null)
			{
				role = superQuery.GetRole(name);
			}
			return role;
		}

		internal bool IsName(string name)
		{
			return aliasNames.ContainsKey(name) ||
			       typeMap.ContainsKey(name) ||
			       collections.ContainsKey(name) ||
			       (superQuery != null && superQuery.IsName(name));
		}

		public IPropertyMapping GetPropertyMapping(string name)
		{
			IPropertyMapping decorator = GetDecoratedPropertyMapping(name);
			if (decorator != null)
			{
				return decorator;
			}

			string type = GetType(name);
			if (type == null)
			{
				string role = GetRole(name);
				if (role == null)
				{
					throw new QueryException(string.Format("alias not found: {0}", name));
				}
				return GetCollectionPersister(role);
			}
			else
			{
				IQueryable persister = GetPersister(type);
				if (persister == null)
				{
					throw new QueryException(string.Format("Persistent class not found for entity named: {0}", type));
				}
				return persister;
			}
		}

		public IPropertyMapping GetDecoratedPropertyMapping(string name)
		{
			IPropertyMapping result;
			decoratedPropertyMappings.TryGetValue(name, out result);
			return result;
		}

		public void DecoratePropertyMapping(string name, IPropertyMapping mapping)
		{
			decoratedPropertyMappings.Add(name, mapping);
		}

		internal IQueryable GetPersisterForName(string name)
		{
			string type = GetType(name);
			IQueryable persister = GetPersister(type);
			if (persister == null)
			{
				throw new QueryException("Persistent class not found for entity named: " + type);
			}

			return persister;
		}

		internal IQueryable GetPersisterUsingImports(string className)
		{
			return SessionFactoryHelper.FindQueryableUsingImports(Factory, className);
		}

		internal IQueryable GetPersister(string clazz)
		{
			try
			{
				return (IQueryable) Factory.GetEntityPersister(clazz);
			}
			catch (Exception)
			{
				throw new QueryException("Persistent class not found for entity named: " + clazz);
			}
		}

		internal IQueryableCollection GetCollectionPersister(string role)
		{
			try
			{
				return (IQueryableCollection) Factory.GetCollectionPersister(role);
			}
			catch (InvalidCastException)
			{
				throw new QueryException(string.Format("collection role is not queryable: {0}", role));
			}
			catch (Exception)
			{
				throw new QueryException(string.Format("collection role not found: {0}", role));
			}
		}

		internal void AddType(string name, string type)
		{
			typeMap[name] = type;
		}

		internal void AddCollection(string name, string role)
		{
			collections[name]= role;
		}

		internal void AddFrom(string name, string type, JoinSequence joinSequence)
		{
			AddType(name, type);
			AddFrom(name, joinSequence);
		}

		internal void AddFromCollection(string name, string collectionRole, JoinSequence joinSequence)
		{
			//register collection role
			AddCollection(name, collectionRole);
			AddJoin(name, joinSequence);
		}

		internal void AddFrom(string name, JoinSequence joinSequence)
		{
			fromTypes.Add(name);
			AddJoin(name, joinSequence);
		}

		internal void AddFromClass(string name, IQueryable classPersister)
		{
			JoinSequence joinSequence = new JoinSequence(Factory)
				.SetRoot(classPersister, name);
			AddFrom(name, classPersister.EntityName, joinSequence);
		}

		internal void AddSelectClass(string name)
		{
			returnedTypes.Add(name);
		}

		internal void AddSelectScalar(IType type)
		{
			scalarTypes.Add(type);
		}

		internal void AppendWhereToken(SqlString token)
		{
			whereTokens.Add(token);
		}

		internal void AppendHavingToken(SqlString token)
		{
			havingTokens.Add(token);
		}

		internal void AppendOrderByToken(string token)
		{
			if (StringHelper.SqlParameter.Equals(token))
				orderByTokens.Add(SqlString.Parameter);
			else
				orderByTokens.Add(new SqlString(token));
		}

		internal void AppendOrderByParameter()
		{
			orderByTokens.Add(SqlString.Parameter);
		}

		internal void AppendGroupByToken(string token)
		{
			groupByTokens.Add(new SqlString(token));
		}

		internal void AppendGroupByParameter()
		{
			groupByTokens.Add(SqlString.Parameter);
		}

		internal void AppendScalarSelectToken(string token)
		{
			scalarSelectTokens.Add(new SqlString(token));
		}

		internal void AppendScalarSelectTokens(string[] tokens)
		{
			scalarSelectTokens.Add(new SqlString(tokens));
		}

		internal void AppendScalarSelectParameter()
		{
			scalarSelectTokens.Add(SqlString.Parameter);
		}

		internal void AddJoin(string name, JoinSequence joinSequence)
		{
			if (!joins.ContainsKey(name))
			{
				joins.Add(name, joinSequence);
			}
		}

		internal void AddNamedParameter(string name)
		{
			if (superQuery != null)
			{
				superQuery.AddNamedParameter(name);
			}

			// want the param index to start at 0 instead of 1
			//int loc = ++parameterCount;
			int loc = parameterCount++;
			List<int> o;
			if (!namedParameters.TryGetValue(name, out o))
			{
				List<int> list = new List<int>(4);
				list.Add(loc);
				namedParameters[name] = list;
			}
			else
			{
				o.Add(loc);
			}
		}

		public override int[] GetNamedParameterLocs(string name)
		{
			List<int> o;
			if (!namedParameters.TryGetValue(name, out o))
			{
				QueryException qe = new QueryException("Named parameter does not appear in Query: " + name);
				qe.QueryString = queryString;
				throw qe;
			}
			return o.ToArray();
		}

		public static string ScalarName(int x, int y)
		{
			return new StringBuilder()
				.Append('x')
				.Append(x)
				.Append(StringHelper.Underscore)
				.Append(y)
				.Append(StringHelper.Underscore)
				.ToString();
		}

		private void RenderSql()
		{
			int rtsize;
			if (returnedTypes.Count == 0 && scalarTypes.Count == 0)
			{
				//ie no select clause in HQL
				returnedTypes = fromTypes;
				rtsize = returnedTypes.Count;
			}
			else
			{
				rtsize = returnedTypes.Count;
				foreach (string entityName in entitiesToFetch)
				{
					returnedTypes.Add(entityName);
				}
			}

			int size = returnedTypes.Count;
			persisters = new IQueryable[size];
			names = new string[size];
			owners = new int[size];
			ownerAssociationTypes = new EntityType[size];
			suffixes = new string[size];
			includeInSelect = new bool[size];
			for (int i = 0; i < size; i++)
			{
				string name = returnedTypes[i];
				//if ( !IsName(name) ) throw new QueryException("unknown type: " + name);
				persisters[i] = GetPersisterForName(name);
				suffixes[i] = (size == 1) ? String.Empty : i.ToString() + StringHelper.Underscore;
				names[i] = name;
				includeInSelect[i] = !entitiesToFetch.Contains(name);
				if (includeInSelect[i])
				{
					selectLength++;
				}
				string oneToOneOwner;
				owners[i] = !oneToOneOwnerNames.TryGetValue(name, out oneToOneOwner) ? -1 : returnedTypes.IndexOf(oneToOneOwner);
				IAssociationType oat;
				if (uniqueKeyOwnerReferences.TryGetValue(name, out oat))
					ownerAssociationTypes[i] = (EntityType) oat;
			}

			fetchedCollections.InitializeCollectionOwnerColumns(returnedTypes);

			if (ArrayHelper.IsAllNegative(owners))
			{
				owners = null;
			}

			SqlString scalarSelect = RenderScalarSelect(); //Must be done here because of side-effect! yuck...

			int scalarSize = scalarTypes.Count;
			hasScalars = scalarTypes.Count != rtsize;

			returnTypes = new IType[scalarSize];
			for (int i = 0; i < scalarSize; i++)
			{
				returnTypes[i] = scalarTypes[i];
			}

			QuerySelect sql = new QuerySelect(Factory.Dialect);
			sql.Distinct = distinct;

			if (!shallowQuery)
			{
				RenderIdentifierSelect(sql);
				RenderPropertiesSelect(sql);
			}

			fetchedCollections.AddSelectFragmentString(sql);

			if (hasScalars || shallowQuery)
			{
				sql.AddSelectFragmentString(scalarSelect);
			}

			// TODO: for some dialects it would be appropriate to add the renderOrderByPropertiesSelect() to other select strings
			MergeJoins(sql.JoinFragment);

			// HQL functions in whereTokens, groupByTokens, havingTokens and orderByTokens aren't rendered
			RenderFunctions(whereTokens);
			sql.SetWhereTokens((ICollection)whereTokens);

			RenderFunctions(groupByTokens);
			sql.SetGroupByTokens((ICollection)groupByTokens);

			RenderFunctions(havingTokens);
			sql.SetHavingTokens((ICollection)havingTokens);

			RenderFunctions(orderByTokens);
			sql.SetOrderByTokens((ICollection)orderByTokens);

			fetchedCollections.AddOrderBy(sql);

			scalarColumnNames = GenerateColumnNames(returnTypes, Factory);

			// initialize the set of queried identifer spaces (ie. tables)
			foreach (string name in collections.Values)
			{
				ICollectionPersister p = GetCollectionPersister(name);
				AddQuerySpaces(p.CollectionSpaces);
			}
			foreach (string name in typeMap.Keys)
			{
				IQueryable p = GetPersisterForName(name);
				AddQuerySpaces(p.QuerySpaces);
			}

			sqlString = sql.ToQuerySqlString();

			try
			{
				if (holderClass != null)
				{
					holderConstructor = ReflectHelper.GetConstructor(holderClass, returnTypes);
				}
			}
			catch (Exception nsme)
			{
				throw new QueryException("could not find constructor for: " + holderClass.Name, nsme);
			}

			if (hasScalars)
			{
				actualReturnTypes = returnTypes;
			}
			else
			{
				actualReturnTypes = new IType[selectLength];
				int j = 0;
				for (int i = 0; i < persisters.Length; i++)
				{
					if (includeInSelect[i])
					{
						actualReturnTypes[j++] = NHibernateUtil.Entity(persisters[i].EntityName);
					}
				}
			}
		}

		private void RenderIdentifierSelect(QuerySelect sql)
		{
			int size = returnedTypes.Count;

			for (int k = 0; k < size; k++)
			{
				string name = returnedTypes[k];
				string suffix = size == 1 ? String.Empty : k.ToString() + StringHelper.Underscore;
				sql.AddSelectFragmentString(new SqlString(persisters[k].IdentifierSelectFragment(name, suffix)));
			}
		}

		private void RenderPropertiesSelect(QuerySelect sql)
		{
			int size = returnedTypes.Count;
			for (int k = 0; k < size; k++)
			{
				string suffix = (size == 1) ? String.Empty : k.ToString() + StringHelper.Underscore;
				string name = (string) returnedTypes[k];
				sql.AddSelectFragmentString(new SqlString(persisters[k].PropertySelectFragment(name, suffix, false)));
			}
		}

		/// <summary> 
		/// WARNING: side-effecty
		/// </summary>
		private SqlString RenderScalarSelect()
		{
			bool isSubselect = superQuery != null;

			SqlStringBuilder buf = new SqlStringBuilder();

			if (scalarTypes.Count == 0)
			{
				//ie. no select clause
				int size = returnedTypes.Count;
				for (int k = 0; k < size; k++)
				{
					scalarTypes.Add(NHibernateUtil.Entity(persisters[k].EntityName));

					string[] _names = persisters[k].IdentifierColumnNames;
					for (int i = 0; i < _names.Length; i++)
					{
						buf.Add(returnedTypes[k].ToString()).Add(StringHelper.Dot.ToString()).Add(_names[i]);
						if (!isSubselect)
						{
							buf.Add(" as ").Add(ScalarName(k, i));
						}
						if (i != _names.Length - 1 || k != size - 1)
						{
							buf.Add(StringHelper.CommaSpace);
						}
					}
				}
			}
			else
			{
				//there _was_ a select clause
				int c = 0;
				bool nolast = false; //real hacky...
				int parenCount = 0; // used to count the nesting of parentheses
				for (int tokenIdx = 0; tokenIdx < scalarSelectTokens.Count; tokenIdx++)
				{
					SqlString next = scalarSelectTokens[tokenIdx];
					if (next.Count == 1)
					{
						string token = next.ToString();
						string lc = token.ToLowerInvariant();
						ISQLFunction func = Factory.SQLFunctionRegistry.FindSQLFunction(lc);
						if (func != null)
						{
							// Render the HQL function
							SqlString renderedFunction = RenderFunctionClause(func, scalarSelectTokens, ref tokenIdx);
							buf.Add(renderedFunction);
						}
						else
						{
							if (StringHelper.OpenParen.Equals(token))
							{
								parenCount++;
							}
							else if (StringHelper.ClosedParen.Equals(token))
							{
								parenCount--;
							}
							else if (lc.Equals(StringHelper.CommaSpace))
							{
								if (nolast)
								{
									nolast = false;
								}
								else
								{
									if (!isSubselect && parenCount == 0)
									{
										buf.Add(" as ").Add(ScalarName(c++, 0));
									}
								}
							}

							buf.Add(token);
							if (lc.Equals("distinct") || lc.Equals("all"))
							{
								buf.Add(" ");
							}
						}
					}
					else
					{
						nolast = true;
						int i = 0;
						foreach (object token in next.Parts)
						{
							buf.AddObject(token);
							if (!isSubselect)
							{
								buf.Add(" as ").Add(ScalarName(c, i));
							}
							if (i != next.Count - 1)
							{
								buf.Add(StringHelper.CommaSpace);
							}
							i++;
						}
						c++;
					}
				}
				if (!isSubselect && !nolast)
				{
					buf.Add(" as ").Add(ScalarName(c++, 0));
				}
			}

			return buf.ToSqlString();
		}

		private void RenderFunctions(IList<SqlString> tokens)
		{
			for (int tokenIdx = 0; tokenIdx < tokens.Count; tokenIdx++)
			{
				string token = tokens[tokenIdx].ToString().ToLowerInvariant();
				ISQLFunction func = Factory.SQLFunctionRegistry.FindSQLFunction(token);
				if (func != null)
				{
					int flTokenIdx = tokenIdx;
					SqlString renderedFunction = RenderFunctionClause(func, tokens, ref flTokenIdx);
					// At this point we have the trunk that represents the function with its 
					// arguments enclosed in parens. Now all token in the tokens list will be
					// removed from the original list and replaced with the rendered function.
					for (int i = 0; i < flTokenIdx - tokenIdx; i++)
					{
						tokens.RemoveAt(tokenIdx + 1);
					}
					tokens[tokenIdx] = new SqlString(renderedFunction);
				}
			}
		}

		/// <summary>
		/// Extract the complete clause of function.
		/// </summary>
		/// <param name="tokens">The list of tokens</param>
		/// <param name="tokenIdx">The index of the list that represent the founded function.</param>
		/// <returns>String trepresentation of each token.</returns>
		/// <remarks>Each token can be string or SqlString </remarks>
		private IList<SqlString> ExtractFunctionClause(IList<SqlString> tokens, ref int tokenIdx)
		{
			SqlString funcName = tokens[tokenIdx];
			IList<SqlString> functionTokens = new List<SqlString>();
			functionTokens.Add(funcName);
			tokenIdx++;
			if (tokenIdx >= tokens.Count ||
				!StringHelper.OpenParen.Equals(tokens[tokenIdx].ToString()))
			{
				// All function with arguments have the syntax
				// <function name> <left paren> <arguments> <right paren>
				throw new QueryException("'(' expected after function " + funcName);
			}
			functionTokens.Add(new SqlString(StringHelper.OpenParen));
			tokenIdx++;
			int parenCount = 1;
			for (; tokenIdx < tokens.Count && parenCount > 0; tokenIdx++)
			{
				if (tokens[tokenIdx].StartsWithCaseInsensitive(ParserHelper.HqlVariablePrefix) || tokens[tokenIdx].ToString().Equals(StringHelper.SqlParameter))
				{
					functionTokens.Add(SqlString.Parameter);
				}
				else
				{
					functionTokens.Add(tokens[tokenIdx]);
				}
				if (StringHelper.OpenParen.Equals(tokens[tokenIdx].ToString()))
				{
					parenCount++;
				}
				else if (StringHelper.ClosedParen.Equals(tokens[tokenIdx].ToString()))
				{
					parenCount--;
				}
			}
			tokenIdx--; // position of the last managed token
			if (parenCount > 0)
			{
				throw new QueryException("')' expected for function " + funcName);
			}
			return functionTokens;
		}

		private SqlString RenderFunctionClause(ISQLFunction func, IList<SqlString> tokens, ref int tokenIdx)
		{
			IList<SqlString> functionTokens;
			if (!func.HasArguments)
			{
				// The function doesn't work with arguments.
				if (func.HasParenthesesIfNoArguments)
					ExtractFunctionClause(tokens, ref tokenIdx);

				// The function render simply translate its name for a specific dialect.
				return func.Render(new List<object>(), Factory);
			}
			functionTokens = ExtractFunctionClause(tokens, ref tokenIdx);

			IFunctionGrammar fg = func as IFunctionGrammar;
			if (fg == null)
				fg = new CommonGrammar();

			IList args = new List<object>();
			SqlStringBuilder argBuf = new SqlStringBuilder();
			// Extract args splitting first 2 token because are: FuncName(
			// last token is ')'
			// To allow expressions like arg (ex:5+5) all tokens between 'argument separator' or
			// a 'know argument' are compacted in a string, 
			// because many HQL function expect IList<string> like args in Render method.
			// This solution give us the ability to use math expression in common function. 
			// Ex: sum(a.Prop+10), cast(yesterday-1 as date)
			for (int argIdx = 2; argIdx < functionTokens.Count - 1; argIdx++)
			{
				object token = functionTokens[argIdx];
				if (fg.IsKnownArgument(token.ToString()))
				{
					if (argBuf.Count > 0)
					{
						// end of the previous argument
						args.Add(argBuf.ToSqlString());
						argBuf = new SqlStringBuilder();
					}
					args.Add(token);
				}
				else if (fg.IsSeparator(token.ToString()))
				{
					// argument end
					if (argBuf.Count > 0)
					{
						args.Add(argBuf.ToSqlString());
						argBuf = new SqlStringBuilder();
					}
				}
				else
				{
					ISQLFunction nfunc = Factory.SQLFunctionRegistry.FindSQLFunction(token.ToString().ToLowerInvariant());
					if (nfunc != null)
					{
						// the token is a nested function call
						argBuf.Add(RenderFunctionClause(nfunc, functionTokens, ref argIdx));
					}
					else
					{
						// the token is a part of an argument (every thing else)
						argBuf.AddObject(token);
					}
				}
			}
			// Add the last arg
			if (argBuf.Count > 0)
				args.Add(argBuf.ToSqlString());
			return func.Render(args, Factory);
		}

		private class Selector : JoinSequence.ISelector
		{
			private QueryTranslator outer;

			public Selector(QueryTranslator outer)
			{
				this.outer = outer;
			}

			public bool IncludeSubclasses(string alias)
			{
				bool include = outer.returnedTypes.Contains(alias) && !outer.IsShallowQuery;
				return include;
			}
		}

		private void MergeJoins(JoinFragment ojf)
		{
			foreach (KeyValuePair<string, JoinSequence> de in joins)
			{
				string name = de.Key;
				JoinSequence join = de.Value;
				join.SetSelector(new Selector(this));

				if (typeMap.ContainsKey(name))
				{
					ojf.AddFragment(join.ToJoinFragment(enabledFilters, true));
				}
				else if (collections.ContainsKey(name))
				{
					ojf.AddFragment(join.ToJoinFragment(enabledFilters, true));
				}
				else
				{
					//name from a super query (a bit inelegant that it shows up here)
				}
			}
		}

		public ISet<string> QuerySpaces
		{
			get { return querySpaces; }
		}

		/// <summary>
		/// Is this query called by Scroll() or Iterate()?
		/// </summary>
		/// <value>true if it is, false if it is called by find() or list()</value>
		public bool IsShallowQuery
		{
			get { return shallowQuery; }
		}

		internal bool Distinct
		{
			set { distinct = value; }
		}

		/// <summary></summary>
		public bool IsSubquery
		{
			get { return superQuery != null; }
		}

		protected override ICollectionPersister[] CollectionPersisters
		{
			get { return fetchedCollections.CollectionPersisters; }
		}

		protected override string[] CollectionSuffixes
		{
			get { return fetchedCollections.CollectionSuffixes; }
		}

		public void AddCollectionToFetch(string role, string name, string ownerName, string entityName)
		{
			IQueryableCollection persister = GetCollectionPersister(role);
			fetchedCollections.Add(name, persister, ownerName);
			if (persister.ElementType.IsEntityType)
			{
				AddEntityToFetch(entityName);
			}
		}

		protected override string[] Suffixes
		{
			get { return suffixes; }
		}

		/// <remarks>Used for collection filters</remarks>
		protected void AddFromAssociation(string elementName, string collectionRole)
		{
			//q.addCollection(collectionName, collectionRole);
			IType collectionElementType = GetCollectionPersister(collectionRole).ElementType;
			if (!collectionElementType.IsEntityType)
			{
				throw new QueryException("collection of values in filter: " + elementName);
			}

			IQueryableCollection persister = GetCollectionPersister(collectionRole);
			string[] keyColumnNames = persister.KeyColumnNames;
			//if (keyColumnNames.Length!=1) throw new QueryException("composite-key collecion in filter: " + collectionRole);

			string collectionName;
			JoinSequence join = new JoinSequence(Factory);
			collectionName = persister.IsOneToMany ? elementName : CreateNameForCollection(collectionRole);
			join.SetRoot(persister, collectionName);
			if (!persister.IsOneToMany)
			{
				//many-to-many
				AddCollection(collectionName, collectionRole);

				try
				{
					join.AddJoin(
						(IAssociationType) persister.ElementType,
						elementName,
						JoinType.InnerJoin,
						persister.GetElementColumnNames(collectionName));
				}
				catch (MappingException me)
				{
					throw new QueryException(me);
				}
			}
			join.AddCondition(collectionName, keyColumnNames, " = ", true);
			EntityType elmType = (EntityType) collectionElementType;
			AddFrom(elementName, elmType.GetAssociatedEntityName(), join);
		}

		internal string GetPathAlias(string path)
		{
			string result;
			pathAliases.TryGetValue(path, out result);
			return result;
		}

		internal JoinSequence GetPathJoin(string path)
		{
			JoinSequence result;
			pathJoins.TryGetValue(path, out result);
			return result;
		}

		internal void AddPathAliasAndJoin(string path, string alias, JoinSequence joinSequence)
		{
			pathAliases.Add(path, alias);
			pathJoins.Add(path, joinSequence);
		}

		public IList List(ISessionImplementor session, QueryParameters queryParameters)
		{
			return List(session, queryParameters, QuerySpaces, actualReturnTypes);
		}

		public IEnumerable GetEnumerable(QueryParameters parameters, ISessionImplementor session)
		{
			bool statsEnabled = session.Factory.Statistics.IsStatisticsEnabled;

			var stopWath = new Stopwatch();
			if (statsEnabled)
			{
				stopWath.Start();
			}

			IDbCommand cmd = PrepareQueryCommand(parameters, false, session);

			// This IDataReader is disposed of in EnumerableImpl.Dispose
			IDataReader rs = GetResultSet(cmd, parameters.HasAutoDiscoverScalarTypes, false, parameters.RowSelection, session);
			HolderInstantiator hi =
				HolderInstantiator.CreateClassicHolderInstantiator(holderConstructor, parameters.ResultTransformer);
			IEnumerable result =
				new EnumerableImpl(rs, cmd, session, ReturnTypes, ScalarColumnNames, parameters.RowSelection, hi);
			if (statsEnabled)
			{
				stopWath.Stop();
				session.Factory.StatisticsImplementor.QueryExecuted("HQL: " + queryString, 0, stopWath.Elapsed);
				// NH: Different behavior (H3.2 use QueryLoader in AST parser) we need statistic for orginal query too.
				// probably we have a bug some where else for statistic RowCount
				session.Factory.StatisticsImplementor.QueryExecuted(QueryIdentifier, 0, stopWath.Elapsed);
			}
			return result;
		}

		public static string[] ConcreteQueries(string query, ISessionFactoryImplementor factory)
		{
			// TODO H3.2 check if the QuerySplitter can do the work (this method is not present in H3.2)

			//scan the query string for class names appearing in the from clause and replace 
			//with all persistent implementors of the class/interface, returning multiple 
			//query strings (make sure we don't pick up a class in the select clause!) 

			//TODO: this is one of the ugliest and most fragile pieces of code in Hibernate...
			string[] tokens = StringHelper.Split(ParserHelper.Whitespace + "(),", query, true);
			if (tokens.Length == 0)
			{
				return new String[] {query};
			} // just especially for the trivial collection filter 

			ArrayList placeholders = new ArrayList();
			ArrayList replacements = new ArrayList();
			StringBuilder templateQuery = new StringBuilder(40);
			int count = 0;
			string last = null;
			int nextIndex = 0;
			string next = null;
			templateQuery.Append(tokens[0]);
			for (int i = 1; i < tokens.Length; i++)
			{
				//update last non-whitespace token, if necessary
				if (!ParserHelper.IsWhitespace(tokens[i - 1]))
				{
					last = tokens[i - 1].ToLowerInvariant();
				}

				string token = tokens[i];
				if (!ParserHelper.IsWhitespace(token) || last == null)
				{
					// scan for the next non-whitespace token
					if (nextIndex <= i)
					{
						for (nextIndex = i + 1; nextIndex < tokens.Length; nextIndex++)
						{
							next = tokens[nextIndex].ToLowerInvariant();
							if (!ParserHelper.IsWhitespace(next))
							{
								break;
							}
						}
					}

					//if ( Character.isUpperCase( token.charAt( token.lastIndexOf(".") + 1 ) ) ) {
					// added the checks for last!=null and next==null because an ISet can not contain 
					// a null key in .net - it is valid for a null key to be in a java.util.Set
					if (
						((last != null && beforeClassTokens.Contains(last)) && (next == null || !notAfterClassTokens.Contains(next))) ||
						PathExpressionParser.EntityClass.Equals(last))
					{
						System.Type clazz = SessionFactoryHelper.GetImportedClass(factory, token);
						if (clazz != null)
						{
							string[] implementors = factory.GetImplementors(clazz.FullName);
							string placeholder = "$clazz" + count++ + "$";

							if (implementors != null)
							{
								placeholders.Add(placeholder);
								replacements.Add(implementors);
							}
							token = placeholder; //Note this!!
						}
					}
				}
				templateQuery.Append(token);
			}
			string[] results =
				StringHelper.Multiply(templateQuery.ToString(), placeholders.GetEnumerator(), replacements.GetEnumerator());
			if (results.Length == 0)
			{
				log.Warn("no persistent classes found for query class: " + query);
			}
			return results;
		}


		private static readonly ISet beforeClassTokens = new HashedSet();
		private static readonly ISet notAfterClassTokens = new HashedSet();

		/// <summary></summary>
		static QueryTranslator()
		{
			beforeClassTokens.Add("from");
			//beforeClassTokens.Add("new"); DEFINITELY DON'T HAVE THIS!!
			beforeClassTokens.Add(",");

			notAfterClassTokens.Add("in");
			//notAfterClassTokens.Add(",");
			notAfterClassTokens.Add("from");
			notAfterClassTokens.Add(")");
		}

		private static string[][] GenerateColumnNames(IType[] types, ISessionFactoryImplementor f)
		{
			string[][] names = new string[types.Length][];
			for (int i = 0; i < types.Length; i++)
			{
				int span = types[i].GetColumnSpan(f);
				names[i] = new string[span];
				for (int j = 0; j < span; j++)
				{
					names[i][j] = ScalarName(i, j);
				}
			}
			return names;
		}

		protected override object GetResultColumnOrRow(object[] row, IResultTransformer resultTransformer, IDataReader rs,
		                                               ISessionImplementor session)
		{
			IType[] _returnTypes = ReturnTypes;
			row = ToResultRow(row);
			bool hasTransform = holderClass != null || resultTransformer != null;
			if (hasScalars)
			{
				string[][] _names = ScalarColumnNames;
				int queryCols = _returnTypes.Length;
				if (holderClass == null && queryCols == 1)
				{
					return _returnTypes[0].NullSafeGet(rs, _names[0], session, null);
				}
				else
				{
					row = new object[queryCols];
					for (int i = 0; i < queryCols; i++)
					{
						row[i] = _returnTypes[i].NullSafeGet(rs, _names[i], session, null);
					}
					return row;
				}
			}
			else if (!hasTransform)
			{
				return (row.Length == 1) ? row[0] : row;
			}
			else
			{
				return row;
			}
		}

		protected override IList GetResultList(IList results, IResultTransformer resultTransformer)
		{
			HolderInstantiator holderInstantiator =
				HolderInstantiator.CreateClassicHolderInstantiator(holderConstructor, resultTransformer);

			if (holderInstantiator.IsRequired)
			{
				for (int i = 0; i < results.Count; i++)
				{
					object[] row = (object[]) results[i];
					results[i] = holderInstantiator.Instantiate(row);
				}

				if (holderConstructor == null && resultTransformer != null)
				{
					return resultTransformer.TransformList(results);
				}
			}

			return results;
		}

		private object[] ToResultRow(object[] row)
		{
			if (selectLength == row.Length)
			{
				return row;
			}
			else
			{
				object[] result = new object[selectLength];
				int j = 0;
				for (int i = 0; i < row.Length; i++)
				{
					if (includeInSelect[i])
					{
						result[j++] = row[i];
					}
				}
				return result;
			}
		}

		internal QueryJoinFragment CreateJoinFragment(bool useThetaStyleInnerJoins)
		{
			return new QueryJoinFragment(Factory.Dialect, useThetaStyleInnerJoins);
		}

		internal System.Type HolderClass
		{
			set { holderClass = value; }
		}

		protected internal override LockMode[] GetLockModes(IDictionary<string, LockMode> lockModes)
		{
			// unfortunately this stuff can't be cached because
			// it is per-invocation, not constant for the
			// QueryTranslator instance
			Dictionary<string, LockMode> nameLockModes = new Dictionary<string, LockMode>();
			if (lockModes != null)
			{
				foreach (KeyValuePair<string, LockMode> mode in lockModes)
				{
					nameLockModes[GetAliasName(mode.Key)] = mode.Value;
				}
			}
			LockMode[] lockModeArray = new LockMode[names.Length];
			for (int i = 0; i < names.Length; i++)
			{
				LockMode lm;
				if (!nameLockModes.TryGetValue(names[i], out lm))
				{
					lm = LockMode.None;
				}
				lockModeArray[i] = lm;
			}
			return lockModeArray;
		}

		protected override SqlString ApplyLocks(SqlString sql, IDictionary<string, LockMode> lockModes, Dialect.Dialect dialect)
		{
			SqlString result;
			if (lockModes == null || lockModes.Count == 0)
			{
				result = sql;
			}
			else
			{
				Dictionary<string, LockMode> aliasedLockModes = new Dictionary<string, LockMode>();
				foreach (KeyValuePair<string, LockMode> de in lockModes)
				{
					aliasedLockModes[GetAliasName(de.Key)] = de.Value;
				}

				Dictionary<string,string[]> keyColumnNames = null;
				if (dialect.ForUpdateOfColumns)
				{
					keyColumnNames = new Dictionary<string, string[]>();
					for (int i = 0; i < names.Length; i++)
					{
						keyColumnNames[names[i]] = persisters[i].IdentifierColumnNames;
					}
				}
				result = dialect.ApplyLocksToSql(sql, aliasedLockModes, keyColumnNames);
			}
			LogQuery(queryString, result.ToString());
			return result;
		}

		protected override bool UpgradeLocks()
		{
			return true;
		}

		protected override int[] CollectionOwners
		{
			get { return fetchedCollections.CollectionOwners; }
		}

		protected bool Compiled
		{
			get { return compiled; }
		}

		public override string ToString()
		{
			return queryString;
		}

		/// <summary></summary>
		protected override int[] Owners
		{
			get { return owners; }
		}

		public IDictionary<string, IFilter> EnabledFilters
		{
			get { return enabledFilters; }
		}

		public void AddFromJoinOnly(string name, JoinSequence joinSequence)
		{
			AddJoin(name, joinSequence.GetFromPart());
		}

		protected internal override bool IsSubselectLoadingEnabled
		{
			get { return HasSubselectLoadableCollections(); }
		}

		protected override string[] Aliases
		{
			get { return names; }
		}

		protected override EntityType[] OwnerAssociationTypes
		{
			get { return ownerAssociationTypes; }
		}

		#region IQueryTranslator Members

		public int ExecuteUpdate(QueryParameters queryParameters, ISessionImplementor session)
		{
			throw new NotSupportedException();
		}

		public string SQLString
		{
			get { return sqlString.ToString(); }
		}

		public IList<string> CollectSqlStrings
		{
			get
			{
				IList<string> result = new List<string>(1);
				result.Add(sqlString.ToString());
				return result;
			}
		}

		public string QueryString
		{
			get { return queryString; }
		}

		public string[] ReturnAliases
		{
			get { return NoReturnAliases; }
		}

		public string[][] GetColumnNames()
		{
			return scalarColumnNames;
		}

		public IParameterTranslations GetParameterTranslations()
		{
			return new ParameterTranslations(this);
		}

		public bool ContainsCollectionFetches
		{
			get { return false; }
		}

		public bool IsManipulationStatement
		{
			get
			{
				// classic parser does not support bulk manipulation statements
				return false;
			}
		}

		#endregion

		private class ParameterTranslations : IParameterTranslations
		{
			private readonly QueryTranslator queryTraslator;

			public ParameterTranslations(QueryTranslator queryTraslator)
			{
				this.queryTraslator = queryTraslator;
			}

			#region IParameterTranslations Members

			public bool SupportsOrdinalParameterMetadata
			{
				get { return false; }
			}

			public int OrdinalParameterCount
			{
				get { return 0; }
			}

			public int GetOrdinalParameterSqlLocation(int ordinalPosition)
			{
				return 0;
			}

			public IType GetOrdinalParameterExpectedType(int ordinalPosition)
			{
				return null;
			}

			public IEnumerable<string> GetNamedParameterNames()
			{
				return queryTraslator.namedParameters.Keys;
			}

			public int[] GetNamedParameterSqlLocations(string name)
			{
				return queryTraslator.GetNamedParameterLocs(name);
			}

			public IType GetNamedParameterExpectedType(string name)
			{
				return null;
			}

			#endregion
		}

		public override string QueryIdentifier
		{
			get { return queryIdentifier; }
		}

		internal void AddQuerySpaces(string[] spaces)
		{
			for (int i = 0; i < spaces.Length; i++)
			{
				querySpaces.Add(spaces[i]);
			}
			if (superQuery != null)
				superQuery.AddQuerySpaces(spaces);
		}

	}
}
