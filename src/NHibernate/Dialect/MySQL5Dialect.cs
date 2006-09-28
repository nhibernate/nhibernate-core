using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;
using NHibernate.Util;
using Environment = NHibernate.Cfg.Environment;

namespace NHibernate.Dialect
{
	class MySQL5Dialect : MySQLDialect
	{
		public override bool SupportsVariableLimit
		{
			get
			{
				return false;
			}
		}

		public override NHibernate.SqlCommand.SqlString GetLimitString(NHibernate.SqlCommand.SqlString querySqlString, int offset, int limit)
		{
			SqlStringBuilder pagingBuilder = new SqlStringBuilder();

			pagingBuilder.Add(querySqlString);
			pagingBuilder.Add(" limit ");
			if (offset > 0)
			{
				pagingBuilder.Add(offset.ToString());
				pagingBuilder.Add(", ");
			}

			pagingBuilder.Add(limit.ToString());


			return pagingBuilder.ToSqlString();

		}
	}
}
