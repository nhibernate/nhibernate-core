using System;
using System.Collections;
using System.Data;
using System.Reflection;
using System.Text;

using log4net;

using NHibernate.Loader;
using NHibernate.Engine;
using NHibernate.Persister;
using NHibernate.Sql;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Hql {
	/// <summary>
	/// Summary description for QueryTranslator.
	/// </summary>
	public class QueryTranslator : Loader.Loader {

		private readonly IDictionary typeMap = new SequencedHashMap();
		private readonly IDictionary collections = new SequencedHashMap();
		private IList returnTypes = new ArrayList();
		private readonly IList fromTypes = new ArrayList();
		private readonly IList scalarTypes = new ArrayList();
		private readonly IDictionary namedParameters = new Hashtable(); //new HashMap();
		private readonly IDictionary aliasNames = new Hashtable();
		private readonly IList crossJoins = new ArrayList(); //Set  new HashSet();
	
		private readonly IList scalarSelectTokens = new ArrayList();
		private readonly IList whereTokens = new ArrayList();
		private readonly IList havingTokens = new ArrayList();
		private readonly IDictionary joins =  new Hashtable(); //new HashMap();
		private readonly IList orderByTokens = new ArrayList();
		private readonly IList groupByTokens = new ArrayList();
		private readonly IList identifierSpaces = new ArrayList(); //Set  new HashSet();
	
		private IQueryable[] persisters;
		private IType[] types;
		private string[][] scalarColumnNames;
		protected ISessionFactoryImplementor factory;
		private IDictionary replacements;
		private int count=0;
		private int parameterCount=0;
		private string queryString;
		private bool distinct=false;
		protected bool compiled;
		private string sqlString;
		private System.Type holderClass;
		private ConstructorInfo holderConstructor;
		private bool hasScalars;
		private bool shallowQuery;
		private QueryTranslator superQuery;
	
		private string[] suffixes;

		private static readonly ILog log = LogManager.GetLogger(typeof(QueryTranslator));

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
		void compile(QueryTranslator superquery, String queryString) {
			this.factory = superquery.factory;
			this.replacements = superquery.replacements;
			this.superQuery = superquery;
			this.shallowQuery = true;
			compile(queryString);																}
	
		/// <summary>
		/// Compile a "normal" query. This method may be called multiple times.
		/// Subsequent invocations are no-ops.
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="queryString"></param>
		/// <param name="replacements"></param>
		/// <param name="scalar"></param>
		//TODO: was sinchronized
		public void compile(ISessionFactoryImplementor factory, string queryString, IDictionary replacements, bool scalar) {
			if (!compiled) {
				this.factory = factory;
				this.replacements = replacements;
				this.shallowQuery = scalar;
				compile(queryString);
			}
		}
	
		/// <summary>
		/// Compile the query (generate the SQL).
		/// </summary>
		/// <param name="queryString"></param>
		protected void compile(string queryString) {
			this.queryString = queryString;
					
			log.Info("compiling query");
			try {
				//TODO:  implement PreprocessingParser
				/*
				ParserHelper.Parse( 
					new PreprocessingParser(replacements), 
					queryString, 
					ParserHelper.HqlSeparators, 
					this
					);
					*/
				RenderSQL();
			}
			catch (QueryException qe) {
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
			
			compiled=true;
		}

		/// <summary>
		/// Persisters for the return values of a find() style query.
		/// </summary>
		public override ILoadable[] Persisters {
			get { return persisters; }
		}
	
		/// <summary>
		/// Types of the return values of an iterate() style query.
		/// </summary>
		public IType[] ReturnTypes {
			get { return types; }
		}
	
		protected bool HasScalarValues {
			get { return hasScalars; }
		}
	
		public string[][] ScalarColumnNames {
			get { return scalarColumnNames; }
		}
	
		private void LogQuery(string hql, string sql) {
			if ( log.IsDebugEnabled ) {
				log.Debug("HQL: " + hql);
				log.Debug("SQL: " + sql);
			}
		}
	
		void SetAliasName(string alias, string name) {
			aliasNames.Add(alias, name);
		}
	
		string Unalias(String path) {
			string alias = StringHelper.Root(path);
			string name = (string) aliasNames[alias]; 
			if (name!=null) {
				return name + path.Substring( alias.Length );
			}
			else {
				return path;
			}
		}
	
		public override string SQLString { //TODO: was protected
			get {
				LogQuery(queryString, sqlString);
				return sqlString;
			}
		}
	
		private string Prefix(string s) {
			if ( s.Length>3 ) {
				return s.Substring(0, 3).ToLower();
			}
			else {
				return s.ToLower();
			}
		}
	
		private int NextCount {
			get { return (superQuery==null) ? count++ : superQuery.count++; }
		}
	
		string CreateNameFor(System.Type type) {
			string typeName = type.Name;  //or FullName???
			return Prefix( StringHelper.Unqualify(typeName) ) + NextCount + StringHelper.Underscore;
		}
	
		string CreateNameForCollection(string role) {
			return Prefix( StringHelper.Unqualify(role) ) + NextCount + StringHelper.Underscore;
		}
	
		System.Type GetType(string name) {
			System.Type type = (System.Type) typeMap[name];
			if ( type==null && superQuery!=null ) type = superQuery.GetType(name);
			return type;
		}
	
		string GetRole(string name) {
			string role = (string) collections[name];
			if ( role==null && superQuery!=null ) role = superQuery.GetRole(name);
			return role;
		}
	
		bool IsName(string name) {
			return aliasNames.Contains(name) ||
				typeMap.Contains(name) || 
				collections.Contains(name) || ( 
				superQuery!=null && superQuery.IsName(name) 
				);
		}

		IQueryable GetPersisterForName(string name) {
			System.Type type = GetType(name);
			if (type==null) {
				IType elemType = GetCollectionPersister( GetRole(name) ).ElementType;
				if ( ! (elemType is EntityType) ) return null; //YUCK! bugprone.....
				return GetPersister( ( (EntityType) elemType ).PersistentClass );
			}
			else {
				IQueryable persister = GetPersister(type);
				if (persister==null) throw new QueryException( "persistent class not found: " + type.Name );
				return persister;
			}
		}
	
		IQueryable GetPersisterUsingImports(string className) {
			string[] imports = factory.Imports;
			try {
				return (IQueryable) factory.GetPersister(className);
			}
			catch (Exception e) {
				for ( int i=0; i<imports.Length; i++ ) {
					try {
						return (IQueryable) factory.GetPersister(imports[i] + StringHelper.Dot + className);
					}
					catch (Exception ex) {}
				}
			}
			return null;
		}
	
		IQueryable GetPersister(System.Type clazz) {
			try {
				return (IQueryable) factory.GetPersister(clazz);
			}
			catch (Exception e) {
				throw new QueryException( "persistent class not found: " + clazz.Name );
			}
		}
	
		Collection.CollectionPersister GetCollectionPersister(string role) {
			try {
				return factory.GetCollectionPersister(role);
			}
			catch (Exception e) {
				throw new QueryException( "collection role not found: " + role );
			}
		}
	
		void AddType(string name, System.Type type) {
			typeMap.Add(name, type);
		}
	
		void AddCollection(string name, string role) {
			collections.Add(name, role);
		}
	
		void AddFrom(string name, System.Type type, JoinFragment join) {
			AddType(name, type);
			AddFrom(name, join);
		}
	
		void AddFrom(string name, JoinFragment join) {
			fromTypes.Add(name);
			AddJoin(name, join);
		}
	
		void AddFromClass(string name, ILoadable classPersister) {
			JoinFragment ojf = CreateJoinFragment();
			ojf.AddCrossJoin( classPersister.TableName, name );
			crossJoins.Add(name);
			AddFrom(name, classPersister.MappedClass, ojf);
		}
	
		void AddSelectClass(string name) {
			returnTypes.Add(name);
		}
	
		void AddSelectScalar(IType type) {
			scalarTypes.Add(type);
		}
		
		void AppendWhereToken(string token) {
			whereTokens.Add(token);
		}
		
		void AppendHavingToken(string token) {
			havingTokens.Add(token);
		}
		
		void AppendOrderByToken(string token) {
			orderByTokens.Add(token);
		}
	
		void AppendGroupByToken(string token) {
			groupByTokens.Add(token);
		}
		
		void AppendScalarSelectToken(string token) {
			scalarSelectTokens.Add(token);
		}
		
		void AppendScalarSelectTokens(string[] tokens) {
			scalarSelectTokens.Add(tokens);
		}
		
		void AddJoin(string name, JoinFragment newjoin) {
			if ( !joins.Contains(name) ) {
				joins.Add(name, newjoin);
			}
		}
	
		void AddNamedParameter(string name) {
			if (superQuery!=null) superQuery.AddNamedParameter(name);
			int loc = ++parameterCount;
			object o = namedParameters[name];
			if (o==null) {
				namedParameters.Add(name, loc);
			}
			else if (o is int) {
				ArrayList list = new ArrayList(4);
				list.Add(o);
				list.Add(loc);
				namedParameters.Add(name, list);
			}
			else {
				( (ArrayList) o ).Add(loc);
			}
		}

		private static readonly int[] NoInts = new int[0];
	
		protected int[] GetNamedParameterLocs(String name) {
			object o = namedParameters[name];
			if (o==null) {
				QueryException qe = new QueryException("Named parameter does not appear in Query: " + name);
				qe.QueryString = queryString;
				throw qe;
			}
			if (o is int) {
				return new int[] { (int) o };
			}
			else {
				return ArrayHelper.ToIntArray( (ArrayList) o );
			}
		}
	
		public ICollection getNamedParameters() { //TODO: ICollection was Collection  ????
			return namedParameters.Keys;
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
	
		private IList associations;  //TODO: was List ???
	
		private void RenderSQL() {
		
			if ( returnTypes.Count==0 && scalarTypes.Count==0 ) {
				//ie no select clause in HQL
				returnTypes = fromTypes;
			}
			int size = returnTypes.Count;
			persisters = new IQueryable[size];
			suffixes = new string[size];
			for ( int i=0; i<size; i++ ) {
				string name = (string) returnTypes[i];
				//if ( !isName(name) ) throw new QueryException("unknown type: " + name);
				persisters[i] = GetPersisterForName(name);
				suffixes[i] = (size==1) ? StringHelper.EmptyString : i.ToString() + StringHelper.Underscore;
			}
			
			string scalarSelect = RenderScalarSelect(); //Must be done here because of side-effect! yuck...
			
			int scalarSize = scalarTypes.Count;
			hasScalars = scalarTypes.Count!=size;
			
			types = new IType[scalarSize];
			for ( int i=0; i<scalarSize; i++ ) {
				types[i] = (IType) scalarTypes[i];
			}
			
			QuerySelect sql = new QuerySelect( factory.Dialect );
			sql.Distinct = distinct;
			
			if ( !shallowQuery ) {
				RenderIdentifierSelect(sql);
				RenderPropertiesSelect(sql);
			}
			
			if ( hasScalars || shallowQuery ) sql.AddSelectFragmentString(scalarSelect);
			
			//TODO: for some dialiects it would be appropriate to add the renderOrderByPropertiesSelect() to other select strings
			MergeJoins( sql.JoinFragment );
			/*TODO:
						sql.setWhereTokens( whereTokens.iterator() );
			
						sql.setGroupByTokens( groupByTokens.iterator() );
						sql.setHavingTokens( havingTokens.iterator() );
						sql.setOrderByTokens( orderByTokens.iterator() );
						*/
			scalarColumnNames = GenerateColumnNames(types, factory);
			
			// initialize the Set of queried identifier spaces (ie. tables)
			foreach(string iter in collections) {
				Collection.CollectionPersister p = GetCollectionPersister( iter );
				AddIdentifierSpace( p.QualifiedTableName );
			}

			foreach(string iter in typeMap.Keys) {
				IQueryable p = GetPersisterForName( iter );
				AddIdentifierSpace( p.IdentifierSpace );
			}
			
			sqlString = sql.ToQueryString();
			
			System.Type[] classes = new System.Type[types.Length];
			for ( int i=0; i<types.Length; i++ ) {
				if ( types[i]!=null ) classes[i] = (types[i] is PrimitiveType) ? 
										  ( (PrimitiveType) types[i] ).PrimitiveClass : 
										  types[i].ReturnedClass;
			}
			
			try {
				if (holderClass!=null) holderConstructor = holderClass.GetConstructor(classes);
			}
			catch {
				throw new QueryException("could not find constructor for: " + holderClass.Name); //This should be impossible 'cause GetConstructor return null if no constructor is found
			}
			
		}
		
		private void RenderIdentifierSelect(QuerySelect sql) {
			int size = returnTypes.Count;
			
			for ( int k=0; k<size; k++ ) {
				string name = (string) returnTypes[k];
				string suffix = size==1 ? StringHelper.EmptyString : k.ToString() + StringHelper.Underscore;
				sql.AddSelectFragmentString( persisters[k].IdentifierSelectFragment(name, suffix) );
			}
		}

		/*private String renderOrderByPropertiesSelect() {
			StringBuffer buf = new StringBuffer(10);
			
			//add the columns we are ordering by to the select ID select clause
			Iterator iter = orderByTokens.iterator();
			while ( iter.hasNext() ) {
				String token = (String) iter.next();
				if ( token.lastIndexOf(".") > 0 ) {
					//ie. it is of form "foo.bar", not of form "asc" or "desc"
					buf.append(StringHelper.COMMA_SPACE).append(token);
				}
			}
			
			return buf.toString();
		}*/
	
		private void RenderPropertiesSelect(QuerySelect sql) {
			int size = returnTypes.Count;
			for ( int k=0; k<size; k++ ) {
				string suffix =  (size==1) ? StringHelper.EmptyString : k.ToString() + StringHelper.Underscore;
				string name = (string) returnTypes[k] ;
				sql.AddSelectFragmentString( persisters[k].PropertySelectFragment(name, suffix) );
			}
		}
	
		/**
		 * WARNING: side-effecty
		 */
		private string RenderScalarSelect() {
		
			bool isSubselect = superQuery!=null;
		
			StringBuilder buf = new StringBuilder(32);
		
			if ( scalarTypes.Count==0 ) {
				//ie. no select clause
				int size = returnTypes.Count;
				for ( int k=0; k<size; k++ ) {
				
					scalarTypes.Add( NHibernate.Association(
						persisters[k].MappedClass
						) );
				
					string[] names = persisters[k].IdentifierColumnNames;
					for (int i=0; i<names.Length; i++) {
						buf.Append( returnTypes[k] ).Append(StringHelper.Dot).Append( names[i] );
						if (!isSubselect) buf.Append(" as ").Append( ScalarName(k, i) );
						if (i!=names.Length-1 || k!=size-1 ) buf.Append(StringHelper.CommaSpace);
					}
				
				}
			
			}
			else {
				//there _was_ a select clause
				int c=0;
				bool nolast=false; //real hacky...
				foreach ( object next in scalarSelectTokens ) {
					if (next is String) {
						string token = (string) next;
						string lc = token.ToLower();
						if ( lc.Equals(StringHelper.CommaSpace) ) {
							if (nolast) {
								nolast = false;
							}
							else {
								if (!isSubselect) buf.Append(" as ").Append( ScalarName(c++, 0) );					}
						}
						buf.Append(token);																		if ( lc.Equals("distinct") || lc.Equals("all") ) {
																													buf.Append(' ');
																												}
					}
					else {
						nolast = true;
						string[] tokens = (string[]) next;
						for ( int i=0; i<tokens.Length; i++ ) {
							buf.Append(tokens[i]);
							if (!isSubselect) buf.Append(" as ").Append( ScalarName(c, i) );
							if (i!=tokens.Length-1) buf.Append(StringHelper.CommaSpace);
						}
						c++;
					}
				}
				if (!isSubselect && !nolast) buf.Append(" as ").Append( ScalarName(c++, 0) );
			
			}
		
			return buf.ToString();
		}

		private JoinFragment MergeJoins(JoinFragment ojf) {
		
			//classes
			foreach( string name in typeMap.Keys ) {
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

			foreach ( string iter in collections.Keys ) {
				JoinFragment collJoin = (JoinFragment) joins[iter];
				if (collJoin!=null) ojf.AddFragment(collJoin);
			}
			
			return ojf;
		}
	
		public ICollection QuerySpaces {
			get { return identifierSpaces; }
		}
	
		/**
		* Is this query called by scroll() or iterate()?
		* @return true if it is, false if it is called by find() or list()
		*/
		public bool IsShallowQuery {
			get { return shallowQuery; }
		}
		
		void AddIdentifierSpace(object table) {
			identifierSpaces.Add(table);
			if (superQuery!=null) superQuery.AddIdentifierSpace(table);
		}
		
		bool Distinct {
			set { this.distinct = value; }
		}
		
		protected override Collection.CollectionPersister CollectionPersister {
			get { return null; }
		}
	
		protected override string[] Suffixes {
			get { return suffixes; }
			set { suffixes = value; }
		}

		protected void addFromCollection(string elementName, string collectionRole) {
			//TODO: There was final in each parameters ?????

			//q.addCollection(collectionName, collectionRole);
			IType collectionElementType = GetCollectionPersister(collectionRole).ElementType;
			if ( !collectionElementType.IsEntityType ) throw new QueryException(
				"collection of values in filter: " + elementName
			);
			EntityType elemType = (EntityType) collectionElementType;
			
			Collection.CollectionPersister persister = GetCollectionPersister(collectionRole);
			string[] keyColumnNames = persister.KeyColumnNames;
			if (keyColumnNames.Length!=1) throw new QueryException("composite-key collection in filter: " + collectionRole);
			
			JoinFragment join = CreateJoinFragment();
			if ( persister.IsOneToMany ) {
				join.AddCrossJoin( persister.QualifiedTableName, elementName );
				join.AddCondition(elementName, keyColumnNames, " = ?");
			}
			else { //many-to-many
				string collectionName = CreateNameForCollection(collectionRole);
				AddCollection(collectionName, collectionRole);
				join.AddCrossJoin( persister.QualifiedTableName, collectionName );
				join.AddCondition(collectionName, keyColumnNames, " = ?");
				
				IQueryable p = GetPersister( elemType.PersistentClass );
				string[] idColumnNames =  p.IdentifierColumnNames;
				string[] eltColumnNames = persister.ElementColumnNames;
				join.AddJoin( 
					p.TableName, 
					elementName, 
					StringHelper.Prefix(eltColumnNames, collectionName + StringHelper.Dot), 
					idColumnNames, 
					JoinType.InnerJoin
				);
			}
			AddFrom(elementName, elemType.PersistentClass, join);

		}
		
		private readonly IDictionary pathAliases = new Hashtable();
		private readonly IDictionary pathJoins = new Hashtable();
		
		string GetPathAlias(string path) {
			return (string) pathAliases[path];
		}
		
		JoinFragment GetPathJoin(string path) {
			return (JoinFragment) pathJoins[path];
		}
		
		void AddPathAliasAndJoin(string path, string alias, JoinFragment join) {
			pathAliases.Add(path, alias);
			pathJoins.Add( path, join.Copy() );
		}

		/*
		protected void BindNamedParameters(IDbCommand ps, IDictionary namedParams, ISessionImplementor session) {
			if (namedParams!=null) {
				Iterator iter = namedParams.entrySet().iterator();
				while ( iter.hasNext() ) {
					Map.Entry e = (Map.Entry) iter.next();
					String name = (String) e.getKey();
					TypedValue typedval = (TypedValue) e.getValue();
					int[] locs = getNamedParameterLocs(name);
					for ( int i=0; i<locs.length; i++ ) {
						typedval.getType().nullSafeSet( ps, typedval.getValue(), locs[i], session );
					}
				}
			}
		}
		*/
	
		/*
		public IEnumerator Iterate(object[] values, IType[] types, RowSelection selection, IDictionary namedParams, ISessionImplementor session) {
			
			IDbCommand st = PrepareQueryStatement( GetSQLString(), values, types, selection, false, session );
			try {
				BindNamedParameters(st, namedParams, session);
				SetMaxRows(st, selection);
				IDataReader rs = st.executeQuery();
				Advance(rs, selection, session);
				return new IteratorImpl( rs, session, getReturnTypes(), getScalarColumnNames() );
			}
			catch (SQLException sqle) {
				JDBCExceptionReporter.logExceptions(sqle);
				closePreparedStatement(st, selection, session);
				throw sqle;
			}
		}
	*/
		/*
		public ScrollableResults scroll(Object[] values, Type[] types, RowSelection selection, Map namedParams, SessionImplementor session) throws HibernateException, SQLException {
			
			PreparedStatement st = prepareQueryStatement( getSQLString(), values, types, selection, true, session );
			try {
				bindNamedParameters(st, namedParams, session);
				setMaxRows(st, selection);
				ResultSet rs = st.executeQuery();
				advance(rs, selection, session);
				return new ScrollableResultsImpl( rs, session, getReturnTypes() );
			}
			catch (SQLException sqle) {
				JDBCExceptionReporter.logExceptions(sqle);
				closePreparedStatement(st, selection, session);
				throw sqle;
			}
		}
		*/
		
		public static string[] ConcreteQueries(string query, ISessionFactoryImplementor factory) {
			StringTokenizer tokens = new StringTokenizer(query, StringHelper.CommaSpace);
			ArrayList placeholders = new ArrayList();
			ArrayList replacements = new ArrayList();
			int count=0;
			bool check = false;
			foreach ( string token in tokens ) {
				if ( "class".Equals( token.ToLower() ) ) {
					check = true;
				}
				else if (check) {
					check = false;
					System.Type clazz = GetImportedClass(token, factory);
					if (clazz!=null) {
						string[] implementors = factory.GetImplementors(clazz);
						string placeholder = "$clazz" + count + "$";
						query = StringHelper.ReplaceOnce(query, token, placeholder);
						if ( implementors!=null ) {
							placeholders.Add(placeholder);
							replacements.Add(implementors);
						}
					}
				}
			}
			return StringHelper.Multiply( query, placeholders.GetEnumerator(), replacements.GetEnumerator() );
		}
	
		System.Type GetImportedClass(string name) {
			return GetImportedClass(name, factory);
		}

		private static System.Type GetImportedClass(string name, ISessionFactoryImplementor factory) {
			try {
				return ReflectHelper.ClassForName(name);
			}
			catch (Exception e) {
				string[] imports = factory.Imports;
				for (int i=0; i<imports.Length; i++) {
					try {
						return ReflectHelper.ClassForName( imports[i] + StringHelper.Dot + name );
					}
					catch (Exception ex) {}
				}
				return null;
			}
		}
	
		private static string[][] GenerateColumnNames(IType[] types, ISessionFactoryImplementor f) {
			string[][] names = new string[types.Length][];
			for (int i=0; i<types.Length; i++) {
				int span = types[i].GetColumnSpan(f);
				names[i] = new string[span];
				for ( int j=0; j<span; j++ ) {
					names[i][j] = ScalarName(i, j);
				}
			}
			return names;
		}

		protected override IList Find( //TODO: was   public readonly List
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
					}
					else {
						row = new object[queryCols];
						for ( int i=0; i<queryCols; i++ )
						row[i] = returnTypes[i].NullSafeGet( rs, names[i], session, null );
						if (holderClass==null) return row;
					}
				}
			else if (holderClass==null) {
				return (row.Length==1) ? row[0] : row;
			}

			try {
				return holderConstructor.Invoke(row); //TODO: is it correct???
			}
			catch (Exception e) {
				throw new QueryException("could not instantiate: " + holderClass, e);
			}
		}
	
		QueryJoinFragment CreateJoinFragment() {
			return new QueryJoinFragment( factory.Dialect );
		}
		
		void SetHolderClass(System.Type clazz) {
			holderClass = clazz;
		}
	}
}
