using System;
using System.Collections;
using System.Text;

using NHibernate.Sql;
using NHibernate.Util;

namespace NHibernate.SqlCommand 
{

	/// <summary>
	/// Represents an SQL <c>for update of ... nowait</c> statement
	/// </summary>
	public class ForUpdateFragment 
	{

		private StringBuilder aliases = new StringBuilder();
		private bool nowait;

		public ForUpdateFragment() 
		{
		}

		public bool NoWait 
		{
			get { return nowait;}
			set { nowait = value;}
		}

		public ForUpdateFragment AddTableAlias(string alias) 
		{
			if(aliases.Length > 0 ) aliases.Append(StringHelper.CommaSpace);
			aliases.Append(alias);
			return this;
		}

		public SqlString ToSqlStringFragment() 
		{
			return aliases.Length==0 ?
				new SqlString("") :
				new SqlString(" for update of " + aliases + ( nowait ? " nowait" : "")); 
		}

	}
}


