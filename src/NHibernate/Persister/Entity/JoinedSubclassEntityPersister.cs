using System;
using System.Collections.Generic;
using NHibernate.Cache;
using NHibernate.Engine;
using NHibernate.Mapping;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Persister.Entity
{
	/// <summary>
	/// A <c>IEntityPersister</c> implementing the normalized "table-per-subclass" mapping strategy
	/// </summary>
	public class JoinedSubclassEntityPersister : AbstractEntityPersister
	{
		// the class hierarchy structure
		private readonly int tableSpan;

		// all of the table names that this Persister uses for just its data 
		// it is indexed as tableNames[tableIndex]
		// for both the superclass and subclass the index of 0=basetable
		// for the base class there is only 1 table
		private readonly string[] tableNames;

		private readonly string[] naturalOrderTableNames;
		// two dimensional array that is indexed as tableKeyColumns[tableIndex][columnIndex]
		private readonly string[][] tableKeyColumns;
		private readonly string[][] naturalOrderTableKeyColumns;
		private readonly bool[] naturalOrderCascadeDeleteEnabled;
		private readonly string[] spaces;

		// the Type of objects for the subclass
		// the array is indexed as subclassClosure[subclassIndex].  
		// The length of the array is the number of subclasses + 1 for the Base Class.
		// The last index of the array contains the Type for the Base Class.
		// in the example of JoinedSubclassBase/One the values are :
		// subclassClosure[0] = JoinedSubclassOne
		// subclassClosure[1] = JoinedSubclassBase
		private readonly string[] subclassClosure;

		// the names of the tables for the subclasses
		// the array is indexed as subclassTableNameColsure[tableIndex] = "tableName"
		// for the RootClass the index 0 is the base table
		// for the subclass the index 0 is also the base table
		private readonly string[] subclassTableNameClosure;

		// the names of the columns that are the Keys for the table - I don't know why they would
		// be different - I thought the subclasses didn't have their own PK, but used the one generated
		// by the base class??
		// the array is indexed as subclassTableKeyColumnClosure[tableIndex][columnIndex] = "columnName"
		private readonly string[][] subclassTableKeyColumnClosure;

		// TODO: figure out what this is being used for - when initializing the base class the values
		// are isClassOrSuperclassTable[0] = true, isClassOrSuperclassTable[1] = false
		// when initialized the subclass the values are [0]=true, [1]=true.  I believe this is telling
		// us that the table is used to populate this class or the superclass.
		// I would guess this is telling us specifically which tables this Persister will write information to.
		private readonly bool[] isClassOrSuperclassTable;

		// properties of this class, including inherited properties
		private readonly int[] naturalOrderPropertyTableNumbers;

		// the index of the table that the property is coming from
		// the array is indexed as propertyTables[propertyIndex] = tableIndex 
		private readonly int[] propertyTableNumbers;

		// the closure of all properties in the entire hierarchy including
		// subclasses and superclasses of this class
		private readonly int[] subclassPropertyTableNumberClosure;

		// the closure of all columns used by the entire hierarchy including
		// subclasses and superclasses of this class
		private readonly int[] subclassColumnTableNumberClosure;
		private readonly int[] subclassFormulaTableNumberClosure;

		// subclass discrimination works by assigning particular
		// values to certain combinations of null primary key
		// values in the outer join using an SQL CASE
		private readonly Dictionary<object, string> subclassesByDiscriminatorValue = new Dictionary<object, string>();
		private readonly string[] discriminatorValues;
		private readonly string[] notNullColumnNames;
		private readonly int[] notNullColumnTableNumbers;

		private readonly string[] constraintOrderedTableNames;
		private readonly string[][] constraintOrderedKeyColumnNames;
		private readonly string discriminatorSQLString;
		private readonly object discriminatorValue;

		/// <summary>
		/// Constructs the NormalizedEntityPerister for the PersistentClass.
		/// </summary>
		/// <param name="persistentClass">The PersistentClass to create the EntityPersister for.</param>
		/// <param name="cache">The configured <see cref="ICacheConcurrencyStrategy" />.</param>
		/// <param name="factory">The SessionFactory that this EntityPersister will be stored in.</param>
		/// <param name="mapping">The mapping used to retrieve type information.</param>
		public JoinedSubclassEntityPersister(PersistentClass persistentClass, ICacheConcurrencyStrategy cache,
		                                     ISessionFactoryImplementor factory, IMapping mapping)
			: base(persistentClass, cache, factory)
		{
			#region DISCRIMINATOR

			if (persistentClass.IsPolymorphic)
			{
				try
				{
					discriminatorValue = persistentClass.SubclassId;
					discriminatorSQLString = discriminatorValue.ToString();
				}
				catch (Exception e)
				{
					throw new MappingException("Could not format discriminator value to SQL string", e);
				}
			}
			else
			{
				discriminatorValue = null;
				discriminatorSQLString = null;
			}

			if (OptimisticLockMode > Versioning.OptimisticLock.Version)
				throw new MappingException(string.Format("optimistic-lock=all|dirty not supported for joined-subclass mappings [{0}]", EntityName));

			#endregion

			#region MULTITABLES
			int idColumnSpan = IdentifierColumnSpan;

			List<string> tables = new List<string>();
			List<string[]> keyColumns = new List<string[]>();
			List<bool> cascadeDeletes = new List<bool>();
			IEnumerator<IKeyValue> kiter = persistentClass.KeyClosureIterator.GetEnumerator();
			foreach (Table tab in persistentClass.TableClosureIterator)
			{
				kiter.MoveNext();
				IKeyValue key = kiter.Current;
				string tabname = tab.GetQualifiedName(factory.Dialect, factory.Settings.DefaultCatalogName, factory.Settings.DefaultSchemaName);
				tables.Add(tabname);

				List<string> keyCols = new List<string>(idColumnSpan);
				IEnumerable<Column> enumerableKCols = new SafetyEnumerable<Column>(key.ColumnIterator);
				foreach (Column kcol in enumerableKCols)
					keyCols.Add(kcol.GetQuotedName(factory.Dialect));

				keyColumns.Add(keyCols.ToArray());
				cascadeDeletes.Add(key.IsCascadeDeleteEnabled && factory.Dialect.SupportsCascadeDelete);				
			}
			naturalOrderTableNames = tables.ToArray();
			naturalOrderTableKeyColumns = keyColumns.ToArray();
			naturalOrderCascadeDeleteEnabled = cascadeDeletes.ToArray();

			List<string> subtables = new List<string>();
			List<bool> isConcretes = new List<bool>();
			keyColumns = new List<string[]>();
			foreach (Table tab in persistentClass.SubclassTableClosureIterator)
			{
				isConcretes.Add(persistentClass.IsClassOrSuperclassTable(tab));
				string tabname = tab.GetQualifiedName(factory.Dialect, factory.Settings.DefaultCatalogName, factory.Settings.DefaultSchemaName);
				subtables.Add(tabname);
				List<string> key = new List<string>(idColumnSpan);
				foreach (Column column in tab.PrimaryKey.ColumnIterator)
					key.Add(column.GetQuotedName(factory.Dialect));

				keyColumns.Add(key.ToArray());				
			}
			subclassTableNameClosure = subtables.ToArray();
			subclassTableKeyColumnClosure = keyColumns.ToArray();
			isClassOrSuperclassTable = isConcretes.ToArray();

			constraintOrderedTableNames = new string[subclassTableNameClosure.Length];
			constraintOrderedKeyColumnNames = new string[subclassTableNameClosure.Length][];
			int currentPosition = 0;
			for (int i = subclassTableNameClosure.Length - 1; i >= 0; i--, currentPosition++)
			{
				constraintOrderedTableNames[currentPosition] = subclassTableNameClosure[i];
				constraintOrderedKeyColumnNames[currentPosition] = subclassTableKeyColumnClosure[i];
			}

			tableSpan = naturalOrderTableNames.Length;
			tableNames = Reverse(naturalOrderTableNames);
			tableKeyColumns = Reverse(naturalOrderTableKeyColumns);
			Reverse(subclassTableNameClosure, tableSpan);
			Reverse(subclassTableKeyColumnClosure, tableSpan);

			spaces = ArrayHelper.Join(tableNames, ArrayHelper.ToStringArray(persistentClass.SynchronizedTables));

			// Custom sql
			customSQLInsert = new SqlString[tableSpan];
			customSQLUpdate = new SqlString[tableSpan];
			customSQLDelete = new SqlString[tableSpan];
			insertCallable = new bool[tableSpan];
			updateCallable = new bool[tableSpan];
			deleteCallable = new bool[tableSpan];
			insertResultCheckStyles = new ExecuteUpdateResultCheckStyle[tableSpan];
			updateResultCheckStyles = new ExecuteUpdateResultCheckStyle[tableSpan];
			deleteResultCheckStyles = new ExecuteUpdateResultCheckStyle[tableSpan];

			PersistentClass pc = persistentClass;
			int jk = tableSpan - 1;
			while (pc != null)
			{
				customSQLInsert[jk] = pc.CustomSQLInsert;
				insertCallable[jk] = customSQLInsert[jk] != null && pc.IsCustomInsertCallable;
				insertResultCheckStyles[jk] = pc.CustomSQLInsertCheckStyle
				                              ??
				                              ExecuteUpdateResultCheckStyle.DetermineDefault(customSQLInsert[jk], insertCallable[jk]);
				customSQLUpdate[jk] = pc.CustomSQLUpdate;
				updateCallable[jk] = customSQLUpdate[jk] != null && pc.IsCustomUpdateCallable;
				updateResultCheckStyles[jk] = pc.CustomSQLUpdateCheckStyle
				                              ??
				                              ExecuteUpdateResultCheckStyle.DetermineDefault(customSQLUpdate[jk], updateCallable[jk]);
				customSQLDelete[jk] = pc.CustomSQLDelete;
				deleteCallable[jk] = customSQLDelete[jk] != null && pc.IsCustomDeleteCallable;
				deleteResultCheckStyles[jk] = pc.CustomSQLDeleteCheckStyle
				                              ??
				                              ExecuteUpdateResultCheckStyle.DetermineDefault(customSQLDelete[jk], deleteCallable[jk]);
				jk--;
				pc = pc.Superclass;
			}
			if (jk != -1)
			{
				throw new AssertionFailure("Tablespan does not match height of joined-subclass hierarchy.");
			}

			#endregion

			#region PROPERTIES

			int hydrateSpan = PropertySpan;
			naturalOrderPropertyTableNumbers = new int[hydrateSpan];
			propertyTableNumbers = new int[hydrateSpan];
			int i2 = 0;
			foreach (Property prop in persistentClass.PropertyClosureIterator)
			{
				string tabname = prop.Value.Table.GetQualifiedName(factory.Dialect, factory.Settings.DefaultCatalogName, factory.Settings.DefaultSchemaName);
				propertyTableNumbers[i2] = GetTableId(tabname, tableNames);
				naturalOrderPropertyTableNumbers[i2] = GetTableId(tabname, naturalOrderTableNames);
				i2++;
			}

			// subclass closure properties
			List<int> columnTableNumbers = new List<int>();
			List<int> formulaTableNumbers = new List<int>();
			List<int> propTableNumbers = new List<int>();
			foreach (Property prop in persistentClass.SubclassPropertyClosureIterator)
			{
				Table tab = prop.Value.Table;
				string tabname = tab.GetQualifiedName(factory.Dialect, factory.Settings.DefaultCatalogName, factory.Settings.DefaultSchemaName);
				int tabnum = GetTableId(tabname, subclassTableNameClosure);
				propTableNumbers.Add(tabnum);

				foreach (ISelectable thing in prop.ColumnIterator)
				{
					if (thing.IsFormula)
						formulaTableNumbers.Add(tabnum);
					else
						columnTableNumbers.Add(tabnum);
				}
			}

			subclassColumnTableNumberClosure = columnTableNumbers.ToArray();
			subclassPropertyTableNumberClosure = propTableNumbers.ToArray();
			subclassFormulaTableNumberClosure = formulaTableNumbers.ToArray();
			#endregion

			#region SUBCLASSES

			int subclassSpan = persistentClass.SubclassSpan + 1;
			subclassClosure = new string[subclassSpan];
			subclassClosure[subclassSpan - 1] = EntityName;
			if (persistentClass.IsPolymorphic)
			{
				subclassesByDiscriminatorValue[discriminatorValue] = EntityName;
				discriminatorValues = new string[subclassSpan];
				discriminatorValues[subclassSpan - 1] = discriminatorSQLString;
				notNullColumnTableNumbers = new int[subclassSpan];
				int id =
					GetTableId(
						persistentClass.Table.GetQualifiedName(factory.Dialect, factory.Settings.DefaultCatalogName,
						                                       factory.Settings.DefaultSchemaName), subclassTableNameClosure);
				notNullColumnTableNumbers[subclassSpan - 1] = id;
				notNullColumnNames = new string[subclassSpan];
				notNullColumnNames[subclassSpan - 1] = subclassTableKeyColumnClosure[id][0]; 
				//( (Column) model.getTable().getPrimaryKey().getColumnIterator().next() ).getName();
			}
			else
			{
				discriminatorValues = null;
				notNullColumnTableNumbers = null;
				notNullColumnNames = null;
			}

			int k2 = 0;
			foreach (Subclass sc in persistentClass.SubclassIterator)
			{
				subclassClosure[k2] = sc.EntityName;
				try
				{
					if (persistentClass.IsPolymorphic)
					{
						// we now use subclass ids that are consistent across all
						// persisters for a class hierarchy, so that the use of 
						// "foo.class = Bar" works in HQL
						int subclassId = sc.SubclassId; //new Integer(k+1);
						subclassesByDiscriminatorValue[subclassId] = sc.EntityName;
						discriminatorValues[k2] = subclassId.ToString();
						int id =
							GetTableId(
								sc.Table.GetQualifiedName(factory.Dialect, factory.Settings.DefaultCatalogName,
								                          factory.Settings.DefaultSchemaName), subclassTableNameClosure);
						notNullColumnTableNumbers[k2] = id;
						notNullColumnNames[k2] = subclassTableKeyColumnClosure[id][0]; 
						//( (Column) sc.getTable().getPrimaryKey().getColumnIterator().next() ).getName();
					}
				}
				catch (Exception e)
				{
					throw new MappingException("Error parsing discriminator value", e);
				}
				k2++;				
			}

			#endregion

			InitLockers();

			InitSubclassPropertyAliasesMap(persistentClass);

			PostConstruct(mapping);
		}

		public override IType DiscriminatorType
		{
			get { return NHibernateUtil.Int32; }
		}

		public override string DiscriminatorSQLValue
		{
			get { return discriminatorSQLString; }
		}

		public override object DiscriminatorValue
		{
			get { return discriminatorValue; }
		}

		public override string[] PropertySpaces
		{
			get
			{
				// don't need subclass tables, because they can't appear in conditions
				return spaces;
			}
		}

		public override string[] IdentifierColumnNames
		{
			get { return tableKeyColumns[0]; }
		}

		protected internal override int[] PropertyTableNumbersInSelect
		{
			get { return propertyTableNumbers; }
		}

		public override bool IsMultiTable
		{
			get { return true; }
		}

		protected override int[] SubclassColumnTableNumberClosure
		{
			get { return subclassColumnTableNumberClosure; }
		}

		protected override int[] SubclassFormulaTableNumberClosure
		{
			get { return subclassFormulaTableNumberClosure; }
		}

		protected internal override int[] PropertyTableNumbers
		{
			get { return naturalOrderPropertyTableNumbers; }
		}

		public override string[] ConstraintOrderedTableNameClosure
		{
			get { return constraintOrderedTableNames; }
		}

		public override string[][] ContraintOrderedTableKeyColumnClosure
		{
			get { return constraintOrderedKeyColumnNames; }
		}

		public override string RootTableName
		{
			get { return naturalOrderTableNames[0]; }
		}

		public override string GetSubclassPropertyTableName(int i)
		{
			return subclassTableNameClosure[subclassPropertyTableNumberClosure[i]];
		}

		public override string GetSubclassForDiscriminatorValue(object value)
		{
			string result;
			subclassesByDiscriminatorValue.TryGetValue(value, out result);
			return result;
		}

		protected override string GetTableName(int table)
		{
			return naturalOrderTableNames[table];
		}

		protected override string[] GetKeyColumns(int table)
		{
			return naturalOrderTableKeyColumns[table];
		}

		protected override bool IsTableCascadeDeleteEnabled(int j)
		{
			return naturalOrderCascadeDeleteEnabled[j];
		}

		protected override bool IsPropertyOfTable(int property, int table)
		{
			return naturalOrderPropertyTableNumbers[property] == table;
		}

		private static void Reverse(object[] objects, int len)
		{
			object[] temp = new object[len];
			for (int i = 0; i < len; i++)
				temp[i] = objects[len - i - 1];

			for (int i = 0; i < len; i++)
				objects[i] = temp[i];
		}

		private static string[] Reverse(string[] objects)
		{
			int len = objects.Length;
			string[] temp = new string[len];
			for (int i = 0; i < len; i++)
				temp[i] = objects[len - i - 1];

			return temp;
		}

		private static string[][] Reverse(string[][] objects)
		{
			int len = objects.Length;
			string[][] temp = new string[len][];
			for (int i = 0; i < len; i++)
				temp[i] = objects[len - i - 1];

			return temp;
		}

		public override string FromTableFragment(string alias)
		{
			return TableName + ' ' + alias;
		}

		public override string TableName
		{
			get { return tableNames[0]; }
		}

		/// <summary>
		/// Find the Index of the table name from a list of table names.
		/// </summary>
		/// <param name="tableName">The name of the table to find.</param>
		/// <param name="tables">The array of table names</param>
		/// <returns>The Index of the table in the array.</returns>
		/// <exception cref="AssertionFailure">Thrown when the tableName specified can't be found</exception>
		private static int GetTableId(string tableName, string[] tables)
		{
			for (int i = 0; i < tables.Length; i++)
			{
				if (tableName.Equals(tables[i]))
					return i;
			}

			throw new AssertionFailure(string.Format("Table [{0}] not found", tableName));
		}

		protected override void AddDiscriminatorToSelect(SelectFragment select, string name, string suffix)
		{
			if (HasSubclasses)
			{
				select.SetExtraSelectList(DiscriminatorFragment(name), DiscriminatorAlias);
			}
		}

		private CaseFragment DiscriminatorFragment(string alias)
		{
			CaseFragment cases = Factory.Dialect.CreateCaseFragment();

			for (int i = 0; i < discriminatorValues.Length; i++)
			{
				cases.AddWhenColumnNotNull(GenerateTableAlias(alias, notNullColumnTableNumbers[i]), notNullColumnNames[i],
				                           discriminatorValues[i]);
			}
			return cases;
		}

		public override string FilterFragment(string alias)
		{
			return HasWhere ? " and " + GetSQLWhereString(GenerateFilterConditionAlias(alias)) : string.Empty;
		}

		public override string GenerateFilterConditionAlias(string rootAlias)
		{
			return GenerateTableAlias(rootAlias, tableSpan - 1);
		}

		public override string[] ToColumns(string alias, string propertyName)
		{
			if (EntityClass.Equals(propertyName))
			{
				// This doesn't actually seem to work but it *might* 
				// work on some dbs. Also it doesn't work if there 
				// are multiple columns of results because it 
				// is not accounting for the suffix: 
				// return new String[] { getDiscriminatorColumnName() }; 

				//TODO: this will need to be changed to return a SqlString but for now the SqlString
				// is being converted to a string for existing interfaces to work.
				return new string[] { DiscriminatorFragment(alias).ToSqlStringFragment() };
			}
			else
			{
				return base.ToColumns(alias, propertyName);
			}
		}

		protected override int GetSubclassPropertyTableNumber(int i)
		{
			return subclassPropertyTableNumberClosure[i];
		}

		protected override int TableSpan
		{
			get { return tableSpan; }
		}

		protected override string[] GetSubclassTableKeyColumns(int j)
		{
			return subclassTableKeyColumnClosure[j];
		}

		public override string GetSubclassTableName(int j)
		{
			return subclassTableNameClosure[j];
		}

		protected override int SubclassTableSpan
		{
			get { return subclassTableNameClosure.Length; }
		}

		protected override bool IsClassOrSuperclassTable(int j)
		{
			return isClassOrSuperclassTable[j];
		}

		public override string GetPropertyTableName(string propertyName)
		{
			int? index = EntityMetamodel.GetPropertyIndexOrNull(propertyName);
			if (!index.HasValue) return null;
			return tableNames[propertyTableNumbers[index.Value]];
		}

		public override string GetRootTableAlias(string drivingAlias)
		{
			return GenerateTableAlias(drivingAlias, GetTableId(RootTableName, tableNames));
		}

		public override Declarer GetSubclassPropertyDeclarer(string propertyPath)
		{
			if ("class".Equals(propertyPath))
			{
				// special case where we need to force include all subclass joins
				return Declarer.SubClass;
			}
			return base.GetSubclassPropertyDeclarer(propertyPath);
		}
	}
}
