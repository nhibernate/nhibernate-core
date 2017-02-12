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

		public HqlWith With(HqlExpression expression)
		{
			return new HqlWith(_factory, expression);
		}

		public HqlHaving Having(HqlExpression expression)
		{
			return new HqlHaving(_factory, expression);
		}

		// TODO - constant will be removed when we have parameter handling done properly.  Particularly bad datetime handling here, so it'll be good to delete it :)
		public HqlConstant Constant(object value)
		{
			if (value == null)
			{
				return new HqlNull(_factory);
			}

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
					return new HqlStringConstant(_factory, "\'" + (DateTime)value + "\'");
				case TypeCode.Boolean:
					return (bool)value ? (HqlConstant)True() : (HqlConstant)False();
				default:
					throw new NotSupportedException(string.Format("The constant for '{0}' is not supported", value));
			}
		}

		public HqlOrderBy OrderBy()
		{
			return new HqlOrderBy(_factory);
		}

		public HqlSkip Skip(HqlExpression parameter)
		{
			return new HqlSkip(_factory, parameter);
		}

		public HqlTake Take(HqlExpression parameter)
		{
			return new HqlTake(_factory, parameter);
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

		public HqlBooleanNot BooleanNot(HqlBooleanExpression operand)
		{
			return new HqlBooleanNot(_factory, operand);
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

		public HqlGroupBy GroupBy(params HqlExpression[] expressions)
		{
			return new HqlGroupBy(_factory, expressions);
		}

		public HqlAll All()
		{
			return new HqlAll(_factory);
		}

		public HqlLike Like(HqlExpression lhs, HqlExpression rhs)
		{
			return new HqlLike(_factory, lhs, rhs);
		}

		public HqlLike Like(HqlExpression lhs, HqlExpression rhs, HqlConstant escapeCharacter)
		{
			return new HqlLike(_factory, lhs, rhs, escapeCharacter);
		}

		public HqlConcat Concat(params HqlExpression[] args)
		{
			return new HqlConcat(_factory, args);
		}

		public HqlMethodCall MethodCall(string methodName, IEnumerable<HqlExpression> parameters)
		{
			return new HqlMethodCall(_factory, methodName, parameters);
		}

		public HqlMethodCall MethodCall(string methodName, params HqlExpression[] parameters)
		{
			return new HqlMethodCall(_factory, methodName, parameters);
		}

		public HqlBooleanMethodCall BooleanMethodCall(string methodName, IEnumerable<HqlExpression> parameters)
		{
			return new HqlBooleanMethodCall(_factory, methodName, parameters);
		}

		public HqlExpressionSubTreeHolder ExpressionSubTreeHolder(params HqlTreeNode[] children)
		{
			return new HqlExpressionSubTreeHolder(_factory, children);
		}

		public HqlExpressionSubTreeHolder ExpressionSubTreeHolder(IEnumerable<HqlTreeNode> children)
		{
			return new HqlExpressionSubTreeHolder(_factory, children);
		}

		public HqlIsNull IsNull(HqlExpression lhs)
		{
			return new HqlIsNull(_factory, lhs);
		}

		public HqlIsNotNull IsNotNull(HqlExpression lhs)
		{
			return new HqlIsNotNull(_factory, lhs);
		}

		public HqlTreeNode ExpressionList(IEnumerable<HqlExpression> expressions)
		{
			return new HqlExpressionList(_factory, expressions);
		}

		public HqlStar Star()
		{
			return new HqlStar(_factory);
		}

		public HqlTrue True()
		{
			return new HqlTrue(_factory);
		}

		public HqlFalse False()
		{
			return new HqlFalse(_factory);
		}

		public HqlIn In(HqlExpression itemExpression, HqlTreeNode source)
		{
			return new HqlIn(_factory, itemExpression, source);
		}

		public HqlLeftJoin LeftJoin(HqlExpression expression, HqlAlias @alias)
		{
			return new HqlLeftJoin(_factory, expression, @alias);
		}

		public HqlFetchJoin FetchJoin(HqlExpression expression, HqlAlias @alias)
		{
			return new HqlFetchJoin(_factory, expression, @alias);
		}

		public HqlLeftFetchJoin LeftFetchJoin(HqlExpression expression, HqlAlias @alias)
		{
			return new HqlLeftFetchJoin(_factory, expression, @alias);
		}

		public HqlClass Class()
		{
			return new HqlClass(_factory);
		}

		public HqlBitwiseAnd BitwiseAnd(HqlExpression lhs, HqlExpression rhs)
		{
			return new HqlBitwiseAnd(_factory, lhs, rhs);
		}

		public HqlBitwiseOr BitwiseOr(HqlExpression lhs, HqlExpression rhs)
		{
			return new HqlBitwiseOr(_factory, lhs, rhs);
		}

		public HqlTreeNode Coalesce(HqlExpression lhs, HqlExpression rhs)
		{
			return new HqlCoalesce(_factory, lhs, rhs);
		}

		public HqlTreeNode DictionaryItem(HqlExpression dictionary, HqlExpression index)
		{
			return new HqlDictionaryIndex(_factory, dictionary, index);
		}

		public HqlTreeNode Indices(HqlExpression dictionary)
		{
			return new HqlIndices(_factory, dictionary);
		}
	}
}
