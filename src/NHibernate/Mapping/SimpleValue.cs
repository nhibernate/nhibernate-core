using System;
using System.Collections;
using NHibernate.Engine;
using NHibernate.Id;
using NHibernate.Loader;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Mapping
{
	/// <summary>
	/// Any value that maps to columns.
	/// </summary>
	public class SimpleValue : IValue
	{
		private ArrayList columns = new ArrayList();
		private IType type;
		private IDictionary identifierGeneratorProperties;
		private string identifierGeneratorStrategy = "assigned";
		private string nullValue;
		private Table table;
		private Formula formula;
		private string foreignKeyName;
		private bool unique;
		private IIdentifierGenerator uniqueIdentifierGenerator;

		/// <summary>
		/// 
		/// </summary>
		public SimpleValue( )
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="table"></param>
		public SimpleValue( Table table )
		{
			this.table = table;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="column"></param>
		public virtual void AddColumn( Column column )
		{
			if( !columns.Contains( column ) )
			{
				columns.Add( column );
			}
		}

		/// <summary></summary>
		public virtual int ColumnSpan
		{
			get { return columns.Count; }
		}

		/// <summary></summary>
		public virtual ICollection ColumnCollection
		{
			get { return columns; }
		}

		/// <summary></summary>
		public virtual IList ConstraintColumns
		{
			get { return columns; }
		}

		/// <summary></summary>
		public virtual IType Type
		{
			get { return type; }
			set
			{
				this.type = value;
				int count = 0;

				foreach( Column col in ColumnCollection )
				{
					col.Type = type;
					col.TypeIndex = count++;
				}
			}
		}

		/// <summary></summary>
		public string ForeignKeyName
		{
			get { return foreignKeyName; }
			set { foreignKeyName = value; }
		}

		/// <summary></summary>
		public Table Table
		{
			get { return table; }
			set { table = value; }
		}

		/// <summary></summary>
		public virtual void CreateForeignKey()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="persistentClass"></param>
		public void CreateForeignKeyOfClass( System.Type persistentClass )
		{
			table.CreateForeignKey( ForeignKeyName, ConstraintColumns, persistentClass );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dialect"></param>
		/// <returns></returns>
		public IIdentifierGenerator CreateIdentifierGenerator( Dialect.Dialect dialect )
		{
			if( uniqueIdentifierGenerator == null )
			{
				uniqueIdentifierGenerator = IdentifierGeneratorFactory.Create( identifierGeneratorStrategy, type, identifierGeneratorProperties, dialect );
			}

			return uniqueIdentifierGenerator;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="propertyClass"></param>
		/// <param name="propertyName"></param>
		public void SetTypeByReflection( System.Type propertyClass, string propertyName )
		{
			SetTypeByReflection( propertyClass, propertyName, "property" );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="propertyClass"></param>
		/// <param name="propertyName"></param>
		/// <param name="propertyAccess"></param>
		public virtual void SetTypeByReflection( System.Type propertyClass, string propertyName, string propertyAccess )
		{
			try
			{
				if( type == null )
				{
					type = ReflectHelper.ReflectedPropertyType( propertyClass, propertyName, propertyAccess );
					int count = 0;
					foreach( Column col in ColumnCollection )
					{
						col.Type = type;
						col.TypeIndex = count++;
					}
				}
			}
			catch( HibernateException he )
			{
				throw new MappingException( "Problem trying to set property type by reflection", he );
			}
		}

		/// <summary></summary>
		public virtual OuterJoinFetchStrategy OuterJoinFetchSetting
		{
			get { return OuterJoinFetchStrategy.Lazy; }
			set { throw new NotSupportedException(); }
		}

		/// <summary></summary>
		public IDictionary IdentifierGeneratorProperties
		{
			get { return identifierGeneratorProperties; }
			set { identifierGeneratorProperties = value; }
		}

		/// <summary></summary>
		public string IdentifierGeneratorStrategy
		{
			get { return identifierGeneratorStrategy; }
			set { identifierGeneratorStrategy = value; }
		}

		/// <summary></summary>
		public virtual bool IsComposite
		{
			get { return false; }
		}

		/// <summary></summary>
		public bool IsSimpleValue
		{
			get { return true; }
		}

		/// <summary></summary>
		public bool IsUnique
		{
			get { return unique; }
			set { unique = value; }
		}

		/// <summary></summary>
		public virtual bool IsNullable
		{
			get 
			{
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

		/// <summary></summary>
		public string NullValue
		{
			get { return nullValue; }
			set { nullValue = value; }
		}

		/// <summary></summary>
		public Formula Formula
		{
			get { return formula; }
			set { formula = value; }
		}

		/// <summary></summary>
		public bool IsValid( IMapping mapping )
		{
			return ColumnSpan == Type.GetColumnSpan( mapping );
		}
	}
}
