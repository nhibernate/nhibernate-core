using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using NHibernate.Cache;
using NHibernate.Engine;
using NHibernate.Mapping;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Persister.Entity
{
	/// <summary>
	/// Default implementation of the <c>ClassPersister</c> interface. Implements the
	/// "table-per-class hierarchy" mapping strategy for an entity class.
	/// </summary>
	public class SingleTableEntityPersister : AbstractEntityPersister, IQueryable
	{
		// the class hierarchy structure
		private readonly int joinSpan;
		private IType[] identifierTypes;
		private readonly string[] qualifiedTableNames;
		private readonly bool[] isInverseTable;
		private readonly bool[] isNullableTable;
		private readonly string[][] keyColumnNames;
		private readonly bool[] cascadeDeleteEnabled;
		private readonly bool hasSequentialSelects;
		private readonly string[] spaces;
		private readonly string[] subclassClosure;
		private readonly string[] subclassTableNameClosure;
		private readonly bool[] subclassTableIsLazyClosure;
		private readonly bool[] isInverseSubclassTable;
		private readonly bool[] isNullableSubclassTable;
		private readonly bool[] subclassTableSequentialSelect;
		private readonly string[][] subclassTableKeyColumnClosure;
		private readonly bool[] isClassOrSuperclassTable;

		// properties of this class, including inherited properties
		private readonly int[] propertyTableNumbers;

		// if the id is a property of the base table eg join to property-ref
		// if the id is not a property the value will be null
		private readonly Dictionary<int, int> tableIdPropertyNumbers;

		// the closure of all columns used by the entire hierarchy including
		// subclasses and superclasses of this class
		private readonly int[] subclassPropertyTableNumberClosure;

		private readonly int[] subclassColumnTableNumberClosure;
		private readonly int[] subclassFormulaTableNumberClosure;

		// discriminator column
		private readonly Dictionary<object, string> subclassesByDiscriminatorValue = new Dictionary<object, string>();
		private readonly bool forceDiscriminator;
		private readonly string discriminatorColumnName;
		private readonly string discriminatorFormula;
		private readonly string discriminatorFormulaTemplate;
		private readonly string discriminatorAlias;
		private readonly IType discriminatorType;
		private readonly string discriminatorSQLValue;
		private readonly object discriminatorValue;
		private readonly bool discriminatorInsertable;

		private readonly string[] constraintOrderedTableNames;
		private readonly string[][] constraintOrderedKeyColumnNames;

		//private readonly IDictionary propertyTableNumbersByName = new Hashtable();
		private readonly Dictionary<string, int> propertyTableNumbersByNameAndSubclass = new Dictionary<string, int>();

		private readonly Dictionary<string, SqlString> sequentialSelectStringsByEntityName = new Dictionary<string, SqlString>();

		private static readonly object NullDiscriminator = new object();
		private static readonly object NotNullDiscriminator = new object();

		//provided so we can join to keys other than the primary key
		private readonly Dictionary<int, string[]> joinToKeyColumns;

		public SingleTableEntityPersister(PersistentClass persistentClass, ICacheConcurrencyStrategy cache,
																			ISessionFactoryImplementor factory, IMapping mapping)
			: base(persistentClass, cache, factory)
		{
			#region CLASS + TABLE

			joinSpan = persistentClass.JoinClosureSpan + 1;
			identifierTypes = new IType[joinSpan];
			qualifiedTableNames = new string[joinSpan];
			isInverseTable = new bool[joinSpan];
			isNullableTable = new bool[joinSpan];
			keyColumnNames = new string[joinSpan][];
			Table table = persistentClass.RootTable;
			identifierTypes[0] = IdentifierType;
			qualifiedTableNames[0] =table.GetQualifiedName(factory.Dialect, factory.Settings.DefaultCatalogName, factory.Settings.DefaultSchemaName);
			isInverseTable[0] = false;
			isNullableTable[0] = false;
			keyColumnNames[0] = IdentifierColumnNames;
			cascadeDeleteEnabled = new bool[joinSpan];

			// Custom sql
			customSQLInsert = new SqlString[joinSpan];
			customSQLUpdate = new SqlString[joinSpan];
			customSQLDelete = new SqlString[joinSpan];
			insertCallable = new bool[joinSpan];
			updateCallable = new bool[joinSpan];
			deleteCallable = new bool[joinSpan];
			insertResultCheckStyles = new ExecuteUpdateResultCheckStyle[joinSpan];
			updateResultCheckStyles = new ExecuteUpdateResultCheckStyle[joinSpan];
			deleteResultCheckStyles = new ExecuteUpdateResultCheckStyle[joinSpan];

			customSQLInsert[0] = persistentClass.CustomSQLInsert;
			insertCallable[0] = customSQLInsert[0] != null && persistentClass.IsCustomInsertCallable;
			insertResultCheckStyles[0] = persistentClass.CustomSQLInsertCheckStyle
																	 ?? ExecuteUpdateResultCheckStyle.DetermineDefault(customSQLInsert[0], insertCallable[0]);
			customSQLUpdate[0] = persistentClass.CustomSQLUpdate;
			updateCallable[0] = customSQLUpdate[0] != null && persistentClass.IsCustomUpdateCallable;
			updateResultCheckStyles[0] = persistentClass.CustomSQLUpdateCheckStyle
																	 ?? ExecuteUpdateResultCheckStyle.DetermineDefault(customSQLUpdate[0], updateCallable[0]);
			customSQLDelete[0] = persistentClass.CustomSQLDelete;
			deleteCallable[0] = customSQLDelete[0] != null && persistentClass.IsCustomDeleteCallable;
			deleteResultCheckStyles[0] = persistentClass.CustomSQLDeleteCheckStyle
																	 ?? ExecuteUpdateResultCheckStyle.DetermineDefault(customSQLDelete[0], deleteCallable[0]);

			#endregion

			#region JOINS
			int j = 1;
			foreach (Join join in persistentClass.JoinClosureIterator)
			{
				identifierTypes[j] = join.Key.Type;
				qualifiedTableNames[j] = join.Table.GetQualifiedName(factory.Dialect, factory.Settings.DefaultCatalogName, factory.Settings.DefaultSchemaName);
				isInverseTable[j] = join.IsInverse;
				isNullableTable[j] = join.IsOptional;
				cascadeDeleteEnabled[j] = join.Key.IsCascadeDeleteEnabled && factory.Dialect.SupportsCascadeDelete;

				customSQLInsert[j] = join.CustomSQLInsert;
				insertCallable[j] = customSQLInsert[j] != null && join.IsCustomInsertCallable;
				insertResultCheckStyles[j] = join.CustomSQLInsertCheckStyle
																		 ??
																		 ExecuteUpdateResultCheckStyle.DetermineDefault(customSQLInsert[j], insertCallable[j]);
				customSQLUpdate[j] = join.CustomSQLUpdate;
				updateCallable[j] = customSQLUpdate[j] != null && join.IsCustomUpdateCallable;
				updateResultCheckStyles[j] = join.CustomSQLUpdateCheckStyle
																		 ??
																		 ExecuteUpdateResultCheckStyle.DetermineDefault(customSQLUpdate[j], updateCallable[j]);
				customSQLDelete[j] = join.CustomSQLDelete;
				deleteCallable[j] = customSQLDelete[j] != null && join.IsCustomDeleteCallable;
				deleteResultCheckStyles[j] = join.CustomSQLDeleteCheckStyle
																		 ??
																		 ExecuteUpdateResultCheckStyle.DetermineDefault(customSQLDelete[j], deleteCallable[j]);

				keyColumnNames[j] = join.Key.ColumnIterator.OfType<Column>().Select(col => col.GetQuotedName(factory.Dialect)).ToArray();

				j++;
			}

			constraintOrderedTableNames = new string[qualifiedTableNames.Length];
			constraintOrderedKeyColumnNames = new string[qualifiedTableNames.Length][];
			for (int i = qualifiedTableNames.Length - 1, position = 0; i >= 0; i--, position++)
			{
				constraintOrderedTableNames[position] = qualifiedTableNames[i];
				constraintOrderedKeyColumnNames[position] = keyColumnNames[i];
			}

			spaces = qualifiedTableNames.Concat(persistentClass.SynchronizedTables).ToArray();

			bool lazyAvailable = IsInstrumented;

			bool hasDeferred = false;
			List<string> subclassTables = new List<string>();
			List<string[]> joinKeyColumns = new List<string[]>();
			//provided so we can join to keys other than the primary key
			joinToKeyColumns = new Dictionary<int, string[]>();
			//Columns that also function as Id's
			List<Column> idColumns = new List<Column>();
			tableIdPropertyNumbers = new Dictionary<int, int>();
			List<bool> isConcretes = new List<bool>();
			List<bool> isDeferreds = new List<bool>();
			List<bool> isInverses = new List<bool>();
			List<bool> isNullables = new List<bool>();
			List<bool> isLazies = new List<bool>();
			subclassTables.Add(qualifiedTableNames[0]);
			joinKeyColumns.Add(IdentifierColumnNames);
			isConcretes.Add(true);
			isDeferreds.Add(false);
			isInverses.Add(false);
			isNullables.Add(false);
			isLazies.Add(false);
			foreach (Join join in persistentClass.SubclassJoinClosureIterator)
			{
				isConcretes.Add(persistentClass.IsClassOrSuperclassJoin(join));
				isDeferreds.Add(join.IsSequentialSelect);
				isInverses.Add(join.IsInverse);
				isNullables.Add(join.IsOptional);
				isLazies.Add(lazyAvailable && join.IsLazy);
				if (join.IsSequentialSelect && !persistentClass.IsClassOrSuperclassJoin(join))
					hasDeferred = true;
				subclassTables.Add(join.Table.GetQualifiedName(factory.Dialect, factory.Settings.DefaultCatalogName, factory.Settings.DefaultSchemaName));

				var keyCols = join.Key.ColumnIterator.OfType<Column>().Select(col => col.GetQuotedName(factory.Dialect)).ToArray();
				joinKeyColumns.Add(keyCols);

				//are we joining to other than the primary key?
				if (join.RefIdProperty != null)
				{
					var curTableIndex = joinKeyColumns.Count - 1;
					//there should only ever be one key
					var toKeyCols = new List<string>(join.RefIdProperty.ColumnSpan);
					foreach (Column col in join.RefIdProperty.ColumnIterator)
					{
						toKeyCols.Add(col.GetQuotedName(factory.Dialect));
						
						//find out what property index this is
						int i = 0;
						foreach (var prop in persistentClass.PropertyClosureIterator)
						{
							if (prop == @join.RefIdProperty)
							{
								tableIdPropertyNumbers.Add(curTableIndex, i);
								break;
			}
							i++;
						}

						idColumns.Add(col);
					}
					joinToKeyColumns.Add(curTableIndex, toKeyCols.ToArray());
				}
			}

			subclassTableSequentialSelect = isDeferreds.ToArray();
			subclassTableNameClosure = subclassTables.ToArray();
			subclassTableIsLazyClosure = isLazies.ToArray();
			subclassTableKeyColumnClosure = joinKeyColumns.ToArray();
			isClassOrSuperclassTable = isConcretes.ToArray();
			isInverseSubclassTable = isInverses.ToArray();
			isNullableSubclassTable = isNullables.ToArray();
			hasSequentialSelects = hasDeferred;

			#endregion

			#region DISCRIMINATOR

			if (persistentClass.IsPolymorphic)
			{
				IValue discrimValue = persistentClass.Discriminator;
				if (discrimValue == null)
					throw new MappingException("Discriminator mapping required for single table polymorphic persistence");

				forceDiscriminator = persistentClass.IsForceDiscriminator;
				IEnumerator<ISelectable> iSel = discrimValue.ColumnIterator.GetEnumerator();
				iSel.MoveNext();
				ISelectable selectable = iSel.Current;
				if (discrimValue.HasFormula)
				{
					Formula formula = (Formula)selectable;
					discriminatorFormula = formula.FormulaString;
					discriminatorFormulaTemplate = formula.GetTemplate(factory.Dialect, factory.SQLFunctionRegistry);
					discriminatorColumnName = null;
					discriminatorAlias = Discriminator_Alias;
				}
				else
				{
					Column column = (Column)selectable;
					discriminatorColumnName = column.GetQuotedName(factory.Dialect);
					discriminatorAlias = column.GetAlias(factory.Dialect, persistentClass.RootTable);
					discriminatorFormula = null;
					discriminatorFormulaTemplate = null;
				}
				discriminatorType = persistentClass.Discriminator.Type;
				if (persistentClass.IsDiscriminatorValueNull)
				{
					discriminatorValue = NullDiscriminator;
					discriminatorSQLValue = InFragment.Null;
					discriminatorInsertable = false;
				}
				else if (persistentClass.IsDiscriminatorValueNotNull)
				{
					discriminatorValue = NotNullDiscriminator;
					discriminatorSQLValue = InFragment.NotNull;
					discriminatorInsertable = false;
				}
				else
				{
					discriminatorInsertable = persistentClass.IsDiscriminatorInsertable && !discrimValue.HasFormula;
					try
					{
						IDiscriminatorType dtype = (IDiscriminatorType)discriminatorType;
						discriminatorValue = dtype.StringToObject(persistentClass.DiscriminatorValue);
						discriminatorSQLValue = dtype.ObjectToSQLString(discriminatorValue, factory.Dialect);
					}
					catch (InvalidCastException cce)
					{
						throw new MappingException(
							string.Format("Illegal discriminator type: {0} of entity {1}", discriminatorType.Name, persistentClass.EntityName), cce);
					}
					catch (Exception e)
					{
						throw new MappingException("Could not format discriminator value to SQL string of entity " + persistentClass.EntityName, e);
					}
				}
			}
			else
			{
				forceDiscriminator = false;
				discriminatorInsertable = false;
				discriminatorColumnName = null;
				discriminatorAlias = null;
				discriminatorType = null;
				discriminatorValue = null;
				discriminatorSQLValue = null;
				discriminatorFormula = null;
				discriminatorFormulaTemplate = null;
			}

			#endregion

			#region PROPERTIES

			propertyTableNumbers = new int[PropertySpan];
			int i2 = 0;
			foreach (Property prop in persistentClass.PropertyClosureIterator)
			{
				propertyTableNumbers[i2++] = persistentClass.GetJoinNumber(prop);
			}

			List<int> columnJoinNumbers = new List<int>();
			List<int> formulaJoinedNumbers = new List<int>();
			List<int> propertyJoinNumbers = new List<int>();
			foreach (Property prop in persistentClass.SubclassPropertyClosureIterator)
			{
				int join = persistentClass.GetJoinNumber(prop);
				propertyJoinNumbers.Add(join);

				propertyTableNumbersByNameAndSubclass[prop.PersistentClass.EntityName + '.' + prop.Name] = join;
				foreach (ISelectable thing in prop.ColumnIterator)
				{
					if (thing.IsFormula)
						formulaJoinedNumbers.Add(join);
					else
						columnJoinNumbers.Add(join);
				}
			}

			subclassColumnTableNumberClosure = columnJoinNumbers.ToArray();
			subclassFormulaTableNumberClosure = formulaJoinedNumbers.ToArray();
			subclassPropertyTableNumberClosure = propertyJoinNumbers.ToArray();

			int subclassSpan = persistentClass.SubclassSpan + 1;
			subclassClosure = new string[subclassSpan];
			subclassClosure[0] = EntityName;
			if (persistentClass.IsPolymorphic)
				subclassesByDiscriminatorValue[discriminatorValue] = EntityName;

			#endregion

			#region SUBCLASSES
			if (persistentClass.IsPolymorphic)
			{
				int k = 1;
				foreach (Subclass sc in persistentClass.SubclassIterator)
				{
					subclassClosure[k++] = sc.EntityName;
					if (sc.IsDiscriminatorValueNull)
					{
						subclassesByDiscriminatorValue[NullDiscriminator] = sc.EntityName;
					}
					else if (sc.IsDiscriminatorValueNotNull)
					{
						subclassesByDiscriminatorValue[NotNullDiscriminator] = sc.EntityName;
					}
					else
					{
						if (discriminatorType == null)
							throw new MappingException("Not available discriminator type of entity " + persistentClass.EntityName);
						try
						{
							IDiscriminatorType dtype = (IDiscriminatorType)discriminatorType;
							subclassesByDiscriminatorValue[dtype.StringToObject(sc.DiscriminatorValue)] = sc.EntityName;
						}
						catch (InvalidCastException cce)
						{
							throw new MappingException(
								string.Format("Illegal discriminator type: {0} of entity {1}", discriminatorType.Name, persistentClass.EntityName), cce);
						}
						catch (Exception e)
						{
							throw new MappingException("Error parsing discriminator value of entity " + persistentClass.EntityName, e);
						}
					}
				}
			}

			#endregion

			InitLockers();

			InitSubclassPropertyAliasesMap(persistentClass);

			PostConstruct(mapping);
		}

		public override string DiscriminatorColumnName
		{
			get { return discriminatorColumnName; }
		}

		protected override string DiscriminatorFormulaTemplate
		{
			get { return discriminatorFormulaTemplate; }
		}

		public override IType DiscriminatorType
		{
			get { return discriminatorType; }
		}

		public override string DiscriminatorSQLValue
		{
			get { return discriminatorSQLValue; }
		}

		public override object DiscriminatorValue
		{
			get { return discriminatorValue; }
		}

		public override string[] SubclassClosure
		{
			get { return subclassClosure; }
		}

		public override string[] PropertySpaces
		{
			get { return spaces; }
		}

		protected internal override int[] PropertyTableNumbersInSelect
		{
			get { return propertyTableNumbers; }
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
			get { return propertyTableNumbers; }
		}

		public override bool IsMultiTable
		{
			get { return TableSpan > 1; }
		}

		public override string[] ConstraintOrderedTableNameClosure
		{
			get { return constraintOrderedTableNames; }
		}

		public override IType GetIdentifierType(int j)
		{
			return identifierTypes[j];
		}

		public override string[][] ConstraintOrderedTableKeyColumnClosure
		{
			get { return constraintOrderedKeyColumnNames; }
		}

		protected override bool IsInverseTable(int j)
		{
			return isInverseTable[j];
		}

		protected override bool IsInverseSubclassTable(int j)
		{
			return isInverseSubclassTable[j];
		}

		protected internal override string DiscriminatorAlias
		{
			get { return discriminatorAlias; }
		}

		public override string TableName
		{
			get { return qualifiedTableNames[0]; }
		}

		public override string GetSubclassForDiscriminatorValue(object value)
		{
			string result;
			if (value == null)
			{
				subclassesByDiscriminatorValue.TryGetValue(NullDiscriminator, out result);
			}
			else
			{
				if (!subclassesByDiscriminatorValue.TryGetValue(value, out result))
					subclassesByDiscriminatorValue.TryGetValue(NotNullDiscriminator, out result);
			}
			return result;
		}

		protected bool IsDiscriminatorFormula
		{
			get { return discriminatorColumnName == null; }
		}

		protected string DiscriminatorFormula
		{
			get { return discriminatorFormula; }
		}

		protected override string GetTableName(int table)
		{
			return qualifiedTableNames[table];
		}

		protected override string[] GetKeyColumns(int table)
		{
			return keyColumnNames[table];
		}

		protected override bool IsTableCascadeDeleteEnabled(int j)
		{
			return cascadeDeleteEnabled[j];
		}

		protected override object GetJoinTableId(int table, object obj)
		{
			//0 is the base table there is no join
			if (table == 0)
				return null;

			//check index first for speed
			var refIdColumn = GetRefIdColumnOfTable(table);
			if (refIdColumn == null)
				return null;

			object[] fields = GetPropertyValues(obj);
			return GetJoinTableId(table, refIdColumn, fields);
		}

		//gets the identifier for a join table if other than pk
		protected override object GetJoinTableId(int table, object[] fields)
		{
			//0 is the base table there is no join
			if (table == 0)
				return null;

			return GetJoinTableId(table, GetRefIdColumnOfTable(table), fields);
		}

		private static object GetJoinTableId(int table, int? index, object[] fields)
		{
			if (index == null)
				return null;

			return fields[index.Value];
		}

		//if the table's id is a reference column, returns the index of that property
		//returns null if not found
		protected override int? GetRefIdColumnOfTable(int table)
		{
			int value;
			if (tableIdPropertyNumbers.TryGetValue(table, out value))
				return value;

			return null;
		}

		protected override bool IsIdOfTable(int property, int table)
		{
			return GetRefIdColumnOfTable(table) == property;
		}

		protected override bool IsPropertyOfTable(int property, int table)
		{
			return propertyTableNumbers[property] == table;
		}

		protected override bool IsSubclassTableSequentialSelect(int table)
		{
			return subclassTableSequentialSelect[table] && !isClassOrSuperclassTable[table];
		}

		public override string FromTableFragment(string name)
		{
			return TableName + " " + name;
		}

		public override string FilterFragment(string alias)
		{
			string result = DiscriminatorFilterFragment(alias);
			if (HasWhere)
				result += " and " + GetSQLWhereString(alias);

			return result;
		}

		public override string OneToManyFilterFragment(string alias)
		{
			return forceDiscriminator ? DiscriminatorFilterFragment(alias) : string.Empty;
		}

		private string DiscriminatorFilterFragment(string alias)
		{
			const string abstractClassWithNoSubclassExceptionMessageTemplate = 
@"The class {0} can't be instatiated and does not have mapped subclasses; 
possible solutions:
- don't map the abstract class
- map its subclasses.";

			if (NeedsDiscriminator)
			{
				InFragment frag = new InFragment();

				if (IsDiscriminatorFormula)
				{
					frag.SetFormula(alias, DiscriminatorFormulaTemplate);
				}
				else
				{
					frag.SetColumn(alias, DiscriminatorColumnName);
				}

				string[] subclasses = SubclassClosure;
				int validValuesForInFragment = 0;
				foreach (string t in subclasses)
				{
					var queryable = (IQueryable) Factory.GetEntityPersister(t);
					if (!queryable.IsAbstract)
					{
						frag.AddValue(queryable.DiscriminatorSQLValue);
						validValuesForInFragment++;
					}
				}
				if(validValuesForInFragment == 0)
				{
					throw new NotSupportedException(string.Format(abstractClassWithNoSubclassExceptionMessageTemplate, subclasses[0]));
				}
				StringBuilder buf = new StringBuilder(50).Append(" and ").Append(frag.ToFragmentString().ToString());

				return buf.ToString();
			}
			else
			{
				return string.Empty;
			}
		}

		private bool NeedsDiscriminator
		{
			get { return forceDiscriminator || IsInherited; }
		}

		public override string GetSubclassPropertyTableName(int i)
		{
			return subclassTableNameClosure[subclassPropertyTableNumberClosure[i]];
		}

		protected override void AddDiscriminatorToSelect(SelectFragment select, string name, string suffix)
		{
			if (IsDiscriminatorFormula)
				select.AddFormula(name, DiscriminatorFormulaTemplate, DiscriminatorAlias);
			else
				select.AddColumn(name, DiscriminatorColumnName, DiscriminatorAlias);
		}

		protected override int GetSubclassPropertyTableNumber(int i)
		{
			return subclassPropertyTableNumberClosure[i];
		}

		protected override int TableSpan
		{
			get { return joinSpan; }
		}

		protected override void AddDiscriminatorToInsert(SqlInsertBuilder insert)
		{
			if (discriminatorInsertable)
				insert.AddColumn(DiscriminatorColumnName, DiscriminatorSQLValue);
		}

		protected override bool IsSubclassPropertyDeferred(string propertyName, string entityName)
		{
			return
				hasSequentialSelects && IsSubclassTableSequentialSelect(GetSubclassPropertyTableNumber(propertyName, entityName));
		}

		public override bool HasSequentialSelect
		{
			get { return hasSequentialSelects; }
		}

		public int GetSubclassPropertyTableNumber(string propertyName, string entityName)
		{
			IType type = propertyMapping.ToType(propertyName);
			if (type.IsAssociationType && ((IAssociationType)type).UseLHSPrimaryKey)
				return 0;
			int tabnum;
			propertyTableNumbersByNameAndSubclass.TryGetValue(entityName + '.' + propertyName, out tabnum);
			return tabnum;
		}

		protected override SqlString GetSequentialSelect(string entityName)
		{
			SqlString result;
			sequentialSelectStringsByEntityName.TryGetValue(entityName, out result);
			return result;
		}

		private SqlString GenerateSequentialSelect(ILoadable persister)
		{
			//note that this method could easily be moved up to BasicEntityPersister,
			//if we ever needed to reuse it from other subclasses

			//figure out which tables need to be fetched (only those that contains at least a no-lazy-property)
			AbstractEntityPersister subclassPersister = (AbstractEntityPersister)persister;
			var tableNumbers = new HashSet<int>();
			string[] props = subclassPersister.PropertyNames;
			string[] classes = subclassPersister.PropertySubclassNames;
			for (int i = 0; i < props.Length; i++)
			{
				int propTableNumber = GetSubclassPropertyTableNumber(props[i], classes[i]);
				if (IsSubclassTableSequentialSelect(propTableNumber) && !IsSubclassTableLazy(propTableNumber))
				{
					tableNumbers.Add(propTableNumber);
				}
			}
			if ((tableNumbers.Count == 0))
				return null;

			//figure out which columns are needed (excludes lazy-properties)
			List<int> columnNumbers = new List<int>();
			int[] columnTableNumbers = SubclassColumnTableNumberClosure;
			for (int i = 0; i < SubclassColumnClosure.Length; i++)
			{
				if (tableNumbers.Contains(columnTableNumbers[i]))
				{
					columnNumbers.Add(i);
				}
			}

			//figure out which formulas are needed (excludes lazy-properties)
			List<int> formulaNumbers = new List<int>();
			int[] formulaTableNumbers = SubclassFormulaTableNumberClosure;
			for (int i = 0; i < SubclassFormulaTemplateClosure.Length; i++)
			{
				if (tableNumbers.Contains(formulaTableNumbers[i]))
				{
					formulaNumbers.Add(i);
				}
			}

			//render the SQL
			return RenderSelect(tableNumbers.ToArray(), columnNumbers.ToArray(), formulaNumbers.ToArray());
		}

		//provide columns to join to if the key is other than the primary key
		protected override string[] GetJoinIdKeyColumns(int j)
		{
			string[] key;
			if (joinToKeyColumns.TryGetValue(j, out key))
				return key;

			return base.GetJoinIdKeyColumns(j);
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

		protected internal override bool IsSubclassTableLazy(int j)
		{
			return subclassTableIsLazyClosure[j];
		}

		protected override bool IsNullableTable(int j)
		{
			return isNullableTable[j];
		}

		protected override bool IsNullableSubclassTable(int j)
		{
			return isNullableSubclassTable[j];
		}

		public override string GetPropertyTableName(string propertyName)
		{
			int? index = EntityMetamodel.GetPropertyIndexOrNull(propertyName);
			if (!index.HasValue) return null;
			return qualifiedTableNames[propertyTableNumbers[index.Value]];
		}

		public override void PostInstantiate()
		{
			base.PostInstantiate();
			if (hasSequentialSelects)
			{
				string[] entityNames = SubclassClosure;
				for (int i = 1; i < entityNames.Length; i++)
				{
					ILoadable loadable = (ILoadable)Factory.GetEntityPersister(entityNames[i]);
					if (!loadable.IsAbstract)
					{
						//perhaps not really necessary...
						SqlString sequentialSelect = GenerateSequentialSelect(loadable);
						sequentialSelectStringsByEntityName[entityNames[i]] = sequentialSelect;
					}
				}
			}
		}
	}
}
