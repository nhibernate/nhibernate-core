using System;
using System.Collections;

namespace NHibernate.Loader.Custom
{
	/// <summary>
	/// Represents the base information for a return defined as part of
	/// a native sql query.
	/// </summary>
	public abstract class SQLQueryNonScalarReturn : ISQLQueryReturn
	{
		private string alias;
		private LockMode lockMode;
		protected IDictionary propertyResults = new Hashtable();

		protected SQLQueryNonScalarReturn(string alias, IDictionary propertyResults, LockMode lockMode)
		{
			this.alias = alias;
			if (alias == null)
			{
				throw new HibernateException("alias must be specified");
			}
			this.lockMode = lockMode;
			if (propertyResults != null)
			{
				this.propertyResults = propertyResults;
			}
		}

		public string Alias
		{
			get { return alias; }
		}

		public LockMode LockMode
		{
			get { return lockMode; }
		}

		public IDictionary PropertyResultsMap
		{
			get { return propertyResults; }
		}
	}
}