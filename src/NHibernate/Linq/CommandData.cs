using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NHibernate.Hql.Ast;

namespace NHibernate.Linq
{
    public class CommandData
    {
        public CommandData(HqlQuery statement, NamedParameter[] namedParameters, LambdaExpression projectionExpression, List<Action<IQuery>> additionalCriteria)
        {
            Statement = statement;
            NamedParameters = namedParameters;
            ProjectionExpression = projectionExpression;
            AdditionalCriteria = additionalCriteria;
        }

        public HqlQuery Statement { get; private set; }
        public NamedParameter[] NamedParameters { get; private set; }
        public LambdaExpression ProjectionExpression { get; set; }
        public List<Action<IQuery>> AdditionalCriteria { get; set; }

        public IQuery CreateQuery(ISession session, System.Type type)
        {
            var query = session.CreateQuery(new HqlExpression(Statement, type));

            foreach (var parameter in NamedParameters)
                query.SetParameter(parameter.Name, parameter.Value);

            if (ProjectionExpression != null)
            {
                query.SetResultTransformer(new ResultTransformer(ProjectionExpression));
            }

            foreach (var criteria in AdditionalCriteria)
            {
                criteria(query);
            }

            return query;
        }
    }
}