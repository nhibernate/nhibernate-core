using System;
using System.Collections;
using System.Text;
using Iesi.Collections;
using NHibernate.Cache;
using NHibernate.Engine;
using NHibernate.Mapping;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate.Persister.Entity
{
	/// <summary>
	/// Default implementation of the <c>ClassPersister</c> interface. Implements the
	/// "table-per-class hierarchy" mapping strategy for an entity class.
	/// </summary>
	public class SingleTableEntityPersister : AbstractEntityPersister, IQueryable
	{
		// the class hierarchy structure
		private readonly string qualifiedTableName;
		private readonly string[] tableNames;
		private readonly System.Type[] subclassClosure;

		// discriminator column
		private readonly Hashtable subclassesByDiscriminatorValue = new Hashtable();
		private readonly bool forceDiscriminator;
		private readonly string discriminatorColumnName;
		private readonly string discriminatorFormula;
		private readonly string discriminatorFormulaTemplate;
		private readonly string discriminatorAlias;
		private readonly IType discriminatorType;
		private readonly string discriminatorSQLValue;
		private readonly bool discriminatorInsertable;

		private readonly int[] propertyTableNumbers;

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

		protected string DiscriminatorFormulaTemplate
		{
			get { return discriminatorFormulaTemplate; }
		}

		public override string TableName
		{
			get { return qualifiedTableName; }
		}

		public override IType DiscriminatorType
		{
			get { return discriminatorType; }
		}

		public override string DiscriminatorSQLValue
		{
			get { return discriminatorSQLValue; }
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
			get { return tableNames; }
		}

		protected bool IsDiscriminatorFormula
		{
			get { return discriminatorColumnName == null; }
		}

		protected string DiscriminatorFormula
		{
			get { return discriminatorFormula; }
		}

		protected override SqlCommandInfo GenerateInsertString(bool identityInsert, bool[] includeProperty, int j)
		{
			SqlInsertBuilder builder = new SqlInsertBuilder(Factory)
				.SetTableName(TableName);

			for (int i = 0; i < HydrateSpan; i++)
			{
				if (includeProperty[i])
				{
					builder.AddColumns( GetPropertyColumnNames( i ), PropertyColumnInsertable[i], PropertyTypes[i] );
				}
			}

			if (discriminatorInsertable)
			{
				builder.AddColumn(DiscriminatorColumnName, DiscriminatorSQLValue);
			}

			if (!identityInsert)
			{
				builder.AddColumn(IdentifierColumnNames, IdentifierType);
			}
			else
			{
				// make sure the Dialect has an identity insert string because we don't want
				// to add the column when there is no value to supply the SqlBuilder
				if (Dialect.IdentityInsertString != null)
				{
					// only 1 column if there is IdentityInsert enabled.
					builder.AddColumn(IdentifierColumnNames[0], Dialect.IdentityInsertString);
				}
			}

			return builder.ToSqlCommandInfo();
		}

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

		/// <summary>
		/// Generate the SQL that selects a row by id, excluding subclasses
		/// </summary>
		protected override SqlString GenerateConcreteSelectString()
		{
			SqlSimpleSelectBuilder builder = new SqlSimpleSelectBuilder(Factory);

			// set the table and the identity columns
			builder.SetTableName(TableName)
				.AddColumns(IdentifierColumnNames);

			for (int i = 0; i < PropertyNames.Length; i++)
			{
				if (PropertyUpdateability[i])
				{
					builder.AddColumns(GetPropertyColumnNames(i), GetPropertyColumnAliases(i));
				}
			}

			builder.SetIdentityColumn(IdentifierColumnNames, IdentifierType);
			if (IsVersioned)
			{
				builder.SetVersionColumn(new string[] {VersionColumnName}, VersionType);
			}

			return builder.ToSqlString();
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

		public SingleTableEntityPersister(PersistentClass model, ICacheConcurrencyStrategy cache, ISessionFactoryImplementor factory, IMapping mapping)
			: base(model, cache, factory)
		{
			// CLASS + TABLE

			System.Type mappedClass = model.MappedClass;
			Table table = model.RootTable;
			qualifiedTableName = table.GetQualifiedName(Dialect, factory.DefaultSchema);
			tableNames = new string[] {qualifiedTableName};

			// Custom sql
			customSQLInsert = new SqlString[1];
			customSQLUpdate = new SqlString[1];
			customSQLDelete = new SqlString[1];
			insertCallable = new bool[1];
			updateCallable = new bool[1];
			deleteCallable = new bool[1];
			insertResultCheckStyles = new ExecuteUpdateResultCheckStyle[1];
			updateResultCheckStyles = new ExecuteUpdateResultCheckStyle[1];
			deleteResultCheckStyles = new ExecuteUpdateResultCheckStyle[1];

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

			// detect mapping errors
			HashedSet distinctColumns = new HashedSet();

			// DISCRIMINATOR
			object discriminatorValue;
			if (model.IsPolymorphic)
			{
				IValue d = model.Discriminator;
				if (d == null)
				{
					throw new MappingException("discriminator mapping required for polymorphic persistence");
				}
				forceDiscriminator = model.IsForceDiscriminator;

				// the discriminator will have only one column 
				foreach (ISelectable selectable in d.ColumnCollection)
				{
					if (d.HasFormula)
					{
						Formula formula = (Formula)selectable;
						discriminatorFormula = formula.FormulaString;
						discriminatorFormulaTemplate = formula.GetTemplate(factory.Dialect);
						discriminatorColumnName = null;
						discriminatorAlias = "clazz_";
					}
					else
					{
						Column column = (Column)selectable;
						discriminatorColumnName = column.GetQuotedName( Dialect );
						discriminatorAlias = column.GetAlias( Dialect );
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

			// PROPERTIES
			HashedSet thisClassProperties = new HashedSet();

			foreach (Mapping.Property prop in model.PropertyClosureCollection)
			{
				thisClassProperties.Add(prop);
			}

			// SQL string generation moved to PostInstantiate

			int subclassSpan = model.SubclassSpan + 1;
			subclassClosure = new System.Type[subclassSpan];
			subclassClosure[0] = mappedClass;
			if (model.IsPolymorphic)
			{
				subclassesByDiscriminatorValue.Add(discriminatorValue, mappedClass);
			}

			// SUBCLASSES
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

			// This is in PostInstatiate as it needs identifier info
			//InitLockers();

			propertyTableNumbers = new int[EntityMetamodel.PropertySpan];
			for (int i = 0; i < propertyTableNumbers.Length; i++)
			{
				propertyTableNumbers[i] = 0;
			}

			InitSubclassPropertyAliasesMap(model);
			PostConstruct(mapping);
		}

		public override SqlString FromTableFragment(string alias)
		{
			return new SqlString(TableName + ' ' + alias);
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
				frag.SetFormula( alias, DiscriminatorFormulaTemplate );
			}
			else
			{
				frag.SetColumn( alias, DiscriminatorColumnName );
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
			return qualifiedTableName;
		}

		public override SqlString FromJoinFragment(string alias, bool innerJoin, bool includeSubclasses)
		{
			return SqlString.Empty;
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
			get { return qualifiedTableName; }
		}

		protected override int GetSubclassPropertyTableNumber(int i)
		{
			return 0;
		}

		protected override void AddDiscriminatorToSelect(SelectFragment select, string name, string suffix)
		{
			if (IsDiscriminatorFormula)
			{
				select.AddFormula( name, DiscriminatorFormulaTemplate, DiscriminatorAlias );
			}
			else
			{
				select.AddColumn( name, DiscriminatorColumnName, DiscriminatorAlias );
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

		public override string GetPropertyTableName(string propertyName)
		{
			return TableName;
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
			get { return 1; }
		}

		protected override bool IsPropertyOfTable(int property, int table)
		{
			return true;
		}

		protected override string[] GetKeyColumns(int table)
		{
			return KeyColumnNames;
		}

		protected override string GetTableName(int table)
		{
			return tableNames[table];
		}

		protected override int[] PropertyTableNumbers
		{
			get { return propertyTableNumbers; }
		}
	}
}
