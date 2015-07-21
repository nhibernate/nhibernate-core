using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NHibernate.Linq.Visitors;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Collections;

namespace NHibernate.Linq.Clauses
{
	/// <summary>
	/// All joins are created as outer joins. An optimization in <see cref="WhereJoinDetector"/> finds
	/// joins that may be inner joined and calls <see cref="MakeInner"/> on them.
	/// <see cref="QueryModelVisitor"/>'s <see cref="QueryModelVisitor.VisitAdditionalFromClause"/> will
	/// then emit the correct HQL join.
	/// </summary>
	public class NhJoinClause : AdditionalFromClause
	{
		public NhJoinClause(string itemName, System.Type itemType, Expression fromExpression)
			: this(itemName, itemType, fromExpression, new NhWithClause[0])
		{
		}

		public NhJoinClause(string itemName, System.Type itemType, Expression fromExpression, IEnumerable<NhWithClause> restrictions)
			: base(itemName, itemType, fromExpression)
		{
			Restrictions = new ObservableCollection<NhWithClause>();
			foreach (var withClause in restrictions)
				Restrictions.Add(withClause);
			IsInner = false;
		}

		public ObservableCollection<NhWithClause> Restrictions { get; private set; }

		public bool IsInner { get; private set; }

		public override AdditionalFromClause Clone(CloneContext cloneContext)
		{
			var joinClause = new NhJoinClause(ItemName, ItemType, FromExpression);
			foreach (var withClause in Restrictions)
			{
				var withClause2 = new NhWithClause(withClause.Predicate);
				joinClause.Restrictions.Add(withClause2);
			}

			cloneContext.QuerySourceMapping.AddMapping(this, new QuerySourceReferenceExpression(joinClause));
			return base.Clone(cloneContext);
		}

		public void MakeInner()
		{
			IsInner = true;
		}

		public override void TransformExpressions(Func<Expression, Expression> transformation)
		{
			foreach (var withClause in Restrictions)
				withClause.TransformExpressions(transformation);
			base.TransformExpressions(transformation);
		}
	}
}