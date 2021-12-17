using System;
using NHibernate.Criterion;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.Util;

namespace NHibernate.Loader
{
	internal class DynamicBatchingHelper
	{
		private static string BatchIdPlaceholder = "$$BATCH_ID_PLACEHOLDER$$";

		public static SqlStringBuilder BuildBatchFetchRestrictionFragment()
		{
			return new SqlStringBuilder(1).Add(BatchIdPlaceholder);
		}

		public static SqlString ExpandBatchIdPlaceholder(SqlString sqlString, QueryParameters queryParameters, string[] columns, Dialect.Dialect dialect, out Parameter[] parameters)
		{
			var bogusParam = Parameter.Placeholder;
			var wherePart = InExpression.GetSqlString(columns, queryParameters.PositionalParameterValues.Length, dialect, bogusParam);
			parameters = wherePart.Parameters.ToArray(x => Parameter.Placeholder);
			wherePart.SubstituteBogusParameters(parameters, bogusParam);

			return sqlString.ReplaceLast(BatchIdPlaceholder, wherePart);
		}

		public static int GetIdsToLoad(object[] batch, out object[] idsToLoad)
		{
			int numberOfIds = Array.IndexOf(batch, null);
			if (numberOfIds < 0)
			{
				idsToLoad = batch;
				return batch.Length;
			}

			if (numberOfIds == 1)
			{
				idsToLoad = null;
				return numberOfIds;
			}

			idsToLoad = new object[numberOfIds];
			Array.Copy(batch, 0, idsToLoad, 0, numberOfIds);
			return numberOfIds;
		}
	}
}
