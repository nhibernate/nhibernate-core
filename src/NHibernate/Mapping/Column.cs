using System;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Mapping
{
	/// <summary></summary>
	public class Column
	{
		private static readonly int DefaultPropertyLength = 255;

		private int length = DefaultPropertyLength;
		private IType type;
		private int typeIndex;
		private string name;
		private bool nullable = true;
		private bool unique = false;
		private string sqlType;
		private bool quoted = false;

		/// <summary></summary>
		internal int uniqueInteger;

		/// <summary></summary>
		public int Length
		{
			get { return length; }
			set { length = value; }
		}

		/// <summary></summary>
		public IType Type
		{
			get { return type; }
			set { type = value; }
		}

		/// <summary></summary>
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
		/// 
		/// </summary>
		/// <param name="d"></param>
		/// <returns></returns>
		public string GetQuotedName( Dialect.Dialect d )
		{
			return IsQuoted ?
				d.QuoteForColumnName( name ) :
				name;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="d"></param>
		/// <returns></returns>
		public string Alias( Dialect.Dialect d )
		{
			if( quoted )
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
		/// 
		/// </summary>
		/// <param name="d"></param>
		/// <param name="suffix"></param>
		/// <returns></returns>
		public string Alias( Dialect.Dialect d, string suffix )
		{
			if( quoted )
			{
				return "y" + uniqueInteger.ToString() + StringHelper.Underscore;
			}

			if( ( name.Length + suffix.Length ) < 11 )
			{
				return name + suffix;
			}
				//return name.Substring(0, name.Length - suffix.Length);
			else
			{
				return ( new Alias( 10, uniqueInteger.ToString() + StringHelper.Underscore + suffix ) ).ToAliasString( name, d );
			}

		}

		/// <summary></summary>
		public bool IsNullable
		{
			get { return nullable; }
			set { nullable = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="type"></param>
		/// <param name="typeIndex"></param>
		public Column( IType type, int typeIndex )
		{
			this.type = type;
			this.typeIndex = typeIndex;
		}

		/// <summary></summary>
		public int TypeIndex
		{
			get { return typeIndex; }
			set { typeIndex = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="mapping"></param>
		/// <returns></returns>
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

		/// <summary></summary>
		public bool IsUnique
		{
			get { return unique; }
			set { unique = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dialect"></param>
		/// <param name="mapping"></param>
		/// <returns></returns>
		public string GetSqlType( Dialect.Dialect dialect, IMapping mapping )
		{
			if( sqlType == null )
			{
				SqlType sqlTypeObject = GetAutoSqlType( mapping );
				return dialect.GetTypeName( sqlTypeObject, Length );
			}
			else
			{
				return sqlType;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals( object obj )
		{
			return obj is Column && Equals( ( Column ) obj );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="column"></param>
		/// <returns></returns>
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

		/// <summary></summary>
		public override int GetHashCode()
		{
			return name.GetHashCode();
		}

		/// <summary></summary>
		public string SqlType
		{
			get { return sqlType; }
			set { sqlType = value; }
		}

		/// <summary></summary>
		public bool IsQuoted
		{
			get { return quoted; }
			set { quoted = value; }
		}

	}
}