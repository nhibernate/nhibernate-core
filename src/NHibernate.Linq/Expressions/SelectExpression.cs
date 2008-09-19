using System.Linq.Expressions;

namespace NHibernate.Linq.Expressions
{
	/// <summary>
	/// A select expression is executed against the database to retrieve data. 
	/// </summary>
	public class SelectExpression : NHExpression
	{
		public SelectExpression(System.Type type, string alias, Expression projection, Expression from, Expression where)
			: base(NHExpressionType.Select, type)
		{
			Where = where;
			From = from;
			FromAlias = alias;
			Projection = projection;
		}

		public Expression Projection { get; protected set; }
		public string FromAlias { get; protected set; }
		public Expression Where { get; protected set; }
		public Expression From { get; protected set; }
	}
}