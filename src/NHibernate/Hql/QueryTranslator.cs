//$Id$
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
using NHibernate.Type;
using NHibernate.Util;
using BaseLoader = NHibernate.Loader.Loader;

namespace NHibernate.Hql {
	/// <summary> 
	/// An instance of <tt>QueryTranslator</tt> translates a Hibernate query string to SQL.
	/// </summary>
	public class QueryTranslator : BaseLoader {
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

		private IQueryable[] persisters;
		private IType[] types;
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

		private string[] suffixes;

		/// <summary> 
		/// Construct a query translator
		/// </summary>
		public QueryTranslator() {
		}

		/// <summary>
		/// Compile a subquery
		/// </summary>
		/// <param name="superquery"></param>
		/// <param name="queryString"></param>
		internal protected void Compile(QueryTranslator superquery, string queryString) {
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
		public void Compile(ISessionFactoryImplementor factory, string queryString, IDictionary replacements, bool scalar) {
			lock(this) {
				if (!compiled) {
					this.factory = factory;
					this.replacements = replacements;
					this.shallowQuery = scalar;
					Compile(queryString);
				}
			}
		}

		/// <summary> 
		/// Compile the query (generate the SQL).
		/// </summary>
		protected void Compile(string queryString) {
			this.queryString = queryString;

			log.Debug("compiling query");
			try {
				ParserHelper.Parse(
					new PreprocessingParser(replacements),
					queryString,
					ParserHelper.HqlSeparators,
					this);
				RenderSql();
			} catch (QueryException qe) {
				qe.QueryString = queryString;
				throw qe;
			}
			catch (MappingException me) {
				throw me;
			}
			catch (Exception e) {
				log.Debug("unexpected query compilation problem", e);
				QueryException qe = new QueryException("Incorrect query syntax", e);
				qe.QueryString = queryString;
				throw qe;
			}
			
			compiled = true;
		}
		
		/// <summary>
		/// Persisters for the return values of a <c>Find</c> style query
		/// </summary>
		public override ILoadable[] Persisters {
			get	{ return persisters; }			
		}
		
		/// <summary>
		///Types of the return values of an <tt>iterate()</tt> style query.
		///Return an array of <tt>Type</tt>s.
		/// </summary>
		public virtual IType[] ReturnTypes {
			get	{ return types;	}			
		}

		protected bool HasScalarValues {
			get { return hasScalars; }
		}
		
		public virtual string[][] ScalarColumnNames {
			get	{ return scalarColumnNames;	}			
		}
		private void LogQuery(string hql, string sql) {
			if (log.IsDebugEnabled) {
				log.Debug("HQL: " + hql);
				log.Debug("SQL: " + sql);
			}
		}
		internal void SetAliasName(string alias, string name) {
			aliasNames.Add(alias, name);
		}
		internal string Unalias(string path) {
			string alias = StringHelper.Root(path);
			string name = (string) aliasNames[alias];
			if (name!=null) {
				return name + path.Substring( alias.Length );
			} else {
				return path;
			}
		}

		public override string SQLString {
			get { LogQuery(queryString, sqlString); return sqlString; }
		}

		private string Prefix(string s) {
			if ( s.Length > 3 ) {
				return s.Substring(0, 3).ToLower();
			} else {
				return s.ToLower();
			}
		}

		private int NextCount() {
			return (superQuery==null) ? count++ : superQuery.count++;
		}

		internal string CreateNameFor(System.Type type) {
			string typeName = type.Name;
			return Prefix( StringHelper.Unqualify(typeName) ) + NextCount() + StringHelper.Underscore;
		}

		internal string CreateNameForCollection(string role) {
			return Prefix( StringHelper.Unqualify(role) ) + NextCount() + StringHelper.Underscore;
		}

		internal System.Type GetType(string name) {
			System.Type type = (System.Type) typeMap[name];
			if ( type==null && superQuery!=null ) type = superQuery.GetType(name);
			return type;
		}

		internal string GetRole(string name) {
			string role = (string) collections[name];
			if ( role==null && superQuery!=null ) role = superQuery.GetRole(name);
			return role;
		}

		internal bool IsName(string name) {
			return aliasNames.Contains(name) ||
				typeMap.Contains(name) ||
				collections.Contains(name) || 
				( superQuery!=null && superQuery.IsName(name) );
		}

		internal IQueryable GetPersisterForName(string name) {
			System.Type type = GetType(name);
			if (type == null) {
				IType elemType = GetCollectionPersister( GetRole(name) ).ElementType;
				if ( ! (elemType is EntityType) ) return null;
				return GetPersister( ( (EntityType) elemType ).PersistentClass );
			} else {
				IQueryable persister = GetPersister(type);
				if (persister==null) throw new QueryException( "persistent class not found: " + type.Name );
				return persister;
			}
		}

		internal IQueryable GetPersisterUsingImports(string className) {
			try {
				return (IQueryable) factory.GetPersister( factory.GetImportedClassName( className) );
			}
			catch(Exception) {
				return null;
			}
		}

		internal IQueryable GetPersister(System.Type clazz) {
			try {
				return (IQueryable) factory.GetPersister(clazz);
			} catch (Exception) {
				throw new QueryException( "persistent class not found: " + clazz.Name );
			}
		}

		internal CollectionPersister GetCollectionPersister(string role) {
			try {
				return factory.GetCollectionPersister(role);
			} catch (Exception) {
				throw new QueryException( "collection role not found: " + role );
			}
		}

		internal void AddType(string name, System.Type type) {
			typeMap.Add(name, type);
		}

		internal void AddCollection(string name, string role) {
			collections.Add(name, role);
		}

		internal void AddFrom(string name, System.Type type, JoinFragment join) {
			AddType(name, type);
			AddFrom(name, join);
		}

		internal void AddFrom(string name, JoinFragment join) {
			fromTypes.Add(name);
			AddJoin(name, join);
		}

		internal void AddFromClass(string name, ILoadable classPersister) {
			JoinFragment ojf = CreateJoinFragment();
			ojf.AddCrossJoin( classPersister.TableName, name );
			crossJoins.Add( name );
			AddFrom(name, classPersister.MappedClass, ojf);
		}

		internal void AddSelectClass(string name) {
			returnTypes.Add(name);
		}

		internal void AddSelectScalar(IType type) {
			scalarTypes.Add(type);
		}

		internal void AppendWhereToken(string token) {
			whereTokens.Add(token);
		}

		internal void AppendHavingToken(string token) {
			havingTokens.Add(token);
		}

		internal void AppendOrderByToken(string token) {
			orderByTokens.Add(token);
		}

		internal void AppendGroupByToken(string token) {
			groupByTokens.Add(token);
		}

		internal void AppendScalarSelectToken(string token) {
			scalarSelectTokens.Add(token);
		}

		internal void AppendScalarSelectTokens(string[] tokens) {
			scalarSelectTokens.Add(tokens);
		}

		internal void AddJoin(string name, JoinFragment newjoin) {
			if ( !joins.Contains(name) ) {
				joins.Add(name, newjoin);
			}
		}

		internal void AddNamedParameter(string name) {
			if (superQuery != null) superQuery.AddNamedParameter(name);

			int loc = ++parameterCount;
			object o  = namedParameters[name];
			if (o == null) {
				namedParameters.Add(name, loc);
			} else if (o is int) {
				ArrayList list = new ArrayList(4);
				list.Add(o);
				list.Add(loc);
				namedParameters.Add(name, list);
			} else {
				((ArrayList) o).Add(loc);
			}
		}

		internal int[] GetNamedParameterLocs(string name) {
			object o = namedParameters[name];
			if (o == null) {
				QueryException qe = new QueryException("Named parameter does not appear in Query: " + name);
				qe.QueryString = queryString;
				throw qe;
			} if (o is int) {
				return new int[] { ((int) o) };
			} else {
				return ArrayHelper.ToIntArray( (ArrayList) o);
			}
		}

		public ICollection NamedParameters {
			get { return namedParameters.Keys; }
		}

		public static string ScalarName(int x, int y) {
			return new StringBuilder()
				.Append('x')
				.Append(x)
				.Append(StringHelper.Underscore)
				.Append(y)
				.Append(StringHelper.Underscore)
				.ToString();
		}

		

		private void RenderSql() {
			if (returnTypes.Count == 0 && scalarTypes.Count == 0) {
				//ie no select clause in HQL
				returnTypes = fromTypes;
			}
			
			int size = returnTypes.Count;
			persisters = new IQueryable[size];
			suffixes = new string[size];
			for (int i=0; i<size; i++) {
				string name = (string) returnTypes[i];
				persisters[i] = GetPersisterForName(name);
				suffixes[i] = (size==1) ? StringHelper.EmptyString : i.ToString() + StringHelper.Underscore;
			}

			string scalarSelect = RenderScalarSelect();

			int scalarSize = scalarTypes.Count;
			hasScalars = scalarTypes.Count!=size;

			types = new IType[scalarSize];
			for (int i=0; i<scalarSize; i++ ) {
				types[i] = (IType) scalarTypes[i];
			}

			QuerySelect sql = new QuerySelect( factory.Dialect );
			sql.Distinct = distinct;

			if ( !shallowQuery ) {
				RenderIdentifierSelect(sql);
				RenderPropertiesSelect(sql);
			}

			if ( hasScalars || shallowQuery ) sql.AddSelectFragmentString(scalarSelect);

			// TODO: for some dialects it would be appropriate to add the renderOrderByProertySelecT() to other select strings
			MergeJoins( sql.JoinFragment );

			sql.WhereTokens = whereTokens;

			sql.GroupByTokens = groupByTokens;
			sql.HavingTokens = havingTokens;
			sql.OrderByTokens = orderByTokens;

			scalarColumnNames = GenerateColumnNames(types, factory);

			// initialize the set of queries identifer spaces
			foreach(string name in collections.Values) {
				CollectionPersister p = GetCollectionPersister(name);
				AddIdentifierSpace( p.QualifiedTableName );
			}
			foreach(string name in typeMap.Keys) {
				IQueryable p = GetPersisterForName( name );
				AddIdentifierSpace( p.IdentifierSpace );
			}

			sqlString = sql.ToQueryString();

			System.Type[] classes = new System.Type[types.Length];
			for (int i=0; i<types.Length; i++) {
				if (types[i]!=null) classes[i] = (types[i] is PrimitiveType) ? 
										((PrimitiveType) types[i]).PrimitiveClass :
										types[i].ReturnedClass;
			}

			try {
				if (holderClass!=null) holderConstructor = holderClass.GetConstructor(classes);
			} catch(Exception nsme) {
				throw new QueryException("could not find constructor for: " + holderClass.Name, nsme);
			}
		}

		private void RenderIdentifierSelect(QuerySelect sql) {
			int size = returnTypes.Count;

			for (int k=0; k<size; k++) {
				string name = (string) returnTypes[k];
				string suffix = size==1 ? StringHelper.EmptyString : k.ToString() + StringHelper.Underscore;
				sql.AddSelectFragmentString( persisters[k].IdentifierSelectFragment(name, suffix) );
			}
		}

		private void RenderPropertiesSelect(QuerySelect sql) {
			int size = returnTypes.Count;
			for (int k=0; k<size; k++) {
				string suffix = (size==1) ? StringHelper.EmptyString : k.ToString() + StringHelper.Underscore;
				string name = (string) returnTypes[k];
				sql.AddSelectFragmentString( persisters[k].PropertySelectFragment(name, suffix) );
			}
		}

		/// <summary> 
		/// WARNING: side-effecty
		/// </summary>
		private string RenderScalarSelect() {
			bool isSubselect = superQuery != null;
			
			StringBuilder buf = new StringBuilder(20);
			
			if (scalarTypes.Count == 0) {
				//ie. no select clause
				int size = returnTypes.Count;
				for (int k=0; k<size; k++) {
					scalarTypes.Add(NHibernate.Association(persisters[k].MappedClass));
					
					string[] names = persisters[k].IdentifierColumnNames;
					for (int i=0; i<names.Length; i++) {
						buf.Append(returnTypes[k]).Append(StringHelper.Dot).Append(names[i]);
						if (!isSubselect) buf.Append(" as ").Append(ScalarName(k, i)); 
						if (i != names.Length - 1 || k != size - 1) buf.Append(StringHelper.CommaSpace);
					}
					
				}
			} else {
				//there _was_ a select clause
				int c = 0;
				bool nolast = false; //real hacky...
				foreach (object next in scalarSelectTokens) {
					if (next is string) {
						string token = (string) next;
						string lc = token.ToLower();
						if (lc.Equals(StringHelper.CommaSpace)) {
							if (nolast) {
								nolast = false;
							}
							else {
								if (!isSubselect) buf.Append(" as ").Append(ScalarName(c++, 0));
							}
						}
						buf.Append(token);
						if (lc.Equals("distinct") || lc.Equals("all")) {
							buf.Append(' ');
						}
					} else {
						nolast = true;
						string[] tokens = (string[]) next;
						for (int i = 0; i < tokens.Length; i++) {
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

		private JoinFragment MergeJoins(JoinFragment ojf) {
			foreach(string name in typeMap.Keys) {
				IQueryable p = GetPersisterForName(name);
				bool includeSubclasses = returnTypes.Contains(name) && !IsShallowQuery;

				JoinFragment join = (JoinFragment) joins[name];
				if (join!=null) {
					bool isCrossJoin = crossJoins.Contains(name);
					ojf.AddFragment(join);
					ojf.AddJoins(
						p.FromJoinFragment(name, isCrossJoin, includeSubclasses),
						p.QueryWhereFragment(name, isCrossJoin, includeSubclasses)
						);
				}
			}

			foreach(string name in collections.Keys) {
				JoinFragment collJoin = (JoinFragment) joins[name];
				if (collJoin!=null) ojf.AddFragment(collJoin);
			}

			return ojf;
		}

		public IList QuerySpaces {
			get { return identifierSpaces; }
		}

		/// <summary>
		/// Is this query called by Scroll() or Iterate()?
		/// </summary>
		public bool IsShallowQuery {
			get { return shallowQuery; }
		}

		internal void AddIdentifierSpace(object table) {
			identifierSpaces.Add(table);
			if (superQuery!=null) superQuery.AddIdentifierSpace(table);
		}

		internal bool Distinct {
			get { return distinct; }
			set { distinct = value; }
		}

		protected override CollectionPersister CollectionPersister {
			get { return null; }
		}

		protected override string[] Suffixes {
			get { return suffixes; }
			set { suffixes = value; }
		}

		protected void AddFromCollection(string elementName, string collectionRole) {
			IType collectionElementType = GetCollectionPersister(collectionRole).ElementType;
			if ( !collectionElementType.IsEntityType ) throw new QueryException(
														   "collection of values in filter: " + elementName);
			EntityType elemType = (EntityType) collectionElementType;

			CollectionPersister persister = GetCollectionPersister(collectionRole);
			string[] keyColumnNames = persister.KeyColumnNames;
			if (keyColumnNames.Length!=1) throw new QueryException("composite-key collecion in filter: " + collectionRole);

			JoinFragment join = CreateJoinFragment();
			if ( persister.IsOneToMany ) {
				join.AddCrossJoin( persister.QualifiedTableName, elementName );
				join.AddCondition( elementName, keyColumnNames, " = ?");
				if ( persister.HasWhere ) join.AddCondition( persister.GetSQLWhereString(elementName) );
			} else {
				//many-to-many
				string collectionName = CreateNameForCollection(collectionRole);
				AddCollection(collectionName, collectionRole);
				join.AddCrossJoin(collectionName, collectionRole);
				join.AddCrossJoin( persister.QualifiedTableName, collectionName );
				join.AddCondition( collectionName, keyColumnNames, " = ?");

				IQueryable p = GetPersister( elemType.PersistentClass );
				string[] idColumnNames = p.IdentifierColumnNames;
				string[] eltColumnNames = persister.ElementColumnNames;
				join.AddJoin(
					p.TableName,
					elementName,
					StringHelper.Prefix(eltColumnNames, collectionName + StringHelper.Dot),
					idColumnNames,
					JoinType.InnerJoin);
				if ( persister.HasWhere ) join.AddCondition( persister.GetSQLWhereString(collectionName) );
			}
			AddFrom(elementName, elemType.PersistentClass, join);
		}

		private IDictionary pathAliases = new Hashtable();
		private IDictionary pathJoins = new Hashtable();

		internal string GetPathAlias(string path) {
			return (string) pathAliases[path];
		}

		internal JoinFragment GetPathJoin(string path) {
			return (JoinFragment) pathJoins[path];
		}

		internal void AddPathAliasAndJoin(string path, string alias, JoinFragment join) {
			pathAliases.Add(path, alias);
			pathJoins.Add( path, join.Copy() );
		}

		protected override void BindNamedParameters(IDbCommand ps, IDictionary namedParams, int start, ISessionImplementor session) {
			if (namedParams != null) {
				foreach (DictionaryEntry e in namedParams) {
					string name = (string) e.Key;
					TypedValue typedval = (TypedValue) e.Value;
					int[] locs = GetNamedParameterLocs(name);
					for (int i = 0; i < locs.Length; i++) {
						typedval.Type.NullSafeSet(ps, typedval.Value, locs[i] + start, session);
					}
				}
			}
		}

		public IEnumerable GetEnumerable(object[] values, IType[] types, RowSelection selection, IDictionary namedParams, ISessionImplementor session) {
			IDbCommand st = PrepareQueryStatement( SQLString, values, types, namedParams, selection, false, session);
			try {
				SetMaxRows(st, selection);
				IDataReader rs = st.ExecuteReader();
				Advance(rs, selection, session);
				return new EnumerableImpl(rs, session, ReturnTypes, ScalarColumnNames );
			} catch (Exception e) {
				ClosePreparedStatement(st, selection, session);
				throw e;
			}
		}

		

		public static string[] ConcreteQueries(string query, ISessionFactoryImplementor factory) {
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
			for (int i=1; i<tokens.Length; i++) {

				//update last non-whitespace token, if necessary
				if ( !ParserHelper.IsWhitespace( tokens[i-1] ) ) last = tokens[i-1].ToLower();

				string token = tokens[i];
				if ( ParserHelper.IsWhitespace(token) || last==null ) {

					// scan for the next non-whitespace token
					if (nextIndex<=i) {
						for ( nextIndex=i+1; nextIndex<tokens.Length; nextIndex++ ) {
							next = tokens[nextIndex].ToLower();
							if ( !ParserHelper.IsWhitespace(next) ) break;
						}
					}

					if (
						( beforeClassTokens.Contains(last) && !notAfterClassTokens.Contains(next) ) ||
						"class".Equals(last) ) {
						System.Type clazz = GetImportedClass(token, factory);
						if ( clazz!=null ) {
							string[] implementors = factory.GetImplementors(clazz);
							string placeholder = "$clazz" + count++ + "$";
						
							if ( implementors!=null ) {
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

		static QueryTranslator() {
			beforeClassTokens.Add("from");
			beforeClassTokens.Add(",");
			notAfterClassTokens.Add("in");
			notAfterClassTokens.Add(",");
			notAfterClassTokens.Add("from");
			notAfterClassTokens.Add(")");
		}

		internal System.Type GetImportedClass(string name) {
			return GetImportedClass(name, factory);
		}

		private static System.Type GetImportedClass(string name, ISessionFactoryImplementor factory) {
			try {
				return ReflectHelper.ClassForName( factory.GetImportedClassName(name) );
			} catch(Exception) {
				return null;
			}
		}

		private static string[][] GenerateColumnNames(IType[] types, ISessionFactoryImplementor f) {
			string[][] names = new string[types.Length][];
			for (int i=0; i<types.Length; i++) {
				int span = types[i].GetColumnSpan(f);
				names[i] = new string[span];
				for (int j=0; j<span; j++) {
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
			IDictionary namedParams) {

			return base.Find(session, values, types, returnProxies, selection, namedParams);
		}

		protected override object GetResultColumnOrRow(object[] row, IDataReader rs, ISessionImplementor session) {
			IType[] returnTypes = ReturnTypes;
			if (hasScalars) {
				string[][] names = ScalarColumnNames;
				int queryCols = returnTypes.Length;
				if ( holderClass==null && queryCols==1 ) {
					return returnTypes[0].NullSafeGet( rs, names[0], session, null );
				} else {
					row = new object[queryCols];
					for (int i=0; i<queryCols; i++ ) {
						row[i] = returnTypes[i].NullSafeGet( rs, names[i], session, null );
					}
					if ( holderClass==null ) return row;
				}
			} else if (holderClass==null) {
				return (row.Length==1) ? row[0] : row;
			}

			try {
				return holderConstructor.Invoke(row);
			} catch (Exception e) {
				throw new QueryException("could not instantiate: " + holderClass, e);
			}
		}

		internal QueryJoinFragment CreateJoinFragment() {
			return new QueryJoinFragment( factory.Dialect );
		}

		internal System.Type HolderClass {
			get { return holderClass; }
			set { holderClass = value; }
		}

	}
}