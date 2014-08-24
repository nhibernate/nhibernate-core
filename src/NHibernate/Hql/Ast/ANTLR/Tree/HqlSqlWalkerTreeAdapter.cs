using System;
using Antlr.Runtime;

namespace NHibernate.Hql.Ast.ANTLR.Tree
{
	[CLSCompliant(false)]
	public class HqlSqlWalkerTreeAdaptor : ASTTreeAdaptor
	{
		private readonly HqlSqlWalker _walker;

		public HqlSqlWalkerTreeAdaptor(object walker)
		{
			_walker = (HqlSqlWalker) walker;
		}

		public override object Create(IToken payload)
		{
			if (payload == null)
			{
				return base.Create(payload);
			}

			object ret;

			switch (payload.Type)
			{
				case HqlSqlWalker.SELECT:
				case HqlSqlWalker.QUERY:
					ret = new QueryNode(payload);
					break;
				case HqlSqlWalker.UPDATE:
					ret = new UpdateStatement(payload);
					break;
				case HqlSqlWalker.DELETE:
					ret = new DeleteStatement(payload);
					break;
				case HqlSqlWalker.INSERT:
					ret = new InsertStatement(payload);
					break;
				case HqlSqlWalker.INTO:
					ret = new IntoClause(payload);
					break;
				case HqlSqlWalker.FROM:
					ret = new FromClause(payload);
					break;
				case HqlSqlWalker.FROM_FRAGMENT:
					ret = new FromElement(payload);
					break;
				case HqlSqlWalker.IMPLIED_FROM:
					ret = new ImpliedFromElement(payload);
					break;
				case HqlSqlWalker.DOT:
					ret = new DotNode(payload);
					break;
				case HqlSqlWalker.INDEX_OP:
					ret = new IndexNode(payload);
					break;
					// Alias references and identifiers use the same node class.
				case HqlSqlWalker.ALIAS_REF:
				case HqlSqlWalker.IDENT:
					ret = new IdentNode(payload);
					break;
				case HqlSqlWalker.SQL_TOKEN:
					ret = new SqlFragment(payload);
					break;
				case HqlSqlWalker.METHOD_CALL:
					ret = new MethodNode(payload);
					break;
				case HqlSqlWalker.ELEMENTS:
				case HqlSqlWalker.INDICES:
					ret = new CollectionFunction(payload);
					break;
				case HqlSqlWalker.SELECT_CLAUSE:
					ret = new SelectClause(payload);
					break;
				case HqlSqlWalker.SELECT_EXPR:
					ret = new SelectExpressionImpl(payload);
					break;
				case HqlSqlWalker.AGGREGATE:
					ret = new AggregateNode(payload);
					break;
				case HqlSqlWalker.COUNT:
					ret = new CountNode(payload);
					break;
				case HqlSqlWalker.CONSTRUCTOR:
					ret = new ConstructorNode(payload);
					break;
				case HqlSqlWalker.NUM_INT:
				case HqlSqlWalker.NUM_FLOAT:
				case HqlSqlWalker.NUM_LONG:
				case HqlSqlWalker.NUM_DOUBLE:
                case HqlSqlWalker.NUM_DECIMAL:
                case HqlSqlWalker.QUOTED_String:
					ret = new LiteralNode(payload);
					break;
				case HqlSqlWalker.TRUE:
				case HqlSqlWalker.FALSE:
					ret = new BooleanLiteralNode(payload);
					break;
				case HqlSqlWalker.JAVA_CONSTANT:
					ret = new JavaConstantNode(payload);
					break;
				case HqlSqlWalker.ORDER:
					ret = new OrderByClause(payload);
					break;
				case HqlSqlWalker.PLUS:
				case HqlSqlWalker.MINUS:
				case HqlSqlWalker.STAR:
				case HqlSqlWalker.DIV:
				case HqlSqlWalker.BAND:
				case HqlSqlWalker.BOR:
				case HqlSqlWalker.BXOR:
					ret = new BinaryArithmeticOperatorNode(payload);
					break;
				case HqlSqlWalker.UNARY_MINUS:
				case HqlSqlWalker.UNARY_PLUS:
				case HqlSqlWalker.BNOT:
					ret = new UnaryArithmeticNode(payload);
					break;
				case HqlSqlWalker.CASE2:
					ret = new Case2Node(payload);
					break;
				case HqlSqlWalker.CASE:
					ret = new CaseNode(payload);
					break;
				case HqlSqlWalker.PARAM:
				case HqlSqlWalker.NAMED_PARAM:
					ret = new ParameterNode(payload);
					break;
				case HqlSqlWalker.EQ:
				case HqlSqlWalker.NE:
				case HqlSqlWalker.LT:
				case HqlSqlWalker.GT:
				case HqlSqlWalker.LE:
				case HqlSqlWalker.GE:
				case HqlSqlWalker.LIKE:
				case HqlSqlWalker.NOT_LIKE:
					ret = new BinaryLogicOperatorNode(payload);
					break;
				case HqlSqlWalker.IN:
				case HqlSqlWalker.NOT_IN:
					ret = new InLogicOperatorNode(payload);
					break;
				case HqlSqlWalker.BETWEEN:
				case HqlSqlWalker.NOT_BETWEEN:
					ret = new BetweenOperatorNode(payload);
					break;
				case HqlSqlWalker.IS_NULL:
					ret = new IsNullLogicOperatorNode(payload);
					break;
				case HqlSqlWalker.IS_NOT_NULL:
					ret = new IsNotNullLogicOperatorNode(payload);
					break;
				case HqlSqlWalker.EXISTS:
					ret = new UnaryLogicOperatorNode(payload);
					break;
				default:
					ret = new SqlNode(payload);
					break;
			}

			Initialise(ret);
			return ret;
		}

		public override object ErrorNode(ITokenStream input, IToken start, IToken stop, RecognitionException e)
		{
			return new ASTErrorNode(input, start, stop, e);
		}

		public override object DupNode(object t)
		{
			var node = t as IASTNode;

			if (node != null)
			{
				var dupped = (IASTNode) Create(node.Token);

				dupped.Parent = node.Parent;

				return dupped;
			}

			throw new NotImplementedException();
		}

		private void Initialise(object node)
		{
			var initableNode = node as IInitializableNode;

			if (initableNode != null)
			{
				initableNode.Initialize(_walker);
			}

			var sessionNode = node as ISessionFactoryAwareNode;

			if (sessionNode != null)
			{
				sessionNode.SessionFactory = _walker.SessionFactoryHelper.Factory;
			}
		}
	}
}