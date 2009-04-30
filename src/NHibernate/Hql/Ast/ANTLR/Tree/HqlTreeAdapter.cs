using System;
using Antlr.Runtime;
using Antlr.Runtime.Tree;

namespace NHibernate.Hql.Ast.ANTLR.Tree
{
	[CLSCompliant(false)]
	public class HqlTreeAdaptor : CommonTreeAdaptor
	{
		private readonly HqlSqlWalker _walker;

		public HqlTreeAdaptor(HqlSqlWalker walker)
		{
			_walker = walker;
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
					//return UpdateStatement.class;
					ret = new SqlNode(payload);
					break;
				case HqlSqlWalker.DELETE:
					//return DeleteStatement.class;
					ret = new SqlNode(payload);
					break;
				case HqlSqlWalker.INSERT:
					//return InsertStatement.class;
					ret = new SqlNode(payload);
					break;
				case HqlSqlWalker.INTO:
					//return IntoClause.class;
					ret = new SqlNode(payload);
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
					//return AggregateNode.class;
					ret = new SqlNode(payload);
					break;
				case HqlSqlWalker.COUNT:
					//return CountNode.class;
					ret = new SqlNode(payload);
					break;
				case HqlSqlWalker.CONSTRUCTOR:
					ret = new ConstructorNode(payload);
					break;
				case HqlSqlWalker.NUM_INT:
				case HqlSqlWalker.NUM_FLOAT:
				case HqlSqlWalker.NUM_LONG:
				case HqlSqlWalker.NUM_DOUBLE:
				case HqlSqlWalker.QUOTED_String:
					//return LiteralNode.class;
					ret = new SqlNode(payload);
					break;
				case HqlSqlWalker.TRUE:
				case HqlSqlWalker.FALSE:
					//return BooleanLiteralNode.class;
					ret = new SqlNode(payload);
					break;
				case HqlSqlWalker.JAVA_CONSTANT:
					//return JavaConstantNode.class;
					ret = new SqlNode(payload);
					break;
				case HqlSqlWalker.ORDER:
					ret = new OrderByClause(payload);
					break;
				case HqlSqlWalker.PLUS:
				case HqlSqlWalker.MINUS:
				case HqlSqlWalker.STAR:
				case HqlSqlWalker.DIV:
					//return BinaryArithmeticOperatorNode.class;
					ret = new SqlNode(payload);
					break;
				case HqlSqlWalker.UNARY_MINUS:
				case HqlSqlWalker.UNARY_PLUS:
					//return UnaryArithmeticNode.class;
					ret = new SqlNode(payload);
					break;
				case HqlSqlWalker.CASE2:
					//return Case2Node.class;
					ret = new SqlNode(payload);
					break;
				case HqlSqlWalker.CASE:
					//return CaseNode.class;
					ret = new SqlNode(payload);
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
					return new BinaryLogicOperatorNode(payload);
				case HqlSqlWalker.IN:
				case HqlSqlWalker.NOT_IN:
					//return InLogicOperatorNode.class;
					ret = new SqlNode(payload);
					break;
				case HqlSqlWalker.BETWEEN:
				case HqlSqlWalker.NOT_BETWEEN:
					//return BetweenOperatorNode.class;
					ret = new SqlNode(payload);
					break;
				case HqlSqlWalker.IS_NULL:
					//return IsNullLogicOperatorNode.class;
					ret = new SqlNode(payload);
					break;
				case HqlSqlWalker.IS_NOT_NULL:
					//return IsNotNullLogicOperatorNode.class;
					ret = new SqlNode(payload);
					break;
				case HqlSqlWalker.EXISTS:
					//return UnaryLogicOperatorNode.class;
					ret = new SqlNode(payload);
					break;
				default:
					ret = new SqlNode(payload);
					break;

			}

			Initialise(ret);
			return ret;
		}

		public override object DupNode(object t)
		{
			CommonTree node = t as CommonTree;

			if (node != null)
			{
				CommonTree dupped = (CommonTree)Create(node.Token);

				dupped.Parent = node.Parent;

				return dupped;
			}
			else
			{
				return base.DupNode(t);
			}
		}

		void Initialise(object node)
		{
			IInitializableNode initableNode = node as IInitializableNode;

			if (initableNode != null)
			{
				initableNode.Initialize(_walker);
			}
		}
	}
}
