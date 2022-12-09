namespace NHibernate.Linq
{
	/// <summary>
	/// Defines a linq query expression.
	/// </summary>
	public interface ILinqQueryExpression: IQueryExpression, ICacheableQueryExpression
	{
		ExpressionToHqlTranslationResults ExpressionToHqlTranslationResults { get; }

		void CopyExpressionTranslation(NhLinqExpressionCache cache);
	}
}
