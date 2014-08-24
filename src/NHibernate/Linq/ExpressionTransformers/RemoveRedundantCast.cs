using System.Linq.Expressions;
using Remotion.Linq.Parsing.ExpressionTreeVisitors.Transformation;

namespace NHibernate.Linq.ExpressionTransformers
{
	/// <summary>
	/// Remove redundant casts to the same type or to superclass (upcast) in <see cref="ExpressionType.Convert"/>, <see cref=" ExpressionType.ConvertChecked"/> 
	/// and <see cref="ExpressionType.TypeAs"/> <see cref="UnaryExpression"/>s  
	/// </summary>
	public class RemoveRedundantCast : IExpressionTransformer<UnaryExpression>
	{
		private static readonly ExpressionType[] _supportedExpressionTypes = new[]
			{
				ExpressionType.TypeAs,
				ExpressionType.Convert,
				ExpressionType.ConvertChecked,
			};

		public Expression Transform(UnaryExpression expression)
		{
			if (expression.Type != typeof(object) &&
				expression.Type.IsAssignableFrom(expression.Operand.Type) &&
				expression.Method == null &&
				!expression.IsLiftedToNull)
			{
				return expression.Operand;
			}

			return expression;
		}

		public ExpressionType[] SupportedExpressionTypes
		{
			get { return _supportedExpressionTypes; }
		}
	}
}