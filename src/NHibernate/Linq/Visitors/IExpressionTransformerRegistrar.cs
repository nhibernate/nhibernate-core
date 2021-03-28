using Remotion.Linq.Parsing.ExpressionVisitors.Transformation;

namespace NHibernate.Linq.Visitors
{
	/// <summary>
	/// Provides a way to register custom transformers for expressions.
	/// </summary>
	public interface IExpressionTransformerRegistrar
	{
		/// <summary>
		/// Registers additional transformers on the expression transformer registry.
		/// </summary>
		/// <param name="expressionTransformerRegistry">The expression transformer registry.</param>
		void Register(ExpressionTransformerRegistry expressionTransformerRegistry);
	}
}
