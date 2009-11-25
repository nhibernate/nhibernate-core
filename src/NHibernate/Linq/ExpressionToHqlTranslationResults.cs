using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Hql.Ast;

namespace NHibernate.Linq
{
    public class ExpressionToHqlTranslationResults
    {
        public HqlQuery Statement { get; private set; }
        public ResultTransformer ResultTransformer { get; private set; }
        public List<Action<IQuery, IDictionary<string, object>>> AdditionalCriteria { get; private set; }

        public ExpressionToHqlTranslationResults(HqlQuery statement, IList<LambdaExpression> itemTransformers, IList<LambdaExpression> listTransformers, List<Action<IQuery, IDictionary<string, object>>> additionalCriteria)
        {
            Statement = statement;

            var itemTransformer = MergeLambdas(itemTransformers);
            var listTransformer = MergeLambdas(listTransformers);

            if (itemTransformer != null || listTransformer != null)
            {
                 ResultTransformer = new ResultTransformer(itemTransformer, listTransformer);
            }

        	AdditionalCriteria = additionalCriteria;
        }

        private static LambdaExpression MergeLambdas(IList<LambdaExpression> transformations)
        {
            if (transformations == null || transformations.Count == 0)
            {
                return null;
            }

            var listTransformLambda = transformations[0];

            for (int i = 1; i < transformations.Count; i++)
            {
                var invoked = Expression.Invoke(transformations[i], listTransformLambda.Body);

                listTransformLambda = Expression.Lambda(invoked, listTransformLambda.Parameters.ToArray());
            }

            return listTransformLambda;
        }
    }
}