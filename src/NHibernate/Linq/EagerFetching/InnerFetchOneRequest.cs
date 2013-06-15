using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.EagerFetching;
using Remotion.Linq.Utilities;

namespace NHibernate.Linq.EagerFetching
{
    public class InnerFetchOneRequest : FetchRequestBase
    {
        public InnerFetchOneRequest(System.Reflection.MemberInfo relationMember)
            : base(ArgumentUtility.CheckNotNull<System.Reflection.MemberInfo>("relationMember", relationMember))
        {
        }
        protected override void ModifyFetchQueryModel(QueryModel fetchQueryModel)
        {
            ArgumentUtility.CheckNotNull<QueryModel>("fetchQueryModel", fetchQueryModel);
            fetchQueryModel.SelectClause.Selector = System.Linq.Expressions.Expression.MakeMemberAccess(fetchQueryModel.SelectClause.Selector, base.RelationMember);
        }
        public override ResultOperatorBase Clone(CloneContext cloneContext)
        {
            ArgumentUtility.CheckNotNull<CloneContext>("cloneContext", cloneContext);
            InnerFetchOneRequest fetchOneRequest = new InnerFetchOneRequest(base.RelationMember);
            foreach (FetchRequestBase current in base.InnerFetchRequests)
            {
                fetchOneRequest.GetOrAddInnerFetchRequest((FetchRequestBase)current.Clone(cloneContext));
            }
            return fetchOneRequest;
        }
        public override void TransformExpressions(System.Func<System.Linq.Expressions.Expression, System.Linq.Expressions.Expression> transformation)
        {
        }
    }
}
