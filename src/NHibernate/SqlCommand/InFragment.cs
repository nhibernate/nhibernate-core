using System;
using System.Collections;

using NHibernate.Util;

namespace NHibernate.SqlCommand
{
	/// <summary>
	/// Represents an <c>... in (...)</c> expression
	/// </summary>
	public class InFragment
	{
		private string columnName;
		private ArrayList values = new ArrayList();
		
		public InFragment AddValue(string value) 
		{
			values.Add(value);
			return this;

		}

		public InFragment SetColumn(string columnName) 
		{
			this.columnName = columnName;
			return this;
		}

		public InFragment SetColumn(string alias, string columnName) 
		{
			this.columnName = alias + StringHelper.Dot + columnName;
			return SetColumn( this.columnName );
		}

		public SqlString ToFragmentString() 
		{
			SqlStringBuilder buf = new SqlStringBuilder(values.Count + 5);
			buf.Add(columnName);
			if (values.Count > 1) 
			{
				bool allowNull = false;
				buf.Add(" in (");
				for(int i=0; i<values.Count; i++) 
				{
					if("null".Equals(values[i])) 
					{
						allowNull = true;
					}
					
					else 
					{
						buf.Add( (string)values[i] );
						if ( i<values.Count-1) buf.Add(StringHelper.CommaSpace);
					}
				}

				buf.Add(StringHelper.ClosedParen);
				if(allowNull) 
				{
					
					buf.Insert(0, " is null or ")
						.Insert(0, columnName)
						.Insert(0, StringHelper.OpenParen)
						.Add(StringHelper.ClosedParen);
				}
			} 
			else 
			{
				string value = values[0] as string;
				if ( "null".Equals(value) ) 
				{
					buf.Add(" is null");
				} 
				else 
				{
					buf.Add( "=" + values[0] );
				}
			}
			return buf.ToSqlString();
		}				
	}
}
