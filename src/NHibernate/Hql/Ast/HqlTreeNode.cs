using System;
using System.Collections.Generic;
using System.Reflection;
using NHibernate.Hql.Ast.ANTLR;
using NHibernate.Hql.Ast.ANTLR.Tree;

namespace NHibernate.Hql.Ast
{
    public class HqlTreeNode
    {
        protected readonly IASTNode _node;
        protected readonly List<HqlTreeNode> _children;

        protected HqlTreeNode(int type, string text, IASTFactory factory, params HqlTreeNode[] children)
        {
            _node = factory.CreateNode(type, text);
            _children = new List<HqlTreeNode>();

            foreach (var child in children)
            {
                _children.Add(child);
                _node.AddChild(child.AstNode);
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
            get 
            { 
                foreach (var child in _children)
                {
                    yield return child;
                }
            }
        }

        public void ClearChildren()
        {
            _children.Clear();
            _node.ClearChildren();
        }

        internal IASTNode AstNode
        {
            get { return _node; }
        }

        internal void AddChild(HqlTreeNode child)
        {
            _children.Add(child);
            _node.AddChild(child.AstNode);
        }

        public void AddChild(int index, HqlTreeNode node)
        {
            _children.Insert(index, node);
            _node.InsertChild(index, node.AstNode);
        }

    }

    public class HqlQuery : HqlTreeNode
    {
        internal HqlQuery(IASTFactory factory, params HqlTreeNode[] children)
            : base(HqlSqlWalker.QUERY, "query", factory, children)
        {
        }
    }


    public class HqlIdent : HqlTreeNode
    {
        internal HqlIdent(IASTFactory factory, string ident)
            : base(HqlSqlWalker.IDENT, ident, factory)
        {
        }

        internal HqlIdent(IASTFactory factory, System.Type type)
            : base(HqlSqlWalker.IDENT, "", factory)
        {
            if (IsNullableType(type))
            {
                type = ExtractUnderlyingTypeFromNullable(type);
            }

            switch (System.Type.GetTypeCode(type))
            {
                case TypeCode.Int32:
                    _node.Text = "integer";
                    break;
                case TypeCode.Decimal:
                    _node.Text = "decimal";
                    break;
                case TypeCode.DateTime:
                    _node.Text = "datetime";
                    break;
                default:
                    throw new NotSupportedException(string.Format("Don't currently support idents of type {0}", type.Name));
            }
        }

        private static System.Type ExtractUnderlyingTypeFromNullable(System.Type type)
        {
            return type.GetGenericArguments()[0];
        }

        private static bool IsNullableType(System.Type type)
        {
            return (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

    }

    public class HqlRange : HqlTreeNode
    {
        internal HqlRange(IASTFactory factory, params HqlTreeNode[] children)
            : base(HqlSqlWalker.RANGE, "range", factory, children)
        {
        }
    }

    public class HqlFrom : HqlTreeNode
    {
        internal HqlFrom(IASTFactory factory, params HqlTreeNode[] children)
            : base(HqlSqlWalker.FROM, "from", factory, children)
        {
        }
    }

    public class HqlSelectFrom : HqlTreeNode
    {
        internal HqlSelectFrom(IASTFactory factory, params HqlTreeNode[] children)
            : base(HqlSqlWalker.SELECT_FROM, "select_from", factory, children)
        {
        }
    }

    public class HqlAlias : HqlTreeNode
    {
        public HqlAlias(IASTFactory factory, string @alias)
            : base(HqlSqlWalker.ALIAS, alias, factory)
        {
        }
    }

    public class HqlDivide : HqlTreeNode
    {
        public HqlDivide(IASTFactory factory)
            : base(HqlSqlWalker.DIV, "/", factory)
        {
        }
    }

    public class HqlMultiplty : HqlTreeNode
    {
        public HqlMultiplty(IASTFactory factory)
            : base(HqlSqlWalker.STAR, "*", factory)
        {
        }
    }

    public class HqlSubtract : HqlTreeNode
    {
        public HqlSubtract(IASTFactory factory)
            : base(HqlSqlWalker.MINUS, "-", factory)
        {
        }
    }

    public class HqlAdd : HqlTreeNode
    {
        public HqlAdd(IASTFactory factory)
            : base(HqlSqlWalker.PLUS, "+", factory)
        {
        }
    }

    public class HqlBooleanOr : HqlTreeNode
    {
        public HqlBooleanOr(IASTFactory factory)
            : base(HqlSqlWalker.OR, "or", factory)
        {
        }
    }

    public class HqlBooleanAnd : HqlTreeNode
    {
        public HqlBooleanAnd(IASTFactory factory)
            : base(HqlSqlWalker.AND, "/", factory)
        {
        }
    }

    public class HqlEquality : HqlTreeNode
    {
        public HqlEquality(IASTFactory factory)
            : base(HqlSqlWalker.EQ, "==", factory)
        {
        }

        public HqlEquality(IASTFactory factory, HqlTreeNode lhs, HqlTreeNode rhs)
            : base(HqlSqlWalker.EQ, "==", factory, lhs, rhs)
        {
        }
    }

    public class HqlParameter : HqlTreeNode
    {
        public HqlParameter(IASTFactory factory, string name)
            : base(HqlSqlWalker.PARAM, name, factory)
        {
        }
    }

    public class HqlDot : HqlTreeNode
    {
        public HqlDot(IASTFactory factory)
            : base(HqlSqlWalker.DOT, ".", factory)
        {
        }
    }

    public class HqlWhere : HqlTreeNode
    {
        public HqlWhere(IASTFactory factory)
            : base(HqlSqlWalker.WHERE, "where", factory)
        {
        }

        public HqlWhere(IASTFactory factory, HqlTreeNode expression)
            : base(HqlSqlWalker.WHERE, "where", factory, expression)
        {
        }
    }

    public class HqlConstant : HqlTreeNode
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

    public class HqlNull : HqlConstant
    {
        public HqlNull(IASTFactory factory)
            : base(factory, HqlSqlWalker.NULL, "null")
        {
        }
    }

    public class HqlOrderBy : HqlTreeNode
    {
        public HqlOrderBy(IASTFactory factory)
            : base(HqlSqlWalker.ORDER, "", factory)
        {
        }

        public HqlOrderBy(IASTFactory factory, HqlTreeNode expression, HqlDirection hqlDirection)
            : base(HqlSqlWalker.ORDER, "", factory, expression,
                   hqlDirection == HqlDirection.Ascending ?
                        (HqlTreeNode)new HqlDirectionAscending(factory) : (HqlTreeNode)new HqlDirectionDescending(factory))
        {
        }
    }

    public enum HqlDirection
    {
        Ascending,
        Descending
    }

    public class HqlDirectionAscending : HqlTreeNode
    {
        public HqlDirectionAscending(IASTFactory factory)
            : base(HqlSqlWalker.ASCENDING, "asc", factory)
        {
        }
    }

    public class HqlDirectionDescending : HqlTreeNode
    {
        public HqlDirectionDescending(IASTFactory factory)
            : base(HqlSqlWalker.DESCENDING, "desc", factory)
        {
        }
    }

    public class HqlSelect : HqlTreeNode
    {
        public HqlSelect(IASTFactory factory, params HqlTreeNode[] expression)
            : base(HqlSqlWalker.SELECT, "select", factory, expression)
        {
        }
    }

    public class HqlConstructor : HqlTreeNode
    {
        public HqlConstructor(IASTFactory factory, ConstructorInfo ctor)
            : base(HqlSqlWalker.CONSTRUCTOR, "ctor", factory)
        {
            ((ASTNode)_node).Hack = ctor;
        }
    }

    public class HqlNill : HqlTreeNode
    {
        public HqlNill(IASTFactory factory)
            : base(0, "nill", factory)
        {
        }
    }

    public class HqlElse : HqlTreeNode
    {
        public HqlElse(IASTFactory factory)
            : base(HqlSqlWalker.ELSE, "else", factory)
        {
        }
    }

    public class HqlWhen : HqlTreeNode
    {
        public HqlWhen(IASTFactory factory)
            : base(HqlSqlWalker.WHEN, "when", factory)
        {
        }
    }

    public class HqlCase : HqlTreeNode
    {
        public HqlCase(IASTFactory factory)
            : base(HqlSqlWalker.CASE, "case", factory)
        {
        }
    }

    public class HqlGreaterThanOrEqual : HqlTreeNode
    {
        public HqlGreaterThanOrEqual(IASTFactory factory)
            : base(HqlSqlWalker.GE, "ge", factory)
        {
        }
    }

    public class HqlGreaterThan : HqlTreeNode
    {
        public HqlGreaterThan(IASTFactory factory)
            : base(HqlSqlWalker.GT, "gt", factory)
        {
        }
    }

    public class HqlLessThanOrEqual : HqlTreeNode
    {
        public HqlLessThanOrEqual(IASTFactory factory)
            : base(HqlSqlWalker.LE, "le", factory)
        {
        }
    }

    public class HqlLessThan : HqlTreeNode
    {
        public HqlLessThan(IASTFactory factory)
            : base(HqlSqlWalker.LT, "lt", factory)
        {
        }
    }

    public class HqlInequality : HqlTreeNode
    {
        public HqlInequality(IASTFactory factory)
            : base(HqlSqlWalker.NE, "ne", factory)
        {
        }
    }

    public class HqlRowStar : HqlTreeNode
    {
        public HqlRowStar(IASTFactory factory)
            : base(HqlSqlWalker.ROW_STAR, "*", factory)
        {
        }
    }

    public class HqlCount : HqlTreeNode
    {

        public HqlCount(IASTFactory factory)
            : base(HqlSqlWalker.COUNT, "count", factory)
        {
        }
        
        public HqlCount(IASTFactory factory, HqlTreeNode child)
            : base(HqlSqlWalker.COUNT, "count", factory, child)
        {
        }
    }

    public class HqlAs : HqlTreeNode
    {
        public HqlAs(IASTFactory factory, HqlTreeNode expression, System.Type type) : base(HqlSqlWalker.AS, "as", factory, expression)
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

    public class HqlCast : HqlTreeNode
    {
        public HqlCast(IASTFactory factory, HqlTreeNode expression, System.Type type) : base(HqlSqlWalker.METHOD_CALL, "method", factory)
        {
            AddChild(new HqlIdent(factory, "cast"));
            AddChild(new HqlExpressionList(factory, expression, new HqlIdent(factory, type)));
        }
    }

    public class HqlExpressionList : HqlTreeNode
    {
        public HqlExpressionList(IASTFactory factory, params HqlTreeNode[] expression) : base(HqlSqlWalker.EXPR_LIST, "expr_list", factory, expression)
        {
        }
    }

    public class HqlNot : HqlTreeNode
    {
        public HqlNot(IASTFactory factory) : base(HqlSqlWalker.NOT, "not", factory)
        {
        }
    }

    public class HqlAverage : HqlTreeNode
    {
        public HqlAverage(IASTFactory factory)
            : base(HqlSqlWalker.AGGREGATE, "avg", factory)
        {
        }

        public HqlAverage(IASTFactory factory, HqlTreeNode expression) : base(HqlSqlWalker.AGGREGATE, "avg", factory, expression)
        {
        }
    }

    public class HqlBitwiseNot : HqlTreeNode
    {
        public HqlBitwiseNot(IASTFactory factory) : base(HqlSqlWalker.BNOT, "not", factory)
        {
        }
    }

    public class HqlSum : HqlTreeNode
    {
        public HqlSum(IASTFactory factory)
            : base(HqlSqlWalker.AGGREGATE, "sum", factory)
        {
        }

        public HqlSum(IASTFactory factory, HqlTreeNode expression)
            : base(HqlSqlWalker.AGGREGATE, "sum", factory, expression)
        {
        }
    }

    public class HqlMax : HqlTreeNode
    {
        public HqlMax(IASTFactory factory) : base(HqlSqlWalker.AGGREGATE, "max", factory)
        {
        }

        public HqlMax(IASTFactory factory, HqlTreeNode expression)
            : base(HqlSqlWalker.AGGREGATE, "max", factory, expression)
        {
        }
}

    public class HqlMin : HqlTreeNode
    {
        public HqlMin(IASTFactory factory)
            : base(HqlSqlWalker.AGGREGATE, "min", factory)
        {
        }

        public HqlMin(IASTFactory factory, HqlTreeNode expression)
            : base(HqlSqlWalker.AGGREGATE, "min", factory, expression)
        {
        }
    }

    public class HqlAnd : HqlTreeNode
    {
        public HqlAnd(IASTFactory factory, HqlTreeNode left, HqlTreeNode right) : base(HqlSqlWalker.AND, "and", factory, left, right)
        {
        }
    }

    public class HqlJoin : HqlTreeNode
    {
        public HqlJoin(IASTFactory factory, HqlTreeNode expression, HqlAlias @alias) : base(HqlSqlWalker.JOIN, "join", factory, expression, @alias)
        {
        }
    }

    public class HqlAny : HqlTreeNode
    {
        public HqlAny(IASTFactory factory) : base(HqlSqlWalker.ANY, "any", factory)
        {
        }
    }

    public class HqlExists : HqlTreeNode
    {
        public HqlExists(IASTFactory factory) : base(HqlSqlWalker.EXISTS, "exists", factory)
        {
        }
    }

    public class HqlElements : HqlTreeNode
    {
        public HqlElements(IASTFactory factory) : base(HqlSqlWalker.ELEMENTS, "elements", factory)
        {
        }
    }

    public class HqlDistinct : HqlTreeNode
    {
        public HqlDistinct(IASTFactory factory) : base(HqlSqlWalker.DISTINCT, "distinct", factory)
        {
        }
    }

    public class HqlGroupBy : HqlTreeNode
    {
        public HqlGroupBy(IASTFactory factory) : base(HqlSqlWalker.GROUP, "group by", factory)
        {
        }
    }

}