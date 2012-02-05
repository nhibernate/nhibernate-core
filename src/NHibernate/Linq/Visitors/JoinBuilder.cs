using System.Collections.Generic;
using System.Linq.Expressions;
using Remotion.Linq;
using Remotion.Linq.Clauses.Expressions;

namespace NHibernate.Linq.Visitors
{
	public interface IJoiner
	{
		Expression AddJoin(Expression expression, string key);
		void MakeInnerIfJoined(string key);
	}

	public class Joiner : IJoiner
	{
		private readonly NameGenerator _nameGenerator;
		private readonly QueryModel _queryModel;
		private readonly Dictionary<string, NhJoinClause> _joins = new Dictionary<string, NhJoinClause>();

		internal Joiner(QueryModel queryModel)
		{
			_nameGenerator = new NameGenerator(queryModel);
			_queryModel = queryModel;
		}

		public Expression AddJoin(Expression expression, string key)
		{
			NhJoinClause join;

			if (!_joins.TryGetValue(key, out join))
			{
				join = new NhJoinClause(_nameGenerator.GetNewName(), expression.Type, expression);
				_queryModel.BodyClauses.Add(join);
				_joins.Add(key, join);
			}

			return new QuerySourceReferenceExpression(@join);
		}

		public void MakeInnerIfJoined(string key)
		{
			// key is not joined if it occurs only at tails of expressions, e.g.
			// a.B == null, a.B != null, a.B == c.D etc.
			if (_joins.ContainsKey(key))
			{
				_joins[key].MakeInner();
			}
		}

		public IEnumerable<NhJoinClause> Joins
		{
			get { return _joins.Values; }
		}
	}
}
