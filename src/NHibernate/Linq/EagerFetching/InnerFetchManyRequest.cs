using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.EagerFetching;
using Remotion.Linq.Utilities;

namespace NHibernate.Linq.EagerFetching
{
    public class InnerFetchManyRequest : FetchRequestBase
    {
        private readonly System.Type _relatedObjectType;
        public InnerFetchManyRequest(System.Reflection.MemberInfo relationMember)
            : base(ArgumentUtility.CheckNotNull<System.Reflection.MemberInfo>("relationMember", relationMember))
        {
            System.Type memberReturnType = ReflectionUtility.GetMemberReturnType(relationMember);
            this._relatedObjectType = ReflectionUtility.GetItemTypeOfIEnumerable(memberReturnType, "relationMember");
        }
        protected override void ModifyFetchQueryModel(QueryModel fetchQueryModel)
        {
            ArgumentUtility.CheckNotNull<QueryModel>("fetchQueryModel", fetchQueryModel);
            System.Linq.Expressions.MemberExpression fromExpression = System.Linq.Expressions.Expression.MakeMemberAccess(new QuerySourceReferenceExpression(fetchQueryModel.MainFromClause), base.RelationMember);
            AdditionalFromClause additionalFromClause = new AdditionalFromClause(fetchQueryModel.GetNewName("#fetch"), this._relatedObjectType, fromExpression);
            fetchQueryModel.BodyClauses.Add(additionalFromClause);
            QuerySourceReferenceExpression selector = new QuerySourceReferenceExpression(additionalFromClause);
            SelectClause selectClause = new SelectClause(selector);
            fetchQueryModel.SelectClause = selectClause;
        }
        public override ResultOperatorBase Clone(CloneContext cloneContext)
        {
            ArgumentUtility.CheckNotNull<CloneContext>("cloneContext", cloneContext);
            InnerFetchManyRequest fetchManyRequest = new InnerFetchManyRequest(base.RelationMember);
            foreach (FetchRequestBase current in base.InnerFetchRequests)
            {
                fetchManyRequest.GetOrAddInnerFetchRequest((FetchRequestBase)current.Clone(cloneContext));
            }
            return fetchManyRequest;
        }
        public override void TransformExpressions(System.Func<System.Linq.Expressions.Expression, System.Linq.Expressions.Expression> transformation)
        {
        }
    }
}
