using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NHibernate.Hql.Ast.ANTLR.Tree;

namespace NHibernate.Hql.Ast
{
    public class HqlTreeBuilder
    {
        private readonly IASTFactory _factory;

        public HqlTreeBuilder()
        {
            _factory = new ASTFactory(new ASTTreeAdaptor());
        }

        public HqlQuery Query()
        {
            return new HqlQuery(_factory);
        }

        public HqlQuery Query(HqlSelectFrom selectFrom)
        {
            return new HqlQuery(_factory, selectFrom);
        }

        public HqlQuery Query(HqlSelectFrom selectFrom, HqlWhere where)
        {
            return new HqlQuery(_factory, selectFrom, where);
        }

        public HqlTreeNode Query(HqlSelectFrom selectFrom, HqlWhere where, HqlOrderBy orderBy)
        {
            return new HqlQuery(_factory, selectFrom, where, orderBy);
        }

        public HqlSelectFrom SelectFrom()
        {
            return new HqlSelectFrom(_factory);
        }

        public HqlSelectFrom SelectFrom(HqlSelect select)
        {
            return new HqlSelectFrom(_factory, select);
        }

        public HqlSelectFrom SelectFrom(HqlFrom @from, HqlSelect select)
        {
            return new HqlSelectFrom(_factory, @from, select);
        }

        public HqlSelectFrom SelectFrom(HqlFrom @from)
        {
            return new HqlSelectFrom(_factory, @from);
        }

        public HqlFrom From(HqlRange range)
        {
            return new HqlFrom(_factory, range);
        }

        public HqlFrom From()
        {
            return new HqlFrom(_factory);
        }

        public HqlRange Range(HqlIdent ident)
        {
            return new HqlRange(_factory, ident);
        }

        public HqlRange Range()
        {
            return new HqlRange(_factory);
        }

        public HqlRange Range(HqlTreeNode ident, HqlAlias alias)
        {
            return new HqlRange(_factory, ident, alias);
        }

        public HqlIdent Ident(string ident)
        {
            return new HqlIdent(_factory, ident);
        }

        public HqlIdent Ident(System.Type type)
        {
            return new HqlIdent(_factory, type);
        }

        public HqlAlias Alias(string alias)
        {
            return new HqlAlias(_factory, alias);
        }

        public HqlEquality Equality()
        {
            return new HqlEquality(_factory);
        }

        public HqlEquality Equality(HqlTreeNode lhs, HqlTreeNode rhs)
        {
            return new HqlEquality(_factory, lhs, rhs);
        }

        public HqlBooleanAnd BooleanAnd()
        {
            return new HqlBooleanAnd(_factory);
        }

        public HqlBooleanOr BooleanOr()
        {
            return new HqlBooleanOr(_factory);
        }

        public HqlAdd Add()
        {
            return new HqlAdd(_factory);
        }

        public HqlSubtract Subtract()
        {
            return new HqlSubtract(_factory);
        }

        public HqlMultiplty Multiply()
        {
            return new HqlMultiplty(_factory);
        }

        public HqlDivide Divide()
        {
            return new HqlDivide(_factory);
        }

        public HqlDot Dot()
        {
            return new HqlDot(_factory);
        }

        public HqlParameter Parameter(string name)
        {
            return new HqlParameter(_factory, name);
        }

        public HqlWhere Where(HqlTreeNode expression)
        {
            return new HqlWhere(_factory, expression);
        }

        public HqlWhere Where()
        {
            return new HqlWhere(_factory);
        }

        // TODO - constant will be removed when we have parameter handling done properly.  Particularly bad datetime handling here, so it'll be good to delete it :)
        public HqlConstant Constant(object value)
        {
            if (value == null)
            {
                return new HqlNull(_factory);
            }
            else
            {
                switch (System.Type.GetTypeCode(value.GetType()))
                {
                    case TypeCode.Int16:
                    case TypeCode.Int32:
                    case TypeCode.Int64:
                        return new HqlIntegerConstant(_factory, value.ToString());
                    case TypeCode.Single:
                        return new HqlFloatConstant(_factory, value.ToString());
                    case TypeCode.Double:
                        return new HqlDoubleConstant(_factory, value.ToString());
                    case TypeCode.Decimal:
                        return new HqlDecimalConstant(_factory, value.ToString());
                    case TypeCode.String:
                    case TypeCode.Char:
                        return new HqlStringConstant(_factory, "\'" + value + "\'");
                    case TypeCode.DateTime:
                        return new HqlStringConstant(_factory, "\'" + ((DateTime)value).ToString() + "\'");
                    case TypeCode.Boolean:
                        return new HqlStringConstant(_factory, "\'" + (((bool)value) ? "true" : "false") + "\'");
                    default:
                        throw new NotSupportedException(string.Format("The constant for '{0}' is not supported", value));
                }
            }
        }

        public HqlOrderBy OrderBy()
        {
            return new HqlOrderBy(_factory);
        }

        public HqlOrderBy OrderBy(HqlTreeNode expression, HqlDirection hqlDirection)
        {
            return new HqlOrderBy(_factory, expression, hqlDirection);
        }

        public HqlSelect Select(HqlTreeNode expression)
        {
            return new HqlSelect(_factory, expression);
        }

        public HqlSelect Select(params HqlTreeNode[] expression)
        {
            return new HqlSelect(_factory, expression);
        }

        public HqlSelect Select(IEnumerable<HqlTreeNode> expressions)
        {
            return new HqlSelect(_factory, expressions.ToArray());
        }

        public HqlConstructor Constructor(ConstructorInfo constructor)
        {
            return new HqlConstructor(_factory, constructor);
        }

        public HqlNill Holder()
        {
            return new HqlNill(_factory);
        }

        public HqlCase Case()
        {
            return new HqlCase(_factory);
        }

        public HqlWhen When()
        {
            return new HqlWhen(_factory);
        }

        public HqlElse Else()
        {
            return new HqlElse(_factory);
        }

        public HqlInequality Inequality()
        {
            return new HqlInequality(_factory);
        }

        public HqlLessThan LessThan()
        {
            return new HqlLessThan(_factory);
        }

        public HqlLessThanOrEqual LessThanOrEqual()
        {
            return new HqlLessThanOrEqual(_factory);
        }

        public HqlGreaterThan GreaterThan()
        {
            return new HqlGreaterThan(_factory);
        }

        public HqlGreaterThanOrEqual GreaterThanOrEqual()
        {
            return new HqlGreaterThanOrEqual(_factory);
        }

        public HqlCount Count()
        {
            return new HqlCount(_factory);
        }

        public HqlCount Count(HqlTreeNode child)
        {
            return new HqlCount(_factory, child);
        }

        public HqlRowStar RowStar()
        {
            return new HqlRowStar(_factory);
        }

        public HqlCast Cast(HqlTreeNode expression, System.Type type)
        {
            return new HqlCast(_factory, expression, type);
        }

        public HqlBitwiseNot BitwiseNot()
        {
            return new HqlBitwiseNot(_factory);
        }

        public HqlNot Not()
        {
            return new HqlNot(_factory);
        }

        public HqlAverage Average()
        {
            return new HqlAverage(_factory);
        }

        public HqlAverage Average(HqlTreeNode expression)
        {
            return new HqlAverage(_factory, expression);
        }

        public HqlSum Sum()
        {
            return new HqlSum(_factory);
        }

        public HqlSum Sum(HqlTreeNode expression)
        {
            return new HqlSum(_factory, expression);
        }

        public HqlMin Min()
        {
            return new HqlMin(_factory);
        }

        public HqlMin Min(HqlTreeNode expression)
        {
            return new HqlMin(_factory, expression);
        }

        public HqlMax Max()
        {
            return new HqlMax(_factory);
        }

        public HqlMax Max(HqlTreeNode expression)
        {
            return new HqlMax(_factory, expression);
        }

        public HqlAnd And(HqlTreeNode left, HqlTreeNode right)
        {
            return new HqlAnd(_factory, left, right);
        }

        public HqlJoin Join(HqlTreeNode expression, HqlAlias @alias)
        {
            return new HqlJoin(_factory, expression, @alias);
        }

        public HqlAny Any()
        {
            return new HqlAny(_factory);
        }

        public HqlExists Exists()
        {
            return new HqlExists(_factory);
        }

        public HqlElements Elements()
        {
            return new HqlElements(_factory);
        }

        public HqlDistinct Distinct()
        {
            return new HqlDistinct(_factory);
        }

        public HqlDirectionAscending Ascending()
        {
            return new HqlDirectionAscending(_factory);
        }

        public HqlDirectionDescending Descending()
        {
            return new HqlDirectionDescending(_factory);
        }

        public HqlGroupBy GroupBy()
        {
            return new HqlGroupBy(_factory);
        }

		public HqlAll All()
    	{
    		return new HqlAll(_factory);
    	}

        public HqlLike Like()
        {
            return new HqlLike(_factory);
        }

        public HqlConcat Concat()
        {
            return new HqlConcat(_factory);
        }

        public HqlExpressionList ExpressionList()
        {
            return new HqlExpressionList(_factory);
        }
    }
}