//$Id$
using System;
using System.Collections;
using System.Text;
using NHibernate;
using NHibernate.Collection;
using NHibernate.Persister;
using NHibernate.Type;
using NHibernate.Util;
using NHibernate.Portable;

namespace NHibernate.Hql
{	
	/// <summary> 
	/// Parses an expression of the form foo.bar.baz and builds up an expression
	/// involving two less table joins than there are path components.
	/// </summary>
	public class PathExpressionParser : IParser
	{
		public const string EntityID = "id";
		public const string EntityClass = "class";
		public const string CollectionSize = "size";
		public const string CollectionElements = "elements";
		public const string CollectionIndices = "indices";
		public const string CollectionMaxIndex = "maxIndex";
		public const string CollectionMinIndex = "minIndex";
		public const string CollectionMaxElement = "maxElement";
		public const string CollectionMinElement = "minElement";
		
		private int dotcount;
		protected string currentName;
		protected string currentProperty;
		protected StringBuilder join;
		protected string[] columns;
		protected string[] collectionElementColumns;
		private string collectionName;
		private string collectionRole;
		private string collectionTable;
		protected IType collectionElementType;
		private string componentPath;
		protected IType type;
		private string path;
		private bool skippedId;
		private bool continuation;
		private bool expectingCollectionIndex;
		private LinkedList collectionElements;

		public sealed class CollectionElement
		{
			public IType         Type;
			public bool          IsOneToMany;
			public string        Alias;
			public string[]      ElementColumns;
			public string        Join;
			public StringBuilder IndexValue = new StringBuilder();
		}
		

		public PathExpressionParser()
		{
			collectionElements = new LinkedList();
		}

		private string PropertyPath
		{
			get
			{
				if (currentProperty == null)
				{
					return EntityID;
				}
				else
				{
					return currentProperty + 
						(skippedId ? StringHelper.Dot + EntityID : StringHelper.EmptyString) + 
						((componentPath == null) ? StringHelper.EmptyString : StringHelper.Dot + componentPath);
				}
			}
		}

		private void SetType(QueryTranslator value)
		{
			if (currentProperty == null)
			{
				IClassPersister p = value.GetPersisterForName(currentName);
				type = NHibernate.Association(p.MappedClass);
			}
			else
			{
				type = GetPropertyType(value);
			}
		}

		public string LastCollectionElementIndexValue
		{
			set
			{
				((CollectionElement) collectionElements.GetLast()).IndexValue.Append(value);
			}
		}
		
		public string WhereJoin
		{
			get { return join.ToString(); }
		}

		public string WhereColumn
		{
			get
			{
				if (columns.Length != 1)
					throw new QueryException("path expression ends in a composite value");
				return columns[0];
			}
		}

		public string[] WhereColumns
		{
			get	{ return columns; }			
		}

		public IType WhereColumnType
		{
			get	{ return type; }			
		}

		public string CollectionName
		{
			get	{ return collectionName; }
			
		}

		public string CollectionRole
		{
			get	{ return collectionRole; }
		}

		public string CollectionTable
		{
			get { return collectionTable; }
		}
		
		private void AddJoin(string name, string[] rhsCols, QueryTranslator q)
		{
			string[] lhsCols = CurrentColumns(q);
			for (int i = 0; i < rhsCols.Length; i++)
			{
				join.Append(" and ")
					.Append(lhsCols[i])
					.Append('=')
					.Append(name)
					.Append(StringHelper.Dot)
					.Append(rhsCols[i]);
			}
		}
		
		public string ContinueFromManyToMany(System.Type clazz, string[] joinColumns, QueryTranslator q)
		{
			Start(q);
			continuation = true;
			currentName = q.CreateNameFor(clazz);
			q.AddType(currentName, clazz.FullName);
			
			for (int i = 0; i < joinColumns.Length; i++)
			{
				join.Append(" and ")
					.Append(joinColumns[i])
					.Append('=')
					.Append(currentName)
					.Append(StringHelper.Dot)
					.Append(q.GetPersister(clazz)
					.IdentifierColumnNames[i]);
			}
			return currentName;
		}
		
		public string ContinueFromSubcollection(string role, string[] joinColumns, QueryTranslator q)
		{
			Start(q);
			continuation = true;
			collectionName = q.CreateNameForCollection(role);
			collectionRole = role;
			currentName = null;
			currentProperty = null;
			CollectionPersister p = q.GetCollectionPersister(role);
			collectionTable = p.QualifiedTableName;
			
			for (int i = 0; i < joinColumns.Length; i++)
			{
				join.Append(" and ")
					.Append(joinColumns[i])
					.Append('=')
					.Append(collectionName)
					.Append(StringHelper.Dot)
					.Append(p.KeyColumnNames[i]);
			}
			return collectionName;
		}
		
		public void Token(string token, QueryTranslator q)
		{
			path += token;
			
			string alias = q.GetPathAlias(path);
			if (alias != null)
			{
				Reset(); //reset the dotcount (but not the path)
				currentName = alias; //after reset!
				join.Append(q.GetPathJoin(path)); //after reset!
			}
			else if (".".Equals(token))
			{
				dotcount++;
			}
			else
			{
				if (dotcount == 0)
				{
					if (!continuation)
					{
						if (!q.IsName(token))
							throw new QueryException("undefined alias: " + token);
						currentName = token;
					}
				}
				else if (dotcount == 1)
				{
					if (currentName != null)
					{
						currentProperty = token;
					}
					else if (collectionName != null)
					{
						CollectionPersister p = q.GetCollectionPersister(collectionRole);
						DoCollectionProperty(token, p, collectionName);
						continuation = false;
					}
					else
					{
						throw new QueryException("unexpected");
					}
				}
				else
				{
					// dotcount>=2
					
					// Do the corresponding RHS
					IType propertyType = GetPropertyType(q);
					
					if (propertyType == null)
					{
						throw new QueryException("unresolved property: " + currentProperty);
					}
					
					if (propertyType.IsComponentType)
					{
						if (componentPath == null)
						{
							componentPath = token;
						}
						else
						{
							componentPath += StringHelper.Dot + token;
						}
					}
					else
					{
						
						if (propertyType.IsEntityType)
						{
							System.Type memberClass = ((EntityType) propertyType).PersistentClass;
							IQueryable memberPersister = q.GetPersister(memberClass);
							if (EntityID.Equals(token) || (memberPersister.HasIdentifierProperty && memberPersister.IdentifierPropertyName.Equals(token)))
							{
								// special shortcut for id properties, skip the join!
								// this must only occur at the _end_ of a path expression
								skippedId = true;
							}
							else
							{
								
								string name = q.CreateNameFor(memberClass);
								q.AddType(name, memberClass.FullName);
								string[] keyColNames = memberPersister.IdentifierColumnNames;
								AddJoin(name, keyColNames, q);
								currentName = name;
								currentProperty = token;
								q.AddPathAliasAndJoin(path.Substring(0, (path.LastIndexOf((System.Char) StringHelper.Dot)) - (0)), name, join.ToString());
								
							}
							componentPath = null;
						}
						else if (propertyType.IsPersistentCollectionType)
						{
							
							collectionRole = ((PersistentCollectionType) propertyType).Role;
							CollectionPersister p = q.GetCollectionPersister(collectionRole);
							string[] colNames = p.KeyColumnNames;
							string name = q.CreateNameForCollection(collectionRole);
							AddJoin(name, colNames, q);
							DoCollectionProperty(token, p, name);
							collectionName = name;
							collectionTable = p.QualifiedTableName;
							currentName = null;
							currentProperty = null;
							componentPath = null;
						}
						else
						{
							if (token != null)
								throw new QueryException("dereferenced: " + currentProperty);
						}
						
					}
					
				}
			}
		}
		
		protected IType GetPropertyType(QueryTranslator q)
		{
			return q.GetPersisterForName(currentName).GetPropertyType(PropertyPath);
		}
		
		protected string[] CurrentColumns(QueryTranslator q)
		{
			return q.GetPersisterForName(currentName).ToColumns(currentName, PropertyPath);
		}
		
		private void Reset()
		{
			join = new StringBuilder(50);
			dotcount = 0;
			currentName = null;
			currentProperty = null;
			collectionName = null;
			collectionRole = null;
			collectionTable = null;
			collectionElementColumns = null;
			collectionElementType = null;
			componentPath = null;
			type = null;
			collectionName = null;
			columns = null;
			expectingCollectionIndex = false;
			skippedId = false;
			continuation = false;
		}
		
		public void Start(QueryTranslator q)
		{
			if (!continuation)
			{
				Reset();
				path = StringHelper.EmptyString;
			}
		}
		
		public virtual void End(QueryTranslator q)
		{
			if (IsCollectionValued())
			{
				columns = collectionElementColumns;
				type = collectionElementType;
			}
			else
			{
				
				if (!continuation)
				{
					IType propertyType = GetPropertyType(q);
					if (propertyType != null && propertyType.IsPersistentCollectionType)
					{
						collectionRole = ((PersistentCollectionType) propertyType).Role;
						collectionName = q.CreateNameForCollection(collectionRole);
					}
				}
				if (collectionRole != null)
				{
					//special case; expecting: [index]
					CollectionPersister memberPersister = q.GetCollectionPersister(collectionRole);
					if (!memberPersister.HasIndex)
						throw new QueryException("unindexed collection before []");
					
					string[] keyCols = memberPersister.KeyColumnNames;
					
					if (!continuation)
						AddJoin(collectionName, keyCols, q);
					
					string[] indexCols = memberPersister.IndexColumnNames;
					if (indexCols.Length != 1)
						throw new QueryException("composite-index appears in []");
					join.Append(" and ")
						.Append(collectionName)
						.Append(StringHelper.Dot)
						.Append(indexCols[0])
						.Append("=");
					
					string[] eltCols = memberPersister.ElementColumnNames;
					//if ( eltCols.length!=1 ) throw new QueryException("composite-id collection element []");
					
					CollectionElement elem = new CollectionElement();
					elem.ElementColumns = StringHelper.Prefix(eltCols, collectionName + StringHelper.Dot);
					elem.Type           = memberPersister.ElementType;
					elem.IsOneToMany    = memberPersister.IsOneToMany;
					elem.Alias          = collectionName;
					elem.Join           = join.ToString();
					collectionElements.AddLast(elem);
					SetExpectingCollectionIndex();
					
					q.AddCollection(collectionName, collectionRole);
				}
				else
				{
					columns = CurrentColumns(q);
					SetType(q);
				}
				
			}
			
			//important!!
			continuation = false;
			
		}
		
		public CollectionElement LastCollectionElement()
		{
			return (CollectionElement) collectionElements.RemoveLast();
		}

		public bool IsExpectingCollectionIndex()
		{
			return expectingCollectionIndex;
		}

		protected virtual void  SetExpectingCollectionIndex()
		{
			expectingCollectionIndex = true;
		}
		
		public string GetCollectionSubquery(string extraJoin)
		{
			if (extraJoin != null)
				join.Append(extraJoin);
			
			return new StringBuilder("SELECT ")
				.Append(StringHelper.Join(", ", collectionElementColumns))
				.Append(" FROM ")
				.Append(collectionTable)
				.Append(' ')
				.Append(collectionName)
				.Append(" WHERE ")
				.Append(join.ToString().Substring(5))
				.ToString();
		}
		
		public bool IsCollectionValued()
		{
			return collectionElementColumns != null;
		}
		
		public void  AddFromCollection(QueryTranslator q, string elementName)
		{
			
			if (collectionElementType == null)
				throw new QueryException("must specify 'elements' for collection valued property in from clause: " + elementName);
			
			if (!collectionElementType.IsEntityType)
				throw new QueryException("collection of values in from clause: " + elementName);

			EntityType elemType = (EntityType) collectionElementType;
			q.AddFromType(elementName, elemType.PersistentClass.FullName);
			
			CollectionPersister persister = q.GetCollectionPersister(collectionRole);
			if (persister.IsOneToMany)
			{
				q.AddJoin(StringHelper.Replace(join.ToString(), collectionName, elementName));
			}
			else
			{
				q.AddCollection(collectionName, collectionRole);
				string[] keyColumnNames = q.GetPersisterForName(elementName).IdentifierColumnNames;
				
				for (int i = 0; i < keyColumnNames.Length; i++)
				{
					join.Append(" and ")
						.Append(collectionElementColumns[i])
						.Append('=')
						.Append(elementName)
						.Append(StringHelper.Dot)
						.Append(keyColumnNames[i]);
				}
				q.AddJoin(join.ToString());
			}
		}
		
		
		private void DoCollectionProperty(string token, CollectionPersister memberPersister, string name)
		{
			if (token.Equals(CollectionElements))
			{
				string[] cols = memberPersister.ElementColumnNames;
				collectionElementColumns = StringHelper.Prefix(cols, name + StringHelper.Dot);
				collectionElementType = memberPersister.ElementType;
			}
			else if (token.Equals(CollectionIndices))
			{
				if (!memberPersister.HasIndex)
					throw new QueryException("unindexed collection before .indices");
				string[] cols = memberPersister.IndexColumnNames;
				collectionElementColumns = StringHelper.Prefix(cols, name + StringHelper.Dot);
				collectionElementType = memberPersister.IndexType;
			}
			else if (token.Equals(CollectionSize))
			{
				collectionElementColumns = new string[]{"count(*)"};
				collectionElementType = NHibernate.Integer;
			}
			else if (token.Equals(CollectionMaxIndex))
			{
				if (!memberPersister.HasIndex)
					throw new QueryException("unindexed collection before .maxIndex");
				string[] cols = memberPersister.IndexColumnNames;
				if (cols.Length != 1)
					throw new QueryException("composite collection index in maxIndex");
				collectionElementColumns = new string[]{"max(" + cols[0] + StringHelper.ClosedParen};
				collectionElementType = memberPersister.IndexType;
			}
			else if (token.Equals(CollectionMinIndex))
			{
				if (!memberPersister.HasIndex)
					throw new QueryException("unindexed collection before .minIndex");
				string[] cols = memberPersister.IndexColumnNames;
				if (cols.Length != 1)
					throw new QueryException("composite collection index in minIndex");
				collectionElementColumns = new string[]{"min(" + cols[0] + StringHelper.ClosedParen};
				collectionElementType = memberPersister.IndexType;
			}
			else if (token.Equals(CollectionMaxElement))
			{
				string[] cols = memberPersister.ElementColumnNames;
				if (cols.Length != 1)
					throw new QueryException("composite collection element in maxElement");
				collectionElementColumns = new string[]{"max(" + cols[0] + StringHelper.ClosedParen};
				collectionElementType = memberPersister.ElementType;
			}
			else if (token.Equals(CollectionMinElement))
			{
				string[] cols = memberPersister.ElementColumnNames;
				if (cols.Length != 1)
					throw new QueryException("composite collection element in minElement");
				collectionElementColumns = new string[]{"min(" + cols[0] + StringHelper.ClosedParen};
				collectionElementType = memberPersister.ElementType;
			}
			else
			{
				throw new QueryException("expecting 'elements' or 'indices' after " + currentProperty + StringHelper.Dot);
			}
		}
	}
}