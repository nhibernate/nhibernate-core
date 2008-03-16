using System.Collections.Generic;
using System.Text;
using Iesi.Collections.Generic;
using NHibernate.Cache;
using NHibernate.Cfg;
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
		private readonly string subquery;
		private readonly string tableName;
		private readonly string[] subclassClosure;
		private readonly string[] spaces;
		private readonly string[] subclassSpaces;
		private readonly string discriminatorSQLValue;
		private readonly object discriminatorValue;
		private readonly Dictionary<int, string> subclassByDiscriminatorValue = new Dictionary<int, string>();
		private readonly string[] constraintOrderedTableNames;
		private readonly string[][] constraintOrderedKeyColumnNames;

		public UnionSubclassEntityPersister(PersistentClass persistentClass, ICacheConcurrencyStrategy cache, 
			ISessionFactoryImplementor factory, IMapping mapping):base(persistentClass, cache, factory)
		{
			if (IdentifierGenerator is IdentityGenerator)
			{
				throw new MappingException("Cannot use identity column key generation with <union-subclass> mapping for: " + EntityName);
			}

			// TABLE

			tableName =
				persistentClass.Table.GetQualifiedName(factory.Dialect, factory.Settings.DefaultCatalogName,
				                                       factory.Settings.DefaultSchemaName);

			#region Custom SQL

			SqlString sql;
			bool callable;
			ExecuteUpdateResultCheckStyle checkStyle;

			sql = persistentClass.CustomSQLInsert;
			callable = sql != null && persistentClass.IsCustomInsertCallable;
			checkStyle = sql == null
			             	? ExecuteUpdateResultCheckStyle.Count
			             	: (persistentClass.CustomSQLInsertCheckStyle
			             	   ?? ExecuteUpdateResultCheckStyle.DetermineDefault(sql, callable));
			customSQLInsert = new SqlString[] { sql };
			insertCallable = new bool[] { callable };
			insertResultCheckStyles = new ExecuteUpdateResultCheckStyle[] { checkStyle };

			sql = persistentClass.CustomSQLUpdate;
			callable = sql != null && persistentClass.IsCustomUpdateCallable;
			checkStyle = sql == null
			             	? ExecuteUpdateResultCheckStyle.Count
			             	: (persistentClass.CustomSQLUpdateCheckStyle
			             	   ?? ExecuteUpdateResultCheckStyle.DetermineDefault(sql, callable));
			customSQLUpdate = new SqlString[] { sql };
			updateCallable = new bool[] { callable };
			updateResultCheckStyles = new ExecuteUpdateResultCheckStyle[] { checkStyle };

			sql = persistentClass.CustomSQLDelete;
			callable = sql != null && persistentClass.IsCustomDeleteCallable;
			checkStyle = sql == null
			             	? ExecuteUpdateResultCheckStyle.Count
			             	: (persistentClass.CustomSQLDeleteCheckStyle
			             	   ?? ExecuteUpdateResultCheckStyle.DetermineDefault(sql, callable));
			customSQLDelete = new SqlString[] { sql };
			deleteCallable = new bool[] { callable };
			deleteResultCheckStyles = new ExecuteUpdateResultCheckStyle[] { checkStyle };

			#endregion

			discriminatorValue = persistentClass.SubclassId;
			discriminatorSQLValue = persistentClass.SubclassId.ToString();

			#region PROPERTIES

			int subclassSpan = persistentClass.SubclassSpan + 1;
			subclassClosure = new string[subclassSpan];
			subclassClosure[0] = EntityName;

			#endregion

			#region SUBCLASSES

			subclassByDiscriminatorValue[persistentClass.SubclassId] = persistentClass.EntityName;
			if (persistentClass.IsPolymorphic)
			{
				int k = 1;
				foreach (Subclass sc in persistentClass.SubclassIterator)
				{
					subclassClosure[k++] = sc.EntityName;
					subclassByDiscriminatorValue[sc.SubclassId] = sc.EntityName;
				}
			}

			#endregion

			#region SPACES
			//TODO: i'm not sure, but perhaps we should exclude abstract denormalized tables?

			int spacesSize = 1 + persistentClass.SynchronizedTables.Count;
			spaces = new string[spacesSize];
			spaces[0] = tableName;
			IEnumerator<string> iSyncTab = persistentClass.SynchronizedTables.GetEnumerator();
			for (int i = 1; i < spacesSize; i++)
			{
				iSyncTab.MoveNext();
				spaces[i] = iSyncTab.Current;
			}

			HashedSet<string> subclassTables = new HashedSet<string>();
			foreach (Table table in persistentClass.SubclassTableClosureIterator)
			{
				subclassTables.Add(
					table.GetQualifiedName(factory.Dialect, factory.Settings.DefaultCatalogName, factory.Settings.DefaultSchemaName));
			}
			subclassSpaces = new string[subclassSpaces.Length];
			subclassTables.CopyTo(subclassSpaces, 0);

			subquery = GenerateSubquery(persistentClass, mapping);

			if (IsMultiTable)
			{
				int idColumnSpan = IdentifierColumnSpan;
				List<string> tableNames = new List<string>();
				List<string[]> keyColumns = new List<string[]>();
				if (!IsAbstract)
				{
					tableNames.Add(tableName);
					keyColumns.Add(IdentifierColumnNames);
				}
				foreach (Table tab in persistentClass.SubclassTableClosureIterator)
				{
					if (!tab.IsAbstractUnionTable)
					{
						string _tableName =
							tab.GetQualifiedName(factory.Dialect, factory.Settings.DefaultCatalogName, factory.Settings.DefaultSchemaName);
						tableNames.Add(_tableName);

						List<string> key = new List<string>(idColumnSpan);
						foreach (Column column in tab.PrimaryKey.ColumnIterator)
							key.Add(column.GetQuotedName(factory.Dialect));

						keyColumns.Add(key.ToArray());
					}					
				}

				constraintOrderedTableNames = tableNames.ToArray();
				constraintOrderedKeyColumnNames = keyColumns.ToArray();
			}
			else
			{
				constraintOrderedTableNames = new string[] { tableName };
				constraintOrderedKeyColumnNames = new string[][] { IdentifierColumnNames };
			}
			#endregion

			InitLockers();

			InitSubclassPropertyAliasesMap(persistentClass);

			PostConstruct(mapping);
		}

		public override string[] QuerySpaces
		{
			get { return subclassSpaces; }
		}

		public override Type.IType DiscriminatorType
		{
			get { return NHibernateUtil.Int32; }
		}

		public override string DiscriminatorSQLValue
		{
			get { return discriminatorSQLValue;}
		}

		public override object DiscriminatorValue
		{
			get { return discriminatorValue; }
		}

		public string[] SubclassClosure
		{
			get { return subclassClosure; }
		}

		public override string[] PropertySpaces
		{
			get { return spaces; }
		}

		protected internal override int[] PropertyTableNumbersInSelect
		{
			get { return new int[PropertySpan]; }
		}

		public override bool IsMultiTable
		{
			get
			{
				// This could also just be true all the time...
				return IsAbstract || HasSubclasses;
			}
		}

		protected override int[] SubclassColumnTableNumberClosure
		{
			get { return new int[SubclassColumnClosure.Length]; }
		}

		protected override int[] SubclassFormulaTableNumberClosure
		{
			get { return new int[SubclassFormulaClosure.Length]; }
		}

		protected internal override int[] PropertyTableNumbers
		{
			get { return new int[PropertySpan]; }
		}

		public override string[] ConstraintOrderedTableNameClosure
		{
			get { return constraintOrderedTableNames; }
		}

		public override string[][] ContraintOrderedTableKeyColumnClosure
		{
			get { return constraintOrderedKeyColumnNames; }
		}

		public override string TableName
		{
			get { return subquery; }
		}

		public override string GetSubclassForDiscriminatorValue(object value)
		{
			string result;
			subclassByDiscriminatorValue.TryGetValue((int)value, out result);
			return result;
		}

		protected internal virtual bool IsDiscriminatorFormula
		{
			get { return false; }
		}

		/// <summary> Generate the SQL that selects a row by id</summary>
		protected internal virtual SqlString GenerateSelectString(LockMode lockMode)
		{
			SqlSimpleSelectBuilder select = new SqlSimpleSelectBuilder(Factory.Dialect, Factory)
				.SetLockMode(lockMode)
				.SetTableName(TableName)
				.AddColumns(IdentifierColumnNames)
				.AddColumns(SubclassColumnClosure, SubclassColumnAliasClosure, SubclassColumnLazyiness)
				.AddColumns(SubclassFormulaClosure, SubclassFormulaAliasClosure, SubclassFormulaLazyiness);
			//TODO: include the rowids!!!!
			if (HasSubclasses)
			{
				if (IsDiscriminatorFormula)
				{
					select.AddColumn(DiscriminatorFormula, DiscriminatorAlias);
				}
				else
				{
					select.AddColumn(DiscriminatorColumnName, DiscriminatorAlias);
				}
			}
			if (Factory.Settings.IsCommentsEnabled)
			{
				select.SetComment("load " + EntityName);
			}
			return select.AddWhereFragment(IdentifierColumnNames, IdentifierType, "=").ToSqlString();
		}

		protected internal string DiscriminatorFormula
		{
			get { return null; } // NH : what this mean ? (see GenerateSelectString) 
		}

		protected override string GetTableName(int table)
		{
			return tableName;
		}

		protected override string[] GetKeyColumns(int table)
		{
			return IdentifierColumnNames;
		}

		protected override bool IsTableCascadeDeleteEnabled(int j)
		{
			return false;
		}

		protected override bool IsPropertyOfTable(int property, int j)
		{
			return true;
		}

		public override string FromTableFragment(string name)
		{
			return TableName + ' ' + name;
		}

		public override string FilterFragment(string alias)
		{
			return HasWhere ? " and " + GetSQLWhereString(alias) : string.Empty;
		}

		public override string GetSubclassPropertyTableName(int i)
		{
			return TableName; //ie. the subquery! yuck!
		}

		protected override void AddDiscriminatorToSelect(SelectFragment select, string name, string suffix)
		{
			select.AddColumn(name, DiscriminatorColumnName, DiscriminatorAlias);
		}

		protected override int GetSubclassPropertyTableNumber(int i)
		{
			return 0;
		}

		public override int GetSubclassPropertyTableNumber(string propertyName)
		{
			return 0;
		}

		protected override int TableSpan
		{
			get { return 1; }
		}

		protected string GenerateSubquery(PersistentClass model, IMapping mapping)
		{
			Dialect.Dialect dialect = Factory.Dialect;
			Settings settings = Factory.Settings;

			if (!model.HasSubclasses)
			{
				return model.Table.GetQualifiedName(dialect, settings.DefaultCatalogName, settings.DefaultSchemaName);
			}

			HashedSet<Column> columns = new HashedSet<Column>();
			foreach (Table table in model.SubclassTableClosureIterator)
			{
				if (!table.IsAbstractUnionTable)
				{
					foreach (Column column in table.ColumnIterator)
						columns.Add(column);
				}
			}

			StringBuilder buf = new StringBuilder().Append("( ");
			IEnumerable<PersistentClass> siter =
				new JoinedEnumerable<PersistentClass>(new SingletonEnumerable<PersistentClass>(model),
				                                      new SafetyEnumerable<PersistentClass>(model.SubclassIterator));

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
							SqlType sqlType = col.GetSqlTypeCode(mapping);
							buf.Append(dialect.GetSelectClauseNullString(sqlType)).Append(" as ");
						}
						buf.Append(col.Name);
						buf.Append(StringHelper.CommaSpace);
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

		protected override string[] GetSubclassTableKeyColumns(int j)
		{
			if (j != 0)
				throw new AssertionFailure("only one table");
			return IdentifierColumnNames;
		}

		public override string GetSubclassTableName(int j)
		{
			if (j != 0)
				throw new AssertionFailure("only one table");
			return tableName;
		}

		protected override int SubclassTableSpan
		{
			get { return 1; }
		}

		protected override bool IsClassOrSuperclassTable(int j)
		{
			if (j != 0)
				throw new AssertionFailure("only one table");
			return true;
		}

		public override string GetPropertyTableName(string propertyName)
		{
			//TODO: check this....
			return TableName;
		}
	}
}