using System;
using System.Collections;

namespace NHibernate.Loader.Custom
{
	/// <summary>
	/// Represents a return defined as part of a native sql query which
	/// names a "root" entity.  A root entity means it is explicitly a
	/// "column" in the result, as opposed to a fetched relationship or role.
	/// </summary>
	public class SQLQueryRootReturn : SQLQueryNonScalarReturn
	{
		private string returnEntityName;

		public SQLQueryRootReturn( string alias, string returnEntityName, LockMode lockMode )
			: this( alias, returnEntityName, null, lockMode )
		{
		}

		public SQLQueryRootReturn( string alias, string entityName, IDictionary propertyResults, LockMode lockMode )
			: base( alias, propertyResults, lockMode )
		{
			this.returnEntityName = entityName;
		}

		public string ReturnEntityName
		{
			get { return returnEntityName; }
		}
	}
}
