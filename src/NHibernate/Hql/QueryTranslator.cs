//$Id$
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Data;
using System.Text;
using System.Runtime.Serialization;
using NHibernate;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Impl;
using NHibernate.Loader;
using NHibernate.Persister;
using NHibernate.Portable;
using NHibernate.Sql;
using NHibernate.Type;
using NHibernate.Util;
using BaseLoader = NHibernate.Loader.Loader;

namespace NHibernate.Hql
{
	/// <summary> 
	/// An instance of <tt>QueryTranslator</tt> translates a Hibernate query string to SQL.
	/// </summary>
	public class QueryTranslator : BaseLoader
	{
		private static readonly log4net.ILog log  = log4net.LogManager.GetLogger(typeof(QueryTranslator));
		private static StringCollection dontSpace = new StringCollection();
		private static int[]            NO_INTS   = new int[0];

		private Hashtable       typeMap;
		private Hashtable       collections;
		private Hashtable       names;
		private ArrayList       returnTypes;
		private ArrayList       fromTypes;
		private ArrayList       scalarTypes;
		private Hashtable       namedParameters;
		private IList           scalarSelectTokens;
		private IList           whereTokens;
		private IList           havingTokens;
		private StringBuilder   joins;
		private IList           orderByTokens;
		private IList           groupByTokens;
		private ISet            identifierSpaces;

		private IQueryable[]    persisters;
		private IType[]         types;
		private string[][]      scalarColumnNames;
		protected ISessionFactoryImplementor factory;
		private IDictionary     replacements;
		private int             count = 0;
		private int             parameterCount = 0;
		private string          queryString;
		private bool            distinct = false;
		private string          fromWhereString;
		private string          selectPropertiesString;
		private string          scalarSelectString;
		protected bool          compiled;
		private bool            hasScalars;
		private bool            shallowQuery;
		private QueryTranslator superQuery;
		private string[]        suffixes;
		private IList           associations;
		private IDictionary     pathAliases;
		private IDictionary     pathJoins;
		
		
		static QueryTranslator()
		{
			//dontSpace.Add("'");
			dontSpace.Add(".");
			dontSpace.Add("+");
			dontSpace.Add("-");
			dontSpace.Add("/");
			dontSpace.Add("*");
			dontSpace.Add("<");
			dontSpace.Add(">");
			dontSpace.Add("=");
			dontSpace.Add("#");
			dontSpace.Add("~");
			dontSpace.Add("|");
			dontSpace.Add("&");
			dontSpace.Add("<=");
			dontSpace.Add(">=");
			dontSpace.Add("=>");
			dontSpace.Add("=<");
			dontSpace.Add("!=");
			dontSpace.Add("<>");
			dontSpace.Add("!#");
			dontSpace.Add("!~");
			dontSpace.Add("!<");
			dontSpace.Add("!>");
			dontSpace.Add(StringHelper.OpenParen); //for MySQL
			dontSpace.Add(StringHelper.ClosedParen);
		}

		/// <summary> 
		/// Construct a query translator
		/// </summary>
		public QueryTranslator()
		{
			typeMap            = new Hashtable();
			collections        = new Hashtable();
			names              = new Hashtable();
			returnTypes        = new ArrayList();
			fromTypes          = new ArrayList();
			scalarTypes        = new ArrayList();
			namedParameters    = new Hashtable();
			scalarSelectTokens = new ArrayList();
			whereTokens        = new ArrayList();
			havingTokens       = new ArrayList();
			joins              = new StringBuilder();
			orderByTokens      = new ArrayList();
			groupByTokens      = new ArrayList();
			identifierSpaces   = new Portable.ListSet();
			pathAliases        = new Hashtable();
			pathJoins          = new Hashtable();
		}
		
		public override ILoadable[] Persisters
		{
			get	{ return persisters; }			
		}
		
		/// <summary>
		///Types of the return values of an <tt>iterate()</tt> style query.
		///Return an array of <tt>Type</tt>s.
		/// </summary>
		public virtual IType[] ReturnTypes
		{
			get	{ return types;	}			
		}
		
		public virtual string[][] ScalarColumnNames
		{
			get	{ return scalarColumnNames;	}			
		}
		
		/// <summary>
		/// Return the SQL for a <tt>find()</tt> style query.
		/// Rturn the SQL as a string
		/// </summary>
		public override string SQLString
		{
			get
			{
				string result = selectPropertiesString + fromWhereString;
				LogQuery(queryString, result);

				return result;
			}
			
		}

		/// <summary>
		/// Return the SQL for a <tt>find()</tt> style query.
		/// Return the SQL as a string
		/// </summary>
		internal protected string ScalarSelectSQL
		{
			get
			{
				string result = scalarSelectString + fromWhereString;
				LogQuery(queryString, result);

				return result;
			}
			
		}

		public virtual ICollection NamedParameters
		{
			get	{ return namedParameters.Keys; }
			
		}
		public virtual ISet QuerySpaces
		{
			get	{ return identifierSpaces; }			
		}

		internal protected bool Distinct
		{
			set	{ distinct = value; }
		}

		protected override CollectionPersister CollectionPersister
		{
			get	{ return null; }
		}

		protected override string[] Suffixes
		{
			get	{ return suffixes; }
			set { ; }
		}
		
		/// <summary>
		/// Compile a subquery
		/// </summary>
		/// <param name="superquery"></param>
		/// <param name="queryString"></param>
		internal protected void Compile(QueryTranslator superquery, string queryString)
		{
			this.factory      = superquery.factory;
			this.replacements = superquery.replacements;
			this.superQuery   = superquery;
			this.shallowQuery = true;

			Compile(queryString);
		}
		
		/// <summary>
		/// Compile a "normal" query. This method may be called multiple
		/// times. Subsequent invocations are no-ops.
		/// 
		/// note: Threadsafe
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="queryString"></param>
		/// <param name="replacements"></param>
		/// <param name="scalar"></param>
		
		public void Compile(ISessionFactoryImplementor factory, string queryString, IDictionary replacements, bool scalar)
		{
			lock(this)
			{
				if (!compiled)
				{
					this.factory      = factory;
					this.replacements = replacements;
					this.shallowQuery = scalar;

					Compile(queryString);
				}
			}
		}
		
		/// <summary> 
		/// Compile the query (generate the SQL).
		/// </summary>
		protected void Compile(string queryString)
		{
			this.queryString = queryString;
			log.Info("compiling query");

			try
			{
				ParserHelper.Parse(new PreprocessingParser(replacements), queryString, ParserHelper.HqlSeparators, this);
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
				QueryException qe;

				log.Debug("unexpected query compilation problem", e);
				
				qe             = new QueryException("Incorrect query syntax", e);
				qe.QueryString = queryString;
				
				throw qe;
			}
			
			compiled = true;
		}
		
		
		internal protected bool HasScalarValues
		{
			get { return hasScalars; }
		}
		
		
		private void  LogQuery(string hql, string sql)
		{
			if (log.IsDebugEnabled)
			{
				log.Debug("HQL: " + hql);
				log.Debug("SQL: " + sql);
			}
		}
		
		
		private string Prefix(string s)
		{
			if (s.Length > 3)
			{
				return s.Substring(0, (3) - (0)).ToLower();
			}
			else
			{
				return s.ToLower();
			}
		}
		
		private int NextCount()
		{
			return (superQuery == null) ? count++ : superQuery.count++;
		}
		
		internal protected string CreateNameFor(System.Type type)
		{
			string typeName = type.FullName;

			return Prefix(StringHelper.Unqualify(typeName)) + NextCount() + StringHelper.Underscore;
		}
		
		internal protected string CreateNameForCollection(string role)
		{
			return Prefix(StringHelper.Unqualify(role, "/")) + NextCount() + StringHelper.Underscore;
		}
		
		internal protected string GetType(string name)
		{
			string type = (string) typeMap[name];
			if (type == null && superQuery != null)
			{
				type = superQuery.GetType(name);
			}

			return type;
		}
		
		internal protected string GetRole(string name)
		{
			string role = (string) collections[name];
			if (role == null && superQuery != null)
			{
				role = superQuery.GetRole(name);
			}

			return role;
		}
		
		internal protected bool IsName(string name)
		{
			return typeMap.ContainsKey(name) || 
				collections.ContainsKey(name) || 
				(superQuery != null && superQuery.IsName(name));
		}
		
		internal protected IQueryable GetPersisterForName(string name)
		{
			string typeName = GetType(name);
			if (typeName == null)
			{
				return GetPersister(((EntityType) GetCollectionPersister(GetRole(name)).ElementType).PersistentClass);
			}
			else
			{
				IQueryable persister = GetPersister(typeName);
				if (persister == null)
				{
					throw new QueryException("persistent class not found: " + typeName);
				}

				return persister;
			}
		}
		
		/// <summary>
		/// Persisters for the return values of a <tt>find()</tt> style query.
		/// </summary>
		/// <param name="className"></param>
		/// <returns>an array of <tt>ClassPersister</tt>s.</returns>
		internal protected IQueryable GetPersister(string className)
		{
			string[] imports = factory.Imports;

			try
			{
				return (IQueryable) factory.GetPersister(className);
			}
			catch (Exception)
			{
				for (int i = 0; i < imports.Length; i++)
				{
					try
					{
						return (IQueryable) factory.GetPersister(imports[i] + StringHelper.Dot + className);
					}
					catch (Exception)
					{
					}
				}
			}

			return null;
		}
		
		internal protected IQueryable GetPersister(System.Type clazz)
		{
			try
			{
				return (IQueryable) factory.GetPersister(clazz);
			}
			catch (Exception)
			{
				throw new QueryException("persistent class not found: " + clazz.FullName);
			}
		}
		
		internal protected CollectionPersister GetCollectionPersister(string role)
		{
			try
			{
				return factory.GetCollectionPersister(role);
			}
			catch (Exception)
			{
				throw new QueryException("collection role not found: " + role);
			}
		}
		
		internal protected void  AddType(string name, string type)
		{
			typeMap[name] = type;
			names[type]   = name;
		}
		
		internal protected void  AddCollection(string name, string role)
		{
			collections[name] = role;
		}
		
		internal protected void  AddFromType(string name, string type)
		{
			AddType(name, type);
			fromTypes.Add(name);
		}
		
		internal protected void  AddReturnType(string name)
		{
			returnTypes.Add(name);
		}
		
		internal protected void  AddScalarType(IType type)
		{
			scalarTypes.Add(type);
		}
		
		internal protected void  AppendWhereToken(string token)
		{
			whereTokens.Add(token);
		}
		
		internal protected void  AppendHavingToken(string token)
		{
			havingTokens.Add(token);
		}
		
		internal protected void  AppendOrderByToken(string token)
		{
			orderByTokens.Add(token);
		}
		
		internal protected void  AppendGroupByToken(string token)
		{
			groupByTokens.Add(token);
		}
		
		internal protected void  AppendScalarSelectToken(string token)
		{
			scalarSelectTokens.Add(token);
		}
		
		internal protected void  AppendScalarSelectTokens(string[] tokens)
		{
			scalarSelectTokens.Add(tokens);
		}
		
		internal protected void  AddJoin(string token)
		{
			if (joins.Length == 0 && token.Length != 0)
			{
				joins.Append(token.Substring(5)); // remove leading " and " (ugly and causes bugs!)
			}
			else
			{
				joins.Append(token);
			}
		}
		
		internal protected void  AddNamedParameter(string name)
		{
			if (superQuery != null)
			{
				superQuery.AddNamedParameter(name);
			}

			Int32 loc = ++parameterCount;
			object o  = namedParameters[name];
			if (o == null)
			{
				namedParameters[name] = loc;
			}
			else if (o is Int32)
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
		
		internal protected int[] GetNamedParameterLocs(string name)
		{
			object o = namedParameters[name];
			if (o == null)
			{
				QueryException qe = new QueryException("Named parameter does not appear in Query: " + name);
				qe.QueryString = queryString;
				throw qe;
			}
			if (o is Int32)
			{
				return new int[]{ ((Int32) o) };
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
			if (returnTypes.Count == 0 && scalarTypes.Count == 0)
			{
				//ie no select clause in HQL
				returnTypes = fromTypes;
			}
			
			int               size                  = returnTypes.Count;
			string            outerJoinedProperties = null;
			OuterJoinFragment outerjoin             = null;
			string            selectProperties;
			string            selectIdentifiers;

			if (!IsShallowQuery() && returnTypes.Count == 1 && typeMap.Count == 1)
			{
#if PORTED
				OuterJoinLoader ojl  = new OuterJoinLoader(factory.Dialect);
				string          name = (string) returnTypes[0];

				if (!IsName(name))
				{
					throw new QueryException("unknown type: " + name);
				}
				IQueryable persister = GetPersisterForName(name);
				associations = ojl.WalkTree(persister, name, factory);
				int joins = associations.Count;
				string[] ojsuffixes = new string[joins];
				for (int i = 0; i < joins; i++)
				{
					ojsuffixes[i] = System.Convert.ToString(i) + StringHelper.Underscore;
				}
				ojl.Suffixes = ojsuffixes;
				selectProperties = persister.PropertySelectFragment(name, StringHelper.EmptyString);
				selectIdentifiers = persister.IdentifierSelectFragment(name, StringHelper.EmptyString);
				outerJoinedProperties = ojl.SelectString(associations);
				outerjoin = ojl.OuterJoins(associations);
				persisters = new Queryable[joins + 1]; 
				suffixes = new string[joins + 1];
				persisters[joins] = persister;
				suffixes[joins] = StringHelper.EmptyString;
				for (int i = 0; i < joins; i++)
				{
					suffixes[i]   = System.Convert.ToString(i) + StringHelper.Underscore;
					persisters[i] = (IQueryable) ((OuterJoinLoader.OuterJoinableAssociation) associations[i]).Subpersister; //TODO: dont like the typecast to Queryable
				}
#else
#warning RenderSQL has not been PORTED

				throw new NotImplementedException("Not ported yet");
#endif
			}
			else
			{
				persisters = new IQueryable[size];
				suffixes = new string[size];
				for (int i = 0; i < size; i++)
				{
					string name = (string) returnTypes[i];
					if (!IsName(name))
					{
						throw new QueryException("unknown type: " + name);
					}
					persisters[i] = GetPersisterForName(name);
					suffixes[i]   = (size == 1) ? 
						StringHelper.EmptyString : 
						System.Convert.ToString(i) + StringHelper.Underscore;
				}
				selectProperties  = RenderPropertiesSelect();
				selectIdentifiers = RenderIdentifierSelect();
			}
			
			string selectPerhapsDistinct = "SELECT ";
			if (distinct)
			{
				selectPerhapsDistinct += "DISTINCT ";
			}
			string selectScalars   = RenderScalarSelect();
			scalarSelectString     = selectPerhapsDistinct + selectScalars;
			selectPropertiesString = selectPerhapsDistinct + selectIdentifiers + selectProperties;
			if (outerJoinedProperties != null && outerJoinedProperties.Length > 0)
			{
				selectPropertiesString += ", " + outerJoinedProperties;
			}
			//TODO: for some dialiects it would be appropriate to add the renderOrderByPropertiesSelect() to other select strings
			fromWhereString = RenderFromClause(outerjoin) + RenderWhereClause(outerjoin);
			
			if (scalarTypes.Count != size)
			{
				hasScalars = true;
				if (size != 0)
				{
					selectPropertiesString += ", ";
				}
				selectPropertiesString += selectScalars;
			}
			else
			{
				hasScalars = false;
			}
			
			int scalarSize = scalarTypes.Count;
			types = new IType[scalarSize];
			for (int i = 0; i < scalarSize; i++)
			{
				types[i] = (IType) scalarTypes[i];
			}
			
			scalarColumnNames = GenerateColumnNames(types, factory);
			
			// initialize the Set of queried identifier spaces (ie. tables)
			foreach (string key in collections.Values)
			{
				CollectionPersister p = GetCollectionPersister(key);
				AddIdentifierSpace(p.QualifiedTableName);
			}
			foreach (string key in typeMap.Keys)
			{
				IQueryable p = GetPersisterForName(key);
				AddIdentifierSpace(p.IdentifierSpace);
			}
			
		}
		
		private string RenderIdentifierSelect()
		{
			StringBuilder buf = new StringBuilder(40);
			int size = returnTypes.Count;
			
			for (int k = 0; k < size; k++)
			{
				string name = (string) returnTypes[k];
				string suffix = size == 1 ? 
					StringHelper.EmptyString : 
					System.Convert.ToString(k) + StringHelper.Underscore;
				buf.Append(persisters[k].IdentifierSelectFragment(name, suffix));
				if (k != size - 1)
				{
					buf.Append(StringHelper.CommaSpace);
				}
			}
			
			return buf.ToString();
		}
		
		private string RenderOrderByPropertiesSelect()
		{
			StringBuilder buf = new StringBuilder(10);
			
			//add the columns we are ordering by to the select ID select clause
			foreach (string token in orderByTokens)
			{
				if (token.LastIndexOf(".") > 0)
				{
					//ie. it is of form "foo.bar", not of form "asc" or "desc"
					buf.Append(StringHelper.CommaSpace).Append(token);
				}
			}
			
			return buf.ToString();
		}
		
		private string RenderPropertiesSelect()
		{
			StringBuilder buf = new StringBuilder(40);
			int size = returnTypes.Count;
			for (int k = 0; k < size; k++)
			{
				string suffix = (size == 1) ?
					StringHelper.EmptyString :
					System.Convert.ToString(k) + StringHelper.Underscore;
				string name = (string) returnTypes[k];
				buf.Append(persisters[k].PropertySelectFragment(name, suffix));
			}
			return buf.ToString();
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
				for (int k = 0; k < size; k++)
				{
					scalarTypes.Add(NHibernate.Association(persisters[k].MappedClass));
					
					string[] names = persisters[k].IdentifierColumnNames;
					for (int i = 0; i < names.Length; i++)
					{
						buf.Append(returnTypes[k]).Append(StringHelper.Dot).Append(names[i]);
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
								if (!isSubselect)
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
						string[] tokens = (string[]) next;
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
		
		private string RenderFromClause(OuterJoinFragment outerjoin)
		{
			int  index = 0;
			//FROM
			StringBuilder buf = new StringBuilder(120).Append(" FROM");
			foreach (string name in typeMap.Keys)
			{
				//render the " foo_table foo," bit
				IQueryable p = GetPersisterForName(name);
				bool includeSubclasses = returnTypes.Contains(name) && !IsShallowQuery();
				buf.Append(' ')
					.Append(p.FromTableFragment(name))
					.Append(p.FromJoinFragment(name, true, includeSubclasses));
				if (index++ < typeMap.Keys.Count || collections.Count != 0)
				{
					buf.Append(',');
				}
			}
			
			// add any outerjoins required for association fetching 
			// TODO: (need to move inside loop, eventually)
			if (outerjoin != null)
			{
				buf.Append(outerjoin.ToFromFragmentString());
			}

			index = 0;
			
			// --- PORT NOTE ---
			// old : collections.entrySet().iterator()
			// new : in collections) 
			// note: is this still the same??
			foreach (DictionaryEntry entry in collections) 
			{                          
				buf.Append(' ');
				string name = (string) entry.Key;
				string role = (string) entry.Value;
				CollectionPersister p = GetCollectionPersister(role);
				buf.Append(p.QualifiedTableName).Append(' ').Append(name);
				if (index++ < collections.Count)
				{
					buf.Append(',');
				}
			}
			
			return buf.ToString();
		}
		
		private string RenderWhereClause(OuterJoinFragment outerjoin)
		{
			StringBuilder inClassWheres = new StringBuilder(50);
			
			// add any outerjoins required for fetching associations
			// TODO: (need to move inside loop, eventually)
			if (outerjoin != null)
			{
				inClassWheres.Append(outerjoin.ToWhereFragmentString());
			}
			
			foreach (string name in typeMap.Keys)
			{
				
				IQueryable p = GetPersisterForName(name);
				AddIdentifierSpace(p.IdentifierSpace);
				
				//render the " and foo.class in ( 'Foo', 'Bar' ) " bit
				string where = p.QueryWhereFragment(name, returnTypes.Contains(name) && !IsShallowQuery());
				if (where != null)
				{
					inClassWheres.Append(where);
				}
			}
			
			//if ( inClassWheres.toString().toLowerCase().startsWith(" and ") ) {
			inClassWheres.Remove(0, 5); //remove the leading " and "
			//}
			
			StringBuilder buf = new StringBuilder(120);
			
			//WHERE
			StringBuilder whereTokenBuf = new StringBuilder(40);
			AppendTokens(whereTokenBuf, whereTokens);
			
			string part1 = inClassWheres.ToString().Trim();
			string part2 = joins.ToString().Trim();
			string part3 = whereTokenBuf.ToString().Trim();
			
			bool hasPart1 = part1.Length != 0;
			bool hasPart2 = part2.Length != 0;
			bool hasPart3 = part3.Length != 0;
			
			if (hasPart1 || hasPart2 || hasPart3)
			{
				buf.Append(" WHERE ");
			}
			if (hasPart1)
			{
				buf.Append(part1);
			}
			if (hasPart1 && hasPart2)
			{
				buf.Append(" AND ");
			}
			if (hasPart2)
			{
				buf.Append(part2);
			}
			if (hasPart3)
			{
				if (hasPart1 || hasPart2)
				{
					buf.Append(" AND (");
				}
				buf.Append(part3);
				if (hasPart1 || hasPart2)
				{
					buf.Append(')');
				}
			}
			
			if (groupByTokens.Count != 0)
			{
				//GROUP BY
				buf.Append(" GROUP BY ");
				AppendTokens(buf, groupByTokens);
			}
			
			if (havingTokens.Count != 0)
			{
				buf.Append(" HAVING ");
				AppendTokens(buf, havingTokens);
			}
			
			if (orderByTokens.Count != 0)
			{
				//ORDER BY
				buf.Append(" ORDER BY ");
				AppendTokens(buf, orderByTokens);
			}
			
			return buf.ToString();
		}
		
		private void  AppendTokens(StringBuilder buf, IEnumerable enumerator)
		{
			bool lastSpaceable = true;

			foreach (string token in enumerator)
			{
				bool spaceable = !dontSpace.Contains(token);
				if (spaceable && lastSpaceable)
				{
					buf.Append(' ');
				}
				lastSpaceable = spaceable;
				buf.Append(token);
			}
		}
		
		
		/// <summary> Is this query called by scroll() or iterate()?
		/// </summary>
		/// <returns>true if it is, false if it is called by Find() or list()
		/// 
		/// </returns>
		public virtual bool IsShallowQuery()
		{
			return shallowQuery;
		}
		
		// --- PORT NOTE ---
		// old : protected void  AddIdentifierSpace(ISerializable table)
		// new : protected void  AddIdentifierSpace(object table)
		// note: Java String implements Serializable but .Net doesn't. 
		//       Let's see what the impact will be cause it's called with "String" here!

		internal protected void  AddIdentifierSpace(object table)
		{                                                             
			identifierSpaces.Add(table);
			if (superQuery != null)
			{
				superQuery.AddIdentifierSpace(table);
			}
		}
		
		internal protected void  AddFromCollection(string elementName, string collectionRole)
		{
			//q.addCollection(collectionName, collectionRole);
			IType collectionElementType = GetCollectionPersister(collectionRole).ElementType;
			if (!collectionElementType.IsEntityType)
			{
				throw new QueryException("collection of values in filter: " + elementName);
			}
			EntityType elemType = (EntityType) collectionElementType;
			AddFromType(elementName, elemType.PersistentClass.FullName);
			
			CollectionPersister persister = GetCollectionPersister(collectionRole);
			string[] keyColumnNames = persister.KeyColumnNames;
			if (keyColumnNames.Length != 1)
			{
				throw new QueryException("composite-key collection in filter: " + collectionRole);
			}
			
			StringBuilder join = new StringBuilder(25);
			join.Append(" and "); // all conditions must begin with " and "
			
			if (persister.IsOneToMany)
			{
				join.Append(elementName)
					.Append(StringHelper.Dot)
					.Append(keyColumnNames[0])
					.Append(" = ?");
			}
			else
			{
				//many-to-many
				string collectionName = CreateNameForCollection(collectionRole);
				AddCollection(collectionName, collectionRole);
				join.Append(collectionName)
					.Append(StringHelper.Dot)
					.Append(keyColumnNames[0])
					.Append(" = ?");
				
				string[] idColumnNames = GetPersisterForName(elementName).IdentifierColumnNames;
				string[] eltColumnNames = persister.ElementColumnNames;
				for (int i = 0; i < idColumnNames.Length; i++)
				{
					join.Append(" and ")
						.Append(collectionName)
						.Append(StringHelper.Dot)
						.Append(eltColumnNames[i])
						.Append('=')
						.Append(elementName)
						.Append(StringHelper.Dot)
						.Append(idColumnNames[i]);
				}
			}
			AddJoin(join.ToString());
		}
		
		internal  protected string GetPathAlias(string path)
		{
			return (string) pathAliases[path];
		}
		
		internal protected string GetPathJoin(string path)
		{
			return (string) pathJoins[path];
		}
		
		internal protected void  AddPathAliasAndJoin(string path, string alias, string join)
		{
			pathAliases[path] = alias;
			pathJoins[path] = join;
		}
		
		
		protected override void  BindNamedParameters(IDbCommand ps, IDictionary namedParams, ISessionImplementor session)
		{
			if (namedParams != null)
			{
				foreach (DictionaryEntry e in namedParams)
				{
					string name = (string) e.Key;
					TypedValue typedval = (TypedValue) e.Value;
					int[] locs = GetNamedParameterLocs(name);
					for (int i = 0; i < locs.Length; i++)
					{
						typedval.Type.NullSafeSet(ps, typedval.Value, locs[i], session);
					}
				}
			}
		}
		
		public virtual IIterator Iterate(object[] values, IType[] types, RowSelection selection, IDictionary namedParams, ISessionImplementor session)
		{
			IDbCommand st = PrepareQueryStatement(ScalarSelectSQL, values, types, selection, false, session);
			try
			{
				BindNamedParameters(st, namedParams, session);
				SetMaxRows(st, selection);
				IDataReader rs = st.ExecuteReader();
				Advance(rs, selection, session);

				return new IteratorImpl(rs, session, ReturnTypes, ScalarColumnNames);
			}
			catch (System.Data.OleDb.OleDbException sqle)
			{
#if PORTED
				JDBCExceptionReporter.logExceptions(sqle);
				session.Batcher.CloseQueryStatement(st);
#else
#warning CloseQuery has not been PORTED
#endif

				throw sqle;
			}
		}
		
		public virtual IScrollableResults Scroll(object[] values, IType[] types, RowSelection selection, IDictionary namedParams, ISessionImplementor session)
		{
			
			IDbCommand st = PrepareQueryStatement(ScalarSelectSQL, values, types, selection, true, session);
			try
			{
				BindNamedParameters(st, namedParams, session);
				SetMaxRows(st, selection);
				IDataReader rs = st.ExecuteReader();
				Advance(rs, selection, session);

				return new ScrollableResultsImpl(rs, session, ReturnTypes);
			}
			catch (System.Data.OleDb.OleDbException sqle)
			{
#if PORTED
				JDBCExceptionReporter.logExceptions(sqle);
				session.Batcher.CloseQueryStatement(st);
#else
#warning CloseQuery has not been PORTED
#endif

				throw sqle;
			}
		}
		
		public static string[] ConcreteQueries(string query, ISessionFactoryImplementor factory)
		{
			Portable.StringTokenizer tokens = new Portable.StringTokenizer(query, StringHelper.CommaSpace);
			ArrayList placeholders = new ArrayList();
			ArrayList replacements = new ArrayList();
			int count = 0;
			bool check = false;
			while (tokens.HasMoreTokens())
			{
				string token = tokens.NextToken();
				if ("class".Equals(token.ToLower()))
				{
					check = true;
				}
				else if (check)
				{
					check = false;
					System.Type clazz = GetImportedClass(token, factory);
					if (clazz != null)
					{
						string[] implementors = factory.GetImplementors(clazz);
						string placeholder = "$clazz" + count + "$";
						query = StringHelper.ReplaceOnce(query, token, placeholder);
						if (implementors != null)
						{
							placeholders.Add(placeholder);
							replacements.Add(implementors);
						}
					}
				}
			}
			return StringHelper.Multiply(query, placeholders.GetEnumerator(), replacements.GetEnumerator());
		}
		
		private static System.Type GetImportedClass(string name, ISessionFactoryImplementor factory)
		{
			try
			{
				return ReflectHelper.ClassForName(name);
			}
			catch (Exception)
			{
				string[] imports = factory.Imports;
				for (int i = 0; i < imports.Length; i++)
				{
					try
					{
						return ReflectHelper.ClassForName(imports[i] + StringHelper.Dot + name);
					}
					catch (Exception)
					{
					}
				}
				return null;
			}
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
		
#if PORTED
		public override IList Find(ISessionImplementor session, object[] values, IType[] types, bool returnProxies, RowSelection selection, IDictionary namedParams)
		{
			return base.Find(session, values, types, returnProxies, selection, namedParams);
		}
#else
#warning Find has not been PORTED
#endif		
		
		protected override object GetResultColumnOrRow(object[] row, IDataReader rs, ISessionImplementor session)
		{
			IType[] returnTypes = ReturnTypes;
			if (hasScalars)
			{
				string[][] names = ScalarColumnNames;
				int queryCols = returnTypes.Length;
				if (queryCols == 1)
				{
					return returnTypes[0].NullSafeGet(rs, names[0], session, null);
				}
				else
				{
					object[] queryRow = new object[queryCols];
					for (int i = 0; i < queryCols; i++)
					{
						queryRow[i] = returnTypes[i].NullSafeGet(rs, names[i], session, null);
					}
					return queryRow;
				}
			}
			else if (returnTypes.Length == 1 && persisters.Length > 1)
			{
				// we are doing some outerjoining
				return row[persisters.Length - 1];
			}
			else
			{
				return (row.Length == 1) ? row[0] : row;
			}
			
		}
	}
}