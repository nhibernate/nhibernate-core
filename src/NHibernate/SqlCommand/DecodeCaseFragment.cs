using System;
using System.Collections;
using System.Text;

using NHibernate.Sql;
using NHibernate.Util;

namespace NHibernate.SqlCommand
{
	/// <summary>
	/// Represents an SQL decode(pkvalue, key1, 1, key2, 2, ..., 0)
	/// </summary>
	public class DecodeCaseFragment : CaseFragment 
	{
		private string returnColumnName;
		private IList cases = new ArrayList();

		public override CaseFragment SetReturnColumnName(string returnColumnName) 
		{
			this.returnColumnName = returnColumnName;
			return this;
		}

		public override CaseFragment SetReturnColumnName(string returnColumnName, string suffix) 
		{
			return SetReturnColumnName( new Alias(suffix).ToAliasString(returnColumnName) );
		}

		public override CaseFragment AddWhenColumnNotNull(string alias, string columnName, string columnValue) 
		{
			string key = alias + StringHelper.Dot + columnName;

			if(columnValue.Equals("0")) 
			{
				cases.Insert(0, key);
			}
			else 
			{
				cases.Add(", " + key + ", " + columnValue);
			}
			
			return this;
		}

		public override SqlString ToSqlStringFragment() 
		{
			
			StringBuilder buf = new StringBuilder( cases.Count * 15 + 10 );

			buf.Append("decode (");

			for(int i = 0; i < cases.Count; i++) 
			{
				buf.Append(cases[i]);
			}

			buf.Append(",0 )");

			if(returnColumnName!=null) 
			{
				buf.Append(" as ")
					.Append(returnColumnName);
			}

			return new SqlString(buf.ToString());
		}
	}
}
