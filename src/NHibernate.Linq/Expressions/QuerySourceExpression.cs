using NHibernate.Persister.Entity;
using IQueryable=System.Linq.IQueryable;

namespace NHibernate.Linq.Expressions
{
	/// <summary>
	/// A QuerySource expression is where we want to execute our query on, in RDBMS case it is a physical Table in general.
	/// May also be used in from clause of a select expression
	/// </summary>
	public class QuerySourceExpression : NHExpression
	{
		public QuerySourceExpression(System.Type type, IOuterJoinLoadable entityPersister)
			: base(NHExpressionType.QuerySource, type)
		{
			this.Persister = entityPersister;
		}
		public IEntityPersister Persister { get; protected set; }

		public override string ToString()
		{
			return string.Format("({0})", Persister.EntityName);
		}
	}
}