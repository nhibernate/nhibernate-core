using System;
using System.Collections;
using System.Text;

using NHibernate.Sql;
using NHibernate.Util;


namespace NHibernate.SqlCommand
{
	/// <summary>
	/// Represents an SQL <c>case when ... then ... end as ...</c>
	/// </summary>
	/// <remarks>This class looks StringHelper.SqlParameter safe...</remarks>
	public class ANSICaseFragment : CaseFragment
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
			return SetReturnColumnName( new Alias(suffix).ToAliasString( returnColumnName ) );
		}

		public override CaseFragment AddWhenColumnNotNull(string alias, string columnName, string columnValue) 
		{
			string key = alias + StringHelper.Dot + columnName + " is not null";
			
			cases.Add(" when " + key + " then " + columnValue );
			return this;
		}

		public override SqlString ToSqlStringFragment()
		{
			
			StringBuilder buf = new StringBuilder( cases.Count * 15 + 10 );

			buf.Append("case");

			for(int i = 0 ; i < cases.Count; i++) 
			{
				buf.Append(cases[i]);
			}

			buf.Append(" end");

			if( returnColumnName != null ) 
			{
				buf.Append(" as ")
					.Append(returnColumnName);
			}

			return new SqlString(buf.ToString());
		}
	}
}
