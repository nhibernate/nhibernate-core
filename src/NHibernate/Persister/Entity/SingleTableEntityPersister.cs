using System;
using System.Collections.Generic;
using System.Text;
using Iesi.Collections.Generic;
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

		public SingleTableEntityPersister(PersistentClass persistentClass, ICacheConcurrencyStrategy cache,
																			ISessionFactoryImplementor factory, IMapping mapping)
			: base(persistentClass, cache, factory)
		{
			#region CLASS + TABLE

			joinSpan = persistentClass.JoinClosureSpan + 1;
			qualifiedTableNames = new string[joinSpan];
			isInverseTable = new bool[joinSpan];
			isNullableTable = new bool[joinSpan];
			keyColumnNames = new string[joinSpan][];
			Table table = persistentClass.RootTable;
			qualifiedTableNames[0] =
				table.GetQualifiedName(factory.Dialect, factory.Settings.DefaultCatalogName, factory.Settings.DefaultSchemaName);
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

				IEnumerable<Column> enumerableKeyCol = new SafetyEnumerable<Column>(join.Key.ColumnIterator);
				List<string> kcName = new List<string>(join.Key.ColumnSpan);
				foreach (Column col in enumerableKeyCol)
					kcName.Add(col.GetQuotedName(factory.Dialect));

				keyColumnNames[j] = kcName.ToArray();

				j++;
			}

			constraintOrderedTableNames = new string[qualifiedTableNames.Length];
			constraintOrderedKeyColumnNames = new string[qualifiedTableNames.Length][];
			for (int i = qualifiedTableNames.Length - 1, position = 0; i >= 0; i--, position++)
			{
				constraintOrderedTableNames[position] = qualifiedTableNames[i];
				constraintOrderedKeyColumnNames[position] = keyColumnNames[i];
			}

			spaces = ArrayHelper.Join(qualifiedTableNames, ArrayHelper.ToStringArray(persistentClass.SynchronizedTables));

			bool lazyAvailable = IsInstrumented(EntityMode.Poco);

			bool hasDeferred = false;
			List<string> subclassTables = new List<string>();
			List<string[]> joinKeyColumns = new List<string[]>();
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
				IEnumerable<Column> enumerableKeyCol = new SafetyEnumerable<Column>(join.Key.ColumnIterator);
				List<string> keyCols = new List<string>(join.Key.ColumnSpan);
				foreach (Column col in enumerableKeyCol)
					keyCols.Add(col.GetQuotedName(factory.Dialect));

				joinKeyColumns.Add(keyCols.ToArray());
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

				//propertyTableNumbersByName.put( prop.getName(), join );
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

		public virtual string[] SubclassClosure
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

		public override string[][] ContraintOrderedTableKeyColumnClosure
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
			return TableName + ' ' + name;
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
- map the its subclasses.";

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

			//figure out which tables need to be fetched
			AbstractEntityPersister subclassPersister = (AbstractEntityPersister)persister;
			HashedSet<int> tableNumbers = new HashedSet<int>();
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

			//figure out which columns are needed
			List<int> columnNumbers = new List<int>();
			int[] columnTableNumbers = SubclassColumnTableNumberClosure;
			for (int i = 0; i < SubclassColumnClosure.Length; i++)
			{
				if (tableNumbers.Contains(columnTableNumbers[i]))
					columnNumbers.Add(i);
			}

			//figure out which formulas are needed
			List<int> formulaNumbers = new List<int>();
			int[] formulaTableNumbers = SubclassColumnTableNumberClosure;
			for (int i = 0; i < SubclassFormulaTemplateClosure.Length; i++)
			{
				if (tableNumbers.Contains(formulaTableNumbers[i]))
					formulaNumbers.Add(i);
			}

			//render the SQL
			return RenderSelect(ArrayHelper.ToIntArray(tableNumbers), columnNumbers.ToArray(), formulaNumbers.ToArray());
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
