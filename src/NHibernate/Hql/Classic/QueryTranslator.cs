using System;
using System.Collections;
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Iesi.Collections;
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
using System.Text.RegularExpressions;

namespace NHibernate.Hql.Classic
{
	/// <summary> 
	/// An instance of <c>QueryTranslator</c> translates a Hibernate query string to SQL.
	/// </summary>
	public class QueryTranslator : BasicLoader, IFilterTranslator
	{
		private static string[] NoReturnAliases = new string[] {};

		private readonly string queryString;

		private readonly IDictionary typeMap = new SequencedHashMap();
		private readonly IDictionary collections = new SequencedHashMap();
		private IList returnedTypes = new ArrayList();
		private readonly IList fromTypes = new ArrayList();
		private readonly IList scalarTypes = new ArrayList();
		private readonly IDictionary namedParameters = new Hashtable();
		private readonly IDictionary aliasNames = new Hashtable();
		private readonly IDictionary oneToOneOwnerNames = new Hashtable();
		private readonly IDictionary uniqueKeyOwnerReferences = new Hashtable();
		private readonly IDictionary decoratedPropertyMappings = new Hashtable();

		private readonly IList scalarSelectTokens = new ArrayList(); // contains a List of strings
		private readonly IList whereTokens = new ArrayList(); // contains a List of strings containing Sql or SqlStrings
		private readonly IList havingTokens = new ArrayList();
		private readonly IDictionary joins = new SequencedHashMap();
		private readonly IList orderByTokens = new ArrayList();
		private readonly IList groupByTokens = new ArrayList();
		private readonly ISet querySpaces = new HashedSet();
		private readonly ISet entitiesToFetch = new HashedSet();

		private readonly IDictionary pathAliases = new Hashtable();
		private readonly IDictionary pathJoins = new Hashtable();

		private IQueryable[] persisters;
		private int[] owners;
		private EntityType[] ownerAssociationTypes;
		private string[] names;
		private bool[] includeInSelect;
		private int selectLength;
		private IType[] returnTypes;
		private IType[] actualReturnTypes;
		private string[][] scalarColumnNames;
		private IDictionary tokenReplacements;
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
			private ArrayList persisters; // IQueryableCollection
			private ArrayList names; // string
			private ArrayList ownerNames; // string
			private ArrayList suffixes; // string

			// True if one of persisters represents a collection that is not safe to use in multiple join scenario.
			// Currently every collection except bag is safe.
			private bool hasUnsafeCollection = false;

			private ArrayList ownerColumns; // int

			private static string GenerateSuffix(int count)
			{
				return count + "__";
			}

			private static bool IsUnsafe(IQueryableCollection collectionPersister)
			{
				return collectionPersister.CollectionType is BagType
				       || collectionPersister.CollectionType is IdentifierBagType;
			}

			public void Add(string name, IQueryableCollection collectionPersister, string ownerName)
			{
				if (persisters == null)
				{
					persisters = new ArrayList(2);
					names = new ArrayList(2);
					ownerNames = new ArrayList(2);
					suffixes = new ArrayList(2);
				}

				count++;

				hasUnsafeCollection = hasUnsafeCollection || IsUnsafe(collectionPersister);

				if (count > 1 && hasUnsafeCollection)
				{
					// The comment only mentions a bag since I don't want to confuse users.
					throw new QueryException("Cannot fetch multiple collections in a single query if one of them is a bag");
				}

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
					return (int[]) ownerColumns.ToArray(typeof(int));
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
					return (ICollectionPersister[]) persisters.ToArray(typeof(ICollectionPersister));
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
					return (string[]) suffixes.ToArray(typeof(string));
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
					sql.AddSelectFragmentString(
						((IQueryableCollection) persisters[i]).SelectFragment(
							(string) names[i], (string) suffixes[i]));
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
						sql.AddOrderBy(persister.GetSQLOrderByString((string) names[i]));
					}
				}
			}

			public void InitializeCollectionOwnerColumns(IList returnedTypes)
			{
				if (count == 0)
				{
					return;
				}

				ownerColumns = new ArrayList(count);
				for (int i = 0; i < count; i++)
				{
					string ownerName = (string) ownerNames[i];
					// This is quite slow in theory but there should only be a few collections
					// so it shouldn't be a problem in practice.
					int ownerIndex = returnedTypes.IndexOf(ownerName);
					ownerColumns.Add(ownerIndex);
				}
			}
		}

		private FetchedCollections fetchedCollections = new FetchedCollections();

		private string[] suffixes;

		private IDictionary enabledFilters;

		private static readonly ILog log = LogManager.GetLogger(typeof(QueryTranslator));

		/// <summary> 
		/// Construct a query translator
		/// </summary>
		public QueryTranslator(ISessionFactoryImplementor factory, string queryString, IDictionary enabledFilters)
			: base(factory)
		{
			this.queryString = queryString;
			this.enabledFilters = enabledFilters;
		}

		/// <summary>
		/// Compile a subquery
		/// </summary>
		/// <param name="superquery"></param>
		protected internal void Compile(QueryTranslator superquery)
		{
			this.tokenReplacements = superquery.tokenReplacements;
			this.superQuery = superquery;
			this.shallowQuery = true;
			this.enabledFilters = superquery.EnabledFilters;

			Compile();
		}

		/// <summary>
		/// Compile a "normal" query. This method may be called multiple
		/// times. Subsequent invocations are no-ops.
		/// </summary>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void Compile(IDictionary replacements, bool scalar)
		{
			if (!Compiled)
			{
				this.tokenReplacements = replacements;
				this.shallowQuery = scalar;

				Compile();
			}
		}

		/// <summary>
		/// Compile a filter. This method may be called multiple
		/// times. Subsequent invocations are no-ops.
		/// </summary>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void Compile(string collectionRole, IDictionary replacements, bool scalar)
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

		public new object LoadSingleRow(IDataReader resultSet, ISessionImplementor session, QueryParameters queryParameters,
		                                bool returnProxies)
		{
			return base.LoadSingleRow(resultSet, session, queryParameters, returnProxies);
		}

		/// <summary>
		/// Persisters for the return values of a <c>Find</c> style query
		/// </summary>
		/// <remarks>
		/// The <c>Persisters</c> stored by QueryTranslator have to be <see cref="IQueryable"/>.  The
		/// <c>setter</c> will attempt to cast the <c>ILoadable</c> array passed in into an 
		/// <c>IQueryable</c> array.
		/// </remarks>
		protected internal override ILoadable[] EntityPersisters
		{
			get { return persisters; }
			set { persisters = (IQueryable[]) value; }
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

		private void LogQuery(string hql, string sql)
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
			string name = (string) aliasNames[alias];
			if (name == null)
			{
				if (superQuery != null)
				{
					name = superQuery.GetAliasName(alias);
				}
				else
				{
					name = alias;
				}
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
			set { throw new InvalidOperationException("QueryTranslator.SqlString is read-only"); }
		}

		private int NextCount()
		{
			return (superQuery == null) ? nameCount++ : superQuery.nameCount++;
		}

		internal string CreateNameFor(System.Type type)
		{
			return StringHelper.GenerateAlias(type.Name, NextCount());
		}

		internal string CreateNameForCollection(string role)
		{
			return StringHelper.GenerateAlias(role, NextCount());
		}

		internal System.Type GetType(string name)
		{
			System.Type type = (System.Type) typeMap[name];
			if (type == null && superQuery != null)
			{
				type = superQuery.GetType(name);
			}
			return type;
		}

		internal string GetRole(string name)
		{
			string role = (string) collections[name];
			if (role == null && superQuery != null)
			{
				role = superQuery.GetRole(name);
			}
			return role;
		}

		internal bool IsName(string name)
		{
			return aliasNames.Contains(name) ||
			       typeMap.Contains(name) ||
			       collections.Contains(name) ||
			       (superQuery != null && superQuery.IsName(name));
		}

		public IPropertyMapping GetPropertyMapping(string name)
		{
			IPropertyMapping decorator = GetDecoratedPropertyMapping(name);
			if (decorator != null)
			{
				return decorator;
			}

			System.Type type = GetType(name);
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
					throw new QueryException(string.Format("persistent class not found: {0}", type.Name));
				}
				return persister;
			}
		}

		public IPropertyMapping GetDecoratedPropertyMapping(string name)
		{
			return (IPropertyMapping) decoratedPropertyMappings[name];
		}

		public void DecoratePropertyMapping(string name, IPropertyMapping mapping)
		{
			decoratedPropertyMappings.Add(name, mapping);
		}

		internal IQueryable GetPersisterForName(string name)
		{
			System.Type type = GetType(name);
			IQueryable persister = GetPersister(type);
			if (persister == null)
			{
				throw new QueryException("persistent class not found: " + type.Name);
			}

			return persister;
		}

		internal IQueryable GetPersisterUsingImports(string className)
		{
			return SessionFactoryHelper.FindQueryableUsingImports(Factory, className);
		}

		internal IQueryable GetPersister(System.Type clazz)
		{
			try
			{
				return (IQueryable) Factory.GetEntityPersister(clazz);
			}
			catch (Exception)
			{
				throw new QueryException("persistent class not found: " + clazz.Name);
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

		internal void AddType(string name, System.Type type)
		{
			typeMap.Add(name, type);
		}

		internal void AddCollection(string name, string role)
		{
			collections.Add(name, role);
		}

		internal void AddFrom(string name, System.Type type, JoinSequence joinSequence)
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
			AddFrom(name, classPersister.MappedClass, joinSequence);
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
			orderByTokens.Add(token);
		}

		internal void AppendOrderByParameter()
		{
			orderByTokens.Add(SqlString.Parameter);
		}

		internal void AppendGroupByToken(string token)
		{
			groupByTokens.Add(token);
		}

		internal void AppendScalarSelectToken(string token)
		{
			scalarSelectTokens.Add(token);
		}

		internal void AppendScalarSelectTokens(string[] tokens)
		{
			scalarSelectTokens.Add(tokens);
		}

		internal void AddJoin(string name, JoinSequence joinSequence)
		{
			if (!joins.Contains(name))
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
			object o = namedParameters[name];
			if (o == null)
			{
				namedParameters.Add(name, loc);
			}
			else if (o is int)
			{
				ArrayList list = new ArrayList(4);
				list.Add(o);
				list.Add(loc);
				namedParameters[name] = list;
			}
			else
			{
				((ArrayList) o).Add(loc);
			}
		}

		public override int[] GetNamedParameterLocs(string name)
		{
			object o = namedParameters[name];
			if (o == null)
			{
				QueryException qe = new QueryException("Named parameter does not appear in Query: " + name);
				qe.QueryString = queryString;
				throw qe;
			}
			if (o is int)
			{
				return new int[] {((int) o)};
			}
			else
			{
				return ArrayHelper.ToIntArray((ArrayList) o);
			}
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
				string name = (string) returnedTypes[i];
				//if ( !IsName(name) ) throw new QueryException("unknown type: " + name);
				persisters[i] = GetPersisterForName(name);
				suffixes[i] = (size == 1) ? String.Empty : i.ToString() + StringHelper.Underscore;
				names[i] = name;
				includeInSelect[i] = !entitiesToFetch.Contains(name);
				if (includeInSelect[i])
				{
					selectLength++;
				}
				string oneToOneOwner = (string) oneToOneOwnerNames[name];
				owners[i] = oneToOneOwner == null ? -1 : returnedTypes.IndexOf(oneToOneOwner);
				ownerAssociationTypes[i] = (EntityType) uniqueKeyOwnerReferences[name];
			}

			fetchedCollections.InitializeCollectionOwnerColumns(returnedTypes);

			if (ArrayHelper.IsAllNegative(owners))
			{
				owners = null;
			}

			string scalarSelect = RenderScalarSelect(); //Must be done here because of side-effect! yuck...

			int scalarSize = scalarTypes.Count;
			hasScalars = scalarTypes.Count != rtsize;

			returnTypes = new IType[scalarSize];
			for (int i = 0; i < scalarSize; i++)
			{
				returnTypes[i] = (IType) scalarTypes[i];
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
			sql.SetWhereTokens(whereTokens);

			RenderFunctions(groupByTokens);
			sql.SetGroupByTokens(groupByTokens);

			RenderFunctions(havingTokens);
			sql.SetHavingTokens(havingTokens);

			RenderFunctions(orderByTokens);
			sql.SetOrderByTokens(orderByTokens);

			fetchedCollections.AddOrderBy(sql);

			scalarColumnNames = GenerateColumnNames(returnTypes, Factory);

			// initialize the set of queried identifer spaces (ie. tables)
			foreach (string name in collections.Values)
			{
				ICollectionPersister p = GetCollectionPersister(name);
				AddQuerySpace(p.CollectionSpace);
			}
			foreach (string name in typeMap.Keys)
			{
				IQueryable p = GetPersisterForName(name);
				object[] spaces = p.PropertySpaces;
				for (int i = 0; i < spaces.Length; i++)
				{
					AddQuerySpace(spaces[i]);
				}
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
						actualReturnTypes[j++] = NHibernateUtil.Entity(persisters[i].MappedClass);
					}
				}
			}
		}

		private void RenderIdentifierSelect(QuerySelect sql)
		{
			int size = returnedTypes.Count;

			for (int k = 0; k < size; k++)
			{
				string name = (string) returnedTypes[k];
				string suffix = size == 1 ? String.Empty : k.ToString() + StringHelper.Underscore;
				sql.AddSelectFragmentString(persisters[k].IdentifierSelectFragment(name, suffix));
			}
		}

		private void RenderPropertiesSelect(QuerySelect sql)
		{
			int size = returnedTypes.Count;
			for (int k = 0; k < size; k++)
			{
				string suffix = (size == 1) ? String.Empty : k.ToString() + StringHelper.Underscore;
				string name = (string) returnedTypes[k];
				sql.AddSelectFragmentString(persisters[k].PropertySelectFragment(name, suffix));
			}
		}

		/// <summary> 
		/// WARNING: side-effecty
		/// </summary>
		private string RenderScalarSelect()
		{
			bool isSubselect = superQuery != null;

			StringBuilder buf = new StringBuilder(20);

			if (scalarTypes.Count == 0)
			{
				//ie. no select clause
				int size = returnedTypes.Count;
				for (int k = 0; k < size; k++)
				{
					scalarTypes.Add(NHibernateUtil.Entity(persisters[k].MappedClass));

					string[] names = persisters[k].IdentifierColumnNames;
					for (int i = 0; i < names.Length; i++)
					{
						buf.Append(returnedTypes[k]).Append(StringHelper.Dot).Append(names[i]);
						if (!isSubselect)
						{
							buf.Append(" as ").Append(ScalarName(k, i));
						}
						if (i != names.Length - 1 || k != size - 1)
						{
							buf.Append(StringHelper.CommaSpace);
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
					object next = scalarSelectTokens[tokenIdx];
					if (next is string)
					{
						string token = (string)next;
						string lc = token.ToLower(CultureInfo.InvariantCulture);
						ISQLFunction func = Factory.SQLFunctionRegistry.FindSQLFunction(lc);
						if (func != null)
						{
							// Render the HQL function
							string renderedFunction = RenderFunctionClause(func, scalarSelectTokens, ref tokenIdx);
							buf.Append(renderedFunction);
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
										buf.Append(" as ").Append(ScalarName(c++, 0));
									}
								}
							}

							buf.Append(token);
							if (lc.Equals("distinct") || lc.Equals("all"))
							{
								buf.Append(' ');
							}
						}
					}
					else
					{
						nolast = true;
						string[] tokens = (string[])next;
						for (int i = 0; i < tokens.Length; i++)
						{
							buf.Append(tokens[i]);
							if (!isSubselect)
							{
								buf.Append(" as ").Append(ScalarName(c, i));
							}
							if (i != tokens.Length - 1)
							{
								buf.Append(StringHelper.CommaSpace);
							}
						}
						c++;
					}
				}
				if (!isSubselect && !nolast)
				{
					buf.Append(" as ").Append(ScalarName(c++, 0));
				}
			}

			return buf.ToString();
		}

		private class FunctionPlaceHolder
		{
			public readonly int startToken;
			public readonly int tokensCount;
			public readonly string renderedFunction;
			public FunctionPlaceHolder(int startToken, int tokensCount, string renderedFunction)
			{
				this.startToken = startToken;
				this.tokensCount = tokensCount;
				this.renderedFunction = renderedFunction;
			}
		}

		// Parameters inside function are not supported
		private void RenderFunctions(IList tokens)
		{
			for (int tokenIdx = 0; tokenIdx < tokens.Count; tokenIdx++)
			{
				string token = tokens[tokenIdx].ToString().ToLower(CultureInfo.InvariantCulture);
				ISQLFunction func = Factory.SQLFunctionRegistry.FindSQLFunction(token);
				if (func != null)
				{
					int flTokenIdx = tokenIdx;
					string renderedFunction = RenderFunctionClause(func, tokens, ref flTokenIdx);
					// At this point we have the trunk that represent the function with it's parameters enclosed
					// in paren. Now all token in the tokens list can be removed from original list because they must
					// be replased with the rendered function.
					for (int i = 1; i <= (flTokenIdx - tokenIdx); i++)
						tokens.RemoveAt(tokenIdx+1);
					tokens[tokenIdx] = renderedFunction;
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
		private StringCollection ExtractFunctionClause(IList tokens, ref int tokenIdx)
		{
			string funcName = tokens[tokenIdx].ToString();
			StringCollection functionTokens = new StringCollection();
			functionTokens.Add(funcName);
			tokenIdx++;
			if (tokenIdx >= tokens.Count ||
				!StringHelper.OpenParen.Equals(tokens[tokenIdx].ToString()))
			{
				// All function with parameters have the syntax
				// <function name> <left paren> <parameters> <right paren>
				throw new QueryException("'(' expected after function " + funcName);
			}
			functionTokens.Add(StringHelper.OpenParen);
			tokenIdx++;
			int parenCount = 1;
			for (; tokenIdx < tokens.Count && parenCount > 0; tokenIdx++)
			{
				if (!(tokens[tokenIdx] is string) && !(tokens[tokenIdx] is SqlString))
				{
					// Only to protect this method from unmanaged types
					throw new QueryException(string.Format("The function {0} have not supported parameters list. The parameter is {1}"
						, funcName, tokens[tokenIdx].GetType().AssemblyQualifiedName), new NotSupportedException());
				}
				if (tokens[tokenIdx].ToString().StartsWith(StringHelper.NamePrefix) || tokens[tokenIdx].ToString().Equals(StringHelper.SqlParameter))
				{
					throw new QueryException(string.Format("Parameters inside function are not supported (function '{0}').", funcName),
						new NotSupportedException());
				}
				functionTokens.Add(tokens[tokenIdx].ToString());
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

		private string RenderFunctionClause(ISQLFunction func, IList tokens, ref int tokenIdx)
		{
			StringCollection functionTokens;
			if (!func.HasArguments)
			{
				// The function don't work with arguments.
				if (func.HasParenthesesIfNoArguments)
					functionTokens = ExtractFunctionClause(tokens, ref tokenIdx);

				// The function render simply translate is't name for a specific dialect.
				return func.Render(null, Factory);
			}
			functionTokens = ExtractFunctionClause(tokens, ref tokenIdx);

			IFunctionGrammar fg = func as IFunctionGrammar;
			if (fg == null)
				fg = new CommonGrammar();

			StringCollection args = new StringCollection();
			StringBuilder argBuf = new StringBuilder(20);
			// Extract args spliting first 2 token because are: FuncName(
			// last token is ')'
			// To allow expressions like arg (ex:5+5) all tokens between 'argument separator' or
			// a 'know argument' are compacted in a string, 
			// because many HQL function expect IList<string> like args in Render method.
			// This solution give us the ability to use math expression in common function. 
			// Ex: sum(a.Prop+10), cast(yesterday-1 as date)
			for (int argIdx = 2; argIdx < functionTokens.Count - 1; argIdx++)
			{
				string token = functionTokens[argIdx];
				if(fg.IsKnownArgument(token))
				{
					if (argBuf.Length > 0)
					{
						// end of the previous argument
						args.Add(argBuf.ToString());
						argBuf = new StringBuilder(20);
					}
					args.Add(token);
				}
				else if (fg.IsSeparator(token))
				{
					// argument end
					if (argBuf.Length > 0)
					{
						args.Add(argBuf.ToString());
						argBuf = new StringBuilder(20);
					}
				}
				else
				{
					ISQLFunction nfunc = Factory.SQLFunctionRegistry.FindSQLFunction(token.ToLower(CultureInfo.InvariantCulture));
					if (nfunc != null)
					{
						// the token is a nested function call
						argBuf.Append(RenderFunctionClause(nfunc, functionTokens, ref argIdx));
					}
					else
					{
						// the token is a part of an argument (every thing else)
						argBuf.Append(token);
					}
				}
			}
			// Add the last arg
			if (argBuf.Length > 0)
				args.Add(argBuf.ToString());
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
			foreach (DictionaryEntry de in joins)
			{
				string name = (string) de.Key;
				JoinSequence join = (JoinSequence) de.Value;
				join.SetSelector(new Selector(this));

				if (typeMap.Contains(name))
				{
					ojf.AddFragment(join.ToJoinFragment(enabledFilters, true));
				}
				else if (collections.Contains(name))
				{
					ojf.AddFragment(join.ToJoinFragment(enabledFilters, true));
				}
				else
				{
					//name from a super query (a bit inelegant that it shows up here)
				}
			}
		}

		public ISet QuerySpaces
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

		internal void AddQuerySpace(object table)
		{
			querySpaces.Add(table);
			if (superQuery != null)
			{
				superQuery.AddQuerySpace(table);
			}
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
			set { throw new InvalidOperationException("QueryTranslator.CollectionSuffixes_set"); }
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
			set { suffixes = value; }
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

				IQueryable p = (IQueryable) persister.ElementPersister;
				string[] idColumnNames = p.IdentifierColumnNames;
				string[] eltColumnNames = persister.ElementColumnNames;
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
			AddFrom(elementName, elmType.AssociatedClass, join);
		}

		internal string GetPathAlias(string path)
		{
			return (string) pathAliases[path];
		}

		internal JoinSequence GetPathJoin(string path)
		{
			return (JoinSequence) pathJoins[path];
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
			IDbCommand cmd = PrepareQueryCommand(parameters, false, session);

			// This IDataReader is disposed of in EnumerableImpl.Dispose
			IDataReader rs = GetResultSet(cmd, parameters.RowSelection, session);
			HolderInstantiator hi =
				HolderInstantiator.CreateClassicHolderInstantiator(holderConstructor, parameters.ResultTransformer);
			return new EnumerableImpl(rs, cmd, session, ReturnTypes, ScalarColumnNames, parameters.RowSelection,
			                          hi);
		}

		public static string[] ConcreteQueries(string query, ISessionFactoryImplementor factory)
		{
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
					last = tokens[i - 1].ToLower(CultureInfo.InvariantCulture);
				}

				string token = tokens[i];
				if (!ParserHelper.IsWhitespace(token) || last == null)
				{
					// scan for the next non-whitespace token
					if (nextIndex <= i)
					{
						for (nextIndex = i + 1; nextIndex < tokens.Length; nextIndex++)
						{
							next = tokens[nextIndex].ToLower(CultureInfo.InvariantCulture);
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
							string[] implementors = factory.GetImplementors(clazz);
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
			IType[] returnTypes = ReturnTypes;
			row = ToResultRow(row);
			if (hasScalars)
			{
				string[][] names = ScalarColumnNames;
				int queryCols = returnTypes.Length;
				if (holderClass == null && queryCols == 1)
				{
					return returnTypes[0].NullSafeGet(rs, names[0], session, null);
				}
				else
				{
					row = new object[queryCols];
					for (int i = 0; i < queryCols; i++)
					{
						row[i] = returnTypes[i].NullSafeGet(rs, names[i], session, null);
					}
					return row;
				}
			}
			else if (holderClass == null)
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

		protected internal override LockMode[] GetLockModes(IDictionary lockModes)
		{
			// unfortunately this stuff can't be cached because
			// it is per-invocation, not constant for the
			// QueryTranslator instance
			IDictionary nameLockModes = new Hashtable();
			if (lockModes != null)
			{
				IDictionaryEnumerator it = lockModes.GetEnumerator();
				while (it.MoveNext())
				{
					DictionaryEntry me = it.Entry;
					nameLockModes.Add(
						GetAliasName((String) me.Key),
						me.Value
						);
				}
			}
			LockMode[] lockModeArray = new LockMode[names.Length];
			for (int i = 0; i < names.Length; i++)
			{
				LockMode lm = (LockMode) nameLockModes[names[i]];
				if (lm == null)
				{
					lm = LockMode.None;
				}
				lockModeArray[i] = lm;
			}
			return lockModeArray;
		}

		protected override SqlString ApplyLocks(SqlString sql, IDictionary lockModes, Dialect.Dialect dialect)
		{
			SqlString result;
			if (lockModes == null || lockModes.Count == 0)
			{
				result = sql;
			}
			else
			{
				IDictionary aliasedLockModes = new Hashtable();
				foreach (DictionaryEntry de in lockModes)
				{
					aliasedLockModes[GetAliasName((string) de.Key)] = de.Value;
				}

				IDictionary keyColumnNames = null;
				if (dialect.ForUpdateOfColumns)
				{
					keyColumnNames = new Hashtable();
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
			set { owners = value; }
		}

		public IDictionary EnabledFilters
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

		public IList CollectSqlStrings
		{
			get
			{
				IList result = new ArrayList(1);
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

			public IList GetNamedParameterNames()
			{
				return new ArrayList(queryTraslator.namedParameters.Keys);
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
	}
}