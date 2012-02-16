using System.Linq.Expressions;
using Remotion.Linq.Clauses;


namespace NHibernate.Linq.Visitors
{
    /// <summary>
    /// All joins are created as outer joins. An optimization in <see cref="WhereJoinDetector"/> finds
    /// joins that may be inner joined and calls <see cref="MakeInner"/> on them.
    /// <see cref="QueryModelVisitor"/>'s <see cref="QueryModelVisitor.VisitAdditionalFromClause"/> will
    /// then emit the correct HQL join.
    /// </summary>
    public class NhJoinClause : AdditionalFromClause
    {
        public bool IsInner { get; private set; }
        public void MakeInner() {
            IsInner = true;
        }
        public NhJoinClause(string itemName, System.Type itemType, Expression fromExpression) : base(itemName, itemType, fromExpression) {
            IsInner = false;
        }
    }
}
