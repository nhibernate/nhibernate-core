using System;
using System.Linq.Expressions;
using NHibernate.Impl;
using NHibernate.Type;
using NHibernate.Dialect.Function;

namespace NHibernate.Criterion
{
	/// <summary>
	/// The <tt>criterion</tt> package may be used by applications as a framework for building
	/// new kinds of <tt>Projection</tt>. However, it is intended that most applications will
	/// simply use the built-in projection types via the static factory methods of this class.<br/>
	/// <br/>
	/// The factory methods that take an alias allow the projected value to be referred to by 
	/// criterion and order instances.
	/// </summary>
	public static class Projections
	{
		/// <summary>
		/// Create a distinct projection from a projection
		/// </summary>
		/// <param name="proj"></param>
		/// <returns></returns>
		public static IProjection Distinct(IProjection proj)
		{
			return new Distinct(proj);
		}

		/// <summary>
		/// Create a new projection list
		/// </summary>
		/// <returns></returns>
		public static ProjectionList ProjectionList()
		{
			return new ProjectionList();
		}

		/// <summary>
		/// The query row count, ie. <tt>count(*)</tt>
		/// </summary>
		/// <returns>The RowCount projection mapped to an <see cref="Int32"/>.</returns>
		public static IProjection RowCount()
		{
			return new RowCountProjection();
		}

		/// <summary>
		/// The query row count, ie. <tt>count(*)</tt>
		/// </summary>
		/// <returns>The RowCount projection mapped to an <see cref="Int64"/>.</returns>
		public static IProjection RowCountInt64()
		{
			return new RowCountInt64Projection();
		}
		/// <summary>
		/// A property value count
		/// </summary>
		/// <param name="projection"></param>
		/// <returns></returns>
		public static CountProjection Count(IProjection projection)
		{
			return new CountProjection(projection);
		}
		/// <summary>
		/// A property value count
		/// </summary>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		public static CountProjection Count(string propertyName)
		{
			return new CountProjection(propertyName);
		}

		/// <summary>
		/// A distinct property value count
		/// </summary>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		public static CountProjection CountDistinct(string propertyName)
		{
			return new CountProjection(propertyName).SetDistinct();
		}

		/// <summary>
		/// A property maximum value
		/// </summary>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		public static AggregateProjection Max(String propertyName)
		{
			return new AggregateProjection("max", propertyName);
		}

		/// <summary>
		/// A projection maximum value
		/// </summary>
		/// <param name="projection"></param>
		/// <returns></returns>
		public static AggregateProjection Max(IProjection projection)
		{
			return new AggregateProjection("max", projection);
		}


		/// <summary>
		/// A property minimum value
		/// </summary>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		public static AggregateProjection Min(String propertyName)
		{
			return new AggregateProjection("min", propertyName);
		}

		/// <summary>
		/// A projection minimum value
		/// </summary>
		/// <param name="projection"></param>
		/// <returns></returns>
		public static AggregateProjection Min(IProjection projection)
		{
			return new AggregateProjection("min", projection);
		}

		/// <summary>
		/// A property average value
		/// </summary>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		public static AggregateProjection Avg(string propertyName)
		{
			return new AvgProjection(propertyName);
		}

		/// <summary>
		/// A property average value
		/// </summary>
		/// <param name="projection"></param>
		/// <returns></returns>
		public static AggregateProjection Avg(IProjection projection)
		{
			return new AvgProjection(projection);
		}

		/// <summary>
		/// A property value sum
		/// </summary>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		public static AggregateProjection Sum(String propertyName)
		{
			return new AggregateProjection("sum", propertyName);
		}

		/// <summary>
		/// A property value sum
		/// </summary>
		/// <param name="projection"></param>
		/// <returns></returns>
		public static AggregateProjection Sum(IProjection projection)
		{
			return new AggregateProjection("sum", projection);
		}

		/// <summary>
		/// A SQL projection, a typed select clause fragment
		/// </summary>
		/// <param name="sql"></param>
		/// <param name="columnAliases"></param>
		/// <param name="types"></param>
		/// <returns></returns>
		public static IProjection SqlProjection(string sql, string[] columnAliases, IType[] types)
		{
			return new SQLProjection(sql, columnAliases, types);
		}

		/// <summary>
		/// A grouping SQL projection, specifying both select clause and group by clause fragments
		/// </summary>
		/// <param name="sql"></param>
		/// <param name="groupBy"></param>
		/// <param name="columnAliases"></param>
		/// <param name="types"></param>
		/// <returns></returns>
		public static IProjection SqlGroupProjection(string sql, string groupBy, string[] columnAliases, IType[] types)
		{
			return new SQLProjection(sql, groupBy, columnAliases, types);
		}

		/// <summary>
		/// A grouping property value
		/// </summary>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		public static PropertyProjection GroupProperty(string propertyName)
		{
			return new PropertyProjection(propertyName, true);
		}

		/// <summary>
		/// A grouping projection value
		/// </summary>
		/// <param name="projection"></param>
		/// <returns></returns>
		public static GroupedProjection GroupProperty(IProjection projection)
		{
			return new GroupedProjection(projection);
		}

		/// <summary>
		/// A projected property value
		/// </summary>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		public static PropertyProjection Property(string propertyName)
		{
			return new PropertyProjection(propertyName);
		}

		/// <summary>
		/// A projected identifier value
		/// </summary>
		/// <returns></returns>
		public static IdentifierProjection Id()
		{
			return new IdentifierProjection();
		}

		/// <summary>
		/// Assign an alias to a projection, by wrapping it
		/// </summary>
		/// <param name="projection"></param>
		/// <param name="alias"></param>
		/// <returns></returns>
		public static IProjection Alias(IProjection projection, string alias)
		{
			return new AliasedProjection(projection, alias);
		}

		/// <summary>
		/// Casts the projection result to the specified type.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="projection">The projection.</param>
		/// <returns></returns>
		public static IProjection Cast(IType type, IProjection projection)
		{
			return new CastProjection(type, projection);
		}


		/// <summary>
		/// Return a constant value
		/// </summary>
		/// <param name="obj">The obj.</param>
		/// <returns></returns>
		public static IProjection Constant(object obj)
		{
			return new ConstantProjection(obj);
		}

		/// <summary>
		/// Return a constant value
		/// </summary>
		/// <param name="obj">The obj.</param>
		/// <param name="type"></param>
		/// <returns></returns>
		public static IProjection Constant(object obj, IType type)
		{
			return new ConstantProjection(obj,type);
		}


		/// <summary>
		/// Calls the named <see cref="ISQLFunction"/>
		/// </summary>
		/// <param name="functionName">Name of the function.</param>
		/// <param name="type">The type.</param>
		/// <param name="projections">The projections.</param>
		/// <returns></returns>
		public static IProjection SqlFunction(string functionName, IType type, params IProjection [] projections)
		{
			return new SqlFunctionProjection(functionName, type, projections);
		}

		/// <summary>
		/// Calls the specified <see cref="ISQLFunction"/>
		/// </summary>
		/// <param name="function">the function.</param>
		/// <param name="type">The type.</param>
		/// <param name="projections">The projections.</param>
		/// <returns></returns>
		public static IProjection SqlFunction(ISQLFunction function, IType type, params IProjection[] projections)
		{
			return new SqlFunctionProjection(function, type, projections);
		}

		/// <summary>
		/// Conditionally return the true or false part, depending on the criterion
		/// </summary>
		/// <param name="criterion">The criterion.</param>
		/// <param name="whenTrue">The when true.</param>
		/// <param name="whenFalse">The when false.</param>
		/// <returns></returns>
		public static IProjection Conditional(ICriterion criterion, IProjection whenTrue, IProjection whenFalse)
		{
			return new ConditionalProjection(criterion, whenTrue, whenFalse);
		}

		public static IProjection SubQuery(DetachedCriteria detachedCriteria)
		{
			SelectSubqueryExpression expr = new SelectSubqueryExpression(detachedCriteria);
			return new SubqueryProjection(expr);
		}

		/// <summary>
		/// A property average value
		/// </summary>
		public static AggregateProjection Avg<T>(Expression<Func<T, object>> expression)
		{
			return ExpressionProcessor.FindMemberProjection(expression.Body).Create<AggregateProjection>(Projections.Avg, Projections.Avg);
		}

		/// <summary>
		/// A property average value
		/// </summary>
		public static AggregateProjection Avg(Expression<Func<object>> expression)
		{
			return ExpressionProcessor.FindMemberProjection(expression.Body).Create<AggregateProjection>(Projections.Avg, Projections.Avg);
		}

		/// <summary>
		/// A property value count
		/// </summary>
		public static CountProjection Count<T>(Expression<Func<T, object>> expression)
		{
			return ExpressionProcessor.FindMemberProjection(expression.Body).Create<CountProjection>(Projections.Count, Projections.Count);
		}

		/// <summary>
		/// A property value count
		/// </summary>
		public static CountProjection Count(Expression<Func<object>> expression)
		{
			return ExpressionProcessor.FindMemberProjection(expression.Body).Create<CountProjection>(Projections.Count, Projections.Count);
		}

		/// <summary>
		/// A distinct property value count
		/// </summary>
		public static CountProjection CountDistinct<T>(Expression<Func<T, object>> expression)
		{
			return Projections.CountDistinct(ExpressionProcessor.FindMemberExpression(expression.Body));
		}

		/// <summary>
		/// A distinct property value count
		/// </summary>
		public static CountProjection CountDistinct(Expression<Func<object>> expression)
		{
			return Projections.CountDistinct(ExpressionProcessor.FindMemberExpression(expression.Body));
		}

		/// <summary>
		/// A grouping property value
		/// </summary>
		public static PropertyProjection Group<T>(Expression<Func<T, object>> expression)
		{
			return Projections.GroupProperty(ExpressionProcessor.FindMemberExpression(expression.Body));
		}

		/// <summary>
		/// A grouping property value
		/// </summary>
		public static PropertyProjection Group(Expression<Func<object>> expression)
		{
			return Projections.GroupProperty(ExpressionProcessor.FindMemberExpression(expression.Body));
		}

		/// <summary>
		/// A property maximum value
		/// </summary>
		public static AggregateProjection Max<T>(Expression<Func<T, object>> expression)
		{
			return ExpressionProcessor.FindMemberProjection(expression.Body).Create<AggregateProjection>(Projections.Max, Projections.Max);
		}

		/// <summary>
		/// A property maximum value
		/// </summary>
		public static AggregateProjection Max(Expression<Func<object>> expression)
		{
			return ExpressionProcessor.FindMemberProjection(expression.Body).Create<AggregateProjection>(Projections.Max, Projections.Max);
		}

		/// <summary>
		/// A property minimum value
		/// </summary>
		public static AggregateProjection Min<T>(Expression<Func<T, object>> expression)
		{
			return ExpressionProcessor.FindMemberProjection(expression.Body).Create<AggregateProjection>(Projections.Min, Projections.Min);
		}

		/// <summary>
		/// A property minimum value
		/// </summary>
		public static AggregateProjection Min(Expression<Func<object>> expression)
		{
			return ExpressionProcessor.FindMemberProjection(expression.Body).Create<AggregateProjection>(Projections.Min, Projections.Min);
		}

		/// <summary>
		/// A projected property value
		/// </summary>
		public static PropertyProjection Property<T>(Expression<Func<T, object>> expression)
		{
			return Projections.Property(ExpressionProcessor.FindMemberExpression(expression.Body));
		}

		/// <summary>
		/// A projected property value
		/// </summary>
		public static PropertyProjection Property(Expression<Func<object>> expression)
		{
			return Projections.Property(ExpressionProcessor.FindMemberExpression(expression.Body));
		}

		public static IProjection SubQuery<T>(QueryOver<T> detachedQueryOver)
		{
			return Projections.SubQuery(detachedQueryOver.DetachedCriteria);
		}

		/// <summary>
		/// A property value sum
		/// </summary>
		public static AggregateProjection Sum<T>(Expression<Func<T, object>> expression)
		{
			return ExpressionProcessor.FindMemberProjection(expression.Body).Create<AggregateProjection>(Projections.Sum, Projections.Sum);
		}

		/// <summary>
		/// A property value sum
		/// </summary>
		public static AggregateProjection Sum(Expression<Func<object>> expression)
		{
			return ExpressionProcessor.FindMemberProjection(expression.Body).Create<AggregateProjection>(Projections.Sum, Projections.Sum);
		}

		/// <summary>
		/// Project SQL function concat()
		/// Note: throws an exception outside of a QueryOver expression
		/// </summary>
		public static string Concat(params string[] strings)
		{
			throw new Exception("Not to be used directly - use inside QueryOver expression");
		}

		internal static IProjection ProcessConcat(MethodCallExpression methodCallExpression)
		{
			NewArrayExpression args = (NewArrayExpression)methodCallExpression.Arguments[0];
			IProjection[] projections = new IProjection[args.Expressions.Count];

			for (var i=0; i<args.Expressions.Count; i++)
				projections[i] = ExpressionProcessor.FindMemberProjection(args.Expressions[i]).AsProjection();

			return Projections.SqlFunction("concat", NHibernateUtil.String, projections);
		}
	}
}
