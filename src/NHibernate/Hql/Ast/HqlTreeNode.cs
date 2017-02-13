using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Hql.Ast.ANTLR;
using NHibernate.Hql.Ast.ANTLR.Tree;
using NHibernate.Util;

namespace NHibernate.Hql.Ast
{
	public class HqlTreeNode
	{
		public IASTFactory Factory { get; private set; }
		private readonly IASTNode _node;
		private readonly List<HqlTreeNode> _children;

		protected HqlTreeNode(int type, string text, IASTFactory factory, IEnumerable<HqlTreeNode> children)
		{
			Factory = factory;
			_node = factory.CreateNode(type, text);
			_children = new List<HqlTreeNode>();

			AddChildren(children);
		}

		protected HqlTreeNode(int type, string text, IASTFactory factory, params HqlTreeNode[] children) : this(type, text, factory, (IEnumerable<HqlTreeNode>)children)
		{
		}

		private void AddChildren(IEnumerable<HqlTreeNode> children)
		{
			foreach (var child in children)
			{
				if (child != null)
				{
					AddChild(child);
				}
			}
		}

		public IEnumerable<HqlTreeNode> NodesPreOrder
		{
			get
			{
				yield return this;

				foreach (var child in _children)
				{
					foreach (var node in child.NodesPreOrder)
					{
						yield return node;
					}
				}
			}
		}

		public IEnumerable<HqlTreeNode> NodesPostOrder
		{
			get
			{
				foreach (var child in _children)
				{
					foreach (var node in child.NodesPostOrder)
					{
						yield return node;
					}
				}

				yield return this;
			}
		}

		public IEnumerable<HqlTreeNode> Children
		{
			get { return _children; }
		}

		public void ClearChildren()
		{
			_children.Clear();
			_node.ClearChildren();
		}

		protected void SetText(string text)
		{
			_node.Text = text;
		}

		internal IASTNode AstNode
		{
			get { return _node; }
		}

		internal void AddChild(HqlTreeNode child)
		{
			if (child is HqlExpressionSubTreeHolder)
			{
				AddChildren(child.Children);
			}
			else
			{
				_children.Add(child);
				_node.AddChild(child.AstNode);
			}
		}
	}

	public static class HqlTreeNodeExtensions
	{
		public static HqlExpression AsExpression(this HqlTreeNode node)
		{
			// TODO - nice error handling if cast fails
			return (HqlExpression)node;
		}

		public static HqlBooleanExpression AsBooleanExpression(this HqlTreeNode node)
		{
			if (node is HqlDot)
			{
				return new HqlBooleanDot(node.Factory, (HqlDot)node);
			}

			// TODO - nice error handling if cast fails
			return (HqlBooleanExpression)node;
		}
	}

	public abstract class HqlStatement : HqlTreeNode
	{
		protected HqlStatement(int type, string text, IASTFactory factory, params HqlTreeNode[] children)
			: base(type, text, factory, children)
		{
		}

		protected HqlStatement(int type, string text, IASTFactory factory, IEnumerable<HqlTreeNode> children)
			: base(type, text, factory, children)
		{
		}
	}

	public abstract class HqlExpression : HqlTreeNode
	{
		protected HqlExpression(int type, string text, IASTFactory factory, IEnumerable<HqlTreeNode> children)
			: base(type, text, factory, children)
		{
		}

		protected HqlExpression(int type, string text, IASTFactory factory, params HqlTreeNode[] children)
			: base(type, text, factory, children)
		{
		}
	}

	public abstract class HqlBooleanExpression : HqlExpression
	{
		protected HqlBooleanExpression(int type, string text, IASTFactory factory, IEnumerable<HqlTreeNode> children)
			: base(type, text, factory, children)
		{
		}

		protected HqlBooleanExpression(int type, string text, IASTFactory factory, params HqlTreeNode[] children)
			: base(type, text, factory, children)
		{
		}
	}

	public class HqlQuery : HqlExpression
	{
		internal HqlQuery(IASTFactory factory, params HqlStatement[] children)
			: base(HqlSqlWalker.QUERY, "query", factory, children)
		{
		}
	}

	public class HqlIdent : HqlExpression
	{
		internal HqlIdent(IASTFactory factory, string ident)
			: base(HqlSqlWalker.IDENT, ident, factory)
		{
		}

		internal HqlIdent(IASTFactory factory, System.Type type)
			: base(HqlSqlWalker.IDENT, "", factory)
		{
			type = type.UnwrapIfNullable();

			switch (System.Type.GetTypeCode(type))
			{
				case TypeCode.Boolean:
					SetText("bool");
					break;
				case TypeCode.Int16:
					SetText("short");
					break;
				case TypeCode.Int32:
					SetText("integer");
					break;
				case TypeCode.Int64:
					SetText("long");
					break;
				case TypeCode.Decimal:
					SetText("decimal");
					break;
				case TypeCode.Single:
					SetText("single");
					break;
				case TypeCode.DateTime:
					SetText("datetime");
					break;
				case TypeCode.String:
					SetText("string");
					break;
				case TypeCode.Double:
					SetText("double");
					break;
				default:
					if (type == typeof(Guid))
					{
						SetText("guid");
						break;
					}
					if (type == typeof(DateTimeOffset))
					{
						SetText("datetimeoffset");
						break;
					}
					throw new NotSupportedException(string.Format("Don't currently support idents of type {0}", type.Name));
			}
		}
	}

	public class HqlRange : HqlStatement
	{
		internal HqlRange(IASTFactory factory, params HqlTreeNode[] children)
			: base(HqlSqlWalker.RANGE, "range", factory, children)
		{
		}
	}

	public class HqlFrom : HqlStatement
	{
		internal HqlFrom(IASTFactory factory, params HqlTreeNode[] children)
			: base(HqlSqlWalker.FROM, "from", factory, children)
		{
		}
	}

	public class HqlSelectFrom : HqlStatement
	{
		internal HqlSelectFrom(IASTFactory factory, params HqlTreeNode[] children)
			: base(HqlSqlWalker.SELECT_FROM, "select_from", factory, children)
		{
		}
	}

	public class HqlAlias : HqlExpression
	{
		public HqlAlias(IASTFactory factory, string @alias)
			: base(HqlSqlWalker.ALIAS, alias, factory)
		{
		}
	}

	public class HqlDivide : HqlExpression
	{
		public HqlDivide(IASTFactory factory, HqlExpression lhs, HqlExpression rhs)
			: base(HqlSqlWalker.DIV, "/", factory, lhs, rhs)
		{
		}
	}

	public class HqlMultiplty : HqlExpression
	{
		public HqlMultiplty(IASTFactory factory, HqlExpression lhs, HqlExpression rhs)
			: base(HqlSqlWalker.STAR, "*", factory, lhs, rhs)
		{
		}
	}

	public class HqlSubtract : HqlExpression
	{
		public HqlSubtract(IASTFactory factory, HqlExpression lhs, HqlExpression rhs)
			: base(HqlSqlWalker.MINUS, "-", factory, lhs, rhs)
		{
		}
	}

	public class HqlAdd : HqlExpression
	{
		public HqlAdd(IASTFactory factory, HqlExpression lhs, HqlExpression rhs)
			: base(HqlSqlWalker.PLUS, "+", factory, lhs, rhs)
		{
		}
	}

	public class HqlBooleanOr : HqlBooleanExpression
	{
		public HqlBooleanOr(IASTFactory factory, HqlBooleanExpression lhs, HqlBooleanExpression rhs)
			: base(HqlSqlWalker.OR, "or", factory, lhs, rhs)
		{
		}
	}

	public class HqlBooleanAnd : HqlBooleanExpression
	{
		public HqlBooleanAnd(IASTFactory factory, HqlBooleanExpression lhs, HqlBooleanExpression rhs)
			: base(HqlSqlWalker.AND, "and", factory, lhs, rhs)
		{
		}
	}

	public class HqlEquality : HqlBooleanExpression
	{
		public HqlEquality(IASTFactory factory, HqlExpression lhs, HqlExpression rhs)
			: base(HqlSqlWalker.EQ, "==", factory, lhs, rhs)
		{
		}
	}

	public class HqlParameter : HqlExpression
	{
		public HqlParameter(IASTFactory factory, string name)
			: base(HqlSqlWalker.COLON, ":", factory)
		{
			AddChild(new HqlIdent(factory, name));
		}
	}

	public class HqlDot : HqlExpression
	{
		public HqlDot(IASTFactory factory, HqlExpression lhs, HqlExpression rhs)
			: base(HqlSqlWalker.DOT, ".", factory, lhs, rhs)
		{
		}
	}

	public class HqlBooleanDot : HqlBooleanExpression
	{
		public HqlBooleanDot(IASTFactory factory, HqlDot dot) : base(dot.AstNode.Type, dot.AstNode.Text, factory, dot.Children)
		{
		}
	}

	public class HqlWhere : HqlStatement
	{
		public HqlWhere(IASTFactory factory, HqlExpression expression)
			: base(HqlSqlWalker.WHERE, "where", factory, expression)
		{
		}
	}

	public class HqlWith : HqlStatement
	{
		public HqlWith(IASTFactory factory, HqlExpression expression)
			: base(HqlSqlWalker.WITH, "with", factory, expression)
		{
		}
	}

	public class HqlHaving : HqlStatement
	{
		public HqlHaving(IASTFactory factory, HqlExpression expression)
			: base(HqlSqlWalker.HAVING, "having", factory, expression)
		{
		}
	}

	public class HqlSkip : HqlStatement
	{
		public HqlSkip(IASTFactory factory, HqlExpression parameter)
			: base(HqlSqlWalker.SKIP, "skip", factory, parameter) { }
	}

	public class HqlTake : HqlStatement
	{
		public HqlTake(IASTFactory factory, HqlExpression parameter)
			: base(HqlSqlWalker.TAKE, "take", factory, parameter) { }
	}

	public class HqlConstant : HqlExpression
	{
		public HqlConstant(IASTFactory factory, int type, string value)
			: base(type, value, factory)
		{
		}
	}

	public class HqlStringConstant : HqlConstant
	{
		public HqlStringConstant(IASTFactory factory, string s)
			: base(factory, HqlSqlWalker.QUOTED_String, s)
		{
		}
	}

	public class HqlDoubleConstant : HqlConstant
	{
		public HqlDoubleConstant(IASTFactory factory, string s)
			: base(factory, HqlSqlWalker.NUM_DOUBLE, s)
		{
		}
	}

	public class HqlFloatConstant : HqlConstant
	{
		public HqlFloatConstant(IASTFactory factory, string s)
			: base(factory, HqlSqlWalker.NUM_FLOAT, s)
		{
		}
	}

	public class HqlIntegerConstant : HqlConstant
	{
		public HqlIntegerConstant(IASTFactory factory, string s)
			: base(factory, HqlSqlWalker.NUM_INT, s)
		{
		}
	}

	public class HqlDecimalConstant : HqlConstant
	{
		public HqlDecimalConstant(IASTFactory factory, string s)
			: base(factory, HqlSqlWalker.NUM_DECIMAL, s)
		{
		}
	}

	public class HqlFalse : HqlConstant
	{
		public HqlFalse(IASTFactory factory)
			: base(factory, HqlSqlWalker.FALSE, "false")
		{
		}
	}

	public class HqlTrue : HqlConstant
	{
		public HqlTrue(IASTFactory factory)
			: base(factory, HqlSqlWalker.TRUE, "true")
		{
		}
	}

	public class HqlNull : HqlConstant
	{
		public HqlNull(IASTFactory factory)
			: base(factory, HqlSqlWalker.NULL, "null")
		{
		}
	}

	public class HqlOrderBy : HqlStatement
	{
		public HqlOrderBy(IASTFactory factory)
			: base(HqlSqlWalker.ORDER, "order by", factory)
		{
		}
	}

	public enum HqlDirection
	{
		Ascending,
		Descending
	}

	public class HqlDirectionStatement : HqlStatement
	{
		public HqlDirectionStatement(int type, string text, IASTFactory factory)
			: base(type, text, factory)
		{
		}
	}

	public class HqlDirectionAscending : HqlDirectionStatement
	{
		public HqlDirectionAscending(IASTFactory factory)
			: base(HqlSqlWalker.ASCENDING, "asc", factory)
		{
		}
	}

	public class HqlDirectionDescending : HqlDirectionStatement
	{
		public HqlDirectionDescending(IASTFactory factory)
			: base(HqlSqlWalker.DESCENDING, "desc", factory)
		{
		}
	}

	public class HqlSelect : HqlStatement
	{
		public HqlSelect(IASTFactory factory, params HqlExpression[] expression)
			: base(HqlSqlWalker.SELECT, "select", factory, expression)
		{
		}
	}

	public class HqlElse : HqlStatement
	{
		public HqlElse(IASTFactory factory, HqlExpression ifFalse)
			: base(HqlSqlWalker.ELSE, "else", factory, ifFalse)
		{
		}
	}

	public class HqlWhen : HqlStatement
	{
		public HqlWhen(IASTFactory factory, HqlExpression predicate, HqlExpression ifTrue)
			: base(HqlSqlWalker.WHEN, "when", factory, predicate, ifTrue)
		{
		}
	}

	public class HqlCase : HqlExpression
	{
		public HqlCase(IASTFactory factory, HqlWhen[] whenClauses, HqlExpression ifFalse)
			: base(HqlSqlWalker.CASE, "case", factory, whenClauses)
		{
			if (ifFalse != null)
			{
				AddChild(new HqlElse(factory, ifFalse));
			}
		}
	}

	public class HqlGreaterThanOrEqual : HqlBooleanExpression
	{
		public HqlGreaterThanOrEqual(IASTFactory factory, HqlExpression lhs, HqlExpression rhs)
			: base(HqlSqlWalker.GE, "ge", factory, lhs, rhs)
		{
		}
	}

	public class HqlGreaterThan : HqlBooleanExpression
	{
		public HqlGreaterThan(IASTFactory factory, HqlExpression lhs, HqlExpression rhs)
			: base(HqlSqlWalker.GT, "gt", factory, lhs, rhs)
		{
		}
	}

	public class HqlLessThanOrEqual : HqlBooleanExpression
	{
		public HqlLessThanOrEqual(IASTFactory factory, HqlExpression lhs, HqlExpression rhs)
			: base(HqlSqlWalker.LE, "le", factory, lhs, rhs)
		{
		}
	}

	public class HqlLessThan : HqlBooleanExpression
	{
		public HqlLessThan(IASTFactory factory, HqlExpression lhs, HqlExpression rhs)
			: base(HqlSqlWalker.LT, "lt", factory, lhs, rhs)
		{
		}
	}

	public class HqlInequality : HqlBooleanExpression
	{
		public HqlInequality(IASTFactory factory, HqlExpression lhs, HqlExpression rhs)
			: base(HqlSqlWalker.NE, "ne", factory, lhs, rhs)
		{
		}
	}

	public class HqlRowStar : HqlStatement
	{
		public HqlRowStar(IASTFactory factory)
			: base(HqlSqlWalker.ROW_STAR, "*", factory)
		{
		}
	}

	public class HqlCount : HqlExpression
	{
		public HqlCount(IASTFactory factory)
			: base(HqlSqlWalker.COUNT, "count", factory)
		{
		}

		public HqlCount(IASTFactory factory, HqlExpression child)
			: base(HqlSqlWalker.COUNT, "count", factory, child)
		{
		}
	}

	public class HqlAs : HqlExpression
	{
		public HqlAs(IASTFactory factory, HqlExpression expression, System.Type type)
			: base(HqlSqlWalker.AS, "as", factory, expression)
		{
			switch (System.Type.GetTypeCode(type))
			{
				case TypeCode.Int32:
					AddChild(new HqlIdent(factory, "integer"));
					break;
				default:
					throw new InvalidOperationException();
			}
		}
	}

	public class HqlCast : HqlExpression
	{
		public HqlCast(IASTFactory factory, HqlExpression expression, System.Type type)
			: base(HqlSqlWalker.METHOD_CALL, "method", factory)
		{
			AddChild(new HqlIdent(factory, "cast"));
			AddChild(new HqlExpressionList(factory, expression, new HqlIdent(factory, type)));
		}
	}

	public class HqlCoalesce : HqlExpression
	{
		public HqlCoalesce(IASTFactory factory, HqlExpression lhs, HqlExpression rhs)
			: base(HqlSqlWalker.METHOD_CALL, "coalesce", factory)
		{
			AddChild(new HqlIdent(factory, "coalesce"));
			AddChild(new HqlExpressionList(factory, lhs, rhs));
		}
	}

	public class HqlDictionaryIndex : HqlExpression
	{
		public HqlDictionaryIndex(IASTFactory factory, HqlExpression dictionary, HqlExpression index)
			: base(HqlSqlWalker.INDEX_OP, "[", factory, dictionary, index)
		{
		}
	}

	public class HqlIndices : HqlExpression
	{
		public HqlIndices(IASTFactory factory, HqlExpression dictionary)
			: base(HqlSqlWalker.INDICES, "indices", factory, dictionary)
		{
		}
	}

	public class HqlExpressionList : HqlStatement
	{
		public HqlExpressionList(IASTFactory factory, params HqlExpression[] expressions)
			: base(HqlSqlWalker.EXPR_LIST, "expr_list", factory, expressions)
		{
		}

		public HqlExpressionList(IASTFactory factory, IEnumerable<HqlExpression> expressions)
			: base(HqlSqlWalker.EXPR_LIST, "expr_list", factory, expressions.Cast<HqlTreeNode>())
		{
		}
	}

	public class HqlBooleanNot : HqlBooleanExpression
	{
		public HqlBooleanNot(IASTFactory factory, HqlBooleanExpression operand)
			: base(HqlSqlWalker.NOT, "not", factory, operand)
		{
		}
	}

	public class HqlAverage : HqlExpression
	{
		public HqlAverage(IASTFactory factory, HqlExpression expression)
			: base(HqlSqlWalker.AGGREGATE, "avg", factory, expression)
		{
		}
	}

	public class HqlBitwiseNot : HqlExpression
	{
		public HqlBitwiseNot(IASTFactory factory) : base(HqlSqlWalker.BNOT, "not", factory)
		{
		}
	}

	public class HqlSum : HqlExpression
	{
		public HqlSum(IASTFactory factory)
			: base(HqlSqlWalker.AGGREGATE, "sum", factory)
		{
		}

		public HqlSum(IASTFactory factory, HqlExpression expression)
			: base(HqlSqlWalker.AGGREGATE, "sum", factory, expression)
		{
		}
	}

	public class HqlMax : HqlExpression
	{
		public HqlMax(IASTFactory factory, HqlExpression expression)
			: base(HqlSqlWalker.AGGREGATE, "max", factory, expression)
		{
		}
	}

	public class HqlMin : HqlExpression
	{
		public HqlMin(IASTFactory factory, HqlExpression expression)
			: base(HqlSqlWalker.AGGREGATE, "min", factory, expression)
		{
		}
	}

	public class HqlJoin : HqlStatement
	{
		public HqlJoin(IASTFactory factory, HqlExpression expression, HqlAlias @alias) : base(HqlSqlWalker.JOIN, "join", factory, expression, @alias)
		{
		}
	}

	public class HqlLeftJoin : HqlTreeNode
	{
		public HqlLeftJoin(IASTFactory factory, HqlExpression expression, HqlAlias @alias) : base(HqlSqlWalker.JOIN, "join", factory, new HqlLeft(factory), expression, @alias)
		{
		}
	}

	public class HqlFetchJoin : HqlTreeNode
	{
		public HqlFetchJoin(IASTFactory factory, HqlExpression expression, HqlAlias @alias)
			: base(HqlSqlWalker.JOIN, "join", factory, new HqlFetch(factory), expression, @alias)
		{
		}
	}

	public class HqlLeftFetchJoin : HqlTreeNode
	{
		public HqlLeftFetchJoin(IASTFactory factory, HqlExpression expression, HqlAlias @alias)
			: base(HqlSqlWalker.JOIN, "join", factory, new HqlLeft(factory), new HqlFetch(factory), expression, @alias)
		{
		}
	}

	public class HqlFetch : HqlTreeNode
	{
		public HqlFetch(IASTFactory factory) : base(HqlSqlWalker.FETCH, "fetch", factory)
		{
		}
	}

	public class HqlClass : HqlExpression
	{
		public HqlClass(IASTFactory factory)
			: base(HqlSqlWalker.CLASS, "class", factory)
		{
		}
	}

	public class HqlBitwiseOr : HqlExpression
	{
		public HqlBitwiseOr(IASTFactory factory, HqlExpression lhs, HqlExpression rhs)
			: base(HqlSqlWalker.BOR, "bor", factory, lhs, rhs)
		{
		}
	}

	public class HqlBitwiseAnd : HqlExpression
	{
		public HqlBitwiseAnd(IASTFactory factory, HqlExpression lhs, HqlExpression rhs)
			: base(HqlSqlWalker.BAND, "band", factory, lhs, rhs)
		{
		}
	}

	public class HqlLeft : HqlTreeNode
	{
		public HqlLeft(IASTFactory factory)
			: base(HqlSqlWalker.LEFT, "left", factory)
		{
		}
	}

	public class HqlAny : HqlBooleanExpression
	{
		public HqlAny(IASTFactory factory) : base(HqlSqlWalker.ANY, "any", factory)
		{
		}
	}

	public class HqlExists : HqlBooleanExpression
	{
		public HqlExists(IASTFactory factory, HqlQuery query) : base(HqlSqlWalker.EXISTS, "exists", factory, query)
		{
		}
	}

	public class HqlElements : HqlBooleanExpression
	{
		public HqlElements(IASTFactory factory) : base(HqlSqlWalker.ELEMENTS, "elements", factory)
		{
		}
	}

	public class HqlDistinct : HqlStatement
	{
		public HqlDistinct(IASTFactory factory) : base(HqlSqlWalker.DISTINCT, "distinct", factory)
		{
		}
	}

	public class HqlGroupBy : HqlStatement
	{
		public HqlGroupBy(IASTFactory factory, params HqlExpression[] expressions) : base(HqlSqlWalker.GROUP, "group by", factory, expressions)
		{
		}
	}

	public class HqlAll : HqlBooleanExpression
	{
		public HqlAll(IASTFactory factory)
			: base(HqlSqlWalker.ALL, "all", factory)
		{
		}
	}

	public class HqlLike : HqlBooleanExpression
	{
		public HqlLike(IASTFactory factory, HqlExpression lhs, HqlExpression rhs)
			: base(HqlSqlWalker.LIKE, "like", factory, lhs, rhs)
		{
		}

		public HqlLike(IASTFactory factory, HqlExpression lhs, HqlExpression rhs, HqlConstant escapeCharacter)
		: base(HqlSqlWalker.LIKE, "like", factory, lhs, rhs, new HqlEscape(factory, escapeCharacter))
		{
		}
	}

	public class HqlEscape : HqlStatement
	{
		public HqlEscape(IASTFactory factory, HqlConstant escapeCharacter)
			: base(HqlSqlWalker.ESCAPE, "escape", factory, escapeCharacter)
		{
		}
	}

	public class HqlConcat : HqlExpression
	{
		public HqlConcat(IASTFactory factory, params HqlExpression[] args)
			: base(HqlSqlWalker.METHOD_CALL, "method", factory)
		{
			AddChild(new HqlIdent(factory, "concat"));
			AddChild(new HqlExpressionList(factory, args));
		}
	}

	public class HqlMethodCall : HqlExpression
	{
		public HqlMethodCall(IASTFactory factory, string methodName, IEnumerable<HqlExpression> parameters)
			: base(HqlSqlWalker.METHOD_CALL, "method", factory)
		{
			AddChild(new HqlIdent(factory, methodName));
			AddChild(new HqlExpressionList(factory, parameters));
		}
	}

	public class HqlBooleanMethodCall : HqlBooleanExpression
	{
		public HqlBooleanMethodCall(IASTFactory factory, string methodName, IEnumerable<HqlExpression> parameters)
			: base(HqlSqlWalker.METHOD_CALL, "method", factory)
		{
			AddChild(new HqlIdent(factory, methodName));
			AddChild(new HqlExpressionList(factory, parameters));
		}
	}

	public class HqlExpressionSubTreeHolder : HqlExpression
	{
		public HqlExpressionSubTreeHolder(IASTFactory factory, HqlTreeNode[] children) : base(int.MinValue, "expression sub-tree holder", factory, children)
		{
		}

		public HqlExpressionSubTreeHolder(IASTFactory factory, IEnumerable<HqlTreeNode> children) : base(int.MinValue, "expression sub-tree holder", factory, children)
		{
		}
	}

	public class HqlIsNull : HqlBooleanExpression
	{
		public HqlIsNull(IASTFactory factory, HqlExpression lhs)
			: base(HqlSqlWalker.IS_NULL, "is null", factory, lhs)
		{
		}
	}

	public class HqlIsNotNull : HqlBooleanExpression
	{
		public HqlIsNotNull(IASTFactory factory, HqlExpression lhs) : base(HqlSqlWalker.IS_NOT_NULL, "is not null", factory, lhs)
		{
		}
	}

	public class HqlStar : HqlExpression
	{
		public HqlStar(IASTFactory factory) : base(HqlSqlWalker.ROW_STAR, "*", factory)
		{
		}
	}

	public class HqlIn : HqlBooleanExpression
	{
		public HqlIn(IASTFactory factory, HqlExpression itemExpression, HqlTreeNode source)
			: base(HqlSqlWalker.IN, "in", factory, itemExpression)
		{
			AddChild(new HqlInList(factory, source));
		}
	}

	public class HqlInList : HqlTreeNode
	{
		public HqlInList(IASTFactory factory, HqlTreeNode source)
			: base(HqlSqlWalker.IN_LIST, "inlist", factory, source)
		{
		}
	}
}
