using System;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Mapping
{
	/// <summary>
	/// Represents the mapping to a column in a database.
	/// </summary>
	public class Column
	{
		private static readonly int DefaultPropertyLength = -1;

		private int length = DefaultPropertyLength;
		private IType type;
		private int typeIndex = 0;
		private string name;
		private bool nullable = true;
		private bool unique = false;
		private string sqlType;
		private bool quoted = false;
		private string checkConstraint;

		/// <summary></summary>
		internal int uniqueInteger;

		/// <summary>
		/// Gets or sets the length of the datatype in the database.
		/// </summary>
		/// <value>The length of the datatype in the database.</value>
		public int Length
		{
			get { return length; }
			set { length = value; }
		}

		/// <summary>
		/// Gets or sets the NHibernate <see cref="IType"/> of the column.
		/// </summary>
		/// <value>
		/// The NHibernate <see cref="IType"/> of the column.
		/// </value>
		public IType Type
		{
			get { return type; }
			set { type = value; }
		}

		/// <summary>
		/// Gets or sets the name of the column in the database.
		/// </summary>
		/// <value>
		/// The name of the column in the database.  The get does 
		/// not return a Quoted column name.
		/// </value>
		/// <remarks>
		/// <p>
		/// If a value is passed in that is wrapped by <c>`</c> then 
		/// NHibernate will Quote the column whenever SQL is generated
		/// for it.  How the column is quoted depends on the Dialect.
		/// </p>
		/// <p>
		/// The value returned by the getter is not Quoted.  To get the
		/// column name in quoted form use <see cref="GetQuotedName(Dialect.Dialect)"/>.
		/// </p>
		/// </remarks>
		public string Name
		{
			get { return name; }
			set
			{
				if( value[ 0 ] == '`' )
				{
					quoted = true;
					name = value.Substring( 1, value.Length - 2 );
				}
				else
				{
					name = value;
				}
			}
		}

		/// <summary>
		/// Gets the name of this Column in quoted form if it is necessary.
		/// </summary>
		/// <param name="d">
		/// The <see cref="Dialect.Dialect"/> that knows how to quote
		/// the column name.
		/// </param>
		/// <returns>
		/// The column name in a form that is safe to use inside of a SQL statement.
		/// Quoted if it needs to be, not quoted if it does not need to be.
		/// </returns>
		public string GetQuotedName( Dialect.Dialect d )
		{
			return IsQuoted ?
				d.QuoteForColumnName( name ) :
				name;
		}

		/// <summary>
		/// Gets an Alias for the column name.
		/// </summary>
		/// <param name="d">The <see cref="Dialect.Dialect"/> that contains the rules for Aliasing.</param>
		/// <returns>
		/// A string that can be used as the alias for this Column.
		/// </returns>
		public string Alias( Dialect.Dialect d )
		{
			if( quoted || name[0] == StringHelper.SingleQuote || char.IsDigit( name, 0 ) )
			{
				return "y" + uniqueInteger.ToString() + StringHelper.Underscore;
			}

			if( name.Length < 11 )
			{
				return name;
			}
			else
			{
				return ( new Alias( 10, uniqueInteger.ToString() + StringHelper.Underscore ) ).ToAliasString( name, d );
			}
		}

		/// <summary>
		/// Gets an Alias for the column name.
		/// </summary>
		/// <param name="d">The <see cref="Dialect.Dialect"/> that contains the rules for Aliasing.</param>
		/// <param name="suffix">A string to use as the suffix for the Alias.</param>
		/// <returns>
		/// A string that can be used as the alias for this Column.
		/// </returns>
		public string Alias( Dialect.Dialect d, string suffix )
		{
			if( quoted || name[0] == StringHelper.SingleQuote || char.IsDigit( name, 0 ) )
			{
				return "y" + uniqueInteger.ToString() + StringHelper.Underscore + suffix;
			}

			if( ( name.Length + suffix.Length ) < 11 )
			{
				return name + suffix;
			}
			else
			{
				return ( new Alias( 10, uniqueInteger.ToString() + StringHelper.Underscore + suffix ) ).ToAliasString( name, d );
			}

		}

		/// <summary>
		/// Gets or sets if the column can have null values in it.
		/// </summary>
		/// <value><c>true</c> if the column can have a null value in it.</value>
		public bool IsNullable
		{
			get { return nullable; }
			set { nullable = value; }
		}

		/// <summary>
		/// Initializes a new instance of <see cref="Column"/>.
		/// </summary>
		/// <param name="type">The NHibernate <see cref="IType"/> that reads from and writes to the column.</param>
		/// <param name="typeIndex">The index of the column in the <see cref="IType"/>.</param>
		public Column( IType type, int typeIndex )
		{
			this.type = type;
			this.typeIndex = typeIndex;
		}

		/// <summary>
		/// Gets or sets the index of the column in the <see cref="IType"/>.
		/// </summary>
		/// <value>
		/// The index of the column in the <see cref="IType"/>.
		/// </value>
		public int TypeIndex
		{
			get { return typeIndex; }
			set { typeIndex = value; }
		}

		/// <summary>
		/// Gets the <see cref="SqlType"/> of the column based on the <see cref="IType"/>.
		/// </summary>
		/// <param name="mapping"></param>
		/// <returns>
		/// The <see cref="SqlType"/> of the column based on the <see cref="IType"/>.
		/// </returns>
		public SqlType GetAutoSqlType( IMapping mapping )
		{
			try
			{
				return Type.SqlTypes( mapping )[ TypeIndex ];
			}
			catch( Exception e )
			{
				throw new MappingException(
					"GetAutoSqlType - Could not determine type for column " +
						name +
						" of type " +
						type.GetType().FullName +
						": " +
						e.GetType().FullName, e );
			}
		}

		/// <summary>
		/// Gets or sets if the column contains unique values.
		/// </summary>
		/// <value><c>true</c> if the column contains unique values.</value>
		public bool IsUnique
		{
			get { return unique; }
			set { unique = value; }
		}

		/// <summary>
		/// Gets the name of the data type for the column.
		/// </summary>
		/// <param name="dialect">The <see cref="Dialect.Dialect"/> to use to get the valid data types.</param>
		/// <param name="mapping"></param>
		/// <returns>
		/// The name of the data type for the column. 
		/// </returns>
		/// <remarks>
		/// If the mapping file contains a value of the attribute <c>sql-type</c> this will
		/// return the string contained in that attribute.  Otherwise it will use the 
		/// typename from the <see cref="Dialect.Dialect"/> of the <see cref="SqlType"/> object. 
		/// </remarks>
		public string GetSqlType( Dialect.Dialect dialect, IMapping mapping )
		{
			if( sqlType == null )
			{
				SqlType sqlTypeObject = GetAutoSqlType( mapping );
				if( Length != DefaultPropertyLength )
				{
					return dialect.GetTypeName( sqlTypeObject, Length );
				}
				else
				{
					return dialect.GetTypeName( sqlTypeObject );
				}
			}
			else
			{
				return sqlType;
			}
		}

		#region System.Object Members

		/// <summary>
		/// Determines if this instance of <see cref="Column"/> and a specified object, 
		/// which must be a <b>Column</b> can be considered the same.
		/// </summary>
		/// <param name="obj">An <see cref="Object"/> that should be a <see cref="Column"/>.</param>
		/// <returns>
		/// <c>true</c> if the name of this Column and the other Column are the same, 
		/// otherwise <c>false</c>.
		/// </returns>
		public override bool Equals( object obj )
		{
			Column columnObj = obj as Column;
			return columnObj!=null && Equals( columnObj );
		}

		/// <summary>
		/// Determines if this instance of <see cref="Column"/> and the specified Column 
		/// can be considered the same.
		/// </summary>
		/// <param name="column">A <see cref="Column"/> to compare to this Column.</param>
		/// <returns>
		/// <c>true</c> if the name of this Column and the other Column are the same, 
		/// otherwise <c>false</c>.
		/// </returns>
		public bool Equals( Column column )
		{
			if( null == column )
			{
				return false;
			}
			if( this == column )
			{
				return true;
			}

			return name.Equals( column.Name );
		}

		/// <summary>
		/// Returns the hash code for this instance.
		/// </summary>
		/// <value>The value of Name.GetHashCode().</value>
		public override int GetHashCode()
		{
			return name.GetHashCode();
		}

		#endregion

		/// <summary>
		/// Gets or sets the sql data type name of the column.
		/// </summary>
		/// <value>
		/// The sql data type name of the column. 
		/// </value>
		/// <remarks>
		/// This is usually read from the <c>sql-type</c> attribute.
		/// </remarks>
		public string SqlType
		{
			get { return sqlType; }
			set { sqlType = value; }
		}

		/// <summary>
		/// Gets or sets if the column needs to be quoted in SQL statements.
		/// </summary>
		/// <value><c>true</c> if the column is quoted.</value>
		public bool IsQuoted
		{
			get { return quoted; }
			set { quoted = value; }
		}

		/// <summary>
		/// Gets or sets whether the column is unique.
		/// </summary>
		public bool Unique
		{
			get { return unique; }
			set { unique = value; }
		}

		/// <summary>
		/// Gets or sets a check constraint on the column
		/// </summary>
		public string CheckConstraint
		{
			get { return checkConstraint; }
			set { checkConstraint = value; }
		}

		/// <summary>
		/// Do we have a check constraint?
		/// </summary>
		public bool HasCheckConstraint
		{
			get { return checkConstraint != null && checkConstraint.Length > 0; }
		}
	}
}