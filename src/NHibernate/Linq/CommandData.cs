using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Hql.Ast;

namespace NHibernate.Linq
{
    public class CommandData
    {
        private readonly NamedParameter[] _namedParameters;
        private readonly List<LambdaExpression> _itemTransformers;
        private readonly List<LambdaExpression> _listTransformers;
        private readonly List<Action<IQuery>> _additionalCriteria;

        public CommandData(HqlQuery statement, NamedParameter[] namedParameters, List<LambdaExpression> itemTransformers, List<LambdaExpression> listTransformers, List<Action<IQuery>> additionalCriteria)
        {
            _itemTransformers = itemTransformers;
            _listTransformers = listTransformers;

            Statement = statement;
            _namedParameters = namedParameters;
            _additionalCriteria = additionalCriteria;
        }

        public HqlQuery Statement { get; private set; }

        public void SetParameters(IQuery query)
        {
            foreach (var parameter in _namedParameters)
            {
                query.SetParameter(parameter.Name, parameter.Value);
            }
        }

        public void AddAdditionalCriteria(IQuery query)
        {
            foreach (var criteria in _additionalCriteria)
            {
                criteria(query);
            }
        }

        public void SetResultTransformer(IQuery query)
        {
            var itemTransformer = MergeLambdas(_itemTransformers);
            var listTransformer = MergeLambdas(_listTransformers);

            if (itemTransformer != null || listTransformer != null)
            {
                query.SetResultTransformer(new ResultTransformer(itemTransformer, listTransformer));
            }
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