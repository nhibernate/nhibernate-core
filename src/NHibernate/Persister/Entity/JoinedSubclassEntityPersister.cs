using System;
using System.Collections;
using NHibernate.Cache;
using NHibernate.Engine;
using NHibernate.Hql.Classic;
using NHibernate.Mapping;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Util;
using Array=System.Array;

namespace NHibernate.Persister.Entity
{
	/// <summary>
	/// A <c>IEntityPersister</c> implementing the normalized "table-per-subclass" mapping strategy
	/// </summary>
	public class JoinedSubclassEntityPersister : AbstractEntityPersister
	{
		// the class hierarchy structure
		private readonly int tableSpan;
		private readonly string qualifiedTableName;

		// all of the table names that this Persister uses for just its data 
		// it is indexed as tableNames[tableIndex]
		// for both the superclass and subclass the index of 0=basetable
		// for the base class there is only 1 table
		private readonly string[] tableNames;
		private readonly string[] naturalOrderTableNames;

		// two dimensional array that is indexed as tableKeyColumns[tableIndex][columnIndex]
		private readonly string[][] tableKeyColumns;
		private readonly string[][] naturalOrderTableKeyColumns;

		// the Type of objects for the subclass
		// the array is indexed as subclassClosure[subclassIndex].  
		// The length of the array is the number of subclasses + 1 for the Base Class.
		// The last index of the array contains the Type for the Base Class.
		// in the example of JoinedSubclassBase/One the values are :
		// subclassClosure[0] = JoinedSubclassOne
		// subclassClosure[1] = JoinedSubclassBase
		private readonly System.Type[] subclassClosure;

		// the names of the tables for the subclasses
		// the array is indexed as subclassTableNameColsure[tableIndex] = "tableName"
		// for the RootClass the index 0 is the base table
		// for the subclass the index 0 is also the base table
		private readonly string[] subclassTableNameClosure;

		// the names of the columns that are the Keys for the table - I don't know why they would
		// be different - I thought the subclasses didn't have their own PK, but used the one generated
		// by the base class??
		// the array is indexed as subclassTableKeyColumns[tableIndex][columnIndex] = "columnName"
		private readonly string[][] subclassTableKeyColumns;

		// TODO: figure out what this is being used for - when initializing the base class the values
		// are isClassOrSuperclassTable[0] = true, isClassOrSuperclassTable[1] = false
		// when initialized the subclass the values are [0]=true, [1]=true.  I believe this is telling
		// us that the table is used to populate this class or the superclass.
		// I would guess this is telling us specifically which tables this Persister will write information to.
		private readonly bool[] isClassOrSuperclassTable;

		// the index of the table that the property is coming from
		// the array is indexed as propertyTables[propertyIndex] = tableIndex 
		private readonly int[] propertyTables;
		private readonly int[] naturalOrderPropertyTables;

		private readonly int[] subclassPropertyTableNumberClosure;
		private readonly int[] subclassColumnTableNumberClosure;

		private readonly Hashtable tableNumberByPropertyPath = new Hashtable();

		private readonly int[] subclassFormulaTableNumberClosure;

		// subclass discrimination works by assigning particular values to certain 
		// combinations of null primary key values in the outer join using an SQL CASE

		// key = DiscrimatorValue, value = SubclassType Type
		private readonly Hashtable subclassesByDiscriminatorValue = new Hashtable();
		private readonly string[] discriminatorValues;
		private readonly string[] notNullColumns;
		private readonly int[] tableNumbers;

		private readonly IDiscriminatorType discriminatorType;
		private readonly string discriminatorSQLString;
		private readonly object discriminatorValue;
		private readonly string discriminatorColumnName;

		public override string DiscriminatorColumnName
		{
			get { return discriminatorColumnName; }
		}

		protected override string DiscriminatorAlias
		{
			// Is always "clazz_", so just use columnName
			get { return DiscriminatorColumnName; }
		}

		public override string GetSubclassPropertyTableName(int i)
		{
			return subclassTableNameClosure[subclassPropertyTableNumberClosure[i]];
		}

		public override IType DiscriminatorType
		{
			get { return discriminatorType; }
		}

		public override string DiscriminatorSQLValue
		{
			get { return discriminatorSQLString; }
		}

		public override object DiscriminatorValue
		{
			get { return discriminatorValue; }
		}

		public override System.Type GetSubclassForDiscriminatorValue(object value)
		{
			return (System.Type) subclassesByDiscriminatorValue[value];
		}

		public override object[] PropertySpaces
		{
			get
			{
				// don't need subclass tables, because they can't appear in conditions
				return tableNames;
			}
		}

		// Generate all the SQL

		protected override bool IsPropertyOfTable(int property, int table)
		{
			return naturalOrderPropertyTables[property] == table;
		}

		protected override SqlCommandInfo GenerateInsertString(bool identityInsert, bool[] includeProperty, int j)
		{
			SqlInsertBuilder builder = new SqlInsertBuilder(Factory);
			builder.SetTableName(naturalOrderTableNames[j]);

			for (int i = 0; i < PropertyTypes.Length; i++)
			{
				if (includeProperty[i] && IsPropertyOfTable(i, j))
				{
					builder.AddColumns(GetPropertyColumnNames(i), PropertyColumnInsertable[i], PropertyTypes[i]);
				}
			}

			if (identityInsert && j == 0)
			{
				// make sure the Dialect has an identity insert string because we don't want
				// to add the column when there is no value to supply the SqlBuilder
				if (Dialect.IdentityInsertString != null)
				{
					// only 1 column if there is IdentityInsert enabled.
					builder.AddColumn(naturalOrderTableKeyColumns[j][0], Dialect.IdentityInsertString);
				}
			}
			else
			{
				builder.AddColumns(naturalOrderTableKeyColumns[j], null, IdentifierType);
			}

			return builder.ToSqlCommandInfo();
		}

		private const string ConcreteAlias = "x";

		//INITIALIZATION:

		/// <summary>
		/// Constructs the NormalizedEntityPerister for the PersistentClass.
		/// </summary>
		/// <param name="model">The PeristentClass to create the EntityPersister for.</param>
		/// <param name="cache">The configured <see cref="ICacheConcurrencyStrategy" />.</param>
		/// <param name="factory">The SessionFactory that this EntityPersister will be stored in.</param>
		/// <param name="mapping">The mapping used to retrieve type information.</param>
		public JoinedSubclassEntityPersister(PersistentClass model, ICacheConcurrencyStrategy cache,
		                                     ISessionFactoryImplementor factory, IMapping mapping)
			: base(model, cache, factory)
		{
			// I am am making heavy use of the "this." just to help me with debugging what is a local variable to the 
			// constructor versus what is an class scoped variable.  I am only doing this when we are using fields 
			// instead of properties because it is easy to tell properties by the Case.

			// CLASS + TABLE

			Table table = model.RootTable;
			this.qualifiedTableName = table.GetQualifiedName(Dialect, Factory.DefaultSchema);

			// DISCRIMINATOR

			if (model.IsPolymorphic)
			{
				// when we have a Polymorphic model then we are going to add a column "clazz_" to 
				// the sql statement that will be a large CASE statement where we will use the 
				// integer value to tell us which class to instantiate for the record.
				this.discriminatorColumnName = "clazz_";

				try
				{
					discriminatorType = (IDiscriminatorType) NHibernateUtil.Int32;
					discriminatorValue = model.SubclassId;
					discriminatorSQLString = discriminatorType.ObjectToSQLString(discriminatorValue, Dialect);
				}
				catch (Exception e)
				{
					throw new MappingException(
						"Could not format discriminator value '0' to sql string using the IType NHibernate.Types.Int32Type", e);
				}
			}
			else
			{
				this.discriminatorColumnName = null;
				this.discriminatorType = null;
				discriminatorValue = null;
				this.discriminatorSQLString = null;
			}

			if (OptimisticLockMode != OptimisticLockMode.Version)
			{
				throw new MappingException("optimistic-lock attribute not supported for joined-subclass mappings: " + ClassName);
			}

			//MULTITABLES

			// these two will later be converted into arrays for the fields tableNames and tableKeyColumns
			ArrayList tables = new ArrayList();
			ArrayList keyColumns = new ArrayList();
			tables.Add(this.qualifiedTableName);
			keyColumns.Add(base.IdentifierColumnNames);

			// move through each table that contains the data for this entity.
			foreach (Table tab in model.TableClosureIterator)
			{
				string tabname = tab.GetQualifiedName(Dialect, factory.DefaultSchema);
				if (!tabname.Equals(qualifiedTableName))
				{
					tables.Add(tabname);
					string[] key = new string[tab.PrimaryKey.ColumnSpan];
					int k = 0;
					foreach (Column col in tab.PrimaryKey.ColumnIterator)
					{
						key[k++] = col.GetQuotedName(Dialect);
					}
					keyColumns.Add(key);
				}
			}

			this.naturalOrderTableNames = (string[]) tables.ToArray(typeof(string));
			this.naturalOrderTableKeyColumns = (string[][]) keyColumns.ToArray(typeof(string[]));

			// the description of these variables is the same as before
			ArrayList subtables = new ArrayList();
			keyColumns = new ArrayList();
			subtables.Add(this.qualifiedTableName);
			keyColumns.Add(base.IdentifierColumnNames);
			foreach (Table tab in model.SubclassTableClosureIterator)
			{
				string tabname = tab.GetQualifiedName(Dialect, factory.DefaultSchema);
				if (!tabname.Equals(qualifiedTableName))
				{
					subtables.Add(tabname);
					string[] key = new string[tab.PrimaryKey.ColumnSpan];
					int k = 0;
					foreach (Column col in tab.PrimaryKey.ColumnIterator)
					{
						key[k++] = col.GetQuotedName(Dialect);
					}
					keyColumns.Add(key);
				}
			}

			// convert the local ArrayList variables into arrays for the fields in the class
			this.subclassTableNameClosure = (string[]) subtables.ToArray(typeof(string));
			this.subclassTableKeyColumns = (string[][]) keyColumns.ToArray(typeof(string[]));
			this.isClassOrSuperclassTable = new bool[this.subclassTableNameClosure.Length];
			for (int j = 0; j < subclassTableNameClosure.Length; j++)
			{
				this.isClassOrSuperclassTable[j] = tables.Contains(this.subclassTableNameClosure[j]);
			}

			int len = naturalOrderTableNames.Length;
			tableSpan = naturalOrderTableNames.Length;
			tableNames = Reverse(naturalOrderTableNames);
			tableKeyColumns = Reverse(naturalOrderTableKeyColumns);
			Array.Reverse(subclassTableNameClosure, 0, len);
			Array.Reverse(subclassTableKeyColumns, 0, len);

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

			PersistentClass pc = model;
			int jk = tableSpan - 1;
			while (pc != null)
			{
				customSQLInsert[jk] = pc.CustomSQLInsert;
				insertCallable[jk] = customSQLInsert[jk] != null && pc.IsCustomInsertCallable;
				insertResultCheckStyles[jk] = pc.CustomSQLInsertCheckStyle == null
				                              	?
				                              ExecuteUpdateResultCheckStyle.DetermineDefault(customSQLInsert[jk], insertCallable[jk])
				                              	: pc.CustomSQLInsertCheckStyle;
				customSQLUpdate[jk] = pc.CustomSQLUpdate;
				updateCallable[jk] = customSQLUpdate[jk] != null && pc.IsCustomUpdateCallable;
				updateResultCheckStyles[jk] = pc.CustomSQLUpdateCheckStyle == null
				                              	?
				                              ExecuteUpdateResultCheckStyle.DetermineDefault(customSQLUpdate[jk], updateCallable[jk])
				                              	: pc.CustomSQLUpdateCheckStyle;
				customSQLDelete[jk] = pc.CustomSQLDelete;
				deleteCallable[jk] = customSQLDelete[jk] != null && pc.IsCustomDeleteCallable;
				deleteResultCheckStyles[jk] = pc.CustomSQLDeleteCheckStyle == null
				                              	?
				                              ExecuteUpdateResultCheckStyle.DetermineDefault(customSQLDelete[jk], deleteCallable[jk])
				                              	: pc.CustomSQLDeleteCheckStyle;
				jk--;
				pc = pc.Superclass;
			}
			if (jk != -1)
			{
				throw new AssertionFailure("Tablespan does not match height of joined-subclass hiearchy.");
			}

			// PROPERTIES

			// initialize the lengths of all of the Property related fields in the class
			this.propertyTables = new int[HydrateSpan];
			this.naturalOrderPropertyTables = new int[HydrateSpan];

			int i = 0;
			foreach (Mapping.Property prop in model.PropertyClosureIterator)
			{
				Table tab = prop.Value.Table;
				string tabname = tab.GetQualifiedName(Dialect, factory.DefaultSchema);
				this.propertyTables[i] = GetTableId(tabname, this.tableNames);
				this.naturalOrderPropertyTables[i] = GetTableId(tabname, this.naturalOrderTableNames);
				i++;
			}

			// SUBCLASS CLOSURE PROPERTIES

			ArrayList columnTableNumbers = new ArrayList(); //this.subclassColumnTableNumberClosure
			ArrayList formulaTableNumbers = new ArrayList();
			ArrayList propTableNumbers = new ArrayList(); // this.subclassPropertyTableNameClosure

			foreach (Mapping.Property prop in model.SubclassPropertyClosureIterator)
			{
				Table tab = prop.Value.Table;
				String tabname = tab.GetQualifiedName(
					factory.Dialect,
					factory.DefaultSchema);
				int tabnum = GetTableId(tabname, subclassTableNameClosure);
				propTableNumbers.Add(tabnum);

				foreach (ISelectable thing in prop.ColumnIterator)
				{
					if (thing.IsFormula)
					{
						formulaTableNumbers.Add(tabnum);
					}
					else
					{
						columnTableNumbers.Add(tabnum);
					}
				}
			}

			subclassColumnTableNumberClosure = ArrayHelper.ToIntArray(columnTableNumbers);
			subclassPropertyTableNumberClosure = ArrayHelper.ToIntArray(propTableNumbers);
			subclassFormulaTableNumberClosure = ArrayHelper.ToIntArray(formulaTableNumbers);

			// ****** Moved the sql generation to PostInstantiate *****

			System.Type mappedClass = model.MappedClass;

			// SUBCLASSES

			// all of the classes spanned, so even though there might be 2 subclasses we need to 
			// add in the baseclass - so we add 1 to the Closure
			int subclassSpan = model.SubclassSpan + 1;
			this.subclassClosure = new System.Type[subclassSpan];

			// start with the mapped class as the last element in the subclassClosure
			this.subclassClosure[subclassSpan - 1] = mappedClass;

			if (model.IsPolymorphic)
			{
				this.subclassesByDiscriminatorValue.Add(discriminatorValue, mappedClass);
				this.discriminatorValues = new string[subclassSpan];
				this.discriminatorValues[subclassSpan - 1] = discriminatorSQLString;

				this.tableNumbers = new int[subclassSpan];
				int id = GetTableId(
					model.Table.GetQualifiedName(Dialect, factory.DefaultSchema),
					this.subclassTableNameClosure);

				this.tableNumbers[subclassSpan - 1] = id;
				this.notNullColumns = new string[subclassSpan];
				this.notNullColumns[subclassSpan - 1] = subclassTableKeyColumns[id][0];
				/*
				foreach( Column col in model.Table.PrimaryKey.ColumnIterator )
				{
					notNullColumns[ subclassSpan - 1 ] = col.GetQuotedName( Dialect ); //only once
				}
				*/
			}
			else
			{
				discriminatorValues = null;
				tableNumbers = null;
				notNullColumns = null;
			}

			int p = 0;
			foreach (Subclass sc in model.SubclassIterator)
			{
				subclassClosure[p] = sc.MappedClass;
				try
				{
					if (model.IsPolymorphic)
					{
						int disc = sc.SubclassId;
						subclassesByDiscriminatorValue.Add(disc, sc.MappedClass);
						discriminatorValues[p] = disc.ToString();
						int id = GetTableId(
							sc.Table.GetQualifiedName(Dialect, factory.DefaultSchema),
							this.subclassTableNameClosure);
						tableNumbers[p] = id;
						notNullColumns[p] = subclassTableKeyColumns[id][0];
						/*
						foreach( Column col in sc.Table.PrimaryKey.ColumnIterator )
						{
							notNullColumns[ p ] = col.GetQuotedName( Dialect ); //only once;
						}
						*/
					}
				}
				catch (Exception e)
				{
					throw new MappingException("Error parsing discriminator value", e);
				}
				p++;
			}

			// moved the propertyHasColumns into PostInstantiate as it needs the SQL strings

			// needs identifier info so moved to PostInstatiate
			//InitLockers( );

			InitSubclassPropertyAliasesMap(model);
			PostConstruct(mapping);
		}

		/// <summary>
		/// Create a new one dimensional array sorted in the Reverse order of the original array.
		/// </summary>
		/// <param name="objects">The original array.</param>
		/// <returns>A new array in the reverse order of the original array.</returns>
		private static string[] Reverse(string[] objects)
		{
			int len = objects.Length;
			string[] temp = new string[len];
			for (int i = 0; i < len; i++)
			{
				temp[i] = objects[len - i - 1];
			}
			return temp;
		}

		/// <summary>
		/// Create a new two dimensional array sorted in the Reverse order of the original array. The 
		/// second dimension is not reversed.
		/// </summary>
		/// <param name="objects">The original array.</param>
		/// <returns>A new array in the reverse order of the original array.</returns>
		private static string[][] Reverse(string[][] objects)
		{
			int len = objects.Length;
			string[][] temp = new string[len][];
			for (int i = 0; i < len; i++)
			{
				temp[i] = objects[len - i - 1];
			}
			return temp;
		}

		protected int GetPropertyTableNumber(string propertyName)
		{
			string[] propertyNames = PropertyNames;

			for (int i = 0; i < propertyNames.Length; i++)
			{
				if (propertyName.Equals(propertyNames[i]))
				{
					return propertyTables[i];
				}
			}
			return 0;
		}

		// TODO: override
		protected void HandlePath(string path, IType type)
		{
			if (type.IsAssociationType && ((IAssociationType) type).UseLHSPrimaryKey)
			{
				tableNumberByPropertyPath[path] = 0;
			}
			else
			{
				string propertyName = StringHelper.Root(path);
				tableNumberByPropertyPath[path] = GetPropertyTableNumber(propertyName);
			}
		}

		public override string TableName
		{
			get { return subclassTableNameClosure[0]; }
		}

		private JoinFragment Outerjoin(string name, bool innerJoin, bool includeSubclasses)
		{
			JoinFragment outerjoin = Factory.Dialect.CreateOuterJoinFragment();
			for (int i = 1; i < subclassTableNameClosure.Length; i++)
			{
				if (includeSubclasses || isClassOrSuperclassTable[i])
				{
					outerjoin.AddJoin(
						subclassTableNameClosure[i],
						Alias(name, i),
						StringHelper.Qualify(name, IdentifierColumnNames),
						subclassTableKeyColumns[i],
						innerJoin && isClassOrSuperclassTable[i]
							?
						JoinType.InnerJoin
							:
						JoinType.LeftOuterJoin);
				}
			}
			return outerjoin;
		}

		/// <summary>
		/// Find the Index of the table name from a list of table names.
		/// </summary>
		/// <param name="tableName">The name of the table to find.</param>
		/// <param name="tables">The array of table names</param>
		/// <returns>The Index of the table in the array.</returns>
		/// <exception cref="AssertionFailure">Thrown when the tableName specified can't be found</exception>
		private int GetTableId(string tableName, string[] tables)
		{
			for (int tableIndex = 0; tableIndex < tables.Length; tableIndex++)
			{
				if (tableName.Equals(tables[tableIndex]))
				{
					return tableIndex;
				}
			}

			throw new AssertionFailure(string.Format("table [{0}] not found", tableName));
		}

		// TODO: override
		public override string[] ToColumns(string alias, string property)
		{
			if (PathExpressionParser.EntityClass.Equals(property))
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
				return base.ToColumns(alias, property);
			}
		}

		protected override int[] PropertyTableNumbersInSelect
		{
			get { return propertyTables; }
		}

		private CaseFragment DiscriminatorFragment(string alias)
		{
			CaseFragment cases = Dialect.CreateCaseFragment();

			for (int i = 0; i < discriminatorValues.Length; i++)
			{
				cases.AddWhenColumnNotNull(
					Alias(alias, tableNumbers[i]),
					notNullColumns[i],
					discriminatorValues[i]
					);
			}
			return cases;
		}

		public override SqlString FromJoinFragment(string alias, bool innerJoin, bool includeSubclasses)
		{
			return Outerjoin(alias, innerJoin, includeSubclasses).ToFromFragmentString;
		}

		public override SqlString WhereJoinFragment(string alias, bool innerJoin, bool includeSubclasses)
		{
			return Outerjoin(alias, innerJoin, includeSubclasses).ToWhereFragmentString;
		}

		public override SqlString QueryWhereFragment(string alias, bool innerJoin, bool includeSubclasses)
		{
			SqlString result = WhereJoinFragment(alias, innerJoin, includeSubclasses);
			string rootAlias = Alias(alias, naturalOrderTableNames.Length - 1); // urgh, ugly!!
			if (HasWhere)
			{
				result = result.Append(" and " + GetSQLWhereString(rootAlias));
			}

			return result;
		}

		public override string[] IdentifierColumnNames
		{
			get { return tableKeyColumns[0]; }
		}

		public override bool IsCacheInvalidationRequired
		{
			get { return HasFormulaProperties || (!IsVersioned && UseDynamicUpdate); }
		}

		protected override string VersionedTableName
		{
			get { return qualifiedTableName; }
		}

		protected override int GetSubclassPropertyTableNumber(int i)
		{
			return subclassPropertyTableNumberClosure[i];
		}

		protected override void AddDiscriminatorToSelect(SelectFragment select, string name, string suffix)
		{
			select.SetExtraSelectList(DiscriminatorFragment(name), DiscriminatorAlias);
		}

		protected override int[] SubclassColumnTableNumberClosure
		{
			get { return subclassColumnTableNumberClosure; }
		}

		protected override int[] SubclassFormulaTableNumberClosure
		{
			get { return subclassFormulaTableNumberClosure; }
		}

		public override string GetPropertyTableName(string propertyName)
		{
			int? index = EntityMetamodel.GetPropertyIndexOrNull(propertyName);
			if (!index.HasValue) return null;
			return tableNames[propertyTables[index.Value]];
		}

		public override string FilterFragment(string alias)
		{
			return HasWhere
			       	?
			       " and " + GetSQLWhereString(GenerateFilterConditionAlias(alias))
			       	:
			       "";
		}

		public override string GenerateFilterConditionAlias(string rootAlias)
		{
			return GenerateTableAlias(rootAlias, tableSpan - 1);
		}

		protected override int TableSpan
		{
			get { return tableSpan; }
		}

		public override bool IsMultiTable
		{
			get { return true; }
		}

		protected override string[] GetSubclassTableKeyColumns(int j)
		{
			return subclassTableKeyColumns[j];
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

		protected override string[] GetKeyColumns(int table)
		{
			return naturalOrderTableKeyColumns[table];
		}

		protected override string GetTableName(int table)
		{
			return naturalOrderTableNames[table];
		}

		protected override int[] PropertyTableNumbers
		{
			get { return naturalOrderPropertyTables; }
		}

		public override string RootTableName
		{
			get { return naturalOrderTableNames[0]; }
		}

		public override string GetRootTableAlias(string drivingAlias)
		{
			return GenerateTableAlias(drivingAlias, GetTableId(RootTableName, tableNames));
		}

	}
}
