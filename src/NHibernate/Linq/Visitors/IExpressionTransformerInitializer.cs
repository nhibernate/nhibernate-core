using Remotion.Linq.Parsing.ExpressionVisitors.Transformation;

namespace NHibernate.Linq.Visitors
{
	/// <summary>
	/// Provides a way to register custom transformers for expressions.
	/// </summary>
	public interface IExpressionTransformerInitializer
	{
		/// <summary>
		/// Initialize expression transformer registry by registering additional transformers.
		/// </summary>
		/// <param name="expressionTransformerRegistry">The expression transformer registry.</param>
		void Initialize(ExpressionTransformerRegistry expressionTransformerRegistry);
	}
}
