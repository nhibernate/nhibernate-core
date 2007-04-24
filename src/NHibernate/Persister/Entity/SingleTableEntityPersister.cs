using System;
using System.Collections;
using System.Text;
using Iesi.Collections;
using NHibernate.Cache;
using NHibernate.Engine;
using NHibernate.Mapping;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Persister.Entity
{
	using NHibernate.Mapping;

	/// <summary>
	/// Default implementation of the <c>ClassPersister</c> interface. Implements the
	/// "table-per-class hierarchy" mapping strategy for an entity class.
	/// </summary>
	public class SingleTableEntityPersister : AbstractEntityPersister, IQueryable
	{
		// the class hierarchy structure
		private readonly int joinSpan;
		private readonly string[] qualifiedTableNames;
		private readonly string[] tableNames;
		private readonly bool[] isInverseTable;
		private readonly bool[] isNullableTable;
		private readonly string[][] keyColumnNames;
		private readonly bool[] cascadeDeleteEnabled;
		private readonly bool hasSequentialSelects;

		private readonly String[] spaces;

		private readonly System.Type[] subclassClosure;

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
		private readonly Hashtable subclassesByDiscriminatorValue = new Hashtable();
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
		private readonly IDictionary propertyTableNumbersByNameAndSubclass = new Hashtable();

		private readonly IDictionary sequentialSelectStringsByEntityName = new Hashtable();


		private static readonly object NullDiscriminator = new object();
		private static readonly object NotNullDiscriminator = new object();

		public override string DiscriminatorColumnName
		{
			get { return discriminatorColumnName; }
		}

		protected override string DiscriminatorAlias
		{
			get { return discriminatorAlias; }
		}

		protected override string DiscriminatorFormulaTemplate
		{
			get { return discriminatorFormulaTemplate; }
		}

		public override string TableName
		{
			get { return qualifiedTableNames[0]; }
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

		public virtual System.Type[] SubclassClosure
		{
			get { return subclassClosure; }
		}

		public override System.Type GetSubclassForDiscriminatorValue(object value)
		{
			if (value == null)
			{
				return (System.Type) subclassesByDiscriminatorValue[NullDiscriminator];
			}
			else
			{
				System.Type result = (System.Type) subclassesByDiscriminatorValue[value];
				if (result == null)
				{
					result = (System.Type) subclassesByDiscriminatorValue[NotNullDiscriminator];
				}
				return result;
			}
		}

		public override object[] PropertySpaces
		{
			get { return qualifiedTableNames; }
		}

		protected bool IsDiscriminatorFormula
		{
			get { return discriminatorColumnName == null; }
		}

		protected string DiscriminatorFormula
		{
			get { return discriminatorFormula; }
		}

		// MOVED GenerateInsertString() to AbstractEntityPersister to implement <join>
		//protected override SqlCommandInfo GenerateInsertString(bool identityInsert, bool[] includeProperty, int j)
		//{
		//    base.GenerateInsertString(identityInsert, includeProperty, j);
		//}

		/// <summary>
		/// Generate the SQL that selects a row by id using <c>FOR UPDATE</c>
		/// </summary>
		/// <returns></returns>
		protected SqlString GenerateSelectForUpdateString()
		{
			return GenerateSelectString(" for update");
		}

		/// <summary>
		/// Generate the SQL that selects a row by id using <c>FOR UPDATE NOWAIT</c>
		/// </summary>
		/// <returns></returns>
		protected SqlString GenerateSelectForUpdateNoWaitString()
		{
			return GenerateSelectString(" for update nowait");
		}

		/// <summary>
		/// Generates an SqlString that selects a row by id
		/// </summary>
		/// <param name="forUpdateFragment">SQL containing <c>FOR UPDATE</c> clauses
		/// to append at the end of the query (optional)</param>
		/// <returns></returns>
		protected virtual SqlString GenerateSelectString(string forUpdateFragment)
		{
			SqlSimpleSelectBuilder builder = new SqlSimpleSelectBuilder(Factory);

			// set the table name and add the columns to select
			builder.SetTableName(TableName)
				.AddColumns(IdentifierColumnNames)
				.AddColumns(SubclassColumnClosure, SubclassColumnAliasClosure)
				.AddColumns(SubclassFormulaClosure, SubclassFormulaAliasClosure);

			if (HasSubclasses)
			{
				builder.AddColumn(DiscriminatorColumnName, DiscriminatorAlias);
			}

			// add the parameters to use in the WHERE clause
			builder.SetIdentityColumn(IdentifierColumnNames, IdentifierType);

			// Ok, render the SELECT statement
			SqlString selectSqlString = builder.ToSqlString();

			// add any special text that is contained in the forUpdateFragment
			if (forUpdateFragment != null && forUpdateFragment.Length > 0)
			{
				selectSqlString = selectSqlString.Append(forUpdateFragment);
			}

			return selectSqlString;
		}

		protected override int[] PropertyTableNumbersInSelect
		{
			get { return propertyTableNumbers; }
		}

		/// <summary>
		/// Generate the SQL that updates a row by id, excluding subclasses
		/// </summary>
		/// <param name="includeProperty"></param>
		/// <returns></returns>
		protected SqlCommandInfo GenerateUpdateString(bool[] includeProperty)
		{
			return GenerateUpdateString(includeProperty, 0, null);
		}

		/// <summary>
		/// Generates the SQL that pessimistically locks a row by id (and version)
		/// </summary>
		/// <param name="sqlString">An existing SqlString to copy for then new SqlString.</param>
		/// <param name="forUpdateFragment"></param>
		/// <returns>A new SqlString</returns>
		/// <remarks>
		/// The parameter <c>sqlString</c> does not get modified.  It is Cloned to make a new SqlString.
		/// If the parameter<c>sqlString</c> is null a new one will be created.
		/// </remarks>
		protected override SqlString GenerateLockString(SqlString sqlString, string forUpdateFragment)
		{
			SqlStringBuilder sqlBuilder;

			if (sqlString == null)
			{
				SqlSimpleSelectBuilder builder = new SqlSimpleSelectBuilder(Factory);

				// set the table name and add the columns to select
				builder.SetTableName(TableName)
					.AddColumns(IdentifierColumnNames);

				// add the parameters to use in the WHERE clause
				builder.SetIdentityColumn(IdentifierColumnNames, IdentifierType);
				if (IsVersioned)
				{
					builder.SetVersionColumn(new string[] {VersionColumnName}, VersionType);
				}

				sqlBuilder = new SqlStringBuilder(builder.ToSqlString());
			}
			else
			{
				sqlBuilder = new SqlStringBuilder(sqlString);
			}

			// add any special text that is contained in the forUpdateFragment
			if (forUpdateFragment != null && forUpdateFragment != string.Empty)
			{
				sqlBuilder.Add(forUpdateFragment);
			}

			return sqlBuilder.ToSqlString();
		}

		//INITIALIZATION:

		public SingleTableEntityPersister(PersistentClass model, ICacheConcurrencyStrategy cache,
		                                  ISessionFactoryImplementor factory, IMapping mapping)
			: base(model, cache, factory)
		{
			int i;

			// CLASS + TABLE
			#region CLASS + TABLE

			System.Type mappedClass = model.MappedClass;

			joinSpan = model.JoinClosureSpan + 1;
			qualifiedTableNames = new string[joinSpan];
			isInverseTable = new bool[joinSpan];
			isNullableTable = new bool[joinSpan];
			keyColumnNames = new string[joinSpan][];
			Table table = model.RootTable;
			qualifiedTableNames[0] = table.GetQualifiedName(Dialect, factory.DefaultSchema);
			isInverseTable[0] = false;
			isNullableTable[0] = false;
			keyColumnNames[0] = this.IdentifierColumnNames;
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

			customSQLInsert[0] = model.CustomSQLInsert;
			insertCallable[0] = customSQLInsert[0] != null && model.IsCustomInsertCallable;
			insertResultCheckStyles[0] = model.CustomSQLInsertCheckStyle == null
			                             	?
			                             ExecuteUpdateResultCheckStyle.DetermineDefault(customSQLInsert[0], insertCallable[0])
			                             	: model.CustomSQLInsertCheckStyle;
			customSQLUpdate[0] = model.CustomSQLUpdate;
			updateCallable[0] = customSQLUpdate[0] != null && model.IsCustomUpdateCallable;
			updateResultCheckStyles[0] = model.CustomSQLUpdateCheckStyle == null
			                             	?
			                             ExecuteUpdateResultCheckStyle.DetermineDefault(customSQLUpdate[0], updateCallable[0])
			                             	: model.CustomSQLUpdateCheckStyle;
			customSQLDelete[0] = model.CustomSQLDelete;
			deleteCallable[0] = customSQLDelete[0] != null && model.IsCustomDeleteCallable;
			deleteResultCheckStyles[0] = model.CustomSQLDeleteCheckStyle == null
			                             	?
			                             ExecuteUpdateResultCheckStyle.DetermineDefault(customSQLDelete[0], deleteCallable[0])
			                             	: model.CustomSQLDeleteCheckStyle;
			#endregion

			// JOINS
			#region JOINS

			int j = 1;
			foreach (Join join in model.JoinClosureCollection)
			{
				qualifiedTableNames[j] = join.Table.GetQualifiedName(
					factory.Dialect, 
					factory.Settings.DefaultSchemaName
				);
				isInverseTable[j] = join.IsInverse;
				isNullableTable[j] = join.IsOptional;
				//cascadeDeleteEnabled[j] = join.Key.IsCascadeDeleteEnabled && factory.Dialect.SupportsCascadeDelete;

				customSQLInsert[j] = join.CustomSQLInsert;
				insertCallable[j] = customSQLInsert[j] != null && join.IsCustomInsertCallable;
				insertResultCheckStyles[j] = join.CustomSQLInsertCheckStyle == null
					? ExecuteUpdateResultCheckStyle.DetermineDefault(customSQLInsert[j], insertCallable[j])
					: join.CustomSQLInsertCheckStyle;
				customSQLUpdate[j] = join.CustomSQLUpdate;
				updateCallable[j] = customSQLUpdate[j] != null && join.IsCustomUpdateCallable;
				updateResultCheckStyles[j] = join.CustomSQLUpdateCheckStyle == null
					? ExecuteUpdateResultCheckStyle.DetermineDefault(customSQLUpdate[j], updateCallable[j])
					: join.CustomSQLUpdateCheckStyle;
				customSQLDelete[j] = join.CustomSQLDelete;
				deleteCallable[j] = customSQLDelete[j] != null && join.IsCustomDeleteCallable;
				deleteResultCheckStyles[j] = join.CustomSQLDeleteCheckStyle == null
					? ExecuteUpdateResultCheckStyle.DetermineDefault(customSQLDelete[j], deleteCallable[j])
					: join.CustomSQLDeleteCheckStyle;

				ICollection keyColumns = join.Key.ColumnCollection;
				keyColumnNames[j] = new string[join.Key.ColumnSpan];
				i = 0;
				foreach (Column col in keyColumns)
				{
					keyColumnNames[j][i++] = col.GetQuotedName(factory.Dialect);
				}

				j++;
			}

			constraintOrderedTableNames = new string[qualifiedTableNames.Length];
			constraintOrderedKeyColumnNames = new string[qualifiedTableNames.Length][];
			for (int k = qualifiedTableNames.Length - 1, position = 0; k >= 0; k--, position++)
			{
				constraintOrderedTableNames[position] = qualifiedTableNames[k];
				constraintOrderedKeyColumnNames[position] = keyColumnNames[k];
			}

			spaces = ArrayHelper.Join(
				qualifiedTableNames,
				ArrayHelper.ToStringArray(model.SynchronizedTables));

			// TODO: H3 - IsInstrumented depends on EntityModel
			//bool lazyAvailable = IsInstrumented();

			bool hasDeferred = false;
			ArrayList subclassTables = new ArrayList();
			ArrayList joinKeyColumns = new ArrayList();
			ArrayList isConcretes = new ArrayList();
			ArrayList isDeferreds = new ArrayList();
			ArrayList isInverses = new ArrayList();
			ArrayList isNullables = new ArrayList();
			ArrayList isLazies = new ArrayList();
			subclassTables.Add(qualifiedTableNames[0]);
			joinKeyColumns.Add(IdentifierColumnNames);
			isConcretes.Add(false);
			isDeferreds.Add(false);
			isInverses.Add(false);
			isNullables.Add(false);
			isLazies.Add(false);

			foreach (Join join in model.SubclassJoinClosureCollection)
			{
				
				isConcretes.Add(model.IsClassOrSuperclassJoin(join));
				isDeferreds.Add(join.IsSequentialSelect);
				isInverses.Add(join.IsInverse);
				isNullables.Add(join.IsOptional);
				// TODO: Fix isLazies when lazy column is implemented
				isLazies.Add(false); //isLazies.Add(lazyAvailable && join.isLazy);
				if (join.IsSequentialSelect && !model.IsClassOrSuperclassJoin(join))
					hasDeferred = true;
				subclassTables.Add(join.Table.GetQualifiedName(factory.Dialect, factory.DefaultSchema));
				string[] keyCols = new string[join.Key.ColumnSpan];
				int k = 0;
				foreach (Column col in join.Key.ColumnCollection)
				{
					keyCols[k++] = col.GetQuotedName(factory.Dialect);
				}
				joinKeyColumns.AddRange(keyCols);
			}

			subclassTableSequentialSelect = ArrayHelper.ToBooleanArray(isDeferreds);
			subclassTableNameClosure = ArrayHelper.ToStringArray(subclassTables);
			subclassTableIsLazyClosure = ArrayHelper.ToBooleanArray(isLazies);
			subclassTableKeyColumnClosure = ArrayHelper.To2DStringArray(joinKeyColumns);
			isClassOrSuperclassTable = ArrayHelper.ToBooleanArray(isConcretes);
			isInverseSubclassTable = ArrayHelper.ToBooleanArray(isInverses);
			isNullableSubclassTable = ArrayHelper.ToBooleanArray(isNullables);
			hasSequentialSelects = hasDeferred;

			#endregion

			// detect mapping errors
			HashedSet distinctColumns = new HashedSet();

			// DISCRIMINATOR
			#region DISCRIMINATOR
			if (model.IsPolymorphic)
			{
				IValue d = model.Discriminator;
				if (d == null)
				{
					throw new MappingException("A discriminator mapping required for polymorphic persistence of " + model.Name);
				}
				forceDiscriminator = model.IsForceDiscriminator;

				// the discriminator will have only one column 
				foreach (ISelectable selectable in d.ColumnCollection)
				{
					if (d.HasFormula)
					{
						Formula formula = (Formula) selectable;
						discriminatorFormula = formula.FormulaString;
						discriminatorFormulaTemplate = formula.GetTemplate(factory.Dialect, factory.SQLFunctionRegistry);
						discriminatorColumnName = null;
						discriminatorAlias = "clazz_";
					}
					else
					{
						Column column = (Column) selectable;
						discriminatorColumnName = column.GetQuotedName(Dialect);
						discriminatorAlias = column.GetAlias(Dialect);
						discriminatorFormula = null;
						discriminatorFormulaTemplate = null;
					}
				}
				discriminatorType = model.Discriminator.Type;

				if (model.IsDiscriminatorValueNull)
				{
					discriminatorValue = NullDiscriminator;
					discriminatorSQLValue = InFragment.Null;
					discriminatorInsertable = false;
				}
				else if (model.IsDiscriminatorValueNotNull)
				{
					discriminatorValue = NotNullDiscriminator;
					discriminatorSQLValue = InFragment.NotNull;
					discriminatorInsertable = false;
				}
				else
				{
					discriminatorInsertable = model.IsDiscriminatorInsertable && !d.HasFormula;
					try
					{
						IDiscriminatorType dtype = (IDiscriminatorType) discriminatorType;
						discriminatorValue = dtype.StringToObject(model.DiscriminatorValue);
						discriminatorSQLValue = dtype.ObjectToSQLString(discriminatorValue);
					}
					catch (InvalidCastException)
					{
						throw new MappingException(string.Format("Illegal discriminator type: {0}", discriminatorType.Name));
					}
					catch (Exception e)
					{
						string msg = String.Format("Could not format discriminator value '{0}' to sql string using the IType {1}",
						                           model.DiscriminatorValue,
						                           model.Discriminator.Type.ToString());

						throw new MappingException(msg, e);
					}

					if (discriminatorInsertable)
					{
						distinctColumns.Add(discriminatorColumnName);
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
			}
			#endregion 

			// PROPERTIES
			#region PROPERTIES

			propertyTableNumbers = new int[PropertySpan];
			i = 0;
			foreach (Property prop in model.PropertyClosureCollection)
			{
				propertyTableNumbers[i++] = model.GetJoinNumber(prop);
			}

			ArrayList columnJoinNumbers = new ArrayList();
			ArrayList formulaJoinedNumbers = new ArrayList();
			ArrayList propertyJoinNumbers = new ArrayList();

			foreach (Property prop in model.SubclassPropertyClosureCollection)
			{
				int joinNumber = model.GetJoinNumber(prop);
				propertyJoinNumbers.Add(joinNumber);

				propertyTableNumbersByNameAndSubclass.Add(
					prop.PersistentClass.MappedClass.FullName + "." + prop.Name,
					joinNumber);

				foreach (ISelectable thing in prop.ColumnCollection)
				{
					if (thing.IsFormula)
						formulaJoinedNumbers.Add(joinNumber);
					else
						columnJoinNumbers.Add(joinNumber);
				}
			}
			subclassColumnTableNumberClosure = ArrayHelper.ToIntArray(columnJoinNumbers);
			subclassFormulaTableNumberClosure = ArrayHelper.ToIntArray(formulaJoinedNumbers);
			subclassPropertyTableNumberClosure = ArrayHelper.ToIntArray(propertyJoinNumbers);

			// SQL string generation moved to PostInstantiate

			int subclassSpan = model.SubclassSpan + 1;
			subclassClosure = new System.Type[subclassSpan];
			subclassClosure[0] = mappedClass;
			if (model.IsPolymorphic)
			{
				subclassesByDiscriminatorValue.Add(discriminatorValue, mappedClass);
			}
			#endregion

			// SUBCLASSES
			#region SUBCLASSES
			if (model.IsPolymorphic)
			{
				int k = 1;
				foreach (Subclass sc in model.SubclassCollection)
				{
					subclassClosure[k++] = sc.MappedClass;
					if (sc.IsDiscriminatorValueNull)
					{
						subclassesByDiscriminatorValue.Add(NullDiscriminator, sc.MappedClass);
					}
					else if (sc.IsDiscriminatorValueNotNull)
					{
						subclassesByDiscriminatorValue.Add(NotNullDiscriminator, sc.MappedClass);
					}
					else
					{
						try
						{
							IDiscriminatorType dtype = discriminatorType as IDiscriminatorType;
							subclassesByDiscriminatorValue.Add(
								dtype.StringToObject(sc.DiscriminatorValue),
								sc.MappedClass);
						}
						catch (InvalidCastException)
						{
							throw new MappingException(string.Format("Illegal discriminator type: {0}", discriminatorType.Name));
						}
						catch (Exception e)
						{
							throw new MappingException(string.Format("Error parsing discriminator value: '{0}'", sc.DiscriminatorValue), e);
						}
					}
				}
			}
			#endregion

			// This is in PostInstatiate as it needs identifier info
			//InitLockers();

			InitSubclassPropertyAliasesMap(model);

			PostConstruct(mapping);
		}

		protected override bool IsInverseTable(int j)
		{
			return isInverseTable[j];
		}

		protected override bool IsInverseSubclassTable(int j)
		{
			return isInverseSubclassTable[j];
		}

		public override SqlString QueryWhereFragment(string name, bool innerJoin, bool includeSubclasses)
		{
			if (innerJoin && NeedsDiscriminator)
			{
				SqlStringBuilder builder = new SqlStringBuilder();
				builder.Add(" and " + DiscriminatorWhereCondition(name));

				if (HasWhere)
				{
					builder.Add(" and " + GetSQLWhereString(name));
				}

				return builder.ToSqlString();
			}
			else
			{
				if (HasWhere)
				{
					return new SqlString(" and " + GetSQLWhereString(name));
				}
				else
				{
					return SqlString.Empty;
				}
			}
		}

		private SqlString DiscriminatorWhereCondition(string alias)
		{
			InFragment frag = new InFragment()
				.SetColumn(alias, DiscriminatorColumnName);

			if (IsDiscriminatorFormula)
			{
				frag.SetFormula(alias, DiscriminatorFormulaTemplate);
			}
			else
			{
				frag.SetColumn(alias, DiscriminatorColumnName);
			}

			System.Type[] subclasses = SubclassClosure;
			for (int i = 0; i < subclasses.Length; i++)
			{
				IQueryable queryable = (IQueryable) Factory.GetEntityPersister(subclasses[i]);
				frag.AddValue(queryable.DiscriminatorSQLValue);
			}

			return frag.ToFragmentString();
		}

		public override string GetSubclassPropertyTableName(int i)
		{
			return subclassTableNameClosure[subclassPropertyTableNumberClosure[i]];
		}

		public override SqlString WhereJoinFragment(string alias, bool innerJoin, bool includeSubclasses)
		{
			return SqlString.Empty;
		}

		/// <summary></summary>
		public override bool IsCacheInvalidationRequired
		{
			get { return HasFormulaProperties || (!IsVersioned && UseDynamicUpdate); }
		}

		/// <summary></summary>
		protected override string VersionedTableName
		{
			get { return qualifiedTableNames[0]; }
		}

		protected override bool IsSubclassPropertyDeferred(string propertyName, System.Type entityName)
		{
			return hasSequentialSelects &&
				IsSubclassTableSequentialSelect(GetSubclassPropertyTableNumber(propertyName, entityName));
		}

		public override bool HasSequentialSelect
		{
			get { return hasSequentialSelects; }
		}

		public int GetSubclassPropertyTableNumber(string propertyPath, System.Type entityName)
		{
			IType type = propertyMapping.ToType(propertyPath);
			if (type.IsAssociationType && ((IAssociationType)type).UseLHSPrimaryKey)
				return 0;
			string propertyFullName = entityName.FullName + '.' + propertyPath;
			if (propertyTableNumbersByNameAndSubclass.Contains(propertyFullName))
			{
				return (int)propertyTableNumbersByNameAndSubclass[propertyFullName];
			}
			else
			{
				return 0;
			}
		}

		protected override SqlString GetSequentialSelect(System.Type entityName)
		{
			return (SqlString)sequentialSelectStringsByEntityName[entityName];
		}

		private SqlString GenerateSequentialSelect(ILoadable persister)
		{
			//note that this method could easily be moved up to BasicEntityPersister,
			//if we ever needed to reuse it from other subclasses

			//figure out which tables need to be fetched
			AbstractEntityPersister subclassPersister = (AbstractEntityPersister)persister;
			HashedSet tableNumbers = new HashedSet();
			string[] props = subclassPersister.PropertyNames;
			System.Type[] classes = subclassPersister.PropertySubclassNames;
			for (int i = 0; i < props.Length; i++)
			{
				int propTableNumber = GetSubclassPropertyTableNumber(props[i], classes[i]);
				if (IsSubclassTableSequentialSelect(propTableNumber) && !IsSubclassTableLazy(propTableNumber))
				{
					tableNumbers.Add(propTableNumber);
				}
			}
			if (tableNumbers.IsEmpty) return null;

			//figure out which columns are needed
			ArrayList columnNumbers = new ArrayList();
			int[] columnTableNumbers = SubclassColumnTableNumberClosure;
			for (int i = 0; i < SubclassColumnClosure.Length; i++)
			{
				if (tableNumbers.Contains(columnTableNumbers[i]))
				{
					columnNumbers.Add(i);
				}
			}

			//figure out which formulas are needed
			ArrayList formulaNumbers = new ArrayList();
			int[] formulaTableNumbers = SubclassColumnTableNumberClosure;
			for (int i = 0; i < SubclassFormulaTemplateClosure.Length; i++)
			{
				if (tableNumbers.Contains(formulaTableNumbers[i]))
				{
					formulaNumbers.Add(i);
				}
			}

			//render the SQL
			return RenderSelect(
				ArrayHelper.ToIntArray(tableNumbers),
				ArrayHelper.ToIntArray(columnNumbers),
				ArrayHelper.ToIntArray(formulaNumbers)
			);
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

		protected override void AddDiscriminatorToSelect(SelectFragment select, string name, string suffix)
		{
			if (IsDiscriminatorFormula)
			{
				select.AddFormula(name, DiscriminatorFormulaTemplate, DiscriminatorAlias);
			}
			else
			{
				select.AddColumn(name, DiscriminatorColumnName, DiscriminatorAlias);
			}
		}

		protected override int GetSubclassPropertyTableNumber(int i)
		{
			return subclassPropertyTableNumberClosure[i];
		}

		protected override void AddDiscriminatorToInsert(SqlInsertBuilder insert)
		{
			if (discriminatorInsertable)
			{
				insert.AddColumn(DiscriminatorColumnName, DiscriminatorSQLValue);
			}
		}

		protected override int[] SubclassColumnTableNumberClosure
		{
			get { return this.subclassColumnTableNumberClosure; }
		}

		protected override int[] SubclassFormulaTableNumberClosure
		{
			get { return this.subclassFormulaTableNumberClosure; }
		}

		public override string GetPropertyTableName(string propertyName)
		{
			return tableNames[0];
		}

		public override string FilterFragment(string alias)
		{
			string result = DiscriminatorFilterFragment(alias);
			if (HasWhere)
			{
				result += " and " + GetSQLWhereString(alias);
			}
			return result;
		}

		private string DiscriminatorFilterFragment(string alias)
		{
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

				System.Type[] subclasses = SubclassClosure;
				for (int i = 0; i < subclasses.Length; i++)
				{
					IQueryable queryable = (IQueryable) Factory.GetEntityPersister(subclasses[i]);

					// TODO H3:
//					if ( !queryable.IsAbstract )
//					{
					frag.AddValue(queryable.DiscriminatorSQLValue);
//					}
				}

				StringBuilder buf = new StringBuilder(50)
					.Append(" and ")
					.Append(frag.ToFragmentString().ToString());

				return buf.ToString();
			}
			else
			{
				return "";
			}
		}

		private bool NeedsDiscriminator
		{
			get { return forceDiscriminator || IsInherited; }
		}

		public override string OneToManyFilterFragment(string alias)
		{
			return forceDiscriminator
			       	?
			       DiscriminatorFilterFragment(alias)
			       	:
			       string.Empty;
		}

		protected override int TableSpan
		{
			get { return joinSpan; }
		}

		protected override bool IsPropertyOfTable(int property, int table)
		{
			return propertyTableNumbers[property] == table;
		}

		protected override bool IsSubclassTableSequentialSelect(int table)
		{
			return subclassTableSequentialSelect[table] && !isClassOrSuperclassTable[table];
		}

		protected override string[] GetKeyColumns(int table)
		{
			return keyColumnNames[table];
		}

		protected override string GetTableName(int table)
		{
			return qualifiedTableNames[table];
		}

		protected override int[] PropertyTableNumbers
		{
			get { return propertyTableNumbers; }
		}

		protected bool IsSubclassTableLazy(int j)
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

		public override void PostInstantiate()
		{
			base.PostInstantiate();
			if (hasSequentialSelects)
			{
				System.Type[] entityNames = SubclassClosure;
				for (int i = 1; i < entityNames.Length; i++)
				{
					ILoadable loadable = (ILoadable)Factory.GetEntityPersister(entityNames[i]);
					if (!loadable.IsAbstract)
					{ //perhaps not really necessary...
						SqlString sequentialSelect = GenerateSequentialSelect(loadable);
						sequentialSelectStringsByEntityName[entityNames[i]] = sequentialSelect;
					}
				}
			}
		}

		public override bool IsMultiTable
		{
			get { return TableSpan > 1; }
		}


	}
}
