using System;
using System.Collections.Generic;
using System.Linq;
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

        public HqlEquality Equality(HqlExpression lhs, HqlExpression rhs)
        {
            return new HqlEquality(_factory, lhs, rhs);
        }

        public HqlBooleanAnd BooleanAnd(HqlBooleanExpression lhs, HqlBooleanExpression rhs)
        {
            return new HqlBooleanAnd(_factory, lhs, rhs);
        }

        public HqlBooleanOr BooleanOr(HqlBooleanExpression lhs, HqlBooleanExpression rhs)
        {
            return new HqlBooleanOr(_factory, lhs, rhs);
        }

        public HqlAdd Add(HqlExpression lhs, HqlExpression rhs)
        {
            return new HqlAdd(_factory, lhs, rhs);
        }

        public HqlSubtract Subtract(HqlExpression lhs, HqlExpression rhs)
        {
            return new HqlSubtract(_factory, lhs, rhs);
        }

        public HqlMultiplty Multiply(HqlExpression lhs, HqlExpression rhs)
        {
            return new HqlMultiplty(_factory, lhs, rhs);
        }

        public HqlDivide Divide(HqlExpression lhs, HqlExpression rhs)
        {
            return new HqlDivide(_factory, lhs, rhs);
        }

        public HqlDot Dot(HqlExpression lhs, HqlExpression rhs)
        {
            return new HqlDot(_factory, lhs, rhs);
        }

        public HqlParameter Parameter(string name)
        {
            return new HqlParameter(_factory, name);
        }

        public HqlWhere Where(HqlExpression expression)
        {
            return new HqlWhere(_factory, expression);
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

        public HqlSelect Select(HqlExpression expression)
        {
            return new HqlSelect(_factory, expression);
        }

        public HqlSelect Select(params HqlExpression[] expression)
        {
            return new HqlSelect(_factory, expression);
        }

        public HqlSelect Select(IEnumerable<HqlExpression> expressions)
        {
            return new HqlSelect(_factory, expressions.ToArray());
        }

        public HqlCase Case(HqlWhen[] whenClauses)
        {
            return new HqlCase(_factory, whenClauses, null);
        }

        public HqlCase Case(HqlWhen[] whenClauses, HqlExpression ifFalse)
        {
            return new HqlCase(_factory, whenClauses, ifFalse);
        }

        public HqlWhen When(HqlExpression predicate, HqlExpression ifTrue)
        {
            return new HqlWhen(_factory, predicate, ifTrue);
        }

        public HqlElse Else(HqlExpression ifFalse)
        {
            return new HqlElse(_factory, ifFalse);
        }

        public HqlInequality Inequality(HqlExpression lhs, HqlExpression rhs)
        {
            return new HqlInequality(_factory, lhs, rhs);
        }

        public HqlLessThan LessThan(HqlExpression lhs, HqlExpression rhs)
        {
            return new HqlLessThan(_factory, lhs, rhs);
        }

        public HqlLessThanOrEqual LessThanOrEqual(HqlExpression lhs, HqlExpression rhs)
        {
            return new HqlLessThanOrEqual(_factory, lhs, rhs);
        }

        public HqlGreaterThan GreaterThan(HqlExpression lhs, HqlExpression rhs)
        {
            return new HqlGreaterThan(_factory, lhs, rhs);
        }

        public HqlGreaterThanOrEqual GreaterThanOrEqual(HqlExpression lhs, HqlExpression rhs)
        {
            return new HqlGreaterThanOrEqual(_factory, lhs, rhs);
        }

        public HqlCount Count()
        {
            return new HqlCount(_factory);
        }

        public HqlCount Count(HqlExpression child)
        {
            return new HqlCount(_factory, child);
        }

        public HqlRowStar RowStar()
        {
            return new HqlRowStar(_factory);
        }

        public HqlCast Cast(HqlExpression expression, System.Type type)
        {
            return new HqlCast(_factory, expression, type);
        }

        public HqlBitwiseNot BitwiseNot()
        {
            return new HqlBitwiseNot(_factory);
        }

        public HqlNot Not(HqlBooleanExpression operand)
        {
            return new HqlNot(_factory, operand);
        }

        public HqlAverage Average(HqlExpression expression)
        {
            return new HqlAverage(_factory, expression);
        }

        public HqlSum Sum(HqlExpression expression)
        {
            return new HqlSum(_factory, expression);
        }

        public HqlMin Min(HqlExpression expression)
        {
            return new HqlMin(_factory, expression);
        }

        public HqlMax Max(HqlExpression expression)
        {
            return new HqlMax(_factory, expression);
        }

        public HqlJoin Join(HqlExpression expression, HqlAlias @alias)
        {
            return new HqlJoin(_factory, expression, @alias);
        }

        public HqlAny Any()
        {
            return new HqlAny(_factory);
        }

        public HqlExists Exists(HqlQuery query)
        {
            return new HqlExists(_factory, query);
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

        public HqlGroupBy GroupBy(HqlExpression expression)
        {
            return new HqlGroupBy(_factory, expression);
        }

		public HqlAll All()
    	{
    		return new HqlAll(_factory);
    	}

        public HqlLike Like(HqlExpression lhs, HqlExpression rhs)
        {
            return new HqlLike(_factory, lhs, rhs);
        }

        public HqlConcat Concat(params HqlExpression[] args)
        {
            return new HqlConcat(_factory, args);
        }

        public HqlExpressionList ExpressionList()
        {
            return new HqlExpressionList(_factory);
        }

        public HqlMethodCall MethodCall(string methodName, HqlExpression parameter)
        {
            return new HqlMethodCall(_factory, methodName, parameter);
        }

        public HqlDistinctHolder DistinctHolder(params HqlTreeNode[] children)
        {
            return new HqlDistinctHolder(_factory, children);
        }
    }
}