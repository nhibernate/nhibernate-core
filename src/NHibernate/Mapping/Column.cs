using System;
using System.Data;
using NHibernate.Dialect;
using NHibernate.Engine;
using NHibernate.Type;
using NHibernate.Util;
using NHibernate.Sql;
using NHibernate.SqlTypes;

namespace NHibernate.Mapping 
{
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
		internal int uniqueInteger;

		public int Length 
		{
			get { return length; }
			set { length = value; }
		}

		public IType Type 
		{
			get { return type; }
			set { type = value; }
		}

		public string Name 
		{
			get { return name; }
			set 
			{ 
				if (value[0] == '`') 
				{
					quoted = true;
					name = value.Substring(1, value.Length - 2);
				}
				else 
				{
					name = value; 
				}
			}
		}

		public string GetQuotedName(Dialect.Dialect d) 
		{
			return d.QuoteForColumnName(name);
		}

		public string Alias 
		{
			get 
			{	
				if(quoted) 
					return "y" + uniqueInteger.ToString() + StringHelper.Underscore;
				 
				if ( name.Length < 11 )
					return name;
				else
					return (new Alias(10, uniqueInteger.ToString() + StringHelper.Underscore)).ToAliasString(name);
			}
		}

		public bool IsNullable 
		{ 
			get { return nullable; }
			set { nullable = value; }
		}

		public Column(IType type, int typeIndex) 
		{
			this.type = type;
			this.typeIndex = typeIndex;
		}

		public int TypeIndex {
			get { return typeIndex; }
			set { typeIndex = value; }
		}

		public SqlType GetAutoSqlType(IMapping mapping) 
		{
			try 
			{
				return Type.SqlTypes(mapping)[TypeIndex];
			}
			catch(Exception e) 
			{
				throw new MappingException(
					"GetAutoSqlType - Could not determine type for column " + 
					name + 
					" of type " + 
					type.GetType().FullName + 
					": " + 
					e.GetType().FullName, e);
			}
		}

		public bool IsUnique 
		{
			get { return unique; }
			set { unique = value; }
		}

		public string GetSqlType(Dialect.Dialect dialect, IMapping mapping) 
		{		
			if(sqlType==null) 
			{
				SqlType sqlTypeObject = GetAutoSqlType(mapping);
//				if(sqlTypeObject!=null) {
					return dialect.SqlTypeToString(sqlTypeObject);
//				}
//				else {
//					returnString = dialect.GetTypeName( GetAutoDbType(mapping), Length );
//				}
			}
			else 
			{
				return sqlType;
			}
		}

		public override bool Equals(object obj) 
		{
			return obj is Column && Equals( (Column) obj );
		}

		public bool Equals(Column column) 
		{
			if (null == column) return false;
			if (this == column) return true;

			return name.Equals(column.Name);
		}

		public override int GetHashCode() 
		{
			return name.GetHashCode();
		}

		public string SqlType 
		{
			get { return sqlType; }
			set { sqlType = value; }
		}

		public bool IsQuoted 
		{
			get { return quoted; }
			set { quoted = value; }
		}

	}
}
