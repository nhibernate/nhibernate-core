using System;
using System.Collections;

namespace NHibernate.Loader.Custom
{
	/// <summary>
	/// Represents a return defined as part of a native sql query which
	/// names a fetched role.
	/// </summary>
	public class SQLQueryJoinReturn : SQLQueryReturn
	{
		private string ownerAlias;
		private string ownerProperty;

		public SQLQueryJoinReturn( string alias, string ownerAlias, string ownerProperty, IDictionary propertyResults, LockMode lockMode )
			: base( alias, propertyResults, lockMode )
		{
			this.ownerAlias = ownerAlias;
			this.ownerProperty = ownerProperty;
		}

		public string OwnerAlias
		{
			get { return ownerAlias; }
		}

		public string OwnerProperty
		{
			get { return ownerProperty; }
		}
	}
}
