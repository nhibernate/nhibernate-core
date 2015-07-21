using NHibernate.Param;

namespace NHibernate.Linq.Visitors.ResultOperatorProcessors
{
    public class ProcessCacheable : IResultOperatorProcessor<CacheableResultOperator>
    {
        public void Process(CacheableResultOperator resultOperator, QueryModelVisitor queryModelVisitor, IntermediateHqlTree tree)
        {
            NamedParameter parameterName;

            switch (resultOperator.ParseInfo.ParsedExpression.Method.Name)
            {
                case "Cacheable":
                    tree.AddAdditionalCriteria((q, p) => q.SetCacheable(true));
                    break;
                case "CacheMode":
                    queryModelVisitor.VisitorParameters.ConstantToParameterMap.TryGetValue(resultOperator.Data,
                                                                                           out parameterName);
                    if (parameterName != null)
                    {
                        tree.AddAdditionalCriteria((q, p) => q.SetCacheMode((CacheMode) p[parameterName.Name].Item1));
                    }
                    else
                    {
                        tree.AddAdditionalCriteria((q, p) => q.SetCacheMode((CacheMode) resultOperator.Data.Value));
                    }
                    break;
                case "CacheRegion":
                    queryModelVisitor.VisitorParameters.ConstantToParameterMap.TryGetValue(resultOperator.Data,
                                                                                           out parameterName);
                    if (parameterName != null)
                    {
                        tree.AddAdditionalCriteria((q, p) => q.SetCacheRegion((string) p[parameterName.Name].Item1));
                    }
                    else
                    {
                        tree.AddAdditionalCriteria((q, p) => q.SetCacheRegion((string) resultOperator.Data.Value));
                    }
                    break;
            }
        }
    }
}