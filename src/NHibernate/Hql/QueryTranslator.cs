using System;
using System.Collections;
using System.Collections.Specialized;
using System.Data;
using System.Text;
using System.Reflection;
using System.Runtime.Serialization;
using NHibernate;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Impl;
using NHibernate.Loader;
using NHibernate.Persister;
using NHibernate.Sql;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Util;
using BaseLoader = NHibernate.Loader.Loader;
using System.Runtime.CompilerServices;

namespace NHibernate.Hql 
{
	/// <summary> 
	/// An instance of <c>QueryTranslator</c> translates a Hibernate query string to SQL.
	/// </summary>
	public class QueryTranslator : BaseLoader 
	{
		private static readonly log4net.ILog log  = log4net.LogManager.GetLogger(typeof(QueryTranslator));
		private static StringCollection dontSpace = new StringCollection();

		private IDictionary typeMap = new SequencedHashMap();
		private IDictionary collections = new SequencedHashMap();
		private IList returnTypes = new ArrayList();
		private IList fromTypes = new ArrayList();
		private IList scalarTypes = new ArrayList();
		private IDictionary namedParameters = new Hashtable();
		private IDictionary aliasNames = new Hashtable();
		private ArrayList crossJoins = new ArrayList();

		private IList scalarSelectTokens = new ArrayList();
		private IList whereTokens = new ArrayList();
		private IList havingTokens = new ArrayList();
		private IDictionary joins = new Hashtable();
		private IList orderByTokens = new ArrayList();
		private IList groupByTokens = new ArrayList();
		private ArrayList identifierSpaces = new ArrayList();
		private ArrayList entitiesToFetch = new ArrayList();

		private IQueryable[] persisters;
		private string[] names;
		private bool[] includeInSelect;
		private IType[] types;
		private int selectLength;
		private string[][] scalarColumnNames;
		
		//--- PORT NOTE ---
		//original modifier was protected
		//I change in internal because Hql.SelectParser.AggregateType use factory.

		internal ISessionFactoryImplementor factory;

		//--- END NOTE ---
		private IDictionary replacements;
		private int count = 0;
		private int parameterCount = 0;
		private string queryString;
		private bool distinct = false;
		protected bool compiled;
		private string sqlString;
		private System.Type holderClass;
		private ConstructorInfo holderConstructor;
		private bool hasScalars;
		private bool shallowQuery;
		private QueryTranslator superQuery;
		
		private CollectionPersister collectionPersister;
		private int collectionOwnerColumn = -1;
		private string collectionOwnerName;
		private string fetchName;

		private string[] suffixes;

		/// <summary> 
		/// Construct a query translator
		/// </summary>
		public QueryTranslator(Dialect.Dialect d) : base(d)
		{
		}

		/// <summary>
		/// Compile a subquery
		/// </summary>
		/// <param name="superquery"></param>
		/// <param name="queryString"></param>
		internal protected void Compile(QueryTranslator superquery, string queryString) 
		{
			this.factory = superquery.factory;
			this.replacements = superquery.replacements;
			this.superQuery = superquery;
			this.shallowQuery = true;

			Compile(queryString);
		}

		/// <summary>
		/// Compile a "normal" query. This method may be called multiple
		/// times. Subsequent invocations are no-ops.
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="queryString"></param>
		/// <param name="replacements"></param>
		/// <param name="scalar"></param>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void Compile(ISessionFactoryImplementor factory, string queryString, IDictionary replacements, bool scalar) 
		{
			if (!compiled) 
			{
				this.factory = factory;
				this.replacements = replacements;
				this.shallowQuery = scalar;
				Compile(queryString);
			}
		}

		/// <summary> 
		/// Compile the query (generate the SQL).
		/// </summary>
		protected void Compile(string queryString) 
		{
			this.queryString = queryString;

			log.Debug("compiling query");
			try 
			{
				ParserHelper.Parse(
					new PreprocessingParser(replacements),
					queryString,
					ParserHelper.HqlSeparators,
					this);
				RenderSql();
			} 
			catch (QueryException qe) 
			{
				qe.QueryString = queryString;
				throw qe;
			}
			catch (MappingException me) 
			{
				throw me;
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
		/// Persisters for the return values of a <c>Find</c> style query
		/// </summary>
		public override ILoadable[] Persisters 
		{
			get	
			{ 
				return persisters; 
			}			
		}
		
		/// <summary>
		///Types of the return values of an <tt>iterate()</tt> style query.
		///Return an array of <tt>Type</tt>s.
		/// </summary>
		public virtual IType[] ReturnTypes 
		{
			get	
			{ 
				return types;	
			}
		}

		protected bool HasScalarValues 
		{
			get 
			{ 
				return hasScalars; 
			}
		}
		
		public virtual string[][] ScalarColumnNames 
		{
			get	
			{ 
				return scalarColumnNames;
			}			
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

		internal string GetAliasName(String alias) 
		{
			String name = (String) aliasNames[alias];
			if (name==null) 
			{
				if (superQuery!=null) 
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
			string name = (string) aliasNames[alias];
			if (name!=null) 
			{
				return name + path.Substring( alias.Length );
			} 
			else 
			{
				return path;
			}
		}

		public void AddEntityToFetch(string name) 
		{
			entitiesToFetch.Add(name);
		}

		public override string SQLString 
		{
			get 
			{ 
				LogQuery(queryString, sqlString); 
				return sqlString; 
			}
		}


		public override SqlString SqlString
		{
			get 
			{
				throw new InvalidOperationException("SqlString not yet implemented in HQL.QueryTranslator");
			}
		}

		private string Prefix(string s) 
		{
			//TODO: H2.0.3: Using Prefix or using alias?
			if ( s.Length > 3 ) 
			{
				return s.Substring(0, 3).ToLower();
			} 
			else 
			{
				return s.ToLower();
			}
		}

		private int NextCount() 
		{
			return (superQuery==null) ? count++ : superQuery.count++;
		}

		internal string CreateName(string description) 
		{
			// this is a bit ugly, since Alias is really for
			// aliasing SQL identifiers ... but it does what
			// we want!
			return new Alias(10, NextCount().ToString() + StringHelper.Underscore)
				.ToAliasString(StringHelper.Unqualify(description).ToLower(), dialect);
		}

		internal string CreateNameFor(System.Type type) 
		{
			return CreateName(type.Name);
			//return Prefix(type.Name) + NextCount() + StringHelper.Underscore;
		}

		internal string CreateNameForCollection(string role) 
		{
			return CreateName(role);
			//return Prefix( StringHelper.Unqualify(role) ) + NextCount() + StringHelper.Underscore;
		}

		internal System.Type GetType(string name) 
		{
			System.Type type = (System.Type) typeMap[name];
			if ( type==null && superQuery!=null ) type = superQuery.GetType(name);
			return type;
		}

		internal string GetRole(string name) 
		{
			string role = (string) collections[name];
			if ( role==null && superQuery!=null ) role = superQuery.GetRole(name);
			return role;
		}

		internal bool IsName(string name) 
		{
			return aliasNames.Contains(name) ||
				typeMap.Contains(name) ||
				collections.Contains(name) || 
				( superQuery!=null && superQuery.IsName(name) );
		}

		internal IQueryable GetPersisterForName(string name) 
		{
			System.Type type = GetType(name);
			if (type == null) 
			{
				IType elemType = GetCollectionPersister( GetRole(name) ).ElementType;
				if ( ! (elemType is EntityType) ) return null;
				return GetPersister( ( (EntityType) elemType ).PersistentClass );
			} 
			else 
			{
				IQueryable persister = GetPersister(type);
				if (persister==null) throw new QueryException( "persistent class not found: " + type.Name );
				return persister;
			}
		}

		internal IQueryable GetPersisterUsingImports(string className) 
		{
			try 
			{
				return (IQueryable) factory.GetPersister( factory.GetImportedClassName( className) );
			}
			catch(Exception) 
			{
				return null;
			}
		}

		internal IQueryable GetPersister(System.Type clazz) 
		{
			try 
			{
				return (IQueryable) factory.GetPersister(clazz);
			} 
			catch (Exception) 
			{
				throw new QueryException( "persistent class not found: " + clazz.Name );
			}
		}

		internal CollectionPersister GetCollectionPersister(string role) 
		{
			try 
			{
				return factory.GetCollectionPersister(role);
			} 
			catch (Exception) 
			{
				throw new QueryException( "collection role not found: " + role );
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

		internal void AddFrom(string name, System.Type type, JoinFragment join) 
		{
			AddType(name, type);
			AddFrom(name, join);
		}

		internal void AddFrom(string name, JoinFragment join) 
		{
			fromTypes.Add(name);
			AddJoin(name, join);
		}

		internal void AddFromClass(string name, ILoadable classPersister) 
		{
			JoinFragment ojf = CreateJoinFragment(false);
			ojf.AddCrossJoin( classPersister.TableName, name );
			crossJoins.Add( name );
			AddFrom(name, classPersister.MappedClass, ojf);
		}

		internal void AddSelectClass(string name) 
		{
			returnTypes.Add(name);
		}

		internal void AddSelectScalar(IType type) 
		{
			scalarTypes.Add(type);
		}

		internal void AppendWhereToken(string token) 
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

		internal void AddJoin(string name, JoinFragment newjoin) 
		{
			JoinFragment oldjoin = (JoinFragment) joins[name];
			if (oldjoin==null) 
			{
				joins.Add(name, newjoin);
			}
			else 
			{
				oldjoin.AddCondition( newjoin.ToWhereFragmentString );
				if ( oldjoin.ToFromFragmentString.IndexOf( newjoin.ToFromFragmentString.Trim()) < 0 ) 
				{
					throw new AssertionFailure("bug in query parser: " + queryString);
					//TODO: what about the toFromFragmentString() ????
				}
			}
		}

		internal void AddNamedParameter(string name) 
		{
			if (superQuery != null) superQuery.AddNamedParameter(name);

			int loc = ++parameterCount;
			object o  = namedParameters[name];
			if (o == null) 
			{
				namedParameters.Add(name, loc);
			} 
			else if (o is int) 
			{
				ArrayList list = new ArrayList(4);
				list.Add(o);
				list.Add(loc);
				namedParameters.Add(name, list);
			} 
			else 
			{
				((ArrayList) o).Add(loc);
			}
		}

		internal int[] GetNamedParameterLocs(string name) 
		{
			object o = namedParameters[name];
			if (o == null) 
			{
				QueryException qe = new QueryException("Named parameter does not appear in Query: " + name);
				qe.QueryString = queryString;
				throw qe;
			} if (o is int) 
			  {
				  return new int[] { ((int) o) };
			  } 
			  else 
			  {
				  return ArrayHelper.ToIntArray( (ArrayList) o);
			  }
		}

		public ICollection NamedParameters 
		{
			get 
			{ 
				return namedParameters.Keys; 
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
			if (returnTypes.Count == 0 && scalarTypes.Count == 0) 
			{
				//ie no select clause in HQL
				returnTypes = fromTypes;
				rtsize = returnTypes.Count;
			}
			else
			{
				rtsize = returnTypes.Count;
				foreach(string entityName in entitiesToFetch)
					returnTypes.Add(entityName);
			}
			
			int size = returnTypes.Count;
			names = new string[size];
			persisters = new IQueryable[size];
			suffixes = new string[size];
			includeInSelect = new bool[size];
			for (int i=0; i<size; i++) 
			{
				string name = (string) returnTypes[i];
				persisters[i] = GetPersisterForName(name);
				suffixes[i] = (size==1) ? String.Empty : i.ToString() + StringHelper.Underscore;
				names[i] = name;
				includeInSelect[i] = !entitiesToFetch.Contains(name);
				if ( includeInSelect[i] ) selectLength++;
				if ( name.Equals(collectionOwnerName) ) collectionOwnerColumn = i;
			}

			string scalarSelect = RenderScalarSelect();

			int scalarSize = scalarTypes.Count;
			hasScalars = scalarTypes.Count!=size;

			types = new IType[scalarSize];
			for (int i=0; i<scalarSize; i++ ) 
			{
				types[i] = (IType) scalarTypes[i];
			}

			QuerySelect sql = new QuerySelect( factory.Dialect );
			sql.Distinct = distinct;

			if ( !shallowQuery ) 
			{
				RenderIdentifierSelect(sql);
				RenderPropertiesSelect(sql);
			}
			
			if ( CollectionPersister!=null ) 
			{
				//TODO: H2.0.3: When collection is updated
				//sql.AddSelectFragmentString( CollectionPersister.MultiselectClauseFragment(fetchName) );
			}
			if ( hasScalars || shallowQuery ) sql.AddSelectFragmentString(scalarSelect);

			// TODO: for some dialects it would be appropriate to add the renderOrderByProertySelecT() to other select strings
			MergeJoins( sql.JoinFragment );

			sql.WhereTokens = whereTokens;

			sql.GroupByTokens = groupByTokens;
			sql.HavingTokens = havingTokens;
			sql.OrderByTokens = orderByTokens;
			
			if ( CollectionPersister!=null && CollectionPersister.HasOrdering ) 
			{
				//TODO: H2.0.3: When Sql/QuerySelect is updated
				//sql.addOrderBy( CollectionPersister.GetSQLOrderByString(fetchName) );
			}

			scalarColumnNames = GenerateColumnNames(types, factory);

			// initialize the set of queries identifer spaces
			foreach(string name in collections.Values) 
			{
				CollectionPersister p = GetCollectionPersister(name);
				AddIdentifierSpace( p.QualifiedTableName );
			}
			foreach(string name in typeMap.Keys) 
			{
				IQueryable p = GetPersisterForName( name );
				AddIdentifierSpace( p.IdentifierSpace );
			}

			sqlString = sql.ToQueryString();

			System.Type[] classes = new System.Type[types.Length];
			for (int i=0; i<types.Length; i++) 
			{
				if (types[i]!=null) classes[i] = (types[i] is PrimitiveType) ? 
										((PrimitiveType) types[i]).PrimitiveClass :
										types[i].ReturnedClass;
			}

			try 
			{
				if (holderClass!=null) holderConstructor = holderClass.GetConstructor(classes);
			} 
			catch(Exception nsme) 
			{
				throw new QueryException("could not find constructor for: " + holderClass.Name, nsme);
			}
		}

		private void RenderIdentifierSelect(QuerySelect sql) 
		{
			int size = returnTypes.Count;

			for (int k=0; k<size; k++) 
			{
				string name = (string) returnTypes[k];
				string suffix = size==1 ? String.Empty : k.ToString() + StringHelper.Underscore;
				sql.AddSelectFragmentString( persisters[k].IdentifierSelectFragment(name, suffix) );
			}
		}

		private void RenderPropertiesSelect(QuerySelect sql) 
		{
			int size = returnTypes.Count;
			for (int k=0; k<size; k++) 
			{
				string suffix = (size==1) ? String.Empty : k.ToString() + StringHelper.Underscore;
				string name = (string) returnTypes[k];
				sql.AddSelectFragmentString( persisters[k].PropertySelectFragment(name, suffix) );
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
				int size = returnTypes.Count;
				for (int k=0; k<size; k++) 
				{
					scalarTypes.Add(NHibernate.Association(persisters[k].MappedClass));
					
					string[] names = persisters[k].IdentifierColumnNames;
					for (int i=0; i<names.Length; i++) 
					{
						buf.Append(returnTypes[k]).Append(StringHelper.Dot).Append(names[i]);
						if (!isSubselect) buf.Append(" as ").Append(ScalarName(k, i)); 
						if (i != names.Length - 1 || k != size - 1) buf.Append(StringHelper.CommaSpace);
					}
					
				}
			} 
			else 
			{
				//there _was_ a select clause
				int c = 0;
				bool nolast = false; //real hacky...
				foreach (object next in scalarSelectTokens) 
				{
					if (next is string) 
					{
						string token = (string) next;
						string lc = token.ToLower();
						if (lc.Equals(StringHelper.CommaSpace)) 
						{
							if (nolast) 
							{
								nolast = false;
							}
							else 
							{
								if (!isSubselect) buf.Append(" as ").Append(ScalarName(c++, 0));
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
						string[] tokens = (string[]) next;
						for (int i = 0; i < tokens.Length; i++) 
						{
							buf.Append(tokens[i]);
							if (!isSubselect) buf.Append(" as ").Append(ScalarName(c, i));
							if (i != tokens.Length - 1) buf.Append(StringHelper.CommaSpace);
						}
						c++;
					}
				}
				if (!isSubselect && !nolast) buf.Append(" as ").Append(ScalarName(c++, 0));
				
			}
			
			return buf.ToString();
		}

		private JoinFragment MergeJoins(JoinFragment ojf) 
		{
			foreach(string name in typeMap.Keys) 
			{
				IQueryable p = GetPersisterForName(name);
				bool includeSubclasses = returnTypes.Contains(name) && !IsShallowQuery;

				JoinFragment join = (JoinFragment) joins[name];
				if (join!=null) 
				{
					bool isCrossJoin = crossJoins.Contains(name);
					ojf.AddFragment(join);
					ojf.AddJoins(
						p.FromJoinFragment(name, isCrossJoin, includeSubclasses),
						p.QueryWhereFragment(name, isCrossJoin, includeSubclasses)
						);
				}
			}

			foreach(string name in collections.Keys) 
			{
				JoinFragment collJoin = (JoinFragment) joins[name];
				if (collJoin!=null) ojf.AddFragment(collJoin);
			}

			return ojf;
		}

		public IList QuerySpaces 
		{
			get 
			{ 
				return identifierSpaces; 
			}
		}

		/// <summary>
		/// Is this query called by Scroll() or Iterate()?
		/// </summary>
		public bool IsShallowQuery 
		{
			get 
			{ 
				return shallowQuery; 
			}
		}

		internal void AddIdentifierSpace(object table) 
		{
			identifierSpaces.Add(table);
			if (superQuery!=null) superQuery.AddIdentifierSpace(table);
		}

		internal bool Distinct 
		{
			//			get 
			//			{ 
			//				return distinct; 
			//			}
			set 
			{ 
				distinct = value; 
			}
		}
		
		public bool IsSubquery
		{
			get
			{
				return superQuery!=null;
			}
		}

		protected override CollectionPersister CollectionPersister 
		{
			get 
			{ 
				return collectionPersister; 
			}
		}

		public void SetCollectionToFetch(string role, string name, string ownerName)
		{
			fetchName = name;
			collectionPersister = GetCollectionPersister(role);
			collectionOwnerName = ownerName;
		}

		protected override string[] Suffixes 
		{
			get 
			{ 
				return suffixes; 
			}
			set 
			{ 
				suffixes = value; 
			}
		}

		protected void AddFromCollection(string elementName, string collectionRole) 
		{
			IType collectionElementType = GetCollectionPersister(collectionRole).ElementType;
			if ( !collectionElementType.IsEntityType ) throw new QueryException(
														   "collection of values in filter: " + elementName);
			EntityType elemType = (EntityType) collectionElementType;

			CollectionPersister persister = GetCollectionPersister(collectionRole);
			string[] keyColumnNames = persister.KeyColumnNames;
			//if (keyColumnNames.Length!=1) throw new QueryException("composite-key collecion in filter: " + collectionRole);

			string collectionName;
			JoinFragment join = CreateJoinFragment(false);
			collectionName = persister.IsOneToMany ? elementName : CreateNameForCollection(collectionRole);
			join.AddCrossJoin( persister.QualifiedTableName, collectionName);
			if ( !persister.IsOneToMany ) 
			{
				//many-to-many
				AddCollection(collectionName, collectionRole);

				IQueryable p = GetPersister( elemType.PersistentClass );
				string[] idColumnNames = p.IdentifierColumnNames;
				string[] eltColumnNames = persister.ElementColumnNames;
				join.AddJoin(
					p.TableName,
					elementName,
					StringHelper.Prefix(eltColumnNames, collectionName + StringHelper.Dot),
					idColumnNames,
					JoinType.InnerJoin);
			}
			join.AddCondition( collectionName, keyColumnNames, " = ?");
			if (persister.HasWhere) join.AddCondition(persister.GetSQLWhereString(collectionName));
			AddFrom(elementName, elemType.PersistentClass, join);
		}

		private IDictionary pathAliases = new Hashtable();
		private IDictionary pathJoins = new Hashtable();

		internal string GetPathAlias(string path) 
		{
			return (string) pathAliases[path];
		}

		internal JoinFragment GetPathJoin(string path) 
		{
			return (JoinFragment) pathJoins[path];
		}

		internal void AddPathAliasAndJoin(string path, string alias, JoinFragment join) 
		{
			pathAliases.Add(path, alias);
			pathJoins.Add( path, join.Copy() );
		}

		protected override int BindNamedParameters(IDbCommand ps, IDictionary namedParams, int start, ISessionImplementor session) 
		{
			if (namedParams != null) 
			{
				int result = 0;
				foreach (DictionaryEntry e in namedParams) 
				{
					string name = (string) e.Key;
					TypedValue typedval = (TypedValue) e.Value;
					int[] locs = GetNamedParameterLocs(name);
					for (int i = 0; i < locs.Length; i++) 
					{
						// Hack: parametercollection starts at 0
						typedval.Type.NullSafeSet(ps, typedval.Value, Impl.AdoHack.ParameterPos(locs[i] + start), session);
						// end-of Hack
					}
					result += locs.Length;
				}
				return result;
			}
			else 
			{
				return 0;
			}
		}

		//TODO: H2.0.3: iterate and scroll instead of getenumerable
		public IEnumerable GetEnumerable(object[] values, IType[] types, RowSelection selection, 
			IDictionary namedParams, IDictionary lockModes, ISessionImplementor session) 
		{
			IDbCommand st = PrepareQueryStatement(
				ApplyLocks(SqlString, lockModes, session.Factory.Dialect).ToString(),
				values, types, namedParams, selection, false, session);
			try 
			{
				SetMaxRows(st, selection);
				IDataReader rs = st.ExecuteReader();
				Advance(rs, selection, session);
				return new EnumerableImpl(rs, session, ReturnTypes, ScalarColumnNames );
			} 
			catch (Exception e) 
			{
				ClosePreparedStatement(st, selection, session);
				throw e;
			}
		}

		

		public static string[] ConcreteQueries(string query, ISessionFactoryImplementor factory) 
		{
			//scan the query string for class names appearing in the from clause and replace 
			//with all persistent implementors of the class/interface, returning multiple 
			//query strings (make sure we don't pick up a class in the select clause!) 

			//TODO: this is one of the ugliest and most fragile pieces of code in Hibernate...
			string[] tokens = StringHelper.Split( ParserHelper.Whitespace + "(),", query, true ); 
			if (tokens.Length==0) return new String[] { query }; // just especially for the trivial collection filter 

			ArrayList placeholders = new ArrayList();
			ArrayList replacements = new ArrayList();
			StringBuilder templateQuery = new StringBuilder(40);
			int count = 0;
			string last = null;
			int nextIndex = 0;
			string next = null;
			templateQuery.Append( tokens[0] );
			for (int i=1; i<tokens.Length; i++) 
			{

				//update last non-whitespace token, if necessary
				if ( !ParserHelper.IsWhitespace( tokens[i-1] ) ) last = tokens[i-1].ToLower();

				string token = tokens[i];
				if ( ParserHelper.IsWhitespace(token) || last==null ) 
				{

					// scan for the next non-whitespace token
					if (nextIndex<=i) 
					{
						for ( nextIndex=i+1; nextIndex<tokens.Length; nextIndex++ ) 
						{
							next = tokens[nextIndex].ToLower();
							if ( !ParserHelper.IsWhitespace(next) ) break;
						}
					}

					if (
						( beforeClassTokens.Contains(last) && !notAfterClassTokens.Contains(next) ) ||
						"class".Equals(last) ) 
					{
						System.Type clazz = GetImportedClass(token, factory);
						if ( clazz!=null ) 
						{
							string[] implementors = factory.GetImplementors(clazz);
							string placeholder = "$clazz" + count++ + "$";
						
							if ( implementors!=null ) 
							{
								placeholders.Add(placeholder);
								replacements.Add(implementors);
							}
							token = placeholder; //Note this!!
						}
					}
				}
				templateQuery.Append( token );

			}
			string[] results = StringHelper.Multiply( templateQuery.ToString(), placeholders.GetEnumerator(), replacements.GetEnumerator() );
			if(results.Length == 0)
				log.Warn("no persistent classes found for query class: "+query);
			return results;
		}


		private static readonly IList beforeClassTokens = new ArrayList();
		private static readonly IList notAfterClassTokens = new ArrayList();

		static QueryTranslator() 
		{
			beforeClassTokens.Add("from");
			beforeClassTokens.Add(",");
			notAfterClassTokens.Add("in");
			notAfterClassTokens.Add(",");
			notAfterClassTokens.Add("from");
			notAfterClassTokens.Add(")");
		}

		internal System.Type GetImportedClass(string name) 
		{
			return GetImportedClass(name, factory);
		}

		private static System.Type GetImportedClass(string name, ISessionFactoryImplementor factory) 
		{
			try 
			{
				return ReflectHelper.ClassForName( factory.GetImportedClassName(name) );
			} 
			catch(Exception) 
			{
				return null;
			}
		}

		private static string[][] GenerateColumnNames(IType[] types, ISessionFactoryImplementor f) 
		{
			string[][] names = new string[types.Length][];
			for (int i=0; i<types.Length; i++) 
			{
				int span = types[i].GetColumnSpan(f);
				names[i] = new string[span];
				for (int j=0; j<span; j++) 
				{
					names[i][j] = ScalarName(i, j);
				}
			}
			return names;
		}

		public IList FindList(
			ISessionImplementor session,
			object[] values,
			IType[] types,
			bool returnProxies,
			RowSelection selection,
			IDictionary namedParams,
			IDictionary lockModes) 
		{

			// TODO: fix the last parameter that is suppoesd to use lockModes
			return base.Find(session, values, types, returnProxies, selection, namedParams, lockModes);
		}

		protected override object GetResultColumnOrRow(object[] row, IDataReader rs, ISessionImplementor session) 
		{
			IType[] returnTypes = ReturnTypes;
			if (hasScalars) 
			{
				string[][] names = ScalarColumnNames;
				int queryCols = returnTypes.Length;
				if ( holderClass==null && queryCols==1 ) 
				{
					return returnTypes[0].NullSafeGet( rs, names[0], session, null );
				} 
				else 
				{
					row = new object[queryCols];
					for (int i=0; i<queryCols; i++ ) 
					{
						row[i] = returnTypes[i].NullSafeGet( rs, names[i], session, null );
					}
					if ( holderClass==null ) return row;
				}
			} 
			else if (holderClass==null) 
			{
				return (row.Length==1) ? row[0] : row;
			}

			try 
			{
				return holderConstructor.Invoke(row);
			} 
			catch (Exception e) 
			{
				throw new QueryException("could not instantiate: " + holderClass, e);
			}
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
				int j=0;
				for (int i=0; i<row.Length; i++) 
				{
					if ( includeInSelect[i] ) result[j++] = row[i];
				}
				return result;
			}
		}

		internal QueryJoinFragment CreateJoinFragment(bool useThetaStyleInnerJoins) 
		{
			return new QueryJoinFragment( factory.Dialect );
			//TODO: H2.0.3 when QueryJoinFragment is updated
			//return new QueryJoinFragment( factory.Dialect, useThetaStyleInnerJoins );
		}

		internal System.Type HolderClass 
		{
			get { return holderClass; }
			set { holderClass = value; }
		}

		//TODO: implement this method - in here for compilation
		protected override LockMode[] GetLockModes(IDictionary lockModes) 
		{
			IDictionary nameLockModes = new Hashtable();
			if (lockModes!=null) 
			{
				IDictionaryEnumerator it = lockModes.GetEnumerator();
				while ( it.MoveNext() ) 
				{
					DictionaryEntry me = it.Entry;
					nameLockModes.Add( 
						GetAliasName( (String) me.Key ),
						me.Value
						);
				}
			}
			LockMode[] lockModeArray = new LockMode[names.Length];
			for ( int i=0; i < names.Length; i++ ) 
			{
				LockMode lm = (LockMode) nameLockModes[names[i]];
				if (lm==null) lm = LockMode.None;
				lockModeArray[i] = lm;
			}
			return lockModeArray;
		}

		protected override SqlString ApplyLocks(SqlString sql, IDictionary lockModes, Dialect.Dialect dialect)
		{
			if (lockModes == null || lockModes.Count == 0)
			{
				return sql;
			}
			else if (dialect.SupportsForUpdateOf)
			{
				LockMode upgradeType = null;
				ForUpdateFragment updateClause = new ForUpdateFragment();
				IDictionaryEnumerator it = lockModes.GetEnumerator();
				it.MoveNext();
				DictionaryEntry me = (DictionaryEntry) it.Current;
				String name = GetAliasName( (String) me.Key );
				LockMode lockMode = (LockMode) me.Value;
				if ( LockMode.Read.LessThan(lockMode) ) 
				{
					updateClause.AddTableAlias(name);
					if ( upgradeType!=null && lockMode!=upgradeType ) throw new QueryException("mixed LockModes");
					upgradeType = lockMode;
				}
				if ( upgradeType==LockMode.UpgradeNoWait && dialect.SupportsForUpdateNoWait) 
				{ 
					updateClause.NoWait = true;
				}

				return sql.Append(updateClause.ToSqlStringFragment());
			}
			else
			{
				log.Debug("dialect does not support FOR UPDATE OF");
				return sql;
			}
		}

		protected bool upgradeLocks() 
		{
			return true;
		}

		protected override int CollectionOwner
		{
			get
			{
				return collectionOwnerColumn;
			}
		}

		protected ISessionFactoryImplementor Factory
		{
			set
			{
				this.factory = value;
			}
			get
			{
				return factory;
			}
		}

		protected bool Compiled
		{
			get
			{
				return compiled;
			}
		}

		public IDictionary AggregateFunctions
		{
			get
			{
				return factory.Dialect.AggregateFunctions;
			}
		}
	}
}