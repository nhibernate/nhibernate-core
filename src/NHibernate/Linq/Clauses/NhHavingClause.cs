using System;
using System.Linq.Expressions;
using Remotion.Linq;
using Remotion.Linq.Clauses;

namespace NHibernate.Linq.Clauses
{
	public class NhHavingClause : NhClauseBase, IBodyClause
	{
		Expression _predicate;

		/// <summary>
		///     Initializes a new instance of the <see cref="T:NhHavingClause" /> class.
		/// </summary>
		/// <param name="predicate">The predicate used to filter data items.</param>
		public NhHavingClause(Expression predicate)
		{
			if (predicate == null) throw new ArgumentNullException(nameof(predicate));
			_predicate = predicate;
		}

		/// <summary>
		///     Gets the predicate, the expression representing the where condition by which the data items are filtered
		/// </summary>
		public Expression Predicate
		{
			get { return _predicate; }
			set
			{
				if (value == null) throw new ArgumentNullException(nameof(value));
				_predicate = value;
			}
		}

		protected override void Accept(INhQueryModelVisitor visitor, QueryModel queryModel, int index)
		{
			visitor.VisitNhHavingClause(this, queryModel, index);
		}

		/// <inheritdoc />
		IBodyClause IBodyClause.Clone(CloneContext cloneContext)
		{
			return Clone(cloneContext);
		}

		/// <summary>
		///     Transforms all the expressions in this clause and its child objects via the given
		///     <paramref name="transformation" /> delegate.
		/// </summary>
		/// <param name="transformation">
		///     The transformation object. This delegate is called for each <see cref="T:System.Linq.Expressions.Expression" />
		///     within this
		///     clause, and those expressions will be replaced with what the delegate returns.
		/// </param>
		public void TransformExpressions(Func<Expression, Expression> transformation)
		{
			if (transformation == null) throw new ArgumentNullException(nameof(transformation));
			Predicate = transformation(Predicate);
		}

		public override string ToString()
		{
			return "having " + Predicate;
		}

		/// <summary>Clones this clause.</summary>
		/// <param name="cloneContext">
		///     The clones of all query source clauses are registered with this
		///     <see cref="T:Remotion.Linq.Clauses.CloneContext" />.
		/// </param>
		/// <returns></returns>
		public NhHavingClause Clone(CloneContext cloneContext)
		{
			if (cloneContext == null) throw new ArgumentNullException(nameof(cloneContext));
			return new NhHavingClause(Predicate);
		}
	}
}
