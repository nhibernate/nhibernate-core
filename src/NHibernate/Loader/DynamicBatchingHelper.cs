using System;
using System.Collections.Generic;
using NHibernate.Criterion;
using NHibernate.Engine;
using NHibernate.Param;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate.Loader
{
	internal class DynamicBatchingHelper
	{
		private static string BatchIdPlaceholder = "$$BATCH_ID_PLACEHOLDER$$";

		public static SqlStringBuilder BuildBatchFetchRestrictionFragment()
		{
			return new SqlStringBuilder(1).Add(BatchIdPlaceholder);
		}

		public static SqlString ExpandBatchIdPlaceholder(
			SqlString sqlString,
			ISet<IParameterSpecification> specifications,
			string[] columns,
			IType[] types,
			ISessionFactoryImplementor factory)
		{
			var parameters = GeneratePositionalParameters(specifications, types, factory);

			var wherePart = InExpression.GetSqlString(columns, types.Length, parameters, factory.Dialect);

			return sqlString.ReplaceLast(BatchIdPlaceholder, wherePart);
		}

		public static List<Parameter> GeneratePositionalParameters(
			ISet<IParameterSpecification> specifications,
			IType[] types,
			ISessionFactoryImplementor factory)
		{
			var parameters = new List<Parameter>();
			for (var i = 0; i < types.Length; i++)
			{
				var specification = new PositionalParameterSpecification(1, 0, i) { ExpectedType = types[i] };
				foreach (var id in specification.GetIdsForBackTrack(factory))
				{
					var p = Parameter.Placeholder;
					p.BackTrack = id;
					parameters.Add(p);
				}

				specifications.Add(specification);
			}

			return parameters;
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
