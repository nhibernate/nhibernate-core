using System;
using System.Collections;
using NHibernate.Engine;
using NHibernate.Id;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Mapping
{
	/// <summary>
	/// Any value that maps to columns.
	/// </summary>
	public class SimpleValue : IKeyValue
	{
		private ArrayList columns = new ArrayList();
		private IType type;
		private IDictionary identifierGeneratorProperties;
		private string identifierGeneratorStrategy = "assigned";
		private string nullValue;
		private Table table;
		private string foreignKeyName;
		private bool unique;
		private IIdentifierGenerator uniqueIdentifierGenerator;

		public SimpleValue( )
		{
		}

		public SimpleValue( Table table )
		{
			this.table = table;
		}

		public virtual void AddColumn( Column column )
		{
			if( !columns.Contains( column ) )
			{
				columns.Add( column );
			}

			// TODO H3:			
//			column.Value = this;
//			column.TypeIndex = columns.Count - 1;
		}

		public virtual void AddFormula( Formula formula )
		{
			columns.Add( formula );
		}

		public virtual int ColumnSpan
		{
			get { return columns.Count; }
		}

		public virtual ICollection ColumnCollection
		{
			get { return columns; }
		}

		public virtual IList ConstraintColumns
		{
			get { return columns; }
		}

		public virtual IType Type
		{
			get { return type; }
			set
			{
				this.type = value;
				int count = 0;

				foreach( ISelectable sel in ColumnCollection )
				{
					if (sel is Column)
					{
						Column col = (Column) sel;
						col.Type = type;
						col.TypeIndex = count++;
					}
				}
			}
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

		public virtual void CreateForeignKey()
		{
		}

		public void CreateForeignKeyOfClass( System.Type persistentClass )
		{
			table.CreateForeignKey( ForeignKeyName, ConstraintColumns, persistentClass );
		}

		public IIdentifierGenerator CreateIdentifierGenerator( Dialect.Dialect dialect )
		{
			if( uniqueIdentifierGenerator == null )
			{
				uniqueIdentifierGenerator = IdentifierGeneratorFactory.Create( identifierGeneratorStrategy, type, identifierGeneratorProperties, dialect );
			}

			return uniqueIdentifierGenerator;
		}

		public void SetTypeByReflection( System.Type propertyClass, string propertyName )
		{
			SetTypeByReflection( propertyClass, propertyName, "property" );
		}

		public virtual void SetTypeByReflection( System.Type propertyClass, string propertyName, string propertyAccess )
		{
			try
			{
				if( type == null )
				{
					type = ReflectHelper.ReflectedPropertyType( propertyClass, propertyName, propertyAccess );
					int count = 0;
					foreach( ISelectable thing in ColumnCollection )
					{
						Column col = thing as Column;
						if( col != null )
						{
							col.Type = type;
							col.TypeIndex = count++;
						}
					}
				}
			}
			catch( HibernateException he )
			{
				throw new MappingException( "Problem trying to set property type by reflection", he );
			}
		}

		public virtual FetchMode FetchMode
		{
			get { return FetchMode.Select; }
			// Needed so that subclasses have something to override
			set { throw new NotSupportedException(); }
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

		public bool IsSimpleValue
		{
			get { return true; }
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
				if( HasFormula )
				{
					return true;
				}

				bool nullable = true;
				foreach( Column col in ColumnCollection )
				{
					if ( !col.IsNullable )
					{
						nullable = false;
					}
				}

				return nullable;
			}
		}

		public string NullValue
		{
			get { return nullValue; }
			set { nullValue = value; }
		}

		public bool IsValid( IMapping mapping )
		{
			return ColumnSpan == Type.GetColumnSpan( mapping );
		}

		public virtual bool[] ColumnInsertability
		{
			get
			{
				bool[] result = new bool[ ColumnSpan ];
				int i = 0;
				foreach( ISelectable s in ColumnCollection )
				{
					result[ i++ ] = !s.IsFormula;
				}
				return result;
			}
		}

		public virtual bool[] ColumnUpdateability
		{
			get { return ColumnInsertability; }
		}

		public bool HasFormula
		{
			get
			{
				foreach( ISelectable s in ColumnCollection )
				{
					if( s.IsFormula )
					{
						return true;
					}
				}
				return false;
			}
		}
	}
}
