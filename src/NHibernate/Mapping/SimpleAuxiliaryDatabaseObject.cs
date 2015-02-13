using System;
using System.Collections.Generic;
using NHibernate.DdlGen.Operations;
using NHibernate.Engine;
using NHibernate.Util;

namespace NHibernate.Mapping
{
	/// <summary> 
	/// A simple implementation of AbstractAuxiliaryDatabaseObject in which the CREATE and DROP strings are
	/// provided up front.
	/// </summary>
	/// <remarks>
	/// Contains simple facilities for templating the catalog and schema
	/// names into the provided strings.
	/// This is the form created when the mapping documents use &lt;create/&gt; and &lt;drop/&gt;.
	/// </remarks>
	[Serializable]
	public class SimpleAuxiliaryDatabaseObject : AbstractAuxiliaryDatabaseObject
	{
		private readonly string sqlCreateString;
		private readonly string sqlDropString;

		public SimpleAuxiliaryDatabaseObject(String sqlCreateString, String sqlDropString)
		{
			this.sqlCreateString = sqlCreateString;
			this.sqlDropString = sqlDropString;
		}

		public SimpleAuxiliaryDatabaseObject(String sqlCreateString, String sqlDropString, HashSet<string> dialectScopes)
			: base(dialectScopes)
		{
			this.sqlCreateString = sqlCreateString;
			this.sqlDropString = sqlDropString;
		}


        public override IDdlOperation GetCreateOperation(Dialect.Dialect dialect, IMapping mapping, string defaultCatalog,
            string defaultSchema)
        {
            var sql = InjectCatalogAndSchema(sqlCreateString, defaultCatalog, defaultSchema);
            return new SqlDdlOperation(sql);
        }

        public override IDdlOperation GetDropOperation(Dialect.Dialect dialect, IMapping mapping, string defaultCatalog,
            string defaultSchema)
        {
            var sql = InjectCatalogAndSchema(sqlDropString, defaultCatalog, defaultSchema);
            return new SqlDdlOperation(sql);
        }

		private static string InjectCatalogAndSchema(string ddlString, string defaultCatalog, string defaultSchema)
		{
			string rtn = StringHelper.Replace(ddlString, "${catalog}", defaultCatalog);
			rtn = StringHelper.Replace(rtn, "${schema}", defaultSchema);
			return rtn;
		}
	}
}