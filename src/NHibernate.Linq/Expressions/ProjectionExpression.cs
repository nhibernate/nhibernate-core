using System.Linq.Expressions;

namespace NHibernate.Linq.Expressions
{
	public class ProjectionExpression : NHExpression
	{
		public ProjectionExpression(SelectExpression source, Expression projector)
			: base(NHExpressionType.Projection, projector.Type)
		{
			Projector = projector;
			Source = source;
		}

		public Expression Projector { get; protected set; }
		public SelectExpression Source { get; protected set; }

		public override string ToString()
		{
			return string.Format("({0})", Projector);
		}
	}
}