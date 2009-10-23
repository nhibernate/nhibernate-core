using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NHibernate.Linq
{
	public class NhQueryProvider : IQueryProvider
	{
		private readonly ISession _session;

		public NhQueryProvider(ISession session)
		{
			_session = session;
		}

		public object Execute(Expression expression)
		{
			var nhLinqExpression = new NhLinqExpression(expression);

			var query = _session.CreateQuery(nhLinqExpression);

			SetParameters(query, nhLinqExpression.ParameterValuesByName);

			var results = query.List();

			if (nhLinqExpression.ReturnType == NhLinqExpressionReturnType.Sequence)
			{
				return results.AsQueryable();
			}

			return results[0];
		}

		public TResult Execute<TResult>(Expression expression)
		{
			return (TResult) Execute(expression);
		}

		public IQueryable CreateQuery(Expression expression)
		{
			throw new NotImplementedException();
		}

		public IQueryable<T> CreateQuery<T>(Expression expression)
		{
			return new NhQueryable<T>(this, expression);
		}

		void SetParameters(IQuery query, IDictionary<string, object> parameters)
		{
			foreach (var parameterName in query.NamedParameters)
			{
				query.SetParameter(parameterName, parameters[parameterName]);
			}
		}
	}
}