using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Impl;
using NHibernate.Type;
using NHibernate.Util;
using Remotion.Linq;
using Remotion.Linq.Parsing.ExpressionTreeVisitors;
using System.Threading;
using System.Threading.Tasks;

namespace NHibernate.Linq
{
	public static class LinqExtensionMethods
	{
		private static readonly Dictionary<string, MethodInfo> cachableQueryableMethods;

		static LinqExtensionMethods()
		{
			cachableQueryableMethods = new Dictionary<string, MethodInfo>
			{
				{"Count", GetMethod(() => new EnumerableQuery<int>(new int[] { }).Count())},
				{"CountParam", GetMethod(() => new EnumerableQuery<int>(new int[] { }).Count(o => true))},

				{"LongCount", GetMethod(() => new EnumerableQuery<long>(new long[] { }).LongCount())},
				{"LongCountParam", GetMethod(() => new EnumerableQuery<long>(new long[] { }).LongCount(o => true))},

				{"Any", GetMethod(() => new EnumerableQuery<int>(new int[] { }).Any())},
				{"AnyParam", GetMethod(() => new EnumerableQuery<int>(new int[] { }).Any(o => true))},

				{"First", GetMethod(() => new EnumerableQuery<int>(new int[] { }).First())},
				{"FirstParam", GetMethod(() => new EnumerableQuery<int>(new int[] { }).First(o => true))},

				{"FirstOrDefault", GetMethod(() => new EnumerableQuery<int>(new int[] { }).FirstOrDefault())},
				{"FirstOrDefaultParam", GetMethod(() => new EnumerableQuery<int>(new int[] { }).FirstOrDefault(o => true))},

				{"Single", GetMethod(() => new EnumerableQuery<int>(new int[] { }).Single())},
				{"SingleParam", GetMethod(() => new EnumerableQuery<int>(new int[] { }).Single(o => true))},

				{"SingleOrDefault", GetMethod(() => new EnumerableQuery<int>(new int[] { }).SingleOrDefault())},
				{"SingleOrDefaultParam", GetMethod(() => new EnumerableQuery<int>(new int[] { }).SingleOrDefault(o => true))},

				{"Min", GetMethod(() => new EnumerableQuery<int>(new int[] { }).Min())},
				{"MinParam", GetMethod(() => new EnumerableQuery<int>(new int[] { }).Min(o => true))},

				{"Max", GetMethod(() => new EnumerableQuery<int>(new int[] { }).Max())},
				{"MaxParam", GetMethod(() => new EnumerableQuery<int>(new int[] { }).Max(o => true))},

				{"SumInt", GetMethod(() => new EnumerableQuery<int>(new int[] { }).Sum())},
				{"SumInt?", GetMethod(() => new EnumerableQuery<int?>(new int?[] { }).Sum())},
				{"SumLong", GetMethod(() => new EnumerableQuery<long>(new long[] { }).Sum())},
				{"SumLong?", GetMethod(() => new EnumerableQuery<long?>(new long?[] { }).Sum())},
				{"SumFloat", GetMethod(() => new EnumerableQuery<float>(new float[] { }).Sum())},
				{"SumFloat?", GetMethod(() => new EnumerableQuery<float?>(new float?[] { }).Sum())},
				{"SumDouble", GetMethod(() => new EnumerableQuery<double>(new double[] { }).Sum())},
				{"SumDouble?", GetMethod(() => new EnumerableQuery<double?>(new double?[] { }).Sum())},
				{"SumDecimal", GetMethod(() => new EnumerableQuery<decimal>(new decimal[] { }).Sum())},
				{"SumDecimal?", GetMethod(() => new EnumerableQuery<decimal?>(new decimal?[] { }).Sum())},
				{"SumIntParam", GetMethod(() => new EnumerableQuery<int>(new int[] { }).Sum(o => 1))},
				{"SumInt?Param", GetMethod(() => new EnumerableQuery<int?>(new int?[] { }).Sum(o => (int?)1))},
				{"SumLongParam", GetMethod(() => new EnumerableQuery<long>(new long[] { }).Sum(o => (long)1))},
				{"SumLong?Param", GetMethod(() => new EnumerableQuery<long?>(new long?[] { }).Sum(o => (long?)1))},
				{"SumFloatParam", GetMethod(() => new EnumerableQuery<float>(new float[] { }).Sum(o => (float)1))},
				{"SumFloat?Param", GetMethod(() => new EnumerableQuery<float?>(new float?[] { }).Sum(o => (float?)1))},
				{"SumDoubleParam", GetMethod(() => new EnumerableQuery<double>(new double[] { }).Sum(o => (double)1))},
				{"SumDouble?Param", GetMethod(() => new EnumerableQuery<double?>(new double?[] { }).Sum(o => (double?)1))},
				{"SumDecimalParam", GetMethod(() => new EnumerableQuery<decimal>(new decimal[] { }).Sum(o => (decimal)1))},
				{"SumDecimal?Param", GetMethod(() => new EnumerableQuery<decimal?>(new decimal?[] { }).Sum(o => (decimal?)1))},

				{"AverageInt", GetMethod(() => new EnumerableQuery<int>(new int[] { }).Average())},
				{"AverageInt?", GetMethod(() => new EnumerableQuery<int?>(new int?[] { }).Average())},
				{"AverageLong", GetMethod(() => new EnumerableQuery<long>(new long[] { }).Average())},
				{"AverageLong?", GetMethod(() => new EnumerableQuery<long?>(new long?[] { }).Average())},
				{"AverageFloat", GetMethod(() => new EnumerableQuery<float>(new float[] { }).Average())},
				{"AverageFloat?", GetMethod(() => new EnumerableQuery<float?>(new float?[] { }).Average())},
				{"AverageDouble", GetMethod(() => new EnumerableQuery<double>(new double[] { }).Average())},
				{"AverageDouble?", GetMethod(() => new EnumerableQuery<double?>(new double?[] { }).Average())},
				{"AverageDecimal", GetMethod(() => new EnumerableQuery<decimal>(new decimal[] { }).Average())},
				{"AverageDecimal?", GetMethod(() => new EnumerableQuery<decimal?>(new decimal?[] { }).Average())},
				{"AverageIntParam", GetMethod(() => new EnumerableQuery<int>(new int[] { }).Average(o => 1))},
				{"AverageInt?Param", GetMethod(() => new EnumerableQuery<int?>(new int?[] { }).Average(o => (int?)1))},
				{"AverageLongParam", GetMethod(() => new EnumerableQuery<long>(new long[] { }).Average(o => (long)1))},
				{"AverageLong?Param", GetMethod(() => new EnumerableQuery<long?>(new long?[] { }).Average(o => (long?)1))},
				{"AverageFloatParam", GetMethod(() => new EnumerableQuery<float>(new float[] { }).Average(o => (float)1))},
				{"AverageFloat?Param", GetMethod(() => new EnumerableQuery<float?>(new float?[] { }).Average(o => (float?)1))},
				{"AverageDoubleParam", GetMethod(() => new EnumerableQuery<double>(new double[] { }).Average(o => (double)1))},
				{"AverageDouble?Param", GetMethod(() => new EnumerableQuery<double?>(new double?[] { }).Average(o => (double?)1))},
				{"AverageDecimalParam", GetMethod(() => new EnumerableQuery<decimal>(new decimal[] { }).Average(o => (decimal)1))},
				{"AverageDecimal?Param", GetMethod(() => new EnumerableQuery<decimal?>(new decimal?[] { }).Average(o => (decimal?)1))},
			};
		}

		private static MethodInfo GetMethod<T>(Expression<Func<T>> expression)
		{
			var member = expression.Body as MethodCallExpression;
			if (member == null)
			{
				throw new ArgumentException("Expression is not a method", nameof(expression));
			}
			return member.Method.IsGenericMethod
				? member.Method.GetGenericMethodDefinition()
				: member.Method;
		}

		#region AnyAsync

		/// <summary>Determines whether a sequence contains any elements.</summary>
		/// <returns>true if the source sequence contains any elements; otherwise, false.</returns>
		/// <param name="source">A sequence to check for being empty.</param>
		/// <param name="cancellationToken"></param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="source" /> is null.</exception>
		public static Task<bool> AnyAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}
			var provider = (INhQueryProvider)source.Provider;
			var methodInfo = cachableQueryableMethods["Any"].MakeGenericMethod(typeof(TSource));
			var expression = new[] { source.Expression };
			return provider.ExecuteAsync<bool>(Expression.Call(null, methodInfo, expression), cancellationToken);
		}

		/// <summary>Determines whether any element of a sequence satisfies a condition.</summary>
		/// <returns>true if any elements in the source sequence pass the test in the specified predicate; otherwise, false.</returns>
		/// <param name="source">A sequence whose elements to test for a condition.</param>
		/// <param name="predicate">A function to test each element for a condition.</param>
		/// <param name="cancellationToken"></param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="source" /> or <paramref name="predicate" /> is null.</exception>
		public static Task<bool> AnyAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}
			if (predicate == null)
			{
				throw new ArgumentNullException(nameof(predicate));
			}
			var provider = (INhQueryProvider)source.Provider;
			var methodInfo = cachableQueryableMethods["AnyParam"].MakeGenericMethod(typeof(TSource));
			var expression = new[] { source.Expression, Expression.Quote(predicate) };
			return provider.ExecuteAsync<bool>(Expression.Call(null, methodInfo, expression), cancellationToken);
		}

		#endregion

		#region CountAsync

		/// <summary>Returns the number of elements in a sequence.</summary>
		/// <returns>The number of elements in the input sequence.</returns>
		/// <param name="source">The <see cref="T:System.Linq.IQueryable`1" /> that contains the elements to be counted.</param>
		/// <param name="cancellationToken"></param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="source" /> is null.</exception>
		/// <exception cref="T:System.OverflowException">The number of elements in <paramref name="source" /> is larger than <see cref="F:System.Int32.MaxValue" />.</exception>
		public static Task<int> CountAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}
			var provider = (INhQueryProvider)source.Provider;
			var methodInfo = cachableQueryableMethods["Count"].MakeGenericMethod(typeof(TSource));
			var expression = new[] { source.Expression };
			return provider.ExecuteAsync<int>(Expression.Call(null, methodInfo, expression), cancellationToken);
		}

		/// <summary>Returns the number of elements in the specified sequence that satisfies a condition.</summary>
		/// <returns>The number of elements in the sequence that satisfies the condition in the predicate function.</returns>
		/// <param name="source">An <see cref="T:System.Linq.IQueryable`1" /> that contains the elements to be counted.</param>
		/// <param name="predicate">A function to test each element for a condition.</param>
		/// <param name="cancellationToken"></param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="source" /> or <paramref name="predicate" /> is null.</exception>
		/// <exception cref="T:System.OverflowException">The number of elements in <paramref name="source" /> is larger than <see cref="F:System.Int32.MaxValue" />.</exception>
		public static Task<int> CountAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}
			if (predicate == null)
			{
				throw new ArgumentNullException(nameof(predicate));
			}
			var provider = (INhQueryProvider)source.Provider;
			var methodInfo = cachableQueryableMethods["CountParam"].MakeGenericMethod(typeof(TSource));
			var expression = new[] { source.Expression, Expression.Quote(predicate) };
			return provider.ExecuteAsync<int>(Expression.Call(null, methodInfo, expression), cancellationToken);
		}

		#endregion

		#region SumAsync

		/// <summary>
		/// Computes the sum of a sequence of <see cref="T:System.Int32"/> values.
		/// </summary>
		/// 
		/// <returns>
		/// The sum of the values in the sequence.
		/// </returns>
		/// <param name="source">A sequence of <see cref="T:System.Int32"/> values to calculate the sum of.</param>
		/// <param name="cancellationToken"></param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="source"/> is null.</exception>
		/// <exception cref="T:System.OverflowException">The sum is larger than <see cref="F:System.Int32.MaxValue"/>.</exception>
		public static Task<int> SumAsync(this IQueryable<int> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}
			var provider = (INhQueryProvider)source.Provider;
			var methodInfo = cachableQueryableMethods["SumInt"];
			var expression = new[] { source.Expression };
			return provider.ExecuteAsync<int>(Expression.Call(null, methodInfo, expression), cancellationToken);
		}

		/// <summary>
		/// Computes the sum of a sequence of nullable <see cref="T:System.Int32"/> values.
		/// </summary>
		/// 
		/// <returns>
		/// The sum of the values in the sequence.
		/// </returns>
		/// <param name="source">A sequence of nullable <see cref="T:System.Int32"/> values to calculate the sum of.</param>
		/// <param name="cancellationToken"></param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="source"/> is null.</exception>
		/// <exception cref="T:System.OverflowException">The sum is larger than <see cref="F:System.Int32.MaxValue"/>.</exception>
		public static Task<int?> SumAsync(this IQueryable<int?> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}
			var provider = (INhQueryProvider)source.Provider;
			var methodInfo = cachableQueryableMethods["SumInt?"];
			var expression = new[] { source.Expression };
			return provider.ExecuteAsync<int?>(Expression.Call(null, methodInfo, expression), cancellationToken);
		}

		/// <summary>
		/// Computes the sum of a sequence of <see cref="T:System.Int64"/> values.
		/// </summary>
		/// 
		/// <returns>
		/// The sum of the values in the sequence.
		/// </returns>
		/// <param name="source">A sequence of <see cref="T:System.Int64"/> values to calculate the sum of.</param>
		/// <param name="cancellationToken"></param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="source"/> is null.</exception>
		/// <exception cref="T:System.OverflowException">The sum is larger than <see cref="F:System.Int64.MaxValue"/>.</exception>
		public static Task<long> SumAsync(this IQueryable<long> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}
			var provider = (INhQueryProvider)source.Provider;
			var methodInfo = cachableQueryableMethods["SumLong"];
			var expression = new[] { source.Expression };
			return provider.ExecuteAsync<long>(Expression.Call(null, methodInfo, expression), cancellationToken);
		}

		/// <summary>
		/// Computes the sum of a sequence of nullable <see cref="T:System.Int64"/> values.
		/// </summary>
		/// 
		/// <returns>
		/// The sum of the values in the sequence.
		/// </returns>
		/// <param name="source">A sequence of nullable <see cref="T:System.Int64"/> values to calculate the sum of.</param>
		/// <param name="cancellationToken"></param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="source"/> is null.</exception>
		/// <exception cref="T:System.OverflowException">The sum is larger than <see cref="F:System.Int64.MaxValue"/>.</exception>
		public static Task<long?> SumAsync(this IQueryable<long?> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}
			var provider = (INhQueryProvider)source.Provider;
			var methodInfo = cachableQueryableMethods["SumLong?"];
			var expression = new[] { source.Expression };
			return provider.ExecuteAsync<long?>(Expression.Call(null, methodInfo, expression), cancellationToken);
		}

		/// <summary>
		/// Computes the sum of a sequence of <see cref="T:System.Single"/> values.
		/// </summary>
		/// 
		/// <returns>
		/// The sum of the values in the sequence.
		/// </returns>
		/// <param name="source">A sequence of <see cref="T:System.Single"/> values to calculate the sum of.</param>
		/// <param name="cancellationToken"></param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="source"/> is null.</exception>
		/// <exception cref="T:System.OverflowException">The sum is larger than <see cref="F:System.Single.MaxValue"/>.</exception>
		public static Task<float> SumAsync(this IQueryable<float> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}
			var provider = (INhQueryProvider)source.Provider;
			var methodInfo = cachableQueryableMethods["SumFloat"];
			var expression = new[] { source.Expression };
			return provider.ExecuteAsync<float>(Expression.Call(null, methodInfo, expression), cancellationToken);
		}

		/// <summary>
		/// Computes the sum of a sequence of nullable <see cref="T:System.Single"/> values.
		/// </summary>
		/// 
		/// <returns>
		/// The sum of the values in the sequence.
		/// </returns>
		/// <param name="source">A sequence of nullable <see cref="T:System.Single"/> values to calculate the sum of.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="source"/> is null.</exception>
		/// <exception cref="T:System.OverflowException">The sum is larger than <see cref="F:System.Single.MaxValue"/>.</exception>
		public static Task<float?> SumAsync(this IQueryable<float?> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}
			var provider = (INhQueryProvider)source.Provider;
			var methodInfo = cachableQueryableMethods["SumFloat?"];
			var expression = new[] { source.Expression };
			return provider.ExecuteAsync<float?>(Expression.Call(null, methodInfo, expression), cancellationToken);
		}

		/// <summary>
		/// Computes the sum of a sequence of <see cref="T:System.Double"/> values.
		/// </summary>
		/// 
		/// <returns>
		/// The sum of the values in the sequence.
		/// </returns>
		/// <param name="source">A sequence of <see cref="T:System.Double"/> values to calculate the sum of.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="source"/> is null.</exception>
		/// <exception cref="T:System.OverflowException">The sum is larger than <see cref="F:System.Double.MaxValue"/>.</exception>
		public static Task<double> SumAsync(this IQueryable<double> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}
			var provider = (INhQueryProvider)source.Provider;
			var methodInfo = cachableQueryableMethods["SumDouble"];
			var expression = new[] { source.Expression };
			return provider.ExecuteAsync<double>(Expression.Call(null, methodInfo, expression), cancellationToken);
		}

		/// <summary>
		/// Computes the sum of a sequence of nullable <see cref="T:System.Double"/> values.
		/// </summary>
		/// 
		/// <returns>
		/// The sum of the values in the sequence.
		/// </returns>
		/// <param name="source">A sequence of nullable <see cref="T:System.Double"/> values to calculate the sum of.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="source"/> is null.</exception>
		/// <exception cref="T:System.OverflowException">The sum is larger than <see cref="F:System.Double.MaxValue"/>.</exception>
		public static Task<double?> SumAsync(this IQueryable<double?> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}
			var provider = (INhQueryProvider)source.Provider;
			var methodInfo = cachableQueryableMethods["SumDouble?"];
			var expression = new[] { source.Expression };
			return provider.ExecuteAsync<double?>(Expression.Call(null, methodInfo, expression), cancellationToken);
		}

		/// <summary>
		/// Computes the sum of a sequence of <see cref="T:System.Decimal"/> values.
		/// </summary>
		/// 
		/// <returns>
		/// The sum of the values in the sequence.
		/// </returns>
		/// <param name="source">A sequence of <see cref="T:System.Decimal"/> values to calculate the sum of.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="source"/> is null.</exception>
		/// <exception cref="T:System.OverflowException">The sum is larger than <see cref="F:System.Decimal.MaxValue"/>.</exception>
		public static Task<decimal> SumAsync(this IQueryable<decimal> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}
			var provider = (INhQueryProvider)source.Provider;
			var methodInfo = cachableQueryableMethods["SumDecimal"];
			var expression = new[] { source.Expression };
			return provider.ExecuteAsync<decimal>(Expression.Call(null, methodInfo, expression), cancellationToken);
		}

		/// <summary>
		/// Computes the sum of a sequence of nullable <see cref="T:System.Decimal"/> values.
		/// </summary>
		/// 
		/// <returns>
		/// The sum of the values in the sequence.
		/// </returns>
		/// <param name="source">A sequence of nullable <see cref="T:System.Decimal"/> values to calculate the sum of.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="source"/> is null.</exception>
		/// <exception cref="T:System.OverflowException">The sum is larger than <see cref="F:System.Decimal.MaxValue"/>.</exception>
		public static Task<decimal?> SumAsync(this IQueryable<decimal?> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}
			var provider = (INhQueryProvider)source.Provider;
			var methodInfo = cachableQueryableMethods["SumDecimal?"];
			var expression = new[] { source.Expression };
			return provider.ExecuteAsync<decimal?>(Expression.Call(null, methodInfo, expression), cancellationToken);
		}

		/// <summary>
		/// Computes the sum of the sequence of <see cref="T:System.Int32"/> values that is obtained by invoking a projection function on each element of the input sequence.
		/// </summary>
		/// 
		/// <returns>
		/// The sum of the projected values.
		/// </returns>
		/// <param name="source">A sequence of values of type <paramref name="source"/>.</param>
		/// <param name="selector">A projection function to apply to each element.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is null.</exception>
		/// <exception cref="T:System.OverflowException">The sum is larger than <see cref="F:System.Int32.MaxValue"/>.</exception>
		public static Task<int> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, int>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}
			if (selector == null)
			{
				throw new ArgumentNullException(nameof(selector));
			}
			var provider = (INhQueryProvider)source.Provider;
			var methodInfo = cachableQueryableMethods["SumIntParam"].MakeGenericMethod(typeof(TSource));
			var expression = new[] { source.Expression, Expression.Quote(selector) };
			return provider.ExecuteAsync<int>(Expression.Call(null, methodInfo, expression), cancellationToken);
		}

		/// <summary>
		/// Computes the sum of the sequence of nullable <see cref="T:System.Int32"/> values that is obtained by invoking a projection function on each element of the input sequence.
		/// </summary>
		/// 
		/// <returns>
		/// The sum of the projected values.
		/// </returns>
		/// <param name="source">A sequence of values of type <paramref name="source"/>.</param>
		/// <param name="selector">A projection function to apply to each element.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is null.</exception>
		/// <exception cref="T:System.OverflowException">The sum is larger than <see cref="F:System.Int32.MaxValue"/>.</exception>
		public static Task<int?> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, int?>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}
			if (selector == null)
			{
				throw new ArgumentNullException(nameof(selector));
			}
			var provider = (INhQueryProvider)source.Provider;
			var methodInfo = cachableQueryableMethods["SumInt?Param"].MakeGenericMethod(typeof(TSource));
			var expression = new[] { source.Expression, Expression.Quote(selector) };
			return provider.ExecuteAsync<int?>(Expression.Call(null, methodInfo, expression), cancellationToken);
		}

		/// <summary>
		/// Computes the sum of the sequence of <see cref="T:System.Int64"/> values that is obtained by invoking a projection function on each element of the input sequence.
		/// </summary>
		/// 
		/// <returns>
		/// The sum of the projected values.
		/// </returns>
		/// <param name="source">A sequence of values of type <paramref name="source"/>.</param>
		/// <param name="selector">A projection function to apply to each element.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is null.</exception>
		/// <exception cref="T:System.OverflowException">The sum is larger than <see cref="F:System.Int64.MaxValue"/>.</exception>
		public static Task<long> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, long>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}
			if (selector == null)
			{
				throw new ArgumentNullException(nameof(selector));
			}
			var provider = (INhQueryProvider)source.Provider;
			var methodInfo = cachableQueryableMethods["SumLongParam"].MakeGenericMethod(typeof(TSource));
			var expression = new[] { source.Expression, Expression.Quote(selector) };
			return provider.ExecuteAsync<long>(Expression.Call(null, methodInfo, expression), cancellationToken);
		}

		/// <summary>
		/// Computes the sum of the sequence of nullable <see cref="T:System.Int64"/> values that is obtained by invoking a projection function on each element of the input sequence.
		/// </summary>
		/// 
		/// <returns>
		/// The sum of the projected values.
		/// </returns>
		/// <param name="source">A sequence of values of type <paramref name="source"/>.</param>
		/// <param name="selector">A projection function to apply to each element.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is null.</exception>
		/// <exception cref="T:System.OverflowException">The sum is larger than <see cref="F:System.Int64.MaxValue"/>.</exception>
		public static Task<long?> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, long?>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}
			if (selector == null)
			{
				throw new ArgumentNullException(nameof(selector));
			}
			var provider = (INhQueryProvider)source.Provider;
			var methodInfo = cachableQueryableMethods["SumLong?Param"].MakeGenericMethod(typeof(TSource));
			var expression = new[] { source.Expression, Expression.Quote(selector) };
			return provider.ExecuteAsync<long?>(Expression.Call(null, methodInfo, expression), cancellationToken);
		}

		/// <summary>
		/// Computes the sum of the sequence of <see cref="T:System.Single"/> values that is obtained by invoking a projection function on each element of the input sequence.
		/// </summary>
		/// 
		/// <returns>
		/// The sum of the projected values.
		/// </returns>
		/// <param name="source">A sequence of values of type <paramref name="source"/>.</param>
		/// <param name="selector">A projection function to apply to each element.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is null.</exception>
		/// <exception cref="T:System.OverflowException">The sum is larger than <see cref="F:System.Single.MaxValue"/>.</exception>
		public static Task<float> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, float>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}
			if (selector == null)
			{
				throw new ArgumentNullException(nameof(selector));
			}
			var provider = (INhQueryProvider)source.Provider;
			var methodInfo = cachableQueryableMethods["SumFloatParam"].MakeGenericMethod(typeof(TSource));
			var expression = new[] { source.Expression, Expression.Quote(selector) };
			return provider.ExecuteAsync<float>(Expression.Call(null, methodInfo, expression), cancellationToken);
		}

		/// <summary>
		/// Computes the sum of the sequence of nullable <see cref="T:System.Single"/> values that is obtained by invoking a projection function on each element of the input sequence.
		/// </summary>
		/// 
		/// <returns>
		/// The sum of the projected values.
		/// </returns>
		/// <param name="source">A sequence of values of type <paramref name="source"/>.</param>
		/// <param name="selector">A projection function to apply to each element.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is null.</exception>
		/// <exception cref="T:System.OverflowException">The sum is larger than <see cref="F:System.Single.MaxValue"/>.</exception>
		public static Task<float?> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, float?>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}
			if (selector == null)
			{
				throw new ArgumentNullException(nameof(selector));
			}
			var provider = (INhQueryProvider)source.Provider;
			var methodInfo = cachableQueryableMethods["SumFloat?Param"].MakeGenericMethod(typeof(TSource));
			var expression = new[] { source.Expression, Expression.Quote(selector) };
			return provider.ExecuteAsync<float?>(Expression.Call(null, methodInfo, expression), cancellationToken);
		}

		/// <summary>
		/// Computes the sum of the sequence of <see cref="T:System.Double"/> values that is obtained by invoking a projection function on each element of the input sequence.
		/// </summary>
		/// 
		/// <returns>
		/// The sum of the projected values.
		/// </returns>
		/// <param name="source">A sequence of values of type <paramref name="source"/>.</param>
		/// <param name="selector">A projection function to apply to each element.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is null.</exception>
		/// <exception cref="T:System.OverflowException">The sum is larger than <see cref="F:System.Double.MaxValue"/>.</exception>
		public static Task<double> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, double>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}
			if (selector == null)
			{
				throw new ArgumentNullException(nameof(selector));
			}
			var provider = (INhQueryProvider)source.Provider;
			var methodInfo = cachableQueryableMethods["SumDoubleParam"].MakeGenericMethod(typeof(TSource));
			var expression = new[] { source.Expression, Expression.Quote(selector) };
			return provider.ExecuteAsync<double>(Expression.Call(null, methodInfo, expression), cancellationToken);
		}

		/// <summary>
		/// Computes the sum of the sequence of nullable <see cref="T:System.Double"/> values that is obtained by invoking a projection function on each element of the input sequence.
		/// </summary>
		/// 
		/// <returns>
		/// The sum of the projected values.
		/// </returns>
		/// <param name="source">A sequence of values of type <paramref name="source"/>.</param>
		/// <param name="selector">A projection function to apply to each element.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is null.</exception>
		/// <exception cref="T:System.OverflowException">The sum is larger than <see cref="F:System.Double.MaxValue"/>.</exception>
		public static Task<double?> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, double?>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}
			if (selector == null)
			{
				throw new ArgumentNullException(nameof(selector));
			}
			var provider = (INhQueryProvider)source.Provider;
			var methodInfo = cachableQueryableMethods["SumDouble?Param"].MakeGenericMethod(typeof(TSource));
			var expression = new[] { source.Expression, Expression.Quote(selector) };
			return provider.ExecuteAsync<double?>(Expression.Call(null, methodInfo, expression), cancellationToken);
		}

		/// <summary>
		/// Computes the sum of the sequence of <see cref="T:System.Decimal"/> values that is obtained by invoking a projection function on each element of the input sequence.
		/// </summary>
		/// 
		/// <returns>
		/// The sum of the projected values.
		/// </returns>
		/// <param name="source">A sequence of values of type <paramref name="source"/>.</param>
		/// <param name="selector">A projection function to apply to each element.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is null.</exception>
		/// <exception cref="T:System.OverflowException">The sum is larger than <see cref="F:System.Decimal.MaxValue"/>.</exception>
		public static Task<decimal> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, decimal>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}
			if (selector == null)
			{
				throw new ArgumentNullException(nameof(selector));
			}
			var provider = (INhQueryProvider)source.Provider;
			var methodInfo = cachableQueryableMethods["SumDecimalParam"].MakeGenericMethod(typeof(TSource));
			var expression = new[] { source.Expression, Expression.Quote(selector) };
			return provider.ExecuteAsync<decimal>(Expression.Call(null, methodInfo, expression), cancellationToken);
		}

		/// <summary>
		/// Computes the sum of the sequence of nullable <see cref="T:System.Decimal"/> values that is obtained by invoking a projection function on each element of the input sequence.
		/// </summary>
		/// 
		/// <returns>
		/// The sum of the projected values.
		/// </returns>
		/// <param name="source">A sequence of values of type <paramref name="source"/>.</param>
		/// <param name="selector">A projection function to apply to each element.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is null.</exception>
		/// <exception cref="T:System.OverflowException">The sum is larger than <see cref="F:System.Decimal.MaxValue"/>.</exception>
		public static Task<decimal?> SumAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, decimal?>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}
			if (selector == null)
			{
				throw new ArgumentNullException(nameof(selector));
			}
			var provider = (INhQueryProvider)source.Provider;
			var methodInfo = cachableQueryableMethods["SumDecimal?Param"].MakeGenericMethod(typeof(TSource));
			var expression = new[] { source.Expression, Expression.Quote(selector) };
			return provider.ExecuteAsync<decimal?>(Expression.Call(null, methodInfo, expression), cancellationToken);
		}

		#endregion

		#region Average

		/// <summary>
		/// Computes the average of a sequence of <see cref="T:System.Int32"/> values.
		/// </summary>
		/// 
		/// <returns>
		/// The average of the sequence of values.
		/// </returns>
		/// <param name="source">A sequence of <see cref="T:System.Int32"/> values to calculate the average of.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="source"/> is null.</exception>
		/// <exception cref="T:System.InvalidOperationException"><paramref name="source"/> contains no elements.</exception>
		public static Task<double> AverageAsync(this IQueryable<int> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}
			var provider = (INhQueryProvider)source.Provider;
			var methodInfo = cachableQueryableMethods["AverageInt"];
			var expression = new[] { source.Expression };
			return provider.ExecuteAsync<double>(Expression.Call(null, methodInfo, expression), cancellationToken);
		}

		/// <summary>
		/// Computes the average of a sequence of nullable <see cref="T:System.Int32"/> values.
		/// </summary>
		/// 
		/// <returns>
		/// The average of the sequence of values, or null if the source sequence is empty or contains only null values.
		/// </returns>
		/// <param name="source">A sequence of nullable <see cref="T:System.Int32"/> values to calculate the average of.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="source"/> is null.</exception>
		public static Task<double?> AverageAsync(this IQueryable<int?> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}
			var provider = (INhQueryProvider)source.Provider;
			var methodInfo = cachableQueryableMethods["AverageInt?"];
			var expression = new[] { source.Expression };
			return provider.ExecuteAsync<double?>(Expression.Call(null, methodInfo, expression), cancellationToken);
		}

		/// <summary>
		/// Computes the average of a sequence of <see cref="T:System.Int64"/> values.
		/// </summary>
		/// 
		/// <returns>
		/// The average of the sequence of values.
		/// </returns>
		/// <param name="source">A sequence of <see cref="T:System.Int64"/> values to calculate the average of.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="source"/> is null.</exception>
		/// <exception cref="T:System.InvalidOperationException"><paramref name="source"/> contains no elements.</exception>
		public static Task<double> AverageAsync(this IQueryable<long> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}
			var provider = (INhQueryProvider)source.Provider;
			var methodInfo = cachableQueryableMethods["AverageLong"];
			var expression = new[] { source.Expression };
			return provider.ExecuteAsync<double>(Expression.Call(null, methodInfo, expression), cancellationToken);
		}

		/// <summary>
		/// Computes the average of a sequence of nullable <see cref="T:System.Int64"/> values.
		/// </summary>
		/// 
		/// <returns>
		/// The average of the sequence of values, or null if the source sequence is empty or contains only null values.
		/// </returns>
		/// <param name="source">A sequence of nullable <see cref="T:System.Int64"/> values to calculate the average of.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="source"/> is null.</exception>
		public static Task<double?> AverageAsync(this IQueryable<long?> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}
			var provider = (INhQueryProvider)source.Provider;
			var methodInfo = cachableQueryableMethods["AverageLong?"];
			var expression = new[] { source.Expression };
			return provider.ExecuteAsync<double?>(Expression.Call(null, methodInfo, expression), cancellationToken);
		}

		/// <summary>
		/// Computes the average of a sequence of <see cref="T:System.Single"/> values.
		/// </summary>
		/// 
		/// <returns>
		/// The average of the sequence of values.
		/// </returns>
		/// <param name="source">A sequence of <see cref="T:System.Single"/> values to calculate the average of.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="source"/> is null.</exception>
		/// <exception cref="T:System.InvalidOperationException"><paramref name="source"/> contains no elements.</exception>
		public static Task<float> AverageAsync(this IQueryable<float> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}
			var provider = (INhQueryProvider)source.Provider;
			var methodInfo = cachableQueryableMethods["AverageFloat"];
			var expression = new[] { source.Expression };
			return provider.ExecuteAsync<float>(Expression.Call(null, methodInfo, expression), cancellationToken);
		}

		/// <summary>
		/// Computes the average of a sequence of nullable <see cref="T:System.Single"/> values.
		/// </summary>
		/// 
		/// <returns>
		/// The average of the sequence of values, or null if the source sequence is empty or contains only null values.
		/// </returns>
		/// <param name="source">A sequence of nullable <see cref="T:System.Single"/> values to calculate the average of.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="source"/> is null.</exception>
		public static Task<float?> AverageAsync(this IQueryable<float?> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}
			var provider = (INhQueryProvider)source.Provider;
			var methodInfo = cachableQueryableMethods["AverageFloat?"];
			var expression = new[] { source.Expression };
			return provider.ExecuteAsync<float?>(Expression.Call(null, methodInfo, expression), cancellationToken);
		}

		/// <summary>
		/// Computes the average of a sequence of <see cref="T:System.Double"/> values.
		/// </summary>
		/// 
		/// <returns>
		/// The average of the sequence of values.
		/// </returns>
		/// <param name="source">A sequence of <see cref="T:System.Double"/> values to calculate the average of.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="source"/> is null.</exception>
		/// <exception cref="T:System.InvalidOperationException"><paramref name="source"/> contains no elements.</exception>
		public static Task<double> AverageAsync(this IQueryable<double> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}
			var provider = (INhQueryProvider)source.Provider;
			var methodInfo = cachableQueryableMethods["AverageDouble"];
			var expression = new[] { source.Expression };
			return provider.ExecuteAsync<double>(Expression.Call(null, methodInfo, expression), cancellationToken);
		}

		/// <summary>
		/// Computes the average of a sequence of nullable <see cref="T:System.Double"/> values.
		/// </summary>
		/// 
		/// <returns>
		/// The average of the sequence of values, or null if the source sequence is empty or contains only null values.
		/// </returns>
		/// <param name="source">A sequence of nullable <see cref="T:System.Double"/> values to calculate the average of.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="source"/> is null.</exception>
		public static Task<double?> AverageAsync(this IQueryable<double?> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}
			var provider = (INhQueryProvider)source.Provider;
			var methodInfo = cachableQueryableMethods["AverageDouble?"];
			var expression = new[] { source.Expression };
			return provider.ExecuteAsync<double?>(Expression.Call(null, methodInfo, expression), cancellationToken);
		}

		/// <summary>
		/// Computes the average of a sequence of <see cref="T:System.Decimal"/> values.
		/// </summary>
		/// 
		/// <returns>
		/// The average of the sequence of values.
		/// </returns>
		/// <param name="source">A sequence of <see cref="T:System.Decimal"/> values to calculate the average of.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="source"/> is null.</exception>
		/// <exception cref="T:System.InvalidOperationException"><paramref name="source"/> contains no elements.</exception>
		public static Task<decimal> AverageAsync(this IQueryable<decimal> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}
			var provider = (INhQueryProvider)source.Provider;
			var methodInfo = cachableQueryableMethods["AverageDecimal"];
			var expression = new[] { source.Expression };
			return provider.ExecuteAsync<decimal>(Expression.Call(null, methodInfo, expression), cancellationToken);
		}

		/// <summary>
		/// Computes the average of a sequence of nullable <see cref="T:System.Decimal"/> values.
		/// </summary>
		/// 
		/// <returns>
		/// The average of the sequence of values, or null if the source sequence is empty or contains only null values.
		/// </returns>
		/// <param name="source">A sequence of nullable <see cref="T:System.Decimal"/> values to calculate the average of.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="source"/> is null.</exception>
		public static Task<decimal?> AverageAsync(this IQueryable<decimal?> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}
			var provider = (INhQueryProvider)source.Provider;
			var methodInfo = cachableQueryableMethods["AverageDecimal?"];
			var expression = new[] { source.Expression };
			return provider.ExecuteAsync<decimal?>(Expression.Call(null, methodInfo, expression), cancellationToken);
		}

		/// <summary>
		/// Computes the average of a sequence of <see cref="T:System.Int32"/> values that is obtained by invoking a projection function on each element of the input sequence.
		/// </summary>
		/// 
		/// <returns>
		/// The average of the sequence of values.
		/// </returns>
		/// <param name="source">A sequence of values to calculate the average of.</param>
		/// <param name="selector">A projection function to apply to each element.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is null.</exception>
		/// <exception cref="T:System.InvalidOperationException"><paramref name="source"/> contains no elements.</exception>
		public static Task<double> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, int>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}
			if (selector == null)
			{
				throw new ArgumentNullException(nameof(selector));
			}
			var provider = (INhQueryProvider)source.Provider;
			var methodInfo = cachableQueryableMethods["AverageIntParam"].MakeGenericMethod(typeof(TSource));
			var expression = new[] { source.Expression, Expression.Quote(selector) };
			return provider.ExecuteAsync<double>(Expression.Call(null, methodInfo, expression), cancellationToken);
		}

		/// <summary>
		/// Computes the average of a sequence of nullable <see cref="T:System.Int32"/> values that is obtained by invoking a projection function on each element of the input sequence.
		/// </summary>
		/// 
		/// <returns>
		/// The average of the sequence of values, or null if the <paramref name="source"/> sequence is empty or contains only null values.
		/// </returns>
		/// <param name="source">A sequence of values to calculate the average of.</param>
		/// <param name="selector">A projection function to apply to each element.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is null.</exception>
		public static Task<double?> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, int?>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}
			if (selector == null)
			{
				throw new ArgumentNullException(nameof(selector));
			}
			var provider = (INhQueryProvider)source.Provider;
			var methodInfo = cachableQueryableMethods["AverageInt?Param"].MakeGenericMethod(typeof(TSource));
			var expression = new[] { source.Expression, Expression.Quote(selector) };
			return provider.ExecuteAsync<double?>(Expression.Call(null, methodInfo, expression), cancellationToken);
		}

		/// <summary>
		/// Computes the average of a sequence of <see cref="T:System.Int64"/> values that is obtained by invoking a projection function on each element of the input sequence.
		/// </summary>
		/// 
		/// <returns>
		/// The average of the sequence of values.
		/// </returns>
		/// <param name="source">A sequence of values to calculate the average of.</param>
		/// <param name="selector">A projection function to apply to each element.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is null.</exception>
		/// <exception cref="T:System.InvalidOperationException"><paramref name="source"/> contains no elements.</exception>
		public static Task<double> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, long>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}
			if (selector == null)
			{
				throw new ArgumentNullException(nameof(selector));
			}
			var provider = (INhQueryProvider)source.Provider;
			var methodInfo = cachableQueryableMethods["AverageLongParam"].MakeGenericMethod(typeof(TSource));
			var expression = new[] { source.Expression, Expression.Quote(selector) };
			return provider.ExecuteAsync<double>(Expression.Call(null, methodInfo, expression), cancellationToken);
		}

		/// <summary>
		/// Computes the average of a sequence of nullable <see cref="T:System.Int64"/> values that is obtained by invoking a projection function on each element of the input sequence.
		/// </summary>
		/// 
		/// <returns>
		/// The average of the sequence of values, or null if the <paramref name="source"/> sequence is empty or contains only null values.
		/// </returns>
		/// <param name="source">A sequence of values to calculate the average of.</param>
		/// <param name="selector">A projection function to apply to each element.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is null.</exception>
		public static Task<double?> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, long?>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}
			if (selector == null)
			{
				throw new ArgumentNullException(nameof(selector));
			}
			var provider = (INhQueryProvider)source.Provider;
			var methodInfo = cachableQueryableMethods["AverageLong?Param"].MakeGenericMethod(typeof(TSource));
			var expression = new[] { source.Expression, Expression.Quote(selector) };
			return provider.ExecuteAsync<double?>(Expression.Call(null, methodInfo, expression), cancellationToken);
		}

		/// <summary>
		/// Computes the average of a sequence of <see cref="T:System.Single"/> values that is obtained by invoking a projection function on each element of the input sequence.
		/// </summary>
		/// 
		/// <returns>
		/// The average of the sequence of values.
		/// </returns>
		/// <param name="source">A sequence of values to calculate the average of.</param>
		/// <param name="selector">A projection function to apply to each element.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is null.</exception>
		/// <exception cref="T:System.InvalidOperationException"><paramref name="source"/> contains no elements.</exception>
		public static Task<float> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, float>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}
			if (selector == null)
			{
				throw new ArgumentNullException(nameof(selector));
			}
			var provider = (INhQueryProvider)source.Provider;
			var methodInfo = cachableQueryableMethods["AverageFloatParam"].MakeGenericMethod(typeof(TSource));
			var expression = new[] { source.Expression, Expression.Quote(selector) };
			return provider.ExecuteAsync<float>(Expression.Call(null, methodInfo, expression), cancellationToken);
		}

		/// <summary>
		/// Computes the average of a sequence of nullable <see cref="T:System.Single"/> values that is obtained by invoking a projection function on each element of the input sequence.
		/// </summary>
		/// 
		/// <returns>
		/// The average of the sequence of values, or null if the <paramref name="source"/> sequence is empty or contains only null values.
		/// </returns>
		/// <param name="source">A sequence of values to calculate the average of.</param>
		/// <param name="selector">A projection function to apply to each element.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is null.</exception>
		public static Task<float?> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, float?>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}
			if (selector == null)
			{
				throw new ArgumentNullException(nameof(selector));
			}
			var provider = (INhQueryProvider)source.Provider;
			var methodInfo = cachableQueryableMethods["AverageFloat?Param"].MakeGenericMethod(typeof(TSource));
			var expression = new[] { source.Expression, Expression.Quote(selector) };
			return provider.ExecuteAsync<float?>(Expression.Call(null, methodInfo, expression), cancellationToken);
		}

		/// <summary>
		/// Computes the average of a sequence of <see cref="T:System.Double"/> values that is obtained by invoking a projection function on each element of the input sequence.
		/// </summary>
		/// 
		/// <returns>
		/// The average of the sequence of values.
		/// </returns>
		/// <param name="source">A sequence of values to calculate the average of.</param>
		/// <param name="selector">A projection function to apply to each element.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is null.</exception>
		/// <exception cref="T:System.InvalidOperationException"><paramref name="source"/> contains no elements.</exception>
		public static Task<double> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, double>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}
			if (selector == null)
			{
				throw new ArgumentNullException(nameof(selector));
			}
			var provider = (INhQueryProvider)source.Provider;
			var methodInfo = cachableQueryableMethods["AverageDoubleParam"].MakeGenericMethod(typeof(TSource));
			var expression = new[] { source.Expression, Expression.Quote(selector) };
			return provider.ExecuteAsync<double>(Expression.Call(null, methodInfo, expression), cancellationToken);
		}

		/// <summary>
		/// Computes the average of a sequence of nullable <see cref="T:System.Double"/> values that is obtained by invoking a projection function on each element of the input sequence.
		/// </summary>
		/// 
		/// <returns>
		/// The average of the sequence of values, or null if the <paramref name="source"/> sequence is empty or contains only null values.
		/// </returns>
		/// <param name="source">A sequence of values to calculate the average of.</param>
		/// <param name="selector">A projection function to apply to each element.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is null.</exception>
		public static Task<double?> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, double?>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}
			if (selector == null)
			{
				throw new ArgumentNullException(nameof(selector));
			}
			var provider = (INhQueryProvider)source.Provider;
			var methodInfo = cachableQueryableMethods["AverageDouble?Param"].MakeGenericMethod(typeof(TSource));
			var expression = new[] { source.Expression, Expression.Quote(selector) };
			return provider.ExecuteAsync<double?>(Expression.Call(null, methodInfo, expression), cancellationToken);
		}

		/// <summary>
		/// Computes the average of a sequence of <see cref="T:System.Decimal"/> values that is obtained by invoking a projection function on each element of the input sequence.
		/// </summary>
		/// 
		/// <returns>
		/// The average of the sequence of values.
		/// </returns>
		/// <param name="source">A sequence of values to calculate the average of.</param>
		/// <param name="selector">A projection function to apply to each element.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is null.</exception>
		/// <exception cref="T:System.InvalidOperationException"><paramref name="source"/> contains no elements.</exception>
		public static Task<decimal> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, decimal>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}
			if (selector == null)
			{
				throw new ArgumentNullException(nameof(selector));
			}
			var provider = (INhQueryProvider)source.Provider;
			var methodInfo = cachableQueryableMethods["AverageDecimalParam"].MakeGenericMethod(typeof(TSource));
			var expression = new[] { source.Expression, Expression.Quote(selector) };
			return provider.ExecuteAsync<decimal>(Expression.Call(null, methodInfo, expression), cancellationToken);
		}

		/// <summary>
		/// Computes the average of a sequence of nullable <see cref="T:System.Decimal"/> values that is obtained by invoking a projection function on each element of the input sequence.
		/// </summary>
		/// 
		/// <returns>
		/// The average of the sequence of values, or null if the <paramref name="source"/> sequence is empty or contains only null values.
		/// </returns>
		/// <param name="source">A sequence of values to calculate the average of.</param>
		/// <param name="selector">A projection function to apply to each element.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is null.</exception>
		public static Task<decimal?> AverageAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, decimal?>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}
			if (selector == null)
			{
				throw new ArgumentNullException(nameof(selector));
			}
			var provider = (INhQueryProvider)source.Provider;
			var methodInfo = cachableQueryableMethods["AverageDecimal?Param"].MakeGenericMethod(typeof(TSource));
			var expression = new[] { source.Expression, Expression.Quote(selector) };
			return provider.ExecuteAsync<decimal?>(Expression.Call(null, methodInfo, expression), cancellationToken);
		}

		#endregion

		#region MinAsync

		/// <summary>
		/// Returns the minimum value of a generic <see cref="T:System.Linq.IQueryable`1"/>.
		/// </summary>
		/// 
		/// <returns>
		/// The minimum value in the sequence.
		/// </returns>
		/// <param name="source">A sequence of values to determine the minimum of.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="source"/> is null.</exception>
		public static Task<TSource> MinAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}
			var provider = (INhQueryProvider)source.Provider;
			var methodInfo = cachableQueryableMethods["Min"].MakeGenericMethod(typeof(TSource));
			var expression = new[] { source.Expression };
			return provider.ExecuteAsync<TSource>(Expression.Call(null, methodInfo, expression), cancellationToken);
		}

		/// <summary>
		/// Invokes a projection function on each element of a generic <see cref="T:System.Linq.IQueryable`1"/> and returns the minimum resulting value.
		/// </summary>
		/// 
		/// <returns>
		/// The minimum value in the sequence.
		/// </returns>
		/// <param name="source">A sequence of values to determine the minimum of.</param>
		/// <param name="selector">A projection function to apply to each element.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
		/// <typeparam name="TResult">The type of the value returned by the function represented by <paramref name="selector"/>.</typeparam>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is null.</exception>
		public static Task<TResult> MinAsync<TSource, TResult>(this IQueryable<TSource> source, Expression<Func<TSource, TResult>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}
			if (selector == null)
			{
				throw new ArgumentNullException(nameof(selector));
			}
			var provider = (INhQueryProvider)source.Provider;
			var methodInfo = cachableQueryableMethods["MinParam"].MakeGenericMethod(typeof(TSource), typeof(TResult));
			var expression = new[] { source.Expression, Expression.Quote(selector) };
			return provider.ExecuteAsync<TResult>(Expression.Call(null, methodInfo, expression), cancellationToken);
		}

		#endregion

		#region MaxAsync

		/// <summary>
		/// Returns the maximum value in a generic <see cref="T:System.Linq.IQueryable`1"/>.
		/// </summary>
		/// 
		/// <returns>
		/// The maximum value in the sequence.
		/// </returns>
		/// <param name="source">A sequence of values to determine the maximum of.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="source"/> is null.</exception>
		public static Task<TSource> MaxAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}
			var provider = (INhQueryProvider)source.Provider;
			var methodInfo = cachableQueryableMethods["Max"].MakeGenericMethod(typeof(TSource));
			var expression = new[] { source.Expression };
			return provider.ExecuteAsync<TSource>(Expression.Call(null, methodInfo, expression), cancellationToken);
		}

		/// <summary>
		/// Invokes a projection function on each element of a generic <see cref="T:System.Linq.IQueryable`1"/> and returns the maximum resulting value.
		/// </summary>
		/// 
		/// <returns>
		/// The maximum value in the sequence.
		/// </returns>
		/// <param name="source">A sequence of values to determine the maximum of.</param>
		/// <param name="selector">A projection function to apply to each element.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
		/// <typeparam name="TResult">The type of the value returned by the function represented by <paramref name="selector"/>.</typeparam>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is null.</exception>
		public static Task<TResult> MaxAsync<TSource, TResult>(this IQueryable<TSource> source, Expression<Func<TSource, TResult>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}
			if (selector == null)
			{
				throw new ArgumentNullException(nameof(selector));
			}
			var provider = (INhQueryProvider)source.Provider;
			var methodInfo = cachableQueryableMethods["MaxParam"].MakeGenericMethod(typeof(TSource), typeof(TResult));
			var expression = new[] { source.Expression, Expression.Quote(selector) };
			return provider.ExecuteAsync<TResult>(Expression.Call(null, methodInfo, expression), cancellationToken);
		}

		#endregion

		#region LongCountAsync

		/// <summary>Returns the number of elements in a sequence.</summary>
		/// <returns>The number of elements in the input sequence.</returns>
		/// <param name="source">The <see cref="T:System.Linq.IQueryable`1" /> that contains the elements to be counted.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="source" /> is null.</exception>
		/// <exception cref="T:System.OverflowException">The number of elements in <paramref name="source" /> is larger than <see cref="F:System.Int64.MaxValue" />.</exception>
		public static Task<long> LongCountAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}
			var provider = (INhQueryProvider)source.Provider;
			var methodInfo = cachableQueryableMethods["LongCount"].MakeGenericMethod(typeof(TSource));
			var expression = new[] { source.Expression };
			return provider.ExecuteAsync<long>(Expression.Call(null, methodInfo, expression), cancellationToken);
		}

		/// <summary>Returns the number of elements in the specified sequence that satisfies a condition.</summary>
		/// <returns>The number of elements in the sequence that satisfies the condition in the predicate function.</returns>
		/// <param name="source">An <see cref="T:System.Linq.IQueryable`1" /> that contains the elements to be counted.</param>
		/// <param name="predicate">A function to test each element for a condition.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="source" /> or <paramref name="predicate" /> is null.</exception>
		/// <exception cref="T:System.OverflowException">The number of elements in <paramref name="source" /> is larger than <see cref="F:System.Int64.MaxValue" />.</exception>
		public static Task<long> LongCountAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}
			if (predicate == null)
			{
				throw new ArgumentNullException(nameof(predicate));
			}
			var provider = (INhQueryProvider)source.Provider;
			var methodInfo = cachableQueryableMethods["LongCountParam"].MakeGenericMethod(typeof(TSource));
			var expression = new[] { source.Expression, Expression.Quote(predicate) };
			return provider.ExecuteAsync<long>(Expression.Call(null, methodInfo, expression), cancellationToken);
		}

		#endregion

		#region FirstAsync

		/// <summary>Returns the first element of a sequence.</summary>
		/// <returns>The first element in <paramref name="source" />.</returns>
		/// <param name="source">The <see cref="T:System.Linq.IQueryable`1" /> to return the first element of.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="source" /> is null.</exception>
		/// <exception cref="T:System.InvalidOperationException">The source sequence is empty.</exception>
		public static Task<TSource> FirstAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}
			var provider = (INhQueryProvider)source.Provider;
			var methodInfo = cachableQueryableMethods["First"].MakeGenericMethod(typeof(TSource));
			var expression = new[] { source.Expression };
			return provider.ExecuteAsync<TSource>(Expression.Call(null, methodInfo, expression), cancellationToken);
		}

		/// <summary>Returns the first element of a sequence that satisfies a specified condition.</summary>
		/// <returns>The first element in <paramref name="source" /> that passes the test in <paramref name="predicate" />.</returns>
		/// <param name="source">An <see cref="T:System.Linq.IQueryable`1" /> to return an element from.</param>
		/// <param name="predicate">A function to test each element for a condition.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="source" /> or <paramref name="predicate" /> is null.</exception>
		/// <exception cref="T:System.InvalidOperationException">No element satisfies the condition in <paramref name="predicate" />.-or-The source sequence is empty.</exception>
		public static Task<TSource> FirstAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}
			if (predicate == null)
			{
				throw new ArgumentNullException(nameof(predicate));
			}
			var provider = (INhQueryProvider)source.Provider;
			var methodInfo = cachableQueryableMethods["FirstParam"].MakeGenericMethod(typeof(TSource));
			var expression = new[] { source.Expression, Expression.Quote(predicate) };
			return provider.ExecuteAsync<TSource>(Expression.Call(null, methodInfo, expression), cancellationToken);
		}

		#endregion

		#region SingleAsync

		/// <summary>Returns the only element of a sequence, and throws an exception if there is not exactly one element in the sequence.</summary>
		/// <returns>The single element in <paramref name="source" />.</returns>
		/// <param name="source">The <see cref="T:System.Linq.IQueryable`1" /> to return the first element of.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="source" /> is null.</exception>
		/// <exception cref="T:System.InvalidOperationException">The source sequence is empty.</exception>
		public static Task<TSource> SingleAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}
			var provider = (INhQueryProvider)source.Provider;
			var methodInfo = cachableQueryableMethods["Single"].MakeGenericMethod(typeof(TSource));
			var expression = new[] { source.Expression };
			return provider.ExecuteAsync<TSource>(Expression.Call(null, methodInfo, expression), cancellationToken);
		}

		/// <summary>Returns the only element of a sequence, and throws an exception if there is not exactly one element in the sequence.</summary>
		/// <returns>The single element in <paramref name="source" /> that passes the test in <paramref name="predicate" />.</returns>
		/// <param name="source">An <see cref="T:System.Linq.IQueryable`1" /> to return an element from.</param>
		/// <param name="predicate">A function to test each element for a condition.</param>
		/// <param name="cancellationToken"></param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="source" /> or <paramref name="predicate" /> is null.</exception>
		/// <exception cref="T:System.InvalidOperationException">No element satisfies the condition in <paramref name="predicate" />.-or-The source sequence is empty.</exception>
		public static Task<TSource> SingleAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}
			if (predicate == null)
			{
				throw new ArgumentNullException(nameof(predicate));
			}
			var provider = (INhQueryProvider)source.Provider;
			var methodInfo = cachableQueryableMethods["SingleParam"].MakeGenericMethod(typeof(TSource));
			var expression = new[] { source.Expression, Expression.Quote(predicate) };
			return provider.ExecuteAsync<TSource>(Expression.Call(null, methodInfo, expression), cancellationToken);
		}

		#endregion

		#region SingleOrDefaultAsync

		/// <summary>Returns the only element of a sequence, or a default value if the sequence is empty; this method throws an exception if there is more than one element in the sequence.</summary>
		/// <returns>default(<paramref name="source" />) if <paramref name="source" /> is empty; otherwise, the single element in <paramref name="source" />.</returns>
		/// <param name="source">The <see cref="T:System.Linq.IQueryable`1" /> to return the single element of.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="source" /> is null.</exception>
		public static Task<TSource> SingleOrDefaultAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}
			var provider = (INhQueryProvider)source.Provider;
			var methodInfo = cachableQueryableMethods["SingleOrDefault"].MakeGenericMethod(typeof(TSource));
			var expression = new[] { source.Expression };
			return provider.ExecuteAsync<TSource>(Expression.Call(null, methodInfo, expression), cancellationToken);
		}

		/// <summary>Returns the only element of a sequence, or a default value if the sequence is empty; this method throws an exception if there is more than one element in the sequence.</summary>
		/// <returns>default(<paramref name="source" />) if <paramref name="source" /> is empty or if no element passes the test specified by <paramref name="predicate" />; otherwise, the single element in <paramref name="source" /> that passes the test specified by <paramref name="predicate" />.</returns>
		/// <param name="source">An <see cref="T:System.Linq.IQueryable`1" /> to return an element from.</param>
		/// <param name="predicate">A function to test each element for a condition.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="source" /> or <paramref name="predicate" /> is null.</exception>
		public static Task<TSource> SingleOrDefaultAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}
			if (predicate == null)
			{
				throw new ArgumentNullException(nameof(predicate));
			}
			var provider = (INhQueryProvider)source.Provider;
			var methodInfo = cachableQueryableMethods["SingleOrDefaultParam"].MakeGenericMethod(typeof(TSource));
			var expression = new[] { source.Expression, Expression.Quote(predicate) };
			return provider.ExecuteAsync<TSource>(Expression.Call(null, methodInfo, expression), cancellationToken);
		}

		#endregion

		#region FirstOrDefaultAsync

		/// <summary>Returns the first element of a sequence, or a default value if the sequence contains no elements.</summary>
		/// <returns>default(<paramref name="source" />) if <paramref name="source" /> is empty; otherwise, the first element in <paramref name="source" />.</returns>
		/// <param name="source">The <see cref="T:System.Linq.IQueryable`1" /> to return the first element of.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="source" /> is null.</exception>
		public static Task<TSource> FirstOrDefaultAsync<TSource>(this IQueryable<TSource> source, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}
			var provider = (INhQueryProvider)source.Provider;
			var methodInfo = cachableQueryableMethods["FirstOrDefault"].MakeGenericMethod(typeof(TSource));
			var expression = new[] { source.Expression };
			return provider.ExecuteAsync<TSource>(Expression.Call(null, methodInfo, expression), cancellationToken);
		}

		/// <summary>Returns the first element of a sequence that satisfies a specified condition or a default value if no such element is found.</summary>
		/// <returns>default(<paramref name="source" />) if <paramref name="source" /> is empty or if no element passes the test specified by <paramref name="predicate" />; otherwise, the first element in <paramref name="source" /> that passes the test specified by <paramref name="predicate" />.</returns>
		/// <param name="source">An <see cref="T:System.Linq.IQueryable`1" /> to return an element from.</param>
		/// <param name="predicate">A function to test each element for a condition.</param>
		/// <param name="cancellationToken"></param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="source" /> or <paramref name="predicate" /> is null.</exception>
		public static Task<TSource> FirstOrDefaultAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}
			if (predicate == null)
			{
				throw new ArgumentNullException(nameof(predicate));
			}
			var provider = (INhQueryProvider)source.Provider;
			var methodInfo = cachableQueryableMethods["FirstOrDefaultParam"].MakeGenericMethod(typeof(TSource));
			var expression = new[] { source.Expression, Expression.Quote(predicate) };
			return provider.ExecuteAsync<TSource>(Expression.Call(null, methodInfo, expression), cancellationToken);
		}

		#endregion

		public static async Task<List<TSource>> ToListAsync<TSource>(this IQueryable<TSource> query, CancellationToken cancellationToken = default(CancellationToken))
		{
			var nhQueryable = query as QueryableBase<TSource>;
			if (nhQueryable == null)
				throw new NotSupportedException("Query needs to be of type QueryableBase<TSource>");

			var provider = (INhQueryProvider)nhQueryable.Provider;
			var result = await provider.ExecuteAsync<IEnumerable<TSource>>(nhQueryable.Expression, cancellationToken);
			return result.ToList();
		}

		public static IAsyncEnumerable<TSource> ToFutureAsync<TSource>(this IQueryable<TSource> query, CancellationToken cancellationToken = default(CancellationToken))
		{
			var nhQueryable = query as QueryableBase<TSource>;
			if (nhQueryable == null)
				throw new NotSupportedException("Query needs to be of type QueryableBase<TSource>");

			var provider = (INhQueryProvider)nhQueryable.Provider;
			return (IAsyncEnumerable<TSource>)provider.ExecuteFutureAsync(nhQueryable.Expression, cancellationToken);
		}

		public static IFutureValueAsync<TSource> ToFutureValueAsync<TSource>(this IQueryable<TSource> query, CancellationToken cancellationToken = default(CancellationToken))
		{
			var nhQueryable = query as QueryableBase<TSource>;
			if (nhQueryable == null)
				throw new NotSupportedException("Query needs to be of type QueryableBase<TSource>");

			var provider = (INhQueryProvider)nhQueryable.Provider;
			var future = provider.ExecuteFutureAsync(nhQueryable.Expression, cancellationToken);
			if (future is IAsyncEnumerable<TSource> asyncEnumerable)
			{
				return new FutureValueAsync<TSource>(async () => await asyncEnumerable.ToList(cancellationToken));
			}

			return (FutureValueAsync<TSource>)future;
		}

		public static IFutureValueAsync<TResult> ToFutureValueAsync<TSource, TResult>(this IQueryable<TSource> query, Expression<Func<IQueryable<TSource>, TResult>> selector, CancellationToken cancellationToken = default(CancellationToken))
		{
			var nhQueryable = query as QueryableBase<TSource>;
			if (nhQueryable == null)
				throw new NotSupportedException("Query needs to be of type QueryableBase<TSource>");

			var provider = (INhQueryProvider)query.Provider;
			var expression = ReplacingExpressionTreeVisitor.Replace(selector.Parameters.Single(),
																	query.Expression,
																	selector.Body);

			return (IFutureValueAsync<TResult>)provider.ExecuteFutureAsync(expression, cancellationToken);
		}

		public static IQueryable<T> Query<T>(this ISession session)
		{
			return new NhQueryable<T>(session.GetSessionImplementation());
		}

		public static IQueryable<T> Query<T>(this ISession session, string entityName)
		{
			return new NhQueryable<T>(session.GetSessionImplementation(), entityName);
		}

		public static IQueryable<T> Query<T>(this IStatelessSession session)
		{
			return new NhQueryable<T>(session.GetSessionImplementation());
		}

		public static IQueryable<T> Query<T>(this IStatelessSession session, string entityName)
		{
			return new NhQueryable<T>(session.GetSessionImplementation(), entityName);
		}

		private static readonly MethodInfo CacheableDefinition = ReflectHelper.GetMethodDefinition(() => Cacheable<object>(null));

		public static IQueryable<T> Cacheable<T>(this IQueryable<T> query)
		{
			var method = CacheableDefinition.MakeGenericMethod(typeof(T));

			var callExpression = Expression.Call(method, query.Expression);

			return new NhQueryable<T>(query.Provider, callExpression);
		}

		private static readonly MethodInfo CacheModeDefinition = ReflectHelper.GetMethodDefinition(() => CacheMode<object>(null, NHibernate.CacheMode.Normal));

		public static IQueryable<T> CacheMode<T>(this IQueryable<T> query, CacheMode cacheMode)
		{
			var method = CacheModeDefinition.MakeGenericMethod(typeof(T));

			var callExpression = Expression.Call(method, query.Expression, Expression.Constant(cacheMode));

			return new NhQueryable<T>(query.Provider, callExpression);
		}

		private static readonly MethodInfo CacheRegionDefinition = ReflectHelper.GetMethodDefinition(() => CacheRegion<object>(null, null));

		public static IQueryable<T> CacheRegion<T>(this IQueryable<T> query, string region)
		{
			var method = CacheRegionDefinition.MakeGenericMethod(typeof(T));

			var callExpression = Expression.Call(method, query.Expression, Expression.Constant(region));

			return new NhQueryable<T>(query.Provider, callExpression);
		}

		private static readonly MethodInfo TimeoutDefinition = ReflectHelper.GetMethodDefinition(() => Timeout<object>(null, 0));

		public static IQueryable<T> Timeout<T>(this IQueryable<T> query, int timeout)
		{
			var method = TimeoutDefinition.MakeGenericMethod(typeof(T));

			var callExpression = Expression.Call(method, query.Expression, Expression.Constant(timeout));

			return new NhQueryable<T>(query.Provider, callExpression);
		}

		public static IEnumerable<T> ToFuture<T>(this IQueryable<T> query)
		{
			var nhQueryable = query as QueryableBase<T>;
			if (nhQueryable == null)
				throw new NotSupportedException("Query needs to be of type QueryableBase<T>");

			var provider = (INhQueryProvider) nhQueryable.Provider;
			var future = provider.ExecuteFuture(nhQueryable.Expression);
			return (IEnumerable<T>) future;
		}

		public static IFutureValue<T> ToFutureValue<T>(this IQueryable<T> query)
		{
			var nhQueryable = query as QueryableBase<T>;
			if (nhQueryable == null)
				throw new NotSupportedException("Query needs to be of type QueryableBase<T>");

			var provider = (INhQueryProvider) nhQueryable.Provider;
			var future = provider.ExecuteFuture(nhQueryable.Expression);
			if (future is IEnumerable<T>)
			{
				return new FutureValue<T>(() => ((IEnumerable<T>) future));
			}

			return (IFutureValue<T>) future;
		}

		public static T MappedAs<T>(this T parameter, IType type)
		{
			throw new InvalidOperationException("The method should be used inside Linq to indicate a type of a parameter");
		}

		public static IFutureValue<TResult> ToFutureValue<T, TResult>(this IQueryable<T> query, Expression<Func<IQueryable<T>, TResult>> selector)
		{
			var nhQueryable = query as QueryableBase<T>;
			if (nhQueryable == null)
				throw new NotSupportedException("Query needs to be of type QueryableBase<T>");

			var provider = (INhQueryProvider) query.Provider;

			var expression = ReplacingExpressionTreeVisitor.Replace(selector.Parameters.Single(),
																	query.Expression,
																	selector.Body);

			return (IFutureValue<TResult>) provider.ExecuteFuture(expression);
		}
	}
}
