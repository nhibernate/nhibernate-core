using System;
using Iesi.Collections;
using NHibernate.Engine;
using NHibernate.Util;

namespace NHibernate.Mapping
{
	public class SimpleAuxiliaryDatabaseObject : AbstractAuxiliaryDatabaseObject
	{
		private readonly string sqlCreateString;
		private readonly string sqlDropString;

		public SimpleAuxiliaryDatabaseObject(String sqlCreateString, String sqlDropString)
		{
			this.sqlCreateString = sqlCreateString;
			this.sqlDropString = sqlDropString;
		}

		public SimpleAuxiliaryDatabaseObject(String sqlCreateString, String sqlDropString, HashedSet dialectScopes)
			: base(dialectScopes)
		{
			this.sqlCreateString = sqlCreateString;
			this.sqlDropString = sqlDropString;
		}

		public override string SqlCreateString(
				Dialect.Dialect dialect,
				IMapping p,
				string defaultSchema)
		{
			return InjectCatalogAndSchema(sqlCreateString, defaultSchema);
		}

		public override string SqlDropString(Dialect.Dialect dialect, string defaultSchema)
		{
			return InjectCatalogAndSchema(sqlDropString, defaultSchema);
		}

		private string InjectCatalogAndSchema(string ddlString, string defaultSchema)
		{
			string rtn = ddlString;
			rtn = StringHelper.Replace(rtn, "${schema}", defaultSchema);
			return rtn;
		}
	}
}
