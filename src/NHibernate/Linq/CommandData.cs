using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Hql.Ast;

namespace NHibernate.Linq
{
    public class CommandData
    {
        private readonly List<LambdaExpression> _itemTransformers;
        private readonly List<LambdaExpression> _listTransformers;

        public CommandData(HqlQuery statement, NamedParameter[] namedParameters, List<LambdaExpression> itemTransformers, List<LambdaExpression> listTransformers, List<Action<IQuery>> additionalCriteria)
        {
            _itemTransformers = itemTransformers;
            _listTransformers = listTransformers;

            Statement = statement;
            NamedParameters = namedParameters;
            AdditionalCriteria = additionalCriteria;
        }

        public HqlQuery Statement { get; private set; }
        public NamedParameter[] NamedParameters { get; private set; }

        public List<Action<IQuery>> AdditionalCriteria { get; set; }

        public System.Type QueryResultType { get; set; }

        public IQuery CreateQuery(ISession session, System.Type type)
        {
            var query = session.CreateQuery(new HqlExpression(Statement, type));

            SetParameters(query);

            SetResultTransformer(query);

            AddAdditionalCriteria(query);

            return query;
        }

        private void SetParameters(IQuery query)
        {
            foreach (var parameter in NamedParameters)
            {
                query.SetParameter(parameter.Name, parameter.Value);
            }
        }

        private void AddAdditionalCriteria(IQuery query)
        {
            foreach (var criteria in AdditionalCriteria)
            {
                criteria(query);
            }
        }

        private void SetResultTransformer(IQuery query)
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