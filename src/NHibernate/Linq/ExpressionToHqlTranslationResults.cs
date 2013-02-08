using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Hql.Ast;
using NHibernate.Type;

namespace NHibernate.Linq
{
    public class ExpressionToHqlTranslationResults
    {
        public HqlTreeNode Statement { get; private set; }
        public ResultTransformer ResultTransformer { get; private set; }
        public Delegate PostExecuteTransformer { get; private set; }
        public List<Action<IQuery, IDictionary<string, Tuple<object, IType>>>> AdditionalCriteria { get; private set; }

        public ExpressionToHqlTranslationResults(HqlTreeNode statement, 
            IList<LambdaExpression> itemTransformers, 
            IList<LambdaExpression> listTransformers,
            IList<LambdaExpression> postExecuteTransformers,
            List<Action<IQuery, IDictionary<string, Tuple<object, IType>>>> additionalCriteria)
        {
            Statement = statement;

            PostExecuteTransformer = MergeLambdasAndCompile(postExecuteTransformers);

            var itemTransformer = (Func<object[], object>) MergeLambdasAndCompile(itemTransformers);
            var listTransformer = (Func<IEnumerable<object>, object>) MergeLambdasAndCompile(listTransformers);

            if (itemTransformer != null || listTransformer != null)
            {
                 ResultTransformer = new ResultTransformer(itemTransformer, listTransformer);
            }

        	AdditionalCriteria = additionalCriteria;
        }

        private static Delegate MergeLambdasAndCompile(IList<LambdaExpression> transformations)
        {
            if (transformations == null || transformations.Count == 0)
            {
                return null;
            }

            var listTransformLambda = transformations[0];

            for (int i = 1; i < transformations.Count; i++)
            {
                var invoked = Expression.Invoke(transformations[i], listTransformLambda.Body);

                listTransformLambda = Expression.Lambda(invoked, listTransformLambda.Parameters);
            }

            var isCovariant = typeof(Func<object, object>).IsAssignableFrom(listTransformLambda.Type);
            if (!isCovariant)
            {
                listTransformLambda = Expression.Lambda(Expression.Convert(listTransformLambda.Body, typeof(object)), listTransformLambda.Parameters);
            }

            return listTransformLambda.Compile();
        }
    }
}