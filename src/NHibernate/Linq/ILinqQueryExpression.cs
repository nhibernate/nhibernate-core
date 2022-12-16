using System.Collections.Generic;
using NHibernate.Type;

namespace NHibernate.Linq
{
	/// <summary>
	/// Defines a linq query expression.
	/// </summary>
	public interface ILinqQueryExpression: IQueryExpression
	{
		ExpressionToHqlTranslationResults ExpressionToHqlTranslationResults { get; }

		IDictionary<string, System.Tuple<IType, bool>> GetNamedParameterTypes();

		void CopyExpressionTranslation(NhLinqExpressionCache cache);
	}
}
