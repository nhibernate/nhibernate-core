using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NHibernate.Engine;
using NHibernate.Id;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Mapping
{
	/// <summary>
	/// Any value that maps to columns.
	/// </summary>
	[Serializable]
	public class SimpleValue : IKeyValue
	{
		private readonly List<ISelectable> columns = new List<ISelectable>();
		private IType type;
		private IDictionary<string, string> typeParameters;

		private IDictionary<string, string> identifierGeneratorProperties;
		private string identifierGeneratorStrategy = "assigned";
		private string nullValue;
		private Table table;
		private string foreignKeyName;
		private bool cascadeDeleteEnabled;
		private bool isAlternateUniqueKey;
		private string typeName;

		public SimpleValue()
		{
		}

		public SimpleValue(Table table)
		{
			this.table = table;
		}

		public virtual IEnumerable<Column> ConstraintColumns
		{
			get { return new SafetyEnumerable<Column>(columns); }
		}

		public string ForeignKeyName
		{
			get { return foreignKeyName; }
			set { foreignKeyName = value; }
		}

		public Table Table
		{
			get { return table; }
			set { table = value; }
		}

		public IDictionary<string, string> IdentifierGeneratorProperties
		{
			get { return identifierGeneratorProperties; }
			set { identifierGeneratorProperties = value; }
		}

		public string IdentifierGeneratorStrategy
		{
			get { return identifierGeneratorStrategy; }
			set { identifierGeneratorStrategy = value; }
		}

		public virtual bool IsComposite
		{
			get { return false; }
		}

		#region IKeyValue Members

		public void CreateForeignKeyOfEntity(string entityName)
		{
			if (!HasFormula && ! "none".Equals(ForeignKeyName, StringComparison.InvariantCultureIgnoreCase))
			{
				ForeignKey fk = table.CreateForeignKey(ForeignKeyName, ConstraintColumns, entityName);
				fk.CascadeDeleteEnabled = cascadeDeleteEnabled;
			}
		}

		public bool IsCascadeDeleteEnabled
		{
			get { return cascadeDeleteEnabled; }
			set { cascadeDeleteEnabled = value; }
		}

		public bool IsIdentityColumn(Dialect.Dialect dialect)
		{
			return
				IdentifierGeneratorFactory.GetIdentifierGeneratorClass(identifierGeneratorStrategy, dialect).Equals(
					typeof (IdentityGenerator));
		}

		public string NullValue
		{
			get { return nullValue; }
			set { nullValue = value; }
		}

		public virtual bool IsUpdateable
		{
			get { return true; }//needed to satisfy IKeyValue
		}

		public virtual bool IsTypeSpecified
		{
			get { return typeName != null; }
		}

		public IDictionary<string, string> TypeParameters
		{
			get { return typeParameters; }
			set
			{
				if (!CollectionHelper.DictionaryEquals((IDictionary)typeParameters, (IDictionary)value))
				{
					typeParameters = value;
					type = null; // invalidate type
				}
			}
		}

		public string TypeName
		{
			get { return typeName; }
			set
			{
				if ((typeName == null && value != null) || (typeName != null && !typeName.Equals(value)))
				{
					// the property change
					typeName = value;
					type = null; // invalidate type
				}
			}
		}

		public IIdentifierGenerator CreateIdentifierGenerator(Dialect.Dialect dialect, string defaultCatalog,
		                                                      string defaultSchema, RootClass rootClass)
		{
			Dictionary<string, string> @params = new Dictionary<string, string>();

			//if the hibernate-mapping did not specify a schema/catalog, use the defaults
			//specified by properties - but note that if the schema/catalog were specified
			//in hibernate-mapping, or as params, they will already be initialized and
			//will override the values set here (they are in identifierGeneratorProperties)
			if (!string.IsNullOrEmpty(defaultSchema))
			{
				@params[PersistentIdGeneratorParmsNames.Schema] = defaultSchema;
			}
			if (!string.IsNullOrEmpty(defaultCatalog))
			{
				@params[PersistentIdGeneratorParmsNames.Catalog] = defaultCatalog;
			}

			//pass the entity-name, if not a collection-id
			if (rootClass != null)
			{
				@params[IdGeneratorParmsNames.EntityName] = rootClass.EntityName;
			}

			//init the table here instead of earlier, so that we can get a quoted table name
			//TODO: would it be better to simply pass the qualified table name, instead of
			//      splitting it up into schema/catalog/table names
			string tableName = Table.GetQuotedName(dialect);

			@params[PersistentIdGeneratorParmsNames.Table] = tableName;

			//pass the column name (a generated id almost always has a single column and is not a formula)
			IEnumerator enu = ColumnIterator.GetEnumerator();
			enu.MoveNext();
			string columnName = ((Column)enu.Current).GetQuotedName(dialect);

			@params[PersistentIdGeneratorParmsNames.PK] = columnName;

			if (rootClass != null)
			{
				StringBuilder tables = new StringBuilder();
				bool commaNeeded = false;
				foreach (Table identityTable in rootClass.IdentityTables)
				{
					if (commaNeeded)
						tables.Append(StringHelper.CommaSpace);
					commaNeeded = true;
					tables.Append(identityTable.GetQuotedName(dialect));
				}
				@params[PersistentIdGeneratorParmsNames.Tables] = tables.ToString();
			}
			else
			{
				@params[PersistentIdGeneratorParmsNames.Tables] = tableName;
			}

			if (identifierGeneratorProperties != null)
			{
				ArrayHelper.AddAll(@params, identifierGeneratorProperties);
			}

			return IdentifierGeneratorFactory.Create(identifierGeneratorStrategy, Type, @params, dialect);
		}

		#endregion

		#region IValue Members

		public virtual int ColumnSpan
		{
			get { return columns.Count; }
		}

		public virtual IEnumerable<ISelectable> ColumnIterator
		{
			get { return columns; }
		}

		public virtual IType Type
		{
			get
			{
				// NH: Different implementation
				// we use the internal instance of IType to have better performance
				if (type == null)
				{
					if (string.IsNullOrEmpty(typeName))
					{
						throw new MappingException("No type name specified");
					}
					type = TypeFactory.HeuristicType(typeName, (IDictionary)typeParameters);
					if (type == null)
					{
						string msg = "Could not determine type for: " + typeName;
						if (columns != null && columns.Count > 0)
						{
							msg += (", for columns: " + StringHelper.CollectionToString(columns));
						}
						throw new MappingException(msg);
					}
				}
				return type;
			}
		}

		public bool HasFormula
		{
			get
			{
				foreach (ISelectable s in ColumnIterator)
				{
					if (s.IsFormula)
						return true;
				}
				return false;
			}
		}

		public virtual bool IsNullable
		{
			get
			{
				// NH : Different implementation (to iterate the collection only one time)
				foreach (ISelectable selectable in ColumnIterator)
				{
					if (selectable.IsFormula)
						return true;
					if (!((Column)selectable).IsNullable)
						return false;
				}
				return true;
			}
		}

		public virtual bool[] ColumnUpdateability
		{
			get { return ColumnInsertability; }
		}

		public virtual bool[] ColumnInsertability
		{
			get
			{
				bool[] result = new bool[ColumnSpan];
				int i = 0;
				foreach (ISelectable s in ColumnIterator)
				{
					result[i++] = !s.IsFormula;
				}
				return result;
			}
		}

		public bool IsSimpleValue
		{
			get { return true; }
		}

		public virtual bool IsValid(IMapping mapping)
		{
			return ColumnSpan == Type.GetColumnSpan(mapping);
		}

		public virtual void CreateForeignKey()
		{
		}

		public virtual FetchMode FetchMode
		{
			get { return FetchMode.Select; }
			// Needed so that subclasses have something to override
			set { throw new NotSupportedException(); }
		}

		public bool IsAlternateUniqueKey
		{
			get { return isAlternateUniqueKey; }
			set { isAlternateUniqueKey = value; }
		}

		public virtual void SetTypeUsingReflection(string className, string propertyName, string accesorName)
		{
			if (typeName == null)
			{
				if (className == null)
				{
					throw new MappingException("you must specify types for a dynamic entity: " + propertyName);
				}
				try
				{
					typeName = ReflectHelper.ReflectedPropertyClass(className, propertyName, accesorName).AssemblyQualifiedName;
				}
				catch (HibernateException he)
				{
					throw new MappingException("Problem trying to set property type by reflection", he);
				}
			}
		}

		public virtual object Accept(IValueVisitor visitor)
		{
			return visitor.Accept(this);
		}

		#endregion

		public virtual void AddColumn(Column column)
		{
			if (!columns.Contains(column))
				columns.Add(column);

			column.Value = this;
			column.TypeIndex = columns.Count - 1;
		}

		public virtual void AddFormula(Formula formula)
		{
			columns.Add(formula);
		}

		public override string ToString()
		{
			return string.Format("{0}({1})", GetType().FullName, StringHelper.CollectionToString(columns));
		}
	}
}
