using System;
using System.Text;
using System.Collections;
using NHibernate.Util;
using NHibernate.Dialect;

namespace NHibernate.Mapping 
{
	
	public class UniqueKey : Constraint 
	{
		
		public string SqlConstraintString(Dialect.Dialect d) 
		{
			StringBuilder buf = new StringBuilder(" unique (");
			bool commaNeeded = false;
			
			foreach(Column col in ColumnCollection) 
			{
				if(commaNeeded) buf.Append( StringHelper.CommaSpace );
				commaNeeded = true;
				
				buf.Append( col.GetQuotedName(d) );
				
			}
			
			return buf.Append(StringHelper.ClosedParen).ToString();
		}

		public override string SqlConstraintString(Dialect.Dialect d, string constraintName) 
		{
			StringBuilder buf = new StringBuilder(
				d.GetAddPrimaryKeyConstraintString(constraintName))
				.Append('(');
			
			bool commaNeeded = false;

			foreach(Column col in ColumnCollection) 
			{
				if(commaNeeded) buf.Append( StringHelper.CommaSpace );
				commaNeeded = true;
				
				buf.Append( col.GetQuotedName(d) );
				
			}
			
			return StringHelper.Replace( buf.Append(StringHelper.ClosedParen).ToString(), "primary key", "unique" );
		}
	}
}
