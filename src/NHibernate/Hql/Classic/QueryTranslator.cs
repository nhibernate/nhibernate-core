using System;
using System.Collections;
using System.Data;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Iesi.Collections;
using log4net;
using NHibernate.Engine;
using NHibernate.Impl;
using NHibernate.Loader;
using NHibernate.Persister.Collection;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;
using NHibernate.Transform;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Hql.Classic
{
	/// <summary> 
	/// An instance of <c>QueryTranslator</c> translates a Hibernate query string to SQL.
	/// </summary>
	public class QueryTranslator : BasicLoader, IFilterTranslator
	{
		private static string[] NoReturnAliases = new string[] { };

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

		private IQueryableCollection collectionPersister;
		private int collectionOwnerColumn = -1;
		private string collectionOwnerName;
		private string fetchName;

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

		public new object LoadSingleRow(IDataReader resultSet, ISessionImplementor session, QueryParameters queryParameters, bool returnProxies)
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
		protected override ILoadable[] EntityPersisters
		{
			get { return persisters; }
			set { persisters = (IQueryable[])value; }
		}

		/// <summary>
		///Types of the return values of an <c>Enumerate()</c> style query.
		///Return an array of <see cref="IType" />s.
		/// </summary>
		public virtual IType[] ReturnTypes
		{
			get { return returnTypes; }
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
			string name = (string)aliasNames[alias];
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
		[CLSCompliant(false)] // TODO: Work out why this causes an error in 1.1 - the variable sqlString is private so we're only exposing one name
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
			System.Type type = (System.Type)typeMap[name];
			if (type == null && superQuery != null)
			{
				type = superQuery.GetType(name);
			}
			return type;
		}

		internal string GetRole(string name)
		{
			string role = (string)collections[name];
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
			return (IPropertyMapping)decoratedPropertyMappings[name];
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
			// Slightly altered from H2.1 to avoid needlessly throwing
			// and catching a MappingException.
			return (IQueryable)Factory.GetEntityPersister(
				Factory.GetImportedClassName(className),
				false);
		}

		internal IQueryable GetPersister(System.Type clazz)
		{
			try
			{
				return (IQueryable)Factory.GetEntityPersister(clazz);
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
				return (IQueryableCollection)Factory.GetCollectionPersister(role);
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

		internal void AppendWhereToken(string token)
		{
			whereTokens.Add(token);
		}

		internal void AppendWhereToken(SqlString token)
		{
			whereTokens.Add(token);
		}

		internal void AppendHavingToken(string token)
		{
			havingTokens.Add(token);
		}

		internal void AppendOrderByToken(string token)
		{
			orderByTokens.Add(token);
		}

		internal void AppendOrderByParameter()
		{
			orderByTokens.Add(new SqlString(Parameter.Placeholder));
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
				((ArrayList)o).Add(loc);
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
				return new int[] { ((int)o) };
			}
			else
			{
				return ArrayHelper.ToIntArray((ArrayList)o);
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
				string name = (string)returnedTypes[i];
				//if ( !IsName(name) ) throw new QueryException("unknown type: " + name);
				persisters[i] = GetPersisterForName(name);
				suffixes[i] = (size == 1) ? String.Empty : i.ToString() + StringHelper.Underscore;
				names[i] = name;
				includeInSelect[i] = !entitiesToFetch.Contains(name);
				if (includeInSelect[i])
				{
					selectLength++;
				}
				if (name.Equals(collectionOwnerName))
				{
					collectionOwnerColumn = i;
				}
				string oneToOneOwner = (string)oneToOneOwnerNames[name];
				owners[i] = oneToOneOwner == null ? -1 : returnedTypes.IndexOf(oneToOneOwner);
				ownerAssociationTypes[i] = (EntityType)uniqueKeyOwnerReferences[name];
			}

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
				returnTypes[i] = (IType)scalarTypes[i];
			}

			QuerySelect sql = new QuerySelect(Factory.Dialect);
			sql.Distinct = distinct;

			if (!shallowQuery)
			{
				RenderIdentifierSelect(sql);
				RenderPropertiesSelect(sql);
			}

			if (collectionPersister != null)
			{
				sql.AddSelectFragmentString(collectionPersister.SelectFragment(fetchName, "__"));
			}
			if (hasScalars || shallowQuery)
			{
				sql.AddSelectFragmentString(scalarSelect);
			}

			// TODO: for some dialects it would be appropriate to add the renderOrderByPropertiesSelect() to other select strings
			MergeJoins(sql.JoinFragment);

			sql.SetWhereTokens(whereTokens);

			sql.SetGroupByTokens(groupByTokens);
			sql.SetHavingTokens(havingTokens);
			sql.SetOrderByTokens(orderByTokens);

			if (collectionPersister != null && collectionPersister.HasOrdering)
			{
				sql.AddOrderBy(collectionPersister.GetSQLOrderByString(fetchName));
			}

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
				string name = (string)returnedTypes[k];
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
				string name = (string)returnedTypes[k];
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
				int parenCount = 0;  // used to count the nesting of parentheses
				foreach (object next in scalarSelectTokens)
				{
					if (next is string)
					{
						string token = (string)next;
						string lc = token.ToLower(System.Globalization.CultureInfo.InvariantCulture);

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
				string name = (string)de.Key;
				JoinSequence join = (JoinSequence)de.Value;
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
			get
			{
				if (collectionPersister == null)
				{
					return null;
				}
				return new ICollectionPersister[] { collectionPersister };
			}
		}

		protected override string[] CollectionSuffixes
		{
			get { return collectionPersister == null ? null : new string[] { "__" }; }
			set { throw new InvalidOperationException("QueryTranslator.CollectionSuffixes_set"); }
		}

		public void SetCollectionToFetch(string role, string name, string ownerName, string entityName)
		{
			if (fetchName != null)
			{
				throw new QueryException("cannot fetch multiple collections in one query");
			}

			fetchName = name;
			collectionPersister = GetCollectionPersister(role);
			collectionOwnerName = ownerName;
			if (collectionPersister.ElementType.IsEntityType)
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

				IQueryable p = (IQueryable)persister.ElementPersister;
				string[] idColumnNames = p.IdentifierColumnNames;
				string[] eltColumnNames = persister.ElementColumnNames;
				try
				{
					join.AddJoin(
						(IAssociationType)persister.ElementType,
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
			EntityType elmType = (EntityType)collectionElementType;
			AddFrom(elementName, elmType.AssociatedClass, join);
		}

		internal string GetPathAlias(string path)
		{
			return (string)pathAliases[path];
		}

		internal JoinSequence GetPathJoin(string path)
		{
			return (JoinSequence)pathJoins[path];
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
			HolderInstantiator hi = HolderInstantiator.CreateClassicHolderInstantiator(holderConstructor, parameters.ResultTransformer);
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
				return new String[] { query };
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
					last = tokens[i - 1].ToLower(System.Globalization.CultureInfo.InvariantCulture);
				}

				string token = tokens[i];
				if (!ParserHelper.IsWhitespace(token) || last == null)
				{
					// scan for the next non-whitespace token
					if (nextIndex <= i)
					{
						for (nextIndex = i + 1; nextIndex < tokens.Length; nextIndex++)
						{
							next = tokens[nextIndex].ToLower(System.Globalization.CultureInfo.InvariantCulture);
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
						System.Type clazz = GetImportedClass(token, factory);
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
			string[] results = StringHelper.Multiply(templateQuery.ToString(), placeholders.GetEnumerator(), replacements.GetEnumerator());
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

		/// <summary>
		/// Gets the Type for the name that might be an Imported Class.
		/// </summary>
		/// <param name="name">The name that might be an ImportedClass.</param>
		/// <returns>A <see cref="System.Type"/> if <c>name</c> is an Imported Class, <c>null</c> otherwise.</returns>
		internal System.Type GetImportedClass(string name)
		{
			return GetImportedClass(name, Factory);
		}

		/// <summary>
		/// Gets the Type for the name that might be an Imported Class.
		/// </summary>
		/// <param name="name">The name that might be an ImportedClass.</param>
		/// <param name="factory">The <see cref="ISessionFactoryImplementor"/> that contains the Imported Classes.</param>
		/// <returns>A <see cref="System.Type"/> if <c>name</c> is an Imported Class, <c>null</c> otherwise.</returns>
		private static System.Type GetImportedClass(string name, ISessionFactoryImplementor factory)
		{
			string importedName = factory.GetImportedClassName(name);

			// don't care about the exception, just give us a null value.
			return System.Type.GetType(importedName, false);
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

		protected override object GetResultColumnOrRow(object[] row, IResultTransformer resultTransformer, IDataReader rs, ISessionImplementor session)
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
					object[] row = (object[])results[i];
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

		protected override LockMode[] GetLockModes(IDictionary lockModes)
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
						GetAliasName((String)me.Key),
						me.Value
						);
				}
			}
			LockMode[] lockModeArray = new LockMode[names.Length];
			for (int i = 0; i < names.Length; i++)
			{
				LockMode lm = (LockMode)nameLockModes[names[i]];
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
					aliasedLockModes[GetAliasName((string)de.Key)] = de.Value;
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
				result = sql.Append(new ForUpdateFragment(dialect, aliasedLockModes, keyColumnNames).ToSqlStringFragment());
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
			get { return new int[] { collectionOwnerColumn }; }
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
		
		protected override bool IsSubselectLoadingEnabled
		{
			get { return HasSubselectLoadableCollections(); }
		}

		protected override string[] Aliases
		{
			get { return names; }
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
