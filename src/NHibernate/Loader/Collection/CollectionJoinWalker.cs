using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.Util;

namespace NHibernate.Loader.Collection
{
	/// <summary>
	/// Superclass of walkers for collection initializers
	/// <seealso cref="CollectionLoader" />
	/// <seealso cref="OneToManyJoinWalker" />
	/// <seealso cref="BasicCollectionJoinWalker" />
	/// </summary>
	public abstract class CollectionJoinWalker : JoinWalker
	{
		public CollectionJoinWalker(ISessionFactoryImplementor factory, IDictionary<string, IFilter> enabledFilters)
			: base(factory, enabledFilters) { }

		protected SqlStringBuilder WhereString(string alias, string[] columnNames, SqlString subselect, int batchSize)
		{
			if (subselect == null)
			{
				return WhereString(alias, columnNames, batchSize);
			}
			else
			{
				string columnsJoin =
					string.Join(StringHelper.CommaSpace, columnNames.ToArray(column => StringHelper.Qualify(GenerateAliasForColumn(alias, column), column)));

				SqlStringBuilder buf = new SqlStringBuilder();
				if (columnNames.Length > 1)
				{
					buf.Add("(").Add(columnsJoin).Add(")");
				}
				else
				{
					buf.Add(columnsJoin);
				}

				buf.Add(" in ").Add("(").Add(subselect).Add(")");
				return buf;
			}
		}
	}
}
