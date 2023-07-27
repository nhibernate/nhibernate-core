using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NHibernate.Util
{
	//TODO 6.0: Make internal
	public static class EnumerableExtensions
	{
		//Since v5.1
		[Obsolete("Please use Enumerable.Any<T>(IEnumerable<T>) instead.")]
		public static bool Any(this IEnumerable source)
		{
			return Enumerable.Any(source.Cast<object>());
		}

		//Since v5.1
		[Obsolete("Please use Enumerable.First<T>(IEnumerable<T>) instead.")]
		public static object First(this IEnumerable source)
		{
			return Enumerable.First(source.Cast<object>());
		}

		//Since v5.1
		[Obsolete("Please use Enumerable.FirstOrDefault<T>(IEnumerable<T>) instead.")]
		public static object FirstOrNull(this IEnumerable source)
		{
			return Enumerable.FirstOrDefault(source.Cast<object>());
		}

		//Since v5.1
		[Obsolete("Please use a loop instead.")]
		public static void ForEach<T>(this IEnumerable<T> query, Action<T> method)
		{
			foreach (var item in query)
			{
				method(item);
			}
		}

		internal static TOutput[] ToArray<TInput, TOutput>(this ICollection<TInput> input, Func<TInput, TOutput> converter)
		{
			var results = new TOutput[input.Count];

			int i = 0;
			foreach (var value in input)
			{
				results[i++] = converter(value);
			}

			return results;
		}

		internal static TOutput[] ToArray<TInput, TOutput>(this List<TInput> input, Func<TInput, TOutput> converter)
		{
			var results = new TOutput[input.Count];

			for (var i = 0; i < input.Count; i++)
			{
				results[i] = converter(input[i]);
			}

			return results;
		}

		internal static TOutput[] ToArray<TInput, TOutput>(this TInput[] input, Converter<TInput, TOutput> converter)
		{
			return Array.ConvertAll(input, converter);
		}

		internal static List<TOutput> ToList<TInput, TOutput>(this ICollection<TInput> input, Func<TInput, TOutput> converter)
		{
			var results = new List<TOutput>(input.Count);

			foreach (var value in input)
			{
				results.Add(converter(value));
			}

			return results;
		}

		internal static List<TOutput> ToList<TInput, TOutput>(this TInput[] input, Func<TInput, TOutput> converter)
		{
			var results = new List<TOutput>(input.Length);

			foreach (var value in input)
			{
				results.Add(converter(value));
			}

			return results;
		}

		internal static List<TOutput> ToList<TInput, TOutput>(this List<TInput> input, Converter<TInput, TOutput> converter)
		{
			return input.ConvertAll(converter);
		}

		internal static IList ToIList<T>(this IEnumerable<T> list)
		{
			return list as IList ?? list.ToList();
		}
		
		internal static IReadOnlyList<T> EmptyIfNull<T>(this IReadOnlyList<T> list)
		{
			return list ?? Array.Empty<T>();
		}

		internal static IEnumerable<T> CastOrDefault<T>(this IEnumerable list)
		{
			foreach (var obj in list)
			{
				yield return obj == null
					? default(T)
					: (T) obj;
			}
		}

#if NETFX && !NET471_OR_GREATER
		internal static IEnumerable<TSource> Append<TSource>(this IEnumerable<TSource> source, TSource element)
		{
			foreach (var item in source)
				yield return item;
			yield return element;
		}
#endif
	}
}
