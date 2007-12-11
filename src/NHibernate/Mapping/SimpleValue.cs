using System;
using System.Collections;
using NHibernate.Engine;
using NHibernate.Id;
using NHibernate.Type;
using NHibernate.Util;
using System.Collections.Generic;

namespace NHibernate.Mapping
{
	/// <summary>
	/// Any value that maps to columns.
	/// </summary>
	public class SimpleValue : IKeyValue
	{
		private readonly List<ISelectable> columns = new List<ISelectable>();
		private IType type;

		private IDictionary identifierGeneratorProperties;
		private string identifierGeneratorStrategy = "assigned";
		private string nullValue;
		private Table table;
		private string foreignKeyName;
		private bool unique;
		private IIdentifierGenerator uniqueIdentifierGenerator;
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

		public string TypeName
		{
			get { return typeName; }
			set { typeName = value; }
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

		public IDictionary IdentifierGeneratorProperties
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
			return IdentifierGeneratorFactory.GetIdentifierGeneratorClass(identifierGeneratorStrategy, dialect)
				.Equals(typeof(IdentityGenerator));
		}

		public string NullValue
		{
			get { return nullValue; }
			set { nullValue = value; }
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
			get { return type; }
			set
			{
				// TODO NH: Remove this method and implement only the getter
				type = value;
				int count = 0;

				foreach (ISelectable sel in ColumnIterator)
				{
					if (sel is Column)
					{
						Column col = (Column)sel;
						col.Type = type;
						col.TypeIndex = count++;
					}
				}
			}
		}

		public bool HasFormula
		{
			get
			{
				foreach (ISelectable s in ColumnIterator)
				{
					if (s.IsFormula)
					{
						return true;
					}
				}
				return false;
			}
		}

		public bool IsUnique
		{
			get { return unique; }
			set { unique = value; }
		}

		public virtual bool IsNullable
		{
			get
			{
				if (HasFormula)
				{
					return true;
				}

				bool nullable = true;
				foreach (Column col in ColumnIterator)
				{
					if (!col.IsNullable)
					{
						nullable = false;
					}
				}

				return nullable;
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

		public bool IsValid(IMapping mapping)
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
				typeName = ReflectHelper.ReflectedPropertyClass(className, propertyName, accesorName).FullName;
			}
		}

		#endregion

		public virtual void AddColumn(Column column)
		{
			if (!columns.Contains(column))
			{
				columns.Add(column);
			}
			column.Value = this;
			column.TypeIndex = columns.Count - 1;
		}

		public virtual void AddFormula(Formula formula)
		{
			columns.Add(formula);
		}

		public void SetTypeByReflection(System.Type propertyClass, string propertyName)
		{
			SetTypeByReflection(propertyClass, propertyName, "property");
		}

		public virtual void SetTypeByReflection(System.Type propertyClass, string propertyName, string propertyAccess)
		{
			try
			{
				if (type == null)
				{
					type = ReflectHelper.ReflectedPropertyType(propertyClass, propertyName, propertyAccess);
					int count = 0;
					foreach (ISelectable thing in ColumnIterator)
					{
						Column col = thing as Column;
						if (col != null)
						{
							col.Type = type;
							col.TypeIndex = count++;
						}
					}
				}
			}
			catch (HibernateException he)
			{
				throw new MappingException("Problem trying to set property type by reflection", he);
			}
		}

		public IIdentifierGenerator CreateIdentifierGenerator(Dialect.Dialect dialect)
		{
			if (uniqueIdentifierGenerator == null)
			{
				uniqueIdentifierGenerator =
					IdentifierGeneratorFactory.Create(identifierGeneratorStrategy, type, identifierGeneratorProperties, dialect);
			}

			return uniqueIdentifierGenerator;
		}

		public override string ToString()
		{
			return string.Format("{0}({1})", GetType().FullName, StringHelper.CollectionToString(columns));
		}
	}
}
