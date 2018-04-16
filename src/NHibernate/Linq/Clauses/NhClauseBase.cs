using System;
using Remotion.Linq;

namespace NHibernate.Linq.Clauses
{
	public abstract class NhClauseBase
	{
		/// <summary>
		///     Accepts the specified visitor.
		/// </summary>
		/// <param name="visitor">The visitor to accept.</param>
		/// <param name="queryModel">The query model in whose context this clause is visited.</param>
		/// <param name="index">
		///     The index of this clause in the <paramref name="queryModel" />'s
		///     <see cref="P:Remotion.Linq.QueryModel.BodyClauses" /> collection.
		/// </param>
		public void Accept(IQueryModelVisitor visitor, QueryModel queryModel, int index)
		{
			if (visitor == null) throw new ArgumentNullException(nameof(visitor));
			if (queryModel == null) throw new ArgumentNullException(nameof(queryModel));
			if (!(visitor is INhQueryModelVisitor nhVisitor))
				throw new ArgumentException("Expect visitor to implement INhQueryModelVisitor", nameof(visitor));

			Accept(nhVisitor, queryModel, index);
		}

		protected abstract void Accept(INhQueryModelVisitor visitor, QueryModel queryModel, int index);
	}
}
