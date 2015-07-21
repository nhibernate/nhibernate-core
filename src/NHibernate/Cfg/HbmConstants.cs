using System;

namespace NHibernate.Cfg
{
	public class HbmConstants
	{
		// Make consts of all of these to avoid interning the strings at run-time
		public const string nsPrefix = "hbm";
		public const string nsKey = nsPrefix + ":key";
		public const string nsColumn = nsPrefix + ":column";
		public const string nsFormula = nsPrefix + ":formula";
		public const string nsOneToMany = nsPrefix + ":one-to-many";
		public const string nsParam = nsPrefix + ":param";
		public const string nsIndex = nsPrefix + ":index";
		public const string nsListIndex = nsPrefix + ":list-index";
		public const string nsGenerator = nsPrefix + ":generator";
		public const string nsType = nsPrefix + ":type";
		public const string nsCollectionId = nsPrefix + ":collection-id";
		public const string nsClass = nsPrefix + ":class";
		public const string nsSubclass = nsPrefix + ":subclass";
		public const string nsJoinedSubclass = nsPrefix + ":joined-subclass";
		public const string nsQuery = nsPrefix + ":query";
		public const string nsSqlQuery = nsPrefix + ":sql-query";
		public const string nsSynchronize = nsPrefix + ":synchronize";
		public const string nsImport = nsPrefix + ":import";
		public const string nsMeta = nsPrefix + ":meta";
		public const string nsMetaValue = nsPrefix + ":meta-value";
		public const string nsQueryParam = nsPrefix + ":query-param";
		public const string nsReturnDiscriminator = nsPrefix + ":return-discriminator";
		public const string nsReturnProperty = nsPrefix + ":return-property";
		public const string nsReturnColumn = nsPrefix + ":return-column";

		public const string nsLoader = nsPrefix + ":loader";
		public const string nsSqlInsert = nsPrefix + ":sql-insert";
		public const string nsSqlUpdate = nsPrefix + ":sql-update";
		public const string nsSqlDelete = nsPrefix + ":sql-delete";
		public const string nsSqlDeleteAll = nsPrefix + ":sql-delete-all";

		public const string nsFilterDef = nsPrefix + ":filter-def";
		public const string nsFilterParam = nsPrefix + ":filter-param";
		public const string nsFilter = nsPrefix + ":filter";

		public const string nsDatabaseObject = nsPrefix + ":database-object";
		public const string nsDefinition = nsPrefix + ":definition";
		public const string nsCreate = nsPrefix + ":create";
		public const string nsDrop = nsPrefix + ":drop";
		public const string nsDialectScope = nsPrefix + ":dialect-scope";

		public const string nsResultset = nsPrefix + ":resultset";

		public const string nsUnionSubclass = nsPrefix + ":union-subclass";
		public const string nsTuplizer = nsPrefix + ":tuplizer";
	}
}
