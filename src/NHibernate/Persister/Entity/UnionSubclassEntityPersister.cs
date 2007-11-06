using System.Collections;
using System.Collections.Generic;
using System.Text;
using Iesi.Collections.Generic;
using NHibernate.Cache;
using NHibernate.Engine;
using NHibernate.Id;
using NHibernate.Mapping;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;
using NHibernate.Util;

namespace NHibernate.Persister.Entity
{
	public class UnionSubclassEntityPersister : AbstractEntityPersister
	{
		private readonly string tableName;
		private readonly string discriminatorSQLValue;
		private readonly string[] subclassClosure;
		private readonly Dictionary<int, System.Type> subclassByDiscriminatorValue = new Dictionary<int, System.Type>();
		private readonly string[] spaces;
		private string[] subclassSpaces;
		private readonly string subquery;
		private string[] constraintOrderedTableNames;
		private string[][] constraintOrderedKeyColumnNames;
		private readonly int discriminatorValue;

		public UnionSubclassEntityPersister(PersistentClass persistentClass, ICacheConcurrencyStrategy cache, 
			ISessionFactoryImplementor factory, IMapping mapping):base(persistentClass, cache, factory)
		{
			if (IdentifierGenerator is IdentityGenerator)
			{
				throw new MappingException("Cannot use identity column key generation with <union-subclass> mapping for: " + EntityName);
			}

			// TABLE

			tableName = persistentClass.Table.GetQualifiedName(factory.Dialect, factory.DefaultSchema);
			/*rootTableName = persistentClass.getRootTable().getQualifiedName( 
			factory.getDialect(), 
			factory.getDefaultCatalog(), 
			factory.getDefaultSchema() 
			);*/

			//Custom SQL

			SqlString sql;
			bool callable;
			ExecuteUpdateResultCheckStyle checkStyle;

			sql = persistentClass.CustomSQLInsert;
			callable = sql != null && persistentClass.IsCustomInsertCallable;
			checkStyle = sql == null ? ExecuteUpdateResultCheckStyle.Count: 
				(persistentClass.CustomSQLInsertCheckStyle ?? ExecuteUpdateResultCheckStyle.DetermineDefault(sql, callable));
			customSQLInsert = new SqlString[] { sql };
			insertCallable = new bool[] { callable };
			insertResultCheckStyles = new ExecuteUpdateResultCheckStyle[] { checkStyle };

			sql = persistentClass.CustomSQLUpdate;
			callable = sql != null && persistentClass.IsCustomUpdateCallable;
			checkStyle = sql == null ? ExecuteUpdateResultCheckStyle.Count : 
				(persistentClass.CustomSQLUpdateCheckStyle ?? ExecuteUpdateResultCheckStyle.DetermineDefault(sql, callable));
			customSQLUpdate = new SqlString[] { sql };
			updateCallable = new bool[] { callable };
			updateResultCheckStyles = new ExecuteUpdateResultCheckStyle[] { checkStyle };

			sql = persistentClass.CustomSQLDelete;
			callable = sql != null && persistentClass.IsCustomDeleteCallable;
			checkStyle = sql == null ? ExecuteUpdateResultCheckStyle.Count : 
				(persistentClass.CustomSQLDeleteCheckStyle ?? ExecuteUpdateResultCheckStyle.DetermineDefault(sql, callable));
			customSQLDelete = new SqlString[] { sql };
			deleteCallable = new bool[] { callable };
			deleteResultCheckStyles = new ExecuteUpdateResultCheckStyle[] { checkStyle };

			discriminatorSQLValue = persistentClass.SubclassId.ToString();
			discriminatorValue = persistentClass.SubclassId;

			// PROPERTIES

			int subclassSpan = persistentClass.SubclassSpan + 1;
			subclassClosure = new string[subclassSpan];
			subclassClosure[0] = EntityName;

			// SUBCLASSES
			subclassByDiscriminatorValue[persistentClass.SubclassId] = persistentClass.MappedClass;
			if (persistentClass.IsPolymorphic)
			{
				int k = 1;
				foreach (Subclass sc in persistentClass.SubclassIterator)
				{
					subclassClosure[k++] = sc.EntityName;
					subclassByDiscriminatorValue[sc.SubclassId] = sc.MappedClass;
				}
			}

			//SPACES
			//TODO: i'm not sure, but perhaps we should exclude
			//      abstract denormalized tables?
			spaces = ArrayHelper.Join(new string[] { tableName },
				ArrayHelper.ToStringArray(persistentClass.SynchronizedTables));

			HashedSet<string> subclassTables = new HashedSet<string>();
			foreach (Table table in persistentClass.SubclassTableClosureIterator)
			{
				subclassTables.Add(table.GetQualifiedName(factory.Dialect, factory.DefaultSchema));				
			}
			subclassSpaces = ArrayHelper.ToStringArray((ICollection<string>)subclassTables);

			subquery = GenerateSubquery(persistentClass, mapping);

			if (IsMultiTable)
			{
				int idColumnSpan = IdentifierColumnSpan;
				List<string> tableNames = new List<string>();
				ArrayList keyColumns = new ArrayList();
				if (!IsAbstract)
				{
					tableNames.Add(tableName);
					keyColumns.Add(IdentifierColumnNames);
				}
				foreach (Table tab in persistentClass.SubclassTableClosureIterator)
				{
					if (!tab.IsAbstractUnionTable)
					{
						string tName = tab.GetQualifiedName(factory.Dialect, factory.DefaultSchema);
						tableNames.Add(tName);
						string[] key = new string[idColumnSpan];
						int k = 0;
						foreach (Column col in tab.PrimaryKey.ColumnCollection)
						{
							key[k++] = col.GetQuotedName(factory.Dialect);
							if(k>idColumnSpan) break;
						}
						keyColumns.Add(key);
					}
					
				}
				constraintOrderedTableNames = ArrayHelper.ToStringArray((ICollection<string>)tableNames);
				constraintOrderedKeyColumnNames = ArrayHelper.To2DStringArray(keyColumns);
			}
			else
			{
				constraintOrderedTableNames = new string[] { tableName };
				constraintOrderedKeyColumnNames = new string[][] { IdentifierColumnNames };
			}

			InitLockers();

			InitSubclassPropertyAliasesMap(persistentClass);

			PostConstruct(mapping);
		}

		private string GenerateSubquery(PersistentClass model, IMapping mapping)
		{
			Dialect.Dialect dialect = Factory.Dialect;

			if (!model.HasSubclasses)
			{
				return model.Table.GetQualifiedName(dialect, Factory.DefaultSchema);
			}

			HashedSet<Column> columns = new HashedSet<Column>();
			foreach (Table table in model.SubclassTableClosureIterator)
			{
				if (!table.IsAbstractUnionTable)
				{
					foreach (Column column in table.ColumnCollection)
					{
						columns.Add(column);
					}
				}
			}

			StringBuilder buf = new StringBuilder().Append("( ");
			IEnumerable siter = new JoinedEnumerable(new SingletonEnumerable<PersistentClass>(model), model.SubclassIterator);

			foreach (PersistentClass clazz in siter)
			{
				Table table = clazz.Table;
				if (!table.IsAbstractUnionTable)
				{
					buf.Append("select ");
					foreach (Column col in columns)
					{
						if (!table.ContainsColumn(col))
						{
							SqlType sqlType = col.GetAutoSqlType(mapping);
							buf.Append(dialect.GetSelectClauseNullString(sqlType)).Append(" as ");
						}
						buf.Append(col.Name);
						buf.Append(", ");
					}
					buf.Append(clazz.SubclassId).Append(" as clazz_");
					buf.Append(" from ").Append(table.GetQualifiedName(dialect, Factory.DefaultSchema));
					buf.Append(" union ");
					if (dialect.SupportsUnionAll)
						buf.Append("all ");
				}
			}

			if (buf.Length > 2)
			{
				//chop the last union (all)
				buf.Length -= (dialect.SupportsUnionAll ? 11 : 7); //" union " : "all "
			}

			return buf.Append(" )").ToString();
		}

		public override string GetSubclassTableName(int j)
		{
			return TableName; //ie. the subquery! yuck!
		}

		protected override string[] GetSubclassTableKeyColumns(int j)
		{
			if (j != 0)
				throw new AssertionFailure("only one table");
			return IdentifierColumnNames;
		}

		protected override string DiscriminatorAlias
		{
			get { return DiscriminatorColumnName; }
		}

		protected override string VersionedTableName
		{
			get { return GetTableName(0); }
		}

		public override SqlString QueryWhereFragment(string alias, bool innerJoin, bool includeSubclasses)
		{
			throw new System.Exception("The method or operation is not implemented.");
		}

		public override string DiscriminatorSQLValue
		{
			get { return discriminatorSQLValue;}
		}

		public override object DiscriminatorValue
		{
			get { return discriminatorValue; }
		}

		public override object[] PropertySpaces
		{
			get { return spaces; }
		}

		public override Type.IType DiscriminatorType
		{
			get { return NHibernateUtil.Int32; }
		}

		public override System.Type GetSubclassForDiscriminatorValue(object value)
		{
			System.Type result;
			subclassByDiscriminatorValue.TryGetValue((int)value,out result);
			return result;
		}

		protected override int[] SubclassColumnTableNumberClosure
		{
			get { return new int[SubclassColumnClosure.Length]; }
		}

		protected override int[] SubclassFormulaTableNumberClosure
		{
			get { return new int[SubclassFormulaClosure.Length]; }
		}

		public override string TableName
		{
			get { return subquery; }
		}

		public override SqlString WhereJoinFragment(string alias, bool innerJoin, bool includeSubclasses)
		{
			return new SqlString("");
		}

		public override string DiscriminatorColumnName
		{
			get { return "clazz_"; }
		}

		public override string GetSubclassPropertyTableName(int i)
		{
			return TableName; //ie. the subquery! yuck!
		}

		public override bool IsCacheInvalidationRequired
		{
			get { return HasFormulaProperties || (!IsVersioned && (UseDynamicUpdate || TableSpan > 1)); }
		}

		protected override int GetSubclassPropertyTableNumber(int i)
		{
			return 0;
		}

		public override string GetPropertyTableName(string propertyName)
		{
			//TODO: check this....
			return TableName;
		}

		protected override int[] PropertyTableNumbersInSelect
		{
			get { return new int[PropertySpan]; }
		}

		public override string FilterFragment(string alias)
		{
			return HasWhere ? " and " + GetSQLWhereString(alias) : "";
		}

		protected override bool IsClassOrSuperclassTable(int j)
		{
			if (j != 0)
				throw new AssertionFailure("only one table");
			return true;
		}

		protected override int SubclassTableSpan
		{
			get { return 1; }
		}

		protected override int TableSpan
		{
			get { return 1; }
		}

		protected override string GetTableName(int table)
		{
			return tableName;
		}

		protected override string[] GetKeyColumns(int table)
		{
			return IdentifierColumnNames;
		}

		protected override bool IsPropertyOfTable(int property, int table)
		{
			return true;
		}

		protected override int[] PropertyTableNumbers
		{
			get { return new int[PropertySpan]; }
		}

		public override object[] QuerySpaces
		{
			get { return subclassSpaces; }
		}
	}
}